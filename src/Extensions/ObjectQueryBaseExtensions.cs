namespace XperienceCommunity.ContentRepository.Extensions;

public static class ObjectQueryBaseExtensions
{

    /// <summary>
    /// Filters the query to include only rows where the specified column is not null or empty.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <param name="query">The object query.</param>
    /// <param name="columnName">The name of the column to check for null or empty values.</param>
    /// <returns>The filtered query.</returns>
    public static ObjectQueryBase<TQuery, TObject> WhereNotNullOrEmpty<TQuery, TObject>(this ObjectQueryBase<TQuery, TObject> query, string columnName)
        where TQuery : ObjectQueryBase<TQuery, TObject>, new() where TObject : BaseInfo
    {
        query
            .WhereNotNull(columnName)
            .And()
            .WhereNotEmpty(columnName);

        return query;
    }

    /// <summary>
    /// Retrieves a single object asynchronously from the query result.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <param name="query">The object query.</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The single object from the query result.</returns>
    public static async Task<TObject> SingleAsync<TQuery, TObject>(this ObjectQueryBase<TQuery, TObject> query,
        CancellationToken? cancellationToken = null)
        where TQuery : ObjectQueryBase<TQuery, TObject>, new() where TObject : BaseInfo
    {
        var result = await query.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);

        return result.Single();
    }

    /// <summary>
    /// Retrieves the first object or null asynchronously from the query result.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <param name="query">The object query.</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The first object or null from the query result.</returns>
    public static async Task<TObject?> FirstOrDefaultAsync<TQuery, TObject>(
        this ObjectQueryBase<TQuery, TObject> query, CancellationToken? cancellationToken = null)
        where TQuery : ObjectQueryBase<TQuery, TObject>, new() where TObject : BaseInfo
    {
        var result = await query.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);

        return result.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves a list of objects asynchronously from the query result.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <param name="query">The object query.</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>A list of objects from the query result.</returns>
    public static async Task<IEnumerable<TObject>> ToListAsync<TQuery, TObject>(
        this ObjectQueryBase<TQuery, TObject> query, CancellationToken? cancellationToken = null)
        where TQuery : ObjectQueryBase<TQuery, TObject>, new() where TObject : BaseInfo
    {
        var result = await query.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);

        return result.ToList();
    }
}
