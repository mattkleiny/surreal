namespace Surreal.Collections;

/// <summary>General purpose collection extensions</summary>
public static class CollectionExtensions
{
	public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
	{
		var result = new Queue<T>();

		foreach (var element in enumerable)
		{
			result.Enqueue(element);
		}

		return result;
	}
}
