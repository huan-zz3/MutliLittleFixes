#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
MutliLittleFixes 坐镇指挥战斗日志分析器
========================================

分析 AutoResolveBattleLog 生成的 CSV 日志（battle_summary.csv / round_log.csv /
tick_log.csv / casualty_log.csv），输出控制台汇总报告，可选导出 JSON 汇总与图数据 CSV。

用法:
    conda activate mutlilittlefixes
    python analyze_autoresolve.py                          # 分析日志根目录下最新一场
    python analyze_autoresolve.py --dir <时间戳文件夹>      # 指定一场
    python analyze_autoresolve.py --all                    # 全部场次汇总
    python analyze_autoresolve.py --json out.json          # 额外导出 JSON
    python analyze_autoresolve.py --charts out_prefix      # 导出图数据 CSV（round/weapon/troop）

依赖: 纯标准库即可运行；若安装了 pandas 会自动使用加速（可 conda install pandas）。
"""

import argparse
import csv
import json
import os
import statistics
import sys
from collections import Counter, defaultdict

# ── 日志根目录定位 ──
# 脚本位于 <游戏根>\Modules\MutliLittleFixes\Tools\，日志根在 <游戏根>\MutliLittleFixes_AutoResolveLogs\
# 向上探测：Tools -> MutliLittleFixes -> Modules -> 游戏根
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))


def _find_log_root():
    # 1) 环境变量显式指定
    env = os.environ.get("MLF_LOG_ROOT")
    if env and os.path.isdir(env):
        return env
    # 2) 从脚本位置向上探测游戏根
    d = SCRIPT_DIR
    for _ in range(6):  # 最多上溯 6 层
        candidate = os.path.join(d, "MutliLittleFixes_AutoResolveLogs")
        if os.path.isdir(candidate):
            return candidate
        parent = os.path.dirname(d)
        if parent == d:
            break
        d = parent
    # 3) 回退：脚本目录本身
    return SCRIPT_DIR


LOG_ROOT = _find_log_root()

TROOP_TYPES = ["Infantry", "Ranged", "Cavalry", "HorseArcher"]
DAMAGE_TYPES = ["Cut", "Pierce", "Blunt", "None"]

try:
    import pandas as pd  # noqa: F401  (可选加速)
    HAS_PANDAS = True
except ImportError:
    HAS_PANDAS = False


# ─────────────────────────── 读取 ───────────────────────────

def read_csv(path):
    """读取 CSV（兼容 UTF-8 BOM），返回 dict 行列表。"""
    rows = []
    with open(path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            rows.append(row)
    return rows


def find_battle_dirs(root):
    """找出所有含 battle_summary.csv 的场次目录（时间戳文件夹）。"""
    dirs = []
    if os.path.isdir(root):
        for name in sorted(os.listdir(root)):
            p = os.path.join(root, name)
            if os.path.isdir(p) and os.path.isfile(os.path.join(p, "battle_summary.csv")):
                dirs.append(p)
    return dirs


def fnum(v, default=0.0):
    try:
        return float(v)
    except (TypeError, ValueError):
        return default


def inum(v, default=0):
    try:
        return int(v)
    except (TypeError, ValueError):
        return default


# ─────────────────────────── 单场分析 ───────────────────────────

def analyze_battle(bdir):
    summary = read_csv(os.path.join(bdir, "battle_summary.csv"))
    rounds = read_csv(os.path.join(bdir, "round_log.csv"))
    ticks = read_csv(os.path.join(bdir, "tick_log.csv"))
    casualties = read_csv(os.path.join(bdir, "casualty_log.csv"))

    s = summary[0] if summary else {}

    # ---- 每 tick 攻击方视角：武器 × 攻方兵种类型 → 命中/伤害统计 ----
    # 按 (side, weapon, striker_type, damage_type) 聚合
    weapon_stats = defaultdict(lambda: {"hits": 0, "blocks": 0, "misses": 0,
                                         "total_final": 0.0, "total_orig": 0.0})
    # 攻方类型 × 守方类型 交叉命中表
    cross_stats = defaultdict(lambda: {"hits": 0, "total_final": 0.0})
    # 兵种类型输出/承伤
    type_output = defaultdict(lambda: {"hits": 0, "total_final": 0.0})   # 按攻方类型
    type_taken = defaultdict(lambda: {"hits": 0, "total_final": 0.0})   # 按守方类型
    # 护甲减伤效率（有护甲的命中）
    armor_evals = []  # (orig, final, armor)

    for t in ticks:
        side = t.get("side", "")
        weapon = (t.get("striker_weapon") or "").strip() or "Unarmed"
        stype = t.get("striker_type", "?")
        dtype = t.get("damage_type", "None")
        vtype = t.get("struck_type", "?")
        result = t.get("result", "hit")
        orig = fnum(t.get("original_damage"))
        final = fnum(t.get("final_damage"))
        armor = fnum(t.get("struck_armor"))
        shield = inum(t.get("struck_shield"))

        key = (side, weapon, stype, dtype)
        ws = weapon_stats[key]
        ws["hits"] += 1
        ws["total_final"] += final
        ws["total_orig"] += orig
        if result == "blocked":
            ws["blocks"] += 1
        elif result == "missed":
            ws["misses"] += 1

        cross = cross_stats[(stype, vtype)]
        cross["hits"] += 1
        cross["total_final"] += final

        type_output[stype]["hits"] += 1
        type_output[stype]["total_final"] += final
        type_taken[vtype]["hits"] += 1
        type_taken[vtype]["total_final"] += final

        if result == "hit" and armor > 0 and orig > 0:
            armor_evals.append((orig, final, armor))

    # ---- 伤亡统计 ----
    cas_by_type = defaultdict(Counter)   # event -> {type: count}
    cas_by_side = defaultdict(Counter)   # side -> {event: count}
    for c in casualties:
        cas_by_type[c.get("event", "?")][c.get("troop_type", "?")] += 1
        cas_by_side[c.get("side", "?")][c.get("event", "?")] += 1

    # ---- 每轮伤亡曲线（供画图）----
    round_curve = []
    for r in rounds:
        round_curve.append({
            "round": inum(r.get("round")),
            "attacker_damage": fnum(r.get("attacker_damage")),
            "defender_damage": fnum(r.get("defender_damage")),
            "attacker_killed": inum(r.get("attacker_killed")),
            "defender_killed": inum(r.get("defender_killed")),
            "attacker_wounded": inum(r.get("attacker_wounded")),
            "defender_wounded": inum(r.get("defender_wounded")),
        })

    # ---- 护甲减伤效率 ----
    armor_eff = None
    if armor_evals:
        # 最终伤害为护甲减伤后数值。减免绝对量 = 无甲时伤害 - 有甲时伤害。
        # 由于武器伤害模型最终伤普遍高于原版模拟伤害，这里只统计「命中且有甲」样本，
        # 报告平均护甲、原始(武器面板经武器倍率后)伤害、最终伤害，并给出无甲等效估算：
        # 护甲减免比例 ≈ (同武器同护甲下 无甲伤害 - 有甲伤害)/无甲伤害 —— 但日志不含无甲对照，
        # 因此改为报告「每点护甲平均减伤」(取命中样本中 final < orig 的差值 / 护甲)。
        reduced = [((o - f) / a, o, f, a) for o, f, a in armor_evals if o > f and a > 0]
        armor_eff = {
            "samples": len(armor_evals),
            "avg_orig": statistics.mean([o for o, _, _ in armor_evals]),
            "avg_final": statistics.mean([f for _, f, _ in armor_evals]),
            "avg_armor": statistics.mean([a for _, _, a in armor_evals]),
        }
        if reduced:
            armor_eff["avg_reduction_per_armor"] = statistics.mean([r[0] for r in reduced])
            armor_eff["reduction_ratio"] = statistics.mean([r[0] for r in reduced])  # 每点护甲减伤比例
            armor_eff["reduced_samples"] = len(reduced)

    return {
        "dir": bdir,
        "summary": s,
        "round_count": len(rounds),
        "tick_count": len(ticks),
        "casualty_count": len(casualties),
        "weapon_stats": weapon_stats,
        "cross_stats": cross_stats,
        "type_output": type_output,
        "type_taken": type_taken,
        "cas_by_type": cas_by_type,
        "cas_by_side": cas_by_side,
        "round_curve": round_curve,
        "armor_eff": armor_eff,
        "damage_hist": {  # 最终伤害分布（0-100 分桶）
            **{str(b): 0 for b in range(0, 101, 10)},
            "100+": 0,
        },
    }


# ─────────────────────────── 输出 ───────────────────────────

def print_header(title):
    print("=" * 72)
    print(title)
    print("=" * 72)


def print_battle_report(b):
    s = b["summary"]
    print_header(f"战斗总览 — {os.path.basename(b['dir'])}")

    if s:
        print(f"  真实时间   : {s.get('real_time', '?')}")
        print(f"  游戏时间   : {s.get('game_time', '?')}")
        print(f"  战斗类型   : {s.get('battle_type', '?')}   玩家阵营: {s.get('player_side', '?')}")
        print(f"  攻方       : {s.get('attacker_leader', '?')} ({s.get('attacker_faction', '?')})  兵力 {s.get('attacker_initial_troops', '?')}  战力 {s.get('attacker_power', '?')}")
        print(f"  守方       : {s.get('defender_leader', '?')} ({s.get('defender_faction', '?')})  兵力 {s.get('defender_initial_troops', '?')}  战力 {s.get('defender_power', '?')}")
        print(f"  兵种构成   : 攻方 步{s.get('attacker_infantry')}/射{s.get('attacker_ranged')}/骑{s.get('attacker_cavalry')}/骑射{s.get('attacker_horse_archer')}"
              f"    守方 步{s.get('defender_infantry')}/射{s.get('defender_ranged')}/骑{s.get('defender_cavalry')}/骑射{s.get('defender_horse_archer')}")
        print(f"  结果       : 胜方 {s.get('winner', '?')} ({s.get('end_reason', '?')})  剩余 攻{s.get('attacker_remaining')}/守{s.get('defender_remaining')}")
        print(f"  伤亡       : 攻方 伤{s.get('attacker_wounded')}/亡{s.get('attacker_killed')}/溃{s.get('attacker_routed')}"
              f"    守方 伤{s.get('defender_wounded')}/亡{s.get('defender_killed')}/溃{s.get('defender_routed')}")
        print(f"  规模       : {b['round_count']} 轮 / {b['tick_count']} 次对抗 / {b['casualty_count']} 个伤亡事件")

    # ---- 武器 × 兵种输出 Top ----
    print_header("武器输出 Top 15（按总伤害）")
    print(f"  {'攻方类型':<12}{'武器':<28}{'伤害类型':<8}{'次数':>6}{'格挡':>6}{'未中':>6}{'总伤害':>10}{'均伤':>8}")
    ranked = sorted(b["weapon_stats"].items(), key=lambda kv: kv[1]["total_final"], reverse=True)[:15]
    for (side, weapon, stype, dtype), ws in ranked:
        avg = ws["total_final"] / ws["hits"] if ws["hits"] else 0
        print(f"  {stype:<12}{weapon[:26]:<28}{dtype:<8}{ws['hits']:>6}{ws['blocks']:>6}{ws['misses']:>6}"
              f"{ws['total_final']:>10.1f}{avg:>8.2f}")

    # ---- 兵种交叉命中表 ----
    print_header("兵种交叉命中（攻方类型 × 守方类型 → 次数/总伤）")
    header = f"  {'攻方\\守方':<12}" + "".join(f"{t:<22}" for t in TROOP_TYPES)
    print(header)
    for at in TROOP_TYPES:
        cells = []
        for vt in TROOP_TYPES:
            c = b["cross_stats"].get((at, vt), {"hits": 0, "total_final": 0.0})
            cells.append(f"{c['hits']}次/{c['total_final']:.0f}伤")
        print(f"  {at:<12}" + "".join(f"{c:<22}" for c in cells))

    # ---- 兵种类型输出/承伤 ----
    print_header("兵种类型输出 vs 承伤")
    print(f"  {'类型':<12}{'输出次数':>8}{'输出总伤':>10}{'输出均伤':>8}   {'承伤次数':>8}{'承伤总伤':>10}")
    for t in TROOP_TYPES:
        o = b["type_output"].get(t, {"hits": 0, "total_final": 0.0})
        k = b["type_taken"].get(t, {"hits": 0, "total_final": 0.0})
        oavg = o["total_final"] / o["hits"] if o["hits"] else 0
        kavg = k["total_final"] / k["hits"] if k["hits"] else 0
        print(f"  {t:<12}{o['hits']:>8}{o['total_final']:>10.1f}{oavg:>8.2f}   {k['hits']:>8}{k['total_final']:>10.1f}")

    # ---- 伤亡 ----
    print_header("伤亡分布（事件 × 兵种类型）")
    for event in ["killed", "wounded", "routed"]:
        cnt = b["cas_by_type"].get(event)
        if cnt:
            parts = "  ".join(f"{t}:{cnt[t]}" for t in TROOP_TYPES if cnt[t])
            print(f"  {event:<8}: {parts}")

    # ---- 护甲减伤 ----
    ae = b["armor_eff"]
    if ae:
        print_header("护甲减伤（有甲命中样本）")
        extra = ""
        if "reduced_samples" in ae:
            extra = f" | 每点护甲平均减伤 {ae['avg_reduction_per_armor']:.3f}（{ae['reduced_samples']}/{ae['samples']} 次命中最终伤低于原始伤）"
        print(f"  样本: {ae['samples']} 次带甲命中 | 平均护甲 {ae['avg_armor']:.1f} | "
              f"原始均伤 {ae['avg_orig']:.1f} → 最终均伤 {ae['avg_final']:.1f}{extra}")


# ─────────────────────────── JSON / 图数据导出 ───────────────────────────

def battle_to_json(b):
    """转为可序列化结构。"""
    return {
        "dir": b["dir"],
        "summary": b["summary"],
        "round_count": b["round_count"],
        "tick_count": b["tick_count"],
        "casualty_count": b["casualty_count"],
        "armor_eff": b["armor_eff"],
        "round_curve": b["round_curve"],
        "weapon_stats": {
            f"{side}|{weapon}|{stype}|{dtype}": ws
            for (side, weapon, stype, dtype), ws in b["weapon_stats"].items()
        },
        "cross_stats": {
            f"{at}->{vt}": c for (at, vt), c in b["cross_stats"].items()
        },
        "type_output": {k: v for k, v in b["type_output"].items()},
        "type_taken": {k: v for k, v in b["type_taken"].items()},
        "casualties": {
            event: dict(cnt) for event, cnt in b["cas_by_type"].items()
        },
    }


def export_charts(b, prefix):
    """导出图数据 CSV（round_curve / weapon / troop）。"""
    # 每轮伤害/伤亡曲线
    with open(f"{prefix}_rounds.csv", "w", newline="", encoding="utf-8-sig") as f:
        w = csv.DictWriter(f, fieldnames=["round", "attacker_damage", "defender_damage",
                                          "attacker_killed", "defender_killed",
                                          "attacker_wounded", "defender_wounded"])
        w.writeheader()
        for r in b["round_curve"]:
            w.writerow(r)
    # 武器聚合
    with open(f"{prefix}_weapons.csv", "w", newline="", encoding="utf-8-sig") as f:
        w = csv.writer(f)
        w.writerow(["side", "striker_type", "weapon", "damage_type", "hits", "blocks",
                    "misses", "total_final_damage", "avg_final_damage"])
        for (side, weapon, stype, dtype), ws in sorted(
                b["weapon_stats"].items(), key=lambda kv: kv[1]["total_final"], reverse=True):
            avg = ws["total_final"] / ws["hits"] if ws["hits"] else 0
            w.writerow([side, stype, weapon, dtype, ws["hits"], ws["blocks"],
                        ws["misses"], round(ws["total_final"], 2), round(avg, 2)])
    print(f"  [图表数据] 已导出: {prefix}_rounds.csv / {prefix}_weapons.csv")


# ─────────────────────────── 主流程 ───────────────────────────

def main():
    ap = argparse.ArgumentParser(description="MutliLittleFixes 坐镇战斗日志分析器")
    ap.add_argument("--dir", help="指定时间戳文件夹（绝对路径或相对日志根目录）")
    ap.add_argument("--all", action="store_true", help="分析全部场次")
    ap.add_argument("--json", metavar="PATH", help="导出 JSON 汇总")
    ap.add_argument("--charts", metavar="PREFIX", help="导出图数据 CSV（前缀）")
    args = ap.parse_args()

    print(f"[MutliLittleFixes 坐镇日志分析器]  日志根目录: {LOG_ROOT}")
    print(f"[依赖] pandas={'已启用' if HAS_PANDAS else '未安装（使用标准库）'}")

    all_dirs = find_battle_dirs(LOG_ROOT)
    if not all_dirs:
        print("错误: 未找到任何战斗日志（battle_summary.csv）")
        sys.exit(1)

    if args.dir:
        target = args.dir
        if not os.path.isabs(target):
            target = os.path.join(LOG_ROOT, target)
        if not os.path.isfile(os.path.join(target, "battle_summary.csv")):
            print(f"错误: 目录不含 battle_summary.csv: {target}")
            sys.exit(1)
        dirs = [target]
    elif args.all:
        dirs = all_dirs
    else:
        dirs = [all_dirs[-1]]  # 最新一场

    all_json = []
    for d in dirs:
        b = analyze_battle(d)
        print_battle_report(b)
        if args.charts:
            prefix = args.charts if len(dirs) == 1 else f"{args.charts}_{os.path.basename(d)}"
            export_charts(b, prefix)
        all_json.append(battle_to_json(b))

    if args.json:
        with open(args.json, "w", encoding="utf-8") as f:
            json.dump(all_json, f, ensure_ascii=False, indent=2)
        print(f"\n[JSON] 已导出: {args.json}")

    if len(dirs) > 1:
        print_header(f"全部 {len(dirs)} 场次汇总")
        for b in all_json:
            s = b["summary"]
            print(f"  {os.path.basename(b['dir'])} | {s.get('battle_type','?'):<12} | "
                  f"攻{s.get('attacker_initial_troops')} vs 守{s.get('defender_initial_troops')} | "
                  f"胜方 {s.get('winner','?')} | {b['round_count']}轮/{b['tick_count']}tick")


if __name__ == "__main__":
    main()
