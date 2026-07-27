using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.DWA;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Missions.ShipInput;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects.Usables;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x0200009C RID: 156
	public class MissionShip : MissionObject
	{
		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x0004D4E6 File Offset: 0x0004B6E6
		// (set) Token: 0x06000AE4 RID: 2788 RVA: 0x0004D4ED File Offset: 0x0004B6ED
		public static int MaxShipIndex { get; private set; }

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x0004D4F5 File Offset: 0x0004B6F5
		public bool AnyActiveFormationTroopOnShip
		{
			get
			{
				return this._anyActiveFormationTroopOnShip.Value;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x0004D502 File Offset: 0x0004B702
		// (set) Token: 0x06000AE7 RID: 2791 RVA: 0x0004D50A File Offset: 0x0004B70A
		public int Index { get; private set; }

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x0004D513 File Offset: 0x0004B713
		public bool IsRemoved
		{
			get
			{
				return this._isRemoved;
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x0004D51C File Offset: 0x0004B71C
		public MatrixFrame GlobalFrame
		{
			get
			{
				return base.GameEntity.GetGlobalFrame();
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000AEA RID: 2794 RVA: 0x0004D537 File Offset: 0x0004B737
		public MBReadOnlyList<MatrixFrame> OuterDeckLocalFrames
		{
			get
			{
				return this._outerDeckLocalFrames;
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000AEB RID: 2795 RVA: 0x0004D53F File Offset: 0x0004B73F
		public MBReadOnlyList<MatrixFrame> InnerDeckLocalFrames
		{
			get
			{
				return this._innerDeckLocalFrames;
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000AEC RID: 2796 RVA: 0x0004D547 File Offset: 0x0004B747
		public MBReadOnlyList<MatrixFrame> CrewSpawnLocalFrames
		{
			get
			{
				return this._crewSpawnLocalFrames;
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000AED RID: 2797 RVA: 0x0004D54F File Offset: 0x0004B74F
		public int DeckFrameCount
		{
			get
			{
				return this._innerDeckLocalFrames.Count + this._outerDeckLocalFrames.Count;
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000AEE RID: 2798 RVA: 0x0004D568 File Offset: 0x0004B768
		public MBReadOnlyList<GameEntity> BannerEntities
		{
			get
			{
				return this._bannerEntities;
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000AEF RID: 2799 RVA: 0x0004D570 File Offset: 0x0004B770
		public MBReadOnlyList<GameEntity> SailMeshEntities
		{
			get
			{
				return this._sailMeshEntities;
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000AF0 RID: 2800 RVA: 0x0004D578 File Offset: 0x0004B778
		public Banner Banner
		{
			get
			{
				Banner banner;
				if ((banner = ShipHelper.GetShipBanner(this.ShipOrigin, this.Captain)) == null)
				{
					Team team = this.Team;
					if (team == null)
					{
						return null;
					}
					banner = team.Banner;
				}
				return banner;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x0004D5A0 File Offset: 0x0004B7A0
		[TupleElementNames(new string[] { "sailColor1", "sailColor2" })]
		public ValueTuple<uint, uint> SailColors
		{
			[return: TupleElementNames(new string[] { "sailColor1", "sailColor2" })]
			get
			{
				return ShipHelper.GetSailColors(this.ShipOrigin, this.Captain);
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x0004D5B3 File Offset: 0x0004B7B3
		public NavalPhysics Physics
		{
			get
			{
				return this._physics;
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x0004D5BB File Offset: 0x0004B7BB
		public float MaxHealth
		{
			get
			{
				return this.ShipOrigin.MaxHitPoints;
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x0004D5C8 File Offset: 0x0004B7C8
		public float MaxFireHealth
		{
			get
			{
				return this.ShipOrigin.MaxFireHitPoints;
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000AF5 RID: 2805 RVA: 0x0004D5D5 File Offset: 0x0004B7D5
		public float MaxPartialHealth
		{
			get
			{
				return this.MaxHealth * this._missionShipObject.PartialHitPointsRatio;
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000AF6 RID: 2806 RVA: 0x0004D5E9 File Offset: 0x0004B7E9
		public int TotalCrewCapacity
		{
			get
			{
				return this.ShipOrigin.TotalCrewCapacity;
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x0004D5F6 File Offset: 0x0004B7F6
		// (set) Token: 0x06000AF8 RID: 2808 RVA: 0x0004D5FE File Offset: 0x0004B7FE
		public int CrewSizeOnMainDeck { get; private set; }

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x0004D607 File Offset: 0x0004B807
		public int CrewSizeOnLowerDeck
		{
			get
			{
				return this.ShipOrigin.TotalCrewCapacity - this.CrewSizeOnMainDeck;
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000AFA RID: 2810 RVA: 0x0004D61B File Offset: 0x0004B81B
		public bool HasController
		{
			get
			{
				return this.Controller != null;
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000AFB RID: 2811 RVA: 0x0004D626 File Offset: 0x0004B826
		public AIShipController AIController
		{
			get
			{
				return (AIShipController)this.Controller;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000AFC RID: 2812 RVA: 0x0004D633 File Offset: 0x0004B833
		public bool IsAIControlled
		{
			get
			{
				return this.HasController && this.Controller.IsAIControlled;
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000AFD RID: 2813 RVA: 0x0004D64A File Offset: 0x0004B84A
		public bool IsPlayerControlled
		{
			get
			{
				return this.HasController && this.Controller.IsPlayerControlled;
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000AFE RID: 2814 RVA: 0x0004D661 File Offset: 0x0004B861
		public bool IsFormationAndShipAIControlled
		{
			get
			{
				return this.Formation != null && this.Formation.IsAIControlled && this.IsAIControlled;
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000AFF RID: 2815 RVA: 0x0004D680 File Offset: 0x0004B880
		public PlayerShipController PlayerController
		{
			get
			{
				return (PlayerShipController)this.Controller;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000B00 RID: 2816 RVA: 0x0004D68D File Offset: 0x0004B88D
		public FormationClass FormationIndex
		{
			get
			{
				Formation formation = this.Formation;
				if (formation == null)
				{
					return 0;
				}
				return formation.FormationIndex;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000B01 RID: 2817 RVA: 0x0004D6A0 File Offset: 0x0004B8A0
		public BattleSideEnum BattleSide
		{
			get
			{
				Team team = this.Team;
				if (team == null)
				{
					return -1;
				}
				return team.Side;
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000B02 RID: 2818 RVA: 0x0004D6B3 File Offset: 0x0004B8B3
		public MissionShipObject MissionShipObject
		{
			get
			{
				return this._missionShipObject;
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000B03 RID: 2819 RVA: 0x0004D6BB File Offset: 0x0004B8BB
		// (set) Token: 0x06000B04 RID: 2820 RVA: 0x0004D6C3 File Offset: 0x0004B8C3
		public NavalShipsLogic ShipsLogic { get; private set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000B05 RID: 2821 RVA: 0x0004D6CC File Offset: 0x0004B8CC
		public Team Team
		{
			get
			{
				Formation formation = this.Formation;
				if (formation == null)
				{
					return null;
				}
				return formation.Team;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000B06 RID: 2822 RVA: 0x0004D6DF File Offset: 0x0004B8DF
		// (set) Token: 0x06000B07 RID: 2823 RVA: 0x0004D6E7 File Offset: 0x0004B8E7
		public Formation Formation { get; private set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000B08 RID: 2824 RVA: 0x0004D6F0 File Offset: 0x0004B8F0
		public Agent Captain
		{
			get
			{
				Formation formation = this.Formation;
				if (formation == null)
				{
					return null;
				}
				return formation.Captain;
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000B09 RID: 2825 RVA: 0x0004D703 File Offset: 0x0004B903
		public bool IsInitialized
		{
			get
			{
				return this._missionShipObject != null;
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000B0A RID: 2826 RVA: 0x0004D70E File Offset: 0x0004B90E
		public bool IsRetreating
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000B0B RID: 2827 RVA: 0x0004D711 File Offset: 0x0004B911
		// (set) Token: 0x06000B0C RID: 2828 RVA: 0x0004D719 File Offset: 0x0004B919
		public MissionShip.SailState ShipSailState { get; private set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000B0D RID: 2829 RVA: 0x0004D722 File Offset: 0x0004B922
		// (set) Token: 0x06000B0E RID: 2830 RVA: 0x0004D72A File Offset: 0x0004B92A
		public bool HasCustomSailSetting { get; private set; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000B0F RID: 2831 RVA: 0x0004D733 File Offset: 0x0004B933
		public bool IsSinking
		{
			get
			{
				return this._physics.NavalSinkingState == NavalPhysics.SinkingState.Sinking || this._physics.NavalSinkingState == NavalPhysics.SinkingState.Sunk;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000B10 RID: 2832 RVA: 0x0004D753 File Offset: 0x0004B953
		public bool IsSunk
		{
			get
			{
				return this._physics.NavalSinkingState == NavalPhysics.SinkingState.Sunk;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000B11 RID: 2833 RVA: 0x0004D763 File Offset: 0x0004B963
		// (set) Token: 0x06000B12 RID: 2834 RVA: 0x0004D76B File Offset: 0x0004B96B
		public ShipOrder ShipOrder { get; private set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000B13 RID: 2835 RVA: 0x0004D774 File Offset: 0x0004B974
		// (set) Token: 0x06000B14 RID: 2836 RVA: 0x0004D77C File Offset: 0x0004B97C
		public IShipOrigin ShipOrigin { get; private set; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000B15 RID: 2837 RVA: 0x0004D785 File Offset: 0x0004B985
		public bool IsPlayerShip
		{
			get
			{
				Agent main = Agent.Main;
				MissionShip missionShip;
				if (main == null)
				{
					missionShip = null;
				}
				else
				{
					AgentNavalComponent component = main.GetComponent<AgentNavalComponent>();
					missionShip = ((component != null) ? component.FormationShip : null);
				}
				return missionShip == this;
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x0004D7A7 File Offset: 0x0004B9A7
		// (set) Token: 0x06000B17 RID: 2839 RVA: 0x0004D7AF File Offset: 0x0004B9AF
		public MatrixFrame RallyFrame { get; private set; }

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0004D7B8 File Offset: 0x0004B9B8
		public float HitPoints
		{
			get
			{
				return this.ShipOrigin.HitPoints;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06000B19 RID: 2841 RVA: 0x0004D7C5 File Offset: 0x0004B9C5
		// (set) Token: 0x06000B1A RID: 2842 RVA: 0x0004D7CD File Offset: 0x0004B9CD
		public float FireHitPoints { get; private set; }

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000B1B RID: 2843 RVA: 0x0004D7D6 File Offset: 0x0004B9D6
		// (set) Token: 0x06000B1C RID: 2844 RVA: 0x0004D7DE File Offset: 0x0004B9DE
		public float BurntHullDamageTotal { get; private set; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000B1D RID: 2845 RVA: 0x0004D7E7 File Offset: 0x0004B9E7
		public float VisualRudderRotationPercentage
		{
			get
			{
				return this._actuators.VisualRudderLocalRotation / this.MissionShipObject.RudderRotationMax;
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000B1E RID: 2846 RVA: 0x0004D800 File Offset: 0x0004BA00
		public float VisualRudderRotation
		{
			get
			{
				return this._actuators.VisualRudderLocalRotation;
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000B1F RID: 2847 RVA: 0x0004D80D File Offset: 0x0004BA0D
		public float VisualRudderPullDirection
		{
			get
			{
				return (float)this._actuators.VisualRudderPullDirection;
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000B20 RID: 2848 RVA: 0x0004D81B File Offset: 0x0004BA1B
		public float SailTargetSetting
		{
			get
			{
				MissionSail missionSail = this._actuators.Sails.FirstOrDefault<MissionSail>();
				if (missionSail == null)
				{
					return 0f;
				}
				return missionSail.TargetSailSetting;
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000B21 RID: 2849 RVA: 0x0004D83C File Offset: 0x0004BA3C
		public MBReadOnlyList<MissionSail> Sails
		{
			get
			{
				return this._actuators.Sails;
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000B22 RID: 2850 RVA: 0x0004D849 File Offset: 0x0004BA49
		// (set) Token: 0x06000B23 RID: 2851 RVA: 0x0004D851 File Offset: 0x0004BA51
		public ulong ShipUniqueBitwiseID { get; private set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000B24 RID: 2852 RVA: 0x0004D85A File Offset: 0x0004BA5A
		// (set) Token: 0x06000B25 RID: 2853 RVA: 0x0004D862 File Offset: 0x0004BA62
		public ulong ShipIslandCombinedID { get; private set; }

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000B26 RID: 2854 RVA: 0x0004D86B File Offset: 0x0004BA6B
		// (set) Token: 0x06000B27 RID: 2855 RVA: 0x0004D873 File Offset: 0x0004BA73
		public bool IsShipOrderActive { get; private set; } = true;

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000B28 RID: 2856 RVA: 0x0004D87C File Offset: 0x0004BA7C
		// (set) Token: 0x06000B29 RID: 2857 RVA: 0x0004D884 File Offset: 0x0004BA84
		public bool IsClimbingMachineStandAloneTickingActive { get; private set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000B2A RID: 2858 RVA: 0x0004D88D File Offset: 0x0004BA8D
		public MBReadOnlyList<ShipAttachmentMachine> AttachmentMachines
		{
			get
			{
				return this._attachmentMachines;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000B2B RID: 2859 RVA: 0x0004D895 File Offset: 0x0004BA95
		public MBReadOnlyList<ShipAttachmentPointMachine> AttachmentPointMachines
		{
			get
			{
				return this._attachmentPointMachines;
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000B2C RID: 2860 RVA: 0x0004D89D File Offset: 0x0004BA9D
		public MBReadOnlyList<ShipShieldComponent> Shields
		{
			get
			{
				return this._shields;
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000B2D RID: 2861 RVA: 0x0004D8A5 File Offset: 0x0004BAA5
		// (set) Token: 0x06000B2E RID: 2862 RVA: 0x0004D8AD File Offset: 0x0004BAAD
		public ClimbingMachineDetachment ClimbingMachineDetachment { get; private set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x0004D8B6 File Offset: 0x0004BAB6
		public MBReadOnlyList<ShipAttachmentMachine> ShipAttachmentMachines
		{
			get
			{
				return this._shipAttachmentMachines;
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000B30 RID: 2864 RVA: 0x0004D8BE File Offset: 0x0004BABE
		public MBReadOnlyList<ShipOarMachine> LeftSideShipOarMachines
		{
			get
			{
				return this._leftSideShipOarMachines;
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000B31 RID: 2865 RVA: 0x0004D8C6 File Offset: 0x0004BAC6
		public MBReadOnlyList<ShipOarMachine> RightSideShipOarMachines
		{
			get
			{
				return this._rightSideShipOarMachines;
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000B32 RID: 2866 RVA: 0x0004D8CE File Offset: 0x0004BACE
		public MBReadOnlyList<ShipOarMachine> ShipOarMachines
		{
			get
			{
				return this._shipOarMachines;
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000B33 RID: 2867 RVA: 0x0004D8D6 File Offset: 0x0004BAD6
		public MBReadOnlyList<ClimbingMachine> ClimbingMachines
		{
			get
			{
				return this._climbingMachines;
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000B34 RID: 2868 RVA: 0x0004D8DE File Offset: 0x0004BADE
		public MBReadOnlyList<ShipUnmannedOar> ShipUnmannedOars
		{
			get
			{
				return this._shipUnmannedOars;
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000B35 RID: 2869 RVA: 0x0004D8E6 File Offset: 0x0004BAE6
		public MBReadOnlyList<DestructableComponent> AllDestructableComponents
		{
			get
			{
				return this._allDestructibleComponents;
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000B36 RID: 2870 RVA: 0x0004D8EE File Offset: 0x0004BAEE
		// (set) Token: 0x06000B37 RID: 2871 RVA: 0x0004D8F6 File Offset: 0x0004BAF6
		public ShipControllerMachine ShipControllerMachine { get; private set; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000B38 RID: 2872 RVA: 0x0004D8FF File Offset: 0x0004BAFF
		public float MaxSailHitPoints
		{
			get
			{
				return this.ShipOrigin.MaxSailHitPoints;
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000B39 RID: 2873 RVA: 0x0004D90C File Offset: 0x0004BB0C
		public float SailHitPoints
		{
			get
			{
				return this.ShipOrigin.SailHitPoints;
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000B3A RID: 2874 RVA: 0x0004D919 File Offset: 0x0004BB19
		// (set) Token: 0x06000B3B RID: 2875 RVA: 0x0004D921 File Offset: 0x0004BB21
		public bool IsDeployed { get; private set; }

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000B3C RID: 2876 RVA: 0x0004D92A File Offset: 0x0004BB2A
		// (set) Token: 0x06000B3D RID: 2877 RVA: 0x0004D932 File Offset: 0x0004BB32
		public bool CanBeTakenOver { get; private set; } = true;

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000B3E RID: 2878 RVA: 0x0004D93B File Offset: 0x0004BB3B
		// (set) Token: 0x06000B3F RID: 2879 RVA: 0x0004D943 File Offset: 0x0004BB43
		public TeamSideEnum OriginalTeamSide { get; private set; } = -1;

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000B40 RID: 2880 RVA: 0x0004D94C File Offset: 0x0004BB4C
		// (set) Token: 0x06000B41 RID: 2881 RVA: 0x0004D954 File Offset: 0x0004BB54
		public Agent SailBurnerAgent { get; private set; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000B42 RID: 2882 RVA: 0x0004D95D File Offset: 0x0004BB5D
		// (set) Token: 0x06000B43 RID: 2883 RVA: 0x0004D965 File Offset: 0x0004BB65
		public SoundEvent SailBurningSoundEvent { get; private set; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000B44 RID: 2884 RVA: 0x0004D96E File Offset: 0x0004BB6E
		// (set) Token: 0x06000B45 RID: 2885 RVA: 0x0004D976 File Offset: 0x0004BB76
		public ShipController Controller { get; private set; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000B46 RID: 2886 RVA: 0x0004D97F File Offset: 0x0004BB7F
		// (set) Token: 0x06000B47 RID: 2887 RVA: 0x0004D987 File Offset: 0x0004BB87
		public RangedSiegeWeapon ShipSiegeWeapon { get; private set; }

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000B48 RID: 2888 RVA: 0x0004D990 File Offset: 0x0004BB90
		// (set) Token: 0x06000B49 RID: 2889 RVA: 0x0004D998 File Offset: 0x0004BB98
		public bool IsShipNavmeshDisabled { get; private set; }

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x0004D9A1 File Offset: 0x0004BBA1
		public bool HasDWAAgent
		{
			get
			{
				return this._dwaAgentDelegate != null;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000B4B RID: 2891 RVA: 0x0004D9AC File Offset: 0x0004BBAC
		public int DWAAgentId
		{
			get
			{
				return this._dwaAgentDelegate.Id;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x0004D9B9 File Offset: 0x0004BBB9
		public readonly ref DWAAgentState DWAAgentState
		{
			get
			{
				return this._dwaAgentDelegate.State;
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000B4D RID: 2893 RVA: 0x0004D9C6 File Offset: 0x0004BBC6
		// (set) Token: 0x06000B4E RID: 2894 RVA: 0x0004D9CE File Offset: 0x0004BBCE
		public ShipPlacementDetachment ShipPlacementDetachment { get; private set; }

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000B4F RID: 2895 RVA: 0x0004D9D7 File Offset: 0x0004BBD7
		public bool HasPlayerStandingPointEntity
		{
			get
			{
				return this.PlayerStandingPointEntity != null;
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000B50 RID: 2896 RVA: 0x0004D9E5 File Offset: 0x0004BBE5
		public GameEntity PlayerStandingPointEntity
		{
			get
			{
				return this._playerStandingPointEntity;
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000B51 RID: 2897 RVA: 0x0004D9ED File Offset: 0x0004BBED
		// (set) Token: 0x06000B52 RID: 2898 RVA: 0x0004D9F5 File Offset: 0x0004BBF5
		public bool BeingAbandoned { get; private set; }

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000B53 RID: 2899 RVA: 0x0004D9FE File Offset: 0x0004BBFE
		public override TextObject HitObjectName
		{
			get
			{
				return new TextObject("{=1nbU1tV5}Ship", null);
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000B54 RID: 2900 RVA: 0x0004DA0B File Offset: 0x0004BC0B
		public static uint MissionShipScriptNameHash
		{
			get
			{
				return MissionShip._missionShipScriptNameHash;
			}
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0004DA14 File Offset: 0x0004BC14
		public MissionShip()
		{
			this._anyActiveFormationTroopOnShip = new QueryData<bool>(delegate
			{
				Formation formation = this.Formation;
				if (formation != null && formation.CountOfUnits > 0)
				{
					using (List<IFormationUnit>.Enumerator enumerator = this.Formation.Arrangement.GetAllUnits().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Agent agent;
							if ((agent = enumerator.Current as Agent) != null)
							{
								AgentMovementMode agentMovementMode = agent.MovementMode & 3;
								if (agentMovementMode != 2 && agentMovementMode != 3)
								{
									return true;
								}
							}
						}
					}
					using (List<Agent>.Enumerator enumerator2 = this.Formation.DetachedUnits.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Agent agent2;
							if ((agent2 = enumerator2.Current) != null)
							{
								AgentMovementMode agentMovementMode2 = agent2.MovementMode & 3;
								if (agentMovementMode2 != 2 && agentMovementMode2 != 3)
								{
									return true;
								}
							}
						}
					}
					return false;
				}
				return false;
			}, 5f);
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x0004DAAC File Offset: 0x0004BCAC
		public void BreakAllExistingConnections()
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._shipAttachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null)
				{
					shipAttachmentMachine.CurrentAttachment.Destroy();
					shipAttachmentMachine.CheckCurrentAttachmentAndInitializeRopeBoundingBox();
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null)
				{
					ShipAttachmentMachine attachmentSource = shipAttachmentPointMachine.CurrentAttachment.AttachmentSource;
					if (attachmentSource != null)
					{
						attachmentSource.CheckCurrentAttachmentAndInitializeRopeBoundingBox();
					}
					shipAttachmentPointMachine.CurrentAttachment.Destroy();
				}
			}
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x0004DB78 File Offset: 0x0004BD78
		public bool IsConnectionBlocked()
		{
			return this._connectionBlockedShipTime > Mission.Current.CurrentTime;
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x0004DB8C File Offset: 0x0004BD8C
		public bool IsConnectionPermanentlyBlocked()
		{
			return MBMath.ApproximatelyEqualsTo(this._connectionBlockedShipTime, float.MaxValue, 1E-05f);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0004DBA3 File Offset: 0x0004BDA3
		public bool IsDisconnectionBlocked()
		{
			return this._disconnectionBlockedShipTime > Mission.Current.CurrentTime;
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0004DBB7 File Offset: 0x0004BDB7
		public void BlockConnection()
		{
			this._connectionBlockedShipTime = float.MaxValue;
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x0004DBC4 File Offset: 0x0004BDC4
		public void ResetDisconnectionBlock()
		{
			this._disconnectionBlockedShipTime = 0f;
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0004DBD1 File Offset: 0x0004BDD1
		public void ResetConnectionBlock()
		{
			this._connectionBlockedShipTime = 0f;
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0004DBDE File Offset: 0x0004BDDE
		public void SetShipOrderActive(bool isOrderActive)
		{
			this.IsShipOrderActive = isOrderActive;
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x0004DBE7 File Offset: 0x0004BDE7
		public void SetShipClimbingOrderStandAloneTickingActive(bool isShipClimbingMachineStandaloneTickingActive)
		{
			this.IsClimbingMachineStandAloneTickingActive = isShipClimbingMachineStandaloneTickingActive;
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0004DBF0 File Offset: 0x0004BDF0
		public void SetFoldSailsOnBridgeConnection(bool value)
		{
			this._foldSailsOnBridgeConnection = value;
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0004DBF9 File Offset: 0x0004BDF9
		public void SetOriginalTeamSide(TeamSideEnum teamSide)
		{
			this.OriginalTeamSide = teamSide;
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x0004DC02 File Offset: 0x0004BE02
		public void SetPlayerStandingPointEntity(GameEntity entity = null)
		{
			this._playerStandingPointEntity = entity;
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0004DC0C File Offset: 0x0004BE0C
		public void OnShipConnected(ShipAttachmentMachine.ShipAttachment currentAttachment)
		{
			if (currentAttachment.AttachmentTarget.OwnerShip == this && currentAttachment.AttachmentSource.OwnerShip.BattleSide != this.BattleSide)
			{
				bool flag = true;
				foreach (ShipAttachmentMachine shipAttachmentMachine in this.ShipAttachmentMachines)
				{
					if (shipAttachmentMachine.CurrentAttachment != currentAttachment)
					{
						ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentMachine.CurrentAttachment;
						MissionShip missionShip;
						if (currentAttachment2 == null)
						{
							missionShip = null;
						}
						else
						{
							ShipAttachmentPointMachine attachmentTarget = currentAttachment2.AttachmentTarget;
							missionShip = ((attachmentTarget != null) ? attachmentTarget.OwnerShip : null);
						}
						if (missionShip == this && shipAttachmentMachine.CurrentAttachment.AttachmentSource.OwnerShip.BattleSide != this.BattleSide)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					this._disconnectionBlockedShipTime = Mission.Current.CurrentTime + 30f;
					this._connectionBlockedShipTime = 0f;
				}
			}
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x0004DCF8 File Offset: 0x0004BEF8
		public void OnShipDisconnected(ShipAttachmentMachine.ShipAttachment currentAttachment)
		{
			if (this.ShipsLogic.CanHaveConnectionCooldown && currentAttachment.AttachmentTarget.OwnerShip == this && this._connectionBlockedShipTime <= 0f)
			{
				this._connectionBlockedShipTime = Mission.Current.CurrentTime + 30f;
			}
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x0004DD38 File Offset: 0x0004BF38
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (this._isRemoved)
			{
				return 0;
			}
			ScriptComponentBehavior.TickRequirement tickRequirement = 52;
			if (Mission.Current != null)
			{
				tickRequirement |= 2;
			}
			return tickRequirement;
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0004DD60 File Offset: 0x0004BF60
		public override void OnDeploymentFinished()
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				shipAttachmentMachine.OnDeploymentFinished();
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				shipAttachmentPointMachine.OnDeploymentFinished();
			}
			foreach (ShipOarMachine shipOarMachine in this._leftSideShipOarMachines)
			{
				shipOarMachine.OnDeploymentFinished();
			}
			foreach (ShipOarMachine shipOarMachine2 in this._rightSideShipOarMachines)
			{
				shipOarMachine2.OnDeploymentFinished();
			}
			foreach (ClimbingMachine climbingMachine in this._climbingMachines)
			{
				climbingMachine.OnDeploymentFinished();
			}
			this.ShipControllerMachine.OnDeploymentFinished();
			RangedSiegeWeapon shipSiegeWeapon = this.ShipSiegeWeapon;
			if (shipSiegeWeapon != null)
			{
				shipSiegeWeapon.OnDeploymentFinished();
			}
			foreach (AmmoBarrelBase ammoBarrelBase in this._ammoBarrels)
			{
				ammoBarrelBase.OnDeploymentFinished();
			}
			MissionShipRam ram = this._ram;
			if (ram != null)
			{
				ram.OnDeploymentFinished();
			}
			this.SetSiegeWeaponsInitialAmmoCount();
			this.CrewSizeOnMainDeck = MissionGameModels.Current.MissionShipParametersModel.CalculateMainDeckCrewSize(this.ShipOrigin, this.Formation.GetFirstUnit());
			this.SetAnchor(false, false, 1f);
			ShipWaterEffects firstScriptOfTypeRecursive = base.GameEntity.GetFirstScriptOfTypeRecursive<ShipWaterEffects>();
			if (firstScriptOfTypeRecursive != null)
			{
				firstScriptOfTypeRecursive.EnableWakeAndParticles();
			}
			ShipFloatsamManager firstScriptOfTypeRecursive2 = base.GameEntity.GetFirstScriptOfTypeRecursive<ShipFloatsamManager>();
			if (firstScriptOfTypeRecursive2 != null)
			{
				firstScriptOfTypeRecursive2.EnableFloatsamSystem();
			}
			this.IsDeployed = true;
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0004DF94 File Offset: 0x0004C194
		private void SetSiegeWeaponsInitialAmmoCount()
		{
			if (this.ShipSiegeWeapon != null)
			{
				int num = MissionGameModels.Current.MissionSiegeEngineCalculationModel.CalculateShipSiegeWeaponAmmoCount(this.ShipOrigin, this.Captain, this.ShipSiegeWeapon);
				this.ShipSiegeWeapon.SetStartAmmo(num);
			}
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0004DFD8 File Offset: 0x0004C1D8
		public override void SetAbilityOfFaces(bool enabled)
		{
			if (this.DynamicNavmeshIdStart > 0)
			{
				for (int i = 0; i < 49; i++)
				{
					base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + i, enabled);
				}
			}
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0004E018 File Offset: 0x0004C218
		public bool IsAgentOnShipNavmesh(int testedNavmeshID)
		{
			return testedNavmeshID >= this.DynamicNavmeshIdStart && testedNavmeshID < this.DynamicNavmeshIdStart + 50;
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x0004E031 File Offset: 0x0004C231
		public float GetPartialHitPoints(int index)
		{
			return this._partialHitPoints[index];
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x0004E03C File Offset: 0x0004C23C
		public void SetController(ShipControllerType controllerType, bool autoUpdateController = true)
		{
			this._autoUpdateController = autoUpdateController;
			if ((this.HasController ? this.Controller.ControllerType : ShipControllerType.None) != controllerType)
			{
				switch (controllerType)
				{
				case ShipControllerType.AI:
					this.Controller = new AIShipController(this);
					goto IL_0057;
				case ShipControllerType.Player:
					this.Controller = new PlayerShipController(this);
					goto IL_0057;
				}
				this.Controller = null;
				IL_0057:
				this.ShipsLogic.OnShipControllerChanged(this);
			}
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x0004E0AC File Offset: 0x0004C2AC
		public void SetCanBeTakenOver(bool value)
		{
			this.CanBeTakenOver = value;
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x0004E0B8 File Offset: 0x0004C2B8
		public MBReadOnlyList<MissionShip> GetShipsConnectedWithBridges()
		{
			this._temporaryMissionShipContainer.Clear();
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected && this._temporaryMissionShipContainer.IndexOf(shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip) < 0)
				{
					this._temporaryMissionShipContainer.Add(shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip);
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null && shipAttachmentPointMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected && this._temporaryMissionShipContainer.IndexOf(shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip) < 0)
				{
					this._temporaryMissionShipContainer.Add(shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip);
				}
			}
			return this._temporaryMissionShipContainer;
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x0004E1F0 File Offset: 0x0004C3F0
		public void SetInputRecord(in ShipInputRecord record)
		{
			this._inputRecord = record;
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0004E1FE File Offset: 0x0004C3FE
		public void SetOarAppliedForceMultiplierForStoryMission(float forceMultiplier)
		{
			this._actuators.SetOarAppliedForceMultiplierForStoryMission(forceMultiplier);
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x0004E20C File Offset: 0x0004C40C
		public bool SearchShipConnection(MissionShip soughtShip, bool isDirect, bool findEnemy, bool enforceActive, bool acceptNotBridgedConnections)
		{
			this._temporaryMissionShipQueue.Clear();
			this._visitedMissionShips.Clear();
			bool flag = false;
			foreach (MissionShip missionShip in (acceptNotBridgedConnections ? this.GetConnectedShips() : this.GetShipsConnectedWithBridges()))
			{
				if (missionShip != this && missionShip.Team != null)
				{
					if (missionShip == soughtShip)
					{
						flag = true;
					}
					if (isDirect)
					{
						if (missionShip != soughtShip)
						{
							Team team = this.Team;
							bool? flag2 = ((team != null) ? new bool?(team.IsEnemyOf(missionShip.Team)) : null);
							if (!((findEnemy == flag2.GetValueOrDefault()) & (flag2 != null)) || (enforceActive && !missionShip.AnyActiveFormationTroopOnShip))
							{
								goto IL_00BE;
							}
						}
						this._temporaryMissionShipQueue.Clear();
						this._visitedMissionShips.Clear();
						return true;
					}
					IL_00BE:
					this._temporaryMissionShipQueue.Enqueue(missionShip);
				}
			}
			this._visitedMissionShips.Add(this);
			while (this._temporaryMissionShipQueue.Count > 0)
			{
				MissionShip missionShip2 = this._temporaryMissionShipQueue.Dequeue();
				this._visitedMissionShips.Add(missionShip2);
				if (flag || missionShip2 != soughtShip)
				{
					if (missionShip2.Team != null)
					{
						Team team2 = this.Team;
						bool? flag2 = ((team2 != null) ? new bool?(team2.IsEnemyOf(missionShip2.Team)) : null);
						if (((findEnemy == flag2.GetValueOrDefault()) & (flag2 != null)) && (!enforceActive || missionShip2.AnyActiveFormationTroopOnShip))
						{
							goto IL_016D;
						}
					}
					foreach (MissionShip missionShip3 in (acceptNotBridgedConnections ? missionShip2.GetConnectedShips() : missionShip2.GetShipsConnectedWithBridges()))
					{
						if (!this._visitedMissionShips.Contains(missionShip3))
						{
							this._temporaryMissionShipQueue.Enqueue(missionShip3);
						}
					}
					continue;
				}
				IL_016D:
				this._temporaryMissionShipQueue.Clear();
				this._visitedMissionShips.Clear();
				return true;
			}
			this._temporaryMissionShipQueue.Clear();
			this._visitedMissionShips.Clear();
			return false;
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x0004E440 File Offset: 0x0004C640
		public void SetFormation(Formation newFormation)
		{
			if (this.Formation != newFormation)
			{
				if (this.Formation != null)
				{
					this.ShipOrder.FormationLeaveShip();
					this.Formation.OnUnitAttached -= this.OnUnitAttached;
				}
				this.Formation = newFormation;
				if (newFormation != null)
				{
					this.ShipOrder.FormationJoinShip(this.Formation);
					this.Formation.OnUnitAttached += this.OnUnitAttached;
				}
			}
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x0004E4B4 File Offset: 0x0004C6B4
		private void ProcessDetanglingShips()
		{
			if (this._detanglingMissionShip != null)
			{
				float remainingTimeInSeconds = this._detanglingMissionShipTimer.GetRemainingTimeInSeconds(false);
				if (remainingTimeInSeconds <= 3f)
				{
					float num = 1f - remainingTimeInSeconds / 3f;
					this.DetangleShip(num);
				}
			}
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0004E4F3 File Offset: 0x0004C6F3
		private void AddDetanglingShip(MissionShip ship, Vec3 contactPosAvg)
		{
			if (this._detanglingMissionShip == null || this._detanglingMissionShip == ship)
			{
				if (this._detanglingMissionShip == null)
				{
					this._detanglingMissionShipTimer.Reset();
				}
				this._detanglingMissionShip = ship;
				this._detanglingMissionShipAverageContactPosition = contactPosAvg;
			}
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x0004E527 File Offset: 0x0004C727
		private void RemoveDetanglingShip(MissionShip ship)
		{
			if (ship == this._detanglingMissionShip)
			{
				this._detanglingMissionShip = null;
				this._detanglingMissionShipTimer.Reset();
			}
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0004E544 File Offset: 0x0004C744
		public static float CalculateShipAlignWithVectorZTorque(MissionShip ship, Vec3 alignVector)
		{
			Mat3 rotation = ship.GameEntity.GetBodyWorldTransform().rotation;
			float num = MathF.Atan2(rotation.f.y, rotation.f.x);
			float num2 = MathF.Atan2(alignVector.y, alignVector.x) - num;
			num2 = MBMath.WrapAngle(num2);
			if (MathF.Abs(num2) > 1.5707964f)
			{
				if (num2 > 0f)
				{
					num2 -= 3.1415927f;
				}
				else
				{
					num2 += 3.1415927f;
				}
			}
			return num2 * 0.5f * ship.Physics.Mass * 50f - ship.Physics.AngularVelocity.z * ship.Physics.Mass * 60f;
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0004E600 File Offset: 0x0004C800
		private void DetangleShip(float detanglingHarshness)
		{
			MatrixFrame bodyWorldTransform = this._detanglingMissionShip.GameEntity.GetBodyWorldTransform();
			if (bodyWorldTransform.TransformToLocal(ref this._detanglingMissionShipAverageContactPosition).z < 0f)
			{
				MatrixFrame bodyWorldTransform2 = base.GameEntity.GetBodyWorldTransform();
				Vec2[] array = this.CalculateBoundingXYGlobalPlaneFromLocal(in bodyWorldTransform2, 0.9f);
				Vec2[] array2 = this._detanglingMissionShip.CalculateBoundingXYGlobalPlaneFromLocal(in bodyWorldTransform, 0.9f);
				Vec3 vec;
				if (MBMath.CheckPolygonIntersection(array, array2))
				{
					vec = bodyWorldTransform.origin - this._detanglingMissionShipAverageContactPosition;
					Vec3 vec2 = vec.NormalizedCopy();
					if (vec2.AsVec2.LengthSquared < 0.01f)
					{
						vec2.x = bodyWorldTransform.rotation.f.x;
						vec2.y = bodyWorldTransform.rotation.f.y;
						vec2.Normalize();
					}
					float num = 2f * detanglingHarshness;
					float mass = this._detanglingMissionShip.Physics.Mass;
					float num2 = MathF.Min(this._detanglingMissionShip.Physics.Mass, this.Physics.Mass);
					Vec3 vec3 = vec2 * mass * num;
					Vec3 vec4 = bodyWorldTransform.TransformToLocal(ref this._detanglingMissionShipAverageContactPosition);
					float num3 = num2 * 5f;
					if (vec3.LengthSquared >= num3 * num3)
					{
						vec3.Normalize();
						vec3 *= num3;
					}
					this._detanglingMissionShip.Physics.ApplyGlobalForceAtLocalPos(in vec4, in vec3, 0);
					Vec3 vec5 = bodyWorldTransform2.TransformToLocal(ref this._detanglingMissionShipAverageContactPosition);
					NavalPhysics physics = this.Physics;
					vec = -vec3;
					physics.ApplyGlobalForceAtLocalPos(in vec5, in vec, 0);
				}
				float num4 = MissionShip.CalculateShipAlignWithVectorZTorque(this._detanglingMissionShip, bodyWorldTransform2.rotation.s);
				NavalPhysics physics2 = this._detanglingMissionShip.Physics;
				vec = new Vec3(0f, 0f, num4, -1f);
				physics2.ApplyTorque(in vec, 0);
			}
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x0004E7E5 File Offset: 0x0004C9E5
		private void InitializeDetanglingShipInformation()
		{
			this._detanglingMissionShip = null;
			this._detanglingMissionShipTimer = new MissionTimer(6f);
			this._detanglingMissionShipAverageContactPosition = Vec3.Zero;
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x0004E80C File Offset: 0x0004CA0C
		private void InitializeLocalPhysicsBoundingXYPlane()
		{
			this._localPhysicsBoundingBoxXYPlaneVertices = new Vec2[4];
			this._scaledLocalPhysicsBoundingBoxXYPlaneVertices = new Vec2[4];
			Vec3 min = this.Physics.PhysicsBoundingBoxWithoutChildren.min;
			Vec3 max = this.Physics.PhysicsBoundingBoxWithoutChildren.max;
			this._localPhysicsBoundingBoxXYPlaneVertices[0] = new Vec2(min.x, min.y);
			this._localPhysicsBoundingBoxXYPlaneVertices[1] = new Vec2(min.x, max.y);
			this._localPhysicsBoundingBoxXYPlaneVertices[2] = new Vec2(max.x, max.y);
			this._localPhysicsBoundingBoxXYPlaneVertices[3] = new Vec2(max.x, min.y);
			this._scaledLocalPhysicsBoundingBoxXYPlaneVertices[0] = this._localPhysicsBoundingBoxXYPlaneVertices[0];
			this._scaledLocalPhysicsBoundingBoxXYPlaneVertices[1] = this._localPhysicsBoundingBoxXYPlaneVertices[1];
			this._scaledLocalPhysicsBoundingBoxXYPlaneVertices[2] = this._localPhysicsBoundingBoxXYPlaneVertices[2];
			this._scaledLocalPhysicsBoundingBoxXYPlaneVertices[3] = this._localPhysicsBoundingBoxXYPlaneVertices[3];
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0004E928 File Offset: 0x0004CB28
		public Vec2[] CalculateBoundingXYGlobalPlaneFromLocal(in MatrixFrame shipFrame, float scale = 1f)
		{
			Vec2[] physicsBoundingBoxXYPlaneVertices = this._physicsBoundingBoxXYPlaneVertices;
			int num = 0;
			MatrixFrame matrixFrame = shipFrame;
			Vec2 vec = this._localPhysicsBoundingBoxXYPlaneVertices[0] * scale;
			physicsBoundingBoxXYPlaneVertices[num] = matrixFrame.TransformToParent(ref vec);
			Vec2[] physicsBoundingBoxXYPlaneVertices2 = this._physicsBoundingBoxXYPlaneVertices;
			int num2 = 1;
			matrixFrame = shipFrame;
			vec = this._localPhysicsBoundingBoxXYPlaneVertices[1] * scale;
			physicsBoundingBoxXYPlaneVertices2[num2] = matrixFrame.TransformToParent(ref vec);
			Vec2[] physicsBoundingBoxXYPlaneVertices3 = this._physicsBoundingBoxXYPlaneVertices;
			int num3 = 2;
			matrixFrame = shipFrame;
			vec = this._localPhysicsBoundingBoxXYPlaneVertices[2] * scale;
			physicsBoundingBoxXYPlaneVertices3[num3] = matrixFrame.TransformToParent(ref vec);
			Vec2[] physicsBoundingBoxXYPlaneVertices4 = this._physicsBoundingBoxXYPlaneVertices;
			int num4 = 3;
			matrixFrame = shipFrame;
			vec = this._localPhysicsBoundingBoxXYPlaneVertices[3] * scale;
			physicsBoundingBoxXYPlaneVertices4[num4] = matrixFrame.TransformToParent(ref vec);
			return this._physicsBoundingBoxXYPlaneVertices;
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0004E9F8 File Offset: 0x0004CBF8
		public Vec2[] GetLocalPhysicsBoundingBoxXYPlaneVertices(float scale = 1f)
		{
			if (scale == 1f)
			{
				return this._localPhysicsBoundingBoxXYPlaneVertices;
			}
			for (int i = 0; i < 4; i++)
			{
				this._scaledLocalPhysicsBoundingBoxXYPlaneVertices[i] = this._localPhysicsBoundingBoxXYPlaneVertices[i] * scale;
			}
			return this._scaledLocalPhysicsBoundingBoxXYPlaneVertices;
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x0004EA44 File Offset: 0x0004CC44
		public void SetSinkingState(NavalPhysics.SinkingState state)
		{
			if (state != NavalPhysics.SinkingState.Sinking)
			{
				if (state == NavalPhysics.SinkingState.Sunk)
				{
					base.SetDisabled(true);
					string text = "event:/mission/movement/vessel/ship_sink";
					Vec3 globalPosition = base.GameEntity.GlobalPosition;
					SoundManager.StartOneShotEvent(text, ref globalPosition);
				}
			}
			else
			{
				for (int i = 0; i < this._partialHitPoints.Length; i++)
				{
					this._partialHitPoints[i] = 0f;
					this._physics.SetTargetDurabilityOfPart(i, 0f);
				}
				base.GameEntity.AddBodyFlags(1073741824, true);
				foreach (UsableMachine usableMachine in MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<UsableMachine>(base.GameEntity))
				{
					usableMachine.SetScriptComponentToTick(usableMachine.GetTickRequirement());
				}
				this.SetController(ShipControllerType.None, true);
				if (this.Team != null)
				{
					if (this.Team == Mission.Current.PlayerTeam || this.Team == Mission.Current.PlayerAllyTeam)
					{
						MBInformationManager.AddQuickInformation(MissionShip.PlayerSideShipSinkingText, 0, null, null, "");
					}
					else if (this.Team == Mission.Current.PlayerEnemyTeam)
					{
						MBInformationManager.AddQuickInformation(MissionShip.EnemySideShipSinkingText, 0, null, null, "");
					}
				}
				this.ClimbingMachineDetachment.Deactivate();
			}
			this._physics.SetSinkingState(state);
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0004EB9C File Offset: 0x0004CD9C
		public void SetAnchor(bool isAnchored, bool anchorInPlace = false, float forceMultiplier = 1f)
		{
			this._physics.SetAnchor(isAnchored, anchorInPlace, forceMultiplier);
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x0004EBAC File Offset: 0x0004CDAC
		public void SetAnchorFrame(in Vec2 position, in Vec2 direction, float forceMultiplier = 1f)
		{
			this._physics.SetAnchorFrame(in position, in direction, forceMultiplier);
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x0004EBBC File Offset: 0x0004CDBC
		public void DealCollisionDamage(MissionShip hitterShip, bool isRamDamage, Vec3 point, float damage)
		{
			int num2;
			int num3;
			DamageTypes damageTypes;
			bool flag;
			float num = this.DealDamage(damage, hitterShip, out num2, out num3, out damageTypes, out flag);
			bool flag2 = hitterShip != null && hitterShip.IsPlayerShip;
			if (Agent.Main != null && Agent.Main.IsActive() && (flag2 || this.IsPlayerShip) && num2 > 0)
			{
				CombatLogData combatLogData;
				combatLogData..ctor(false, flag2, flag2, false, false, false, this.IsPlayerShip, this.IsPlayerShip, false, false, false, false, this, false, false, false, 0f);
				combatLogData.InflictedDamage = num2;
				combatLogData.ModifiedDamage = num3;
				combatLogData.DamageType = damageTypes;
				combatLogData.IsFatalDamage = flag;
				combatLogData.IsEntityToEntityCollisionDamage = true;
				if (isRamDamage)
				{
					combatLogData.IsSpecialDamage = true;
				}
				Mission.Current.AddCombatLogSafe(null, null, combatLogData);
			}
			NavalAgentMoraleInteractionLogic moraleInteractionLogic = this._moraleInteractionLogic;
			if (moraleInteractionLogic != null)
			{
				moraleInteractionLogic.OnShipRammed(hitterShip, this);
			}
			Vec3 vec = base.GameEntity.GetBodyWorldTransform().TransformToLocal(ref point);
			int partIndexAtPosition = this._physics.GetPartIndexAtPosition(vec);
			if (partIndexAtPosition < 0 || partIndexAtPosition >= this._partialHitPoints.Length)
			{
				Debug.FailedAssert(string.Format("DealRammingDamage: GetPartIndexAtPosition for localPos {0} returned {1}.", vec, partIndexAtPosition), "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Objects\\MissionShip.cs", "DealCollisionDamage", 1238);
				return;
			}
			this._partialHitPoints[partIndexAtPosition] = MathF.Max(0f, this._partialHitPoints[partIndexAtPosition] - num);
			this._physics.SetTargetDurabilityOfPart(partIndexAtPosition, this._partialHitPoints[partIndexAtPosition] / this.MaxPartialHealth);
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0004ED30 File Offset: 0x0004CF30
		public void ResetFormationPositioning()
		{
			WorldPosition worldPosition;
			this.GetWorldPositionOnDeck(out worldPosition);
			this.Formation.SetPositioning(new WorldPosition?(worldPosition), new Vec2?(this.GlobalFrame.rotation.f.AsVec2.Normalized()), null);
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x0004ED84 File Offset: 0x0004CF84
		public float DealDamage(float rawDamage, MissionShip rammingShip, out int inflictedDamage, out int modifiedDamage, out DamageTypes damageType, out bool isFatalDamage)
		{
			float hitPoints = this.HitPoints;
			float num;
			this.ShipOrigin.OnShipDamaged(rawDamage, (rammingShip != null) ? rammingShip.ShipOrigin : null, ref num);
			modifiedDamage = (int)num;
			float num2 = hitPoints - this.HitPoints;
			damageType = 2;
			isFatalDamage = false;
			if (this.HitPoints <= 0f && this._physics.NavalSinkingState == NavalPhysics.SinkingState.Floating)
			{
				this.SetSinkingState(NavalPhysics.SinkingState.Sinking);
				NavalAgentMoraleInteractionLogic moraleInteractionLogic = this._moraleInteractionLogic;
				if (moraleInteractionLogic != null)
				{
					moraleInteractionLogic.OnShipSunk(this);
				}
				isFatalDamage = true;
			}
			if (this.HitPoints / this.ShipOrigin.MaxHitPoints <= 0.1f && hitPoints / this.ShipOrigin.MaxHitPoints > 0.1f)
			{
				this.ShipsLogic.OnShipLowHealth(this);
			}
			inflictedDamage = (int)rawDamage;
			return num2;
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x0004EE3C File Offset: 0x0004D03C
		public float DealDamageToSails(Agent attackerAgent, float rawDamage, float inflictedDamage, MissionSail sailHit)
		{
			float sailHitPoints = this.SailHitPoints;
			this.ShipOrigin.OnSailDamaged(rawDamage, inflictedDamage);
			float num = sailHitPoints - this.SailHitPoints;
			if (sailHit != null)
			{
				sailHit.OnSailHit(attackerAgent, rawDamage, inflictedDamage);
			}
			if (this.SailHitPoints <= 0f && this.ShipSailState == MissionShip.SailState.Intact)
			{
				foreach (MissionSail missionSail in this.Sails)
				{
					missionSail.StartFire();
				}
				this.SailBurnerAgent = attackerAgent;
				this.ShipSailState = MissionShip.SailState.Burning;
				if (!this.SailBurningSoundEvent.IsPlaying())
				{
					this.SailBurningSoundEvent.Play();
				}
			}
			return num;
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x0004EEF4 File Offset: 0x0004D0F4
		public bool GetIsConnected()
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
				bool flag;
				if (currentAttachment == null)
				{
					flag = null != null;
				}
				else
				{
					ShipAttachmentPointMachine attachmentTarget = currentAttachment.AttachmentTarget;
					flag = ((attachmentTarget != null) ? attachmentTarget.OwnerShip : null) != null;
				}
				if (flag)
				{
					return true;
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
				bool flag2;
				if (currentAttachment2 == null)
				{
					flag2 = null != null;
				}
				else
				{
					ShipAttachmentMachine attachmentSource = currentAttachment2.AttachmentSource;
					flag2 = ((attachmentSource != null) ? attachmentSource.OwnerShip : null) != null;
				}
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x0004EFC4 File Offset: 0x0004D1C4
		public bool GetIsConnectedToEnemyWithoutBridges()
		{
			bool flag = false;
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (this.Team != null)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
					bool flag2;
					if (currentAttachment == null)
					{
						flag2 = null != null;
					}
					else
					{
						ShipAttachmentPointMachine attachmentTarget = currentAttachment.AttachmentTarget;
						if (attachmentTarget == null)
						{
							flag2 = null != null;
						}
						else
						{
							MissionShip ownerShip = attachmentTarget.OwnerShip;
							flag2 = ((ownerShip != null) ? ownerShip.Team : null) != null;
						}
					}
					if (flag2 && shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip.Team.IsEnemyOf(this.Team))
					{
						if (shipAttachmentMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || shipAttachmentMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
						{
							return false;
						}
						flag = true;
					}
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				if (this.Team != null)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
					bool flag3;
					if (currentAttachment2 == null)
					{
						flag3 = null != null;
					}
					else
					{
						ShipAttachmentMachine attachmentSource = currentAttachment2.AttachmentSource;
						if (attachmentSource == null)
						{
							flag3 = null != null;
						}
						else
						{
							MissionShip ownerShip2 = attachmentSource.OwnerShip;
							flag3 = ((ownerShip2 != null) ? ownerShip2.Team : null) != null;
						}
					}
					if (flag3 && shipAttachmentPointMachine.CurrentAttachment.AttachmentTarget.OwnerShip.Team.IsEnemyOf(this.Team))
					{
						if (shipAttachmentPointMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || shipAttachmentPointMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
						{
							return false;
						}
						flag = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0004F158 File Offset: 0x0004D358
		public bool HasThrownOrActiveBridgeConnections()
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
				if (((currentAttachment != null) ? currentAttachment.AttachmentTarget : null) != null)
				{
					ShipAttachmentMachine.ShipAttachment.ShipAttachmentState state = shipAttachmentMachine.CurrentAttachment.State;
					if (state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown || state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
					{
						return true;
					}
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
				if (((currentAttachment2 != null) ? currentAttachment2.AttachmentSource : null) != null)
				{
					ShipAttachmentMachine.ShipAttachment.ShipAttachmentState state2 = shipAttachmentPointMachine.CurrentAttachment.State;
					if (state2 == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown || state2 == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0004F244 File Offset: 0x0004D444
		public bool HasMachine(UsableMachine usableMachine)
		{
			return this.ShipControllerMachine == usableMachine || (this._shipOarMachines != null && this._shipOarMachines.Contains(usableMachine)) || this.ShipSiegeWeapon == usableMachine || (this._attachmentMachines != null && this._attachmentMachines.Contains(usableMachine)) || (this._attachmentPointMachines != null && this._attachmentPointMachines.Contains(usableMachine)) || (this._shipAttachmentMachines != null && this._shipAttachmentMachines.Contains(usableMachine)) || (this._climbingMachines != null && this._climbingMachines.Contains(usableMachine));
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0004F2E0 File Offset: 0x0004D4E0
		public bool IsShipInCriticalZoneBetween(MissionShip ship2, MBReadOnlyList<MissionShip> allShips)
		{
			Vec2[] criticalZoneVerticesBetween = this.GetCriticalZoneVerticesBetween(ship2);
			foreach (MissionShip missionShip in allShips)
			{
				if (missionShip != this && missionShip != ship2)
				{
					MatrixFrame bodyWorldTransform = missionShip.GameEntity.GetBodyWorldTransform();
					Vec2[] array = missionShip.CalculateBoundingXYGlobalPlaneFromLocal(in bodyWorldTransform, 1f);
					if (MBMath.CheckPolygonIntersection(criticalZoneVerticesBetween, array))
					{
						return true;
					}
					if (MBMath.CheckPointInsidePolygon(ref criticalZoneVerticesBetween[0], ref criticalZoneVerticesBetween[1], ref criticalZoneVerticesBetween[2], ref criticalZoneVerticesBetween[3], ref array[0]))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0004F39C File Offset: 0x0004D59C
		public Vec2[] GetCriticalZoneVerticesBetween(MissionShip otherShip)
		{
			float num = 6.35f;
			MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
			MatrixFrame bodyWorldTransform2 = otherShip.GameEntity.GetBodyWorldTransform();
			Vec2[] array = this.CalculateBoundingXYGlobalPlaneFromLocal(in bodyWorldTransform, 1f);
			Vec2[] array2 = otherShip.CalculateBoundingXYGlobalPlaneFromLocal(in bodyWorldTransform2, 1f);
			Vec2 vec = array[0];
			Vec2 vec2 = array[3];
			Vec2 vec3 = array[0];
			Vec2 vec4 = array[1];
			Vec2 vec5 = array[3];
			Vec2 vec6 = array[2];
			Vec2 vec7 = array2[0];
			Vec2 vec8 = array2[1];
			Vec2 vec9 = array2[3];
			Vec2 vec10 = array2[2];
			float distanceSquareOfPointToLineSegment = MBMath.GetDistanceSquareOfPointToLineSegment(ref vec7, ref vec8, vec);
			float num2 = MBMath.GetDistanceSquareOfPointToLineSegment(ref vec7, ref vec8, vec2);
			Vec2 vec11;
			Vec2 vec12;
			if (distanceSquareOfPointToLineSegment < num2)
			{
				vec11 = vec3;
				vec12 = vec4;
			}
			else
			{
				vec11 = vec5;
				vec12 = vec6;
			}
			num2 = MBMath.GetDistanceSquareOfPointToLineSegment(ref vec9, ref vec10, vec);
			Vec2 vec13;
			Vec2 vec14;
			if (distanceSquareOfPointToLineSegment < num2)
			{
				vec13 = vec7;
				vec14 = vec8;
			}
			else
			{
				vec13 = vec9;
				vec14 = vec10;
			}
			Vec2 vec15 = MBMath.ProjectPointOntoLine(vec13, vec11, vec12);
			Vec2 vec16 = MBMath.ProjectPointOntoLine(vec14, vec11, vec12);
			Vec2 vec17 = MBMath.ProjectPointOntoLine(vec11, vec13, vec14);
			Vec2 vec18 = MBMath.ProjectPointOntoLine(vec12, vec13, vec14);
			Vec3 f = bodyWorldTransform.rotation.f;
			Vec3 f2 = bodyWorldTransform2.rotation.f;
			int num3 = ((Vec3.DotProduct(f, f2) < 0f) ? (-1) : 1);
			vec15 = MBMath.ClampToAxisAlignedRectangle(vec15, vec11, vec12);
			vec16 = MBMath.ClampToAxisAlignedRectangle(vec16, vec11, vec12);
			vec17 = MBMath.ClampToAxisAlignedRectangle(vec17, vec13, vec14);
			vec18 = MBMath.ClampToAxisAlignedRectangle(vec18, vec13, vec14);
			Vec2 vec19 = (vec12 - vec11).Normalized();
			Vec2 vec20 = (vec14 - vec13).Normalized();
			vec15 += num * vec19 * (float)num3;
			vec16 -= num * vec19 * (float)num3;
			vec17 += num * vec20 * (float)num3;
			vec18 -= num * vec20 * (float)num3;
			if (num3 > 0)
			{
				this._criticalZoneVertices[0] = vec15;
				this._criticalZoneVertices[1] = vec17;
				this._criticalZoneVertices[2] = vec18;
				this._criticalZoneVertices[3] = vec16;
			}
			else
			{
				this._criticalZoneVertices[0] = vec17;
				this._criticalZoneVertices[1] = vec16;
				this._criticalZoneVertices[2] = vec15;
				this._criticalZoneVertices[3] = vec18;
			}
			return this._criticalZoneVertices;
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0004F634 File Offset: 0x0004D834
		public bool GetIsConnectedToEnemy()
		{
			Team team = this.Team;
			if (team != null)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
					bool flag;
					if (currentAttachment == null)
					{
						flag = false;
					}
					else
					{
						ShipAttachmentPointMachine attachmentTarget = currentAttachment.AttachmentTarget;
						bool? flag2;
						if (attachmentTarget == null)
						{
							flag2 = null;
						}
						else
						{
							Team team2 = attachmentTarget.OwnerShip.Team;
							flag2 = ((team2 != null) ? new bool?(team2.IsEnemyOf(team)) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag)
					{
						return true;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
					bool flag5;
					if (currentAttachment2 == null)
					{
						flag5 = false;
					}
					else
					{
						Team team3 = currentAttachment2.AttachmentSource.OwnerShip.Team;
						bool? flag3 = ((team3 != null) ? new bool?(team3.IsEnemyOf(team)) : null);
						bool flag6 = true;
						flag5 = (flag3.GetValueOrDefault() == flag6) & (flag3 != null);
					}
					if (flag5)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x0004F784 File Offset: 0x0004D984
		public bool GetIsConnectedToEnemy(out MissionShip connectedEnemyShip)
		{
			Team team = this.Team;
			if (team != null)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
					bool flag;
					if (currentAttachment == null)
					{
						flag = false;
					}
					else
					{
						ShipAttachmentPointMachine attachmentTarget = currentAttachment.AttachmentTarget;
						bool? flag2;
						if (attachmentTarget == null)
						{
							flag2 = null;
						}
						else
						{
							Team team2 = attachmentTarget.OwnerShip.Team;
							flag2 = ((team2 != null) ? new bool?(team2.IsEnemyOf(team)) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag)
					{
						connectedEnemyShip = shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip;
						return true;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
					bool flag5;
					if (currentAttachment2 == null)
					{
						flag5 = false;
					}
					else
					{
						Team team3 = currentAttachment2.AttachmentSource.OwnerShip.Team;
						bool? flag3 = ((team3 != null) ? new bool?(team3.IsEnemyOf(team)) : null);
						bool flag6 = true;
						flag5 = (flag3.GetValueOrDefault() == flag6) & (flag3 != null);
					}
					if (flag5)
					{
						connectedEnemyShip = shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip;
						return true;
					}
				}
			}
			connectedEnemyShip = null;
			return false;
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0004F90C File Offset: 0x0004DB0C
		public bool GetIsConnectedToEnemyWithSide(out Vec2 direction)
		{
			direction = Vec2.Zero;
			bool flag = false;
			if (this.Team != null)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
					bool flag2;
					if (currentAttachment == null)
					{
						flag2 = false;
					}
					else
					{
						ShipAttachmentPointMachine attachmentTarget = currentAttachment.AttachmentTarget;
						bool? flag3;
						if (attachmentTarget == null)
						{
							flag3 = null;
						}
						else
						{
							Team team = attachmentTarget.OwnerShip.Team;
							flag3 = ((team != null) ? new bool?(team.IsEnemyOf(this.Team)) : null);
						}
						bool? flag4 = flag3;
						bool flag5 = true;
						flag2 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
					}
					if (flag2)
					{
						flag = true;
						MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
						Vec3 vec = shipAttachmentMachine.GameEntity.GlobalPosition;
						Vec2 vec2 = matrixFrame.TransformToLocal(ref vec).AsVec2;
						if (direction.x * vec2.x < 0f)
						{
							direction = Vec2.Zero;
							return true;
						}
						direction += vec2;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
					bool flag6;
					if (currentAttachment2 == null)
					{
						flag6 = false;
					}
					else
					{
						Team team2 = currentAttachment2.AttachmentSource.OwnerShip.Team;
						bool? flag4 = ((team2 != null) ? new bool?(team2.IsEnemyOf(this.Team)) : null);
						bool flag7 = true;
						flag6 = (flag4.GetValueOrDefault() == flag7) & (flag4 != null);
					}
					if (flag6)
					{
						flag = true;
						MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
						Vec3 vec = shipAttachmentPointMachine.GameEntity.GlobalPosition;
						Vec2 vec2 = matrixFrame.TransformToLocal(ref vec).AsVec2;
						if (direction.x * vec2.x < 0f)
						{
							direction = Vec2.Zero;
							return true;
						}
						direction += vec2;
					}
				}
				if (flag)
				{
					direction.Normalize();
				}
				return flag;
			}
			return false;
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x0004FB68 File Offset: 0x0004DD68
		public void OnShipObjectUpdated()
		{
			this._actuators.OnShipObjectUpdated();
			this.InitializeNavalPhysics();
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x0004FB7C File Offset: 0x0004DD7C
		public MBReadOnlyList<MissionShip> GetConnectedShips()
		{
			this._temporaryMissionShipContainer.Clear();
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
				bool flag;
				if (currentAttachment == null)
				{
					flag = null != null;
				}
				else
				{
					ShipAttachmentPointMachine attachmentTarget = currentAttachment.AttachmentTarget;
					flag = ((attachmentTarget != null) ? attachmentTarget.OwnerShip : null) != null;
				}
				if (flag)
				{
					this._temporaryMissionShipContainer.Add(shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip);
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
				bool flag2;
				if (currentAttachment2 == null)
				{
					flag2 = null != null;
				}
				else
				{
					ShipAttachmentMachine attachmentSource = currentAttachment2.AttachmentSource;
					flag2 = ((attachmentSource != null) ? attachmentSource.OwnerShip : null) != null;
				}
				if (flag2)
				{
					this._temporaryMissionShipContainer.Add(shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip);
				}
			}
			return this._temporaryMissionShipContainer;
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x0004FC8C File Offset: 0x0004DE8C
		public int GetDynamicNavmeshIdStart()
		{
			return this.DynamicNavmeshIdStart;
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x0004FC94 File Offset: 0x0004DE94
		public bool GetBridgeWithEnemyActive()
		{
			using (List<ShipAttachmentMachine>.Enumerator enumerator = this._attachmentMachines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsShipAttachmentMachineBridgeWithEnemy())
					{
						return true;
					}
				}
			}
			using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = this._attachmentPointMachines.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.IsShipAttachmentMachinePointBridgeWithEnemy())
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x0004FD34 File Offset: 0x0004DF34
		public bool GetIsAnyBridgeActive()
		{
			using (List<ShipAttachmentMachine>.Enumerator enumerator = this._attachmentMachines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsShipAttachmentMachineBridged())
					{
						return true;
					}
				}
			}
			using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = this._attachmentPointMachines.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.IsShipAttachmentPointBridged())
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x0004FDD4 File Offset: 0x0004DFD4
		public void GetWorldPositionOnDeck(out WorldPosition worldPosition)
		{
			if (this._isCachedWorldPositionOnDeckDirty)
			{
				MatrixFrame globalFrame = this.GlobalFrame;
				MatrixFrame rallyFrame = this.RallyFrame;
				this._cachedWorldPositionOnDeck = ModuleExtensions.ToWorldPosition(ModuleExtensions.ToWorldPosition(globalFrame.TransformToParent(ref rallyFrame).origin).GetNavMeshVec3());
				this._isCachedWorldPositionOnDeckDirty = false;
			}
			worldPosition = this._cachedWorldPositionOnDeck;
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x0004FE30 File Offset: 0x0004E030
		public NavalState GetNavalState(in NavalVec localOffset)
		{
			MatrixFrame globalFrame = this.GlobalFrame;
			Vec2 vec = globalFrame.rotation.s.AsVec2.Normalized();
			Vec2 vec2 = globalFrame.rotation.f.AsVec2.Normalized();
			Vec2 asVec = this.GlobalFrame.origin.AsVec2;
			NavalVec navalVec = localOffset;
			Vec2 vec3 = asVec + navalVec.DeltaPosition.x * vec;
			navalVec = localOffset;
			Vec2 vec4 = vec3 + navalVec.DeltaPosition.y * vec2;
			Vec2 vec5 = vec2;
			navalVec = localOffset;
			vec5.RotateCCW(navalVec.DeltaOrientation);
			float num = Vec2.DotProduct(this._physics.LinearVelocity.AsVec2, vec2);
			float num2 = num;
			navalVec = localOffset;
			num = num2 + navalVec.DeltaSpeed;
			return new NavalState(in vec4, in vec5, num);
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x0004FF24 File Offset: 0x0004E124
		public FacingOrder GetFacingOrderToRallyPoint()
		{
			MatrixFrame matrixFrame;
			if (this.RallyFrame.IsZero)
			{
				matrixFrame = this.GlobalFrame;
			}
			else
			{
				MatrixFrame globalFrame = this.GlobalFrame;
				MatrixFrame rallyFrame = this.RallyFrame;
				matrixFrame = globalFrame.TransformToParent(ref rallyFrame);
			}
			MatrixFrame matrixFrame2 = matrixFrame;
			return FacingOrder.FacingOrderLookAtDirection(matrixFrame2.rotation.f.AsVec2.Normalized());
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0004FF80 File Offset: 0x0004E180
		public MovementOrder GetMovementOrderToRallyPoint()
		{
			MatrixFrame matrixFrame;
			if (this.RallyFrame.IsZero)
			{
				matrixFrame = this.GlobalFrame;
			}
			else
			{
				MatrixFrame globalFrame = this.GlobalFrame;
				MatrixFrame rallyFrame = this.RallyFrame;
				matrixFrame = globalFrame.TransformToParent(ref rallyFrame);
			}
			return MovementOrder.MovementOrderMove(ModuleExtensions.ToWorldPosition(matrixFrame.origin));
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x0004FFCC File Offset: 0x0004E1CC
		public void SetPositioningOrdersToRallyPoint(bool applyToPlayerFormation, bool playersOrder)
		{
			if (applyToPlayerFormation || this.Formation.PlayerOwner != Mission.Current.MainAgent || !this.Formation.HasPlayerControlledTroop)
			{
				MatrixFrame matrixFrame;
				if (this.RallyFrame.IsZero)
				{
					matrixFrame = this.GlobalFrame;
				}
				else
				{
					MatrixFrame globalFrame = this.GlobalFrame;
					MatrixFrame rallyFrame = this.RallyFrame;
					matrixFrame = globalFrame.TransformToParent(ref rallyFrame);
				}
				MatrixFrame matrixFrame2 = matrixFrame;
				WorldPosition worldPosition = ModuleExtensions.ToWorldPosition(matrixFrame2.origin);
				Vec2 vec = matrixFrame2.rotation.f.AsVec2.Normalized();
				this.Formation.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
				this.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
				this.Formation.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(vec));
			}
			if (applyToPlayerFormation)
			{
				this.ShipOrder.JoinPlayerFormationToPlacementDetachment(playersOrder);
			}
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x0005009C File Offset: 0x0004E29C
		public MBReadOnlyList<MissionShip> GetNavmeshConnectedShips()
		{
			this._temporaryMissionShipContainer.Clear();
			ulong num = 0UL;
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.IsNavmeshConnected)
				{
					MissionShip ownerShip = shipAttachmentMachine.CurrentAttachment.AttachmentSource.OwnerShip;
					MissionShip ownerShip2 = shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip;
					if (ownerShip != this)
					{
						if ((num & ownerShip.ShipUniqueBitwiseID) == 0UL)
						{
							this._temporaryMissionShipContainer.Add(ownerShip);
							num |= ownerShip.ShipUniqueBitwiseID;
						}
					}
					else if ((num & ownerShip2.ShipUniqueBitwiseID) == 0UL)
					{
						this._temporaryMissionShipContainer.Add(ownerShip2);
						num |= ownerShip2.ShipUniqueBitwiseID;
					}
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null && shipAttachmentPointMachine.CurrentAttachment.IsNavmeshConnected)
				{
					MissionShip ownerShip3 = shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip;
					MissionShip ownerShip4 = shipAttachmentPointMachine.CurrentAttachment.AttachmentTarget.OwnerShip;
					if (ownerShip3 != this)
					{
						if ((num & ownerShip3.ShipUniqueBitwiseID) == 0UL)
						{
							this._temporaryMissionShipContainer.Add(ownerShip3);
							num |= ownerShip3.ShipUniqueBitwiseID;
						}
					}
					else if ((num & ownerShip4.ShipUniqueBitwiseID) == 0UL)
					{
						this._temporaryMissionShipContainer.Add(ownerShip4);
						num |= ownerShip4.ShipUniqueBitwiseID;
					}
				}
			}
			return this._temporaryMissionShipContainer;
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x0005024C File Offset: 0x0004E44C
		public int ComputeActiveShipAttachmentCount()
		{
			int num = 0;
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
				if (((currentAttachment != null) ? currentAttachment.AttachmentTarget : null) != null)
				{
					num++;
				}
			}
			using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = this._attachmentPointMachines.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.CurrentAttachment != null)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x000502F8 File Offset: 0x0004E4F8
		public void UpdateSailBurningSoundPosition()
		{
			Vec3 vec = Vec3.Zero;
			if (this.Sails.Count > 0)
			{
				foreach (MissionSail missionSail in this.Sails)
				{
					vec += missionSail.CenterOfSailForceShipLocal;
				}
				vec /= (float)this.Sails.Count;
			}
			else
			{
				vec = base.GameEntity.CenterOfMass;
			}
			Vec3 vec2 = base.GameEntity.GetGlobalFrame().TransformToParent(ref vec);
			this.SailBurningSoundEvent.SetPosition(vec2);
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x000503B4 File Offset: 0x0004E5B4
		protected override void OnSaveAsPrefab()
		{
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x000503B8 File Offset: 0x0004E5B8
		public MissionShip GetOutermostConnectedShipFromSide(bool rightSide, out bool effectiveSideOfOutermostShip, ulong aggregateShipUniqueID)
		{
			if ((aggregateShipUniqueID & this.ShipUniqueBitwiseID) != 0UL)
			{
				effectiveSideOfOutermostShip = rightSide;
				return this;
			}
			aggregateShipUniqueID |= this.ShipUniqueBitwiseID;
			MatrixFrame globalFrame = this.GlobalFrame;
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				bool flag = !rightSide;
				MatrixFrame matrixFrame = shipAttachmentMachine.GameEntity.GetGlobalFrame();
				if (flag ^ (globalFrame.TransformToLocal(ref matrixFrame).origin.x > 0f))
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
					if (((currentAttachment != null) ? currentAttachment.AttachmentTarget : null) != null)
					{
						MissionShip ownerShip = shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip;
						MissionShip missionShip = ownerShip;
						Vec2 vec = globalFrame.rotation.f.AsVec2;
						matrixFrame = ownerShip.GlobalFrame;
						return missionShip.GetOutermostConnectedShipFromSide((vec.DotProduct(matrixFrame.rotation.f.AsVec2) >= 0f) ? rightSide : (!rightSide), out effectiveSideOfOutermostShip, aggregateShipUniqueID);
					}
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				bool flag2 = !rightSide;
				MatrixFrame matrixFrame = shipAttachmentPointMachine.GameEntity.GetGlobalFrame();
				if (flag2 ^ (globalFrame.TransformToLocal(ref matrixFrame).origin.x > 0f))
				{
					ShipAttachmentMachine.ShipAttachment currentAttachment2 = shipAttachmentPointMachine.CurrentAttachment;
					if (((currentAttachment2 != null) ? currentAttachment2.AttachmentSource : null) != null)
					{
						MissionShip ownerShip2 = shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip;
						MissionShip missionShip2 = ownerShip2;
						Vec2 vec = globalFrame.rotation.f.AsVec2;
						matrixFrame = ownerShip2.GlobalFrame;
						return missionShip2.GetOutermostConnectedShipFromSide((vec.DotProduct(matrixFrame.rotation.f.AsVec2) >= 0f) ? rightSide : (!rightSide), out effectiveSideOfOutermostShip, aggregateShipUniqueID);
					}
				}
			}
			effectiveSideOfOutermostShip = rightSide;
			return this;
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x000505C8 File Offset: 0x0004E7C8
		protected override void OnFixedTick(float fixedDt)
		{
			if (!this._isRemoved)
			{
				this.ProcessDetanglingShips();
			}
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x000505D8 File Offset: 0x0004E7D8
		protected override void OnInit()
		{
			this._attachmentMachines = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<ShipAttachmentMachine>(base.GameEntity);
			this._attachmentPointMachines = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<ShipAttachmentPointMachine>(base.GameEntity);
			this.InitializeLists(false);
			base.GameEntity.SetHasCustomBoundingBoxValidationSystem(true);
			base.OnInit();
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x00050624 File Offset: 0x0004E824
		protected override void OnBoundingBoxValidate()
		{
			base.GameEntity.RelaxLocalBoundingBox(ref this._localBoundingBoxCached);
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x00050648 File Offset: 0x0004E848
		public bool GetIsAgentOnShip(Agent agent, bool bypassSteppedShipCheck = false)
		{
			if (!bypassSteppedShipCheck)
			{
				if (!agent.IsInWater())
				{
					AgentNavalComponent component = agent.GetComponent<AgentNavalComponent>();
					if (((component != null) ? component.SteppedShip : null) != null)
					{
						goto IL_0021;
					}
				}
				return false;
			}
			IL_0021:
			int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
			return this.IsAgentOnShipNavmesh(currentNavigationFaceId);
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x00050684 File Offset: 0x0004E884
		public bool GetNextCrewSpawnGlobalFrame(out MatrixFrame crewSpawnGlobalFrame)
		{
			if (this._crewSpawnLocalFrames != null && !Extensions.IsEmpty<MatrixFrame>(this._crewSpawnLocalFrames))
			{
				int nextCrewSpawnFrameIndex = this._nextCrewSpawnFrameIndex;
				this._nextCrewSpawnFrameIndex = (this._nextCrewSpawnFrameIndex + 1) % this._crewSpawnLocalFrames.Count;
				MatrixFrame globalFrame = this.GlobalFrame;
				MatrixFrame matrixFrame = this._crewSpawnLocalFrames[nextCrewSpawnFrameIndex];
				crewSpawnGlobalFrame = globalFrame.TransformToParent(ref matrixFrame);
				return true;
			}
			crewSpawnGlobalFrame = MatrixFrame.Identity;
			return false;
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x000506F8 File Offset: 0x0004E8F8
		public MatrixFrame GetNextOuterInnerSpawnGlobalFrame()
		{
			int nextDeckSpawnFrameIndex = this._nextDeckSpawnFrameIndex;
			this._nextDeckSpawnFrameIndex = (this._nextDeckSpawnFrameIndex + 1) % this.DeckFrameCount;
			MatrixFrame globalFrame = this.GlobalFrame;
			MatrixFrame matrixFrame = ((nextDeckSpawnFrameIndex >= this.OuterDeckLocalFrames.Count) ? this.InnerDeckLocalFrames[nextDeckSpawnFrameIndex - this.OuterDeckLocalFrames.Count] : this.OuterDeckLocalFrames[nextDeckSpawnFrameIndex]);
			return globalFrame.TransformToParent(ref matrixFrame);
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x00050768 File Offset: 0x0004E968
		public MatrixFrame GetMiddleInnerSpawnGlobalFrame()
		{
			MatrixFrame globalFrame = this.GlobalFrame;
			MatrixFrame matrixFrame = this.InnerDeckLocalFrames[this.InnerDeckLocalFrames.Count / 2];
			return globalFrame.TransformToParent(ref matrixFrame);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x000507A0 File Offset: 0x0004E9A0
		public MatrixFrame GetCaptainSpawnGlobalFrame()
		{
			MatrixFrame globalFrame = this.GlobalFrame;
			MatrixFrame matrixFrame = this.InnerDeckLocalFrames[this.InnerDeckLocalFrames.Count - 1];
			return globalFrame.TransformToParent(ref matrixFrame);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x000507D8 File Offset: 0x0004E9D8
		public NavalState GetNavalState()
		{
			MatrixFrame globalFrame = this.GlobalFrame;
			Vec2 vec = globalFrame.rotation.f.AsVec2;
			Vec2 vec2 = vec.Normalized();
			float num = Vec2.DotProduct(this._physics.LinearVelocity.AsVec2, vec2);
			vec = globalFrame.origin.AsVec2;
			return new NavalState(in vec, in vec2, num);
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x00050838 File Offset: 0x0004EA38
		public bool GetIsThereActiveBridgeTo(MissionShip targetShip)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this.AttachmentMachines)
			{
				if (shipAttachmentMachine.IsShipAttachmentMachineBridged() && shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip == targetShip)
				{
					return true;
				}
			}
			foreach (ShipAttachmentMachine shipAttachmentMachine2 in targetShip.AttachmentMachines)
			{
				if (shipAttachmentMachine2.IsShipAttachmentMachineBridged() && shipAttachmentMachine2.CurrentAttachment.AttachmentTarget.OwnerShip == this)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x00050900 File Offset: 0x0004EB00
		public MBReadOnlyList<MissionShip> GetFullyConnectedShips()
		{
			this._temporaryMissionShipContainer.Clear();
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected && this._temporaryMissionShipContainer.IndexOf(shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip) < 0)
				{
					this._temporaryMissionShipContainer.Add(shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip);
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null && shipAttachmentPointMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected && this._temporaryMissionShipContainer.IndexOf(shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip) < 0)
				{
					this._temporaryMissionShipContainer.Add(shipAttachmentPointMachine.CurrentAttachment.AttachmentSource.OwnerShip);
				}
			}
			return this._temporaryMissionShipContainer;
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x00050A38 File Offset: 0x0004EC38
		public void EnableBlockers()
		{
			base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 49, true);
			base.GameEntity.Scene.SetBlockerDirectionForFacesWithId(this.DynamicNavmeshIdStart + 49, base.GameEntity.GetGlobalFrame().rotation.f.AsVec2.RotationInRadians);
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x00050AA8 File Offset: 0x0004ECA8
		protected override void AttachDynamicNavmeshToEntity()
		{
			if (Mission.Current != null && this.NavMeshPrefabName.Length > 0)
			{
				this.DynamicNavmeshIdStart = Mission.Current.GetNextDynamicNavMeshIdStart();
				base.GameEntity.Scene.ImportNavigationMeshPrefab(this.NavMeshPrefabName, this.DynamicNavmeshIdStart);
				this.AttachDynamicNavmeshFromMachines(this._attachmentMachines, this._attachmentPointMachines);
				if (Mission.Current.MissionTeamAIType == 5)
				{
					string text = Extensions.Add(this.NavMeshPrefabName, "_deactivator_dnm", true);
					text = text.Remove(text.Length - 1);
					base.GameEntity.Scene.ImportNavigationMeshPrefab(text, this.DynamicNavmeshIdStart + 45);
					this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 49, false, true, false, true, true);
					base.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 49, false);
				}
			}
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x00050B98 File Offset: 0x0004ED98
		public MissionSail CheckHitSails(Agent attackerAgent, Mission.Missile missile, in Vec3 missileOldPosition, in Vec3 missilePosition, in MissionWeapon missileWeapon)
		{
			bool flag = false;
			if (!base.IsDisabled && (flag || (this.Team != null && this.Team.IsEnemyOf(attackerAgent.Team))))
			{
				MissionWeapon missionWeapon = missileWeapon;
				if (missionWeapon.CurrentUsageItem != null)
				{
					missionWeapon = missileWeapon;
					if (Extensions.HasAnyFlag<WeaponFlags>(missionWeapon.CurrentUsageItem.WeaponFlags, 32768L))
					{
						foreach (MissionSail missionSail in this.Sails)
						{
							if (missile.AlreadyHitEntityToIgnore != missionSail.SailEntity && missionSail.GetVisualSailEnabled() && missionSail.IntersectLineSegmentWithSail(in missileOldPosition, in missilePosition))
							{
								return missionSail;
							}
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x00050C78 File Offset: 0x0004EE78
		protected override bool OnHit(Agent attackerAgent, int inflictedDamage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, int affectorWeaponSlotOrMissileIndex, ScriptComponentBehavior attackerScriptComponentBehavior, out bool reportDamage, out float finalDamage, out float fireDamage, out float modifiedFireDamage)
		{
			reportDamage = false;
			finalDamage = 0f;
			fireDamage = -1f;
			modifiedFireDamage = -1f;
			bool flag = false;
			if (!Mission.Current.DisableDying && Mission.Current.Mode != 1 && Mission.Current.Mode != 9 && !base.IsDisabled)
			{
				MissionWeapon missionWeapon = weapon;
				if (missionWeapon.CurrentUsageItem != null && (flag || (this.Team != null && this.Team.IsEnemyOf(attackerAgent.Team))))
				{
					missionWeapon = weapon;
					bool flag2 = Extensions.HasAnyFlag<WeaponFlags>(missionWeapon.CurrentUsageItem.WeaponFlags, 32768L);
					bool flag3 = this.ShipsLogic.IsMissileFromShipSiegeEngine(affectorWeaponSlotOrMissileIndex);
					float missileVelocityLengthOnFirstSailHit = this.ShipsLogic.GetMissileVelocityLengthOnFirstSailHit(affectorWeaponSlotOrMissileIndex);
					bool flag4 = missileVelocityLengthOnFirstSailHit >= 0f;
					if (flag3)
					{
						float num = (float)inflictedDamage;
						inflictedDamage = MissionGameModels.Current.MissionSiegeEngineCalculationModel.CalculateDamage(attackerAgent, num);
						int num2;
						int num3;
						DamageTypes damageTypes;
						bool flag5;
						finalDamage = this.DealDamage((float)inflictedDamage, null, out num2, out num3, out damageTypes, out flag5);
						reportDamage = true;
					}
					if (flag2)
					{
						missionWeapon = weapon;
						fireDamage = (float)missionWeapon.CurrentUsageItem.FireDamage;
						Vec3 vec;
						if (flag4 && Mission.Current.TryGetMissileVelocityFromMissileIndex(affectorWeaponSlotOrMissileIndex, ref vec))
						{
							fireDamage *= vec.Length / missileVelocityLengthOnFirstSailHit;
						}
						using (List<MissionSail>.Enumerator enumerator = this.Sails.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.GetVisualSailEnabled())
								{
									float num4 = MissionGameModels.Current.AgentApplyDamageModel.CalculateSailFireDamage(attackerAgent, this.ShipOrigin, fireDamage, true);
									this.DealDamageToSails(attackerAgent, fireDamage, num4, null);
									break;
								}
							}
						}
						if (this.FireHitPoints > 0f)
						{
							float num5 = this.DealFireDamage(fireDamage);
							modifiedFireDamage = num5 - fireDamage;
							if (num5 > 40f)
							{
								ShipBurningSystem firstScriptOfTypeRecursive = base.GameEntity.GetFirstScriptOfTypeRecursive<ShipBurningSystem>();
								if (firstScriptOfTypeRecursive != null)
								{
									firstScriptOfTypeRecursive.RegisterBlow(impactPosition);
								}
							}
							reportDamage = true;
							if (this.FireHitPoints <= 0f)
							{
								this.DealDamageToSails(attackerAgent, this.SailHitPoints, this.SailHitPoints, null);
								this.PrepareForAbandonment();
								ShipBurningSystem firstScriptOfTypeRecursive2 = base.GameEntity.GetFirstScriptOfTypeRecursive<ShipBurningSystem>();
								if (firstScriptOfTypeRecursive2 != null)
								{
									firstScriptOfTypeRecursive2.StartFire();
								}
								this.ShipsLogic.OnShipBurned(this);
							}
						}
					}
				}
			}
			this.ShipsLogic.OnShipHit(this, attackerAgent, (int)finalDamage, impactPosition, impactDirection, in weapon, affectorWeaponSlotOrMissileIndex);
			return true;
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x00050EF8 File Offset: 0x0004F0F8
		public float DealFireDamage(float fireDamage)
		{
			float num = MissionGameModels.Current.AgentApplyDamageModel.CalculateHullFireDamage(fireDamage, this.ShipOrigin);
			this.FireHitPoints -= num;
			return num;
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x00050F2C File Offset: 0x0004F12C
		public void PrepareForAbandonment()
		{
			this.BeingAbandoned = true;
			foreach (UsableMachine usableMachine in MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<UsableMachine>(base.GameEntity))
			{
				if (!(usableMachine is ShipAttachmentPointMachine))
				{
					usableMachine.SetIsDisabledForAI(true);
					usableMachine.SetScriptComponentToTick(usableMachine.GetTickRequirement());
				}
				foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
				{
					standingPoint.SetIsDisabledForPlayersSynched(true);
				}
			}
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenRecursive(ref list);
			foreach (WeakGameEntity weakGameEntity in list)
			{
				if (Extensions.HasAnyFlag<BodyFlags>(weakGameEntity.BodyFlag, 512) || Extensions.HasAnyFlag<BodyFlags>(weakGameEntity.BodyFlag, 1024) || Extensions.HasAnyFlag<BodyFlags>(weakGameEntity.BodyFlag, 256))
				{
					weakGameEntity.SetVisibilityExcludeParents(false);
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this.AttachmentPointMachines)
			{
				shipAttachmentPointMachine.SetEnemyRangeToStopUsing(0f);
				shipAttachmentPointMachine.SetIsDisabledForAI(false);
				foreach (StandingPoint standingPoint2 in shipAttachmentPointMachine.StandingPoints)
				{
					if (standingPoint2 == shipAttachmentPointMachine.PilotStandingPoint)
					{
						standingPoint2.LockUserFrames = true;
					}
				}
				foreach (GameEntity gameEntity in shipAttachmentPointMachine.RampPhysicsList)
				{
					gameEntity.SetVisibilityExcludeParents(true);
				}
			}
			this.ShipOrder.StopUsingMachines(false);
			this.IsShipOrderActive = false;
			this.SetController(ShipControllerType.None, true);
			this.ShipsLogic.OnShipPreparedForAbandonment(this);
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x00051180 File Offset: 0x0004F380
		protected override void OnTick(float dt)
		{
			this._isCachedWorldPositionOnDeckDirty = true;
			if (!this._isRemoved)
			{
				if (Mission.Current.IsDeploymentFinished)
				{
					if (this._autoUpdateController)
					{
						this.UpdateController();
					}
					if (this.IsShipOrderActive)
					{
						this.ShipOrder.Tick();
					}
					else if (this.IsClimbingMachineStandAloneTickingActive)
					{
						this.ShipOrder.TickClimbingMachines();
					}
				}
				if (this.HasController)
				{
					this._inputRecord = this.Controller.Update(dt);
				}
				if (this.HasCustomSailSetting)
				{
					this._inputRecord.SetSail(this._customSailSetting);
				}
				if (this._inputRecord.Sail != SailInput.Raised && this._foldSailsOnBridgeConnection && this.HasThrownOrActiveBridgeConnections())
				{
					this._inputRecord.SetSail(SailInput.Raised);
				}
				this.HandleCapsizing();
				float num = MathF.Max(this._physics.PhysicsBoundingBoxWithoutChildren.max.z, this._physics.PhysicsBoundingBoxSizeWithoutChildren.y * 0.5f);
				Vec3 globalPosition = base.GameEntity.GlobalPosition;
				if (this.Physics.NavalSinkingState == NavalPhysics.SinkingState.Sinking && globalPosition.z + num < Mission.Current.Scene.GetWaterLevelAtPosition(globalPosition.AsVec2, true, false))
				{
					this.SetSinkingState(NavalPhysics.SinkingState.Sunk);
					this.ShipSailState = MissionShip.SailState.Destroyed;
					if (this.SailBurningSoundEvent.IsPlaying())
					{
						this.SailBurningSoundEvent.Stop();
					}
					this.ShipsLogic.OnShipSunk(this);
				}
				bool flag = this.IsShipUpsideDown();
				if (flag != this.IsShipNavmeshDisabled)
				{
					this.SetAbilityOfShipNavmeshFaces(!flag);
					this.IsShipNavmeshDisabled = flag;
					this.ShipOrder.ManageShipDetachments();
				}
				this.UpdateSailBurningSoundPosition();
				if (this.ShipSailState == MissionShip.SailState.Burning)
				{
					bool flag2 = true;
					using (List<MissionSail>.Enumerator enumerator = this.Sails.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.IsBurningFinished())
							{
								flag2 = false;
							}
						}
					}
					if (flag2)
					{
						this.ShipSailState = MissionShip.SailState.Destroyed;
						if (this.SailBurningSoundEvent.IsPlaying())
						{
							this.SailBurningSoundEvent.Stop();
						}
						this.ShipsLogic.OnSailsDead(this);
					}
				}
				if (this.FireHitPoints <= 0f && this.BurntHullDamageTotal < this.MaxHealth * 0.5f && !this.IsSinking && Mission.Current.CurrentTime > this._nextPermanentBurnDamageTime && !Mission.Current.DisableDying && Mission.Current.Mode != 1 && Mission.Current.Mode != 9)
				{
					int num2;
					int num3;
					DamageTypes damageTypes;
					bool flag3;
					this.BurntHullDamageTotal += this.DealDamage(this.MaxHealth * 0.03f, null, out num2, out num3, out damageTypes, out flag3);
					this._nextPermanentBurnDamageTime = Mission.Current.CurrentTime + 1f;
				}
				if (this.FireHitPoints > 0f && this.FireHitPoints < this.MaxFireHealth && Mission.Current.CurrentTime > this._nextFireHitPointRestoreTime)
				{
					this.FireHitPoints += this.MaxFireHealth * 0.005f;
					if (this.FireHitPoints > this.MaxFireHealth)
					{
						this.FireHitPoints = this.MaxFireHealth;
					}
					this._nextFireHitPointRestoreTime = Mission.Current.CurrentTime + 1f;
				}
				if (this.IsDisconnectionBlocked())
				{
					bool flag4 = false;
					using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = this.AttachmentPointMachines.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.IsShipAttachmentPointConnectedToEnemy())
							{
								flag4 = true;
								break;
							}
						}
					}
					if (!flag4)
					{
						this.ResetDisconnectionBlock();
					}
				}
				this.HandleQueuedShipCollisions();
			}
			this._actuators.Update(dt);
			if (this._localBoundingBoxCacheInvalid)
			{
				this.ComputeStaticLocalBoundingBox();
				base.GameEntity.RecomputeBoundingBox();
				this._localBoundingBoxCacheInvalid = false;
			}
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x00051548 File Offset: 0x0004F748
		protected override void OnParallelFixedTick(float fixedDt)
		{
			this.ShouldUpdateSoundPos = SoundManager.GetListenerFrame().origin.DistanceSquared(base.GameEntity.GetBodyWorldTransform().origin) < 10000f;
			ShipActuatorRecord shipActuatorRecord = this._inputProcessor.OnParallelFixedTick(fixedDt, in this._inputRecord);
			ShipForceRecord shipForceRecord = this._actuators.OnParallelFixedTick(fixedDt, in shipActuatorRecord);
			this._physics.SetShipForceRecord(in shipForceRecord);
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x000515B8 File Offset: 0x0004F7B8
		protected override void OnPhysicsCollision(ref PhysicsContact contactPairList, WeakGameEntity entity0, WeakGameEntity entity1)
		{
			SoundEvent soundEvent = null;
			Vec3 vec;
			vec..ctor(0f, 0f, 0f, -1f);
			Vec3 vec2;
			vec2..ctor(0f, 0f, 0f, -1f);
			int num = 0;
			int num2 = -1;
			bool flag = false;
			StackArray.StackArray3Int stackArray3Int = default(StackArray.StackArray3Int);
			Vec3 vec3 = Vec3.Zero;
			for (int i = 0; i < contactPairList.NumberOfContactPairs; i++)
			{
				int contactEventType = contactPairList[i].ContactEventType;
				int num3 = stackArray3Int[contactEventType];
				stackArray3Int[contactEventType] = num3 + 1;
				for (int j = 0; j < contactPairList[i].NumberOfContacts; j++)
				{
					num++;
					vec += contactPairList[i][j].Position;
					vec2 += contactPairList[i][j].Normal;
					vec3 += contactPairList[i][j].Impulse;
				}
			}
			int num4 = -1;
			for (int k = this._currentCollisionStatesToShips.Count - 1; k >= 0; k--)
			{
				if (this._currentCollisionStatesToShips[k].CollidingEntity != null && this._currentCollisionStatesToShips[k].CollidingEntity.Scene == null)
				{
					this._currentCollisionStatesToShips.RemoveAt(k);
					if (num4 >= 0)
					{
						num4--;
					}
				}
				else if ((this._currentCollisionStatesToShips[k].CollidingEntity != null && entity1 != null && this._currentCollisionStatesToShips[k].CollidingEntity.Root == entity1.Root) || (this._currentCollisionStatesToShips[k].CollidingEntity == null && entity1 == null && this._currentCollisionStatesToShips[k].CollidingBodyPtr == contactPairList.body1))
				{
					num4 = k;
				}
			}
			MissionShip missionShip = ((entity1 != null) ? (entity1.GetFirstScriptWithNameHash(MissionShip.MissionShipScriptNameHash) as MissionShip) : null);
			PhysicsEventType physicsEventType = 2;
			if (contactPairList.NumberOfContactPairs > 0)
			{
				if (num4 >= 0)
				{
					physicsEventType = this._currentCollisionStatesToShips[num4].CurrentCollisionState;
				}
				switch (physicsEventType)
				{
				case 0:
					if (stackArray3Int[1] > 0)
					{
						physicsEventType = 1;
					}
					else if (stackArray3Int[0] == 0 && stackArray3Int[2] > 0)
					{
						physicsEventType = 2;
					}
					break;
				case 1:
					if (stackArray3Int[0] == 0 && stackArray3Int[1] == 0)
					{
						physicsEventType = 2;
					}
					break;
				case 2:
					if (stackArray3Int[0] > 0 || stackArray3Int[1] > 0)
					{
						physicsEventType = 0;
					}
					break;
				}
				if (num4 >= 0)
				{
					if (physicsEventType == 2)
					{
						this._currentCollisionStatesToShips.RemoveAt(num4);
						num4 = -1;
					}
					else
					{
						this._currentCollisionStatesToShips[num4].UpdateCurrentCollisionState(physicsEventType);
					}
				}
				else if (physicsEventType != 2)
				{
					if (entity1 != null)
					{
						this._currentCollisionStatesToShips.Add(new MissionShip.ShipToEntityCollisionStatus(entity1.Root, physicsEventType));
					}
					else
					{
						this._currentCollisionStatesToShips.Add(new MissionShip.ShipToEntityCollisionStatus(contactPairList.body1, physicsEventType));
					}
				}
				flag = physicsEventType != 2 && missionShip != null;
			}
			vec /= (float)num;
			vec2 /= (float)num;
			if (num4 != -1 && missionShip != null)
			{
				PhysicsEventType currentCollisionState = this._currentCollisionStatesToShips[num4].CurrentCollisionState;
				if (currentCollisionState != 1)
				{
					if (currentCollisionState == 2)
					{
						this.RemoveDetanglingShip(missionShip);
					}
				}
				else
				{
					this.AddDetanglingShip(missionShip, vec);
				}
			}
			if (missionShip != null)
			{
				for (int l = 0; l < this._scrapeSoundEvents.Count; l++)
				{
					if (this._scrapeSoundEvents[l].Item1 == missionShip.Index)
					{
						soundEvent = this._scrapeSoundEvents[l].Item2;
						num2 = l;
						break;
					}
				}
				if (flag)
				{
					if (soundEvent == null)
					{
						soundEvent = SoundEvent.CreateEvent(MissionShip._scrapeSoundEventID, base.GameEntity.Scene);
						this._scrapeSoundEvents.Add(new ValueTuple<int, SoundEvent>(missionShip.Index, soundEvent));
						missionShip._scrapeSoundEvents.Add(new ValueTuple<int, SoundEvent>(this.Index, soundEvent));
					}
					if (!soundEvent.IsPlaying())
					{
						soundEvent.Play();
					}
					Vec3 vec4 = vec2.CrossProductWithUp();
					float num5 = Vec3.DotProduct(vec4, this.Physics.LinearVelocity);
					float num6 = Vec3.DotProduct(vec4, missionShip.Physics.LinearVelocity);
					float num7 = MathF.Min(MathF.Abs(num5 - num6) / 10f, 1f);
					soundEvent.SetParameter("ForceContinuous", num7);
					soundEvent.SetPosition(vec);
					if (!this.IsPlayerControlled && !missionShip.IsPlayerControlled)
					{
						soundEvent.SetParameter("VibrationSend", 0f);
					}
				}
				else
				{
					if (soundEvent != null && soundEvent.IsPlaying())
					{
						soundEvent.Stop();
					}
					if (num2 != -1 && num2 < this._scrapeSoundEvents.Count)
					{
						this._scrapeSoundEvents.RemoveAt(num2);
					}
				}
			}
			if (num > 0 && this.ShipsLogic != null)
			{
				bool flag2 = num4 < 0 && physicsEventType == 0;
				if (flag2 && missionShip != null && this.CanDealDamage(missionShip))
				{
					MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
					MatrixFrame bodyWorldTransform2 = missionShip.GameEntity.GetBodyWorldTransform();
					Vec3 vec5 = bodyWorldTransform.TransformToParent(ref this.Physics.PhysicsBoundingBoxWithoutChildren.center);
					Vec3 vec6 = vec - vec5;
					vec6.z = 0f;
					vec6.Normalize();
					float num8 = Vec3.DotProduct(vec6, bodyWorldTransform.rotation.f);
					if (num8 > 0f && MathF.Acos(num8) * 57.295776f < this.MissionShipObject.BowAngleLimitFromCenterline)
					{
						Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity, vec);
						Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody2 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(missionShip.GameEntity, vec);
						linearVelocityAtGlobalPointForEntityWithDynamicBody - linearVelocityAtGlobalPointForEntityWithDynamicBody2;
						Vec3 vec7 = bodyWorldTransform.origin + Vec3.DotProduct(vec - bodyWorldTransform.origin, bodyWorldTransform.rotation.f) * bodyWorldTransform.rotation.f;
						Vec3 vec8 = (bodyWorldTransform2.origin + Vec3.DotProduct(vec - bodyWorldTransform2.origin, bodyWorldTransform2.rotation.f) * bodyWorldTransform2.rotation.f - vec7).NormalizedCopy();
						float num9 = Vec3.DotProduct(linearVelocityAtGlobalPointForEntityWithDynamicBody - linearVelocityAtGlobalPointForEntityWithDynamicBody2, vec8);
						if (num9 >= 3f)
						{
							float num10 = 12f * (float)Math.Sqrt((double)(this.Physics.Mass / 500f)) * 0.8f * num9;
							missionShip.QueueShipCollision(this, vec, num10);
							this.QueueShipCollision(missionShip, vec, num10 * 0.2f);
							this.UpdateDamageCooldown(missionShip);
						}
					}
				}
				this.ShipsLogic.OnShipCollision(this, entity1, contactPairList.body1Flags, vec, vec3, flag2);
			}
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x00051CE4 File Offset: 0x0004FEE4
		private void HandleQueuedShipCollisions()
		{
			MissionShip.ShipCollisionData shipCollisionData;
			while (this._shipCollisionData.TryDequeue(out shipCollisionData))
			{
				this.DealCollisionDamage(shipCollisionData.CollidingShip, false, shipCollisionData.ContactPosAvg, shipCollisionData.Damage);
			}
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x00051D1B File Offset: 0x0004FF1B
		private void QueueShipCollision(MissionShip collidingShip, Vec3 contactPosAvg, float damage)
		{
			this._shipCollisionData.Enqueue(new MissionShip.ShipCollisionData(collidingShip, contactPosAvg, damage));
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x00051D30 File Offset: 0x0004FF30
		public bool CanDealDamage(MissionShip targetShip)
		{
			float currentTime = Mission.Current.CurrentTime;
			float num;
			return !this._shipDamageCooldowns.TryGetValue(targetShip, out num) || currentTime - num >= 2f;
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x00051D68 File Offset: 0x0004FF68
		public void UpdateDamageCooldown(MissionShip targetShip)
		{
			float currentTime = Mission.Current.CurrentTime;
			this._shipDamageCooldowns[targetShip] = currentTime;
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x00051D8D File Offset: 0x0004FF8D
		protected override bool OnCheckForProblems()
		{
			this.InitializeLists(true);
			return false;
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x00051D98 File Offset: 0x0004FF98
		internal void InitForMission(int shipIndex, ulong shipUniqueBitwiseID, ShipAssignment shipAssignment, NavalShipsLogic shipsLogic)
		{
			if (!this.IsInitialized)
			{
				this.ShipsLogic = shipsLogic;
				this.ValidateShipAndDescendantEntitiesAndBoundingBoxes();
				this.Index = shipIndex;
				MissionShip.MaxShipIndex = MathF.Max(this.Index, MissionShip.MaxShipIndex);
				this._shields = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<ShipShieldComponent>(base.GameEntity);
				base.GameEntity.Scene.SetFixedTickCallbackActive(true);
				base.GameEntity.Scene.SetOnCollisionFilterCallbackActive(true);
				this._missionShipObject = shipAssignment.MissionShipObject;
				this.ShipOrigin = shipAssignment.ShipOrigin;
				this.ShipSailState = ((this.ShipOrigin.SailHitPoints > 250f) ? MissionShip.SailState.Intact : MissionShip.SailState.Destroyed);
				this.FireHitPoints = this.ShipOrigin.MaxFireHitPoints;
				GameEntity gameEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.CollectChildrenEntitiesWithTag("rally_point")[0]);
				MatrixFrame globalFrame = this.GlobalFrame;
				MatrixFrame globalFrame2 = gameEntity.GetGlobalFrame();
				this.RallyFrame = globalFrame.TransformToLocal(ref globalFrame2);
				this.LoadSpawnPoints();
				this.LoadShipBanners();
				this._capsizeDamageTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
				MBList<ClimbingMachine> mblist = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<ClimbingMachine>(base.GameEntity);
				this.ClimbingMachineDetachment = new ClimbingMachineDetachment(ref mblist);
				Team team = Mission.GetTeam(shipAssignment.TeamSide);
				this.Formation = team.GetFormation(shipAssignment.FormationIndex);
				this._inputRecord = ShipInputRecord.None();
				this.Formation.OnUnitRemoved += this.OnFormationUnitRemoved;
				this.SetController(ShipControllerType.AI, true);
				this._inputProcessor = new ShipInputProcessor(this);
				this._actuators = new ShipActuators(this);
				foreach (MissionSail missionSail in this._actuators.Sails)
				{
					missionSail.ForceFold();
				}
				this.InitializeNavalPhysics();
				this._visitedMissionShips = new HashSet<MissionShip>();
				this.InitializeDetanglingShipInformation();
				this.InitializeLocalPhysicsBoundingXYPlane();
				this._physicsBoundingBoxXYPlaneVertices = new Vec2[4];
				this._criticalZoneVertices = new Vec2[4];
				float num = this.MaxPartialHealth - (this.MaxHealth - this.HitPoints) / 6f;
				this._partialHitPoints = Enumerable.Repeat<float>(num, 6).ToArray<float>();
				this.InitializePartialDurabilities();
				this._moraleInteractionLogic = Mission.Current.GetMissionBehavior<NavalAgentMoraleInteractionLogic>();
				this.ShipsLogic.ShipSpawnedEvent += this.OnShipSpawned;
				this.ShipsLogic.ShipTransferredToFormationEvent += this.OnShipTransferred;
				this.ShipsLogic.ShipRemovedEvent += this.OnShipRemoved;
				this.ShipOrder = new ShipOrder(this, this.Formation);
				this.ResetFormationPositioning();
				this._scrapeSoundEvents = new MBList<ValueTuple<int, SoundEvent>>();
				int eventIdFromString = SoundEvent.GetEventIdFromString("event:/mission/ambient/detail/fire/fire_big");
				this.SailBurningSoundEvent = SoundEvent.CreateEvent(eventIdFromString, Mission.Current.Scene);
				this.UpdateSailBurningSoundPosition();
				RangedSiegeWeapon shipSiegeWeapon = this.ShipSiegeWeapon;
				if (shipSiegeWeapon != null)
				{
					shipSiegeWeapon.SetForcedUse(false);
				}
				this.InitializeShipBoundingBox();
				this._shipEventListeners = new MBList<IShipEventListener>();
				using (List<ScriptComponentBehavior>.Enumerator enumerator2 = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<ScriptComponentBehavior>(base.GameEntity).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IShipEventListener shipEventListener;
						if ((shipEventListener = enumerator2.Current as IShipEventListener) != null)
						{
							this._shipEventListeners.Add(shipEventListener);
						}
					}
				}
				this.ShipUniqueBitwiseID = shipUniqueBitwiseID;
				this.ShipIslandCombinedID = this.ShipUniqueBitwiseID;
				this.Formation.OnUnitAttached += this.OnUnitAttached;
				if (!base.GameEntity.IsInEditorScene())
				{
					this.ClearFloaterVolumes();
					WeakGameEntity firstChildEntityWithName = MBExtensions.GetFirstChildEntityWithName(base.GameEntity, "knobs_holder");
					if (firstChildEntityWithName.IsValid)
					{
						firstChildEntityWithName.SetEntityFlags(firstChildEntityWithName.EntityFlags | 536870912);
					}
					WeakGameEntity firstChildEntityWithName2 = MBExtensions.GetFirstChildEntityWithName(base.GameEntity, "brazier_holder");
					if (firstChildEntityWithName2.IsValid)
					{
						firstChildEntityWithName2.SetEntityFlags(firstChildEntityWithName2.EntityFlags | 536870912);
					}
					List<WeakGameEntity> list = new List<WeakGameEntity>();
					base.GameEntity.GetChildrenRecursive(ref list);
					foreach (WeakGameEntity weakGameEntity in list)
					{
						weakGameEntity.SetForceDecalsToRender(true);
						weakGameEntity.SetForceNotAffectedBySeason(true);
					}
				}
				this._anyActiveFormationTroopOnShip.Expire();
				return;
			}
			Debug.FailedAssert("The ship is already initialized", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Objects\\MissionShip.cs", "InitForMission", 3173);
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x0005222C File Offset: 0x0005042C
		private void OnFormationUnitRemoved(Formation formation, Agent agent)
		{
			if (this.BattleSide != Mission.Current.PlayerTeam.Side && formation.CountOfUnits == 0)
			{
				this.ShipControllerMachine.PilotStandingPoint.SetUsableByPlayerOnly();
			}
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x00052260 File Offset: 0x00050460
		private void InitializeNavalPhysics()
		{
			ShipPhysicsReference physicsReference = this._missionShipObject.PhysicsReference;
			NavalPhysics.NavalPhysicsParameters navalPhysicsParameters = new NavalPhysics.NavalPhysicsParameters
			{
				OverrideMass = this._missionShipObject.Mass,
				MassMultiplier = 1f + ((this.ShipOrigin != null) ? this.ShipOrigin.ShipWeightFactor : 0f),
				MomentOfInertiaMultiplier = this._missionShipObject.MomentOfInertiaMultiplier,
				FloatingForceMultiplier = this._missionShipObject.FloatingForceMultiplier,
				MaximumSubmergedVolumeRatio = this._missionShipObject.MaximumSubmergedVolumeRatio,
				ForwardDragMultiplier = 1f + ((this.ShipOrigin != null) ? this.ShipOrigin.ForwardDragFactor : 0f),
				LinearFrictionMultiplier = this._missionShipObject.LinearFrictionMultiplier,
				AngularFrictionMultiplier = this._missionShipObject.AngularFrictionMultiplier,
				TorqueMultiplierOfLateralBuoyantForces = this._missionShipObject.TorqueMultiplierOfLateralBuoyantForces,
				TorqueMultiplierOfVerticalBuoyantForces = this._missionShipObject.TorqueMultiplierOfVerticalBuoyantForces,
				UpSideDownFrictionMultiplier = 3f,
				MaxLinearSpeedForLateralDragCenterShift = this._missionShipObject.MaxLinearSpeed,
				MaxLateralDragShift = this._missionShipObject.MaxLateralDragShift,
				LateralDragShiftCriticalAngle = this._missionShipObject.LateralDragShiftCriticalAngle,
				StepAgentWeightMultiplier = 2f,
				MakeAgentsStepToEntityEvenUnderWater = true
			};
			this._physics = base.GameEntity.GetFirstScriptOfType<NavalPhysics>();
			this._physics.Initialize(navalPhysicsParameters, physicsReference);
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x000523D8 File Offset: 0x000505D8
		internal void OnShipSpawned(MissionShip spawnedShip)
		{
			foreach (IShipEventListener shipEventListener in this._shipEventListeners)
			{
				shipEventListener.OnShipSpawned(spawnedShip);
			}
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0005242C File Offset: 0x0005062C
		internal void OnShipRemoved(MissionShip removedShip)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this.AttachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null)
				{
					shipAttachmentMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					shipAttachmentMachine.CurrentAttachment.Destroy();
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null)
				{
					shipAttachmentPointMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					shipAttachmentPointMachine.CurrentAttachment.Destroy();
				}
			}
			foreach (IShipEventListener shipEventListener in this._shipEventListeners)
			{
				shipEventListener.OnShipRemoved(removedShip);
			}
			if (this.IsAIControlled)
			{
				this.AIController.RemoveShipFromCollisionIgnoreList(removedShip);
			}
			this._actuators.OnShipRemoved(removedShip);
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x00052554 File Offset: 0x00050754
		protected override void OnTickParallel(float dt)
		{
			this._actuators.OnTickParallel(dt);
			NavalPhysics physics = this._physics;
			Formation formation = this.Formation;
			physics.SetContinuousDriftSpeed((formation != null && formation.CountOfUnits > 0) ? 0f : 1f);
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x00052590 File Offset: 0x00050790
		private void ClearFloaterVolumes()
		{
			WeakGameEntity weakGameEntity = WeakGameEntity.Invalid;
			foreach (WeakGameEntity weakGameEntity2 in base.GameEntity.GetChildren())
			{
				if (weakGameEntity2.Name == "floater_volume_holder")
				{
					weakGameEntity = weakGameEntity2;
					break;
				}
			}
			if (weakGameEntity.IsValid)
			{
				weakGameEntity.RemoveAllChildren();
			}
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x0005260C File Offset: 0x0005080C
		internal void SetRemoved(bool value)
		{
			this._isRemoved = value;
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x00052618 File Offset: 0x00050818
		internal void OnShipTransferred(MissionShip ship, Formation oldFormation)
		{
			foreach (IShipEventListener shipEventListener in this._shipEventListeners)
			{
				shipEventListener.OnShipTransferred(ship, oldFormation);
			}
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x0005266C File Offset: 0x0005086C
		public IDWAAgentDelegate CreateDWAAgent(in DWASimulatorParameters parameters)
		{
			if (this._dwaAgentDelegate == null)
			{
				this._dwaAgentDelegate = new ShipDWAAgentDelegate(this, in parameters);
			}
			else
			{
				((IDWAAgentDelegate)this._dwaAgentDelegate).SetParameters(in parameters);
			}
			return this._dwaAgentDelegate;
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x00052698 File Offset: 0x00050898
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this.ShipsLogic.ShipSpawnedEvent -= this.OnShipSpawned;
			this.ShipsLogic.ShipRemovedEvent -= this.OnShipRemoved;
			this.ShipsLogic.ShipTransferredToFormationEvent -= this.OnShipTransferred;
			this.ShipOrder.OnOwnerShipRemoved();
			ShipWaterEffects firstScriptOfTypeRecursive = base.GameEntity.GetFirstScriptOfTypeRecursive<ShipWaterEffects>();
			if (firstScriptOfTypeRecursive != null)
			{
				firstScriptOfTypeRecursive.DeregisterWaterMeshMaterials();
			}
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x00052714 File Offset: 0x00050914
		public void MoveShipToTheTargetWithDirection(MatrixFrame currentFrame, Vec2 targetPosition, Vec2 targetDirection, float maxAcceleration, float maxAngularAcceleration, float fixedDt)
		{
			float num = MathF.Atan2(targetDirection.y, targetDirection.x);
			Vec3 origin = currentFrame.origin;
			Vec3 linearVelocity = this.Physics.LinearVelocity;
			Vec3 angularVelocity = this.Physics.AngularVelocity;
			float mass = this.Physics.Mass;
			float num2 = MathF.Atan2(currentFrame.rotation.f.y, currentFrame.rotation.f.x);
			Vec2 vec = (targetPosition - origin.AsVec2) / fixedDt;
			float num3 = MBMath.WrapAngle(num - num2) / fixedDt;
			Vec2 vec2 = (vec - linearVelocity.AsVec2) / fixedDt;
			vec2.ClampMagnitude(0f, maxAcceleration);
			float num4 = (num3 - angularVelocity.z) / fixedDt;
			float num5 = (float)MathF.Sign(num4);
			float num6 = MathF.Clamp(num4 * num5, 0f, maxAngularAcceleration);
			num4 = num5 * num6;
			Vec2 vec3 = vec2 * mass;
			NavalPhysics physics = this.Physics;
			Vec3 vec4 = vec3.ToVec3(0f);
			physics.ApplyForceToDynamicBody(in vec4, 0);
			NavalPhysics physics2 = this.Physics;
			vec4 = new Vec3(0f, 0f, num4, -1f);
			physics2.ApplyTorque(in vec4, 3);
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x0005284C File Offset: 0x00050A4C
		internal void UpdateController()
		{
			if (!this.IsSinking)
			{
				if (this.ShipControllerMachine.PilotAgent != null && this.ShipControllerMachine.PilotAgent.IsPlayerControlled)
				{
					if (!this.IsPlayerControlled)
					{
						if (this.Formation != null && this.Formation.IsAIControlled)
						{
							this.Formation.SetControlledByAI(false, false);
						}
						this.SetController(ShipControllerType.Player, true);
						this.PlayerController.SetInput(in this._inputRecord);
						return;
					}
					if (this.Formation != null && this.Formation.IsAIControlled)
					{
						this.ShipControllerMachine.PilotAgent.StopUsingGameObject(true, 1);
						this.SetController(ShipControllerType.AI, true);
						return;
					}
				}
				else if (this.IsPlayerShip)
				{
					if (this.Formation != null)
					{
						if (this.Formation.IsAIControlled && !this.HasController)
						{
							this.SetController(ShipControllerType.AI, true);
							return;
						}
						if (!this.Formation.IsAIControlled)
						{
							if (this.IsAIControlled)
							{
								this.ShipOrder.SetShipStopOrder();
								this.SetController(ShipControllerType.None, true);
								ShipInputRecord shipInputRecord = ShipInputRecord.Stop();
								this.SetInputRecord(in shipInputRecord);
								return;
							}
							if (this.HasController)
							{
								this.SetController(ShipControllerType.None, true);
								return;
							}
						}
					}
				}
				else if (!this.IsAIControlled)
				{
					this.SetController(ShipControllerType.AI, true);
				}
			}
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x00052988 File Offset: 0x00050B88
		private void HandleCapsizing()
		{
			bool flag = Vec3.DotProduct(base.GameEntity.GetLocalFrame().rotation.u, Vec3.Up) < -0.5f;
			if (this._isCapsized != flag)
			{
				this._isCapsized = flag;
				if (flag)
				{
					this._capsizeDamageTimer.Reset(Mission.Current.CurrentTime);
				}
			}
			if (this._isCapsized && this._capsizeDamageTimer.Check(Mission.Current.CurrentTime) && !Mission.Current.DisableDying && Mission.Current.Mode != 1 && Mission.Current.Mode != 9)
			{
				int num;
				int num2;
				DamageTypes damageTypes;
				bool flag2;
				this.DealDamage(this.MaxHealth * 0.05f, null, out num, out num2, out damageTypes, out flag2);
			}
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00052A4C File Offset: 0x00050C4C
		private void ValidateShipAndDescendantEntitiesAndBoundingBoxes()
		{
			base.GameEntity.ValidateBoundingBox();
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x00052A67 File Offset: 0x00050C67
		private void OnUnitAttached(Formation formation, Agent agent)
		{
			if (formation.GetReadonlyMovementOrderReference().OrderEnum == 7)
			{
				this.SetPositioningOrdersToRallyPoint(true, false);
			}
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x00052A80 File Offset: 0x00050C80
		private void ComputeStaticLocalBoundingBox()
		{
			this._localBoundingBoxCached.BeginRelaxation();
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				weakGameEntity.ValidateBoundingBox();
				BoundingBox localBoundingBox = weakGameEntity.GetLocalBoundingBox();
				this._localBoundingBoxCached.RelaxWithBoundingBox(localBoundingBox);
			}
			this._localBoundingBoxCacheInvalid = false;
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x00052AFC File Offset: 0x00050CFC
		private void InitializePartialDurabilities()
		{
			for (int i = 0; i < 6; i++)
			{
				this._physics.SetTargetDurabilityOfPart(i, this._partialHitPoints[i] / this.MaxPartialHealth);
			}
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x00052B30 File Offset: 0x00050D30
		private void InitializeShipBoundingBox()
		{
			foreach (ShipOarMachine shipOarMachine in this._leftSideShipOarMachines)
			{
				shipOarMachine.ArrangeOarBoundingBox();
			}
			foreach (ShipOarMachine shipOarMachine2 in this._rightSideShipOarMachines)
			{
				shipOarMachine2.ArrangeOarBoundingBox();
			}
			foreach (ShipUnmannedOar shipUnmannedOar in this._shipUnmannedOars)
			{
				shipUnmannedOar.ArrangeOarBoundingBox();
			}
			foreach (MissionSail missionSail in this._actuators.Sails)
			{
				List<GameEntity> list = new List<GameEntity>();
				missionSail.SailEntity.GetChildrenRecursive(ref list);
				foreach (GameEntity gameEntity in list)
				{
					gameEntity.EntityFlags |= 4096;
				}
				missionSail.SailEntity.SetHasCustomBoundingBoxValidationSystem(true);
				missionSail.SailEntity.SetBoundingboxDirty();
			}
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x00052CB4 File Offset: 0x00050EB4
		private void RecalculateShipIsland()
		{
			this.ShipIslandCombinedID = 0UL;
			ulong num = 0UL;
			this.BuildIslandMaskRecursive(this, ref num);
			ulong num2 = 0UL;
			this.ApplyIslandMaskRecursive(this, num, ref num2);
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x00052CE4 File Offset: 0x00050EE4
		private void BuildIslandMaskRecursive(MissionShip ship, ref ulong islandMask)
		{
			ulong shipUniqueBitwiseID = ship.ShipUniqueBitwiseID;
			if ((islandMask & shipUniqueBitwiseID) != 0UL)
			{
				return;
			}
			islandMask |= shipUniqueBitwiseID;
			foreach (MissionShip missionShip in ship.GetNavmeshConnectedShips())
			{
				this.BuildIslandMaskRecursive(missionShip, ref islandMask);
			}
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00052D4C File Offset: 0x00050F4C
		private void ApplyIslandMaskRecursive(MissionShip ship, ulong finalIslandMask, ref ulong visitedShipsMask)
		{
			ulong shipUniqueBitwiseID = ship.ShipUniqueBitwiseID;
			if ((visitedShipsMask & shipUniqueBitwiseID) != 0UL)
			{
				return;
			}
			visitedShipsMask |= shipUniqueBitwiseID;
			ship.ShipIslandCombinedID = finalIslandMask;
			foreach (MissionShip missionShip in ship.GetNavmeshConnectedShips())
			{
				this.ApplyIslandMaskRecursive(missionShip, finalIslandMask, ref visitedShipsMask);
			}
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x00052DBC File Offset: 0x00050FBC
		private bool IsShipUpsideDown()
		{
			return base.GameEntity.GetLocalFrame().rotation.u.z <= 0.35f;
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x00052DF0 File Offset: 0x00050FF0
		private void SetAbilityOfShipNavmeshFaces(bool enable)
		{
			Mission.Current.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart, enable);
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				int num = this.DynamicNavmeshIdStart + shipAttachmentPointMachine.RelatedShipNavmeshOffset;
				Mission.Current.Scene.SetAbilityOfFacesWithId(num, enable);
			}
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00052E74 File Offset: 0x00051074
		private void AttachDynamicNavmeshFromMachines(MBList<ShipAttachmentMachine> shipAttachmentMachines, MBList<ShipAttachmentPointMachine> shipAttachmentPointMachines)
		{
			this.SetAbilityOfFaces(base.GameEntity.IsValid && GameEntityPhysicsExtensions.GetPhysicsState(base.GameEntity));
			for (int i = 0; i < shipAttachmentPointMachines.Count; i++)
			{
				int num = this.DynamicNavmeshIdStart + shipAttachmentPointMachines[i].RelatedShipNavmeshOffset;
				this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(num, false, false, false, false, false);
			}
			this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart, false, false, false, false, true);
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x00052EF8 File Offset: 0x000510F8
		private bool CheckAttachedNavmeshSanity(bool isEditorMode)
		{
			bool flag = true;
			if (isEditorMode)
			{
				base.GameEntity.Scene.ClearNavMesh();
				base.GameEntity.Scene.ImportNavigationMeshPrefabWithFrame(this.NavMeshPrefabName, base.GameEntity.GetGlobalFrame());
			}
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			MBList<ShipAttachmentMachine> mblist = new MBList<ShipAttachmentMachine>();
			base.GameEntity.GetChildrenRecursive(ref list);
			foreach (WeakGameEntity weakGameEntity in list)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in weakGameEntity.GetScriptComponents<ShipAttachmentMachine>())
				{
					mblist.Add(shipAttachmentMachine);
				}
			}
			MBList<ShipAttachmentPointMachine> mblist2 = new MBList<ShipAttachmentPointMachine>();
			foreach (WeakGameEntity weakGameEntity2 in list)
			{
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in weakGameEntity2.GetScriptComponents<ShipAttachmentPointMachine>())
				{
					mblist2.Add(shipAttachmentPointMachine);
				}
			}
			if (!this.CheckAttachedNavmeshSanityAux(mblist, mblist2, isEditorMode))
			{
				flag = false;
			}
			if (!this.CheckSpawnPointsNavMeshSanityAux(isEditorMode))
			{
				flag = false;
			}
			if (isEditorMode)
			{
				base.GameEntity.Scene.ClearNavMesh();
			}
			return flag;
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x00053098 File Offset: 0x00051298
		private bool CheckPhysicsOfChildren()
		{
			bool flag = true;
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenRecursive(ref list);
			foreach (WeakGameEntity weakGameEntity in list)
			{
				int physicsTriangleCount = GameEntityPhysicsExtensions.GetPhysicsTriangleCount(weakGameEntity);
				if (physicsTriangleCount > 4000)
				{
					string text = string.Format("Physics body has too much polygon {0} for ship part: '{1}' - '{2}'.", physicsTriangleCount, base.GameEntity.Name, weakGameEntity.Name);
					MBEditor.AddEntityWarning(base.GameEntity, text);
				}
			}
			return flag;
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x00053140 File Offset: 0x00051340
		private bool CheckSpawnPoints(bool fromEditor)
		{
			bool flag = true;
			if (MBObjectManager.Instance == null)
			{
				return flag;
			}
			MBReadOnlyList<MissionShipObject> objects = MBObjectManager.Instance.GetObjects<MissionShipObject>((MissionShipObject x) => x.Prefab == this.GameEntity.Name);
			if (objects.Count == 0)
			{
				return flag;
			}
			MissionShipObject missionShipObject = objects[0];
			MBReadOnlyList<ShipHull> objects2 = MBObjectManager.Instance.GetObjects<ShipHull>((ShipHull x) => x.MissionShipObjectId == missionShipObject.StringId);
			if (objects2.Count == 0)
			{
				return flag;
			}
			ShipHull shipHull = objects2[0];
			if (shipHull.TotalCrewCapacity != shipHull.MainDeckCrewCapacity && Extensions.ToMBList<WeakGameEntity>(base.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_crew_spawn")).Count == 0)
			{
				string text = "Ship with reinforcements '" + base.GameEntity.Name + "' does not have any crew spawn point.";
				if (fromEditor)
				{
					MBEditor.AddEntityWarning(base.GameEntity, text);
				}
			}
			List<WeakGameEntity> list = Extensions.ToMBList<WeakGameEntity>(base.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_outer_deck"));
			MBList<WeakGameEntity> mblist = Extensions.ToMBList<WeakGameEntity>(base.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_inner_deck"));
			int num = list.Count + mblist.Count;
			if (base.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_captain").Count == 0)
			{
				string text2 = "Ship '" + base.GameEntity.Name + "' must have at least one captain spawn entity.";
				if (fromEditor)
				{
					MBEditor.AddEntityWarning(base.GameEntity, text2);
				}
			}
			else
			{
				num++;
			}
			float num2 = 1f + Math.Max(NavalPerks.Boatswain.PopularCaptain.PrimaryBonus, NavalPerks.Boatswain.PopularCaptain.SecondaryBonus);
			int num3 = (int)((float)shipHull.MainDeckCrewCapacity * num2);
			if (num < num3)
			{
				string text3 = string.Format("Ship '{0}': Main deck crew spawn point count {1}", base.GameEntity.Name, num) + string.Format("should be equal or greater than the value set in ship hull xml (including perks): {0}.", num3);
				if (fromEditor)
				{
					MBEditor.AddEntityWarning(base.GameEntity, text3);
				}
			}
			return flag;
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00053334 File Offset: 0x00051534
		private bool CheckOarCount(bool fromEditor)
		{
			bool flag = true;
			if (MBObjectManager.Instance == null)
			{
				return flag;
			}
			MBReadOnlyList<MissionShipObject> objects = MBObjectManager.Instance.GetObjects<MissionShipObject>((MissionShipObject x) => x.Prefab == base.GameEntity.Name);
			if (objects.Count == 0)
			{
				return flag;
			}
			int oarCount = objects[0].OarCount;
			int count = base.GameEntity.CollectChildrenEntitiesWithTag("oar_gate_left").Count;
			int count2 = base.GameEntity.CollectChildrenEntitiesWithTag("oar_gate_right").Count;
			if (count + count2 != oarCount)
			{
				string text = "Oar count set in prefab does not match oar count set in mission ship xml for ship '" + base.GameEntity.Name + "'.";
				if (fromEditor)
				{
					MBEditor.AddEntityWarning(base.GameEntity, text);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x000533E8 File Offset: 0x000515E8
		private bool CheckSpawnPointsNavMeshSanityAux(bool fromEditor)
		{
			bool flag = true;
			int num;
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.CollectChildrenEntitiesWithTag("rally_point"))
			{
				Vec3 origin = weakGameEntity.GetGlobalFrame().origin;
				if (base.GameEntity.Scene.GetNavigationMeshForPosition(ref origin, ref num, 1.5f, false) == UIntPtr.Zero)
				{
					string text = string.Concat(new string[]
					{
						"Rally point '",
						weakGameEntity.Name,
						"' is not on any navigation mesh face in ship '",
						base.GameEntity.Name,
						"'."
					});
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(weakGameEntity, text);
					}
					flag = false;
				}
			}
			foreach (WeakGameEntity weakGameEntity2 in base.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_captain"))
			{
				Vec3 origin2 = weakGameEntity2.GetGlobalFrame().origin;
				if (base.GameEntity.Scene.GetNavigationMeshForPosition(ref origin2, ref num, 1.5f, false) == UIntPtr.Zero)
				{
					string text2 = string.Concat(new string[]
					{
						"Captain spawn point '",
						weakGameEntity2.Name,
						"' is not on any navigation mesh face in ship '",
						base.GameEntity.Name,
						"'."
					});
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(weakGameEntity2, text2);
					}
					flag = false;
				}
			}
			foreach (WeakGameEntity weakGameEntity3 in base.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_outer_deck"))
			{
				Vec3 origin3 = weakGameEntity3.GetGlobalFrame().origin;
				if (base.GameEntity.Scene.GetNavigationMeshForPosition(ref origin3, ref num, 1.5f, false) == UIntPtr.Zero)
				{
					string text3 = string.Concat(new string[]
					{
						"Outer deck spawn point '",
						weakGameEntity3.Name,
						"' is not on any navigation mesh face in ship '",
						base.GameEntity.Name,
						"'."
					});
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(weakGameEntity3, text3);
					}
					flag = false;
				}
			}
			foreach (WeakGameEntity weakGameEntity4 in base.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_inner_deck"))
			{
				Vec3 origin4 = weakGameEntity4.GetGlobalFrame().origin;
				int num2;
				if (base.GameEntity.Scene.GetNavigationMeshForPosition(ref origin4, ref num2, 1.5f, false) == UIntPtr.Zero)
				{
					string text4 = string.Concat(new string[]
					{
						"Inner deck spawn point '",
						weakGameEntity4.Name,
						"' is not on any navigation mesh face in ship '",
						base.GameEntity.Name,
						"'."
					});
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(weakGameEntity4, text4);
					}
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0005374C File Offset: 0x0005194C
		private bool CheckAttachedNavmeshSanityAux(MBList<ShipAttachmentMachine> shipAttachmentMachines, MBList<ShipAttachmentPointMachine> shipAttachmentPointMachines, bool fromEditor)
		{
			bool flag = true;
			PathFaceRecord[] array = new PathFaceRecord[fromEditor ? base.GameEntity.Scene.GetNavMeshFaceCount() : base.GameEntity.GetAttachedNavmeshFaceCount()];
			if (fromEditor)
			{
				base.GameEntity.Scene.GetAllNavmeshFaceRecords(array);
			}
			else
			{
				base.GameEntity.GetAttachedNavmeshFaceRecords(array);
			}
			HashSet<int> uniqueIdsFaces = new HashSet<int>();
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			PathFaceRecord[] array2 = new PathFaceRecord[base.GameEntity.Scene.GetNavmeshFaceCountBetweenTwoIds(this.DynamicNavmeshIdStart, this.DynamicNavmeshIdStart + 50)];
			base.GameEntity.Scene.GetNavmeshFaceRecordsBetweenTwoIds(this.DynamicNavmeshIdStart, this.DynamicNavmeshIdStart + 50, array2);
			foreach (PathFaceRecord pathFaceRecord in array2)
			{
				if (pathFaceRecord.FaceGroupIndex < this.DynamicNavmeshIdStart || pathFaceRecord.FaceGroupIndex > this.DynamicNavmeshIdStart + 50)
				{
					string text = string.Format("The face with id {0} must not be attached to {1}. Ids must be between 0 and {2}.", pathFaceRecord.FaceGroupIndex - this.DynamicNavmeshIdStart, base.GameEntity.Name, 50);
					if (fromEditor)
					{
						MBEditor.AddNavMeshWarning(base.GameEntity.Scene, pathFaceRecord, text);
					}
					flag = false;
				}
				else if (pathFaceRecord.FaceGroupIndex > this.DynamicNavmeshIdStart && !uniqueIdsFaces.Add(pathFaceRecord.FaceGroupIndex))
				{
					string text2 = string.Format("Attached navmesh must have faces with unique group ids. Id: {0} is not unique", pathFaceRecord.FaceGroupIndex - this.DynamicNavmeshIdStart);
					if (fromEditor)
					{
						MBEditor.AddNavMeshWarning(base.GameEntity.Scene, pathFaceRecord, text2);
					}
					flag = false;
				}
			}
			foreach (PathFaceRecord pathFaceRecord2 in array2)
			{
				if (pathFaceRecord2.FaceGroupIndex != this.DynamicNavmeshIdStart && !base.GameEntity.Scene.HasNavmeshFaceUnsharedEdges(ref pathFaceRecord2))
				{
					string text3 = string.Format("The face with id {0} must not be fully enclosed; it must have at least one unshared edge.", pathFaceRecord2.FaceGroupIndex - this.DynamicNavmeshIdStart);
					if (fromEditor)
					{
						MBEditor.AddNavMeshWarning(base.GameEntity.Scene, pathFaceRecord2, text3);
					}
					flag = false;
				}
			}
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			string name = base.GameEntity.Name;
			bool flag2 = false;
			foreach (ShipAttachmentMachine shipAttachmentMachine in shipAttachmentMachines)
			{
				int num = this.DynamicNavmeshIdStart + shipAttachmentMachine.RelatedShipNavmeshOffset;
				if (num <= this.DynamicNavmeshIdStart || num > this.DynamicNavmeshIdStart + 50)
				{
					string text4 = string.Format("{0}: Every {1}'s RelatedShipNavmeshOffset must be between 1 and {2}.", name, shipAttachmentMachine.GameEntity.Name, 50);
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(shipAttachmentMachine.GameEntity, text4);
					}
					flag = false;
				}
				if (!hashSet.Add(shipAttachmentMachine.RelatedShipNavmeshOffset))
				{
					flag2 = true;
					list.Add(shipAttachmentMachine.GameEntity);
				}
				if (uniqueIdsFaces.Contains(shipAttachmentMachine.RelatedShipNavmeshOffset + this.DynamicNavmeshIdStart))
				{
					uniqueIdsFaces.Remove(shipAttachmentMachine.RelatedShipNavmeshOffset + this.DynamicNavmeshIdStart);
				}
				MatrixFrame globalFrame = shipAttachmentMachine.GameEntity.GetGlobalFrame();
				int num2;
				if (base.GameEntity.Scene.GetNavigationMeshForPosition(ref globalFrame.origin, ref num2, 1.5f, false) == UIntPtr.Zero)
				{
					string text5 = string.Format("{0}: shipAttachmentMachine with related id {1} is not on any navmesh face", name, shipAttachmentMachine.RelatedShipNavmeshOffset);
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(shipAttachmentMachine.GameEntity, text5);
					}
					flag = false;
				}
				else if (num2 != num)
				{
					string text6 = string.Format("{0}: ShipAttachmentMachine script with nav mesh id {1} is not on a face with the same id. Current face id: {2}", name, shipAttachmentMachine.RelatedShipNavmeshOffset, num2);
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(shipAttachmentMachine.GameEntity, text6);
					}
					flag = false;
				}
			}
			if (flag2)
			{
				foreach (WeakGameEntity weakGameEntity in list)
				{
					string text7 = name + ": shipAttachmentMachine '" + weakGameEntity.Name + "' must have a unique RelatedShipNavmeshOffset with respect to other ShipAttachmentMachines";
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(weakGameEntity, text7);
					}
					flag = false;
				}
				flag2 = false;
				list.Clear();
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in shipAttachmentPointMachines)
			{
				int num3 = this.DynamicNavmeshIdStart + shipAttachmentPointMachine.RelatedShipNavmeshOffset;
				if (num3 <= this.DynamicNavmeshIdStart || num3 > this.DynamicNavmeshIdStart + 50)
				{
					string text8 = string.Format("{0}: Every {1}'s RelatedShipNavmeshOffset must be between 1 and {2}.", name, shipAttachmentPointMachine.GameEntity.Name, 50);
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(shipAttachmentPointMachine.GameEntity, text8);
					}
					flag = false;
				}
				if (!hashSet2.Add(shipAttachmentPointMachine.RelatedShipNavmeshOffset))
				{
					flag2 = true;
					list.Add(shipAttachmentPointMachine.GameEntity);
				}
				if (uniqueIdsFaces.Contains(shipAttachmentPointMachine.RelatedShipNavmeshOffset + this.DynamicNavmeshIdStart))
				{
					uniqueIdsFaces.Remove(shipAttachmentPointMachine.RelatedShipNavmeshOffset + this.DynamicNavmeshIdStart);
				}
				MatrixFrame globalFrame2 = shipAttachmentPointMachine.GameEntity.GetGlobalFrame();
				int num4;
				if (base.GameEntity.Scene.GetNavigationMeshForPosition(ref globalFrame2.origin, ref num4, 1.5f, false) == UIntPtr.Zero)
				{
					string text9 = string.Format("{0}: shipAttachmentPointMachine with related id {1} is not on any navmesh face", name, shipAttachmentPointMachine.RelatedShipNavmeshOffset);
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(shipAttachmentPointMachine.GameEntity, text9);
					}
					flag = false;
				}
				else if (num4 != num3)
				{
					string text10 = string.Format("{0}: ShipAttachmentPointMachine script with nav mesh face id {1} is not on a face with the same id. Current face id: {2}", name, shipAttachmentPointMachine.RelatedShipNavmeshOffset, num4);
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(shipAttachmentPointMachine.GameEntity, text10);
					}
					flag = false;
				}
			}
			foreach (PathFaceRecord pathFaceRecord3 in array2.Where<PathFaceRecord>((PathFaceRecord record) => uniqueIdsFaces.Contains(record.FaceGroupIndex)).ToList<PathFaceRecord>())
			{
				string text11 = string.Format("{0}: The face with id {1} has not been attached to {2}. ", name, pathFaceRecord3.FaceGroupIndex - this.DynamicNavmeshIdStart, base.GameEntity.Name) + string.Format("There should be a shipAttachmentMachine or a shipAttachmentPointMachine with RelatedShipNavmeshOffset: {0}", pathFaceRecord3.FaceGroupIndex - this.DynamicNavmeshIdStart);
				if (fromEditor)
				{
					MBEditor.AddNavMeshWarning(base.GameEntity.Scene, pathFaceRecord3, text11);
				}
				flag = false;
			}
			if (flag2)
			{
				foreach (WeakGameEntity weakGameEntity2 in list)
				{
					string text12 = name + ": ShipAttachmentPointMachine '" + weakGameEntity2.Name + "' must have a unique RelatedShipNavmeshOffset with respect to other ShipAttachmentPoints";
					if (fromEditor)
					{
						MBEditor.AddEntityWarning(weakGameEntity2, text12);
					}
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x00053ECC File Offset: 0x000520CC
		private int DeckSpawnFrameSortingFunction(MatrixFrame deckFrame1, MatrixFrame deckFrame2)
		{
			float num = Vec3.DotProduct(deckFrame1.origin, Vec3.Forward);
			return -Vec3.DotProduct(deckFrame2.origin, Vec3.Forward).CompareTo(num);
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00053F04 File Offset: 0x00052104
		private void InitializeLists(bool isForCheckingForProblems)
		{
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenRecursive(ref list);
			this._rightSideShipOarMachines = new MBList<ShipOarMachine>();
			this._leftSideShipOarMachines = new MBList<ShipOarMachine>();
			this._shipOarMachines = new MBList<ShipOarMachine>();
			this._shipUnmannedOars = new MBList<ShipUnmannedOar>();
			this._climbingMachines = new MBList<ClimbingMachine>();
			this.ShipSiegeWeapon = null;
			this._allDestructibleComponents = new MBList<DestructableComponent>();
			this._ammoBarrels = new MBList<AmmoBarrelBase>();
			foreach (WeakGameEntity weakGameEntity in list)
			{
				if (weakGameEntity.HasScriptOfType<ShipOarMachine>())
				{
					if (weakGameEntity.GetLocalFrame().origin.AsVec2.DotProduct(Vec2.Side) > 0f)
					{
						this._rightSideShipOarMachines.Add(weakGameEntity.GetFirstScriptOfType<ShipOarMachine>());
					}
					else
					{
						this._leftSideShipOarMachines.Add(weakGameEntity.GetFirstScriptOfType<ShipOarMachine>());
					}
				}
				else if (weakGameEntity.HasScriptOfType<ShipControllerMachine>())
				{
					this.ShipControllerMachine = weakGameEntity.GetFirstScriptOfType<ShipControllerMachine>();
				}
				else if (weakGameEntity.HasScriptOfType<ClimbingMachine>())
				{
					this._climbingMachines.Add(weakGameEntity.GetFirstScriptOfType<ClimbingMachine>());
				}
				else if (weakGameEntity.HasScriptOfType<ShipUnmannedOar>())
				{
					this._shipUnmannedOars.Add(weakGameEntity.GetFirstScriptOfType<ShipUnmannedOar>());
				}
				else if (weakGameEntity.HasScriptOfType<RangedSiegeWeapon>())
				{
					this.ShipSiegeWeapon = weakGameEntity.GetFirstScriptOfType<RangedSiegeWeapon>();
				}
				else if (weakGameEntity.HasScriptOfType<MissionShipRam>())
				{
					this._ram = weakGameEntity.GetFirstScriptOfType<MissionShipRam>();
				}
				else if (weakGameEntity.HasScriptOfType<AmmoBarrelBase>())
				{
					this._ammoBarrels.Add(weakGameEntity.GetFirstScriptOfType<AmmoBarrelBase>());
				}
				if (weakGameEntity.HasScriptOfType<DestructableComponent>())
				{
					this._allDestructibleComponents.Add(weakGameEntity.GetFirstScriptOfType<DestructableComponent>());
				}
			}
			this._leftSideShipOarMachines.Sort(delegate(ShipOarMachine oar1, ShipOarMachine oar2)
			{
				float y3 = oar1.GameEntity.GetLocalFrame().origin.y;
				float y4 = oar2.GameEntity.GetLocalFrame().origin.y;
				return y4.CompareTo(y3);
			});
			this._rightSideShipOarMachines.Sort(delegate(ShipOarMachine oar1, ShipOarMachine oar2)
			{
				float y5 = oar1.GameEntity.GetLocalFrame().origin.y;
				float y6 = oar2.GameEntity.GetLocalFrame().origin.y;
				return y6.CompareTo(y5);
			});
			for (int i = 0; i < this._leftSideShipOarMachines.Count; i++)
			{
				ShipOarMachine shipOarMachine = this._leftSideShipOarMachines[i];
				ShipOarMachine shipOarMachine2 = this._rightSideShipOarMachines[i];
				float y = shipOarMachine.GameEntity.GetLocalFrame().origin.y;
				float y2 = shipOarMachine2.GameEntity.GetLocalFrame().origin.y;
				Math.Abs(y - y2);
				this._shipOarMachines.Add(shipOarMachine);
				this._shipOarMachines.Add(shipOarMachine2);
			}
			MBList<ShipControllerMachine> mblist = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<ShipControllerMachine>(base.GameEntity);
			this.ShipControllerMachine = ((mblist.Count > 0) ? mblist[0] : null);
			this._shipAttachmentMachines = Extensions.ToMBList<ShipAttachmentMachine>(from ce in list
				where ce.HasScriptOfType<ShipAttachmentMachine>()
				select ce.GetFirstScriptOfType<ShipAttachmentMachine>());
			this._sailVisuals = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<SailVisual>(base.GameEntity);
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x00054254 File Offset: 0x00052454
		private void LoadSpawnPoints()
		{
			GameEntity gameEntity = GameEntity.CreateFromWeakEntity(base.GameEntity);
			this._outerDeckLocalFrames = new MBList<MatrixFrame>();
			MatrixFrame matrixFrame;
			MatrixFrame matrixFrame2;
			foreach (GameEntity gameEntity2 in Extensions.ToMBList<GameEntity>(MBExtensions.CollectChildrenEntitiesWithTag(gameEntity, "sp_troop_outer_deck")))
			{
				matrixFrame = gameEntity.GetGlobalFrame();
				matrixFrame2 = gameEntity2.GetGlobalFrame();
				MatrixFrame matrixFrame3 = matrixFrame.TransformToLocal(ref matrixFrame2);
				this._outerDeckLocalFrames.Add(matrixFrame3);
			}
			this._innerDeckLocalFrames = new MBList<MatrixFrame>();
			foreach (GameEntity gameEntity3 in Extensions.ToMBList<GameEntity>(MBExtensions.CollectChildrenEntitiesWithTag(gameEntity, "sp_troop_inner_deck")))
			{
				matrixFrame2 = gameEntity.GetGlobalFrame();
				matrixFrame = gameEntity3.GetGlobalFrame();
				MatrixFrame matrixFrame4 = matrixFrame2.TransformToLocal(ref matrixFrame);
				this._innerDeckLocalFrames.Add(matrixFrame4);
			}
			this._crewSpawnLocalFrames = new MBList<MatrixFrame>();
			foreach (GameEntity gameEntity4 in Extensions.ToMBList<GameEntity>(MBExtensions.CollectChildrenEntitiesWithTag(gameEntity, "sp_troop_crew_spawn")))
			{
				matrixFrame = gameEntity.GetGlobalFrame();
				matrixFrame2 = gameEntity4.GetGlobalFrame();
				MatrixFrame matrixFrame5 = matrixFrame.TransformToLocal(ref matrixFrame2);
				this._crewSpawnLocalFrames.Add(matrixFrame5);
			}
			this._outerDeckLocalFrames.Sort(new Comparison<MatrixFrame>(this.DeckSpawnFrameSortingFunction));
			this._innerDeckLocalFrames.Sort(new Comparison<MatrixFrame>(this.DeckSpawnFrameSortingFunction));
			List<GameEntity> list = MBExtensions.CollectChildrenEntitiesWithTag(gameEntity, "sp_troop_captain");
			List<MatrixFrame> innerDeckLocalFrames = this._innerDeckLocalFrames;
			matrixFrame2 = gameEntity.GetGlobalFrame();
			matrixFrame = list[0].GetGlobalFrame();
			innerDeckLocalFrames.Add(matrixFrame2.TransformToLocal(ref matrixFrame));
			this.CrewSizeOnMainDeck = MathF.Min(this.DeckFrameCount, this.ShipOrigin.MainDeckCrewCapacity);
			this.ShipPlacementDetachment = new ShipPlacementDetachment(in this);
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0005446C File Offset: 0x0005266C
		protected override bool CanPhysicsCollideBetweenTwoEntities(WeakGameEntity myEntity, BodyFlags myEntityBodyFlags, WeakGameEntity otherEntity, BodyFlags otherEntityBodyFlags)
		{
			return !Extensions.HasAnyFlag<BodyFlags>(otherEntityBodyFlags, 16) || Extensions.HasAnyFlag<BodyFlags>(otherEntityBodyFlags, 8);
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x0005448C File Offset: 0x0005268C
		private void LoadShipBanners()
		{
			GameEntity gameEntity = GameEntity.CreateFromWeakEntity(base.GameEntity);
			this._bannerEntities = Extensions.ToMBList<GameEntity>(MBExtensions.CollectChildrenEntitiesWithTag(gameEntity, "banner_with_faction_color"));
			this._sailMeshEntities = Extensions.ToMBList<GameEntity>(MBExtensions.CollectChildrenEntitiesWithTag(gameEntity, "sail_mesh_entity"));
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x000544D1 File Offset: 0x000526D1
		public static bool AreShipsConnected(MissionShip ship1, MissionShip ship2)
		{
			return (ship1.ShipIslandCombinedID & ship2.ShipIslandCombinedID) > 0UL;
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x000544E4 File Offset: 0x000526E4
		public void OnSetRangedWeaponControlMode(bool value)
		{
			if (this.ShipSiegeWeapon != null)
			{
				(this.ShipSiegeWeapon.Ai as ShipBallistaAI).SetIsUnderDirectControl(value);
			}
			foreach (SailVisual sailVisual in this._sailVisuals)
			{
				sailVisual.SetBallistaRopeVisibility(!value);
			}
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x00054558 File Offset: 0x00052758
		public bool IsAgentUsingSiegeWeapon(Agent agent)
		{
			return this.ShipSiegeWeapon != null && this.ShipSiegeWeapon.PilotAgent == agent;
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x00054572 File Offset: 0x00052772
		public void SetCustomSailSetting(bool enableCustomSailSetting, SailInput customSailSetting)
		{
			this.HasCustomSailSetting = enableCustomSailSetting;
			this._customSailSetting = customSailSetting;
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x00054582 File Offset: 0x00052782
		public void ShootBallista()
		{
			this.ShipSiegeWeapon.Shoot();
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x00054590 File Offset: 0x00052790
		public void TryToMaintainConnectionToAnotherShip(MissionShip otherShip, bool forceBridge = true, bool unbreakableBridge = false)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment == null)
				{
					shipAttachmentMachine.SetPreferredTargetShip(otherShip);
					if (shipAttachmentMachine.LinkedAttachmentPointMachine.CurrentAttachment == null)
					{
						shipAttachmentMachine.SetCanConnectToFriends(true);
						ShipAttachmentPointMachine bestEnemyAttachment = shipAttachmentMachine.GetBestEnemyAttachment(true, false);
						if (bestEnemyAttachment != null)
						{
							shipAttachmentMachine.ConnectWithAttachmentPointMachine(bestEnemyAttachment, forceBridge, unbreakableBridge, false);
						}
					}
				}
			}
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x00054618 File Offset: 0x00052818
		public void TryToConnectionToAttachmentMachine(ShipAttachmentMachine otherAttachmentMachine, bool forceBridge = true, bool unbreakableBridge = false)
		{
			ShipAttachmentPointMachine shipAttachmentPointMachine = null;
			if (otherAttachmentMachine.CurrentAttachment == null && otherAttachmentMachine.LinkedAttachmentPointMachine.CurrentAttachment == null)
			{
				shipAttachmentPointMachine = otherAttachmentMachine.GetBestEnemyAttachment(true, false);
			}
			if (shipAttachmentPointMachine != null)
			{
				otherAttachmentMachine.SetPreferredTargetShip(shipAttachmentPointMachine.OwnerShip);
				otherAttachmentMachine.SetCanConnectToFriends(true);
				otherAttachmentMachine.ConnectWithAttachmentPointMachine(shipAttachmentPointMachine, forceBridge, unbreakableBridge, false);
			}
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x00054668 File Offset: 0x00052868
		public void DisconnectedWithShip(MissionShip otherShip)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.GetPreferredTargetShip() == otherShip)
				{
					shipAttachmentMachine.SetPreferredTargetShip(null);
					if (shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip == otherShip)
					{
						shipAttachmentMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					}
				}
			}
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x000546EC File Offset: 0x000528EC
		public void InvalidateLocalBoundingBoxCache()
		{
			this._localBoundingBoxCacheInvalid = true;
			base.GameEntity.SetBoundingboxDirty();
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x0005470E File Offset: 0x0005290E
		public void InvalidateActiveFormationTroopOnShipCache()
		{
			this._anyActiveFormationTroopOnShip.Expire();
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x0005471C File Offset: 0x0005291C
		internal void SeparateFromShip(MissionShip otherShip)
		{
			bool flag = false;
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected && shipAttachmentMachine.CurrentAttachment.ShipIslandsConnected && (shipAttachmentMachine.CurrentAttachment.AttachmentSource.OwnerShip == otherShip || shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip == otherShip))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.RecalculateShipIsland();
				if ((this.ShipIslandCombinedID & otherShip.ShipUniqueBitwiseID) == 0UL)
				{
					otherShip.RecalculateShipIsland();
				}
			}
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x000547D8 File Offset: 0x000529D8
		internal static void MergeShipIslands(MissionShip ship1, MissionShip ship2)
		{
			if (ship1.ShipIslandCombinedID != ship2.ShipIslandCombinedID)
			{
				ulong num = ship1.ShipIslandCombinedID | ship2.ShipIslandCombinedID;
				ship1._temporaryMissionShipQueue.Clear();
				ship1._temporaryMissionShipQueue.Enqueue(ship1);
				while (ship1._temporaryMissionShipQueue.Count > 0)
				{
					MissionShip missionShip = ship1._temporaryMissionShipQueue.Dequeue();
					if (missionShip.ShipIslandCombinedID != num)
					{
						missionShip.ShipIslandCombinedID |= num;
						num = missionShip.ShipIslandCombinedID;
						foreach (MissionShip missionShip2 in missionShip.GetNavmeshConnectedShips())
						{
							if (missionShip2.ShipIslandCombinedID != num)
							{
								ship1._temporaryMissionShipQueue.Enqueue(missionShip2);
							}
						}
					}
				}
			}
		}

		// Token: 0x0400066C RID: 1644
		private const int DetanglingDuration = 6;

		// Token: 0x0400066D RID: 1645
		private const float DamageCooldownForShipInSeconds = 2f;

		// Token: 0x0400066E RID: 1646
		private const float CollisionDirectionSpeedThresholdToDamage = 3f;

		// Token: 0x0400066F RID: 1647
		private const float MaxSoundPositionUpdateDistanceSquared = 10000f;

		// Token: 0x04000671 RID: 1649
		public const string OuterDeckTroopSpTag = "sp_troop_outer_deck";

		// Token: 0x04000672 RID: 1650
		public const string InnerDeckTroopSpTag = "sp_troop_inner_deck";

		// Token: 0x04000673 RID: 1651
		public const string CaptainTroopSpTag = "sp_troop_captain";

		// Token: 0x04000674 RID: 1652
		public const string CrewTroopSpTag = "sp_troop_crew_spawn";

		// Token: 0x04000675 RID: 1653
		public const string RallyPointTag = "rally_point";

		// Token: 0x04000676 RID: 1654
		public const string BannerTag = "banner_with_faction_color";

		// Token: 0x04000677 RID: 1655
		public const string SailMeshTag = "sail_mesh_entity";

		// Token: 0x04000678 RID: 1656
		public const float NavmeshDisableLimit = 0.35f;

		// Token: 0x04000679 RID: 1657
		private static TextObject PlayerSideShipSinkingText = new TextObject("{=jX6yqP3T}A friendly ship has started to sink!", null);

		// Token: 0x0400067A RID: 1658
		private static TextObject EnemySideShipSinkingText = new TextObject("{=nvTWWBib}An enemy ship has started to sink!", null);

		// Token: 0x0400067B RID: 1659
		private readonly MBList<MissionShip> _temporaryMissionShipContainer = new MBList<MissionShip>();

		// Token: 0x0400067C RID: 1660
		private readonly MBQueue<MissionShip> _temporaryMissionShipQueue = new MBQueue<MissionShip>();

		// Token: 0x0400067D RID: 1661
		private static readonly int _scrapeSoundEventID = SoundEvent.GetEventIdFromString("event:/physics/vessel/ship_scraping");

		// Token: 0x0400067E RID: 1662
		private readonly QueryData<bool> _anyActiveFormationTroopOnShip;

		// Token: 0x04000694 RID: 1684
		private SailInput _customSailSetting;

		// Token: 0x04000695 RID: 1685
		private MBList<ValueTuple<int, SoundEvent>> _scrapeSoundEvents;

		// Token: 0x04000696 RID: 1686
		private MissionShipObject _missionShipObject;

		// Token: 0x04000698 RID: 1688
		public bool ShouldUpdateSoundPos;

		// Token: 0x04000699 RID: 1689
		private NavalAgentMoraleInteractionLogic _moraleInteractionLogic;

		// Token: 0x0400069A RID: 1690
		private MBList<MatrixFrame> _outerDeckLocalFrames;

		// Token: 0x0400069B RID: 1691
		private MBList<MatrixFrame> _innerDeckLocalFrames;

		// Token: 0x0400069C RID: 1692
		private MBList<MatrixFrame> _crewSpawnLocalFrames;

		// Token: 0x0400069D RID: 1693
		private int _nextDeckSpawnFrameIndex;

		// Token: 0x0400069E RID: 1694
		private bool _autoUpdateController = true;

		// Token: 0x0400069F RID: 1695
		private int _nextCrewSpawnFrameIndex;

		// Token: 0x040006A1 RID: 1697
		private MBList<ShipAttachmentMachine> _attachmentMachines;

		// Token: 0x040006A2 RID: 1698
		private MBList<IShipEventListener> _shipEventListeners;

		// Token: 0x040006A3 RID: 1699
		private bool _isCapsized;

		// Token: 0x040006A4 RID: 1700
		private MBList<ShipAttachmentPointMachine> _attachmentPointMachines;

		// Token: 0x040006A6 RID: 1702
		private MBList<ShipShieldComponent> _shields;

		// Token: 0x040006A7 RID: 1703
		private Timer _capsizeDamageTimer;

		// Token: 0x040006A8 RID: 1704
		private MBList<GameEntity> _bannerEntities;

		// Token: 0x040006A9 RID: 1705
		private MBList<GameEntity> _sailMeshEntities;

		// Token: 0x040006AA RID: 1706
		private WorldPosition _cachedWorldPositionOnDeck;

		// Token: 0x040006AB RID: 1707
		private bool _isCachedWorldPositionOnDeckDirty = true;

		// Token: 0x040006AC RID: 1708
		private GameEntity _playerStandingPointEntity;

		// Token: 0x040006AE RID: 1710
		private bool _isRemoved;

		// Token: 0x040006AF RID: 1711
		private bool _foldSailsOnBridgeConnection = true;

		// Token: 0x040006B0 RID: 1712
		private HashSet<MissionShip> _visitedMissionShips;

		// Token: 0x040006B1 RID: 1713
		private float _nextPermanentBurnDamageTime;

		// Token: 0x040006B2 RID: 1714
		private float _nextFireHitPointRestoreTime;

		// Token: 0x040006B3 RID: 1715
		private Vec2[] _localPhysicsBoundingBoxXYPlaneVertices;

		// Token: 0x040006B4 RID: 1716
		private Vec2[] _scaledLocalPhysicsBoundingBoxXYPlaneVertices;

		// Token: 0x040006B5 RID: 1717
		private Vec2[] _physicsBoundingBoxXYPlaneVertices;

		// Token: 0x040006B6 RID: 1718
		private Vec2[] _criticalZoneVertices;

		// Token: 0x040006B7 RID: 1719
		private MissionShip _detanglingMissionShip;

		// Token: 0x040006B8 RID: 1720
		private Vec3 _detanglingMissionShipAverageContactPosition;

		// Token: 0x040006B9 RID: 1721
		private MissionTimer _detanglingMissionShipTimer;

		// Token: 0x040006BA RID: 1722
		private ShipInputProcessor _inputProcessor;

		// Token: 0x040006BB RID: 1723
		private ShipActuators _actuators;

		// Token: 0x040006BC RID: 1724
		private ShipInputRecord _inputRecord;

		// Token: 0x040006BD RID: 1725
		private NavalPhysics _physics;

		// Token: 0x040006BE RID: 1726
		private float[] _partialHitPoints;

		// Token: 0x040006BF RID: 1727
		private MBList<ShipAttachmentMachine> _shipAttachmentMachines;

		// Token: 0x040006C0 RID: 1728
		private MBList<ShipOarMachine> _leftSideShipOarMachines;

		// Token: 0x040006C1 RID: 1729
		private MBList<ShipOarMachine> _rightSideShipOarMachines;

		// Token: 0x040006C2 RID: 1730
		private MBList<ShipOarMachine> _shipOarMachines;

		// Token: 0x040006C3 RID: 1731
		private MBList<ShipUnmannedOar> _shipUnmannedOars;

		// Token: 0x040006C4 RID: 1732
		private MBList<ClimbingMachine> _climbingMachines;

		// Token: 0x040006C5 RID: 1733
		private MBList<DestructableComponent> _allDestructibleComponents;

		// Token: 0x040006C6 RID: 1734
		private ShipDWAAgentDelegate _dwaAgentDelegate;

		// Token: 0x040006C7 RID: 1735
		private MissionShipRam _ram;

		// Token: 0x040006C8 RID: 1736
		private MBList<AmmoBarrelBase> _ammoBarrels;

		// Token: 0x040006C9 RID: 1737
		private float _connectionBlockedShipTime;

		// Token: 0x040006CA RID: 1738
		private float _disconnectionBlockedShipTime;

		// Token: 0x040006CB RID: 1739
		private MBList<SailVisual> _sailVisuals;

		// Token: 0x040006CC RID: 1740
		private BoundingBox _localBoundingBoxCached;

		// Token: 0x040006CD RID: 1741
		private bool _localBoundingBoxCacheInvalid = true;

		// Token: 0x040006CF RID: 1743
		private List<MissionShip.ShipToEntityCollisionStatus> _currentCollisionStatesToShips = new List<MissionShip.ShipToEntityCollisionStatus>();

		// Token: 0x040006D0 RID: 1744
		private readonly Dictionary<MissionShip, float> _shipDamageCooldowns = new Dictionary<MissionShip, float>();

		// Token: 0x040006D1 RID: 1745
		private readonly ConcurrentQueue<MissionShip.ShipCollisionData> _shipCollisionData = new ConcurrentQueue<MissionShip.ShipCollisionData>();

		// Token: 0x040006D3 RID: 1747
		private static uint _missionShipScriptNameHash = Managed.GetStringHashCode("MissionShip");

		// Token: 0x02000213 RID: 531
		public enum ShipInstanceType
		{
			// Token: 0x04000ECB RID: 3787
			None,
			// Token: 0x04000ECC RID: 3788
			MissionShip,
			// Token: 0x04000ECD RID: 3789
			EditorShip
		}

		// Token: 0x02000214 RID: 532
		public enum SailState : byte
		{
			// Token: 0x04000ECF RID: 3791
			Intact,
			// Token: 0x04000ED0 RID: 3792
			Burning,
			// Token: 0x04000ED1 RID: 3793
			Destroyed
		}

		// Token: 0x02000215 RID: 533
		public struct ShipCollisionData
		{
			// Token: 0x06001AFF RID: 6911 RVA: 0x000B1EFF File Offset: 0x000B00FF
			public ShipCollisionData(MissionShip collidingShip, Vec3 contactPosAvg, float damage)
			{
				this.CollidingShip = collidingShip;
				this.ContactPosAvg = contactPosAvg;
				this.Damage = damage;
			}

			// Token: 0x04000ED2 RID: 3794
			public MissionShip CollidingShip;

			// Token: 0x04000ED3 RID: 3795
			public Vec3 ContactPosAvg;

			// Token: 0x04000ED4 RID: 3796
			public float Damage;
		}

		// Token: 0x02000216 RID: 534
		private struct ShipToEntityCollisionStatus
		{
			// Token: 0x17000406 RID: 1030
			// (get) Token: 0x06001B00 RID: 6912 RVA: 0x000B1F16 File Offset: 0x000B0116
			// (set) Token: 0x06001B01 RID: 6913 RVA: 0x000B1F1E File Offset: 0x000B011E
			public PhysicsEventType CurrentCollisionState { get; private set; }

			// Token: 0x06001B02 RID: 6914 RVA: 0x000B1F27 File Offset: 0x000B0127
			public ShipToEntityCollisionStatus(WeakGameEntity collidingEntity, PhysicsEventType collisionEventType)
			{
				this.CollidingEntity = GameEntity.CreateFromWeakEntity(collidingEntity);
				this.CollidingBodyPtr = IntPtr.Zero;
				this.CurrentCollisionState = collisionEventType;
			}

			// Token: 0x06001B03 RID: 6915 RVA: 0x000B1F47 File Offset: 0x000B0147
			public ShipToEntityCollisionStatus(IntPtr collidingBodyPtr, PhysicsEventType collisionEventType)
			{
				this.CollidingEntity = null;
				this.CollidingBodyPtr = collidingBodyPtr;
				this.CurrentCollisionState = collisionEventType;
			}

			// Token: 0x06001B04 RID: 6916 RVA: 0x000B1F5E File Offset: 0x000B015E
			public void UpdateCurrentCollisionState(PhysicsEventType newCollisionState)
			{
				this.CurrentCollisionState = newCollisionState;
			}

			// Token: 0x04000ED5 RID: 3797
			public readonly GameEntity CollidingEntity;

			// Token: 0x04000ED6 RID: 3798
			public IntPtr CollidingBodyPtr;
		}
	}
}
