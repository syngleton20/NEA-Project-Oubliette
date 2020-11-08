using System.Collections.Generic;
using System.Text;

namespace NEA_Project_Oubliette.Items
{
    internal sealed class Inventory
    {
        private List<Item> items = new List<Item>();

        public int Count => items.Count;

        public void Add(string itemData)
        {
            switch (itemData[0])
            {
                case 'M':
                    items.Add(new MeleeWeapon(itemData));
                    break;

                case 'F':
                    items.Add(new Food(itemData));
                    break;
            }
        }

        ///<summary>Adds each item from an array to this inventory</summary>
        public void Add(params Item[] items)
        {
            foreach (Item item in items)
                if(!this.items.Contains(item))
                    this.items.Add(item);
        }

        ///<summary>If a given item is contained within this inventory it will be removed</summary>
        public void Remove(Item item) { if(items.Contains(item)) items.Remove(item); }

        ///<summary>Adds the items from a given inventory to this inventory</summary>
        public void Combine(Inventory inventory) => Add(inventory.items.ToArray());

        ///<summary>Removes all items in this inventory</summary>
        public void Clear() => items.Clear();

        ///<summary>Loads items from raw data, where '/' is the item delimiter</summary>
        public void Load(string data)
        {
            items.Clear();

            if(data.Contains('/'))
            {
                string[] parts = data.Split('/');

                for (int i = 0; i < parts.Length; i++)
                    Add(parts[i]);
            }
            else Add(data);
        }

        ///<summary>Provides raw data for each item contained in this inventory, so that they can be saved to a file</summary>
        public string Save()
        {
            StringBuilder itemString = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                if(i > 0) itemString.Append('/');
                itemString.Append(items[i].Save());
            }

            return itemString.ToString();
        }

        ///<summary>Returns whether or not a given item is contained within this inventory</summary>
        public bool Contains(Item item) => items.Contains(item);

        ///<summary>Returns an array of the items in this inventory</summary>
        public Item[] GetItems() => items.ToArray();
    }
}