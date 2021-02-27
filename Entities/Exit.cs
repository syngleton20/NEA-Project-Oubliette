using NEA_Project_Oubliette.Entities;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Acts as the finishing point in a map</summary>
    internal sealed class Exit : Entity
    {
        public Exit(int startX, int startY) => position = new Vector(startX, startY);
        public Exit(string data) : base(data) => Load(data);

        // In this case, if the player pushes against this entity, the current game will end
        public override void Push(int deltaX, int deltaY, Entity pusher) { if(pusher.GetType() == typeof(Player)) Game.Current.Finish(); }

        public override void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("██");
            Console.ResetColor();
        }

        public override void Load(string data)
        {
            string[] parts = data.Split(' ');

            id = parts[1].ToInt();
            position = parts[2].ToVector();
        }

        public override string Save() => $"X {id} {position.ToString()}";
    }
}