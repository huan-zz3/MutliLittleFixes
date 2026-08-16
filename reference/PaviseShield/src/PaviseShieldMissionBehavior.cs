using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PaviseShield
{
    public class PaviseShieldMissionBehavior : MissionBehavior
    {
        private const string PAVISE_ITEM_ID   = "pavise_shield";
        private const string PREFAB_NAME      = "pavise_shield_planted";
        private const float  PLACE_DISTANCE   = 0.65f;
        private const float  KEY_COOLDOWN     = 0.5f;

        // AI tuning — how long stationary before deploying, how far moved before undeploying
        private const float AI_DEPLOY_STATIONARY_TIME   = 3.0f;  // seconds standing still
        private const float AI_UNDEPLOY_MOVE_DISTANCE   = 2.0f;  // meters moved while deployed
        private const float AI_TICK_INTERVAL            = 1.0f;  // check every 1 second

        private static readonly InputKey DEPLOY_KEY   = InputKey.F11;
        private static readonly InputKey UNDEPLOY_KEY = InputKey.J;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private readonly Dictionary<Agent, GameEntity>     _deployedAgents    = new Dictionary<Agent, GameEntity>();
        private readonly Dictionary<Agent, EquipmentIndex> _paviseSlots       = new Dictionary<Agent, EquipmentIndex>();

        // AI tracking — how long each agent has been stationary
        private readonly Dictionary<Agent, float>  _stationaryTime = new Dictionary<Agent, float>();
        private readonly Dictionary<Agent, Vec3>   _lastPosition   = new Dictionary<Agent, Vec3>();

        private float _cooldown   = 0f;
        private float _aiTick     = 0f;

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            _cooldown -= dt;
            _aiTick   -= dt;

            // Player hotkeys
            if (_cooldown <= 0f && Input.IsKeyPressed(DEPLOY_KEY))
            {
                _cooldown = KEY_COOLDOWN;
                ToggleDeploy();
            }

            if (_cooldown <= 0f && Input.IsKeyPressed(UNDEPLOY_KEY))
            {
                _cooldown = KEY_COOLDOWN;
                ToggleUndeploy();
            }

            // AI logic — runs every AI_TICK_INTERVAL seconds
            if (_aiTick <= 0f)
            {
                _aiTick = AI_TICK_INTERVAL;
                TickAI();
            }

            // Clean up dead agents
            foreach (var kvp in _deployedAgents.ToList())
            {
                if (kvp.Key == null || !kvp.Key.IsActive())
                {
                    try { kvp.Value?.Remove(0); } catch { }
                    _deployedAgents.Remove(kvp.Key!);
                    _paviseSlots.Remove(kvp.Key!);
                    _stationaryTime.Remove(kvp.Key!);
                    _lastPosition.Remove(kvp.Key!);
                }
            }
        }

        // ── AI Logic ──────────────────────────────────────────────────────────

        private void TickAI()
        {
            var allAgents = Mission.Current?.Agents;
            if (allAgents == null) return;

            foreach (var agent in allAgents)
            {
                if (agent == null || !agent.IsActive()) continue;
                if (agent.IsMainAgent) continue;

                // Only auto-deploy enemy AI — player team is manual command only
                if (agent.Team == Mission.Current?.PlayerTeam) continue;

                if (!HasPavise(agent) && !_deployedAgents.ContainsKey(agent)) continue;

                if (_deployedAgents.ContainsKey(agent))
                {
                    // Undeploy only if agent has moved significantly from deploy point
                    if (_lastPosition.TryGetValue(agent, out Vec3 deployPos))
                    {
                        if (agent.Position.Distance(deployPos) > AI_UNDEPLOY_MOVE_DISTANCE)
                            Undeploy(agent);
                    }
                }
                else
                {
                    if (_lastPosition.TryGetValue(agent, out Vec3 lastPos))
                    {
                        float moved = agent.Position.Distance(lastPos);
                        if (moved < 0.3f)
                        {
                            if (!_stationaryTime.ContainsKey(agent))
                                _stationaryTime[agent] = 0f;
                            _stationaryTime[agent] += AI_TICK_INTERVAL;

                            // Only deploy if stationary long enough AND in a ranged formation
                            if (_stationaryTime[agent] >= AI_DEPLOY_STATIONARY_TIME
                                && agent.Formation?.QuerySystem.IsRangedFormation == true)
                            {
                                Deploy(agent);
                            }
                        }
                        else
                        {
                            _stationaryTime[agent] = 0f;
                        }
                    }

                    _lastPosition[agent] = agent.Position;
                }
            }
        }

        // ── Player Hotkeys ────────────────────────────────────────────────────

        private void ToggleDeploy()
        {
            var team = Mission.Current?.PlayerTeam;
            if (team == null) return;

            var toDeploy = GetTargetAgents(team)
                .Where(a => !_deployedAgents.ContainsKey(a) && HasPavise(a)).ToList();

            if (toDeploy.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "No pavise shield bearers found.", Color.FromUint(0xFFAAAAAA)));
                return;
            }

            foreach (var agent in toDeploy)
                Deploy(agent);

            InformationManager.DisplayMessage(new InformationMessage(
                $"{toDeploy.Count} pavise bearer(s) planted their shields.",
                Color.FromUint(0xFFDDAA00)));
        }

        private void ToggleUndeploy()
        {
            var team = Mission.Current?.PlayerTeam;
            if (team == null) return;

            var toUndeploy = GetTargetAgents(team)
                .Where(a => _deployedAgents.ContainsKey(a)).ToList();

            if (toUndeploy.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "No deployed pavises found.", Color.FromUint(0xFFAAAAAA)));
                return;
            }

            foreach (var agent in toUndeploy)
                Undeploy(agent);

            InformationManager.DisplayMessage(new InformationMessage(
                $"{toUndeploy.Count} pavise bearer(s) picked up their shields.",
                Color.FromUint(0xFFDDAA00)));
        }

        // ── Core Deploy/Undeploy ──────────────────────────────────────────────

        private void Deploy(Agent agent)
        {
            EquipmentIndex slot = GetPaviseSlot(agent);
            if (slot == EquipmentIndex.None) return;

            try
            {
                agent.RemoveEquippedWeapon(slot);
                agent.TryToWieldWeaponInSlot(EquipmentIndex.Weapon0, Agent.WeaponWieldActionType.Instant, false);
            }
            catch { }

            GameEntity? entity = SpawnProp(agent);
            if (entity == null) return;

            _deployedAgents[agent]  = entity;
            _paviseSlots[agent]     = slot;
            _lastPosition[agent]    = agent.Position;
            _stationaryTime.Remove(agent);
        }

        private void Undeploy(Agent agent)
        {
            if (!_deployedAgents.TryGetValue(agent, out GameEntity? entity)) return;

            try { entity?.Remove(0); } catch { }
            _deployedAgents.Remove(agent);
            _lastPosition.Remove(agent);
            _stationaryTime.Remove(agent);

            if (_paviseSlots.TryGetValue(agent, out EquipmentIndex slot))
            {
                _paviseSlots.Remove(agent);
                try
                {
                    ItemObject? item = Game.Current.ObjectManager.GetObject<ItemObject>(PAVISE_ITEM_ID);
                    if (item != null)
                    {
                        MissionWeapon weapon = new MissionWeapon(item, null, null);
                        agent.EquipWeaponWithNewEntity(slot, ref weapon);
                    }
                }
                catch { }
            }
        }

        private GameEntity? SpawnProp(Agent agent)
        {
            try
            {
                Vec3 pos = agent.Position;
                Vec2 dir = agent.GetMovementDirection();

                float len = dir.Length;
                if (len > 0.001f) dir /= len;
                else dir = new Vec2(1f, 0f);

                Vec2 pos2D    = new Vec2(pos.x + dir.x * PLACE_DISTANCE, pos.y + dir.y * PLACE_DISTANCE);
                float groundZ = Mission.Current.Scene.GetTerrainHeight(pos2D);
                Vec3 spawnPos = new Vec3(pos2D.x, pos2D.y, groundZ + 0.9f);

                Mat3 rotation = Mat3.CreateMat3WithForward(new Vec3(dir.x, dir.y, 0f));
                rotation.RotateAboutSide((float)(-Math.PI / 2.0));

                MatrixFrame frame  = new MatrixFrame(rotation, spawnPos);
                GameEntity entity  = GameEntity.Instantiate(Mission.Current.Scene, PREFAB_NAME, frame);
                entity.SetPhysicsState(true, false);
                return entity;
            }
            catch { return null; }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private List<Agent> GetTargetAgents(Team team)
        {
            var selected = team.FormationsIncludingEmpty
                .Where(f => f.CountOfUnits > 0 && team.PlayerOrderController.IsFormationListening(f))
                .ToList();

            if (selected.Count > 0)
            {
                var result = new List<Agent>();
                foreach (var f in selected)
                    f.ApplyActionOnEachUnit(u => { if (u is Agent a && (HasPavise(a) || _deployedAgents.ContainsKey(a))) result.Add(a); });
                return result;
            }

            return Mission.Current.Agents
                .Where(a => a.IsActive() && a.Team == team && (HasPavise(a) || _deployedAgents.ContainsKey(a)))
                .ToList();
        }

        private static bool HasPavise(Agent agent) => GetPaviseSlot(agent) != EquipmentIndex.None;

        private static EquipmentIndex GetPaviseSlot(Agent agent)
        {
            if (agent?.Character == null) return EquipmentIndex.None;
            for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumAllWeaponSlots; i++)
            {
                var el = agent.Equipment[i];
                if (!el.IsEmpty && el.Item?.StringId == PAVISE_ITEM_ID)
                    return i;
            }
            return EquipmentIndex.None;
        }

        public override void OnAgentRemoved(Agent affected, Agent affector, AgentState state, KillingBlow blow)
        {
            if (_deployedAgents.TryGetValue(affected, out GameEntity? entity))
            {
                try { entity?.Remove(0); } catch { }
                _deployedAgents.Remove(affected);
                _paviseSlots.Remove(affected);
                _stationaryTime.Remove(affected);
                _lastPosition.Remove(affected);
            }
        }

        public override void OnRemoveBehavior()
        {
            foreach (var e in _deployedAgents.Values)
                try { e?.Remove(0); } catch { }
            _deployedAgents.Clear();
            _paviseSlots.Clear();
            _stationaryTime.Clear();
            _lastPosition.Clear();
        }
    }
}
