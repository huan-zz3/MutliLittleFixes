using TaleWorlds.SaveSystem;

namespace MutliLittleFixes.Behaviors
{
    /// <summary>
    /// SaveableTypeDefiner,基础 ID 556000(与 LordStrengthTypeDefiner 的 555000 区分)。
    /// 注册 FoodTransportPartyComponent 以支持运输队状态存档持久化。
    /// 存档系统自动发现本类(无需显式注册)。
    /// </summary>
    public class FoodTransportTypeDefiner : SaveableTypeDefiner
    {
        public FoodTransportTypeDefiner() : base(556000)
        {
        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(FoodTransportPartyComponent), 1);
        }

        protected override void DefineEnumTypes()
        {
            // 注意: 枚举 ID 必须与类定义 ID 不同(共用 _allTypeDefinitionsWithId 字典, 重复键会启动崩溃)。
            // 类 FoodTransportPartyComponent 已占用 556001, 枚举用 556002。
            AddEnumDefinition(typeof(FoodTransportPartyComponent.TransportPhase), 2);
        }
    }
}
