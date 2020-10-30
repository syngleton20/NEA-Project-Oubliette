using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Editing;
using System.Collections.Generic;
using System.Linq;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Provides simple methods for receiving input from the keyboard</summary>
    internal static class Input
    {
        private static ConsoleKeyInfo lastInput;

        public static bool IsControlKeyDown => lastInput.Modifiers.HasFlag(ConsoleModifiers.Control);
        public static bool IsShiftKeyDown => lastInput.Modifiers.HasFlag(ConsoleModifiers.Shift);
        public static bool IsAltKeyDown => lastInput.Modifiers.HasFlag(ConsoleModifiers.Alt);

        public static char KeyChar => lastInput.KeyChar;

        ///<summary>Halts the program and waits to return a keycode from the keyboard</summary>
        public static ConsoleKey GetKeyDown()
        {
            lastInput = Console.ReadKey(true);
            return lastInput.Key;
        }

        ///<summary>Gets user input to control the player entity</summary>
        public static void GetPlayerInput()
        {
            switch (GetKeyDown())
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    Player.Instance?.Move(0, -1);
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    Player.Instance?.Move(0, 1);
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    Player.Instance?.Move(-1, 0);
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    Player.Instance?.Move(1, 0);
                    break;

                case ConsoleKey.Escape:
                    int slotIndex = 0;

                    Display.Clear();
                    GUI.Title("Paused");

                    switch (GUI.VerticalMenu("Resume", "Save", "Load", "Main Menu", "Quit"))
                    {
                        case 0:
                            Display.Clear();
                            break;

                        case 1:
                            slotIndex = SaveManager.ChooseSlotToSave();
                            if(slotIndex < 0) break;

                            SaveManager.Save(slotIndex);
                            break;

                        case 2:
                            slotIndex = SaveManager.ChooseSlotToLoad();
                            if(slotIndex < 0) break;

                            Game.Current = SaveManager.Load(slotIndex);
                            Game.Current.Start();
                            break;

                        case 3:
                            Game.Current.Stop();
                            break;

                        case 4:
                            Environment.Exit(0);
                            break;
                    }
                    break;
            }
        }

        ///<summary>Gets user input to control the map editor</summary>
        public static void GetEditorInput()
        {
            switch(GetKeyDown())
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if(Placement.Position.Y > 0) Placement.Move(0, -1);
                    break;

                case ConsoleKey.DownArrow:
                    if(Placement.Position.Y >= (Game.Current.CurrentMap.Height - 1)) Game.Current.CurrentMap.AddArea(0, 1);
                    Placement.Move(0, 1);
                    break;

                case ConsoleKey.S:
                    if(!IsControlKeyDown)
                    {
                        if(Placement.Position.Y >= (Game.Current.CurrentMap.Height - 1)) Game.Current.CurrentMap.AddArea(0, 1);
                        Placement.Move(0, 1);
                    }
                    else MapEditor.Save();
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if(Placement.Position.X > 0) Placement.Move(-1, 0);
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if(Placement.Position.X >= (Game.Current.CurrentMap.Width - 1)) Game.Current.CurrentMap.AddArea(1, 0);
                    Placement.Move(1, 0);
                    break;

                case ConsoleKey.N:
                    if(IsControlKeyDown) MapEditor.New();
                    break;

                case ConsoleKey.O:
                    if(IsControlKeyDown) MapEditor.Open();
                    break;

                case ConsoleKey.Q:
                    Placement.ToggleStamp();
                    break;

                case ConsoleKey.Spacebar:
                    Placement.Place();
                    break;

                case ConsoleKey.Backspace:
                case ConsoleKey.Delete:
                    Placement.Remove();
                    break;

                case ConsoleKey.Tab:
                    if(Placement.Type == PlacementType.Entity) Placement.Type = PlacementType.Tile;
                    else Placement.Type = PlacementType.Entity;
                    break;

                case ConsoleKey.Escape:
                    Display.Clear();
                    GUI.Title("Paused");

                    switch (GUI.VerticalMenu("Resume", "Debugging", "New Map", "Open Map", "Save Map", "Save Map As", "Main Menu", "Quit"))
                    {
                        case 0:
                            Display.Clear();
                            break;

                        case 1:
                            Display.Clear();
                            GUI.Title("Debugging");

                            switch (GUI.VerticalMenu("Back", "Show Entity List"))
                            {
                                case 0:
                                    Display.Clear();
                                    break;

                                case 1:
                                    Entity[] entities = Game.Current.CurrentMap.Collection.Array;
                                    IEnumerable<Entity> orderedEntities = from entity in entities orderby entity.Id select entity;

                                    string[] entityStrings = new string[entities.Length];
                                    entities = orderedEntities.ToArray();

                                    Display.Clear();
                                    GUI.Title("Entity List");

                                    if(entities.Length > 0)
                                    {
                                        for (int i = 0; i < entities.Length; i++)
                                            entityStrings[i] = Display.SplitStringOverBufferWidth(EntityCollection.Names[entities[i].Save()[0]], entities[i].Save());

                                        GUI.VerticalScrollView(entityStrings);
                                    }
                                    else Display.WriteAtCentre("No entities in map!");

                                    Display.WriteAtCentre("Press any key to continue...");
                                    GetKeyDown();
                                    Display.Clear();
                                    break;
                            }
                            break;

                        case 2:
                            MapEditor.New();
                            break;

                        case 3:
                            MapEditor.Open();
                            break;

                        case 4:
                            MapEditor.Save();
                            break;

                        case 5:
                            MapEditor.SaveAs();
                            break;

                        case 6:
                            Game.Current.Stop();
                            break;

                        case 7:
                            Environment.Exit(0);
                            break;
                    }
                    break;

                default:
                    if(Placement.Type == PlacementType.Tile)
                    {
                        if(Game.Current.CurrentMap.TileSet.TryGetProfileFromAscii(lastInput.KeyChar, out TileProfile profile))
                            Placement.Tile = lastInput.KeyChar;
                    }
                    else
                    {
                        if(EntityCollection.Names.ContainsKey(lastInput.KeyChar))
                            Placement.Entity = lastInput.KeyChar;
                    }
                    break;
            }
        }
    }
}