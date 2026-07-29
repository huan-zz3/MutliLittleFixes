using System;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Logic.SubLogic;
using RTSCamera.CommandSystem.QuerySystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.AgentComponents
{
	// Token: 0x02000098 RID: 152
	public class CommandSystemAgentComponent : AgentComponent
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x000201B4 File Offset: 0x0001E3B4
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

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x000201EA File Offset: 0x0001E3EA
		private bool CurrentAlwaysVisible
		{
			get
			{
				return this._currentLevel < 0 || this._colors[this._currentLevel].AlwaysVisible;
			}
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x00020210 File Offset: 0x0001E410
		public CommandSystemAgentComponent(Agent agent)
			: base(agent)
		{
			for (int i = 0; i < this._colors.Length; i++)
			{
				this._colors[i] = new Highlight(null, false);
			}
			this._cachedDistanceUpdateTimer = new Timer(agent.Mission.CurrentTime, 0.2f + MBRandom.RandomFloat * 0.2f, true);
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x00020298 File Offset: 0x0001E498
		public void Refresh()
		{
			if (this._mesh != null)
			{
				Agent agent = this.Agent;
				if (agent != null)
				{
					MBAgentVisuals agentVisuals = agent.AgentVisuals;
					if (agentVisuals != null)
					{
						GameEntity entity = agentVisuals.GetEntity();
						if (entity != null)
						{
							entity.RemoveComponent(this._mesh);
						}
					}
				}
			}
			this.InitializeAux();
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x000202E7 File Offset: 0x0001E4E7
		public override void Initialize()
		{
			base.Initialize();
			this.Agent.SetHasOnAiInputSetCallback(true);
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x000202FC File Offset: 0x0001E4FC
		private void InitializeAux()
		{
			if (this.Agent.IsMount)
			{
				return;
			}
			this._mesh = MetaMesh.GetCopy("rts_unit_marker", true, false);
			if (this._mesh == null)
			{
				return;
			}
			if (CommandSystemAgentComponent._material == null)
			{
				Mesh meshAtIndex = this._mesh.GetMeshAtIndex(0);
				if (meshAtIndex == null)
				{
					return;
				}
				Material material = meshAtIndex.GetMaterial();
				CommandSystemAgentComponent._material = ((material != null) ? material.CreateCopy() : null);
				if (CommandSystemAgentComponent._material == null)
				{
					return;
				}
				CommandSystemAgentComponent._material.Flags |= 32;
			}
			this._mesh.SetMaterial(CommandSystemAgentComponent._material);
			this.UpdateMeshFrame(this.Agent.HasMount);
			MBAgentVisuals agentVisuals = this.Agent.AgentVisuals;
			GameEntity gameEntity = ((agentVisuals != null) ? agentVisuals.GetEntity() : null);
			if (gameEntity == null)
			{
				return;
			}
			gameEntity.AddMultiMesh(this._mesh, true);
			this._mesh.SetFactor1(CommandSystemAgentComponent.InvisibleColor);
			this._mesh.SetContourColor(CommandSystemAgentComponent.InvisibleColor);
			this._mesh.SetContourState(false);
			this._mesh.SetVisibilityMask(0);
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x0002041C File Offset: 0x0001E61C
		public static void ClearMaterial()
		{
			CommandSystemAgentComponent._material = null;
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x00020424 File Offset: 0x0001E624
		public void SetColor(int level, uint? color, bool alwaysVisible, bool updateInstantly)
		{
			if (this._mesh == null)
			{
				this.InitializeAux();
			}
			if (this.SetColorWithoutUpdate(level, color, alwaysVisible))
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

		// Token: 0x06000576 RID: 1398 RVA: 0x00020484 File Offset: 0x0001E684
		private bool SetColorWithoutUpdate(int level, uint? color, bool alwaysVisible)
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

		// Token: 0x06000577 RID: 1399 RVA: 0x0002050B File Offset: 0x0001E70B
		private void UpdateColor()
		{
			this._currentLevel = this.EffectiveLevel(5);
			this._shouldUpdateColor = true;
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x00020524 File Offset: 0x0001E724
		public static void ClearColorForAgent(Agent agent)
		{
			CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
			if (component == null)
			{
				return;
			}
			component.ClearColor();
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x00020544 File Offset: 0x0001E744
		public void ClearColor()
		{
			try
			{
				for (int i = 0; i < this._colors.Length; i++)
				{
					this._colors[i].Color = null;
				}
				if (this._mesh == null)
				{
					this.InitializeAux();
				}
				if (!(this._mesh == null))
				{
					this._mesh.SetFactor1(CommandSystemAgentComponent.InvisibleColor);
					this._mesh.SetContourColor(CommandSystemAgentComponent.InvisibleColor);
					this._mesh.SetContourState(false);
					this._mesh.SetVisibilityMask(0);
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x000205FC File Offset: 0x0001E7FC
		public void ClearTargetOrSelectedFormationColor()
		{
			if (this._mesh == null)
			{
				this.InitializeAux();
			}
			if (this._mesh == null)
			{
				return;
			}
			if (this.SetColorWithoutUpdate(0, null, true) | this.SetColorWithoutUpdate(1, null, true))
			{
				this.UpdateColor();
			}
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x00020658 File Offset: 0x0001E858
		public void ClearFormationColor(bool updateInstantly)
		{
			if (this._mesh == null)
			{
				this.InitializeAux();
			}
			if (this._mesh == null)
			{
				return;
			}
			if (this.SetColorWithoutUpdate(0, null, true) | this.SetColorWithoutUpdate(1, null, true) | this.SetColorWithoutUpdate(2, null, true))
			{
				this.UpdateColor();
			}
			if (updateInstantly)
			{
				this.TryUpdateColor();
			}
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x000206D0 File Offset: 0x0001E8D0
		public override void OnMount(Agent mount)
		{
			base.OnMount(mount);
			try
			{
				if (this._mesh != null)
				{
					this.UpdateMeshFrame(true);
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0002071C File Offset: 0x0001E91C
		public override void OnDismount(Agent mount)
		{
			base.OnDismount(mount);
			try
			{
				if (this._mesh != null)
				{
					this.UpdateMeshFrame(false);
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00020768 File Offset: 0x0001E968
		public void TryUpdateColor()
		{
			if (this._mesh == null)
			{
				this.InitializeAux();
			}
			if (this._mesh == null)
			{
				return;
			}
			if (this._shouldUpdateColor)
			{
				this._shouldUpdateColor = false;
				this.SetColor();
			}
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x000207A4 File Offset: 0x0001E9A4
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

		// Token: 0x06000580 RID: 1408 RVA: 0x000207D8 File Offset: 0x0001E9D8
		private void SetColor()
		{
			try
			{
				if (!(this._mesh == null))
				{
					uint num = ((this.CurrentColor != null) ? this.CurrentColor.Value : CommandSystemAgentComponent.InvisibleColor);
					this._mesh.SetFactor1(num);
					this._mesh.SetContourColor((Color.FromUint(num) * 0.8f).ToUnsignedInteger());
					this._mesh.SetContourState(this.CurrentAlwaysVisible);
					this._mesh.SetVisibilityMask(1);
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));
			}
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x00020890 File Offset: 0x0001EA90
		public override void OnAgentRemoved()
		{
			base.OnAgentRemoved();
			if (this._mesh == null)
			{
				return;
			}
			this.ClearColor();
			Agent agent = this.Agent;
			if (agent != null)
			{
				MBAgentVisuals agentVisuals = agent.AgentVisuals;
				if (agentVisuals != null)
				{
					GameEntity entity = agentVisuals.GetEntity();
					if (entity != null)
					{
						entity.RemoveComponent(this._mesh);
					}
				}
			}
			this._mesh = null;
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x000208F0 File Offset: 0x0001EAF0
		public void SetContourState(bool alwaysVisible)
		{
			if (this._mesh == null)
			{
				this.InitializeAux();
			}
			if (this._mesh == null)
			{
				return;
			}
			if (alwaysVisible)
			{
				this._mesh.SetContourState(true);
				return;
			}
			this._mesh.SetContourState(false);
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0002093C File Offset: 0x0001EB3C
		private void UpdateMeshFrame(bool hasMount)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = new Vec3(0f, 0.3f, 0.2f, -1f);
			Vec3 vec = -Vec3.Forward;
			identity.rotation = Mat3.CreateMat3WithForward(ref vec);
			if (hasMount)
			{
				vec = new Vec3(1.8f, 1.8f, 1f, -1f);
				identity.Scale(ref vec);
			}
			else
			{
				vec = new Vec3(1f, 1f, 1f, -1f);
				identity.Scale(ref vec);
			}
			this._mesh.Frame = identity;
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x000209E0 File Offset: 0x0001EBE0
		public override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this.Agent.Formation == null)
			{
				return;
			}
			if (!this._cachedDistanceUpdateTimer.Check(this.Agent.Mission.CurrentTime))
			{
				return;
			}
			CommandFormationQuerySystem queryForFormation = CommandQuerySystem.GetQueryForFormation(this.Agent.Formation);
			if (queryForFormation == null || !queryForFormation.NeedToUpdateTargetPositionDistance)
			{
				return;
			}
			WorldPosition orderPositionOfUnit = this.Agent.Formation.GetOrderPositionOfUnit(this.Agent);
			if (orderPositionOfUnit.IsValid)
			{
				Vec3 groundVec = orderPositionOfUnit.GetGroundVec3();
				Vec3 position = this.Agent.Position;
				this.DistanceSquaredToTargetPosition = (groundVec - position).LengthSquared;
				return;
			}
			this.DistanceSquaredToTargetPosition = 0f;
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00020A92 File Offset: 0x0001EC92
		public VolleyMode GetVolleyMode()
		{
			return this._agentAIInputHandler.VolleyMode;
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x00020A9F File Offset: 0x0001EC9F
		public void SetVolleyMode(VolleyMode volleyMode)
		{
			this._agentAIInputHandler.SetVolleyMode(this.Agent, volleyMode);
			if (volleyMode != VolleyMode.Disabled)
			{
				this.Agent.SetHasOnAiInputSetCallback(true);
			}
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00020AC2 File Offset: 0x0001ECC2
		public bool ShootUnderVolley()
		{
			return this._agentAIInputHandler.ShootUnderVolley(this.Agent);
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00020AD5 File Offset: 0x0001ECD5
		public bool IsVolleySuspended()
		{
			return this._agentAIInputHandler.IsVolleySuspended;
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00020AE2 File Offset: 0x0001ECE2
		public bool IsCandidateForNextFireAutoVolley()
		{
			return this._agentAIInputHandler.IsCandidateForNextFireInAutoVolley(this.Agent);
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x00020AF8 File Offset: 0x0001ECF8
		public WeaponClass GetCurrentlyUsingWeaponClass()
		{
			if (this.Agent.WieldedWeapon.IsEmpty)
			{
				return 0;
			}
			return this.Agent.WieldedWeapon.CurrentUsageItem.WeaponClass;
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00020B34 File Offset: 0x0001ED34
		public bool IsUsingThrownWeapon()
		{
			return !this.Agent.WieldedWeapon.IsEmpty && this.Agent.WieldedWeapon.CurrentUsageItem.IsConsumable;
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x00020B70 File Offset: 0x0001ED70
		public bool IsReadyForNextFire()
		{
			return this._agentAIInputHandler.IsReadyForNextFire(this.Agent);
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x00020B83 File Offset: 0x0001ED83
		public override void OnAIInputSet(ref Agent.EventControlFlag eventFlag, ref Agent.MovementControlFlag movementFlag, ref Vec2 inputVector)
		{
			base.OnAIInputSet(ref eventFlag, ref movementFlag, ref inputVector);
			this._agentAIInputHandler.OnAIInputSet(this.Agent, ref eventFlag, ref movementFlag, ref inputVector);
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00020BA2 File Offset: 0x0001EDA2
		public override void OnFormationSet()
		{
			base.OnFormationSet();
			this._agentAIInputHandler.OnFormationSet(this.Agent);
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00020BBB File Offset: 0x0001EDBB
		public override void OnHit(Agent affectorAgent, int damage, in MissionWeapon affectorWeapon, in Blow b, in AttackCollisionData collisionData)
		{
			base.OnHit(affectorAgent, damage, ref affectorWeapon, ref b, ref collisionData);
			this._agentAIInputHandler.OnHit(this.Agent, affectorAgent, damage, in affectorWeapon, in b, in collisionData);
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00020BE2 File Offset: 0x0001EDE2
		public void OnControllerChanged(AgentControllerType oldController)
		{
			this._agentAIInputHandler.OnControllerChanged(this.Agent, oldController);
		}

		// Token: 0x040002A1 RID: 673
		public static uint InvisibleColor;

		// Token: 0x040002A2 RID: 674
		private readonly Highlight[] _colors = new Highlight[6];

		// Token: 0x040002A3 RID: 675
		private int _currentLevel = -1;

		// Token: 0x040002A4 RID: 676
		private bool _shouldUpdateColor;

		// Token: 0x040002A5 RID: 677
		public float DistanceSquaredToTargetPosition;

		// Token: 0x040002A6 RID: 678
		private Timer _cachedDistanceUpdateTimer;

		// Token: 0x040002A7 RID: 679
		private MetaMesh _mesh;

		// Token: 0x040002A8 RID: 680
		private static Material _material;

		// Token: 0x040002A9 RID: 681
		private AgentAIInputHandler _agentAIInputHandler = new AgentAIInputHandler();
	}
}
