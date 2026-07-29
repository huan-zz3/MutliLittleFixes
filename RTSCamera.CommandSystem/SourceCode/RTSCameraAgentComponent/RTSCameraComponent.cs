using System;
using System.Security;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCameraAgentComponent
{
	// Token: 0x02000006 RID: 6
	public class RTSCameraComponent : AgentComponent
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020A8 File Offset: 0x000002A8
		private uint? CurrentColor
		{
			get
			{
				if (this._currentLevel >= 0)
				{
					return this._colors[this._currentLevel].Color;
				}
				return null;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000020DE File Offset: 0x000002DE
		private bool CurrentAlwaysVisible
		{
			get
			{
				return this._currentLevel < 0 || this._colors[this._currentLevel].AlwaysVisible;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600000A RID: 10 RVA: 0x00002104 File Offset: 0x00000304
		// (remove) Token: 0x0600000B RID: 11 RVA: 0x0000213C File Offset: 0x0000033C
		public event RTSCameraComponent.OnComponentRemovedDelegate OnComponentRemovedEvent;

		// Token: 0x0600000C RID: 12 RVA: 0x00002174 File Offset: 0x00000374
		public RTSCameraComponent(Agent agent)
			: base(agent)
		{
			for (int i = 0; i < this._colors.Length; i++)
			{
				this._colors[i] = new Contour(null, false);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021CC File Offset: 0x000003CC
		public void SetContourColor(int level, uint? color, bool alwaysVisible, bool updateInstantly)
		{
			if (this.SetContourColorWithoutUpdate(level, color, alwaysVisible))
			{
				this._currentLevel = ((color != null) ? level : this.EffectiveLevel(level - 1));
				if (updateInstantly)
				{
					this._shouldUpdateColor = false;
					this.SetColor();
					return;
				}
				this._shouldUpdateColor = true;
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002218 File Offset: 0x00000418
		private bool SetContourColorWithoutUpdate(int level, uint? color, bool alwaysVisible)
		{
			if (level < 0 || level >= this._colors.Length)
			{
				return false;
			}
			uint? color2 = this._colors[level].Color;
			uint? num = color;
			if ((color2.GetValueOrDefault() == num.GetValueOrDefault()) & (color2 != null == (num != null)))
			{
				return false;
			}
			this._colors[level].Color = color;
			this._colors[level].AlwaysVisible = alwaysVisible;
			return this._currentLevel <= level;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000229F File Offset: 0x0000049F
		private void UpdateColor()
		{
			this._currentLevel = this.EffectiveLevel(5);
			this._shouldUpdateColor = true;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022B8 File Offset: 0x000004B8
		[SecurityCritical]
		public void ClearContourColor()
		{
			try
			{
				for (int i = 0; i < this._colors.Length; i++)
				{
					this._colors[i].Color = null;
				}
				MBAgentVisuals agentVisuals = this.Agent.AgentVisuals;
				if (agentVisuals != null)
				{
					agentVisuals.SetContourColor(null, true);
				}
				if (this.Agent.HasMount)
				{
					MBAgentVisuals agentVisuals2 = this.Agent.MountAgent.AgentVisuals;
					if (agentVisuals2 != null)
					{
						agentVisuals2.SetContourColor(null, true);
					}
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002364 File Offset: 0x00000564
		public void ClearTargetOrSelectedFormationColor()
		{
			if (this.SetContourColorWithoutUpdate(0, null, true) | this.SetContourColorWithoutUpdate(1, null, true))
			{
				this.UpdateColor();
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000239C File Offset: 0x0000059C
		public void ClearFormationColor(bool updateInstantly)
		{
			if (this.SetContourColorWithoutUpdate(0, null, true) | this.SetContourColorWithoutUpdate(1, null, true) | this.SetContourColorWithoutUpdate(2, null, true))
			{
				this.UpdateColor();
			}
			if (updateInstantly)
			{
				this._shouldUpdateColor = false;
				this.SetColor();
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000023F8 File Offset: 0x000005F8
		[SecurityCritical]
		public override void OnMount(Agent mount)
		{
			base.OnMount(mount);
			try
			{
				MBAgentVisuals agentVisuals = mount.AgentVisuals;
				if (agentVisuals != null)
				{
					agentVisuals.SetContourColor(this.CurrentColor, this.CurrentAlwaysVisible);
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000244C File Offset: 0x0000064C
		[SecurityCritical]
		public override void OnDismount(Agent mount)
		{
			base.OnDismount(mount);
			try
			{
				if (this.CurrentColor != null)
				{
					MBAgentVisuals agentVisuals = mount.AgentVisuals;
					if (agentVisuals != null)
					{
						agentVisuals.SetContourColor(null, true);
					}
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000024B0 File Offset: 0x000006B0
		public void UpdateContour()
		{
			if (this._shouldUpdateColor)
			{
				this._shouldUpdateColor = false;
				this.SetColor();
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000024C8 File Offset: 0x000006C8
		private int EffectiveLevel(int maxLevel = 5)
		{
			for (int i = maxLevel; i > -1; i--)
			{
				if (this._colors[i].Color != null)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000024FC File Offset: 0x000006FC
		[SecurityCritical]
		private void SetColor()
		{
			try
			{
				MBAgentVisuals agentVisuals = this.Agent.AgentVisuals;
				if (agentVisuals != null)
				{
					agentVisuals.SetContourColor(this.CurrentColor, this.CurrentAlwaysVisible);
				}
				if (this.Agent.HasMount)
				{
					MBAgentVisuals agentVisuals2 = this.Agent.MountAgent.AgentVisuals;
					if (agentVisuals2 != null)
					{
						agentVisuals2.SetContourColor(this.CurrentColor, this.CurrentAlwaysVisible);
					}
				}
				this._shouldClearColorOnRemove = true;
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000258C File Offset: 0x0000078C
		public override void OnAgentRemoved()
		{
			base.OnAgentRemoved();
			if (this._shouldClearColorOnRemove)
			{
				this.ClearContourColor();
			}
			RTSCameraComponent.OnComponentRemovedDelegate onComponentRemovedEvent = this.OnComponentRemovedEvent;
			if (onComponentRemovedEvent == null)
			{
				return;
			}
			onComponentRemovedEvent(this);
		}

		// Token: 0x0400000B RID: 11
		private readonly Contour[] _colors = new Contour[6];

		// Token: 0x0400000C RID: 12
		private int _currentLevel = -1;

		// Token: 0x0400000D RID: 13
		private bool _shouldUpdateColor;

		// Token: 0x0400000E RID: 14
		private bool _shouldClearColorOnRemove;

		// Token: 0x02000007 RID: 7
		// (Invoke) Token: 0x0600001A RID: 26
		public delegate void OnComponentRemovedDelegate(RTSCameraComponent component);
	}
}
