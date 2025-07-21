namespace XperienceCommunity.ContentRepository.Examples;

/// <summary>
/// Example usage of the new simplified dependency injection approach
/// </summary>
public static class UsageExamples
{
    /// <summary>
    /// Example of how to configure services in Startup.cs or Program.cs
    /// </summary>
    public static void ConfigureServices(IServiceCollection services)
    {
        // Register the Xperience content repositories with configuration
        services.AddXperienceContentRepositories(options =>
        {
            // Configure the page types you want to register
            // These should be your actual page type classes that implement IWebPageFieldsSource
            options.PageTypes = [
                // typeof(YourArticlePage),
                // typeof(YourBlogPostPage),
                // typeof(YourHomePage),
                // Add your page types here
            ];

            // Configure the content types you want to register  
            // These should be your actual content type classes that implement IContentItemFieldsSource
            options.ContentTypes = [
                // typeof(YourSharedContent),
                // typeof(YourNavigationItem),
                // typeof(YourCTAContent),
                // Add your content types here
            ];
        });

        // Or use without configuration (only registers the generic repositories)
        services.AddXperienceContentRepositories();
    }
}

/// <summary>
/// Example service using the ContentRepositories
/// </summary>
/// <typeparam name="TPageType">Your page type that implements IWebPageFieldsSource</typeparam>
/// <typeparam name="TContentType">Your content type that implements IContentItemFieldsSource</typeparam>
public class ExampleService<TPageType, TContentType>(ContentRepositories contentRepositories)
    where TPageType : class, IWebPageFieldsSource
    where TContentType : class, IContentItemFieldsSource
{
    /// <summary>
    /// Example method to get page data
    /// </summary>
    public async Task<IEnumerable<TPageType>> GetPagesAsync(string languageName = "en")
    {
        var pageRepository = contentRepositories.GetPageRepository<TPageType>();
        return await pageRepository.GetAll(languageName);
    }

    /// <summary>
    /// Example method to get content data
    /// </summary>
    public async Task<IEnumerable<TContentType>> GetContentAsync(string languageName = "en")
    {
        var contentRepository = contentRepositories.GetContentRepository<TContentType>();
        return await contentRepository.GetAll(languageName);
    }

    /// <summary>
    /// Example method using other services
    /// </summary>
    public async Task<IEnumerable<MediaFileInfo>> GetMediaFilesAsync(IEnumerable<Guid> mediaFileGuids)
    {
        var mediaRepository = contentRepositories.GetMediaFileRepository();
        return await mediaRepository.GetMediaFiles(mediaFileGuids);
    }
}
