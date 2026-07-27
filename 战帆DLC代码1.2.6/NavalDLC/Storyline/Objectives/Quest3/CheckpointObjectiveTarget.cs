using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x0200005A RID: 90
	internal class CheckpointObjectiveTarget : MissionObjectiveTarget
	{
		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x0002273F File Offset: 0x0002093F
		// (set) Token: 0x060005A9 RID: 1449 RVA: 0x00022747 File Offset: 0x00020947
		public GameEntity GameEntity { get; private set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x00022750 File Offset: 0x00020950
		// (set) Token: 0x060005AB RID: 1451 RVA: 0x00022758 File Offset: 0x00020958
		public bool Active { get; private set; }

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x00022761 File Offset: 0x00020961
		// (set) Token: 0x060005AD RID: 1453 RVA: 0x00022769 File Offset: 0x00020969
		public VolumeBox VolumeBox { get; private set; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x00022772 File Offset: 0x00020972
		// (set) Token: 0x060005AF RID: 1455 RVA: 0x0002277A File Offset: 0x0002097A
		public float Radius { get; private set; } = 20f;

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x00022783 File Offset: 0x00020983
		// (set) Token: 0x060005B1 RID: 1457 RVA: 0x0002278B File Offset: 0x0002098B
		public TextObject Name { get; private set; }

		// Token: 0x060005B2 RID: 1458 RVA: 0x00022794 File Offset: 0x00020994
		public CheckpointObjectiveTarget(GameEntity gameEntity)
		{
			this.GameEntity = gameEntity;
			GameEntity gameEntity2 = this.GameEntity;
			this.VolumeBox = ((gameEntity2 != null) ? gameEntity2.GetFirstScriptOfType<VolumeBox>() : null);
			this.Active = false;
			this.Name = TextObject.GetEmpty();
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x000227E3 File Offset: 0x000209E3
		public void SetActive(bool isActive)
		{
			this.Active = isActive;
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x000227EC File Offset: 0x000209EC
		public void SetRadius(float radius)
		{
			this.Radius = radius;
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x000227F5 File Offset: 0x000209F5
		public void SetName(TextObject name)
		{
			this.Name = name;
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x000227FE File Offset: 0x000209FE
		public override Vec3 GetGlobalPosition()
		{
			return this.GameEntity.GlobalPosition;
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x0002280C File Offset: 0x00020A0C
		public bool IsInside(Vec3 position)
		{
			if (this.VolumeBox != null)
			{
				return this.VolumeBox.IsPointIn(position);
			}
			return this.GetGlobalPosition().DistanceSquared(position) <= this.Radius * this.Radius;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x0002284F File Offset: 0x00020A4F
		public override TextObject GetName()
		{
			return this.Name;
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00022857 File Offset: 0x00020A57
		public override bool IsActive()
		{
			return this.Active;
		}
	}
}
