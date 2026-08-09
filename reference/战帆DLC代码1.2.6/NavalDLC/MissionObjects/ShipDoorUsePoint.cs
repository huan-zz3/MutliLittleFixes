using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.MissionObjects
{
	// Token: 0x020000FB RID: 251
	public class ShipDoorUsePoint : UsableMissionObject
	{
		// Token: 0x060012A3 RID: 4771 RVA: 0x000892AD File Offset: 0x000874AD
		public ShipDoorUsePoint()
			: base(false)
		{
			this._actionStringId = string.Empty;
			this._descriptionStringId = string.Empty;
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x000892CC File Offset: 0x000874CC
		protected override void OnInit()
		{
			base.OnInit();
			this._isEnabled = false;
			this.ActionMessage = GameTexts.FindText(string.IsNullOrEmpty(this._actionStringId) ? "str_open_ship_door" : this._actionStringId, null);
			this.ActionMessage.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			this.DescriptionMessage = GameTexts.FindText(string.IsNullOrEmpty(this._descriptionStringId) ? "str_ui_door" : this._descriptionStringId, null);
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x00089359 File Offset: 0x00087559
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			return this.DescriptionMessage;
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x00089364 File Offset: 0x00087564
		public override void OnUse(Agent userAgent, sbyte agentBoneIndex)
		{
			base.OnUse(userAgent, agentBoneIndex);
			if (userAgent.IsMainAgent)
			{
				string text = "event:/mission/movement/foley/door_open";
				Vec3 position = userAgent.Position;
				SoundManager.StartOneShotEvent(text, ref position);
				userAgent.StopUsingGameObject(true, 1);
			}
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x0008939D File Offset: 0x0008759D
		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			if (this.LockUserFrames || this.LockUserPositions)
			{
				userAgent.ClearTargetFrame();
			}
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x000893BE File Offset: 0x000875BE
		public override bool IsDisabledForAgent(Agent agent)
		{
			return !this._isEnabled && !agent.IsMainAgent;
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x000893D4 File Offset: 0x000875D4
		public override bool IsUsableByAgent(Agent userAgent)
		{
			return this._isEnabled && userAgent.IsMainAgent && base.GameEntity.GlobalPosition.Distance(Agent.Main.Position) <= 2f;
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x00089420 File Offset: 0x00087620
		public void SetShipDoorUsePointEnabled(bool isEnabled)
		{
			if (this._isEnabled != isEnabled || this._highlight == null)
			{
				this._isEnabled = isEnabled;
				if (this._highlight == null)
				{
					foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
					{
						if (weakGameEntity.HasTag("ship_door_highlight"))
						{
							this._highlight = GameEntity.CreateFromWeakEntity(weakGameEntity);
						}
					}
				}
				GameEntity highlight = this._highlight;
				if (highlight == null)
				{
					return;
				}
				highlight.SetVisibilityExcludeParents(false);
			}
		}

		// Token: 0x04000A84 RID: 2692
		private const string ShipDoorHighlightTag = "ship_door_highlight";

		// Token: 0x04000A85 RID: 2693
		private GameEntity _highlight;

		// Token: 0x04000A86 RID: 2694
		private bool _isEnabled;

		// Token: 0x04000A87 RID: 2695
		[EditableScriptComponentVariable(true, "ActionStringId")]
		private string _actionStringId;

		// Token: 0x04000A88 RID: 2696
		[EditableScriptComponentVariable(true, "DescriptionStringId")]
		private string _descriptionStringId;
	}
}
