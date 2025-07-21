namespace XperienceCommunity.ContentRepository.Repositories;

/// <summary>
/// Represents a repository for managing web pages.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IPageTypeRepository<TEntity> : IRepository<TEntity> where TEntity : class, IWebPageFieldsSource
{
    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="topN">Maximum number of results to return. 0 for all results.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentException">Thrown if content type is empty.</exception>
    public Task<IEnumerable<TEntity>> GetAll(string languageName = "en",
        int maxLinkedItems = 0,
        int topN = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities by the specified path asynchronously.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="pathMatch">The path matching strategy.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentException">Thrown if content type is empty, or path is empty.</exception>
    public Task<IEnumerable<TEntity>> GetByPath(string path, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default, PathMatch? pathMatch = null);

    /// <summary>
    /// Gets entities by the specified path asynchronously.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity.</typeparam>
    /// <typeparam name="T2">The type of the second entity.</typeparam>
    /// <param name="path">The path.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentException">Thrown if content type is empty, or path is empty.</exception>
    public Task<IEnumerable<IWebPageFieldsSource>> GetByPath<T1, T2>(string path, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities by the specified path asynchronously.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity.</typeparam>
    /// <typeparam name="T2">The type of the second entity.</typeparam>
    /// <typeparam name="T3">The type of the third entity.</typeparam>
    /// <param name="path">The path.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentException">Thrown if content type is empty, or path is empty.</exception>
    public Task<IEnumerable<IWebPageFieldsSource>> GetByPath<T1, T2, T3>(string path, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities by the specified path asynchronously.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity.</typeparam>
    /// <typeparam name="T2">The type of the second entity.</typeparam>
    /// <typeparam name="T3">The type of the third entity.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity.</typeparam>
    /// <param name="path">The path.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentException">Thrown if content type is empty, or path is empty.</exception>
    public Task<IEnumerable<IWebPageFieldsSource>> GetByPath<T1, T2, T3, T4>(string path, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities by the specified taxonomy tags asynchronously.
    /// </summary>
    /// <param name="whereParametersAction">The action to configure where parameters for the query.</param>
    /// <param name="tagIdentifiers">The collection of tag identifiers.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="topN">Maximum number of results to return. 0 for all results.</param>
    /// <param name="columnName">Name of the column referencing to the taxonomy tags.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentException">Thrown if content type is empty.</exception>
    public Task<IEnumerable<TEntity>> GetByTags(
        Action<WhereParameters> whereParametersAction,
        IEnumerable<Guid> tagIdentifiers,
        int maxLinkedItems = 0,
        int topN = 0,
        string columnName = "TaxonomyTags",
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a custom query with the specified where parameters asynchronously for web pages.
    /// </summary>
    /// <param name="whereParametersAction">The action to configure where parameters for the query.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="topN">Max results</param>
    /// <param name="maxLinkedItems">The maximum number of linked items to retrieve.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="cacheNameParts">Additional cache name parts for unique cache identification.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if content type is empty or whereParametersAction is null.</exception>
    public Task<IEnumerable<TEntity>> GetByCustomWhere(Action<WhereParameters> whereParametersAction, string languageName = "en", int topN = 10, int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default, params object[] cacheNameParts);

    /// <summary>
    /// Executes a custom query with the specified where parameters asynchronously for web pages and returns the first result.
    /// </summary>
    /// <param name="whereParametersAction">The action to configure where parameters for the query.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">The maximum number of linked items to retrieve.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="cacheNameParts">Additional cache name parts for unique cache identification.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first entity or null if no entity is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if content type is empty or whereParametersAction is null.</exception>
    public Task<TEntity?> GetFirstByCustomWhere(Action<WhereParameters> whereParametersAction, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null, CancellationToken cancellationToken = default, params object[] cacheNameParts);
}
