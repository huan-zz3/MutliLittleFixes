using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace MutliLittleFixes
{
    public class SiegeTrajectoryConfig : AttributeGlobalSettings<SiegeTrajectoryConfig>
    {
        public override string Id => "MutliLittleFixes_SiegeTrajectory_v1";

        public override string DisplayName
        {
            get
            {
                return new TextObject("{=st_mod_name}Siege Trajectory", null).ToString();
            }
        }

        public override string FolderName => "MutliLittleFixes";

        public override string FormatType => "json2";

        [SettingPropertyBool("{=st_ballista}Show ballista trajectory", Order = 1, RequireRestart = false, HintText = "{=st_ballista_hint}Enable or disable trajectory preview for ballista and scorpion.")]
        [SettingPropertyGroup("{=st_group_siege}Siege engines")]
        public bool EnableBallista { get; set; } = true;

        [SettingPropertyBool("{=st_mangonel}Show mangonel trajectory", Order = 2, RequireRestart = false, HintText = "{=st_mangonel_hint}Enable or disable trajectory preview for mangonel and trebuchet.")]
        [SettingPropertyGroup("{=st_group_siege}Siege engines")]
        public bool EnableMangonel { get; set; } = true;

        [SettingPropertyBool("{=st_coord}Enable coordinate targeting", Order = 3, RequireRestart = false, HintText = "{=st_coord_hint}Enable or disable AI siege weapons following player-designated target coordinates (period key).")]
        [SettingPropertyGroup("{=st_group_siege}Siege engines")]
        public bool CoordinateTargetingEnabled { get; set; } = true;
    }
}
