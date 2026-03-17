using Kentico.Content.Web.Mvc.Routing;

namespace XperienceCommunity.ContentRepository.Repositories;

public class WebPageRepository(
    IWebPageUrlRetriever urlRetriever,
    IPreferredLanguageRetriever languageRetriever,
    IProgressiveCache cache) : IWebPageRepository
{
    private readonly IWebPageUrlRetriever urlRetriever = urlRetriever;
    private readonly IPreferredLanguageRetriever languageRetriever = languageRetriever;
    private readonly IProgressiveCache cache = cache;

    public async Task<WebPageUrl?> GetUrlByGuid(Guid webPageGuid, string? languageName = null)
    {
        if (webPageGuid == Guid.Empty)
        {
            return null;
        }

        string language = languageName ?? languageRetriever.Get();

        var cacheSettings = new CacheSettings(
            cacheMinutes: 60,
            cacheItemNameParts: ["webpageurl", webPageGuid.ToString(), language]);

        return await cache.LoadAsync(async cs =>
        {
            cs.CacheDependency = CacheHelper.GetCacheDependency(
                $"webpageitem|byguid|{webPageGuid}");

            try
            {
                return await urlRetriever.Retrieve(webPageGuid, language);
            }
            catch
            {
                cs.Cached = false;
                return null;
            }
        }, cacheSettings);
    }
}
