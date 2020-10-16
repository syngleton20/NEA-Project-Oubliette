using System.Collections.Generic;

namespace NEA_Project_Oubliette.Entities
{
    ///<summary>Provides a way of handling and referencing various entities</summary>
    internal sealed class EntityCollection
    {
        private List<Entity> entities = new List<Entity>();

        public EntityCollection(params Entity[] initialEntities) => entities.AddRange(initialEntities);

        ///<summary>Adds a non-existent entity to the collection</summary>
        public void Add(Entity entity) { if(!entities.Contains(entity)) entities.Add(entity); }

        ///<summary>Removes an existing entity from the collection</summary>
        public void Remove(Entity entity) { if(entities.Contains(entity)) entities.Remove(entity); }

        ///<summary>Clears the collection</summary>
        public void Clear() => entities.Clear();

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