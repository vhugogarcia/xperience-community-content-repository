namespace XperienceCommunity.ContentRepository.Repositories;

public sealed class PageTypeRepository<TEntity>(IProgressiveCache cache, IContentQueryExecutor executor,
    IWebsiteChannelContext websiteChannelContext, ICacheDependencyBuilder cacheDependencyBuilder, IWebPageUrlRetriever webPageUrlRetriever) : BaseRepository(cache, executor,
    websiteChannelContext, cacheDependencyBuilder), IPageTypeRepository<TEntity>
    where TEntity : class, IWebPageFieldsSource
{
    private readonly string? contentType = typeof(TEntity)?.GetContentTypeName() ?? string.Empty;
    public override string CachePrefix => $"data|{contentType}|{WebsiteChannelContext.WebsiteChannelName}";

    #region Get by Identifiers

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByGuids(IEnumerable<Guid> nodeGuid, string languageName = "en",
        int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var guidList = nodeGuid?.ToArray() ?? [];

        if (guidList.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentType<TEntity>(
                config =>
                    config
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .OrderByWebPageItemOrder()
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                        .Where(where => where.WhereIn(nameof(IWebPageContentQueryDataContainer.WebPageItemGUID),
                                guidList)))
            .WithLanguage(languageName);

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByGuids), string.Join("_", guidList), languageName, maxLinkedItems);

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByIds(IEnumerable<int> itemIds, string languageName = "en",
        int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        int[] itemIdList = itemIds?.ToArray() ?? [];

        if (itemIdList.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentType<TEntity>(
                config =>
                    config
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .OrderByWebPageItemOrder()
                        .Where(where =>
                            where.WhereIn(nameof(IWebPageFieldsSource.SystemFields.WebPageItemID), itemIdList)))
            .WithLanguage(languageName);

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByIds), itemIdList, languageName, contentType, maxLinkedItems);

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetByGuid(Guid itemGuid, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        var result = await GetByGuids([itemGuid], languageName, maxLinkedItems, dependencyFunc, cancellationToken);

        return result.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetById(int id, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        var result = await GetByIds([id], languageName, maxLinkedItems, dependencyFunc, cancellationToken);

        return result.FirstOrDefault();
    }


    #endregion

    #region Get All

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetAll(string languageName = "en",
        int maxLinkedItems = 0,
        int topN = 10,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);
        ContentItemQueryBuilder builder = new();

        builder.ForContentType<TEntity>(
                config =>
                    config
                        .TopN(topN)
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .OrderByWebPageItemOrder()
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName))
            .WithLanguage(languageName);

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc,
                cancellationToken, CachePrefix, nameof(GetAll), languageName, contentType, maxLinkedItems, topN);

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TSchema>> GetAllBySchema<TSchema>(string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        string? schemaName = typeof(TSchema).GetReusableFieldSchemaName();

        ArgumentException.ThrowIfNullOrEmpty(schemaName);

        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(parameters => parameters
                .WithLinkedItemsAndWebPageData(maxLinkedItems)
            .OfReusableSchema(schemaName)
            .WithWebPageData()
            .ForWebsite(WebsiteChannelContext.WebsiteChannelName))
            .WithLanguage(languageName);

        var result = await ExecuteContentQuery<TSchema>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetAllBySchema), schemaName, languageName, maxLinkedItems);

        return result;
    }

    #endregion

    #region Get by Path

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByPath(string path, string languageName = "en", int maxLinkedItems = 0,
    Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default, PathMatch? pathMatch = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);
        pathMatch ??= PathMatch.Single(path); 

        var builder = new ContentItemQueryBuilder()
            .ForContentType(contentType,
                config =>
                    config
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .OrderByWebPageItemOrder()
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName, pathMatch)) 
            .WithLanguage(languageName);

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc ?? new Func<CMSCacheDependency>(() => CacheDependencyHelper.CreateWebPageItemTypeCacheDependency([contentType], WebsiteChannelContext.WebsiteChannelName)),
            cancellationToken, CachePrefix, nameof(GetByPath), path, contentType, maxLinkedItems);

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IWebPageFieldsSource>> GetByPath<T1, T2>(string path, string languageName = "en",
        int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        string?[] contentTypes =
        [
            typeof(T1).GetContentTypeName(), typeof(T2).GetContentTypeName()
        ];

        if (contentTypes.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentTypes(
                config =>
                    config
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .OfContentType(contentTypes)
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName, PathMatch.Single(path)))
            .WithLanguage(languageName);

        var result = await ExecutePageQuery<IWebPageFieldsSource>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByPath), path, contentTypes, maxLinkedItems);

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IWebPageFieldsSource>> GetByPath<T1, T2, T3>(string path, string languageName = "en",
        int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        string?[] contentTypes =
        [
            typeof(T1).GetContentTypeName(), typeof(T2).GetContentTypeName(), typeof(T3).GetContentTypeName()
        ];

        if (contentTypes.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentTypes(
                config =>
                    config
                        .OfContentType(contentTypes)
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName, PathMatch.Single(path)))
            .WithLanguage(languageName);

        var result = await ExecutePageQuery<IWebPageFieldsSource>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByPath), path, contentTypes, maxLinkedItems);

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IWebPageFieldsSource>> GetByPath<T1, T2, T3, T4>(string path,
        string languageName = "en", int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        string?[] contentTypes =
        [
            typeof(T1).GetContentTypeName(), typeof(T2).GetContentTypeName(), typeof(T3).GetContentTypeName(),
            typeof(T4).GetContentTypeName()
        ];

        if (contentTypes.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentTypes(
                config =>
                    config
                        .OfContentType(contentTypes)
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName, PathMatch.Single(path)))
            .WithLanguage(languageName);

        var result = await ExecutePageQuery<IWebPageFieldsSource>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByPath), path, contentTypes, maxLinkedItems);

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    #endregion

    #region Get by Taxonomy Tags

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByTags(Action<WhereParameters> whereParametersAction,
        IEnumerable<Guid> tagIdentifiers,
        int maxLinkedItems = 0,
        int topN = 0,
        string columnName = "TaxonomyTags",
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var guidList = tagIdentifiers?.ToArray() ?? [];

        if (guidList.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentType<TEntity>(config =>
                    config
                        .TopN(topN)
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .OrderByWebPageItemOrder()
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                        .Where(whereParametersAction)
                        .Where(where => where.WhereNotNull(columnName))
                        .Where(where => where.WhereNotEmpty(columnName))
                        .Where(where => where.WhereContainsTags(columnName, guidList)));

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByTags), columnName, string.Join("_", guidList), maxLinkedItems, topN);

        await UpdateWebPageUrls(webPageUrlRetriever, "en", result, cancellationToken);

        return result;
    }

    #endregion

    #region Get by Custom Where

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByCustomWhere(Action<WhereParameters> whereParametersAction, string languageName = "en", int topN = 10, int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default, params object[] cacheNameParts)
    {
        ArgumentNullException.ThrowIfNull(whereParametersAction);
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var builder = new ContentItemQueryBuilder();

        builder.ForContentType<TEntity>(config => config
                .WithLinkedItemsAndWebPageData(maxLinkedItems)
                .OrderByWebPageItemOrder()
                .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                .Where(whereParametersAction)
                .TopN(topN))
            .WithLanguage(languageName);

        var cacheKeyParts = new List<object> { CachePrefix, nameof(GetByCustomWhere), contentType, maxLinkedItems };
        if (cacheNameParts?.Length > 0)
        {
            cacheKeyParts.AddRange(cacheNameParts);
        }

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, cacheKeyParts.ToArray());

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetFirstByCustomWhere(Action<WhereParameters> whereParametersAction, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default, params object[] cacheNameParts)
    {
        ArgumentNullException.ThrowIfNull(whereParametersAction);
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var builder = new ContentItemQueryBuilder();

        builder.ForContentType<TEntity>(config => config
                .WithLinkedItemsAndWebPageData(maxLinkedItems)
                .OrderByWebPageItemOrder()
                .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                .Where(whereParametersAction)
                .TopN(1))
            .WithLanguage(languageName);

        var cacheKeyParts = new List<object> { CachePrefix, nameof(GetFirstByCustomWhere), contentType, maxLinkedItems };
        if (cacheNameParts?.Length > 0)
        {
            cacheKeyParts.AddRange(cacheNameParts);
        }

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, cacheKeyParts.ToArray());

        await UpdateWebPageUrls(webPageUrlRetriever, languageName, result, cancellationToken);

        return result.FirstOrDefault();
    }

    #endregion

    #region Private Methods

    private async Task UpdateWebPageUrls<T>(IWebPageUrlRetriever webPageUrlRetriever, string? languageName, IEnumerable<T> result, CancellationToken cancellationToken) where T : IWebPageFieldsSource
    {
        var webPageGuids = new ReadOnlyCollection<Guid>(result.Select(x => x.SystemFields.WebPageItemGUID).ToArray());

        var webpageLinks = await webPageUrlRetriever.Retrieve(webPageGuids, WebsiteChannelContext.WebsiteChannelName, languageName, WebsiteChannelContext.IsPreview, cancellationToken);

        foreach (var item in result)
        {
            item.SystemFields.WebPageUrlPath = webpageLinks.FirstOrDefault(x => x.Key == item.SystemFields.WebPageItemGUID).Value.AbsoluteUrl;
        }
    }

    #endregion
}
