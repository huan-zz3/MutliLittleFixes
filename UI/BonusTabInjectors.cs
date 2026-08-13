using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using System.Xml;

namespace MutliLittleFixes.UI
{
    /// <summary>
    /// 在 KingdomManagement 的 ArmiesTabButton 之后注入"国家加成"标签按钮。
    /// 对应教程 §4.3 标签按钮注入。
    /// </summary>
    [PrefabExtension("KingdomManagement",
        "descendant::ButtonWidget[@Id='ArmiesTabButton']")]
    internal sealed class BonusTabButtonPatch : PrefabExtensionInsertPatch
    {
        public override InsertType Type => (InsertType)4; // After

        private readonly XmlDocument _document;

        [PrefabExtensionXmlNode]
        public XmlNode GetPrefabExtension() => _document;

        public BonusTabButtonPatch()
        {
            _document = new XmlDocument();
            _document.LoadXml(
                "<ButtonWidget Id='BonusTabButton' " +
                "  IsSelected='@IsBonusTabSelected' " +
                "  DoNotPassEventsToChildren='true' " +
                "  WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' " +
                "  SuggestedWidth='!Header.Tab.Center.Width.Scaled' " +
                "  SuggestedHeight='!Header.Tab.Center.Height.Scaled' " +
                "  VerticalAlignment='Center' PositionYOffset='2' " +
                "  Brush='Header.Tab.Center' " +
                "  Command.Click='ExecuteShowBonus' " +
                "  UpdateChildrenStates='true'>" +
                "  <Children>" +
                "    <TextWidget DataSource='{..}' " +
                "      WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' " +
                "      HorizontalAlignment='Center' VerticalAlignment='Center' " +
                "      Brush='Clan.TabControl.Text' " +
                "      Text='@BonusTabText' />" +
                "  </Children>" +
                "</ButtonWidget>");
        }
    }

    /// <summary>
    /// 在 KingdomManagement 的 DiplomacyPanel 之后注入"国家加成"内容面板。
    /// 对应教程 §4.4 面板注入。
    /// </summary>
    [PrefabExtension("KingdomManagement",
        "descendant::DiplomacyPanel[@Id='DiplomacyPanel']")]
    internal sealed class BonusTabPanelPatch : PrefabExtensionInsertPatch
    {
        public override InsertType Type => (InsertType)4; // After

        private readonly XmlDocument _document;

        [PrefabExtensionXmlNode]
        public XmlNode GetPrefabExtension() => _document;

        public BonusTabPanelPatch()
        {
            _document = new XmlDocument();
            _document.LoadXml(
                "<Widget Id='BonusTabPanel' IsVisible='@IsBonusTabSelected' " +
                "  WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' " +
                "  MarginTop='188' MarginBottom='75'>" +
                "  <Children>" +
                "    <ListPanel DataSource='{Bonus}' " +
                "      WidthSizePolicy='StretchToParent' HeightSizePolicy='CoverChildren' " +
                "      StackLayout.LayoutMethod='VerticalTopToBottom' " +
                "      MarginTop='40'>" +
                "      <Children>" +
                "        <TextWidget Text='@OverviewTitleText' " +
                "          WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' " +
                "          HorizontalAlignment='Center' " +
                "          Brush='Kingdom.Paragraph.Text' MarginBottom='25' />" +
        "        <ListPanel WidthSizePolicy='StretchToParent' " +
        "          HeightSizePolicy='Fixed' SuggestedHeight='35' " +
        "          MarginLeft='50' MarginRight='50' MarginBottom='5' " +
        "          StackLayout.LayoutMethod='HorizontalLeftToRight'>" +
        "          <Children>" +
        "            <TextWidget WidthSizePolicy='StretchToParent' " +
        "              HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "              HorizontalAlignment='Center' " +
        "              Brush='Clan.TabControl.Text' Text='@ColumnKingdomText' />" +
        "            <TextWidget WidthSizePolicy='StretchToParent' " +
        "              HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "              HorizontalAlignment='Center' " +
        "              Brush='Clan.TabControl.Text' Text='@ColumnTerritoryBonusText' />" +
        "            <TextWidget WidthSizePolicy='StretchToParent' " +
        "              HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "              HorizontalAlignment='Center' " +
        "              Brush='Clan.TabControl.Text' Text='@ColumnWaitingCountText' />" +
        "            <TextWidget WidthSizePolicy='StretchToParent' " +
        "              HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "              HorizontalAlignment='Center' " +
        "              Brush='Clan.TabControl.Text' Text='@ColumnActiveCountText' />" +
        "          </Children>" +
        "        </ListPanel>" +
        "        <NavigatableListPanel DataSource='{KingdomList}' " +
        "          WidthSizePolicy='StretchToParent' HeightSizePolicy='CoverChildren' " +
        "          MarginLeft='50' MarginRight='50' " +
        "          StackLayout.LayoutMethod='VerticalTopToBottom'>" +
        "          <ItemTemplate>" +
        "            <ListPanel WidthSizePolicy='StretchToParent' " +
        "              HeightSizePolicy='Fixed' SuggestedHeight='35' " +
        "              StackLayout.LayoutMethod='HorizontalLeftToRight'>" +
        "              <Children>" +
        "                <TextWidget WidthSizePolicy='StretchToParent' " +
        "                  HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "                  HorizontalAlignment='Center' " +
        "                  Brush='Kingdom.ParagraphSmall.Text' Text='@KingdomName' />" +
        "                <TextWidget WidthSizePolicy='StretchToParent' " +
        "                  HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "                  HorizontalAlignment='Center' " +
        "                  Brush='Kingdom.ParagraphSmall.Text' Text='@TerritoryBonusText' />" +
        "                <TextWidget WidthSizePolicy='StretchToParent' " +
        "                  HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "                  HorizontalAlignment='Center' " +
        "                  Brush='Kingdom.ParagraphSmall.Text' Text='@WaitingCountText' />" +
        "                <TextWidget WidthSizePolicy='StretchToParent' " +
        "                  HeightSizePolicy='CoverChildren' VerticalAlignment='Center' " +
        "                  HorizontalAlignment='Center' " +
        "                  Brush='Kingdom.ParagraphSmall.Text' Text='@ActiveCountText' />" +
        "              </Children>" +
        "            </ListPanel>" +
        "          </ItemTemplate>" +
        "        </NavigatableListPanel>" +
                "      </Children>" +
                "    </ListPanel>" +
                "  </Children>" +
                "</Widget>");
        }
    }
}
