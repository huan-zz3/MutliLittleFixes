using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace ExampleMod.Behaviors
{
    /// <summary>
    /// SaveableTypeDefiner，基础 ID 为 555000。
    /// 注册 PendingRestoration 和 KingdomTerritoryData 以支持存档持久化，
    /// 以及 LordStrengthBehavior 使用的 Dictionary 容器。
    ///
    /// 选择 555000 的原因：该值在游戏原版范围（Recruitment=881200, Crafting=150000）之外，
    /// 为 ExampleMod 预留。
    /// </summary>
    public class LordStrengthTypeDefiner : SaveableTypeDefiner
    {
        public LordStrengthTypeDefiner() : base(555000) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(PendingRestoration), 1);
            AddClassDefinition(typeof(KingdomTerritoryData), 2);
            AddClassDefinition(typeof(TerritoryEvent), 3);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<Hero, PendingRestoration>));
            ConstructContainerDefinition(typeof(Dictionary<Kingdom, KingdomTerritoryData>));
            ConstructContainerDefinition(typeof(List<TerritoryEvent>));
        }
    }
}
