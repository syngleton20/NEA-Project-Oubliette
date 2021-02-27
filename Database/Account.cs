namespace NEA_Project_Oubliette.Database
{
    ///<summary>Base class for accounts</summary>
    internal abstract class Account
    {
        public abstract int UserID { get; }
        public abstract string Username { get; }

        ///<summary>Allows specific information to be updated</summary>
        public abstract void Update(params string[] details);
    }
}