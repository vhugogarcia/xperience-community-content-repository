#pragma warning disable IDE0060 // Remove unused parameter

namespace XperienceCommunity.ContentRepository.Extensions;

/// <summary>
/// Provides extension methods for working with types.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// The name of the static string field that contains the content type name.
    /// </summary>
    private const string ContentTypeFieldName = "CONTENT_TYPE_NAME";

    /// <summary>
    /// The name of the static string field that contains the reusable field schema name.
    /// </summary>
    private const string ReusableFieldSchemaName = "REUSABLE_FIELD_SCHEMA_NAME";

    /// <summary>
    /// Thread safe dictionary for faster content type name lookup.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> sClassNames = new();

    /// <summary>
    /// Thread safe dictionary for faster schema name lookup.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> sSchemaNames = new();

    /// <summary>
    /// Determines whether the specified type inherits from <see cref="IWebPageFieldsSource"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool InheritsFromIWebPageFieldsSource(this Type type) => typeof(IWebPageFieldsSource).IsAssignableFrom(type);

    /// <summary>
    /// Determines whether the specified type inherits from <see cref="IContentItemFieldsSource"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool InheritsFromIContentItemFieldsSource(this Type type) => typeof(IContentItemFieldsSource).IsAssignableFrom(type);

    /// <summary>
    /// Gets the value of a static string field from a given type.
    /// </summary>
    /// <param name="type">The type to get the static string field from.</param>
    /// <param name="fieldName">The name of the static string field.</param>
    /// <returns>The value of the static string field if found; otherwise, an empty string.</returns>
    public static string? GetStaticString(this Type type, string fieldName)
    {
        var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        if (field == null || field.FieldType != typeof(string))
        {
            return null;
        }

        return field.GetValue(null) as string ?? null;
    }

    /// <summary>
    /// Gets the reusable field schema name for a given value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to get the reusable field schema name from.</param>
    /// <returns>The reusable field schema name if found; otherwise, null.</returns>
    public static string? GetReusableFieldSchemaName<T>(this T value)
    {
        var type = typeof(T);

        if (!type.IsInterface)
        {
            return null;
        }

        string? interfaceName = type?.Name ?? type?.GetInterfaces().FirstOrDefault()?.Name;

        if (string.IsNullOrEmpty(interfaceName))
        {
            return null;
        }

        if (sSchemaNames.TryGetValue(interfaceName, out string? schemaName))
        {
            return schemaName;
        }

        schemaName = type?.GetStaticString(ReusableFieldSchemaName);

        if (string.IsNullOrEmpty(schemaName))
        {
            return null;
        }

        sSchemaNames.TryAdd(interfaceName, schemaName);

        return schemaName;
    }

    /// <summary>
    /// Finds the specified property of type <see cref="IEnumerable{AssetRelatedItem}"/> or <see cref="AssetRelatedItem"/>
    /// and returns all GUIDs from the <see cref="AssetRelatedItem.Identifier"/> property.
    /// </summary>
    /// <typeparam name="T">The type that implements <see cref="IContentItemFieldsSource"/>.</typeparam>
    /// <param name="source">The source object to search for properties.</param>
    /// <param name="propertyExpression">The expression specifying the property to search.</param>
    /// <returns>A collection of GUIDs from the <see cref="AssetRelatedItem.Identifier"/> property.</returns>
    public static IEnumerable<Guid> GetRelatedAssetItemGuids<T>(this T source, Expression<Func<T, object>> propertyExpression) where T : IContentItemFieldsSource
    {
        var guids = new List<Guid>();

        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as PropertyInfo;

            if (property == null)
            {
                return [];
            }

            if (property.PropertyType == typeof(AssetRelatedItem))
            {
                if (property.GetValue(source) is AssetRelatedItem item)
                {
                    guids.Add(item.Identifier);
                }
            }
            else if (typeof(IEnumerable<AssetRelatedItem>).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(source) is IEnumerable<AssetRelatedItem> items)
                {
                    guids.AddRange(items.Select(item => item.Identifier));
                }
            }
        }
        else if (propertyExpression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
        {
            var property = unaryMemberExpression.Member as PropertyInfo;

            if (property == null)
            {
                return guids;
            }

            if (property.PropertyType == typeof(AssetRelatedItem))
            {
                if (property.GetValue(source) is AssetRelatedItem item)
                {
                    if (item.Identifier != Guid.Empty)
                    {
                        guids.Add(item.Identifier);
                    }
                }
            }
            else if (typeof(IEnumerable<AssetRelatedItem>).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(source) is IEnumerable<AssetRelatedItem> items)
                {
                    guids.AddRange(items.Where(item => item.Identifier != Guid.Empty).Select(item => item.Identifier));
                }
            }
        }

        return guids;
    }

    /// <summary>
    /// Finds all properties of type <see cref="IEnumerable{AssetRelatedItem}"/> or <see cref="AssetRelatedItem"/>
    /// and returns all GUIDs from the <see cref="AssetRelatedItem.Identifier"/> property.
    /// </summary>
    /// <typeparam name="T">The type that implements <see cref="IContentItemFieldsSource"/>.</typeparam>
    /// <param name="source">The source object to search for properties.</param>
    /// <returns>A collection of GUIDs from the <see cref="AssetRelatedItem.Identifier"/> property.</returns>
    public static IEnumerable<Guid> GetRelatedAssetItemGuids<T>(this T source) where T : IContentItemFieldsSource
    {
        var guids = new List<Guid>();

        var properties = typeof(T)?.GetProperties(BindingFlags.Public | BindingFlags.Instance) ?? [];

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(AssetRelatedItem))
            {
                if (property.GetValue(source) is AssetRelatedItem item)
                {
                    if (item.Identifier != Guid.Empty)
                    {
                        guids.Add(item.Identifier);
                    }
                }
            }
            else if (typeof(IEnumerable<AssetRelatedItem>).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(source) is IEnumerable<AssetRelatedItem> items)
                {
                    guids.AddRange(items.Where(item => item.Identifier != Guid.Empty).Select(item => item.Identifier));
                }
            }
        }

        return guids;
    }

    /// <summary>
    /// Finds the specified property of type <see cref="IEnumerable{WebPageRelatedItem}"/> or <see cref="WebPageRelatedItem"/>
    /// and returns all GUIDs from the <see cref="WebPageRelatedItem.WebPageGuid"/> property.
    /// </summary>
    /// <typeparam name="T">The type that implements <see cref="IContentItemFieldsSource"/>.</typeparam>
    /// <param name="source">The source object to search for properties.</param>
    /// <param name="propertyExpression">The expression specifying the property to search.</param>
    /// <returns>A collection of GUIDs from the <see cref="WebPageRelatedItem.WebPageGuid"/> property.</returns>
    public static IEnumerable<Guid> GetRelatedWebPageGuids<T>(this T source, Expression<Func<T, object>> propertyExpression) where T : IContentItemFieldsSource
    {
        var guids = new List<Guid>();

        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as PropertyInfo;

            if (property == null)
            {
                return [];
            }

            if (property.PropertyType == typeof(WebPageRelatedItem))
            {
                if (property.GetValue(source) is WebPageRelatedItem item)
                {
                    if (item.WebPageGuid != Guid.Empty)
                    {
                        guids.Add(item.WebPageGuid);
                    }
                }
            }
            else if (typeof(IEnumerable<WebPageRelatedItem>).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(source) is IEnumerable<WebPageRelatedItem> items)
                {
                    guids.AddRange(items.Where(item => item.WebPageGuid != Guid.Empty).Select(item => item.WebPageGuid));
                }
            }
        }
        else if (propertyExpression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
        {
            var property = unaryMemberExpression.Member as PropertyInfo;

            if (property == null)
            {
                return guids;
            }

            if (property.PropertyType == typeof(WebPageRelatedItem))
            {
                if (property.GetValue(source) is WebPageRelatedItem item)
                {
                    if (item.WebPageGuid != Guid.Empty)
                    {
                        guids.Add(item.WebPageGuid);
                    }
                }
            }
            else if (typeof(IEnumerable<WebPageRelatedItem>).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(source) is IEnumerable<WebPageRelatedItem> items)
                {
                    guids.AddRange(items.Where(item => item.WebPageGuid != Guid.Empty).Select(item => item.WebPageGuid));
                }
            }
        }

        return guids;
    }

    /// <summary>
    /// Finds all properties of type <see cref="IEnumerable{WebPageRelatedItem}"/> or <see cref="WebPageRelatedItem"/>
    /// and returns all GUIDs from the <see cref="WebPageRelatedItem.WebPageGuid"/> property.
    /// </summary>
    /// <typeparam name="T">The type that implements <see cref="IContentItemFieldsSource"/>.</typeparam>
    /// <param name="source">The source object to search for properties.</param>
    /// <returns>A collection of GUIDs from the <see cref="WebPageRelatedItem.WebPageGuid"/> property.</returns>
    public static IEnumerable<Guid> GetRelatedWebPageGuids<T>(this T source) where T : IContentItemFieldsSource
    {
        var guids = new List<Guid>();

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(WebPageRelatedItem))
            {
                if (property.GetValue(source) is WebPageRelatedItem item)
                {
                    if (item.WebPageGuid != Guid.Empty)
                    {
                        guids.Add(item.WebPageGuid);
                    }
                }
            }
            else if (typeof(IEnumerable<WebPageRelatedItem>).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(source) is IEnumerable<WebPageRelatedItem> items)
                {
                    guids.AddRange(items.Where(item => item.WebPageGuid != Guid.Empty).Select(item => item.WebPageGuid));
                }
            }
        }
        return guids;
    }


    /// <summary>
    /// Gets the content type name for a given type.
    /// </summary>
    /// <returns>The content type name.</returns>
    public static string? GetContentTypeName(this Type? type)
    {
        if (type is null)
        {
            return null;
        }

        if (!type.InheritsFromIWebPageFieldsSource() && !type.InheritsFromIContentItemFieldsSource())
        {
            return null;
        }

        if (string.IsNullOrEmpty(type.FullName))
        {
            return null;
        }

        if (sClassNames.TryGetValue(type.FullName, out string? contentTypeName))
        {
            return contentTypeName;
        }

        contentTypeName = type.GetStaticString(ContentTypeFieldName);

        if (string.IsNullOrEmpty(contentTypeName))
        {
            return null;
        }

        sClassNames.TryAdd(type.FullName, contentTypeName);

        return contentTypeName;
    }
}
