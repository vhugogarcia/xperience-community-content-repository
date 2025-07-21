namespace XperienceCommunity.ContentRepository.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IContentItemFieldsSource"/>.
/// </summary>
public static class IContentItemFieldsSourceExtensions
{
    private const string ContentItemCachePrefix = "contentitem|byid|";


    /// <summary>
    /// Determines whether the specified content item is secure.
    /// </summary>
    /// <param name="source">The content item fields source.</param>
    /// <returns><c>true</c> if the content item is secure; otherwise, <c>false</c>.</returns>
    public static bool IsSecureItem(this IContentItemFieldsSource? source) => source?.SystemFields?.ContentItemIsSecured ?? false;

    /// <summary>
    /// Determines whether the collection contains any secure content items.
    /// </summary>
    /// <param name="source">The collection of content item fields sources.</param>
    /// <returns><c>true</c> if the collection contains any secure content items; otherwise, <c>false</c>.</returns>
    public static bool HasSecureItems(this IEnumerable<IContentItemFieldsSource>? source) => source?.Any(x => x.SystemFields.ContentItemIsSecured) == true;

    /// <summary>
    /// Gets the cache dependency key for the specified content item.
    /// </summary>
    /// <param name="source">The content item fields source.</param>
    /// <returns>An array containing the cache dependency key.</returns>
    public static string[] GetCacheDependencyKey(this IContentItemFieldsSource? source) => source is null ? [] : [$"{ContentItemCachePrefix}{source.SystemFields.ContentItemID}"];

    /// <summary>
    /// Gets the cache dependency keys for the specified collection of content items.
    /// </summary>
    /// <param name="source">The collection of content item fields sources.</param>
    /// <returns>An array containing the cache dependency keys.</returns>
    public static string[] GetCacheDependencyKeys(this IEnumerable<IContentItemFieldsSource>? source) => source?.Select(x => $"{ContentItemCachePrefix}{x.SystemFields.ContentItemID}")?.ToArray() ?? [];

    /// <summary>
    /// Gets the content item IDs from the specified collection of content item fields sources.
    /// </summary>
    /// <param name="source">The collection of content item fields sources.</param>
    /// <returns>An enumerable containing the content item IDs.</returns>
    public static IEnumerable<int> GetContentItemIds(this IEnumerable<IContentItemFieldsSource>? source) => source?.Select(x => x.SystemFields.ContentItemID) ?? [];

    /// <summary>
    /// Gets the content item GUIDs from the specified collection of content item fields sources.
    /// </summary>
    /// <param name="source">The collection of content item fields sources.</param>
    /// <returns>An enumerable containing the content item GUIDs.</returns>
    public static IEnumerable<Guid> GetContentItemGUIDs(this IEnumerable<IContentItemFieldsSource>? source) => source?.Select(x => x.SystemFields.ContentItemGUID) ?? [];

    /// <summary>
    /// Gets the content types from the specified collection of content item fields sources.
    /// </summary>
    /// <param name="source">The collection of content item fields sources.</param>
    /// <returns>An enumerable containing the content types.</returns>
    public static IEnumerable<string?> GetContentTypes<T>(this IEnumerable<T>? source) where T : class, IContentItemFieldsSource => source?.Select(static x => x.GetType().GetContentTypeName()) ?? [];

    /// <summary>
    /// Converts the specified collection of content item fields sources to a typed list.
    /// </summary>
    /// <typeparam name="T">The type of content item fields source.</typeparam>
    /// <param name="source">The collection of content item fields sources.</param>
    /// <returns>A typed list of content item fields sources.</returns>
    public static List<T> ToTypedList<T>(this IEnumerable<T>? source) where T : class, IContentItemFieldsSource, new() => source?.OfType<T>()?.ToList() ?? [];
}
