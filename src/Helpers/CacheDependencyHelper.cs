namespace XperienceCommunity.ContentRepository.Helpers;

/// <summary>
/// Provides helper methods for creating cache dependencies for content and web page items.
/// </summary>
public static class CacheDependencyHelper
{
    /// <summary>
    /// Creates cache keys for web page items by their GUIDs.
    /// </summary>
    /// <param name="itemGuids">The GUIDs of the web page items.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateContentItemGUIDKeys(IEnumerable<Guid>? itemGuids) =>
        itemGuids?.Select(x => CreateContentItemGUIDKey(x))?.ToArray() ?? [];

    /// <summary>
    /// Creates cache keys for web page items by their GUIDs.
    /// </summary>
    /// <param name="itemGuids">The GUIDs of the web page items.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateWebPageItemGUIDKeys(IEnumerable<Guid>? itemGuids) =>
        itemGuids?.Select(x => CreateWebPageItemGUIDKey(x))?.ToArray() ?? [];

    /// <summary>
    /// Creates a cache key for a web page item by its GUID.
    /// </summary>
    /// <param name="itemGuid">The GUID of the web page item.</param>
    /// <returns>A cache key.</returns>
    public static string CreateWebPageItemGUIDKey(Guid? itemGuid) => $"webpageitem|byguid|{itemGuid}";

    /// <summary>
    /// Creates a cache key for a web page item by its ID.
    /// </summary>
    /// <param name="itemId">The ID of the web page item.</param>
    /// <returns>A cache key.</returns>
    public static string CreateWebPageItemIdKey(int? itemId) => $"webpageitem|byid|{itemId}";

    /// <summary>
    /// Creates a cache key for a content item by its GUID.
    /// </summary>
    /// <param name="itemGuid">The GUID of the web page item.</param>
    /// <returns>A cache key.</returns>
    public static string CreateContentItemGUIDKey(Guid? itemGuid) => $"contentitem|byguid|{itemGuid}";

    /// <summary>
    /// Creates a cache key for a content item by its ID.
    /// </summary>
    /// <param name="itemId">The ID of the web page item.</param>
    /// <returns>A cache key.</returns>
    public static string CreateContentItemIdKey(int? itemId) => $"contentitem|byid|{itemId}";

    /// <summary>
    /// Creates cache keys for content items by their IDs.
    /// </summary>
    /// <param name="itemIdList">The IDs of the web page items.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateContentItemIdKeys(IEnumerable<int>? itemIdList) =>
        itemIdList?.Select(x => CreateContentItemIdKey(x))?.ToArray() ?? [];

    /// <summary>
    /// Creates cache keys for web page items by their IDs.
    /// </summary>
    /// <param name="itemIdList">The IDs of the web page items.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateWebPageItemIdKeys(IEnumerable<int>? itemIdList) =>
        itemIdList?.Select(x => CreateWebPageItemIdKey(x))?.ToArray() ?? [];

    /// <summary>
    /// Creates cache keys for web page items by content type and channel name.
    /// </summary>
    /// <param name="contentTypes">The content types.</param>
    /// <param name="channelName">The channel name.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateWebPageItemTypeKeys(IEnumerable<string>? contentTypes, string channelName) =>
        contentTypes?.Select(x => $"webpageitem|bychannel|{channelName}|bycontenttype|{x}")?.ToArray() ?? [];

    /// <summary>
    /// Creates cache keys for content items by content type.
    /// </summary>
    /// <param name="contentTypes">The content types.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateContentItemTypeKeys(IEnumerable<string>? contentTypes) =>
        contentTypes?.Select(x => $"contentitem|bycontenttype|{x}")?.ToArray() ?? [];

    /// <summary>
    /// Creates cache keys for content items by their GUIDs.
    /// </summary>
    /// <typeparam name="T">The type of the content items.</typeparam>
    /// <param name="items">The content items.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateContentItemKeys<T>(IEnumerable<T>? items) where T : IContentItemFieldsSource =>
        items?.Select(x => $"contentitem|byid|{x.SystemFields.ContentItemGUID}")?.ToArray() ?? [];

    /// <summary>
    /// Creates cache keys for web page items by their IDs.
    /// </summary>
    /// <typeparam name="T">The type of the web page items.</typeparam>
    /// <param name="items">The web page items.</param>
    /// <returns>An array of cache keys.</returns>
    public static string[] CreateWebPageItemKeys<T>(IEnumerable<T>? items) where T : IWebPageFieldsSource =>
        items?.Select(x => $"webpageitem|byid|{x.SystemFields.WebPageItemID}")?.ToArray() ?? [];

    /// <summary>
    /// Creates a cache dependency for the specified content items.
    /// </summary>
    /// <typeparam name="T">The type of the content items.</typeparam>
    /// <param name="items">The content items.</param>
    /// <returns>A cache dependency for the specified content items.</returns>
    public static CMSCacheDependency CreateContentItemCacheDependency<T>(IEnumerable<T>? items)
        where T : IContentItemFieldsSource
    {
        string[] contentItemKeys = CreateContentItemKeys(items);

        return CacheHelper.GetCacheDependency(contentItemKeys);
    }

    /// <summary>
    /// Creates a cache dependency for the specified web page items.
    /// </summary>
    /// <typeparam name="T">The type of the web page items.</typeparam>
    /// <param name="items">The web page items.</param>
    /// <returns>A cache dependency for the specified web page items.</returns>
    public static CMSCacheDependency CreateWebPageItemCacheDependency<T>(IEnumerable<T>? items)
        where T : IWebPageFieldsSource
    {
        string[] webPageItemKeys = CreateWebPageItemKeys(items);
        return CacheHelper.GetCacheDependency(webPageItemKeys);
    }

    /// <summary>
    /// Creates a cache dependency for web page items by content type and channel name.
    /// </summary>
    /// <param name="contentTypes">The content types.</param>
    /// <param name="channelName">The channel name.</param>
    /// <returns>A cache dependency for the specified web page items.</returns>
    public static CMSCacheDependency CreateWebPageItemTypeCacheDependency(IEnumerable<string>? contentTypes,
        string channelName)
    {
        string[] webPageItemTypeKeys = CreateWebPageItemTypeKeys(contentTypes, channelName);
        return CacheHelper.GetCacheDependency(webPageItemTypeKeys);
    }

    /// <summary>
    /// Creates a cache dependency for content items by content type.
    /// </summary>
    /// <param name="contentTypes">The content types.</param>
    /// <returns>A cache dependency for the specified content items.</returns>
    public static CMSCacheDependency CreateContentItemTypeCacheDependency(IEnumerable<string>? contentTypes)
    {
        string[] contentItemTypeKeys = CreateContentItemTypeKeys(contentTypes);
        return CacheHelper.GetCacheDependency(contentItemTypeKeys);
    }

    /// <summary>
    /// Creates a cache dependency for web page items by their GUIDs.
    /// </summary>
    /// <param name="itemGuids">The GUIDs of the web page items.</param>
    /// <returns>A cache dependency for the specified web page items.</returns>
    public static CMSCacheDependency CreateWebPageItemGUIDCacheDependency(IEnumerable<Guid>? itemGuids)
    {
        string[] webPageItemGUIDKeys = CreateWebPageItemGUIDKeys(itemGuids);
        return CacheHelper.GetCacheDependency(webPageItemGUIDKeys);
    }

    /// <summary>
    /// Creates a cache dependency for web page items by their IDs.
    /// </summary>
    /// <param name="itemIdList"></param>
    /// <returns></returns>
    public static CMSCacheDependency CreateWebPageItemIDCacheDependency(IEnumerable<int>? itemIdList)
    {
        string[] webPageItemIdKeys = CreateWebPageItemIdKeys(itemIdList);
        return CacheHelper.GetCacheDependency(webPageItemIdKeys);
    }

    /// <summary>
    /// Creates a cache dependency for content items by their GUIDs.
    /// </summary>
    /// <param name="itemGuids">The GUIDs of the web page items.</param>
    /// <returns>A cache dependency for the specified web page items.</returns>
    public static CMSCacheDependency CreateContentItemGUIDCacheDependency(IEnumerable<Guid>? itemGuids)
    {
        string[] webPageItemGUIDKeys = CreateContentItemGUIDKeys(itemGuids);
        return CacheHelper.GetCacheDependency(webPageItemGUIDKeys);
    }

    /// <summary>
    /// Creates a cache dependency for content items by their IDs.
    /// </summary>
    /// <param name="itemIdList"></param>
    /// <returns></returns>
    public static CMSCacheDependency CreateContentItemIDCacheDependency(IEnumerable<int>? itemIdList)
    {
        string[] webPageItemIdKeys = CreateContentItemIdKeys(itemIdList);
        return CacheHelper.GetCacheDependency(webPageItemIdKeys);
    }
}
