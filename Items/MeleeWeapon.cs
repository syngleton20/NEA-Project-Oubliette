namespace NEA_Project_Oubliette.Items
{
    internal sealed class MeleeWeapon : Item
    {
        private int damage;

        public int Damage => damage;

        public MeleeWeapon(string data)
        {
            string[] parts = data.Split('_');

            name = parts[1];
            weight = parts[2].ToFloat();
            damage = parts[3].ToInt();
        }

        public override void OnEquip() { }
        public override void Use() { }

        public override string Save() => $"M_{name}_{weight}_{damage}";
    }
}