using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Surreal.Framework.Tiles.Importers {
  [XmlRoot("map")]
  public sealed class TmxDocument {
    public static TmxDocument Load(Stream stream) {
      using var reader     = new XmlTextReader(stream);
      var       serializer = new XmlSerializer(typeof(TmxDocument));

      return (TmxDocument) serializer.Deserialize(reader);
    }

    [XmlAttribute("version")]    public string Version    { get; set; } = "1.0";
    [XmlAttribute("width")]      public int    Width      { get; set; }
    [XmlAttribute("height")]     public int    Height     { get; set; }
    [XmlAttribute("tilewidth")]  public int    TileWidth  { get; set; }
    [XmlAttribute("tileheight")] public int    TileHeight { get; set; }

    [XmlElement("tileset", typeof(TsxDocument))]
    public List<TsxDocument> TileSets { get; set; } = new List<TsxDocument>();

    [XmlElement("group", typeof(GroupElement))]
    public List<GroupElement> Groups { get; set; } = new List<GroupElement>();

    [XmlElement("layer", typeof(LayerElement))]
    public List<LayerElement> Layers { get; set; } = new List<LayerElement>();

    [XmlElement("objectgroup", typeof(ObjectGroupElement))]
    public List<ObjectGroupElement> ObjectGroups { get; set; } = new List<ObjectGroupElement>();

    [XmlArray("properties")]
    [XmlArrayItem("property", typeof(PropertyElement))]
    public List<PropertyElement> Properties { get; set; } = new List<PropertyElement>();

    public PropertyElement? this[string key] => Properties.FirstOrDefault(_ => string.Equals(_.Name, key, StringComparison.OrdinalIgnoreCase));

    public enum EncodingType {
      [XmlEnum("xml")]    Xml    = 0,
      [XmlEnum("base64")] Base64 = 1,
      [XmlEnum("csv")]    Csv    = 2
    }

    public enum CompressionType {
      None                   = 0,
      [XmlEnum("gzip")] GZip = 1,
      [XmlEnum("zlib")] ZLib = 2
    }

    public sealed class GroupElement {
      [XmlAttribute("name")] public string? Name { get; set; }

      [XmlElement("layer", typeof(LayerElement))]
      public List<LayerElement> Layers { get; set; } = new List<LayerElement>();

      [XmlElement("objectgroup", typeof(ObjectGroupElement))]
      public List<ObjectGroupElement> ObjectGroups { get; set; } = new List<ObjectGroupElement>();
    }

    public sealed class ObjectGroupElement {
      [XmlAttribute("name")] public string? Name { get; set; }

      [XmlElement("object", typeof(ObjectElement))]
      public List<ObjectElement> Objects { get; set; } = new List<ObjectElement>();
    }

    public sealed class ObjectElement {
      [XmlAttribute("name")] public string? Name { get; set; }
      [XmlAttribute("type")] public string? Type { get; set; }
      [XmlAttribute("x")]    public float   X    { get; set; }
      [XmlAttribute("y")]    public float   Y    { get; set; }
    }

    public sealed class LayerElement {
      [XmlAttribute("name")]    public string? Name      { get; set; }
      [XmlAttribute("x")]       public int     X         { get; set; }
      [XmlAttribute("y")]       public int     Y         { get; set; }
      [XmlAttribute("width")]   public int     Width     { get; set; }
      [XmlAttribute("height")]  public int     Height    { get; set; }
      [XmlAttribute("visible")] public int     IsVisible { get; set; } = 1;

      [XmlElement("data", typeof(DataElement))]
      public DataElement? Data { get; set; }
    }

    public class ChunkElement {
      [XmlAttribute("x")]      public int     X      { get; set; }
      [XmlAttribute("y")]      public int     Y      { get; set; }
      [XmlAttribute("width")]  public int     Width  { get; set; }
      [XmlAttribute("height")] public int     Height { get; set; }
      [XmlText]                public string? Data   { get; set; }

      [XmlElement("chunk", typeof(ChunkElement))]
      public List<ChunkElement> Chunks { get; set; } = new List<ChunkElement>();

      public IEnumerable<uint> Decode(EncodingType encoding, CompressionType compression) {
        if (!string.IsNullOrEmpty(Data)) {
          switch (encoding) {
            case EncodingType.Xml:    return DecodeXml();
            case EncodingType.Csv:    return DecodeCsv();
            case EncodingType.Base64: return DecodeBase64(compression);
          }

          throw new ArgumentOutOfRangeException(nameof(encoding));
        }

        return Enumerable.Empty<uint>();
      }

      private IEnumerable<uint> DecodeXml() {
        foreach (var element in XElement.Parse(Data).Elements("tile")) {
          yield return (uint) element.Attribute("gid");
        }
      }

      private IEnumerable<uint> DecodeCsv() {
        var reader   = new StringReader(Data);
        var contents = reader.ReadToEnd();

        foreach (var raw in contents.Split(',')) {
          yield return Convert.ToUInt32(raw);
        }
      }

      private IEnumerable<uint> DecodeBase64(CompressionType compression) {
        switch (compression) {
          case CompressionType.None: throw new NotImplementedException();
          case CompressionType.GZip: throw new NotImplementedException();
          case CompressionType.ZLib: throw new NotImplementedException();
        }

        throw new ArgumentOutOfRangeException(nameof(compression));
      }
    }

    public sealed class DataElement : ChunkElement {
      [XmlAttribute("encoding")]    public EncodingType    Encoding    { get; set; }
      [XmlAttribute("compression")] public CompressionType Compression { get; set; }

      public IEnumerable<uint> Decode() => Decode(Encoding, Compression);
    }

    public enum PropertyType {
      [XmlEnum("string")] String,
      [XmlEnum("int")]    Int,
      [XmlEnum("float")]  Float,
      [XmlEnum("bool")]   Bool,
      [XmlEnum("color")]  Color,
      [XmlEnum("file")]   File
    }

    [DebuggerDisplay("{Name}: {Value} ({Type})")]
    public sealed class PropertyElement {
      [XmlAttribute("name")]  public string?      Name  { get; set; }
      [XmlAttribute("value")] public string?      Value { get; set; }
      [XmlAttribute("type")]  public PropertyType Type  { get; set; } = PropertyType.String;
    }
  }
}