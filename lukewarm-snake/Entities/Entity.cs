using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class EntityBatch
    {
        public List<Entity> entities { get; private set; }

        //Stores pre-set dictionary of entities with specific traits
        public Dictionary<Type, List<Entity>> traitBuckets { get; private set; }

        private Dictionary<Type, List<Entity>> entityBuckets { get; set; }

        public delegate void DisposeDelegate();
        public event DisposeDelegate disposeEvent;

        private Effect CRTShader;
        private float CRTTimer = 0f;
        public RenderTarget2D rt { get; private set; }
        public const float PixelateMultiplier = 1f / 8f;

        public EntityBatch()
        {
            entities = new List<Entity>();
            traitBuckets = new Dictionary<Type, List<Entity>>();
            entityBuckets = new Dictionary<Type, List<Entity>>();

            CRTShader ??= Globals.content.Load<Effect>(@"Effects/Pixel");
            rt = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, (int)(1600 * PixelateMultiplier), (int)(900 * PixelateMultiplier));
        }

        public void InitTraitBucket<T>()
        {
            if (!traitBuckets.ContainsKey(typeof(T)))
                traitBuckets.Add(typeof(T), new List<Entity>());
        }

        public void InitEntityBucket<T>() where T : Entity
        {
            if (!entityBuckets.ContainsKey(typeof(T)))
                entityBuckets.Add(typeof(T), new List<Entity>());
        }

        public List<Entity> GetTraitBucket<T>() => traitBuckets[typeof(T)];

        public List<Entity> GetEntityBucket<T>() where T : Entity => entityBuckets[typeof(T)];

        public void LoadMap(byte[] byteMap)
        {
            entities.Clear();

            foreach (var entry in traitBuckets)
                entry.Value.Clear();

            foreach (var entry in entityBuckets)
                entry.Value.Clear();
        }

        public void Add(Entity e)
        {
            entities.Add(e);
            e.batch = this;

            if (entityBuckets.ContainsKey(e.GetType()))
                entityBuckets[e.GetType()].Add(e);

            List<object> entityTraits = e.GetAllTraits();
            for (int i = 0; i < entityTraits.Count; i++)
            {
                object curTrait = entityTraits[i];
                Type traitType = curTrait.GetType();
                if (traitBuckets.ContainsKey(traitType))
                    traitBuckets[traitType].Add(e);
            }
        }

        public void Remove(Entity e)
        {
            entities.Remove(e);
            e.batch = null;
        }

        public void Update()
        {
            Globals.Camera.Update(Globals.gt);
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                entities[i].Update();


                if (entities[i].exists)
                    continue;

                //Remove references of entity
                Entity curEntity = entities[i];
                entities.RemoveAt(i);

                if (entityBuckets.ContainsKey(curEntity.GetType()))
                    entityBuckets[curEntity.GetType()].Remove(curEntity);

                List<object> entityTraits = curEntity.GetAllTraits();
                for (int j = 0; j < entityTraits.Count; j++)
                {
                    object curTrait = entityTraits[j];
                    Type traitType = curTrait.GetType();
                    if (traitBuckets.ContainsKey(traitType))
                        traitBuckets[traitType].Remove(curEntity);
                }
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < entities.Count; i++)
                entities[i].FixedUpdate();
        }

        public void Draw()
        {
            CRTTimer -= 0.017f;
            CRTShader.Parameters["iTime"].SetValue(CRTTimer);
            CRTShader.Parameters["vinVal"].SetValue(0.3f);
            CRTShader.CurrentTechnique.Passes[0].Apply();

            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            Globals.spriteBatch.GraphicsDevice.Clear(new Color(118, 59, 54));
            Globals.spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
            for (int i = 0; i < entities.Count; i++)
                entities[i].Draw();
            Globals.spriteBatch.End();

            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: CRTShader);
            Globals.spriteBatch.Draw(rt, new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height), Color.White);
            Globals.spriteBatch.End();
        }

        public void Dispose()
        {
            if (disposeEvent is not null)
                disposeEvent();
        }
    }

    public abstract class Entity
    {
        private List<object> traits;
        private Dictionary<string, object> traitSet;
        private List<TUpdates> tUpdates;
        private List<TDraws> tDraws;
        private List<TFixedUpdate> tFixedUpdates;

        public Vector2 Pos { get; set; }
        public float X
        {
            get => Pos.X;
            set => Pos = new Vector2(value, Pos.Y);
        }
        public float Y
        {
            get => Pos.Y;
            set => Pos = new Vector2(Pos.X, value);
        }

        protected Vector2 prevPos;
        public Vector2 DrawPos { get; protected set; }


        public Vector2 DeltaPos { get; set; } = Vector2.Zero;
        public float dx
        {
            get => DeltaPos.X;
            set => DeltaPos = new Vector2(value, DeltaPos.Y);
        }
        public float dy
        {
            get => DeltaPos.Y;
            set => DeltaPos = new Vector2(DeltaPos.X, value);
        }

        public float Width;
        public float Height;

        public bool isVisible = true;

        public bool exists { get; set; } = true;

        public EntityBatch batch { get; set; }

        public Entity(Vector2 pos)
        {
            traits = new List<object>();
            traitSet = new Dictionary<string, object>();
            tUpdates = new List<TUpdates>();
            tDraws = new List<TDraws>();
            tFixedUpdates = new List<TFixedUpdate>();
            Pos = pos;
            prevPos = pos;
        }

        public void AddTrait<T>(T t)
        {
            if (HasTrait<T>())
                return;
            traitSet.Add(typeof(T).FullName, t);
            traits.Add(t);

            if (t is TUpdates tu)
            {
                tUpdates.Add(tu);
                tUpdates.Sort((a, b) =>
                {
                    return a.Priority.CompareTo(b.Priority);
                });
            }

            if (t is TDraws td)
                tDraws.Add(td);

            if (t is TFixedUpdate tf)
            {
                tFixedUpdates.Add(tf);
                tFixedUpdates.Sort((a, b) => { return a.priority.CompareTo(b.priority); });
            }
                
        }

        public T GetTrait<T>()
        {
            traitSet.TryGetValue(typeof(T).FullName, out object toReturn);
            return (T)toReturn;
        }

        public List<object> GetTraits(params Type[] types)
        {
            List<object> toReturn = new List<object>();
            foreach (Type T in types)
            {
                object toAdd;
                if (traitSet.TryGetValue(T.FullName, out toAdd))
                    toReturn.Add(toAdd);
            }
            return toReturn;
        }

        public List<object> GetAllTraits() => new(traits);

        public bool HasTrait<T>()
        {
            return traitSet.ContainsKey(typeof(T).FullName);
        }

        public virtual void Update()
        {
            for (int i = 0; i < tUpdates.Count; i++)
                /*if (traits[i].isActive)*/ tUpdates[i].Update();
        }

        public virtual void FixedUpdate()
        {
            prevPos = Pos;
            for (int i = 0; i < tFixedUpdates.Count; i++)
                tFixedUpdates[i].FixedUpdate();

            if (MathF.Abs(dx) < .01f) dx = 0;
            if (MathF.Abs(dy) < .01f) dy = 0;
            Pos += DeltaPos;
        }

        public virtual void Draw() 
        {
            if (!isVisible) return;
            DrawPos = Vector2.Lerp(prevPos, Pos, Globals.ALPHA);
            for (int i = 0; i < tDraws.Count; i++)
                tDraws[i].Draw();
        }
    }
}
