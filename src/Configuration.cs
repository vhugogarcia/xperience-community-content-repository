namespace XperienceCommunity.ContentRepository;

/// <summary>
/// Configuration options for Xperience content repositories.
/// </summary>
public class ContentRepositoryOptions
{
    /// <summary>
    /// Gets or sets the page types to register repositories for.
    /// </summary>
    public Type[] PageTypes { get; set; } = [];

    /// <summary>
    /// Gets or sets the content types to register repositories for.
    /// </summary>
    public Type[] ContentTypes { get; set; } = [];
}

public static class Configuration
{
    /// <summary>
    /// Adds Xperience content repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the repositories to.</param>
    /// <param name="configure">A delegate to configure the repository options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddXperienceContentRepositories(this IServiceCollection services, Action<ContentRepositoryOptions>? configure = null)
    {
        var options = new ContentRepositoryOptions();
        configure?.Invoke(options);

        // Register the base repositories
        services.AddScoped(typeof(IContentTypeRepository<>), typeof(ContentTypeRepository<>));
        services.AddScoped(typeof(IPageTypeRepository<>), typeof(PageTypeRepository<>));
        services.AddScoped<IMediaFileRepository, MediaFileRepository>();
        services.AddScoped<ICacheDependencyBuilder, Builders.CacheDependencyBuilder>();

        // Register specific content type repositories based on configuration
        foreach (var contentType in options.ContentTypes)
        {
            if (Array.Exists(contentType.GetInterfaces(), i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IContentItemFieldsSource)))
            {
                var repositoryType = typeof(IContentTypeRepository<>).MakeGenericType(contentType);
                var implementationType = typeof(ContentTypeRepository<>).MakeGenericType(contentType);
                services.AddScoped(repositoryType, implementationType);
            }
        }

        // Register specific page type repositories based on configuration
        foreach (var pageType in options.PageTypes)
        {
            if (Array.Exists(pageType.GetInterfaces(), i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IWebPageFieldsSource)))
            {
                var repositoryType = typeof(IPageTypeRepository<>).MakeGenericType(pageType);
                var implementationType = typeof(PageTypeRepository<>).MakeGenericType(pageType);
                services.AddScoped(repositoryType, implementationType);
            }
        }

        // Register the generic ContentRepositories service
        services.AddScoped<ContentRepositories>();

        return services;
    }
}
