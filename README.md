# Xperience Community: Content Repository

[![Nuget](https://img.shields.io/nuget/v/XperienceCommunity.ContentRepository)](https://www.nuget.org/packages/XperienceCommunity.ContentRepository)

## Description

This package provides a comprehensive repository pattern implementation for Kentico Xperience, offering simplified content access, enhanced caching, and streamlined dependency injection. It includes repositories for web pages, content items, and media files with advanced querying capabilities, automatic caching, and flexible configuration options.

## Library Version Matrix

| Xperience Version | Library Version |
|-------------------|-----------------|
| >= 30.6.0         | >= 1.0.0        |

> **Note:** The latest version that has been tested is 30.6.0

## ⚙️ Package Installation

Add the package to your application using the .NET CLI

```bash
dotnet add package XperienceCommunity.ContentRepository
```

## 🚀 Quick Start

### Basic Configuration

Register the repositories in your DI container:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Option 1: Configure specific types for automatic registration
    services.AddXperienceContentRepositories(options =>
    {
        options.PageTypes = [
            typeof(ArticlePage),
            typeof(BlogPostPage),
            typeof(HomePage)
        ];
        
        options.ContentTypes = [
            typeof(SharedContent),
            typeof(NavigationItem),
            typeof(CTAContent)
        ];
    });

    // Option 2: Use generic repositories only
    services.AddXperienceContentRepositories();
}
```

### Basic Usage

```csharp
public class ContentService
{
    private readonly ContentRepositories _contentRepositories;

    public ContentService(ContentRepositories contentRepositories)
    {
        _contentRepositories = contentRepositories;
    }

    public async Task<IEnumerable<ArticlePage>> GetArticlesAsync()
    {
        var pageRepository = _contentRepositories.GetPageRepository<ArticlePage>();
        return await pageRepository.GetAll("en");
    }

    public async Task<IEnumerable<SharedContent>> GetSharedContentAsync()
    {
        var contentRepository = _contentRepositories.GetContentRepository<SharedContent>();
        return await contentRepository.GetAll("en");
    }

    public async Task<IEnumerable<MediaFileInfo>> GetMediaFilesAsync(IEnumerable<Guid> mediaGuids)
    {
        var mediaRepository = _contentRepositories.GetMediaFileRepository();
        return await mediaRepository.GetMediaFilesAsync(mediaGuids);
    }
}
```

## ✨ Features

### Repository Pattern Implementation

**Page Type Repository**: Comprehensive repository for web page content types with support for:
- Get by IDs, GUIDs, and custom where conditions
- Path-based querying with flexible path matching modes
- Multi-type queries (query multiple page types simultaneously)
- Taxonomy tag filtering
- Reusable schema support
- Automatic URL path resolution

**Content Type Repository**: Full-featured repository for content items including:
- Get all content with pagination support
- Smart folder-based queries
- Multi-type content retrieval
- Custom where condition support
- Reusable schema querying
- Taxonomy tag filtering

**Media File Repository**: Specialized repository for media file operations:
- Retrieve media files by GUIDs
- Get media files from asset related items
- Media library management
- Automatic caching with dependency management

### Advanced Querying Capabilities

- **Smart Folder Support**: Query content from smart folders with multi-type support
- **Path Matching**: Flexible path matching for web pages (exact, children, section)
- **Taxonomy Integration**: Built-in support for taxonomy tag filtering
- **Custom Where Conditions**: Flexible query building with custom where parameters
- **Multi-Language Support**: Language-specific content retrieval
- **Linked Items**: Configurable linked item depth for related content

### Caching & Performance

- **Automatic Caching**: Progressive caching with configurable expiration
- **Cache Dependencies**: Intelligent cache invalidation based on content changes
- **Cache Key Management**: Optimized cache key generation for better performance
- **Sliding Expiration**: Configurable sliding expiration for better memory management

### Dependency Injection Enhancements

- **Centralized Access**: Single `ContentRepositories` service for all repository access
- **Type Safety**: Full type safety with generic methods
- **Flexible Configuration**: Configure specific types or use generic approach
- **Backward Compatibility**: Works alongside existing repository patterns

### Extension Methods & Utilities

- **Query Builder Extensions**: Enhanced ContentItemQueryBuilder with conditional operations
- **Type Extensions**: Automatic content type name resolution
- **Cache Dependency Helpers**: Simplified cache dependency creation
- **Parameter Extensions**: Extended where parameter functionality

## 📚 Examples

For detailed examples on how to use custom where conditions and other advanced features, check out the [`Examples`](./src/Examples/) folder in this repository. It contains practical implementations showing:

- Custom where usage examples
- Page custom where usage examples  
- General usage patterns

## 🙏 Acknowledgments

This project was inspired by the excellent work done by [Brandon Henricks](https://github.com/brandonhenricks) in his [Xperience by Kentico Data Repository](https://github.com/brandonhenricks/xperience-by-kentico-data-repository) project. We built upon those foundational concepts to create this enhanced repository pattern implementation.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License.