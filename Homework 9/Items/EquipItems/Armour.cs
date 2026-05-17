using GamePrototype.Utils;

namespace GamePrototype.Items.EquipItems
{
    public sealed class Armour : EquipItem
    {
        public Armour(uint defence, uint durability, string name) : base(durability, name) => Defence = defence;

        // Добавил приватный сет, чтобы можно было изменить в рамках класса
        public uint Defence { get; private set; }

        //Добавил метод поломки брони
        public void BreakArmor()
        {
            if (Defence > 0)
            {
                Defence -= 1;
            }
        }

        public override EquipSlot Slot => EquipSlot.Armour;
    }
}
