namespace XperienceCommunity.ContentRepository.Repositories;

/// <summary>
/// Represents a generic repository interface for accessing data.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IRepository<TEntity>
{
    /// <summary>
    /// Gets all entities asynchronously based on the specified node GUIDs.
    /// </summary>
    /// <param name="nodeGuid">The node GUIDs.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if content type is empty.</exception>
    public Task<IEnumerable<TEntity>> GetByGuids(IEnumerable<Guid> nodeGuid, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities asynchronously based on the specified item IDs.
    /// </summary>
    /// <param name="itemIds">The item IDs.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if content type is empty.</exception>
    public Task<IEnumerable<TEntity>> GetByIds(IEnumerable<int> itemIds, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if content type is empty.</exception>
    public Task<TEntity?> GetById(int id, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by its node GUID asynchronously.
    /// </summary>
    /// <param name="itemGuid">The item GUID of the entity.</param>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if content type is empty.</exception>
    public Task<TEntity?> GetByGuid(Guid itemGuid, string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities by the specified schema asynchronously.
    /// </summary>
    /// <typeparam name="TSchema">The type of the schema.</typeparam>
    /// <param name="languageName">The language name.</param>
    /// <param name="maxLinkedItems">Maximum linked items to return.</param>
    /// <param name="dependencyFunc">The function to create a cache dependency.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    /// <exception cref="ArgumentNullException">Thrown if content type is empty.</exception>
    public Task<IEnumerable<TSchema>> GetAllBySchema<TSchema>(string languageName = "en", int maxLinkedItems = 0,
        Func<CMSCacheDependency>? dependencyFunc = null,
        CancellationToken cancellationToken = default);
}
