using NEA_Project_Oubliette.Items;
using System;

namespace NEA_Project_Oubliette.Entities
{
    internal sealed class Pickup : Entity
    {
        private Inventory inventory = new Inventory();

        public Inventory Inventory => inventory;

        public Pickup(string data) => Load(data);

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("██");
            Console.ResetColor();
        }

        public override void Load(string data)
        {
            string[] parts = data.Split(' ');
            id = parts[1].ToInt();
            position = parts[2].ToVector();

            string[] itemData = new string[0];
            int startIndex = 0;

            for (int i = 0; i < 3; i++)
                startIndex += parts[i].Length + 1;

            string items = data.Substring(startIndex, data.Length - startIndex);

            if(items.Contains('/')) itemData = items.Split('/');
            else itemData = new string[] { items };

            for (int i = 0; i < itemData.Length; i++)
            {
                switch (itemData[i][0])
                {
                    case 'M':
                        inventory.Add(new MeleeWeapon(itemData[i]));
                        break;

                    case 'F':
                        inventory.Add(new Food(itemData[i]));
                        break;
                }
            }
        }

        public override string Save() => $"I {id} {position.ToString()} {inventory.Save()}";
    }
}