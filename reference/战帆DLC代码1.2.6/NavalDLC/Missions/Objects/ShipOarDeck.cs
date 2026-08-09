using System;
using System.Collections.Generic;
using NavalDLC.Missions.ShipActuators;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A7 RID: 167
	public class ShipOarDeck : ScriptComponentBehavior
	{
		// Token: 0x06000CEF RID: 3311 RVA: 0x0006414C File Offset: 0x0006234C
		public OarDeckParameters GetParameters()
		{
			if (this._oarDeckParameters == null)
			{
				this._oarDeckParameters = new OarDeckParameters(this._verticalBaseAngle * 0.017453292f, this._lateralBaseAngle * 0.017453292f, this._verticalRotationAngle * 0.017453292f, this._lateralRotationAngle * 0.017453292f, this._oarLength, 0.4f, 1f);
			}
			else
			{
				this._oarDeckParameters.SetParameters(this._verticalBaseAngle * 0.017453292f, this._lateralBaseAngle * 0.017453292f, this._verticalRotationAngle * 0.017453292f, this._lateralRotationAngle * 0.017453292f, this._oarLength, 0.4f, 1f);
			}
			return this._oarDeckParameters;
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x00064200 File Offset: 0x00062400
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this.UpdateOarLength();
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.CollectChildrenEntitiesWithTag("seat_mesh_entity"))
			{
				WeakGameEntity firstChildEntityWithName = MBExtensions.GetFirstChildEntityWithName(weakGameEntity, "floor");
				if (firstChildEntityWithName != null)
				{
					firstChildEntityWithName.Remove(78);
				}
			}
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x00064290 File Offset: 0x00062490
		internal void UpdateOarLength()
		{
			List<WeakGameEntity> list = base.GameEntity.CollectChildrenEntitiesWithTag("oar_gate_left");
			list.AddRange(base.GameEntity.CollectChildrenEntitiesWithTag("oar_gate_right"));
			if (list.Count > 0)
			{
				float num = -1f;
				foreach (WeakGameEntity weakGameEntity in list)
				{
					Mesh mesh = weakGameEntity.GetFirstMesh();
					WeakGameEntity weakGameEntity2 = weakGameEntity;
					if (mesh == null)
					{
						WeakGameEntity firstChildEntityWithTag = weakGameEntity.GetFirstChildEntityWithTag("upgrade_slot");
						if (firstChildEntityWithTag.ChildCount > 0)
						{
							WeakGameEntity weakGameEntity3 = firstChildEntityWithTag.GetFirstChildEntityWithTag("base");
							if (!weakGameEntity3.IsValid)
							{
								weakGameEntity3 = firstChildEntityWithTag.GetChild(0);
							}
							mesh = weakGameEntity3.GetFirstMesh();
							weakGameEntity2 = weakGameEntity3;
						}
					}
					if (mesh != null)
					{
						float num2 = float.MinValue;
						if (weakGameEntity2.MultiMeshComponentCount == 0)
						{
							Vec3 boundingBoxMax = mesh.GetBoundingBoxMax();
							num2 = MathF.Max(boundingBoxMax.x, boundingBoxMax.y, boundingBoxMax.z);
						}
						else
						{
							for (int i = 0; i < weakGameEntity2.MultiMeshComponentCount; i++)
							{
								MetaMesh metaMesh = weakGameEntity2.GetMetaMesh(i);
								for (int j = 0; j < metaMesh.MeshCount; j++)
								{
									Vec3 boundingBoxMax2 = metaMesh.GetMeshAtIndex(j).GetBoundingBoxMax();
									num2 = MathF.Max(MathF.Max(boundingBoxMax2.x, boundingBoxMax2.y, boundingBoxMax2.z), num2);
								}
							}
						}
						if (num >= 0f)
						{
							MBMath.ApproximatelyEquals(num2, num, 1E-05f);
							num = MathF.Max(num, num2);
						}
						else
						{
							num = num2;
						}
					}
				}
				this._oarLength = num;
				return;
			}
			this._oarLength = 0f;
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0006446C File Offset: 0x0006266C
		public static WeakGameEntity GetOarEntity(WeakGameEntity oarScriptEntity)
		{
			WeakGameEntity weakGameEntity = oarScriptEntity.GetFirstChildEntityWithTag("oar_entity");
			if (!weakGameEntity.IsValid)
			{
				foreach (WeakGameEntity weakGameEntity2 in oarScriptEntity.GetChildren())
				{
					if (weakGameEntity2.Name == "oar")
					{
						weakGameEntity = weakGameEntity2;
					}
				}
			}
			return weakGameEntity;
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x000644E0 File Offset: 0x000626E0
		public static void LoadOarScriptEntity(WeakGameEntity oarScriptEntity, out WeakGameEntity oarEntity, ref MatrixFrame oarExtractedEntitialFrame, ref MatrixFrame oarRetractedEntitialFrame, out WeakGameEntity handTargetEntity)
		{
			handTargetEntity = WeakGameEntity.Invalid;
			oarEntity = ShipOarDeck.GetOarEntity(oarScriptEntity);
			WeakGameEntity weakGameEntity = oarScriptEntity.GetFirstChildEntityWithTag("retracted_entity");
			if (oarEntity.IsValid)
			{
				oarExtractedEntitialFrame = oarEntity.GetFrame();
				handTargetEntity = oarEntity.GetFirstChildEntityWithTag("hand_target_entity");
				if (weakGameEntity.IsValid)
				{
					oarRetractedEntitialFrame = weakGameEntity.GetFrame();
				}
				if (!handTargetEntity.IsValid)
				{
					foreach (WeakGameEntity weakGameEntity2 in oarEntity.GetChildren())
					{
						if (weakGameEntity2.Name == "hand_position")
						{
							handTargetEntity = weakGameEntity2;
						}
					}
				}
				if (!weakGameEntity.IsValid)
				{
					foreach (WeakGameEntity weakGameEntity3 in oarEntity.GetChildren())
					{
						if (weakGameEntity3.Name == "retracted_frame")
						{
							oarRetractedEntitialFrame = weakGameEntity3.GetFrame();
							weakGameEntity = weakGameEntity3;
						}
					}
				}
				if (weakGameEntity != null)
				{
					weakGameEntity.Remove(66);
				}
			}
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x00064620 File Offset: 0x00062820
		private static WeakGameEntity GetRetractedFrameEntity(WeakGameEntity oarMachine)
		{
			WeakGameEntity weakGameEntity = oarMachine.GetFirstChildEntityWithTag("retracted_entity");
			if (weakGameEntity.IsValid)
			{
				return weakGameEntity;
			}
			WeakGameEntity oarEntity = ShipOarDeck.GetOarEntity(oarMachine);
			if (oarEntity.IsValid && !weakGameEntity.IsValid)
			{
				foreach (WeakGameEntity weakGameEntity2 in oarEntity.GetChildren())
				{
					if (weakGameEntity2.Name == "retracted_frame")
					{
						weakGameEntity = weakGameEntity2;
					}
				}
			}
			return weakGameEntity;
		}

		// Token: 0x040007D0 RID: 2000
		public const string OarEntityName = "oar";

		// Token: 0x040007D1 RID: 2001
		public const string OarRetractedFrameEntityName = "retracted_frame";

		// Token: 0x040007D2 RID: 2002
		public const string RightOarMachinesHolderName = "right_oar_machines";

		// Token: 0x040007D3 RID: 2003
		public const string LeftOarMachinesHolderName = "left_oar_machines";

		// Token: 0x040007D4 RID: 2004
		public const string LeftOarGateTag = "oar_gate_left";

		// Token: 0x040007D5 RID: 2005
		public const string RightOarGateTag = "oar_gate_right";

		// Token: 0x040007D6 RID: 2006
		public const string HandTargetEntityName = "hand_position";

		// Token: 0x040007D7 RID: 2007
		public const string OarEntityTag = "oar_entity";

		// Token: 0x040007D8 RID: 2008
		public const string RetractedEntityTag = "retracted_entity";

		// Token: 0x040007D9 RID: 2009
		public const string HandTargetEntityTag = "hand_target_entity";

		// Token: 0x040007DA RID: 2010
		public const string SeatLocationEntity = "seat_location_entity";

		// Token: 0x040007DB RID: 2011
		public const string ShipBodyPhysicsEntityTag = "body_mesh";

		// Token: 0x040007DC RID: 2012
		public const string SeatMeshTag = "seat_mesh_entity";

		// Token: 0x040007DD RID: 2013
		[EditableScriptComponentVariable(true, "")]
		private float _verticalBaseAngle = 15f;

		// Token: 0x040007DE RID: 2014
		[EditableScriptComponentVariable(true, "")]
		private float _lateralBaseAngle;

		// Token: 0x040007DF RID: 2015
		[EditableScriptComponentVariable(true, "")]
		private float _verticalRotationAngle = 10f;

		// Token: 0x040007E0 RID: 2016
		[EditableScriptComponentVariable(true, "")]
		private float _lateralRotationAngle = 17.2f;

		// Token: 0x040007E1 RID: 2017
		private float _oarLength;

		// Token: 0x040007E2 RID: 2018
		private OarDeckParameters _oarDeckParameters;
	}
}
