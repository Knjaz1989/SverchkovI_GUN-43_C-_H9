namespace GamePrototype.Items.EconomicItems
{
    public sealed class Grindstone : EconomicItem
    {
        // Добавил свойство с дефолтным значением
        public uint RepairValue { get => 4; }
        public override bool Stackable => false;

        public Grindstone(string name) : base(name)
        {
        }    
    }
}
