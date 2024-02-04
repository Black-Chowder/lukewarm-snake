using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BlackMagic
{
    //Handles playing requested animations and storing animation sequences
    public class Animator
    {
        private const double twos = 1000 / 12d; //12 fps

        private const string loopPattern = @".+_loop$";
        private const string dataPattern = @".+_data$";
        private static Regex loopRegex = null;
        private static Regex dataRegex = null;

        public struct LayerData //TODO: integrate this with the rest of the system
        {
            public Rectangle Src { get; set; }
            public Vector2 Offset { get; set; }
            public LayerData(Rectangle src, Vector2 offset)
            {
                Src = src;
                Offset = offset;
            }
        }

        private class AnimationLayer
        {
            public string Name { get; private set; }
            public int Count { get => layerData.Length; }
            public bool IsDataLayer { get; set; }
            public bool IsVisible { get; set; } = true;

            protected LayerData[] layerData;

            public AnimationLayer(string name, List<Rectangle> _srcRects, List<Vector2> _offsets = null, bool isDataLayer = false)
            {
                Name = name;
                IsDataLayer = isDataLayer;
                Rectangle[] srcRects = _srcRects.ToArray();

                Vector2[] offsets;
                if (_offsets is null)
                {
                    offsets = new Vector2[Count];
                    for (int i = 0; i < Count; i++)
                        offsets[i] = Vector2.Zero;
                }
                else if (_offsets.Count != _srcRects.Count)
                    throw new Exception("Number of offsets not equal to number of source rectangles");

                else
                    offsets = _offsets.ToArray();

                int count = srcRects.Length;
                layerData = new LayerData[count];
                for (int i = 0; i < count; i++)
                {
                    layerData[i] = new LayerData(srcRects[i], offsets[i]);
                }
            }

            public Rectangle this[int index] { get => layerData[index].Src; }
            public Rectangle GetSrcRect(int index) => layerData[index].Src;

            public Vector2 GetOffset(int index) => layerData[index].Offset;
        }

        //Stores and handles rectangles that represent locations on the sprite sheet for each frame of animation
        private class Animation
        {
            public string Name { get; private set; }
            public bool IsLooping { get; set; } = false;
            public double TotalDuration { get; private set; } = 0d;
            public Point SrcSize { get; private set; }
            public int Count { get => Layers[0].Count; } //Returns number of frames in animation
            public int LayersCount { get => Layers.Length; }

            private double[] durations; //milliseconds

            public AnimationLayer[] Layers { get; private set; }
            private Dictionary<string, AnimationLayer> dataLayers;

            public Animation(string name, Point srcSize, List<Rectangle> srcRects,
                bool isLooping = false,
                List<double> durations = null,
                List<Vector2> animationOffsets = null,
                Dictionary<string, AnimationLayer> dataLayers = null)
            {
                Name = name;
                this.SrcSize = srcSize;

                AnimationLayer layer = new AnimationLayer("auto generated", srcRects, animationOffsets);
                Layers = new AnimationLayer[1];
                Layers[0] = layer;
                this.IsLooping = isLooping;

                this.dataLayers = dataLayers ?? new Dictionary<string, AnimationLayer>();

                setupDurations(durations);
            }

            public Animation(string name, Point srcSize, List<AnimationLayer> layers,
                bool isLooping = false,
                List<double> durations = null,
                Dictionary<string, AnimationLayer> dataLayers = null)
            {
                Name = name;
                this.SrcSize = srcSize;
                this.IsLooping = isLooping;
                this.Layers = layers.ToArray();

                this.dataLayers = dataLayers ?? new Dictionary<string, AnimationLayer>();

                setupDurations(durations);
            }

            //Some checks and whatnot for setting up duration-related variables.  Used by constructors
            private void setupDurations(List<double> durations = null)
            {
                if (durations is null)
                {
                    this.durations = new double[Count];
                    for (int i = 0; i < Count; i++)
                        this.durations[i] = twos;
                    TotalDuration = twos * Count;
                }
                else if (durations.Count != Count)
                    throw new Exception("Number of durations not equal to number of source rectangles");

                else
                {
                    this.durations = durations.ToArray();
                    for (int i = 0; i < Count; i++)
                        TotalDuration += durations[i];
                }

            }

            public void SetTotalDuration(TimeSpan duration)
            {
                for (int i = 0; i < durations.Length; i++)
                {
                    durations[i] = durations[i] * duration.TotalMilliseconds / TotalDuration;
                }
                TotalDuration = duration.TotalMilliseconds;
            }

            public AnimationLayer GetLayer(int index) => Layers[index];
            public AnimationLayer GetDataLayer(string layerName) => dataLayers[layerName];
            public double GetDuration(int index) => durations[index];

            public int GetAnimationIndex(double duration)
            {
                //I'm going to loop through the durations for now, but it should probably be stored in a dictionary
                //and calculate the key based on the duration to get the index
                double durationCount = 0;
                for (int i = 0; i < Count; i++)
                {
                    durationCount += durations[i];
                    if (duration <= durationCount)
                        return i;
                }
                return Count - 1;
            }
        }
        private Animation animation;
        private Dictionary<string, Animation> animations;

        //Controls which sprite to display in animation
        public int AnimationIndex { get; private set; }

        //Used to keep track of time since animation started
        private TimeSpan curAnimationTimer;
        private bool isBeginningAnimation = false;

        //Tracks if animation is over
        private bool animationOver = false;
        public bool isAnimationOver() { return animationOver; }

        public bool isFacingRight
        {
            get => spriteEffects != SpriteEffects.FlipHorizontally;
            set => spriteEffects = value ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public float scale { get; set; } = 1f;

        public float gameScale { private get; set; } = 1f;
        public float cameraZoom { private get; set; } = 1f;

        public SpriteEffects spriteEffects { get; set; } = SpriteEffects.None;

        public Color color { get; set; } = Color.White;
        public float opacity { get; set; } = 1f;

        public float angle { get; set; } = 0;

        public int width { get; private set; }
        public int height { get; private set; }
        public int columns { get; private set; }
        public int rows { get; private set; }

        public Vector2 origin { get; set; }

        public float layer { get; set; } = 0;

        //TODO: Allow for storage of multiple sprite sheets
        public Texture2D spriteSheet { get; set; }

        public Animator(Texture2D spriteSheet, AsepriteData data)
        {
            loopRegex ??= new Regex(loopPattern);
            dataRegex ??= new Regex(dataPattern);

            this.spriteSheet = spriteSheet;
            this.width = data.meta.size.w;
            this.height = data.meta.size.h;
            curAnimationTimer = new TimeSpan(0);

            animations = new Dictionary<string, Animation>();
            animation = animations.FirstOrDefault().Value;
            isFacingRight = true;

            //Get data for each frame
            List<Rectangle> srcRects = new List<Rectangle>();
            List<Vector2> offsets = new List<Vector2>();
            List<double> durations = new List<double>();
            for (int i = 0; i < data.frames.Length; i++)
            {
                AseRect frameSrc = data.frames[i].frame;
                srcRects.Add(new Rectangle(frameSrc.x, frameSrc.y, frameSrc.w, frameSrc.h));

                AseRect offset = data.frames[i].spriteSourceSize;
                offsets.Add(new Vector2(offset.x, offset.y));

                double duration = data.frames[i].duration;
                durations.Add(duration);
            }

            //Add animations
            //TODO: allow for layer data that does not separate layers
            int layersCount = data.meta.layers.Length;
            for (int i = 0; i < data.meta.frameTags.Length; i++)
            {
                TagData tagData = data.meta.frameTags[i];

                int count = tagData.to - tagData.from + 1;
                bool isLooping = loopRegex.IsMatch(tagData.name);

                //Get data for all layers for tag
                List<AnimationLayer> layers = new List<AnimationLayer>();
                Dictionary<string, AnimationLayer> dataLayers = new Dictionary<string, AnimationLayer>();
                for (int j = 0; j < layersCount; j++)
                {
                    //Get source rectangles and and offsets for current tag
                    int layerDataOffset = (int)(((float)j / layersCount) * srcRects.Count);
                    List<Rectangle> curSrcRects = srcRects.GetRange(tagData.from + layerDataOffset, count);
                    List<Vector2> curOffsets = offsets.GetRange(tagData.from + layerDataOffset, count);

                    string layerName = data.meta.layers[j].name;

                    AnimationLayer layer = new AnimationLayer(layerName, curSrcRects, curOffsets);

                    //Add layer to corresponding group (data layer or normal render layers)
                    if (!dataRegex.IsMatch(layerName))
                        layers.Add(layer);
                    else
                        dataLayers.Add(layerName, layer);
                }
                List<double> curDurations = durations.GetRange(tagData.from, count);

                AseSize rawSrcSize = data.frames[0].sourceSize;
                Point srcSize = new Point(rawSrcSize.w, rawSrcSize.h);

                AddAnimation(tagData.name, srcSize, layers, isLooping: isLooping, durations: curDurations, dataLayers: dataLayers);
            }
        }

        private void AddAnimation(string name, Point srcSize, List<AnimationLayer> layers,
            bool isLooping = false,
            List<double> durations = null,
            Dictionary<string, AnimationLayer> dataLayers = null)
        {
            animations.Add(name, new Animation(name, srcSize, layers, isLooping: isLooping, durations: durations, dataLayers: dataLayers));
        }

        private void AddAnimation(string name, Point srcSize, List<Rectangle> srcRects,
            bool isLooping = false,
            List<double> durations = null,
            List<Vector2> offsets = null)
        {
            animations.Add(name, new Animation(name, srcSize, srcRects, isLooping, durations, offsets));
        }

        public void SetAnimation(string name,
            int animationIndex = -1)
        {
            animation = animations[name];

            //Set animation index
            if (animationIndex == 0)
                isBeginningAnimation = true;
            else if (animationIndex < 0)
                animationIndex = this.AnimationIndex;

            else if (animationIndex >= animation.Count)
                curAnimationTimer = new TimeSpan(0, 0, 0, 0, (int)animation.TotalDuration);

            else
                curAnimationTimer = new TimeSpan(0, 0, 0, 0, (int)animation.GetDuration(animationIndex));

            this.AnimationIndex = animation.GetAnimationIndex(curAnimationTimer.TotalMilliseconds);
        }

        //Returns name of animation currently playing
        //TODO: Make GetAnimation O(1)
        public string GetAnimation()
        {
            foreach (KeyValuePair<string, Animation> ani in animations)
                if (ani.Value == animation)
                    return ani.Key;
            return "";
        }

        public void SetAnimationDuration(string animationName, TimeSpan timeSpan)
        {
            Animation animation = animations[animationName];
            animation.SetTotalDuration(timeSpan);
        }

        public void SetAnimationVisibility(string animation, string layer, bool isVisible)
        {
            AnimationLayer[] layers = animations[animation].Layers;
            foreach (AnimationLayer curLayer in layers)
            {
                if (curLayer.Name != layer)
                    continue;
                curLayer.IsVisible = isVisible;
                break;
            }
        }

        public void SetAnimationVisibility(string layer, bool isVisible)
        {
            foreach(var keyValuePair in animations)
            {
                AnimationLayer[] layers = keyValuePair.Value.Layers;
                foreach (AnimationLayer curLayer in layers)
                {
                    if (curLayer.Name != layer)
                        continue;
                    curLayer.IsVisible = isVisible;
                    break;
                }
            }
        }

        public Dictionary<string, List<Color[,]>> GetData(string dataLayerName, out Dictionary<string, LayerData[]> layerDatas)
        {
            //Get the source rectangles for each animation
            layerDatas = new Dictionary<string, LayerData[]>();
            foreach (KeyValuePair<string, Animation> entry in animations)
            {
                AnimationLayer dataLayer = entry.Value.GetDataLayer(dataLayerName);

                LayerData[] toReturn = new LayerData[entry.Value.Count];
                for (int i = 0; i < dataLayer.Count; i++)
                    toReturn[i] = new LayerData(dataLayer.GetSrcRect(i), dataLayer.GetOffset(i));

                layerDatas.Add(entry.Key, toReturn);
            }

            //Convert those rectangles into color matrices
            Dictionary<string, List<Color[,]>> colorMatrices = new Dictionary<string, List<Color[,]>>();
            Color[,] fullTextureMatrix = DrawUtils.TextureTo2DArray(spriteSheet);
            foreach (KeyValuePair<string, LayerData[]> entry in layerDatas)
            {
                List<Color[,]> curAnimationMatrices = new List<Color[,]>();
                foreach (var frame in entry.Value)
                {
                    Color[,] relevantSection = fullTextureMatrix.CopySection(frame.Src);
                    curAnimationMatrices.Add(relevantSection);
                }
                colorMatrices.Add(entry.Key, curAnimationMatrices);
            }

            return colorMatrices;
        }

        //To be run by every frame by entity
        public void Update(GameTime gt)
        {
            curAnimationTimer += gt.ElapsedGameTime;
            if (isBeginningAnimation)
            {
                isBeginningAnimation = false;
                curAnimationTimer = TimeSpan.Zero;
            }

            animationOver = false;

            //If animation isn't set, still try to run
            if (animation == null)
            {
                if (animations.ContainsKey("neutral"))
                    animation = animations["neutral"];

                else
                    animation = animations.FirstOrDefault().Value;

                AnimationIndex = 0;
            }

            AnimationIndex = animation.GetAnimationIndex(curAnimationTimer.TotalMilliseconds);

            if (curAnimationTimer.TotalMilliseconds >= animation.TotalDuration)
            {
                if (animation.IsLooping)
                {
                    //Reset animation to beginning
                    AnimationIndex = 0;
                    curAnimationTimer = new TimeSpan(0);
                }
                else
                {
                    //Freeze animation on last frame
                    AnimationIndex = animation.Count - 1;
                    animationOver = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos) { Draw(spriteBatch, pos.X, pos.Y); }
        public void Draw(SpriteBatch spriteBatch, float x, float y)
        {
            animation ??= animations.FirstOrDefault().Value;

            for (int i = 0; i < animation.LayersCount; i++)
            {
                AnimationLayer layer = animation.GetLayer(i);

                if (!layer.IsVisible)
                    continue;

                Rectangle srcRect = layer[AnimationIndex];

                Vector2 offset = layer.GetOffset(AnimationIndex);
                if (spriteEffects == SpriteEffects.FlipHorizontally)
                    offset = new Vector2(animation.SrcSize.X - (srcRect.Width + offset.X), offset.Y);

                Vector2 screenPos = new Vector2(x, y) + offset * scale;

                spriteBatch.Draw(spriteSheet, //Texture
                    screenPos * gameScale, //Position
                    srcRect, //Source Rectangle
                    color * opacity, // Color Tint
                    angle, //Rotation Angle
                    origin, //Origin Of Sprite (where to rotate around)
                    scale * gameScale * cameraZoom, //Scale
                    spriteEffects, //Sprite Effects
                    this.layer - 0.02f * i); //Layer
            }

            //TODO: Add option to render data layers
        }
    }
}
