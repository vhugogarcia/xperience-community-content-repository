namespace XperienceCommunity.ContentRepository.Builders;

public class CacheDependencyBuilder : ICacheDependencyBuilder
{
    /// <inheritdoc />
    public CMSCacheDependency? Create<T>(IEnumerable<T> items)
    {
        var keys = ExtractCacheDependencyKeys(items);

        if (keys.Count == 0)
        {
            return null;
        }

        return new CMSCacheDependency() { CacheKeys = [.. keys] };
    }


    private static void AddDependencyKeys<T>(T value, HashSet<string> dependencyKeys)
    {
        switch (value)
        {
            case null:
                return;
            case ContentItemReference reference:
                dependencyKeys.Add($"contentitem|byguid|{reference.Identifier}");
                break;
            case WebPageRelatedItem webPageReference:
                dependencyKeys.Add($"webpageitem|byguid|{webPageReference.WebPageGuid}");
                break;
            case IWebPageFieldsSource webPageFieldSource:
                dependencyKeys.Add($"webpageitem|byid|{webPageFieldSource.SystemFields.WebPageItemID}");
                break;
            case IContentItemFieldsSource contentItemFieldSource:
                dependencyKeys.Add($"contentitem|byid|{contentItemFieldSource.SystemFields.ContentItemID}");
                break;
            case IEnumerable<IWebPageFieldsSource> webPageFieldSources:
            {
                foreach (var source in webPageFieldSources)
                {
                    dependencyKeys.Add($"webpageitem|byid|{source.SystemFields.WebPageItemID}");
                }
            }
            break;
            case IEnumerable<IContentItemFieldsSource> contentItemFieldSources:
            {
                foreach (var source in contentItemFieldSources)
                {
                    dependencyKeys.Add($"contentitem|byid|{source.SystemFields.ContentItemID}");
                }
            }
            break;
            default:
                break;
        }
    }

    private static IReadOnlyList<string> ExtractCacheDependencyKeys<T>(in IEnumerable<T> items)
    {
        var dependencyKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            AddDependencyKeys(item, dependencyKeys);

            var properties = item
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                object? value = property.GetValue(item);

                if (value is null)
                {
                    continue;
                }

                AddDependencyKeys(value, dependencyKeys);
            }
        }

        return dependencyKeys.ToList();
    }
}
