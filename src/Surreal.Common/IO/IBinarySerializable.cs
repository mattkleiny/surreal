using System.IO;

namespace Surreal.IO
{
  public interface IBinarySerializable
  {
    void Save(BinaryWriter writer);
    void Load(BinaryReader reader);
  }
}