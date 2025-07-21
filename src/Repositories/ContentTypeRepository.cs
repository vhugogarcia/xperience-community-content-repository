#pragma warning disable S1121
#pragma warning disable S3604
#pragma warning disable IDE0055

namespace XperienceCommunity.ContentRepository.Repositories;

public sealed class ContentTypeRepository<TEntity>(
    IProgressiveCache cache,
    IContentQueryExecutor executor,
    IWebsiteChannelContext websiteChannelContext,
    ICacheDependencyBuilder cacheDependencyBuilder)
    : BaseRepository(cache, executor,
        websiteChannelContext, cacheDependencyBuilder), IContentTypeRepository<TEntity>
    where TEntity : class, IContentItemFieldsSource
{
    private readonly string contentType = typeof(TEntity)?.GetContentTypeName() ?? string.Empty;

    public override string CachePrefix => $"data|{contentType}|{WebsiteChannelContext.WebsiteChannelName}";

    #region Get by Identifiers

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByGuids(IEnumerable<Guid> nodeGuid, string languageName = "en",
        int maxLinkedItems = 0, 
        Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var guidList = nodeGuid?.ToArray() ?? [];

        if (guidList.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder();

        builder.ForContentType<TEntity>(config => config
                .WithLinkedItemsAndWebPageData(maxLinkedItems)
                .Where(where => where.WhereIn(nameof(IContentItemFieldsSource.SystemFields.ContentItemGUID), guidList)))
            .WithLanguage(languageName);

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByGuids), languageName, string.Join("_", guidList), maxLinkedItems);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByIds(IEnumerable<int> itemIds, string languageName = "en",
        int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        int[] idList = itemIds?.ToArray() ?? [];

        if (idList.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder();

        builder.ForContentType<TEntity>(config => config
                .WithLinkedItemsAndWebPageData(maxLinkedItems)
                .Where(where => where.WhereIn(nameof(IContentItemFieldsSource.SystemFields.ContentItemID), idList)))
            .WithLanguage(languageName);

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByIds), languageName, string.Join("_", idList), maxLinkedItems);

        return result;
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetByGuid(Guid itemGuid, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        // Await the result and then call FirstOrDefault
        var result = await GetByGuids([itemGuid], languageName, maxLinkedItems, dependencyFunc, cancellationToken);
        return result.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetById(int id, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        // Await the result and then call FirstOrDefault
        var result = await GetByIds([id], languageName, maxLinkedItems, dependencyFunc, cancellationToken);
        return result.FirstOrDefault();
    }

    #endregion

    #region Get All

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetAll(string languageName = "en",
        int topN = 10,
        int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var builder = new ContentItemQueryBuilder();

        builder.ForContentType<TEntity>(config =>
                config
                    .WithLinkedItemsAndWebPageData(maxLinkedItems)
                    .TopN(topN))
            .WithLanguage(languageName);

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetAll), languageName, contentType, maxLinkedItems);

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
                .WithContentTypeFields())
            .WithLanguage(languageName);

        var result = await ExecuteContentQuery<TSchema>(builder,
            () => CacheHelper.GetCacheDependency($"{schemaName}|all"),
            cancellationToken, CachePrefix, nameof(GetAllBySchema), schemaName, maxLinkedItems);

        return result;
    }

    #endregion

    #region Get by Custom Where

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByCustomWhere(Action<WhereParameters> whereParametersAction, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default, params object[] cacheNameParts)
    {
        ArgumentNullException.ThrowIfNull(whereParametersAction);
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var builder = new ContentItemQueryBuilder();

        builder.ForContentType<TEntity>(config => config
                .WithLinkedItemsAndWebPageData(maxLinkedItems)
                .Where(whereParametersAction))
            .WithLanguage(languageName);

        var cacheKeyParts = new List<object> { CachePrefix, nameof(GetByCustomWhere), contentType, languageName, maxLinkedItems };
        if (cacheNameParts?.Length > 0)
        {
            cacheKeyParts.AddRange(cacheNameParts);
        }

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, cacheKeyParts.ToArray());

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
                .Where(whereParametersAction)
                .TopN(1))
            .WithLanguage(languageName);

        var cacheKeyParts = new List<object> { CachePrefix, nameof(GetFirstByCustomWhere), contentType, languageName, maxLinkedItems };
        if (cacheNameParts?.Length > 0)
        {
            cacheKeyParts.AddRange(cacheNameParts);
        }

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, cacheKeyParts.ToArray());

        return result.FirstOrDefault();
    }

    #endregion

    #region Get by Smart Folder

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetBySmartFolderGuid(Guid smartFolderId, int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(config => config
            .WithLinkedItemsAndWebPageData(maxLinkedItems)
            .OfContentType(contentType)
            .InSmartFolder(smartFolderId));

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetBySmartFolderGuid), smartFolderId, maxLinkedItems);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetBySmartFolderId(int smartFolderId, int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(config => config
            .WithLinkedItemsAndWebPageData(maxLinkedItems)
            .OfContentType(contentType)
            .InSmartFolder(smartFolderId));

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetBySmartFolderId), contentType, smartFolderId,
            maxLinkedItems);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IContentItemFieldsSource>> GetBySmartFolderId<T1, T2>(int smartFolderId,
        int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default)
    {
        string?[] contentTypes = [typeof(T1).GetContentTypeName(), typeof(T2).GetContentTypeName()];

        if (contentTypes.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(config => config
            .WithLinkedItemsAndWebPageData(maxLinkedItems)
            .OfContentType(contentTypes)
            .InSmartFolder(smartFolderId));

        var result = await ExecuteContentQuery<IContentItemFieldsSource>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetBySmartFolderId), contentTypes, smartFolderId,
            maxLinkedItems);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IContentItemFieldsSource>> GetBySmartFolderId<T1, T2, T3>(int smartFolderId,
        int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default)
    {
        string?[] contentTypes =
        [
            typeof(T1).GetContentTypeName(), typeof(T2).GetContentTypeName(), typeof(T3).GetContentTypeName()
        ];

        if (contentTypes.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(config => config
            .WithLinkedItemsAndWebPageData(maxLinkedItems)
            .OfContentType(contentTypes)
            .InSmartFolder(smartFolderId));

        var result = await ExecuteContentQuery<IContentItemFieldsSource>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetBySmartFolderId), contentTypes, smartFolderId,
            maxLinkedItems);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IContentItemFieldsSource>> GetBySmartFolderId<T1, T2, T3, T4>(
        int smartFolderId, int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default)
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

        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(config => config
            .WithLinkedItemsAndWebPageData(maxLinkedItems)
            .OfContentType(contentTypes)
            .InSmartFolder(smartFolderId));

        var result = await ExecuteContentQuery<IContentItemFieldsSource>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetBySmartFolderId), contentTypes, smartFolderId,
            maxLinkedItems);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IContentItemFieldsSource>> GetBySmartFolderId<T1, T2, T3, T4, T5>(
        int smartFolderId, int maxLinkedItems = 0, Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default)
    {
        string?[] contentTypes =
        [
            typeof(T1).GetContentTypeName(), typeof(T2).GetContentTypeName(), typeof(T3).GetContentTypeName(),
            typeof(T4).GetContentTypeName(), typeof(T5).GetContentTypeName()
        ];

        if (contentTypes.Length == 0)
        {
            return [];
        }

        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(config => config
            .WithLinkedItemsAndWebPageData(maxLinkedItems)
            .OfContentType(contentTypes)
            .InSmartFolder(smartFolderId));

        var result = await ExecuteContentQuery<IContentItemFieldsSource>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetBySmartFolderId), contentTypes, smartFolderId,
            maxLinkedItems);

        return result;
    }

    #endregion

    #region Get by Path

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetByPath(string path, PathMatchMode pathMatchMode, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentType);
        ArgumentException.ThrowIfNullOrEmpty(path);

        var pathMatch = PathMatch.Single(path);
        if (pathMatchMode == PathMatchMode.Children)
        {
            pathMatch = PathMatch.Children(path);
        }

        var builder = new ContentItemQueryBuilder()
            .ForContentType(contentType,
                config =>
                    config
                        .WithLinkedItemsAndWebPageData(maxLinkedItems)
                        .OrderByWebPageItemOrder()
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName, pathMatch))
            .WithLanguage(languageName);

        var result = await ExecuteContentQuery<TEntity>(builder, dependencyFunc,
             cancellationToken, CachePrefix, nameof(GetByPath), path, languageName, pathMatchMode.ToString(), maxLinkedItems);

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
                        .ForWebsite(WebsiteChannelContext.WebsiteChannelName)
                        .Where(whereParametersAction)
                        .Where(where => where.WhereNotNull(columnName))
                        .Where(where => where.WhereNotEmpty(columnName))
                        .Where(where => where.WhereContainsTags(columnName, guidList)));

        var result = await ExecutePageQuery<TEntity>(builder, dependencyFunc,
            cancellationToken, CachePrefix, nameof(GetByTags), columnName, string.Join("_", guidList), maxLinkedItems, topN);

        return result;
    }

    #endregion
}
