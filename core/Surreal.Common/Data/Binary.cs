using System.IO;

namespace Surreal.Data {
  public interface IBinarySerializable {
    void Save(BinaryWriter writer);
    void Load(BinaryReader reader);
  }
}