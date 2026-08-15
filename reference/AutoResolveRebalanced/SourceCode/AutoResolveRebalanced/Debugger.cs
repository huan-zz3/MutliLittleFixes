using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Library;

namespace AutoResolveRebalanced
{
	// Token: 0x02000012 RID: 18
	public class Debugger
	{
		// Token: 0x06000097 RID: 151 RVA: 0x000035D4 File Offset: 0x000017D4
		public static void Message(string str, Debugger.Type type, MapEvent mapEvent, bool shouldLog = false)
		{
			Settings settings = new Settings();
			bool flag = false;
			Color color = Colors.White;
			string text = "";
			bool flag2 = false;
			if (mapEvent != null)
			{
				flag2 = mapEvent.IsPlayerMapEvent;
				text = string.Concat(new string[]
				{
					mapEvent.ToString(),
					"/",
					mapEvent.State.ToString(),
					"/",
					mapEvent.EventType.ToString(),
					"/",
					mapEvent.GetNumberOfInvolvedMen(1).ToString(),
					"vs",
					mapEvent.GetNumberOfInvolvedMen(0).ToString()
				});
				if (mapEvent.GetNumberOfInvolvedMen(1) == 1 || mapEvent.GetNumberOfInvolvedMen(0) == 1)
				{
					flag = true;
				}
			}
			switch (type)
			{
			case Debugger.Type.Log:
				color = Colors.Gray;
				if (settings.showLog && (shouldLog || flag2 || flag))
				{
					InformationManager.DisplayMessage(new InformationMessage("AAR: " + str, color));
				}
				str = "AutoResolveRebalanced Log: " + str;
				if (flag || shouldLog)
				{
					Debug.Print(str, 0, 12, 17592186044416UL);
					Debug.Print(text, 0, 12, 17592186044416UL);
				}
				return;
			case Debugger.Type.Error:
				color = Colors.Red;
				if (settings.showError)
				{
					InformationManager.DisplayMessage(new InformationMessage("AAR: " + str, color));
					InformationManager.DisplayMessage(new InformationMessage(text, color));
				}
				str = "AutoResolveRebalanced Error: " + str;
				Debug.Print(str, 0, 12, 17592186044416UL);
				Debug.Print(text, 0, 12, 17592186044416UL);
				return;
			case Debugger.Type.Warn:
				color = Colors.Yellow;
				if (settings.showWarn)
				{
					InformationManager.DisplayMessage(new InformationMessage("AAR: " + str, color));
					InformationManager.DisplayMessage(new InformationMessage(text, color));
				}
				str = "AutoResolveRebalanced Warn: " + str;
				Debug.Print(str, 0, 12, 17592186044416UL);
				Debug.Print(text, 0, 12, 17592186044416UL);
				return;
			case Debugger.Type.Exception:
				color = Colors.Red;
				if (!Debugger.exceptionThrown)
				{
					InformationManager.DisplayMessage(new InformationMessage("AutoResolveRebalanced: Exception thrown, please provide log file to author.", color));
					Debugger.exceptionThrown = true;
				}
				str = "AutoResolveRebalanced Exception: " + str;
				Debug.Print(str, 0, 12, 17592186044416UL);
				return;
			default:
				return;
			}
		}

		// Token: 0x04000035 RID: 53
		public static bool exceptionThrown;

		// Token: 0x02000013 RID: 19
		public enum Type
		{
			// Token: 0x04000037 RID: 55
			Log,
			// Token: 0x04000038 RID: 56
			Error,
			// Token: 0x04000039 RID: 57
			Warn,
			// Token: 0x0400003A RID: 58
			Exception
		}
	}
}
