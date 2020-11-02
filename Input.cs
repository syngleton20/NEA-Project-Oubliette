using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Editing;
using System;
using NEA_Project_Oubliette.Maps;

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
                    if(Placement.Position.Y > 0) Placement.Move(0, -1 * (Input.IsShiftKeyDown ? 4 : 1));
                    break;

                case ConsoleKey.DownArrow:
                    if(Placement.Position.Y >= (Game.Current.CurrentMap.Height - 1)) Game.Current.CurrentMap.AddArea(0, 1);
                    Placement.Move(0, 1 * (Input.IsShiftKeyDown ? 4 : 1));
                    break;

                case ConsoleKey.S:
                    if(!IsControlKeyDown)
                    {
                        if(Placement.Position.Y >= (Game.Current.CurrentMap.Height - 1)) Game.Current.CurrentMap.AddArea(0, 1);
                        Placement.Move(0, 1 * (Input.IsShiftKeyDown ? 4 : 1));
                    }
                    else MapEditor.Save();
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if(Placement.Position.X > 0) Placement.Move(-1 * (Input.IsShiftKeyDown ? 4 : 1), 0);
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if(Placement.Position.X >= (Game.Current.CurrentMap.Width - 1)) Game.Current.CurrentMap.AddArea(1, 0);
                    Placement.Move(1 * (Input.IsShiftKeyDown ? 4 : 1), 0);
                    break;

                case ConsoleKey.N:
                    if(IsControlKeyDown) MapEditor.New();
                    break;

                case ConsoleKey.O:
                    if(IsControlKeyDown) MapEditor.Open();
                    break;

                case ConsoleKey.F:
                    if(IsControlKeyDown) MapEditor.Fill();
                    break;

                case ConsoleKey.Q:
                    Placement.ToggleStamp();
                    break;

                case ConsoleKey.Spacebar:
                    Placement.Place();
                    break;

                case ConsoleKey.Enter:
                    MapEditor.EnterPlayMode();
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
                            MapEditor.DebuggingMenu();
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
                            GUI.Title("Exit to Main Menu");

                            if(!GUI.YesOrNo("Are you sure you want to leave without saving?\nYour changes will not be saved."))
                            {
                                Display.Clear();
                                return;
                            }

                            MapEditor.Reset();
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
                        if(TileSet.TryGetProfileFromAscii(lastInput.KeyChar, out TileProfile profile))
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

        public static void GetTestInput()
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
                    MapEditor.ExitPlayMode();
                    break;
            }
        }
    }
}