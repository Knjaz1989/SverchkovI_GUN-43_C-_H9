using GamePrototype.Items.EconomicItems;
using GamePrototype.Items.EquipItems;
using GamePrototype.Utils;
using System.Text;

namespace GamePrototype.Units
{
    public sealed class Player : Unit
    {
        private Dictionary<EquipSlot, EquipItem> _equipment = new();

        public Player(string name, uint health, uint maxHealth, uint baseDamage) : base(name, health, maxHealth, baseDamage)
        {            
        }

        public override uint GetUnitDamage()
        {
            if (_equipment.TryGetValue(EquipSlot.Weapon, out var item1) && item1 is Weapon weapon && weapon.Damage > 0) 
            {
                return BaseDamage + weapon.Damage;
            }
            else if (_equipment.TryGetValue(EquipSlot.RangeWeapon, out var item2) && item2 is RangeWeapon rangeWeapon)
            {
                return BaseDamage + rangeWeapon.Damage;
            }
            return BaseDamage;
        }

        public override void HandleCombatComplete()
        {
            var items = Inventory.Items;
            // Итерируемся с конца, чтобы смещение индексов при удалении элементов не влияло на количество итераций.
            for (int i = items.Count - 1; i >= 0; i--) 
            {
                if (items[i] is EconomicItem economicItem) 
                {
                    UseEconomicItem(economicItem);
                    Inventory.TryRemove(items[i]);
                }
            }
        }

        public override void AddItemToInventory(Item item)
        {
            if (item is EquipItem equipItem && _equipment.TryAdd(equipItem.Slot, equipItem)) 
            {
                // Item was equipped
                return;
            }
            base.AddItemToInventory(item);
        }

        private void UseEconomicItem(EconomicItem economicItem)
        {
            if (economicItem is HealthPotion healthPotion) 
            {
                Health += healthPotion.HealthRestore;
            }
            // Если нам попадается камень, то мы его используем для починки оружия
            else if (economicItem is Grindstone grindstone)
            {
                if (_equipment.TryGetValue(EquipSlot.Weapon, out var item) && item is Weapon weapon)
                {
                    weapon.Repair(grindstone.RepairValue);
                    Console.WriteLine("Your weapon is fixed");
                }
            }
        }

        protected override uint CalculateAppliedDamage(uint damage)
        {
            if (_equipment.TryGetValue(EquipSlot.Armour, out var item) && item is Armour armour) 
            {
                damage -= (uint)(damage * (armour.Defence / 100f));
            }
            return damage;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(Name);
            builder.AppendLine($"Health {Health}/{MaxHealth}");
            builder.AppendLine("Loot:");
            var items = Inventory.Items;
            for (int i = 0; i < items.Count; i++) 
            {
                builder.AppendLine($"[{items[i].Name}] : {items[i].Amount}");
            }
            return builder.ToString();
        }

        //Переопределил существующий метод, чтобы не писать новое. В нем мы ломаем броню. Этот метод срабатывает после успешного получения дамага пользователя
        protected override void DamageReceiveHandler()
        {
            if (_equipment.TryGetValue(EquipSlot.Armour, out var item) && item is Armour armour)
            {
                armour.BreakArmor();
                Console.WriteLine($"Defence of your Armor is {armour.Defence}");
            }
        }

        protected override bool TryEquipItem(Item item)
        {
            if (item is EquipItem equipItem)
            {
                EquipSlot slot = equipItem.Slot;

                if (_equipment.ContainsKey(slot))
                {
                    Console.WriteLine($"Слот {slot} занят. Заменить? (Yes/No)");
                    if (Console.ReadLine() == "Yes")
                    {
                        Inventory.TryAdd(_equipment[slot]); // старый в инвентарь
                        _equipment[slot] = equipItem;
                        Console.WriteLine($"{equipItem.Name} экипирован.");
                        return true; // уже экипировали, не надо в инвентарь
                    }
                    return false; // в инвентарь
                }
                else
                {
                    _equipment[slot] = equipItem;
                    Console.WriteLine($"{equipItem.Name} экипирован.");
                    return true; // уже экипировали, не надо в инвентарь
                }
            }
            return false; // не экипировка — в инвентарь
        }
    }
}
