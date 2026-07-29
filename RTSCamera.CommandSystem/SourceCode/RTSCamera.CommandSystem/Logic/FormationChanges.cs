using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Logic
{
	// Token: 0x02000084 RID: 132
	public class FormationChanges
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060004ED RID: 1261 RVA: 0x0001CF8C File Offset: 0x0001B18C
		// (set) Token: 0x060004EE RID: 1262 RVA: 0x0001CF94 File Offset: 0x0001B194
		public Dictionary<Formation, FormationChange> VirtualChanges { get; set; } = new Dictionary<Formation, FormationChange>();

		// Token: 0x060004EF RID: 1263 RVA: 0x0001CFA0 File Offset: 0x0001B1A0
		public void SetChanges(IEnumerable<KeyValuePair<Formation, FormationChange>> virtualPositions)
		{
			foreach (KeyValuePair<Formation, FormationChange> keyValuePair in virtualPositions)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(keyValuePair.Key, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.WorldPosition = keyValuePair.Value.WorldPosition;
				formationChange.Direciton = keyValuePair.Value.Direciton;
				formationChange.UnitSpacing = keyValuePair.Value.UnitSpacing;
				formationChange.Width = keyValuePair.Value.Width;
				formationChange.MovementOrderType = keyValuePair.Value.MovementOrderType;
				formationChange.TargetFormation = keyValuePair.Value.TargetFormation;
				formationChange.TargetAgent = keyValuePair.Value.TargetAgent;
				formationChange.FacingEnemyTargetFormation = keyValuePair.Value.FacingEnemyTargetFormation;
				formationChange.TargetEntity = keyValuePair.Value.TargetEntity;
				formationChange.FacingOrderType = keyValuePair.Value.FacingOrderType;
				formationChange.FiringOrderType = keyValuePair.Value.FiringOrderType;
				formationChange.RidingOrderType = keyValuePair.Value.RidingOrderType;
				formationChange.ArrangementOrder = keyValuePair.Value.ArrangementOrder;
				formationChange.VolleyMode = keyValuePair.Value.VolleyMode;
				formationChange.PreviewWidth = keyValuePair.Value.PreviewWidth;
				formationChange.PreviewDepth = keyValuePair.Value.PreviewDepth;
				this.VirtualChanges[keyValuePair.Key] = formationChange;
			}
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0001D154 File Offset: 0x0001B354
		public Dictionary<Formation, FormationChange> CollectChanges(IEnumerable<Formation> formations)
		{
			return this.VirtualChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => formations.Contains(pair.Key)).ToDictionary<KeyValuePair<Formation, FormationChange>, Formation, FormationChange>((KeyValuePair<Formation, FormationChange> pair) => pair.Key, (KeyValuePair<Formation, FormationChange> pair) => pair.Value);
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0001D1C8 File Offset: 0x0001B3C8
		public void UpdateFormationChange(Formation formation, WorldPosition? position, Vec2? direction, int? unitSpacing, float? width)
		{
			FormationChange formationChange;
			if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
			{
				formationChange = default(FormationChange);
			}
			if (position != null)
			{
				formationChange.WorldPosition = new WorldPosition?(position.Value);
			}
			if (direction != null)
			{
				formationChange.Direciton = new Vec2?(direction.Value);
			}
			if (unitSpacing != null)
			{
				formationChange.UnitSpacing = new int?(unitSpacing.Value);
			}
			if (width != null)
			{
				formationChange.Width = new float?(width.Value);
			}
			this.VirtualChanges[formation] = formationChange;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0001D26C File Offset: 0x0001B46C
		public void SetMovementOrder(OrderType orderType, IEnumerable<Formation> formations, Formation targetFormation, Agent targetAgent, IOrderable targetEntity)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.MovementOrderType = new OrderType?(orderType);
				formationChange.TargetFormation = targetFormation;
				formationChange.TargetAgent = targetAgent;
				formationChange.TargetEntity = targetEntity;
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001D2FC File Offset: 0x0001B4FC
		public void SetFacingOrder(OrderType orderType, IEnumerable<Formation> formations, Formation targetFormation = null)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.FacingOrderType = new OrderType?(orderType);
				formationChange.FacingEnemyTargetFormation = targetFormation;
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0001D378 File Offset: 0x0001B578
		public void SetFacingOrder(OrderType orderType, Formation formation, Formation targetFormation = null)
		{
			FormationChange formationChange;
			if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
			{
				formationChange = default(FormationChange);
			}
			formationChange.FacingOrderType = new OrderType?(orderType);
			formationChange.FacingEnemyTargetFormation = targetFormation;
			this.VirtualChanges[formation] = formationChange;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0001D3C0 File Offset: 0x0001B5C0
		public void ClearFacingOrderTarget(IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.FacingEnemyTargetFormation = null;
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001D430 File Offset: 0x0001B630
		public void ClearFacingOrderTarget(Formation formation)
		{
			FormationChange formationChange;
			if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
			{
				formationChange = default(FormationChange);
			}
			formationChange.FacingEnemyTargetFormation = null;
			this.VirtualChanges[formation] = formationChange;
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0001D46C File Offset: 0x0001B66C
		public void SetToggleOrder(OrderType orderType, IEnumerable<Formation> formations)
		{
			switch (orderType)
			{
			case 31:
			case 32:
				this.SetFiringOrder(orderType, formations);
				return;
			case 33:
				break;
			case 34:
			case 35:
				this.SetRidingOrder(orderType, formations);
				return;
			case 36:
			case 37:
				this.SetAIControlOrder(orderType, formations);
				break;
			default:
				return;
			}
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001D4BC File Offset: 0x0001B6BC
		public void SetFiringOrder(OrderType orderType, IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.FiringOrderType = new OrderType?(orderType);
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001D530 File Offset: 0x0001B730
		public void ClearFiringOrder(IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					break;
				}
				formationChange.FiringOrderType = null;
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x0001D5A0 File Offset: 0x0001B7A0
		public void SetRidingOrder(OrderType orderType, IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.RidingOrderType = new OrderType?(orderType);
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0001D614 File Offset: 0x0001B814
		public void ClearRidingOrder(IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					break;
				}
				formationChange.RidingOrderType = null;
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0001D684 File Offset: 0x0001B884
		public void SetArrangementOrder(ArrangementOrder.ArrangementOrderEnum newArrangementOrder, IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.ArrangementOrder = new ArrangementOrder.ArrangementOrderEnum?(newArrangementOrder);
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001D6F8 File Offset: 0x0001B8F8
		public void SetAIControlOrder(OrderType orderType, IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.AIControlOrderType = new OrderType?(orderType);
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0001D76C File Offset: 0x0001B96C
		public void SetVolleyMode(VolleyMode volleyMode, IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					formationChange = default(FormationChange);
				}
				formationChange.VolleyMode = new VolleyMode?(volleyMode);
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0001D7E0 File Offset: 0x0001B9E0
		public void ClearVolleyEnabledOrder(IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				FormationChange formationChange;
				if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
				{
					break;
				}
				formationChange.VolleyMode = null;
				this.VirtualChanges[formation] = formationChange;
			}
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0001D850 File Offset: 0x0001BA50
		public void SetPreviewShape(Formation formation, float width, float depth)
		{
			FormationChange formationChange;
			if (!this.VirtualChanges.TryGetValue(formation, out formationChange))
			{
				formationChange = default(FormationChange);
			}
			formationChange.PreviewWidth = new float?(width);
			formationChange.PreviewDepth = new float?(depth);
			this.VirtualChanges[formation] = formationChange;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0001D89C File Offset: 0x0001BA9C
		private static float TransformCustomWidthBetweenArrangementOrientations(ArrangementOrder.ArrangementOrderEnum orderTypeOld, ArrangementOrder.ArrangementOrderEnum orderTypeNew, float currentCustomWidth)
		{
			if (orderTypeOld == null && orderTypeNew != null && orderTypeNew != 1)
			{
				return (float)((double)currentCustomWidth / 3.141592653589793);
			}
			if (orderTypeOld != 1 && orderTypeNew == 1)
			{
				return currentCustomWidth * 0.1f;
			}
			if (orderTypeOld != 1 || orderTypeNew == 1)
			{
				return currentCustomWidth;
			}
			return currentCustomWidth / 0.1f;
		}
	}
}
