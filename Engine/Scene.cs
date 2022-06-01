using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Engine.CollisionResult;

namespace Engine
{
    /// <summary>
    /// Klasa do której można dodać komponenty, aby były aktualizowany z każdym krokiem gry i/lub rysowane z każdą klatką gry.
    /// Posiada obiekty decydujące o dynamice ruchu, dodawaniu transformacji oraz reakcji na kolizje
    /// Odpowiada za zarządzanie kolizjami
    /// </summary>
    public class Scene
    {
        public CollisionResolution CollisionResolution { get; set; }
        public List<Entity> Entities { get; private set; }
        public Renderer Renderer { get; set; }

        public DynamicsProvider DynamicsProvider { get; set; }
        public Transformer Transformer { get; set; }

        public bool Pause { get; set; }
        public Scene()
        {
            DynamicsProvider = new EuclideanDynamicsProvider();
            this.Entities = new List<Entity>();
        }
        public void addEntity(Entity entity)
        {
            entity.scene = this;
            Entities.Add(entity);
            entity.OnAddedToScene();
        }
        public void Update()
        {
            if(!Pause)
            {
                foreach (Entity entity in Entities)
                {
                    entity.Update();
                }
                HandleCollisions();
            }
            
        }
        public void Render()
        {
            if (!Pause)
            {
                Renderer.Render(this);
            }
        }
        private void HandleCollisions()
        {
            for(int i = 0; i < Entities.Count; i++)
            {
                for(int j = i+1; j < Entities.Count; j++)
                {
                    Entity entity1 = Entities[i], entity2 = Entities[j];
                    for(int k = 0; k < entity1.Updatables.Count; k++)
                    {
                        if(entity1.Updatables[k] is PolygonCollider collider1)
                            for(int l = 0; l < entity2.Updatables.Count; l++)
                            {
                                if (entity2.Updatables[l] is PolygonCollider collider2)
                                {
                                    PolygonCollider.CheckCollision(collider1, collider2, out CollisionResult collisionResult);
                                    if (collisionResult != null)
                                    {
                                        if(CollisionResolution != null)
                                            CollisionResolution(collisionResult);
                                        else
                                            collisionResult.DefaultResolve();
                                        collider1.OnCollidedWith(collider2);
                                        collider2.OnCollidedWith(collider1);
                                    }
                                }
                            }
                    }
                }
            }
        }
    }
}
