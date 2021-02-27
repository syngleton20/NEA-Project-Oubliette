using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Items;
using NEA_Project_Oubliette.Maps;
using System.Text;
using System;

namespace NEA_Project_Oubliette.Editing
{
    public enum PlacementType { Tile, Entity }

    ///<summary>Allows tiles and entities to placed in a map within the map editor</summary>
    internal static class Placement
    {
        private static char tile = '#', entity = 'P';
        private static bool useStamp;

        private static Vector position;
        private static ConsoleColor colour = ConsoleColor.Green;
        private static PlacementType type;

        public static char Tile
        {
            get => tile;
            set => tile = value;
        }

        public static char Entity
        {
            get => entity;
            set => entity = value;
        }

        public static bool UseStamp => useStamp;
        public static Vector Position => position;

        public static PlacementType Type
        {
            get => type;
            set
            {
                type = value;

                if(type == PlacementType.Tile) colour = ConsoleColor.Green;
                else colour = ConsoleColor.Yellow;
            }
        }

        ///<summary>Assigns all fields to their original values</summary>
        public static void Reset()
        {
            tile = '#';
            entity = 'P';
            useStamp = false;

            position = Player.Instance != null ? Player.Instance.Position : Vector.Zero;
            Type = PlacementType.Tile;
        }

        ///<summary>Moves this placement by an amount on the X and Y axes</summary>
        public static void Move(int deltaX, int deltaY)
        {
            // Validates movement on the Y axis to avoid this placement point escaping the bounds of the map
            if(position.Y + deltaY <= 0) position = new Vector(position.X, 0);
            else if((position.Y + deltaY) >= (Game.Current.CurrentMap.Height - 1)) position = new Vector(position.X, Game.Current.CurrentMap.Height - 1);
            else position += new Vector(0, deltaY);

            // Validates movement on the X axis to avoid this placement point escaping the bounds of the map
            if(position.X + deltaX <= 0) position = new Vector(0, position.Y);
            else if((position.X + deltaX) >= (Game.Current.CurrentMap.Width - 1)) position = new Vector(Game.Current.CurrentMap.Width - 1, position.Y);
            else position += new Vector(deltaX, 0);

            if(useStamp && type == PlacementType.Tile) Place();

            Game.Current.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE); // Sets the camera position to the quadrant this placement's position is in, so that the correct quadrant of the map is drawn
        }

        ///<summary>Draws a visual representation of this placement's position</summary>
        public static void Draw()
        {
            Console.BackgroundColor = colour;

            Console.SetCursorPosition(Display.Offset.X + (position.X * 2) % (Map.AREA_SIZE * 2), Display.Offset.Y + (position.Y % Map.AREA_SIZE));
            Console.Write(Game.Current.CurrentMap.TryGetTile(position, out Tile tile) ? tile.ToString() : "  ");
            Console.ResetColor();
        }

        ///<summary>Places either a tile or an entity at this placement's current position</summary>
        public static void Place()
        {
            if(type == PlacementType.Tile)
            {
                if(Game.Current.CurrentMap.TryGetTile(position, out Tile output))
                {
                    // Automatically places a wall tile below a void tile to give the illusion of a topdown perspective and to speed up the author's workflow
                    if(tile == '#')
                        if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y + 1, out Tile tileBelow))
                            if(tileBelow.Ascii != '#') Game.Current.CurrentMap.SetTile(position + Vector.Up, new Tile('^'));

                    // Cancels this method if the author attempts to place a tile which is already placed at this position
                    if(output.Ascii == tile) return;

                    Game.Current.CurrentMap.SetTile(position, new Tile(tile));
                    MapEditor.MakeChange();
                }
            }
            else
            {
                Entity newEntity = null;

                switch(entity)
                {
                    case 'P':
                        Entity[] entities = Game.Current.CurrentMap.Collection.Array;

                        // Checks each entity in the map and removes any duplicate players when placing a new player entity, so that there is only one instance of the player at all times
                        for(int i = 0; i < entities.Length; i++)
                        {
                            if(entities[i].GetType() == typeof(Player))
                            {
                                Game.Current.CurrentMap.Collection.Remove(entities[i]);
                                break;
                            }
                        }

                        newEntity = new Player($"P {Game.Current.CurrentMap.Collection.AssignId()} {position.ToString()} 20");
                        break;

                    case 'E':
                        newEntity = new Enemy($"E {Game.Current.CurrentMap.Collection.AssignId()} {position.ToString()} 20");
                        break;

                    case 'I':
                        StringBuilder data = new StringBuilder();

                        int typeIndex = -1;
                        bool addAnotherItem = false, hasCancelled = false;

                        do
                        {
                            Display.Clear();
                            GUI.Title("Add New Item");

                            typeIndex = GUI.VerticalMenu("Back", "Melee Weapon", "Food");

                            if(typeIndex == 0)
                            {
                                hasCancelled = true;
                                break;
                            }

                            data.Append(Item.ITEM_TYPES[typeIndex - 1].ToString());
                            GUI.Title("Add New Item");
                            data.Append('_' + GUI.TextField("Name: ", 20));
                            GUI.Title("Add New Item");
                            data.Append('_' + GUI.TextField("Weight (kg): ", 4));

                            switch(typeIndex)
                            {
                                case 1:
                                    GUI.Title("Add New Item");
                                    data.Append('_' + GUI.TextField("Damage: ", 4));
                                    break;

                                case 2:
                                    GUI.Title("Add New Item");
                                    data.Append('_' + GUI.TextField("Effect Multiplier: ", 4));
                                    break;
                            }

                            Display.Clear();
                            GUI.Title("Add Another Item");

                            addAnotherItem = GUI.YesOrNo("Do you want to add another item?");
                            if(addAnotherItem) data.Append('/'); // Adds a '/' delimiter so that another item can be added to the inventory of the container
                        }
                        while(addAnotherItem); // Stops once the author chooses to not add another item to the inventory of the container

                        if(hasCancelled) break; // Cancels this case if the author has cancelled the creation of the container
                        newEntity = new Pickup($"I {Game.Current.CurrentMap.Collection.AssignId()} {position.ToString()} {data.ToString()}");
                        break;

                    case 'X':
                        newEntity = new Exit($"X {Game.Current.CurrentMap.Collection.AssignId()} {position.ToString()}");
                        break;
                }

                if(!Game.Current.CurrentMap.Collection.TryGetEntity(position.X, position.Y, out Entity output) && newEntity != null)
                {
                    Game.Current.CurrentMap.Collection.Add(newEntity);
                    MapEditor.MakeChange();
                }
            }
        }

        ///<summary>Attempts to remove an entity at this placement's location</summary>
        public static void Remove()
        {
            if(Game.Current.CurrentMap.Collection.TryGetEntity(position.X, position.Y, out Entity entity))
            {
                Game.Current.CurrentMap.Collection.Remove(entity);
                MapEditor.MakeChange();
            }
        }

        ///<summary>Assigns to the position of this placement</summary>
        public static void SetPosition(int positionX, int positionY) => position = new Vector(positionX, positionY);

        ///<summary>Toggles the use of the stamp tool on or off</summary>
        public static void ToggleStamp()
        {
            useStamp = !useStamp;
            if(useStamp && type == PlacementType.Tile) Place(); // Automatically places a tile if the stamp is toggled on and this placement is in tile placement mode
        }
    }
}