using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200000E RID: 14
	public class TeamAINavalRaidDefenderComponent : TeamAIComponent
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00004F80 File Offset: 0x00003180
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00004F88 File Offset: 0x00003188
		public bool LandingCompleted { get; private set; }

		// Token: 0x06000077 RID: 119 RVA: 0x00004F94 File Offset: 0x00003194
		public TeamAINavalRaidDefenderComponent(Mission currentMission, Team currentTeam, float thinkTimerTime = 10f, float applyTimerTime = 1f)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			this._volumeBoxes = new MBList<VolumeBox>();
			List<GameEntity> list = new List<GameEntity>();
			currentMission.Scene.GetAllEntitiesWithScriptComponent<VolumeBox>(ref list);
			foreach (GameEntity gameEntity in list)
			{
				this._volumeBoxes.Add(gameEntity.GetFirstScriptOfType<VolumeBox>());
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005018 File Offset: 0x00003218
		public override void TickOccasionally()
		{
			if (!this._hasAttackersBreachedDesignatedPoint)
			{
				using (List<VolumeBox>.Enumerator enumerator = this._volumeBoxes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.HasAgentsInAttackerSide())
						{
							this._hasAttackersBreachedDesignatedPoint = true;
							MBList<StrategicArea> mblist = new MBList<StrategicArea>();
							foreach (StrategicArea strategicArea in base.StrategicAreas)
							{
								if (strategicArea.GameEntity.HasTag("volume_box_archer_point"))
								{
									mblist.Add(strategicArea);
								}
							}
							using (List<StrategicArea>.Enumerator enumerator2 = mblist.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									StrategicArea strategicArea2 = enumerator2.Current;
									strategicArea2.IsActive = false;
								}
								break;
							}
						}
					}
				}
			}
			base.TickOccasionally();
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005124 File Offset: 0x00003324
		public void OnLandingCompleted()
		{
			this.LandingCompleted = true;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005130 File Offset: 0x00003330
		public void OnShipLanded()
		{
			if (!this._hasLandingStarted)
			{
				this._hasLandingStarted = true;
				MBList<StrategicArea> mblist = new MBList<StrategicArea>();
				foreach (StrategicArea strategicArea in base.StrategicAreas)
				{
					if (strategicArea.GameEntity.HasTag("unsafe_archer_point"))
					{
						mblist.Add(strategicArea);
					}
				}
				foreach (StrategicArea strategicArea2 in mblist)
				{
					strategicArea2.IsActive = false;
				}
				MBReadOnlyList<Agent> activeAgents = Mission.Current.DefenderTeam.ActiveAgents;
				if (activeAgents.Count > 0)
				{
					Agent agent = activeAgents[MBRandom.RandomInt(activeAgents.Count)];
					string text = "event:/alerts/nods/stop";
					Vec3 position = agent.Position;
					SoundManager.StartOneShotEvent(text, ref position);
				}
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005230 File Offset: 0x00003430
		public override void OnUnitAddedToFormationForTheFirstTime(Formation formation)
		{
			if (GameNetwork.IsServer)
			{
				formation.ForceCalculateCaches();
				if (formation.AI.GetBehavior<BehaviorCharge>() == null)
				{
					if (formation.FormationIndex == 8)
					{
						formation.AI.AddAiBehavior(new BehaviorGeneral(formation));
					}
					else if (formation.FormationIndex == 9)
					{
						formation.AI.AddAiBehavior(new BehaviorProtectGeneral(formation));
					}
					formation.AI.AddAiBehavior(new BehaviorCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorPullBack(formation));
					formation.AI.AddAiBehavior(new BehaviorRegroup(formation));
					formation.AI.AddAiBehavior(new BehaviorReserve(formation));
					formation.AI.AddAiBehavior(new BehaviorRetreat(formation));
					formation.AI.AddAiBehavior(new BehaviorStop(formation));
					formation.AI.AddAiBehavior(new BehaviorTacticalCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPInfantry(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPLastFlagLastStand(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPMounted(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPMountedRanged(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPRanged(formation));
					return;
				}
			}
			else if (!GameNetwork.IsClientOrReplay)
			{
				formation.ForceCalculateCaches();
				if (formation.AI.GetBehavior<BehaviorCharge>() == null)
				{
					if (formation.FormationIndex == 8)
					{
						formation.AI.AddAiBehavior(new BehaviorGeneral(formation));
					}
					else if (formation.FormationIndex == 9)
					{
						formation.AI.AddAiBehavior(new BehaviorProtectGeneral(formation));
					}
					formation.AI.AddAiBehavior(new BehaviorCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorPullBack(formation));
					formation.AI.AddAiBehavior(new BehaviorRegroup(formation));
					formation.AI.AddAiBehavior(new BehaviorReserve(formation));
					formation.AI.AddAiBehavior(new BehaviorRetreat(formation));
					formation.AI.AddAiBehavior(new BehaviorStop(formation));
					formation.AI.AddAiBehavior(new BehaviorTacticalCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorAdvance(formation));
					formation.AI.AddAiBehavior(new BehaviorCautiousAdvance(formation));
					formation.AI.AddAiBehavior(new BehaviorCavalryScreen(formation));
					formation.AI.AddAiBehavior(new BehaviorDefensiveRing(formation));
					formation.AI.AddAiBehavior(new BehaviorFireFromInfantryCover(formation));
					formation.AI.AddAiBehavior(new BehaviorFlank(formation));
					formation.AI.AddAiBehavior(new BehaviorHoldHighGround(formation));
					formation.AI.AddAiBehavior(new BehaviorHorseArcherSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorMountedSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorProtectFlank(formation));
					formation.AI.AddAiBehavior(new BehaviorScreenedSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorSkirmishBehindFormation(formation));
					formation.AI.AddAiBehavior(new BehaviorSkirmishLine(formation));
					formation.AI.AddAiBehavior(new BehaviorVanguard(formation));
					formation.AI.AddAiBehavior(new BehaviorShootFromCliff(formation));
					formation.AI.AddAiBehavior(new BehaviorNavalRaidCliffShooting(formation));
					formation.AI.AddAiBehavior(new BehaviorNavalRaidHoldChokePoint(formation));
				}
			}
		}

		// Token: 0x04000040 RID: 64
		private bool _hasLandingStarted;

		// Token: 0x04000042 RID: 66
		private bool _hasAttackersBreachedDesignatedPoint;

		// Token: 0x04000043 RID: 67
		private MBList<VolumeBox> _volumeBoxes;
	}
}
