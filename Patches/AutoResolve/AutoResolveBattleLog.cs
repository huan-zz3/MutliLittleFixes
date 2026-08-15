using System;
using System.IO;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using EngineUtilities = TaleWorlds.Engine.Utilities;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 玩家坐镇过程全量数据日志（CSV）。
    ///
    /// 记录内容（每场玩家坐镇战斗一个时间戳文件夹，UTF-8 BOM，Excel/WPS 可直接打开）：
    ///   battle_summary.csv  战斗总览 1 行：真实/游戏时间、战斗类型、攻防领主与派系、玩家阵营、
    ///                       初始兵力与战力、兵种构成（步/射/骑/骑射）、胜方、结束原因、剩余兵力、
    ///                       伤亡汇总、总轮数/tick 数、MCM 参数快照
    ///   round_log.csv       每轮 1 行：轮次、双方 ticks、轮前兵力/士气、本轮双方输出伤害/命中/格挡/未命中、
    ///                       本轮伤亡、轮后兵力/士气、轮胜者
    ///   tick_log.csv        每次单兵对抗 1 行：攻方兵种/类型/武器/武器伤害/伤害类型，
    ///                       守方兵种/类型/持盾/命中部位护甲，结果（命中/格挡/未命中），原始伤害 → 最终伤害
    ///   casualty_log.csv    每个伤亡事件 1 行：轮次/tick、阵营、兵种、类型、事件（伤/亡/溃逃）
    ///
    /// 关键约束（零随机污染）：日志绝不调用 MBRandom / SelectWeapon / GetArmorInRandomPart，
    /// 所有伤害/武器/护甲数据由 AutoResolveDamagePatch 在计算后传入，避免消耗随机数改变战斗结果。
    /// 伤亡事件挂 MapEventSide.OnTroopWounded/Killed/Routed 的 Postfix（public，重平衡与原版路径全覆盖）。
    ///
    /// 门控：EnableAutoResolveBattleLog == true 且 mapEvent.IsPlayerSimulation（仅玩家坐镇）。
    /// </summary>
    internal static class AutoResolveBattleLog
    {
        /// <summary>当前活跃会话（玩家坐镇同一时间只有一场）。</summary>
        private static BattleLogSession _session;

        private const string LogRootName = "MutliLittleFixes_AutoResolveLogs";

        // ── 会话状态 ──────────────────────────────────────────────

        private sealed class BattleLogSession
        {
            public MapEvent MapEvent;
            public string FolderPath;
            public StringBuilder SummarySb;
            public StringBuilder RoundSb;
            public StringBuilder TickSb;
            public StringBuilder CasualtySb;

            public int RoundNumber;
            public int TickNumber;
            public int LastTickDamage; // 最近一次对抗的最终伤害，供伤亡事件关联
            public BattleSideEnum LastTickSide; // 最近一次对抗的攻击方

            // 战斗开始快照
            public int AttackerInitialTroops;
            public int DefenderInitialTroops;
            public float AttackerPower;
            public float DefenderPower;
            public int[] AttackerComposition; // [步兵, 射手, 骑手, 骑射手]
            public int[] DefenderComposition;

            // 当前轮累计
            public int CurAttackerTicks;
            public int CurDefenderTicks;
            public int CurAttackerTroopsBefore;
            public int CurDefenderTroopsBefore;
            public float CurAttackerMoraleBefore;
            public float CurDefenderMoraleBefore;
            public float CurAttackerDamage;
            public float CurDefenderDamage;
            public int CurAttackerHits;
            public int CurDefenderHits;
            public int CurAttackerBlocks;
            public int CurDefenderBlocks;
            public int CurAttackerMisses;
            public int CurDefenderMisses;
            public int CurAttackerWounded;
            public int CurDefenderWounded;
            public int CurAttackerKilled;
            public int CurDefenderKilled;
            public int CurAttackerRouted;
            public int CurDefenderRouted;

            // 战斗汇总
            public int TotalAttackerWounded, TotalDefenderWounded;
            public int TotalAttackerKilled, TotalDefenderKilled;
            public int TotalAttackerRouted, TotalDefenderRouted;
            public bool Finished;

            public BattleLogSession(MapEvent mapEvent)
            {
                MapEvent = mapEvent;
                SummarySb = new StringBuilder(512);
                RoundSb = new StringBuilder(512);
                TickSb = new StringBuilder(8192);
                CasualtySb = new StringBuilder(2048);
                AttackerComposition = new int[4];
                DefenderComposition = new int[4];
            }
        }

        // ── 对外 API（全部自带门控） ─────────────────────────────

        /// <summary>战斗开始：确保会话存在并写入战斗快照数据（首次轮次开始时调用）。</summary>
        internal static void EnsureSession(MapEvent mapEvent)
        {
            if (!IsLogEnabled(mapEvent))
            {
                return;
            }
            if (_session != null && _session.MapEvent != mapEvent)
            {
                // 防御：上一场未正常结束，先收尾
                FinishSession(_session.MapEvent);
            }
            if (_session != null)
            {
                return;
            }

            _session = new BattleLogSession(mapEvent);
            try
            {
                _session.FolderPath = Path.Combine(EngineUtilities.GetBasePath(), LogRootName,
                    DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(_session.FolderPath);

                WriteHeader(_session.FolderPath, "battle_summary.csv", BuildSummaryHeader());
                WriteHeader(_session.FolderPath, "round_log.csv", BuildRoundHeader());
                WriteHeader(_session.FolderPath, "tick_log.csv", BuildTickHeader());
                WriteHeader(_session.FolderPath, "casualty_log.csv", BuildCasualtyHeader());

                CaptureBattleSnapshot(mapEvent);
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇日志] 初始化失败: " + ex);
            }
        }

        /// <summary>轮次开始（SimulateBattleRound Prefix）：记录轮前兵力/士气与 ticks。</summary>
        internal static void RecordRoundStart(MapEvent mapEvent, int simulationTicksDefender, int simulationTicksAttacker)
        {
            EnsureSession(mapEvent);
            BattleLogSession s = _session;
            if (s == null)
            {
                return;
            }
            s.RoundNumber++;
            s.CurAttackerTicks = simulationTicksAttacker;
            s.CurDefenderTicks = simulationTicksDefender;
            s.CurAttackerTroopsBefore = mapEvent.AttackerSide.NumRemainingSimulationTroops;
            s.CurDefenderTroopsBefore = mapEvent.DefenderSide.NumRemainingSimulationTroops;
            s.CurAttackerMoraleBefore = mapEvent.AttackerSide.GetSideMorale();
            s.CurDefenderMoraleBefore = mapEvent.DefenderSide.GetSideMorale();
            // 重置本轮统计
            s.CurAttackerDamage = s.CurDefenderDamage = 0f;
            s.CurAttackerHits = s.CurDefenderHits = 0;
            s.CurAttackerBlocks = s.CurDefenderBlocks = 0;
            s.CurAttackerMisses = s.CurDefenderMisses = 0;
            s.CurAttackerWounded = s.CurDefenderWounded = 0;
            s.CurAttackerKilled = s.CurDefenderKilled = 0;
            s.CurAttackerRouted = s.CurDefenderRouted = 0;
        }

        /// <summary>轮次结束（SimulateBattleRound Postfix）：写本轮行 + flush 本轮明细，战斗结束则收尾。</summary>
        internal static void RecordRoundEnd(MapEvent mapEvent)
        {
            BattleLogSession s = _session;
            if (s == null || s.MapEvent != mapEvent)
            {
                return;
            }
            try
            {
                int attackerAfter = mapEvent.AttackerSide.NumRemainingSimulationTroops;
                int defenderAfter = mapEvent.DefenderSide.NumRemainingSimulationTroops;
                float attackerMoraleAfter = mapEvent.AttackerSide.GetSideMorale();
                float defenderMoraleAfter = mapEvent.DefenderSide.GetSideMorale();
                string roundWinner = mapEvent.WonRounds.Count > 0 ? mapEvent.WonRounds[mapEvent.WonRounds.Count - 1].ToString() : "None";

                s.RoundSb.Append(s.RoundNumber).Append(',')
                    .Append(s.CurAttackerTicks).Append(',').Append(s.CurDefenderTicks).Append(',')
                    .Append(s.CurAttackerTroopsBefore).Append(',').Append(s.CurDefenderTroopsBefore).Append(',')
                    .Append(F(s.CurAttackerMoraleBefore)).Append(',').Append(F(s.CurDefenderMoraleBefore)).Append(',')
                    .Append(F(s.CurAttackerDamage)).Append(',').Append(F(s.CurDefenderDamage)).Append(',')
                    .Append(s.CurAttackerHits).Append(',').Append(s.CurDefenderHits).Append(',')
                    .Append(s.CurAttackerBlocks).Append(',').Append(s.CurDefenderBlocks).Append(',')
                    .Append(s.CurAttackerMisses).Append(',').Append(s.CurDefenderMisses).Append(',')
                    .Append(s.CurAttackerWounded).Append(',').Append(s.CurDefenderWounded).Append(',')
                    .Append(s.CurAttackerKilled).Append(',').Append(s.CurDefenderKilled).Append(',')
                    .Append(s.CurAttackerRouted).Append(',').Append(s.CurDefenderRouted).Append(',')
                    .Append(attackerAfter).Append(',').Append(defenderAfter).Append(',')
                    .Append(F(attackerMoraleAfter)).Append(',').Append(F(defenderMoraleAfter)).Append(',')
                    .Append(roundWinner).Append('\n');

                Flush(s.RoundSb, "round_log.csv");
                Flush(s.TickSb, "tick_log.csv");
                Flush(s.CasualtySb, "casualty_log.csv");

                // 战斗结束（任意一方获胜 / 拉回）：收尾
                if (mapEvent.BattleState != BattleState.None)
                {
                    FinishSession(mapEvent);
                }
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇日志] 轮次记录异常: " + ex);
            }
        }

        /// <summary>记录一次单兵对抗（AutoResolveDamagePatch 计算完成后调用，数据为实际结算值）。</summary>
        internal static void RecordHit(CharacterObject strikerTroop, CharacterObject struckTroop, PartyBase strikerParty,
            MapEvent mapEvent, float originalDamage, AutoResolveSimulateModel.WeaponSelection selection, float armor,
            bool blocked, bool missed, float finalDamage)
        {
            if (!IsLogEnabled(mapEvent))
            {
                return;
            }
            BattleLogSession s = _session;
            if (s == null || s.MapEvent != mapEvent)
            {
                return;
            }
            s.TickNumber++;
            s.LastTickDamage = (int)finalDamage;
            s.LastTickSide = strikerParty?.Side ?? BattleSideEnum.None;

            bool isAttacker = s.LastTickSide == BattleSideEnum.Attacker;
            // 本轮统计累计
            if (blocked)
            {
                if (isAttacker) { s.CurAttackerBlocks++; s.CurAttackerHits++; } else { s.CurDefenderBlocks++; s.CurDefenderHits++; }
            }
            else if (missed)
            {
                if (isAttacker) { s.CurAttackerMisses++; s.CurAttackerHits++; } else { s.CurDefenderMisses++; s.CurDefenderHits++; }
            }
            else
            {
                if (isAttacker) { s.CurAttackerHits++; s.CurAttackerDamage += finalDamage; } else { s.CurDefenderHits++; s.CurDefenderDamage += finalDamage; }
            }

            s.TickSb.Append(s.RoundNumber).Append(',').Append(s.TickNumber).Append(',')
                .Append(s.LastTickSide).Append(',')
                .Append(Esc(strikerTroop.Name?.ToString() ?? "?")).Append(',')
                .Append(GetTroopType(strikerTroop)).Append(',')
                .Append(Esc(selection.WeaponName)).Append(',')
                .Append(selection.Damage).Append(',')
                .Append(selection.DamageType).Append(',')
                .Append(Esc(struckTroop.Name?.ToString() ?? "?")).Append(',')
                .Append(GetTroopType(struckTroop)).Append(',')
                .Append(AutoResolveSimulateModel.HasShield(struckTroop) ? 1 : 0).Append(',')
                .Append(F(armor)).Append(',')
                .Append(blocked ? "blocked" : (missed ? "missed" : "hit")).Append(',')
                .Append(F(originalDamage)).Append(',').Append(F(finalDamage)).Append('\n');
        }

        /// <summary>记录伤亡事件（OnTroopWounded/Killed/Routed Postfix 调用）。</summary>
        internal static void RecordCasualty(MapEventSide side, UniqueTroopDescriptor troopDesc, string eventName)
        {
            MapEvent mapEvent = side.MapEvent;
            if (!IsLogEnabled(mapEvent))
            {
                return;
            }
            BattleLogSession s = _session;
            if (s == null || s.MapEvent != mapEvent)
            {
                return;
            }
            CharacterObject troop = side.GetAllocatedTroop(troopDesc);
            bool isAttacker = side.MissionSide == BattleSideEnum.Attacker;

            // 汇总 + 本轮累计
            if (eventName == "wounded") { if (isAttacker) { s.CurAttackerWounded++; s.TotalAttackerWounded++; } else { s.CurDefenderWounded++; s.TotalDefenderWounded++; } }
            else if (eventName == "killed") { if (isAttacker) { s.CurAttackerKilled++; s.TotalAttackerKilled++; } else { s.CurDefenderKilled++; s.TotalDefenderKilled++; } }
            else if (eventName == "routed") { if (isAttacker) { s.CurAttackerRouted++; s.TotalAttackerRouted++; } else { s.CurDefenderRouted++; s.TotalDefenderRouted++; } }

            s.CasualtySb.Append(s.RoundNumber).Append(',').Append(s.TickNumber).Append(',')
                .Append(side.MissionSide).Append(',')
                .Append(Esc(troop?.Name?.ToString() ?? "?")).Append(',')
                .Append(troop != null ? GetTroopType(troop) : "?").Append(',')
                .Append(eventName).Append(',')
                .Append(s.LastTickDamage).Append('\n');
        }

        /// <summary>战斗收尾：写 summary + flush 全部 + 清会话（幂等）。</summary>
        internal static void FinishSession(MapEvent mapEvent)
        {
            BattleLogSession s = _session;
            if (s == null || s.MapEvent != mapEvent || s.Finished)
            {
                return;
            }
            s.Finished = true;
            try
            {
                s.SummarySb.Append(BuildSummaryRow(s));
                Flush(s.SummarySb, "battle_summary.csv");
                Flush(s.RoundSb, "round_log.csv");
                Flush(s.TickSb, "tick_log.csv");
                Flush(s.CasualtySb, "casualty_log.csv");
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇日志] 收尾写入异常: " + ex);
            }
            _session = null;
        }

        // ── 战斗快照采集 ─────────────────────────────────────────

        private static void CaptureBattleSnapshot(MapEvent mapEvent)
        {
            BattleLogSession s = _session;
            s.AttackerInitialTroops = mapEvent.AttackerSide.NumRemainingSimulationTroops;
            s.DefenderInitialTroops = mapEvent.DefenderSide.NumRemainingSimulationTroops;
            s.AttackerPower = ComputeSidePower(mapEvent, mapEvent.AttackerSide, BattleSideEnum.Attacker, s.AttackerComposition);
            s.DefenderPower = ComputeSidePower(mapEvent, mapEvent.DefenderSide, BattleSideEnum.Defender, s.DefenderComposition);
        }

        /// <summary>遍历参战部队 roster 统计战力与兵种构成（步兵/射手/骑手/骑射手）。</summary>
        private static float ComputeSidePower(MapEvent mapEvent, MapEventSide side, BattleSideEnum battleSide, int[] composition)
        {
            float power = 0f;
            foreach (PartyBase party in mapEvent.InvolvedParties)
            {
                if (party.Side != battleSide)
                {
                    continue;
                }
                TroopRoster roster = party.MemberRoster;
                for (int i = 0; i < roster.Count; i++)
                {
                    TroopRosterElement element = roster.GetElementCopyAtIndex(i);
                    CharacterObject troop = element.Character;
                    if (troop == null)
                    {
                        continue;
                    }
                    power += Campaign.Current.Models.MilitaryPowerModel.GetTroopPower(
                        troop, battleSide, MapEvent.PowerCalculationContext.PlainBattle, 1f) * element.Number;
                    composition[GetTroopTypeIndex(troop)] += element.Number;
                }
            }
            return power;
        }

        // ── CSV 行构造 ───────────────────────────────────────────

        private static string BuildSummaryHeader()
        {
            return "real_time,game_time,battle_type,player_side,"
                + "attacker_leader,attacker_faction,defender_leader,defender_faction,"
                + "attacker_initial_troops,defender_initial_troops,attacker_power,defender_power,"
                + "attacker_infantry,attacker_ranged,attacker_cavalry,attacker_horse_archer,"
                + "defender_infantry,defender_ranged,defender_cavalry,defender_horse_archer,"
                + "winner,end_reason,attacker_remaining,defender_remaining,"
                + "attacker_wounded,attacker_killed,attacker_routed,defender_wounded,defender_killed,defender_routed,"
                + "total_rounds,total_ticks,mcm_ai_enabled,mcm_ai_speed,mcm_armor,mcm_shield_block,mcm_javelin,mcm_ranged_hit,mcm_attack_cap";
        }

        private static string BuildSummaryRow(BattleLogSession s)
        {
            MapEvent m = s.MapEvent;
            string winner = m.WinningSide.ToString();
            return Esc(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + ','
                + Esc(CampaignTime.Now.ToString()) + ','
                + m.EventType + ',' + m.PlayerSide + ','
                + Esc(GetLeaderName(m, BattleSideEnum.Attacker)) + ',' + Esc(GetFactionName(m, BattleSideEnum.Attacker)) + ','
                + Esc(GetLeaderName(m, BattleSideEnum.Defender)) + ',' + Esc(GetFactionName(m, BattleSideEnum.Defender)) + ','
                + s.AttackerInitialTroops + ',' + s.DefenderInitialTroops + ','
                + F(s.AttackerPower) + ',' + F(s.DefenderPower) + ','
                + s.AttackerComposition[0] + ',' + s.AttackerComposition[1] + ',' + s.AttackerComposition[2] + ',' + s.AttackerComposition[3] + ','
                + s.DefenderComposition[0] + ',' + s.DefenderComposition[1] + ',' + s.DefenderComposition[2] + ',' + s.DefenderComposition[3] + ','
                + winner + ',' + m.BattleState + ','
                + m.AttackerSide.NumRemainingSimulationTroops + ',' + m.DefenderSide.NumRemainingSimulationTroops + ','
                + s.TotalAttackerWounded + ',' + s.TotalAttackerKilled + ',' + s.TotalAttackerRouted + ','
                + s.TotalDefenderWounded + ',' + s.TotalDefenderKilled + ',' + s.TotalDefenderRouted + ','
                + s.RoundNumber + ',' + s.TickNumber + ','
                + (Settings.Instance?.AutoResolveAiEnabled == true ? 1 : 0) + ','
                + F(Settings.Instance?.AutoResolveAiSimulationSpeed ?? 1f) + ','
                + (Settings.Instance?.AutoResolveArmorEnabled == true ? 1 : 0) + ','
                + F(Settings.Instance?.AutoResolveShieldBlockChance ?? 0.1f) + ','
                + F(Settings.Instance?.AutoResolveJavelinChance ?? 0.05f) + ','
                + F(Settings.Instance?.AutoResolveRangedHitChance ?? 0.8f) + ','
                + F(Settings.Instance?.AutoResolveAttackRatioCap ?? 2f);
        }

        private static string BuildRoundHeader()
        {
            return "round,attacker_ticks,defender_ticks,"
                + "attacker_troops_before,defender_troops_before,attacker_morale_before,defender_morale_before,"
                + "attacker_damage,defender_damage,attacker_hits,defender_hits,"
                + "attacker_blocks,defender_blocks,attacker_misses,defender_misses,"
                + "attacker_wounded,defender_wounded,attacker_killed,defender_killed,attacker_routed,defender_routed,"
                + "attacker_troops_after,defender_troops_after,attacker_morale_after,defender_morale_after,round_winner";
        }

        private static string BuildTickHeader()
        {
            return "round,tick,side,striker_troop,striker_type,striker_weapon,weapon_damage,damage_type,"
                + "struck_troop,struck_type,struck_shield,struck_armor,result,original_damage,final_damage";
        }

        private static string BuildCasualtyHeader()
        {
            return "round,tick,side,troop,troop_type,event,damage";
        }

        // ── 工具 ─────────────────────────────────────────────────

        private static bool IsLogEnabled(MapEvent mapEvent)
        {
            return Settings.Instance?.EnableAutoResolveBattleLog == true && mapEvent != null && mapEvent.IsPlayerSimulation;
        }

        private static string GetTroopType(CharacterObject troop)
        {
            return GetTroopTypeIndex(troop) switch
            {
                0 => "Infantry",
                1 => "Ranged",
                2 => "Cavalry",
                3 => "HorseArcher",
                _ => "Unknown"
            };
        }

        private static int GetTroopTypeIndex(CharacterObject troop)
        {
            bool ranged = troop.IsRanged;
            bool mounted = troop.IsMounted;
            if (ranged && mounted) return 3; // 骑射手
            if (ranged) return 1;            // 射手
            if (mounted) return 2;           // 骑手
            return 0;                        // 步兵
        }

        private static string GetLeaderName(MapEvent mapEvent, BattleSideEnum side)
        {
            PartyBase leader = mapEvent.GetLeaderParty(side);
            return leader?.LeaderHero?.Name?.ToString() ?? leader?.Name?.ToString() ?? "?";
        }

        private static string GetFactionName(MapEvent mapEvent, BattleSideEnum side)
        {
            PartyBase leader = mapEvent.GetLeaderParty(side);
            return leader?.MapFaction?.Name?.ToString() ?? "?";
        }

        /// <summary>CSV 字段转义：含逗号/引号/换行时用双引号包裹。</summary>
        private static string Esc(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            if (value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0)
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        /// <summary>float 转字符串（不变文化，固定小数点）。</summary>
        private static string F(float value)
        {
            return value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static void WriteHeader(string folder, string fileName, string header)
        {
            File.WriteAllText(Path.Combine(folder, fileName), header + "\n", new UTF8Encoding(true));
        }

        private static void Flush(StringBuilder sb, string fileName)
        {
            if (sb.Length == 0)
            {
                return;
            }
            string content = sb.ToString();
            sb.Clear();
            File.AppendAllText(Path.Combine(_session.FolderPath, fileName), content, new UTF8Encoding(false));
        }
    }

    /// <summary>
    /// 坐镇日志补丁挂载点（无 Harmony 属性，由 HarmonyPatchRegistry 显式注册）。
    ///
    /// 挂载目标：
    ///   1. MapEvent.SimulateBattleRound        Prefix 轮开始 / Postfix 轮结束（BattleState != None 时收尾）
    ///   2. MapEventSide.OnTroopWounded          Postfix 伤亡事件（伤）
    ///   3. MapEventSide.OnTroopKilled           Postfix 伤亡事件（亡）
    ///   4. MapEventSide.OnTroopRouted           Postfix 伤亡事件（溃逃）
    ///   5. MapEvent.FinalizeEvent               Postfix 兜底收尾
    ///   6. BattleSimulation.OnFinished          Postfix 兜底收尾（玩家退出坐镇界面/撤退）
    /// </summary>
    internal static class AutoResolveBattleLogPatch
    {
        internal static void PrefixSimulateBattleRound(MapEvent __instance, int simulationTicksDefender, int simulationTicksAttacker)
        {
            AutoResolveBattleLog.RecordRoundStart(__instance, simulationTicksDefender, simulationTicksAttacker);
        }

        internal static void PostfixSimulateBattleRound(MapEvent __instance)
        {
            AutoResolveBattleLog.RecordRoundEnd(__instance);
        }

        internal static void PostfixOnTroopWounded(MapEventSide __instance, UniqueTroopDescriptor troopDesc1)
        {
            AutoResolveBattleLog.RecordCasualty(__instance, troopDesc1, "wounded");
        }

        internal static void PostfixOnTroopKilled(MapEventSide __instance, UniqueTroopDescriptor troopDesc1)
        {
            AutoResolveBattleLog.RecordCasualty(__instance, troopDesc1, "killed");
        }

        internal static void PostfixOnTroopRouted(MapEventSide __instance, UniqueTroopDescriptor troopDesc1, bool isOrderRetreat)
        {
            AutoResolveBattleLog.RecordCasualty(__instance, troopDesc1, "routed");
        }

        internal static void PostfixFinalizeEvent(MapEvent __instance)
        {
            AutoResolveBattleLog.FinishSession(__instance);
        }

        internal static void PostfixBattleSimulationFinished(BattleSimulation __instance)
        {
            AutoResolveBattleLog.FinishSession(__instance.MapEvent);
        }
    }
}
