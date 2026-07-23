using System;
using System.Globalization;

namespace BattlefieldUI.UI
{
	// Token: 0x02000009 RID: 9
	public static class BattlefieldUIColor
	{
		// Token: 0x0600005F RID: 95 RVA: 0x000038C8 File Offset: 0x00001AC8
		public static string Normalize(string value, string fallback)
		{
			byte b;
			byte b2;
			byte b3;
			byte b4;
			if (BattlefieldUIColor.TryParse(value, out b, out b2, out b3, out b4))
			{
				return BattlefieldUIColor.Format(b, b2, b3, b4);
			}
			if (BattlefieldUIColor.TryParse(fallback, out b, out b2, out b3, out b4))
			{
				return BattlefieldUIColor.Format(b, b2, b3, b4);
			}
			return "#FFFFFFFF";
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003910 File Offset: 0x00001B10
		public static string ApplyOpacity(string value, string fallback, int opacityPercent)
		{
			byte b;
			byte b2;
			byte b3;
			byte b4;
			BattlefieldUIColor.TryParse(BattlefieldUIColor.Normalize(value, fallback), out b, out b2, out b3, out b4);
			int num = Math.Max(0, Math.Min(100, opacityPercent));
			byte b5 = (byte)Math.Round((double)((int)b4 * num) / 100.0);
			return BattlefieldUIColor.Format(b, b2, b3, b5);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003964 File Offset: 0x00001B64
		private static bool TryParse(string value, out byte red, out byte green, out byte blue, out byte alpha)
		{
			red = (green = (blue = (alpha = 0)));
			if (string.IsNullOrWhiteSpace(value))
			{
				return false;
			}
			string text = value.Trim();
			if (text.StartsWith("#", StringComparison.Ordinal))
			{
				text = text.Substring(1);
			}
			if (text.Length == 6)
			{
				text += "FF";
			}
			return text.Length == 8 && (byte.TryParse(text.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out red) && byte.TryParse(text.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out green) && byte.TryParse(text.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out blue)) && byte.TryParse(text.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out alpha);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003A34 File Offset: 0x00001C34
		private static string Format(byte red, byte green, byte blue, byte alpha)
		{
			return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { red, green, blue, alpha });
		}
	}
}
