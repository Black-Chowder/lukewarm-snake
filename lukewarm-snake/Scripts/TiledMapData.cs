using SharpDX.Direct3D9;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackMagic
{
    public static class TiledFlags
    {
        public const uint 
            Diagonal = 0x20000000,
            Horizontal = 0x80000000,
            Vertical = 0x40000000,
            Left = Diagonal + Vertical,
            Right = Horizontal + Diagonal,
            Top = Horizontal + Vertical;
    }

    public class TiledMapData
    {
        public string backgroundcolor { get; set; }
        //class
        public int compressionlevel { get; set; }
        public int height { get; set; }
        public int hexsidelength { get; set; }
        public bool infinite { get; set; }
        public LayerData[] layers { get; set; }
        public int nextlayerid { get; set; }
        public int nextobjectid { get; set; }
        public string orientation { get; set; }
        public double parallaxoriginx { get; set; }
        public double parallaxoriginy { get; set; }
        public PropertyData[] properties { get; set; }
        public string renderorder { get; set; }
        public string staggeraxis { get; set; }
        public string staggerindex { get; set; }
        public string tiledversion { get; set; }
        public int tileheight { get; set; }
        public TilesetData[] tilesets { get; set; }
        public int tilewidth { get; set; }
        public string type { get; set; }
        public string version { get; set; }
        public int width { get; set; }
    }

    public class LayerData
    {
        public ChunkData[] chunks { get; set; }
        //class
        public string compression { get; set; }
        public uint[] data { get; set; }
        public string draworder { get; set; }
        public string encoding { get; set; }
        public int height { get; set; }
        public int id { get; set; }
        public string image { get; set; }
        public LayerData[] layers { get; set; }
        public bool locked { get; set; }
        public string name { get; set; }
        public ObjectData[] objects { get; set; }
        public double offsetx { get; set; }
        public double offsety { get; set; }
        public double opacity { get; set; }
        public double parallaxx { get; set; }
        public double parallaxy { get; set; }
        public PropertyData[] properties { get; set; }
        public bool repeatx { get; set; }
        public bool repeaty { get; set; }
        public int startx { get; set; }
        public int starty { get; set; }
        public string tintcolor { get; set; }
        public string transparentcolor { get; set; }
        public string type { get; set; }
        public bool visible { get; set; }
        public int width { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class ChunkData
    {
        public uint[] data { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class ObjectData
    {
        public string type { get; set; }
        public bool ellipse { get; set; }
        public int gid { get; set; }
        public double height { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public bool point { get; set; }
        public PointData[] polygon { get; set; }
        public PointData[] polyline { get; set; }
        public PropertyData[] properties { get; set; }
        public double rotation { get; set; }
        public string template { get; set; }
        public TextData text { get; set; }
        public bool visible { get; set; }
        public double width { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }

    public class TextData
    {
        public bool bold { get; set; }
        public string color { get; set; }
        public string fontfamily { get; set; }
        public string halign { get; set; }
        public bool italic { get; set; }
        public bool kerning { get; set; }
        public int pixelsize { get; set; }
        public bool strikeout { get; set; }
        public string text { get; set; }
        public bool underline { get; set; }
        public string valign { get; set; }
        public bool wrap { get; set; }
    }

    public class TilesetData
    {
        public string backgroundcolor { get; set; }
        //class
        public int columns { get; set; }
        public string fillmode { get; set; }
        public int firstgid { get; set; }
        public GridData[] grid { get; set; }
        public string image { get; set; }
        public int imageheight { get; set; }
        public int imagewidth { get; set; }
        public int margin { get; set; }
        public string name { get; set; }
        public string objectalignment { get; set; }
        public PropertyData[] properties { get; set; }
        public string source { get; set; }
        public int spacing { get; set; }
        public TerrainData[] terrains { get; set; }
        public int tilecount { get; set; }
        public string tiledversion { get; set; }
        public int tileheight { get; set; }
        public TileOffsetData tileoffset { get; set; }
        public string tilerendersize { get; set; }
        public TileData[] tiles { get; set; }
        public int tilewidth { get; set; }
        public TransformationData[] transformations { get; set; }
        public string transparentcolor { get; set; }
        public string type { get; set; }
        public string version { get; set; }
        public WangSetData[] wangsets { get; set; }
    }

    public class GridData
    {
        public int height { get; set; }
        public string orientation { get; set; }
        public int width { get; set; }
    }

    public class TileOffsetData
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class TransformationData
    {
        public bool hflip { get; set; }
        public bool vflip { get; set; }
        public bool rotate { get; set; }
        public bool preferuntransformed { get; set; }
    }

    public class TileData
    {
        public FrameData[] animation { get; set; }
        //class
        public int id { get; set; }
        public string image { get; set; }
        public int imageheight { get; set; }
        public int imagewidth { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public LayerData[] objectgroup { get; set; }
        public double probability { get; set; }
        public PropertyData[] properties { get; set; }
        public TerrainData[] terrain { get; set; }
    }

    public class FrameData
    {
        public int duration { get; set; }
        public int tiledid { get; set; }
    }

    public class TerrainData
    {
        public string name { get; set; }
        public PropertyData[] properties { get; set; }
        public int tile { get; set; }
    }

    public class WangSetData
    {
        //class
        public WangColorData[] colors { get; set; }
        public string name { get; set; }
        public PropertyData[] properties { get; set; }
        public int title { get; set; }
        public string type { get; set; }
        public WangTileData[] wangtiles { get; set; }
    }

    public class WangColorData
    {
        //color
        public string color { get; set; }
        public string name { get; set; }
        public double probability { get; set; }
        public PropertyData[] properties { get; set; }
        public int tile { get; set; }
    }

    public class WangTileData
    {
        public int tileid { get; set; }
        public char[] wangid;
    }

    public class ObjectTemplateData
    {
        string type { get; set; }
        public TilesetData tileset { get; set; }
        //object
    }
    
    public class PropertyData
    {
        public string name { get; set; }
        public string type { get; set; }
        public string propertytype { get; set; }
        public string value { get; set; } //supposed to be of type "value"
    }

    public class PointData
    {
        public double x { get; set; }
        public double y { get; set; }
    }


}
