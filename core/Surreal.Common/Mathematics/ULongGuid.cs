
namespace Surreal.Mathematics;

/// <summary>
/// A 64-bit unsigned integer that represents a globally unique identifier (GUID).
/// <para/>
/// This implementation uses the Snowfall algorithm (see https://github.com/lowjiajin/snowfall).
/// <para/>
/// Snowfall returns unique GUIDs for as long as:
/// 
/// <list type="bullet">
///   <item>The generator id is within [0, 4096)</item>
///   <item>No more than 2048 GUIDs are generated within one ms per generator id.</item>
///   <item>The lifetime of the system is no more than 2^41ms (~70 years) from the epoch time set.</item>
/// </list>
/// </summary>
public readonly record struct ULongGuid(ulong Raw)
{
    // 41 bits for the timestamp
    // 11 bits for looping counter
    // 12 bits for generator id

    private const int BitsForTimestamp = 41;
    private const int BitsForCounter = 11;
    private const int BitsForGeneratorId = 12;

    private const int ShiftForCounter = BitsForGeneratorId;
    private const int ShiftForTimestamp = BitsForCounter + BitsForGeneratorId;

    public static ULongGuid NewGuid()
    {
        throw new NotImplementedException();
    }
}