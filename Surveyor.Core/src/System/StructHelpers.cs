namespace Surveyor.System;

/// <summary>
/// Methods to help with structs.
/// </summary>
public static class StructHelpers
{
    /// <summary>
    /// Get the first value in the sequence or <see langword="null"/> if the sequence is empty.
    /// </summary>
    /// <param name="this">The enumerable.</param>
    /// <typeparam name="T">A struct type.</typeparam>
    /// <returns>
    /// The first value in the sequence or <see langword="null"/> if the sequence is empty.
    /// </returns>
    public static T? FirstOrNull<T>(this IEnumerable<T> @this) where T : struct
    {
        return @this
            .Cast<T?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Get the first value in the sequence or <see langword="null"/> if the sequence is empty.
    /// </summary>
    /// <param name="this">The enumerable.</param>
    /// <param name="predicate">A predicate function to filter.</param>
    /// <typeparam name="T">A struct type.</typeparam>
    /// <returns>
    /// The first value in the sequence or <see langword="null"/> if the sequence is empty.
    /// </returns>
    public static T? FirstOrNull<T>(this IEnumerable<T> @this, Func<T, bool> predicate) where T : struct
    {
        return @this
            .Where(predicate)
            .Cast<T?>()
            .FirstOrDefault();
    }
}
