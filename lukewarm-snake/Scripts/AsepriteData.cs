using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class AsepriteData
    {
        public AseFrameData[] frames { get; set; }
        public AseMeta meta { get; set; }
    }

    public class AseFrameData
    {
        public string filename { get; set; }
        public AseRect frame { get; set; }
        public bool rotated { get; set; }
        public bool trimmed { get; set; }
        public AseRect spriteSourceSize { get; set; }
        public AseSize sourceSize { get; set; }
        public int duration { get; set; }
    }

    public class AseRect
    {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }

    public class AseSize
    {
        public int w { get; set; }
        public int h { get; set; }
    }

    public class AseMeta
    {
        public string app { get; set; }
        public string version { get; set; }
        public string image { get; set; }
        public string format { get; set; }
        public AseSize size { get; set; }
        public string scale { get; set; }
        public TagData[] frameTags { get; set; }
        public AseLayerData[] layers { get; set; }
        //slices...
    }

    public class TagData
    {
        public string name { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string direction { get; set; }
    }

    public class AseLayerData
    {
        public string name { get; set; }
        public int opacity { get; set; }
        public string blendMode { get; set; }
    }
}
