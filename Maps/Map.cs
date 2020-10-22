using NEA_Project_Oubliette.Entities;
using System.Collections.Generic;
using System.Text;
using System;

namespace NEA_Project_Oubliette.Maps
{
    ///<summary>Container of tiles and entities</summary>
    internal sealed class Map
    {
        private string name;

        private Tile[,] tiles;
        private TileSet tileSet;
        private EntityCollection collection;

        public string Name => name;

        public TileSet TileSet => tileSet;
        public EntityCollection Collection => collection;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public const int AREA_SIZE = 20;

        public Map(string name, string data, TileSet tileSet, params Entity[] entities)
        {
            this.name = name;
            this.tileSet = tileSet;

            collection = new EntityCollection(entities);

            string[] lines = data.Split('\n');
            int maxWidth = 0, maxHeight = lines.Length;

            for (int y = 0; y < lines.Length; y++)
                maxWidth = Math.Max(maxWidth, lines[y].Length);

            tiles = new Tile[maxHeight, maxWidth];

            Height = tiles.GetLength(0);
            Width = tiles.GetLength(1);

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    tiles[y, x] = new Tile(lines[y][x], tileSet);

            if(entities.Length > 0)
                for (int i = 0; i < entities.Length; i++)
                    if(TryGetTile(entities[i].Position, out Tile tile))
                        tile.Occupy(entities[i]);
        }

        ///<summary>Draws each tile to the screen</summary>
        public void Draw(int drawX, int drawY)
        {
            Console.SetCursorPosition(2, 2); // This is temporary

            drawX *= AREA_SIZE;
            drawY *= AREA_SIZE;

            for (int y = drawY; y < drawY + AREA_SIZE; y++)
            {
                for (int x = drawX; x < drawX + AREA_SIZE; x++)
                {
                    if(Collection.TryGetEntity(x, y, out Entity entity)) entity.Draw();
                    else tiles[y, x].Draw();
                }

                Display.WriteLine();
            }
        }

        ///<summary>Adds an empty area to the grid (to be used for level editing)</summary>
        public void AddArea(int directionX, int directionY, char fillCharacter = '.')
        {
            Tile[,] newTiles = new Tile[Height + (directionY * AREA_SIZE), Width + (directionX * AREA_SIZE)];
            Tile emptyTile = new Tile('\0', tileSet);

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    newTiles[y, x] = tiles[y, x];

            for (int y = 0; y < newTiles.GetLength(0); y++)
                for (int x = 0; x < newTiles.GetLength(1); x++)
                    if(newTiles[y, x] == emptyTile)
                        newTiles[y, x] = new Tile(fillCharacter, tileSet);

            tiles = newTiles;

            Height = tiles.GetLength(0);
            Width = tiles.GetLength(1);
        }

        ///<summary>Assigns to a tile at a location (to be used for level editing)</summary>
        public void SetTile(int tileX, int tileY, Tile tile) => tiles[tileY, tileX] = tile;

        ///<summary>Attempts to return a tile from a location</summary>
        public bool TryGetTile(int tileX, int tileY, out Tile output)
        {
            if(tileX >= 0 && tileX < Width && tileY >= 0 && tileY < Height)
            {
                output = tiles[tileY, tileX];
                return true;
            }

            output = null;
            return false;
        }

        ///<summary>Attempts to return a tile from a location</summary>
        public bool TryGetTile(Vector tileLocation, out Tile output)
        {
            if(tileLocation.X >= 0 && tileLocation.X < Width && tileLocation.Y >= 0 && tileLocation.Y < Height)
            {
                output = tiles[tileLocation.Y, tileLocation.X];
                return true;
            }

            output = null;
            return false;
        }

        ///<summary>Returns all neighbouring tiles at a location</summary>
        public Tile[] GetNeighbouringTiles(int tileX, int tileY)
        {
            List<Tile> neighbours = new List<Tile>();

            for (int neighbourY = -1; neighbourY <= 1; neighbourY++)
            {
                for (int neighbourX = -1; neighbourX <= 1; neighbourX++)
                {
                    if(neighbourX == 0 && neighbourY == 0) continue;

                    int checkX = tileX + neighbourX;
                    int checkY = tileY + neighbourY;

                    if(checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
                        neighbours.Add(tiles[checkY, checkX]);
                }
            }

            return neighbours.ToArray();
        }

        ///<summary>Returns each tile's ascii character, separating lines with '\' (to be used for serialization and deserialization)</summary>
        public string GetRawData()
        {
            StringBuilder mapData = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                if(y > 0) mapData.Append('\\');

                for (int x = 0; x < Width; x++)
                    mapData.Append(tiles[y, x].Ascii);
            }

            return mapData.ToString();
        }
    }
}