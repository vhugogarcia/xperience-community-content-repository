namespace XperienceCommunity.ContentRepository.Repositories;

/// <summary>
/// Content repositories service that provides access to all registered repositories.
/// </summary>
public class ContentRepositories(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Gets a content repository for the specified type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that implements <see cref="IContentItemFieldsSource"/>.</typeparam>
    /// <returns>The content repository for the specified type.</returns>
    public IContentTypeRepository<TEntity> GetContentRepository<TEntity>() where TEntity : class, IContentItemFieldsSource
        => serviceProvider.GetRequiredService<IContentTypeRepository<TEntity>>();

    /// <summary>
    /// Gets a page repository for the specified type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that implements <see cref="IWebPageFieldsSource"/>.</typeparam>
    /// <returns>The page repository for the specified type.</returns>
    public IPageTypeRepository<TEntity> GetPageRepository<TEntity>() where TEntity : class, IWebPageFieldsSource
        => serviceProvider.GetRequiredService<IPageTypeRepository<TEntity>>();

    /// <summary>
    /// Gets the media file repository.
    /// </summary>
    /// <returns>The media file repository.</returns>
    public IMediaFileRepository GetMediaFileRepository()
        => serviceProvider.GetRequiredService<IMediaFileRepository>();

    /// <summary>
    /// Gets the cache dependency builder.
    /// </summary>
    /// <returns>The cache dependency builder.</returns>
    public ICacheDependencyBuilder GetCacheDependencyBuilder()
        => serviceProvider.GetRequiredService<ICacheDependencyBuilder>();

    /// <summary>
    /// Gets a taxonomy retriever.
    /// </summary>
    /// <returns>The taxonomy retriever.</returns>
    public ITaxonomyRetriever GetTaxonomyRetriever()
        => serviceProvider.GetRequiredService<ITaxonomyRetriever>();

    /// <summary>
    /// Gets the HTTP context accessor.
    /// </summary>
    /// <returns>The HTTP context accessor.</returns>
    public IHttpContextAccessor GetHttpContextAccessor()
        => serviceProvider.GetRequiredService<IHttpContextAccessor>();

    /// <summary>
    /// Gets the web page data context retriever.
    /// </summary>
    public IWebPageDataContextRetriever GetWebPageDataContext()
        => serviceProvider.GetRequiredService<IWebPageDataContextRetriever>();

    /// <summary>
    /// Gets the website channel context.
    /// </summary>
    public IWebsiteChannelContext GetWebsiteChannelContext()
        => serviceProvider.GetRequiredService<IWebsiteChannelContext>();
}
