using System;
using System.Xml;
using TaleWorlds.Library;

namespace NavalDLC
{
	// Token: 0x02000025 RID: 37
	public class NavalVersion
	{
		// Token: 0x06000194 RID: 404 RVA: 0x0000A2F8 File Offset: 0x000084F8
		public static string GetApplicationVersionBuildNumber()
		{
			string text = "__MODULE_NAME__NavalDLC__MODULE_NAME__/Parameters/Version.xml";
			XmlDocument xmlDocument = new XmlDocument();
			string fileContent = VirtualFolders.GetFileContent(text, typeof(VirtualFolders));
			if (fileContent == "")
			{
				return "";
			}
			xmlDocument.LoadXml(fileContent);
			return xmlDocument.ChildNodes[0].ChildNodes[0].Attributes["Value"].InnerText;
		}
	}
}
