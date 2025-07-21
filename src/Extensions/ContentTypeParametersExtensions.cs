namespace XperienceCommunity.ContentRepository.Extensions;

public static class ContentTypeParametersExtensions
{

    /// <summary>
    /// Orders the <see cref="ContentTypeQueryParameters"/> instance by the WebPageItemOrder field in ascending order.
    /// </summary>
    /// <param name="source">The <see cref="ContentTypeQueryParameters"/> instance.</param>
    /// <returns>The <see cref="ContentTypeQueryParameters"/> instance ordered by WebPageItemOrder.</returns>
    public static ContentTypeQueryParameters OrderByWebPageItemOrder(this ContentTypeQueryParameters source)
    {
        source
            .OrderBy(OrderByColumn.Asc(nameof(IWebPageFieldsSource.SystemFields.WebPageItemOrder)));

        return source;
    }

    /// <summary>
    /// Conditionally applies an action to the <see cref="ContentTypeQueryParameters"/> instance.
    /// </summary>
    /// <param name="source">The <see cref="ContentTypeQueryParameters"/> instance.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="action">The action to apply if the condition is true.</param>
    /// <returns>The <see cref="ContentTypeQueryParameters"/> instance.</returns>
    public static ContentTypeQueryParameters When(this ContentTypeQueryParameters source, bool condition, Action<ContentTypeQueryParameters> action)
    {
        if (condition)
        {
            action(source);
            return source;
        }

        return source;
    }

    /// <summary>
    /// Conditionally applies an action to the <see cref="ContentTypesQueryParameters"/> instance.
    /// </summary>
    /// <param name="source">The <see cref="ContentTypesQueryParameters"/> instance.</param>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="action">The action to apply if the condition is true.</param>
    /// <returns>The <see cref="ContentTypesQueryParameters"/> instance.</returns>
    public static ContentTypesQueryParameters When(this ContentTypesQueryParameters source, bool condition, Action<ContentTypesQueryParameters> action)
    {
        if (condition)
        {
            action(source);
            return source;
        }

        return source;
    }

    /// <summary>
    /// Configures the <see cref="ContentTypesQueryParameters"/> instance to include linked items and web page data.
    /// </summary>
    /// <param name="source">The <see cref="ContentTypesQueryParameters"/> instance.</param>
    /// <param name="maxLinkedItems">The maximum number of linked items to include.</param>
    /// <returns>The <see cref="ContentTypesQueryParameters"/> instance.</returns>
    public static ContentTypesQueryParameters WithLinkedItemsAndWebPageData(this ContentTypesQueryParameters source, int maxLinkedItems)
    {
        source.When(maxLinkedItems > 0, options => options.WithLinkedItems(maxLinkedItems,
            linkOptions => linkOptions.IncludeWebPageData()));

        return source;
    }

    /// <summary>
    /// Configures the <see cref="ContentTypesQueryParameters"/> instance to include linked items and web page data.
    /// </summary>
    /// <param name="source">The <see cref="ContentTypesQueryParameters"/> instance.</param>
    /// <param name="maxLinkedItems">The maximum number of linked items to include.</param>
    /// <returns>The <see cref="ContentTypesQueryParameters"/> instance.</returns>
    public static ContentTypeQueryParameters WithLinkedItemsAndWebPageData(this ContentTypeQueryParameters source, int maxLinkedItems)
    {
        source.When(maxLinkedItems > 0, options => options.WithLinkedItems(maxLinkedItems,
            linkOptions => linkOptions.IncludeWebPageData()));

        return source;
    }
}
