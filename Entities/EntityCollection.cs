using NEA_Project_Oubliette.Maps;
using System.Collections.Generic;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Provides a way of handling and referencing various entities</summary>
    internal sealed class EntityCollection
    {
        private List<Entity> entities = new List<Entity>();

        public Entity[] Array => entities.ToArray();

        public static Dictionary<char, string> Names = new Dictionary<char, string>()
        {
            { 'P', "Player" },
            { 'E', "Enemy" },
            { 'I', "Pickup" }
        };

        public EntityCollection(params Entity[] initialEntities) => entities.AddRange(initialEntities);

        ///<summary>Adds a non-existent entity to the collection</summary>
        public void Add(Entity entity)
        {
            if(!entities.Contains(entity))
            {
                entities.Add(entity);

                if(Game.Current.CurrentMap.TryGetTile(entity.Position, out Tile tile))
                    tile.Occupy(entity);
            }
        }

        ///<summary>Adds non-existent entities to the collection</summary>
        public void Add(params Entity[] entities) { foreach (Entity entity in entities) Add(entity); }

        ///<summary>Removes an existing entity from the collection</summary>
        public void Remove(Entity entity)
        {
            if(entities.Contains(entity))
            {
                entity.OnDestroy();
                entities.Remove(entity);
            }
        }

        ///<summary>Clears the collection</summary>
        public void Clear()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if(Game.Current.CurrentMap.TryGetTile(entities[i].Position, out Tile tile))
                {
                    tile.Vacate();
                    entities[i] = null;
                }
            }

            entities.Clear();
        }

        ///<summary>Calls the Update() method on all entities in the collection</summary>
        public void UpdateAll()
        {
            for (int i = 0; i < entities.Count; i++)
                entities[i]?.Update();
        }

        ///<summary>Returns a unique id for a new entity</summary>
        public int AssignId()
        {
            int nextFreeId = 0;

            if(entities.Count <= 0) return nextFreeId;

            for (int i = 0; i < entities.Count; i++)
            {
                if(!TryGetEntityById(nextFreeId, out Entity entity))
                    return nextFreeId;

                nextFreeId++;
            }

            return nextFreeId;
        }

        ///<summary>Attempts to return an entity at a position</summary>
        public bool TryGetEntity(int positionX, int positionY, out Entity output)
        {
            Vector position = new Vector(positionX, positionY);

            foreach (Entity entity in entities)
            {
                if(entity.Position == position)
                {
                    output = entity;
                    return true;
                }
            }

            output = null;
            return false;
        }

        ///<summary>Attempts to return an entity at a position</summary>
        public bool TryGetEntity(Vector position, out Entity output)
        {
            foreach (Entity entity in entities)
            {
                if(entity.Position == position)
                {
                    output = entity;
                    return true;
                }
            }

            output = null;
            return false;
        }

        ///<summary>Attempts to return an entity with an id</summary>
        public bool TryGetEntityById(int id, out Entity output)
        {
            foreach (Entity entity in entities)
            {
                if(entity.Id == id)
                {
                    output = entity;
                    return true;
                }
            }

            output = null;
            return false;
        }

        ///<summary>Returns whether or not an entity of a certain type is contained in this collection</summary>
        public bool ContainsEntityOfType<T>() where T : Entity
        {
            foreach (Entity entity in entities)
                if(entity.GetType() == typeof(T))
                    return true;

            return false;
        }
    }
}