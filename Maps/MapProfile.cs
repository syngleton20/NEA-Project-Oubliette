namespace NEA_Project_Oubliette.Maps
{
    internal struct MapProfile
    {
        public int ID { get; private set; }
        public int Downloads { get; private set; }

        public string Name { get; private set; }
        public string Author { get; private set; }

        public MapProfile(int id, string name, string author)
        {
            ID = id;
            Downloads = 0;
            Name = name;
            Author = author;
        }

        public MapProfile(int id, string name, string author, int downloads)
        {
            ID = id;
            Downloads = downloads;
            Name = name;
            Author = author;
        }
    }
}