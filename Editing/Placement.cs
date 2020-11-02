using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
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

            position = Vector.Zero;
            type = PlacementType.Tile;
        }

        ///<summary>Moves this placement by an amount on the X and Y axes</summary>
        public static void Move(int deltaX, int deltaY)
        {
            if(position.Y + deltaY <= 0) position = new Vector(position.X, 0);
            else if((position.Y + deltaY) >= (Game.Current.CurrentMap.Height - 1)) position = new Vector(position.X, Game.Current.CurrentMap.Height - 1);
            else position += new Vector(0, deltaY);

            if(position.X + deltaX <= 0) position = new Vector(0, position.Y);
            else if((position.X + deltaX) >= (Game.Current.CurrentMap.Width - 1)) position = new Vector(Game.Current.CurrentMap.Width - 1, position.Y);
            else position += new Vector(deltaX, 0);

            if(useStamp && type == PlacementType.Tile) Place();

            Game.Current.SetCameraPosition(position.X / Map.AREA_SIZE, position.Y / Map.AREA_SIZE);
        }

        ///<summary>Draws a visual representation of this placement's position</summary>
        public static void Draw()
        {
            Console.BackgroundColor = colour;

            Console.SetCursorPosition((position.X * 2) % (Map.AREA_SIZE * 2), 4 + (position.Y % Map.AREA_SIZE));
            Console.Write("  ");
            Console.ResetColor();
        }

        ///<summary>Places either a tile or an entity at this placement's current position</summary>
        public static void Place()
        {
            if(type == PlacementType.Tile)
            {
                if(Game.Current.CurrentMap.TryGetTile(position.X, position.Y, out Tile output))
                {
                    if(output.Ascii == tile) return;

                    Game.Current.CurrentMap.SetTile(position, new Tile(tile, Game.Current.CurrentMap.TileSet));
                    MapEditor.MakeChange();
                }
            }
            else
            {
                Entity newEntity = null;

                switch (entity)
                {
                    case 'P':
                        newEntity = new Player($"P {Game.Current.CurrentMap.Collection.AssignId()} {position.ToString()} 20");
                        break;

                    case 'E':
                        newEntity = new Enemy($"E {Game.Current.CurrentMap.Collection.AssignId()} {position.ToString()} 20");
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
            if(useStamp && type == PlacementType.Tile) Place();
        }
    }
}