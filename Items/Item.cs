using System;

namespace NEA_Project_Oubliette.Items
{
    internal abstract class Item
    {
        protected string name;
        protected float weight;

        public string Name => name;
        public float Weight => weight;

        public Item() { }

        ///<summary>Performs actions when the item is eqipped from the player's inventory</summary>
        public abstract void OnEquip();

        ///<summary>Performs actions related to the specific type of item in use</summary>
        public abstract void Use();

        ///<summary>Provides data which can be saved to a file</summary>
        public abstract string Save();
    }
}