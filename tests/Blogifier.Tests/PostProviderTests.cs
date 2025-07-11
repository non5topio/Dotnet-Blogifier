using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blogifier.Data;
using Blogifier.Posts;
using Blogifier.Shared;
using Blogifier.Blogs;
using Blogifier.Helper;
using Blogifier.Helper;
using Blogifier.Helper;
using Microsoft.EntityFrameworkCore;
using Blogifier.Extensions;
using Moq;
using Blogifier.Helper;
using Xunit;
using Microsoft.AspNetCore.Components.Forms;

using ReverseMarkdown.Converters;
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
/*
FAILED TEST: **Analysis:**  
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task GetPostsAsync_VeryLargePageSize_ReturnsLimitedItems()
    {
        // Arrange
        var userId = 1;
        var pageSize = 10000;
    
        using (var context = new AppDbContext(_options))
        {
            for (int i = 1; i <= 50; i++)
            {
                var post = new Post
                {
                    Title = $"Post {i}",
                    Content = "Test content",
                    UserId = userId,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow
                };
                context.Posts.Add(post);
            }
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetPostsAsync(1, pageSize);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(50, result.Items.Count);
            Assert.Equal(50, result.Total);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task AddAsync_MaximumSlugIncrementExceeded_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var title = "Existing Title";
        var slugBase = "existing-title";
    
        using (var context = new AppDbContext(_options))
        {
            for (int i = 1; i <= 100; i++)
            {
                var existingPost = new Post
                {
                    Title = title,
                    Slug = i == 1 ? slugBase : $"{slugBase}{i}",
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                };
                context.Posts.Add(existingPost);
            }
            await context.SaveChangesAsync();
    
            var postInput = new PostEditorDto
            {
                Title = title,
                Content = "Test content",
                PostType = PostType.Post,
                State = PostState.Draft
            };
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.AddAsync(postInput, userId));
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task GetAsync_NoOlderOrNewerPosts_ReturnsNull()
    {
        // Arrange
        var slug = "test-slug";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Slug = slug,
                Content = "Test content",
                UserId = userId,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Post);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task GetAsync_NoRelatedPosts_ReturnsEmptyRelated()
    {
        // Arrange
        var slug = "test-slug";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Slug = slug,
                Content = "Test content",
                UserId = userId,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Post);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
            Assert.Empty(result.Related);
        }
    }

*/
                Assert.NotNull(savedPost);
/*
FAILED TEST: **Analysis:**  
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task UpdateAsync_NullOrEmptyTitle_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var postInput = new PostEditorDto
        {
            Title = null,
            Content = "Test content",
            PostType = PostType.Post,
            State = PostState.Draft
        };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            var post = new Post
            {
                Id = 1,
                Title = "Existing Title",
                Content = "Existing content",
                UserId = userId,
                State = PostState.Draft,
                PostType = PostType.Post
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.UpdateAsync(postInput, userId));
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task GetByCategoryAsync_NoPostsInCategory_ReturnsEmptyResult()
    {
        // Arrange
        var category = "non-existent-category";
        var page = 1;
        var pageSize = 10;
    
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
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
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task GetSearchAsync_NoMatchingPosts_ReturnsEmptyResult()
    {
        // Arrange
        var term = "non-existent-term";
        var page = 1;
        var pageSize = 10;
    
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
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
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task AddAsync_ExistingTitle_GeneratesIncrementedSlug()
    {
        // Arrange
        var userId = 1;
        var title = "Existing Title";
        var expectedSlug = "existing-title-1";
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        var mapperMock = new Mock<IMapper>();
    
        using (var context = new AppDbContext(options))
        {
            // Add an existing post with the same title
            var existingPost = new Post
            {
                Title = title,
                Slug = "existing-title",
                UserId = userId,
                State = PostState.Draft,
                PostType = PostType.Post
            };
            context.Posts.Add(existingPost);
            await context.SaveChangesAsync();
    
            var postInput = new PostEditorDto
            {
                Title = title,
                Content = "Test content",
                PostType = PostType.Post,
                State = PostState.Draft
            };
    
            var postProvider = new PostProvider(mapperMock.Object, context);
    
            // Act
            var result = await postProvider.AddAsync(postInput, userId);
    
            // Assert
            Assert.Equal(expectedSlug, result);
            var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == expectedSlug);
            Assert.NotNull(savedPost);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the system lacks `npm` (Node.js package manager), which is required by the build process to install JavaScript dependencies in the `Blogifier.Admin` and `Blogifier.Themes.Standard` projects.

**Recommended Fix:**  
- Install Node.js and npm on the system, or  
- Modify the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task UpdateAsync_InvalidUserId_ThrowsBlogNotIitializeException()
    {
        // Arrange
        var userId = 1;
        var invalidUserId = 2;
        var postTitle = "Test Post";
        var postContent = "Test content";
        var postInput = new PostEditorDto
        {
            Id = 1,
            Title = postTitle,
            Content = postContent,
            PostType = PostType.Post,
            State = PostState.Draft
        };
    
        using (var context = new AppDbContext(_options))
        {
            var post = new Post
            {
                Id = 1,
                Title = postTitle,
                Content = postContent,
                UserId = userId,
                State = PostState.Draft,
                PostType = PostType.Post
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.UpdateAsync(postInput, invalidUserId));
        }
    }

*/
/*
FAILED TEST: The test run failed because the build process attempted to execute `npm i` (install Node.js packages), but the `npm` command is not found on the system. This is likely due to the absence of Node.js and npm in the environment where the tests are being executed.

### **Recommended Fix:**
Install Node.js and npm on the system, or configure the build/test environment to skip the `npm i` step if Node.js is not required for testing.

    [Fact]
    public async Task GetAsync_NonExistentSlug_ReturnsGracefully()
    {
        // Arrange
        var slug = "non-existent-slug";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Post);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
            Assert.Empty(result.Related);
        }
    }

*/
                Assert.Equal(postTitle, savedPost.Title);
                Assert.Equal(postContent, savedPost.Content);
                Assert.Equal(userId, savedPost.UserId);
            }
        }
    }
}
