using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x02000074 RID: 116
	public class RTSCommandGenericVisualOrderSet : VisualOrderSet
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000439 RID: 1081 RVA: 0x000194CA File Offset: 0x000176CA
		public override bool IsSoloOrder
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x000194CD File Offset: 0x000176CD
		public override string StringId
		{
			get
			{
				return this._stringId;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x000194D8 File Offset: 0x000176D8
		public override string IconId
		{
			get
			{
				if (this._useActiveOrderForIconId)
				{
					for (int i = 0; i < base.Orders.Count; i++)
					{
						if (base.Orders[i].GetActiveState(Mission.Current.PlayerTeam.PlayerOrderController) == 3)
						{
							return base.Orders[i].IconId;
						}
					}
				}
				if (this._otherOrder != null)
				{
					return this._otherOrder.IconId;
				}
				return this._stringId;
			}
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00019554 File Offset: 0x00017754
		public override TextObject GetName(OrderController orderController)
		{
			if (this._useActiveOrderForName)
			{
				for (int i = 0; i < base.Orders.Count; i++)
				{
					if (base.Orders[i].GetActiveState(orderController) == 3)
					{
						return base.Orders[i].GetName(orderController);
					}
				}
			}
			if (this._otherOrder != null)
			{
				return this._otherOrder.GetName(orderController);
			}
			return this._name;
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x000195C2 File Offset: 0x000177C2
		public RTSCommandGenericVisualOrderSet(string stringId, TextObject name, bool useActiveOrderForIconId, bool useActiveOrderForName, VisualOrder otherOrder)
		{
			this._stringId = stringId;
			this._name = name;
			this._useActiveOrderForIconId = useActiveOrderForIconId;
			this._useActiveOrderForName = useActiveOrderForName;
			this._otherOrder = otherOrder;
		}

		// Token: 0x040001B1 RID: 433
		private readonly TextObject _name;

		// Token: 0x040001B2 RID: 434
		private readonly string _stringId;

		// Token: 0x040001B3 RID: 435
		private readonly bool _useActiveOrderForIconId;

		// Token: 0x040001B4 RID: 436
		private readonly bool _useActiveOrderForName;

		// Token: 0x040001B5 RID: 437
		private readonly VisualOrder _otherOrder;
	}
}
