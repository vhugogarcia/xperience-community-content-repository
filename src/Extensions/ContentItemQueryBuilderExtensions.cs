namespace XperienceCommunity.ContentRepository.Extensions;

public static class ContentItemQueryBuilderExtensions
{

    /// <summary>
    /// Configures the <see cref="ContentItemQueryBuilder"/> for the specified content type.
    /// </summary>
    /// <typeparam name="T">The type that implements <see cref="IContentItemFieldsSource"/>.</typeparam>
    /// <param name="source">The <see cref="ContentItemQueryBuilder"/> to configure.</param>
    /// <param name="configureQuery">An optional action to configure the <see cref="ContentTypeQueryParameters"/>.</param>
    /// <returns>The configured <see cref="ContentItemQueryBuilder"/>.</returns>
    public static ContentItemQueryBuilder ForContentType<T>(this ContentItemQueryBuilder source,
        Action<ContentTypeQueryParameters>? configureQuery = null) where T : IContentItemFieldsSource
    {
        string contentType = typeof(T).GetContentTypeName() ?? string.Empty;

        ArgumentException.ThrowIfNullOrEmpty(contentType);

        return source.ForContentType(contentType, configureQuery);
    }

    /// <summary>
    /// Conditionally applies an action to the <see cref="ContentItemQueryBuilder"/> if the specified condition is true.
    /// </summary>
    /// <param name="source">The <see cref="ContentItemQueryBuilder"/> to apply the action to.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="action">The action to apply if the condition is true.</param>
    /// <returns>The original <see cref="ContentItemQueryBuilder"/> with the action applied if the condition is true; otherwise, the original <see cref="ContentItemQueryBuilder"/>.</returns>
    public static ContentItemQueryBuilder When(this ContentItemQueryBuilder source, bool condition, Action<ContentItemQueryBuilder> action)
    {
        if (condition)
        {
            action(source);
            return source;
        }

        return source;
    }

    /// <summary>
    /// Configures the <see cref="ContentItemQueryBuilder"/> to filter by the specified language.
    /// </summary>
    /// <param name="source">The <see cref="ContentItemQueryBuilder"/> to apply the action to.</param>
    /// <param name="language">The language name.</param>
    /// <returns>The original <see cref="ContentItemQueryBuilder"/> with the action applied.</returns>
    public static ContentItemQueryBuilder WithLanguage(this ContentItemQueryBuilder source, string? language)
    {
        source.When(!string.IsNullOrEmpty(language), q => q.InLanguage(language));

        return source;
    }
}
