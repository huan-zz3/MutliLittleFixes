using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MissionSharedLibrary.Config;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Logic
{
	// Token: 0x02000080 RID: 128
	public class OrderInQueue
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x0001A32F File Offset: 0x0001852F
		// (set) Token: 0x06000475 RID: 1141 RVA: 0x0001A337 File Offset: 0x00018537
		public List<Formation> SelectedFormations
		{
			get
			{
				return this._selectedFormation;
			}
			set
			{
				this._selectedFormation = value.ToList<Formation>();
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x0001A345 File Offset: 0x00018545
		// (set) Token: 0x06000477 RID: 1143 RVA: 0x0001A34D File Offset: 0x0001854D
		public List<Formation> RemainingFormations { get; set; } = new List<Formation>();

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x0001A356 File Offset: 0x00018556
		// (set) Token: 0x06000479 RID: 1145 RVA: 0x0001A35E File Offset: 0x0001855E
		public CustomOrderType CustomOrderType { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x0001A367 File Offset: 0x00018567
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x0001A36F File Offset: 0x0001856F
		public OrderType OrderType { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x0001A378 File Offset: 0x00018578
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x0001A380 File Offset: 0x00018580
		public WorldPosition PositionBegin { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x0001A389 File Offset: 0x00018589
		// (set) Token: 0x0600047F RID: 1151 RVA: 0x0001A391 File Offset: 0x00018591
		public WorldPosition PositionEnd { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000480 RID: 1152 RVA: 0x0001A39A File Offset: 0x0001859A
		// (set) Token: 0x06000481 RID: 1153 RVA: 0x0001A3A2 File Offset: 0x000185A2
		public Formation TargetFormation { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x0001A3AB File Offset: 0x000185AB
		// (set) Token: 0x06000483 RID: 1155 RVA: 0x0001A3B3 File Offset: 0x000185B3
		public Agent TargetAgent { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x0001A3BC File Offset: 0x000185BC
		// (set) Token: 0x06000485 RID: 1157 RVA: 0x0001A3C4 File Offset: 0x000185C4
		public IOrderable TargetEntity { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x0001A3CD File Offset: 0x000185CD
		// (set) Token: 0x06000487 RID: 1159 RVA: 0x0001A3D5 File Offset: 0x000185D5
		public bool IsStopUsing { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000488 RID: 1160 RVA: 0x0001A3DE File Offset: 0x000185DE
		// (set) Token: 0x06000489 RID: 1161 RVA: 0x0001A3E6 File Offset: 0x000185E6
		public bool IsLineShort { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x0001A3EF File Offset: 0x000185EF
		// (set) Token: 0x0600048B RID: 1163 RVA: 0x0001A3F7 File Offset: 0x000185F7
		public Dictionary<Formation, bool> ShouldLockFormationInFacingOrder { get; set; } = new Dictionary<Formation, bool>();

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x0001A400 File Offset: 0x00018600
		// (set) Token: 0x0600048D RID: 1165 RVA: 0x0001A408 File Offset: 0x00018608
		[TupleElementNames(new string[] { "formation", "unitSpacingReduced", "customWidth", "position", "direction" })]
		public List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> ActualFormationChanges
		{
			[return: TupleElementNames(new string[] { "formation", "unitSpacingReduced", "customWidth", "position", "direction" })]
			get;
			[param: TupleElementNames(new string[] { "formation", "unitSpacingReduced", "customWidth", "position", "direction" })]
			set;
		} = new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>();

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x0001A411 File Offset: 0x00018611
		// (set) Token: 0x0600048F RID: 1167 RVA: 0x0001A419 File Offset: 0x00018619
		public Dictionary<Formation, FormationChange> VirtualFormationChanges { get; set; } = new Dictionary<Formation, FormationChange>();

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000490 RID: 1168 RVA: 0x0001A422 File Offset: 0x00018622
		// (set) Token: 0x06000491 RID: 1169 RVA: 0x0001A42A File Offset: 0x0001862A
		public bool ShouldAdjustFormationSpeed { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000492 RID: 1170 RVA: 0x0001A433 File Offset: 0x00018633
		// (set) Token: 0x06000493 RID: 1171 RVA: 0x0001A43B File Offset: 0x0001863B
		public Dictionary<Formation, float> FormationSpeedLimits { get; set; } = new Dictionary<Formation, float>();

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x0001A444 File Offset: 0x00018644
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x0001A44C File Offset: 0x0001864C
		public Dictionary<Formation, Vec2> FormationExpectedPositions { get; set; } = new Dictionary<Formation, Vec2>();

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x0001A455 File Offset: 0x00018655
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x0001A45D File Offset: 0x0001865D
		public Dictionary<Formation, float> FormationTargetDistances { get; set; } = new Dictionary<Formation, float>();

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x0001A466 File Offset: 0x00018666
		// (set) Token: 0x06000499 RID: 1177 RVA: 0x0001A46E File Offset: 0x0001866E
		public float MaxDuration { get; set; } = 1f;

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x0001A477 File Offset: 0x00018677
		// (set) Token: 0x0600049B RID: 1179 RVA: 0x0001A47F File Offset: 0x0001867F
		public float DistanceWithMaxDuration { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x0001A488 File Offset: 0x00018688
		// (set) Token: 0x0600049D RID: 1181 RVA: 0x0001A490 File Offset: 0x00018690
		public float MinSpeed { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600049E RID: 1182 RVA: 0x0001A499 File Offset: 0x00018699
		// (set) Token: 0x0600049F RID: 1183 RVA: 0x0001A4A1 File Offset: 0x000186A1
		public bool IsAdjustingSpeedMessageShown { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x0001A4AA File Offset: 0x000186AA
		// (set) Token: 0x060004A1 RID: 1185 RVA: 0x0001A4B2 File Offset: 0x000186B2
		public bool IsExecutingOrderMessageShown { get; set; }

		// Token: 0x060004A2 RID: 1186 RVA: 0x0001A4BC File Offset: 0x000186BC
		public void UpdateMovementSpeed()
		{
			this.FormationSpeedLimits.Clear();
			this.FormationExpectedPositions.Clear();
			this.FormationTargetDistances.Clear();
			this.MaxDuration = 1f;
			this.DistanceWithMaxDuration = 0f;
			this.MinSpeed = float.MaxValue;
			if (!this.ShouldAdjustFormationSpeed)
			{
				return;
			}
			if (MissionConfigBase<CommandSystemConfig>.Get().FormationSpeedSyncMode == FormationSpeedSyncMode.Disabled)
			{
				return;
			}
			Dictionary<Formation, float> dictionary = new Dictionary<Formation, float>();
			Dictionary<Formation, float> dictionary2 = new Dictionary<Formation, float>();
			float num = float.MinValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MaxValue;
			float num5 = float.MaxValue;
			foreach (Formation formation in this.SelectedFormations)
			{
				OrderInQueue orderInQueue;
				if (formation.CountOfUnits != 0 && CommandQueueLogic.PendingOrders.TryGetValue(formation, out orderInQueue))
				{
					bool flag = Utility.FormationArrangementContainsPlayerOnly(formation);
					if (this == orderInQueue && (!CommandQueueLogic.IsMovementOrderCompleted(formation, this) || flag))
					{
						float num6 = MathF.Max(0.1f, formation.CachedMovementSpeed);
						FormationChange formationChange;
						if (this.VirtualFormationChanges.TryGetValue(formation, out formationChange))
						{
							WorldPosition? worldPosition = formationChange.WorldPosition;
							if (worldPosition != null && worldPosition.Value.IsValid)
							{
								Formation.FormationIntegrityDataGroup cachedFormationIntegrityData = formation.CachedFormationIntegrityData;
								float num7 = cachedFormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents * 3f;
								if (cachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents > num7)
								{
									return;
								}
								float num8 = worldPosition.Value.AsVec2.Distance(formation.CurrentPosition);
								if (num8 >= 0.1f)
								{
									if (num8 > num3)
									{
										num3 = num8;
									}
									if (num8 < num4)
									{
										num4 = num8;
									}
									dictionary[formation] = num8;
									float num9 = num8 / num6;
									dictionary2[formation] = num9;
									if (num9 > num && !flag)
									{
										num = num9;
										num2 = num8;
									}
									num5 = MathF.Min(num5, num6);
								}
							}
						}
					}
				}
			}
			Vec2 vec;
			Vec2 vec2;
			foreach (KeyValuePair<Formation, Vec2> keyValuePair in Utility.CollectFormationCurrentAndOrderPositions(dictionary.Keys, out vec, out vec2))
			{
				Formation key = keyValuePair.Key;
				Vec2 value = keyValuePair.Value;
				Vec2 vec3 = vec2 - vec + value;
				this.FormationExpectedPositions[key] = vec3;
			}
			this.MaxDuration = num;
			this.DistanceWithMaxDuration = num2;
			this.FormationTargetDistances = dictionary;
			this.MinSpeed = num5;
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001A76C File Offset: 0x0001896C
		private float GetMaxDistanceSpeed(Dictionary<Formation, float> targetDistances, float distance, float minDistance, float maxDistance, float maxOriginalDuration, float distanceWithMaxDuration, float range)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = float.MaxValue;
			float num4 = 0f;
			foreach (KeyValuePair<Formation, float> keyValuePair in targetDistances)
			{
				Formation key = keyValuePair.Key;
				float value = keyValuePair.Value;
				float cachedMovementSpeed = key.CachedMovementSpeed;
				float num5 = maxDistance - value;
				if (num5 < range)
				{
					if (num3 > cachedMovementSpeed)
					{
						num3 = cachedMovementSpeed;
						num4 = value;
					}
					float num6 = 1f - num5 / range;
					num2 += num6 * cachedMovementSpeed;
					num += num6;
				}
			}
			float num7 = num2 / num;
			return MathF.Lerp(num3, num7, (maxDistance - num4) / range, 1E-05f);
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001A830 File Offset: 0x00018A30
		private float GetMaxDistanceSpeed2(Dictionary<Formation, float> targetDistances, float distance, float minDistance, float maxDistance, float maxOriginalDuration, float distanceWithMaxDuration, float range)
		{
			float num = float.MinValue;
			foreach (KeyValuePair<Formation, float> keyValuePair in targetDistances)
			{
				Formation key = keyValuePair.Key;
				float value = keyValuePair.Value;
				float num2 = MathF.Max(0.1f, key.CachedMovementSpeed);
				if (maxDistance - value < range)
				{
					float num3 = value / num2;
					if (num < num3)
					{
						num = num3;
					}
				}
			}
			return maxDistance / num;
		}

		// Token: 0x040001D4 RID: 468
		private List<Formation> _selectedFormation;
	}
}
