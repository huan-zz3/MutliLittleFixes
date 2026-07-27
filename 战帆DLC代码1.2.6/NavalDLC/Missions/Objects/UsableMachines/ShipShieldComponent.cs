using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000BA RID: 186
	public class ShipShieldComponent : DestructableComponent
	{
		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06000E33 RID: 3635 RVA: 0x0006F06D File Offset: 0x0006D26D
		public override bool IsFocusable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x0006F070 File Offset: 0x0006D270
		private ShipShieldComponent()
		{
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x0006F083 File Offset: 0x0006D283
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x0006F098 File Offset: 0x0006D298
		public void RegisterRampEntityDisablingShield(GameEntity connectionEntity)
		{
			if (this._disablingConnectionEntities.Count == 0)
			{
				base.GameEntity.SetVisibilityExcludeParents(false);
			}
			this._disablingConnectionEntities.Add(connectionEntity);
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x0006F0D0 File Offset: 0x0006D2D0
		public void DeregisterRampEntityDisablingShield(GameEntity connectionEntity)
		{
			if (this._disablingConnectionEntities.Remove(connectionEntity) && this._disablingConnectionEntities.Count == 0)
			{
				base.GameEntity.SetVisibilityExcludeParents(true);
			}
		}

		// Token: 0x040008DE RID: 2270
		private List<GameEntity> _disablingConnectionEntities = new List<GameEntity>();
	}
}
