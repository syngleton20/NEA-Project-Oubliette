using NEA_Project_Oubliette.Entities;

namespace NEA_Project_Oubliette.Items
{
    internal sealed class Food : Item
    {
        private int effectMultiplier;

        public Food(string data)
        {
            string[] parts = data.Split(' ');

            name = parts[1];
            weight = parts[2].ToFloat();
            effectMultiplier = parts[3].ToInt();
        }

        public override void OnEquip() { }

        public override void Use()
        {
            if(Player.Instance.Health < Player.Instance.MaxHealth)
            {
                Player.Instance.Heal(effectMultiplier);
                Player.Instance.Inventory.Remove(this);
            }
        }

        public override string Save() => $"F {name} {weight} {effectMultiplier}";
    }
}