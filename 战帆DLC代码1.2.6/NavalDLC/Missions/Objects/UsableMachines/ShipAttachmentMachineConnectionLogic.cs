using System;
using TaleWorlds.Engine;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B2 RID: 178
	public class ShipAttachmentMachineConnectionLogic : ScriptComponentBehavior
	{
		// Token: 0x06000DA4 RID: 3492 RVA: 0x0006BBCC File Offset: 0x00069DCC
		private void FillAttachmentMachinesList()
		{
			this._ownerShip = base.GameEntity.Root.GetFirstScriptOfType<MissionShip>();
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0006BBF5 File Offset: 0x00069DF5
		protected override void OnInit()
		{
			base.OnInit();
			this.FillAttachmentMachinesList();
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x0006BC03 File Offset: 0x00069E03
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x0006BC08 File Offset: 0x00069E08
		protected override void OnTick(float dt)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._ownerShip.AttachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling && shipAttachmentMachine.CurrentAttachment.ShouldLookForBetterConnections())
				{
					ShipAttachmentPointMachine attachmentTarget = shipAttachmentMachine.CurrentAttachment.AttachmentTarget;
					MissionShip ownerShip = attachmentTarget.OwnerShip;
					float num = ShipAttachmentMachine.ComputePotentialAttachmentValue(shipAttachmentMachine, attachmentTarget, false, false, true);
					if (num > 0f)
					{
						float num2 = num * 1.2f;
						ShipAttachmentPointMachine shipAttachmentPointMachine = null;
						foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in ownerShip.AttachmentPointMachines)
						{
							if (attachmentTarget != shipAttachmentPointMachine2 && shipAttachmentPointMachine2.CurrentAttachment == null)
							{
								ShipAttachmentMachine linkedAttachmentMachine = shipAttachmentPointMachine2.LinkedAttachmentMachine;
								if (((linkedAttachmentMachine != null) ? linkedAttachmentMachine.CurrentAttachment : null) == null)
								{
									float num3 = ShipAttachmentMachine.ComputePotentialAttachmentValue(shipAttachmentMachine, attachmentTarget, true, true, false);
									if (num3 > num2)
									{
										num2 = num3;
										shipAttachmentPointMachine = shipAttachmentPointMachine2;
									}
								}
							}
						}
						if (shipAttachmentPointMachine != null)
						{
							shipAttachmentMachine.CurrentAttachment.Destroy();
							shipAttachmentMachine.ConnectWithAttachmentPointMachine(shipAttachmentPointMachine, false, false, false);
						}
					}
				}
			}
		}

		// Token: 0x0400088A RID: 2186
		private MissionShip _ownerShip;
	}
}
