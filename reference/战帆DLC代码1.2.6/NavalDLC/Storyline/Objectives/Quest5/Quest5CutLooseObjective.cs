using System;
using System.Linq;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000049 RID: 73
	public class Quest5CutLooseObjective : MissionObjective
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x00021D73 File Offset: 0x0001FF73
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_5_cut_loose_objective";
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000542 RID: 1346 RVA: 0x00021D7A File Offset: 0x0001FF7A
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=1IpNoNL4}Cut Loose", null);
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000543 RID: 1347 RVA: 0x00021D87 File Offset: 0x0001FF87
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=2cCuu7kv}Cut the prisoner ship loose, so you can sail it to safety.", null);
			}
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00021D94 File Offset: 0x0001FF94
		public Quest5CutLooseObjective(Mission mission, MBReadOnlyList<ShipAttachmentMachine> attachmentMachines, MBReadOnlyList<ShipAttachmentPointMachine> attachmentPointMachines)
			: base(mission)
		{
			this._attachmentMachines = attachmentMachines;
			this._attachmentPointMachines = attachmentPointMachines;
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null)
				{
					Quest5CutLooseObjective.CutLooseObjectiveTarget cutLooseObjectiveTarget = new Quest5CutLooseObjective.CutLooseObjectiveTarget(shipAttachmentMachine);
					base.AddTarget(cutLooseObjectiveTarget);
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null)
				{
					Quest5CutLooseObjective.CutLooseObjectiveTarget cutLooseObjectiveTarget2 = new Quest5CutLooseObjective.CutLooseObjectiveTarget(shipAttachmentPointMachine);
					base.AddTarget(cutLooseObjectiveTarget2);
				}
			}
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00021E64 File Offset: 0x00020064
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._attachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment == null)
				{
					foreach (StandingPoint standingPoint in shipAttachmentMachine.StandingPoints)
					{
						standingPoint.IsDisabledForPlayers = true;
					}
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._attachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment == null)
				{
					foreach (StandingPoint standingPoint2 in shipAttachmentPointMachine.StandingPoints)
					{
						standingPoint2.IsDisabledForPlayers = true;
					}
				}
			}
			MBReadOnlyList<Quest5CutLooseObjective.CutLooseObjectiveTarget> targetsCopy = base.GetTargetsCopy<Quest5CutLooseObjective.CutLooseObjectiveTarget>();
			this._cachedProgress.RequiredProgressAmount = targetsCopy.Count;
			this._cachedProgress.CurrentProgressAmount = targetsCopy.Count<Quest5CutLooseObjective.CutLooseObjectiveTarget>((Quest5CutLooseObjective.CutLooseObjectiveTarget t) => t.IsCutLoose());
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00021FD0 File Offset: 0x000201D0
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00021FD8 File Offset: 0x000201D8
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00021FDB File Offset: 0x000201DB
		protected override bool IsCompletionRequirementsMet()
		{
			return this._cachedProgress.CurrentProgressAmount == this._cachedProgress.RequiredProgressAmount;
		}

		// Token: 0x040002A8 RID: 680
		private readonly MBReadOnlyList<ShipAttachmentMachine> _attachmentMachines;

		// Token: 0x040002A9 RID: 681
		private readonly MBReadOnlyList<ShipAttachmentPointMachine> _attachmentPointMachines;

		// Token: 0x040002AA RID: 682
		private MissionObjectiveProgressInfo _cachedProgress;

		// Token: 0x020001C7 RID: 455
		private class CutLooseObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x06001A01 RID: 6657 RVA: 0x000AE5D7 File Offset: 0x000AC7D7
			public CutLooseObjectiveTarget(ShipAttachmentMachine attachmentMachine)
			{
				this._attachmentMachine = attachmentMachine;
			}

			// Token: 0x06001A02 RID: 6658 RVA: 0x000AE5E6 File Offset: 0x000AC7E6
			public CutLooseObjectiveTarget(ShipAttachmentPointMachine attachmentPointMachine)
			{
				this._attachmentPointMachine = attachmentPointMachine;
			}

			// Token: 0x06001A03 RID: 6659 RVA: 0x000AE5F5 File Offset: 0x000AC7F5
			public override bool IsActive()
			{
				return !this.IsCutLoose();
			}

			// Token: 0x06001A04 RID: 6660 RVA: 0x000AE600 File Offset: 0x000AC800
			public bool IsCutLoose()
			{
				if (this._attachmentMachine != null)
				{
					return this._attachmentMachine.CurrentAttachment == null;
				}
				return this._attachmentPointMachine == null || this._attachmentPointMachine.CurrentAttachment == null;
			}

			// Token: 0x06001A05 RID: 6661 RVA: 0x000AE634 File Offset: 0x000AC834
			public override Vec3 GetGlobalPosition()
			{
				if (this._attachmentMachine != null)
				{
					return this._attachmentMachine.GameEntity.GlobalPosition;
				}
				if (this._attachmentPointMachine != null)
				{
					return this._attachmentPointMachine.GameEntity.GlobalPosition;
				}
				return Vec3.Zero;
			}

			// Token: 0x06001A06 RID: 6662 RVA: 0x000AE67E File Offset: 0x000AC87E
			public override TextObject GetName()
			{
				return new TextObject("{=Cx5qU2jG}Ties", null);
			}

			// Token: 0x04000D3C RID: 3388
			private readonly ShipAttachmentMachine _attachmentMachine;

			// Token: 0x04000D3D RID: 3389
			private readonly ShipAttachmentPointMachine _attachmentPointMachine;
		}
	}
}
