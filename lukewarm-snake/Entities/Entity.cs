using lukewarm_snake;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlackMagic.Globals;

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
        public const int PixelateScaler = 8;
        public const float PixelateMultiplier = 1f / PixelateScaler;

        //Background effect variables
        private Effect BackgroundEffect;
        private RenderTarget2D backgroundRt;

        //Water ripple effect variables
        private Effect WaterRippleEffect;
        public RenderTarget2D RippleInfluenceBuffer { get; set; }
        private RenderTarget2D hiddenRippleInfluenceBuffer;
        private RenderTarget2D rippleBuffer1;
        private RenderTarget2D rippleBuffer2;
        private RenderTarget2D nextRippleFrame;
        private const float Damping = 0.99f;
        private const float TimeStep = 16f;
        private float rippleEffectTimeAccumulator = TimeStep + 1f;

        public RenderTarget2D rt { get; private set; }
        private RenderTarget2D rtBuffer;

        public EntityBatch()
        {
            entities = new List<Entity>();
            traitBuckets = new Dictionary<Type, List<Entity>>();
            entityBuckets = new Dictionary<Type, List<Entity>>();

            CRTShader ??= Globals.content.Load<Effect>(@"Effects/Pixel");
            rt = new RenderTarget2D(spriteBatch.GraphicsDevice, (int)(Globals.Camera.Width * PixelateMultiplier), (int)(Globals.Camera.Height * PixelateMultiplier));
            rtBuffer = new RenderTarget2D(spriteBatch.GraphicsDevice, rt.Width, rt.Height);

            WaterRippleEffect = content.Load<Effect>(@"Effects/WaterRipple");
            rippleBuffer1 = new RenderTarget2D(spriteBatch.GraphicsDevice, rt.Width, rt.Height);
            rippleBuffer2 = new RenderTarget2D(spriteBatch.GraphicsDevice, rt.Width, rt.Height);
            nextRippleFrame = new RenderTarget2D(spriteBatch.GraphicsDevice, rt.Width, rt.Height);
            RippleInfluenceBuffer = new RenderTarget2D(spriteBatch.GraphicsDevice, rt.Width, rt.Height);
            hiddenRippleInfluenceBuffer = new RenderTarget2D(spriteBatch.GraphicsDevice, rt.Width, rt.Height);

            BackgroundEffect = content.Load<Effect>(@"Effects/Background");
            backgroundRt = new RenderTarget2D(spriteBatch.GraphicsDevice, rt.Width, rt.Height);
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

        public List<Entity> GetEntityBucket<T>() where T : Entity => entityBuckets.ContainsKey(typeof(T)) ? entityBuckets[typeof(T)] : null;

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

        public void DrawRippleInfluence()
        {
            spriteBatch.GraphicsDevice.SetRenderTarget(RippleInfluenceBuffer);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int i = 0; i < entities.Count; i++)
                entities[i].DrawRippleInfluence();
            spriteBatch.End();
        }

        public void Draw()
        {
            DrawRippleInfluence();

            //Draw entities to main render target
            spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.GraphicsDevice.Clear(/*new Color(118, 59, 54)*/ Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
            for (int i = 0; i < entities.Count; i++)
                entities[i].Draw();
            spriteBatch.End();


            /*  <Handle Background Effect>  */

            /*  <Water Ripple Effect>  */

            //Add ripple influence buffer to background buffer 1
            spriteBatch.GraphicsDevice.SetRenderTarget(hiddenRippleInfluenceBuffer);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(rippleBuffer1,
                new Rectangle(0, 0, rippleBuffer1.Width, rippleBuffer1.Height),
                Color.White);
            spriteBatch.Draw(RippleInfluenceBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            //Swap buffers
            (rippleBuffer1, hiddenRippleInfluenceBuffer) = (hiddenRippleInfluenceBuffer, rippleBuffer1);
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            //Handle time manipulation
            rippleEffectTimeAccumulator += MathF.Max(TimeMod, MinTimeMod);
            for(; rippleEffectTimeAccumulator > TimeStep; rippleEffectTimeAccumulator -= TimeStep)
            {
                //Set parameters
                WaterRippleEffect.Parameters["iResolution"].SetValue(new Vector2(nextRippleFrame.Width, nextRippleFrame.Height));
                WaterRippleEffect.Parameters["Damping"].SetValue(Damping);
                WaterRippleEffect.Parameters["Previous"].SetValue(rippleBuffer1);
                WaterRippleEffect.CurrentTechnique.Passes[0].Apply();

                //Calculate new frame of background
                spriteBatch.GraphicsDevice.SetRenderTarget(nextRippleFrame);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(effect: WaterRippleEffect);
                spriteBatch.Draw(rippleBuffer2, Vector2.Zero, Color.White);
                spriteBatch.End();

                //Copy contents of BackgroundRt to BackgroundBuffer2
                spriteBatch.GraphicsDevice.SetRenderTarget(rippleBuffer2);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin();
                spriteBatch.Draw(nextRippleFrame, Vector2.Zero, Color.White);
                spriteBatch.End();

                //Swap buffers
                (rippleBuffer2, rippleBuffer1) = (rippleBuffer1, rippleBuffer2);
                /*  </Water Ripple Effect>  */

                //Render Background
                spriteBatch.GraphicsDevice.SetRenderTarget(backgroundRt);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(effect: BackgroundEffect);
                spriteBatch.Draw(rippleBuffer1, Vector2.Zero, Color.White);
                spriteBatch.End();
            }


            /*  </Handle Background Effect>  */


            //Draw to screen
            CRTTimer -= 0.017f;
            CRTShader.Parameters["iTime"].SetValue(CRTTimer);
            CRTShader.Parameters["vinVal"].SetValue(0.3f);
            CRTShader.CurrentTechnique.Passes[0].Apply();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: CRTShader, sortMode: SpriteSortMode.Immediate);

            //Draw background
            spriteBatch.Draw(backgroundRt,
                new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height),
                Color.White);

            //Draw sprites
            spriteBatch.Draw(rt, new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height), Color.White);

            spriteBatch.End();
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
        private List<TDrawsRippleInfluence> tDrawsRippleInfluences;

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
            tDrawsRippleInfluences = new List<TDrawsRippleInfluence>();
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

            if (t is TDrawsRippleInfluence tri)
                tDrawsRippleInfluences.Add(tri);                
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

        public virtual void DrawRippleInfluence()
        {
            if (!isVisible) return;
            DrawPos = Vector2.Lerp(prevPos, Pos, Globals.ALPHA);
            for (int i = 0; i < tDrawsRippleInfluences.Count; i++)
                tDrawsRippleInfluences[i].DrawRippleInfluence();
        }
    }
}
