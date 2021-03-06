using NEA_Project_Oubliette.Maps;
using System.Collections.Generic;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Provides a way of handling and referencing various entities</summary>
    internal sealed class EntityCollection
    {
        private List<Entity> entities = new List<Entity>();

        public Entity[] Array => entities.ToArray();

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
        public void Remove(Entity entity) { if(entities.Contains(entity)) entities.Remove(entity); }

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
            foreach (Entity entity in entities)
                entity.Update();
        }

        ///<summary>Returns a unique id for a new entity</summary>
        public int AssignId()
        {
            int previousId = 0;

            for (int i = 1; i < entities.Count; i++)
            {
                if(entities[i - 1].Id == previousId) previousId++;
                else return previousId;
            }

            return previousId + 1;
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
    }
}