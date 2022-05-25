using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Scene
    {

        public List<Entity> Entities { get; private set; }
        public Renderer Renderer { get; set; }
        public Scene()
        {
            this.Entities = new List<Entity>();
        }
        public void addEntity(Entity entity)
        {
            entity.scene = this;
            Entities.Add(entity);
        }
        public void Update()
        {
            foreach(Entity entity in Entities)
            {
                entity.Update();
            }
            HandleCollisions();
        }
        public void Render()
        {
            Renderer.Render(this);
        }
        private void HandleCollisions()
        {
            List<PolygonCollider> allColliders = new List<PolygonCollider>();
            foreach(Entity e in Entities)
            {
                foreach(Component c in e.Updatables)
                {
                    if(c is PolygonCollider)
                    {
                        allColliders.Add(c as PolygonCollider);
                    }
                }
            }
            for(int i = 0; i < allColliders.Count; i++)
            {
                for(int j = i+1; j < allColliders.Count; j++)
                {
                    PolygonCollider.CheckCollision(allColliders[i], allColliders[j], out CollisionResult collisionResult);
                    if(collisionResult != null)
                    {
                        collisionResult.DefaultResolve();
                        allColliders[i].OnCollidedWith(allColliders[j]);
                        allColliders[j].OnCollidedWith(allColliders[i]);
                    }
                }
            }
        }
    }
}
