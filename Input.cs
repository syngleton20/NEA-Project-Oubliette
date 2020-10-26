using NEA_Project_Oubliette.Entities;
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

        ///<summary>Halts the program and waits to return a keycode from the keyboard</summary>
        public static ConsoleKey GetKeyDown()
        {
            lastInput = Console.ReadKey(true);
            return lastInput.Key;
        }

        public static void GetPlayerInput()
        {
            switch (Input.GetKeyDown())
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    Player.Instance.Move(0, -1);
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    Player.Instance.Move(0, 1);
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    Player.Instance.Move(-1, 0);
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    Player.Instance.Move(1, 0);
                    break;

                case ConsoleKey.Escape:
                    int slotIndex = 0;

                    Display.Clear();
                    GUI.Title("Paused");

                    switch (GUI.VerticalMenu("Resume", "Save", "Load", "Main Menu", "Quit"))
                    {
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
    }
}