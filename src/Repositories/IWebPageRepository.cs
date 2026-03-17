namespace XperienceCommunity.ContentRepository.Repositories;

public interface IWebPageRepository
{
    /// <summary>
    /// Gets URL by web page GUID.
    /// </summary>
    public Task<WebPageUrl?> GetUrlByGuid(Guid webPageGuid, string? languageName = null);
}
