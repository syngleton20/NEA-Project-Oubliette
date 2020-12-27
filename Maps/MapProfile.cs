namespace NEA_Project_Oubliette.Maps
{
    internal struct MapProfile
    {
        public int ID { get; private set; }

        public string Name { get; private set; }
        public string Author { get; private set; }

        public MapProfile(int id, string name, string author)
        {
            ID = id;
            Name = name;
            Author = author;
        }
    }
}