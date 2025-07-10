using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blogifier.Data;
using Blogifier.Posts;
using Blogifier.Shared;
using Microsoft.EntityFrameworkCore;
using Blogifier.Helper;
using Moq;
using Xunit;

namespace Blogifier.Tests
{
    public class PostProviderTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly DbContextOptions<AppDbContext> _options;

        public PostProviderTests()
        {
            _mapperMock = new Mock<IMapper>();
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"BlogifierTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_CreatesNewPost_ReturnsSlug()
        {
            // Arrange
            var userId = 1;
            var postTitle = "Test Post Title";
            var postContent = "Test post content";
            var postSlug = "test-post-title";
            var expectedPost = new Post
            {
                Id = 1,
                Title = postTitle,
                Slug = postSlug,
                Content = postContent,
                Description = "Test post description",
                PostType = PostType.Post,
                State = PostState.Draft,
                UserId = userId
            };
            
            var postInput = new PostEditorDto
            {
                Title = postTitle,
                Content = postContent,
                Description = "Test post description",
                PostType = PostType.Post,
                State = PostState.Draft
            };

            using (var context = new AppDbContext(_options))
            {
                // Setup mapper to return the slug when ProjectTo is called
                var mockMapper = new Mock<IMapper>();
                mockMapper.Setup(m => m.Map<Post>(It.IsAny<PostEditorDto>()))
                    .Returns(expectedPost);

                // Call the method with our test data
                var postProvider = new PostProvider(mockMapper.Object, context);

                // Setup the mock for the GetSlugFromTitle method by adding a matching post to check against
                var post = new Post
                {
                    Title = "Another Post",
                    Slug = "another-post",
                    Content = "Some content",
                    Description = "Some description",
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                };
                context.Posts.Add(post);
                await context.SaveChangesAsync();
                
                // Act
                var result = await postProvider.AddAsync(postInput, userId);
                
                // Assert
                Assert.NotNull(result);
                var savedPost = await context.Posts.Where(p => p.Title == postTitle).FirstOrDefaultAsync();
                Assert.NotNull(savedPost);
                Assert.Equal(postTitle, savedPost.Title);
                Assert.Equal(postContent, savedPost.Content);
                Assert.Equal(userId, savedPost.UserId);
            }
        }
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**  
1. **Install Node.js and npm** on the system running the tests.  
2. Alternatively, **disable frontend builds** in test environments by modifying `.csproj` files to bypass the `npm i` command if frontend assets are not required for testing.

    [Fact]
    public async Task GetAsync_FeaturedFilter_NoFeaturedPosts_ReturnsEmptyList()
    {
        // Arrange
        var filter = PublishedStatus.Featured;
        var postType = PostType.Post;
    
        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                  .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto()).AsQueryable());
    
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(filter, postType);
    
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**  
1. **Install Node.js and npm** on the system running the tests.  
2. Alternatively, **disable frontend builds** in test environments by modifying `.csproj` files to bypass the `npm i` command if frontend assets are not required for testing.

    [Fact]
    public async Task GetSlugFromTitle_TitleExists100Times_ThrowsException()
    {
        // Arrange
        var title = "Existing Title";
        var slug = "existing-title";
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            // Add 100 posts with the same title and slug
            for (int i = 0; i < 100; i++)
            {
                context.Posts.Add(new Post
                {
                    Title = title,
                    Slug = i == 0 ? slug : $"{slug}{i}",
                    Content = "Some content",
                    Description = "Some description",
                    UserId = 1,
                    State = PostState.Draft,
                    PublishedAt = DateTime.UtcNow,
                    PostType = PostType.Post
                });
            }
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.GetSlugFromTitle(title));
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**  
1. **Install Node.js and npm** on the system running the tests.  
2. Alternatively, **disable frontend builds** in test environments by modifying `.csproj` files to bypass the `npm i` command if frontend assets are not required for testing.

    [Fact]
    public async Task GetSlugFromTitle_TitleExists_GeneratesIncrementedSlug()
    {
        // Arrange
        var title = "Existing Title";
        var slug = "existing-title";
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            // Add a post with the same title and slug
            context.Posts.Add(new Post
            {
                Title = title,
                Slug = slug,
                Content = "Some content",
                Description = "Some description",
                UserId = 1,
                State = PostState.Draft,
                PublishedAt = DateTime.UtcNow,
                PostType = PostType.Post
            });
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetSlugFromTitle(title);
    
            // Assert
            Assert.Equal("existing-title1", result);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**  
1. **Install Node.js and npm** on the system running the tests.  
2. Alternatively, **disable frontend builds** in test environments by modifying `.csproj` files to bypass the `npm i` command if frontend assets are not required for testing.

    [Fact]
    public async Task GetPostsAsync_LargePageSize_ReturnsUpToMaxAllowed()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 1000;
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_GetPostsAsync_LargePageSize")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            // Add multiple posts to the database
            for (int i = 0; i < 1500; i++)
            {
                context.Posts.Add(new Post
                {
                    Id = i + 1,
                    Title = $"Post {i + 1}",
                    Slug = $"post-{i + 1}",
                    Content = "Some content",
                    Description = "Some description",
                    UserId = userId,
                    State = PostState.Release,
                    PublishedAt = DateTime.UtcNow,
                    PostType = PostType.Post
                });
            }
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto()).AsQueryable());
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetPostsAsync(page, pageSize);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1000, result.Items.Count);
            Assert.Equal(1500, result.Total);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**  
1. **Install Node.js and npm** on the system running the tests.  
2. Alternatively, **disable frontend builds** in test environments by modifying `.csproj` files to bypass the `npm i` command if frontend assets are not required for testing.

    [Fact]
    public async Task GetByCategoryAsync_EmptyCategory_ReturnsEmpty()
    {
        // Arrange
        var category = "empty-category";
        var page = 1;
        var pageSize = 10;
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto()).AsQueryable());
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetByCategoryAsync(category, page, pageSize);
    
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**  
1. **Install Node.js and npm** on the system running the tests.  
2. Alternatively, **disable frontend builds** in test environments by modifying `.csproj` files to skip `npm i` if frontend assets are not required for testing.

    [Fact]
    public async Task GetSearchAsync_NoMatchingPosts_ReturnsEmpty()
    {
        // Arrange
        var userId = 1;
        var term = "non-existent-term";
        var page = 1;
        var pageSize = 10;
    
        using (var context = new AppDbContext(_options))
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto()).AsQueryable());
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetSearchAsync(term, page, pageSize);
    
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**  
1. **Install Node.js and npm** on the system running the tests.  
2. Alternatively, **skip frontend builds** in test environments by modifying `.csproj` files to bypass the `npm i` command if frontend assets are not required for testing.

    [Fact]
    public async Task GetEditorAsync_PostWithoutCategories_ReturnsEmptyCategories()
    {
        // Arrange
        var userId = 1;
        var slug = "post-without-categories";
        var post = new Post
        {
            Id = 1,
            Title = "Post Without Categories",
            Slug = slug,
            Content = "Some content",
            Description = "Some description",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow
        };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostEditorDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostEditorDto { Categories = [] }).AsQueryable());
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetEditorAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Empty(result.Categories);
        }
    }

*/
/*
FAILED TEST: The test run failed due to a **build failure** caused by missing `npm` during the execution of `npm i` for `Blogifier.Admin` and `Blogifier.Themes.Standard`.

**Recommended Fix:**
1. **Install Node.js and npm** on the system running the tests.
2. Alternatively, **skip frontend builds** in test environments by modifying `.csproj` files to bypass the `npm i` command if frontend assets are not required for testing.

    [Fact]
    public async Task UpdateAsync_InvalidUserId_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var invalidUserId = 2;
        var slug = "test-post";
        var post = new Post
        {
            Id = 1,
            Title = "Test Post",
            Slug = slug,
            Content = "Some content",
            Description = "Some description",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postInput = new PostEditorDto
            {
                Id = 1,
                Title = "Updated Title",
                Content = "Updated content",
                Description = "Updated description",
                PostType = PostType.Post,
                State = PostState.Release
            };
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostToHtmlDto { Views = 0 }).AsQueryable());
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto()).AsQueryable());
            mockMapper.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostToHtmlDto { Views = 0 }).AsQueryable());
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.UpdateAsync(postInput, invalidUserId));
        }
    }

*/
/*
FAILED TEST: **Analysis:**
The test run failed due to a **build failure** caused by missing `npm` on the system. The project attempts to run `npm i` during the build process for `Blogifier.Admin` and `Blogifier.Themes.Standard`, but `npm` is not found.

**Recommended Fix:**
1. **Install Node.js and npm** on the system running the tests.
2. Alternatively, **disable frontend builds** in CI/CD or test environments by modifying `.csproj` files to skip `npm i` if frontend assets are not required for testing.

    [Fact]
    public async Task GetAsync_PostWithoutRelated_ReturnsEmptyRelated()
    {
        // Arrange
        var userId = 1;
        var slug = "post-without-related";
        var post = new Post
        {
            Id = 1,
            Title = "Post Without Related",
            Slug = slug,
            Content = "Some content",
            Description = "Some description",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow
        };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostToHtmlDto { Views = 0 }).AsQueryable());
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto()).AsQueryable());
            mockMapper.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostToHtmlDto { Views = 0 }).AsQueryable());
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Post.Id);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
            Assert.Empty(result.Related);
        }
    }

*/
/*
FAILED TEST: **Analysis:**

The test run failed due to missing `npm` during the build process, as indicated by the error:

```
/usr/bin/sh: 2: /tmp/...: npm: not found
```

This is not a test failure but a **build failure** caused by missing Node.js and npm dependencies required by the project (likely for frontend assets in `Blogifier.Admin` and `Blogifier.Themes.Standard`).

**Recommended Fix:**

1. **Install Node.js and npm** on the system running the build/test.
2. Alternatively, **skip frontend builds** if not required for testing by modifying `.csproj` files to bypass the `npm i` command during CI/CD or test runs.

    [Fact]
    public async Task GetAsync_NonExistentSlug_ReturnsDefaultValues()
    {
        // Arrange
        var userId = 1;
        var slug = "non-existent-slug";
        var expectedPost = new PostToHtmlDto { Views = 0 };
        var expectedPostSlugDto = new PostSlugDto { Post = expectedPost, Older = null, Newer = null, Related = Array.Empty<PostToHtmlDto>() };
    
        using (var context = new AppDbContext(_options))
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostToHtmlDto { Views = 0 }).AsQueryable());
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto()).AsQueryable());
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPostSlugDto.Post.Views, result.Post.Views);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
            Assert.Empty(result.Related);
        }
    }

*/
    }
}
