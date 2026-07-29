using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.QuerySystem
{
	// Token: 0x02000009 RID: 9
	public class UiQueryData<T> : IQueryData
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00003F4C File Offset: 0x0000214C
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00003F54 File Offset: 0x00002154
		public string TelemetryScopeName { get; set; }

		// Token: 0x06000075 RID: 117 RVA: 0x00003F5D File Offset: 0x0000215D
		public UiQueryData(Func<T> valueFunc, float lifetime)
		{
			this._cachedValue = default(T);
			this._expireTime = 0f;
			this._lifetime = lifetime;
			this._valueFunc = valueFunc;
			this._syncGroup = null;
			this.TelemetryScopeName = "QueryDataNameUninitialized";
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003F9C File Offset: 0x0000219C
		public void Evaluate(float currentTime)
		{
			this.SetValue(this._valueFunc(), currentTime);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003FB0 File Offset: 0x000021B0
		public void SetValue(T value, float currentTime)
		{
			this._cachedValue = value;
			this._expireTime = currentTime + this._lifetime;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00003FC7 File Offset: 0x000021C7
		public T GetCachedValue()
		{
			return this._cachedValue;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00003FCF File Offset: 0x000021CF
		public T GetCachedValueWithMaxAge(float age)
		{
			if ((double)MBCommon.GetApplicationTime() <= (double)this._expireTime - (double)this._lifetime + (double)Math.Min(this._lifetime, age))
			{
				return this._cachedValue;
			}
			this.Expire();
			return this.Value;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600007A RID: 122 RVA: 0x0000400C File Offset: 0x0000220C
		public T Value
		{
			get
			{
				float applicationTime = MBCommon.GetApplicationTime();
				if ((double)applicationTime >= (double)this._expireTime)
				{
					if (this._syncGroup != null)
					{
						foreach (IQueryData queryData in this._syncGroup)
						{
							queryData.Evaluate(applicationTime);
						}
					}
					this.Evaluate(applicationTime);
				}
				return this._cachedValue;
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004080 File Offset: 0x00002280
		public void Expire()
		{
			this._expireTime = 0f;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004090 File Offset: 0x00002290
		public static void SetupSyncGroup(params IQueryData[] groupItems)
		{
			for (int i = 0; i < groupItems.Length; i++)
			{
				groupItems[i].SetSyncGroup(groupItems);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000040B6 File Offset: 0x000022B6
		public void SetSyncGroup(IQueryData[] syncGroup)
		{
			this._syncGroup = syncGroup;
		}

		// Token: 0x04000026 RID: 38
		private T _cachedValue;

		// Token: 0x04000027 RID: 39
		private float _expireTime;

		// Token: 0x04000028 RID: 40
		private readonly float _lifetime;

		// Token: 0x04000029 RID: 41
		private readonly Func<T> _valueFunc;

		// Token: 0x0400002A RID: 42
		private IEnumerable<IQueryData> _syncGroup;
	}
}
