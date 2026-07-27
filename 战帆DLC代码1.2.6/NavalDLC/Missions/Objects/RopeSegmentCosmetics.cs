using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A0 RID: 160
	[ScriptComponentParams("ship_visual_only", "rope_segment_cosmetics")]
	internal class RopeSegmentCosmetics : ScriptComponentBehavior
	{
		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000C44 RID: 3140 RVA: 0x00058D63 File Offset: 0x00056F63
		// (set) Token: 0x06000C45 RID: 3141 RVA: 0x00058D6B File Offset: 0x00056F6B
		public bool IsBurningNode { get; private set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000C46 RID: 3142 RVA: 0x00058D74 File Offset: 0x00056F74
		// (set) Token: 0x06000C47 RID: 3143 RVA: 0x00058D7C File Offset: 0x00056F7C
		public float RopeLocalPosition
		{
			get
			{
				return this._ropeLocalPosition;
			}
			set
			{
				this._ropeLocalPosition = value;
			}
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x00058D85 File Offset: 0x00056F85
		protected override void OnInit()
		{
			base.OnInit();
			this.FetchEntities();
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x00058D93 File Offset: 0x00056F93
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this.FetchEntities();
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x00058DA1 File Offset: 0x00056FA1
		protected override void OnEditorTick(float dt)
		{
			this.FetchEntities();
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x00058DAC File Offset: 0x00056FAC
		private void FetchEntities()
		{
			this.IsBurningNode = base.GameEntity.HasTag("burning_node");
		}

		// Token: 0x04000726 RID: 1830
		[EditableScriptComponentVariable(true, "Normalized Location wrt Rope")]
		private float _ropeLocalPosition = 0.5f;
	}
}
