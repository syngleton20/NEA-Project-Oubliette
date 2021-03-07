using NEA_Project_Oubliette.Items;
using NEA_Project_Oubliette.Maps;
using System.Threading;
using System;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Entity representing the player character</summary>
    internal sealed class Player : Entity, IDamageable
    {
        private int strength = 2, score = 0;
        private bool takingDamage;

        private Item equippedItem;
        private Inventory inventory = new Inventory();

        public int Strength => strength;
        public int Score => score;

        public int Health { get; private set; } = 20;
        public int MaxHealth { get; private set; } = 20;
        public bool IsDead { get; private set; }

        public Item EquippedItem
        {
            get
            {
                if(inventory.Contains(equippedItem)) return equippedItem;

                equippedItem = null;
                return null;
            }
            private set
            {
                equippedItem = value;

                if(equippedItem != null && equippedItem.GetType() == typeof(MeleeWeapon)) strength = (equippedItem as MeleeWeapon).Damage;
                else strength = 2;
            }
        }

        public Inventory Inventory => inventory;

        public static Player Instance { get; private set; }

        public Player(int startX, int startY) : base()
        {
            Instance = this;
            position = new Vector(startX, startY);

            Game.Current.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE); // Sets the camera position to the quadrant of the map this player is in, so that the correct quadrant is drawn
        }

        public Player(string data)
        {
            Instance = this;
            Load(data);

            Game.Current?.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE); // Sets the camera position to the quadrant of the map this player is in, so that the correct quadrant is drawn
        }

        public override void Draw()
        {
            if(!takingDamage) Console.ForegroundColor = ConsoleColor.White;
            base.Draw();
        }

        public override void Move(int deltaX, int deltaY)
        {
            if(Game.Current.CurrentMap.TryGetTile(position.X + deltaX, position.Y + deltaY, out Tile tile))
            {
                if(tile.IsWalkable)
                {
                    if(tile.IsOccupied)
                    {
                        if(tile.Occupant.GetType() == typeof(Enemy)) (tile.Occupant as Enemy).TakeDamage(strength); // Deals melee damage to an enemy within a one tile radius
                        else if(tile.Occupant.GetType() == typeof(Pickup))
                        {
                            inventory.Combine((tile.Occupant as Pickup).Inventory); // Adds all the items from the container into this player's inventory
                            Game.Current.CurrentMap.Collection.Remove(tile.Occupant); // Removes the container from the map
                        }
                    }
                    else
                    {
                        if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y, out Tile oldTile)) oldTile.Vacate();

                        position += new Vector(deltaX, deltaY);
                        tile.Occupy(this);
                    }
                }

                if(tile.IsOccupied) tile.Occupant.Push(deltaX, deltaY, this);
            }

            Game.Current.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE); // Sets the camera position to the quadrant of the map this player is in, so that the correct quadrant is drawn
        }

        public void Heal(int amount = 1)
        {
            Health += amount;
            if(Health > MaxHealth) Health = MaxHealth;
        }

        public void TakeDamage(int amount = 1)
        {
            Health -= amount;
            if(Health <= 0 && !IsDead) Die();
            else
            {
                // Causes this player to flash red when they take damage
                takingDamage = true;

                Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
                Console.ForegroundColor = ConsoleColor.Red;
                Draw();

                Thread.Sleep(50);
                takingDamage = false;

                // Draws the player white again
                Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
                Console.ForegroundColor = ConsoleColor.White;
                Draw();
            }
        }

        public void Die() => IsDead = true;

        ///<summary>Adds an amount to this player's current score</summary>
        public void IncrementScore(int amount = 1) => score += amount;

        ///<summary>Equips an item at an index in this player's inventory</summary>
        public void EquipItemAt(int itemIndex) => EquippedItem = inventory.GetItems()[itemIndex];

        public override void Load(string data)
        {
            string[] parts = data.Split(' ');
            id = parts[1].ToInt();
            position = parts[2].ToVector();
            Health = int.Parse(parts[3]);

            if(parts.Length >= 6)
            {
                int startIndex = 0;
                for(int i = 0; i < 5; i++) startIndex += parts[i].Length + 1;

                string inventoryString = data.Substring(startIndex, data.Length - startIndex);

                if(inventoryString != "" && parts.Length > 4)
                {
                    inventory.Load(inventoryString);
                    int equippedItemIndex = parts[4].ToInt();

                    if(equippedItemIndex >= 0) EquippedItem = inventory.GetItems()[equippedItemIndex];
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null; // Resets the Instance property so that this player cannot be referenced after it has been destroyed
        }

        public override string Save() => $"P {id} {position.ToString()} {Health} {(inventory.Count > 0 && EquippedItem != null ? Array.IndexOf(inventory.GetItems(), EquippedItem) : -1)} {inventory.Save()}";
    }
}