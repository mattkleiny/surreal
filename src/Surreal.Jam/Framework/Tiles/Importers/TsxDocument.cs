using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Surreal.Framework.Tiles.Importers {
  [XmlRoot("tileset")]
  public sealed class TsxDocument {
    public static TsxDocument Load(Stream stream) {
      using var reader     = new XmlTextReader(stream);
      var       serializer = new XmlSerializer(typeof(TsxDocument));

      return (TsxDocument) serializer.Deserialize(reader);
    }

    [XmlAttribute("version")]    public string  Version    { get; set; } = "1.0";
    [XmlAttribute("name")]       public string? Name       { get; set; }
    [XmlAttribute("source")]     public string? Source     { get; set; }
    [XmlAttribute("firstgid")]   public int     FirstGID   { get; set; }
    [XmlAttribute("tilewidth")]  public int     TileWidth  { get; set; }
    [XmlAttribute("tileheight")] public int     TileHeight { get; set; }
    [XmlAttribute("tilecount")]  public int     TileCount  { get; set; }
    [XmlAttribute("columns")]    public int     Columns    { get; set; }
    [XmlAttribute("spacing")]    public int     Spacing    { get; set; }
    [XmlAttribute("margin")]     public int     Margin     { get; set; }

    [XmlElement("image", typeof(ImageElement))]
    public List<ImageElement> Images { get; set; } = new List<ImageElement>();

    [XmlElement("tile", typeof(TileElement))]
    public List<TileElement> Tiles { get; set; } = new List<TileElement>();

    public class ImageElement {
      [XmlAttribute("source")] public string? Source { get; set; }
      [XmlAttribute("width")]  public int     Width  { get; set; }
      [XmlAttribute("height")] public int     Height { get; set; }
    }

    public class TileElement {
      [XmlAttribute("id")]   public int     Id   { get; set; }
      [XmlAttribute("type")] public string? Type { get; set; }

      [XmlElement("animation", typeof(AnimationElement))]
      public AnimationElement? Animation { get; set; }
    }

    public class AnimationElement {
      [XmlElement("frame", typeof(FrameElement))]
      public List<FrameElement> Frames { get; set; } = new List<FrameElement>();
    }

    public class FrameElement {
      [XmlAttribute("tileid")]   public int   Id       { get; set; }
      [XmlAttribute("duration")] public float Duration { get; set; }
    }
  }
}