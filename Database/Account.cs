namespace NEA_Project_Oubliette.Database
{
    internal abstract class Account
    {
        public abstract int UserID { get; }
        public abstract string Username { get; }

        public abstract void Update(params string[] details);
    }
}