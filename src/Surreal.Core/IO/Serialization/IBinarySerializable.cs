using System.IO;

namespace Surreal.IO.Serialization {
  public interface IBinarySerializable {
    void Save(BinaryWriter writer);
    void Load(BinaryReader reader);
  }
}