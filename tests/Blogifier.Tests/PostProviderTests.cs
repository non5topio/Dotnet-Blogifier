using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blogifier.Data;
using Microsoft.AspNetCore.Mvc;
using Blogifier.Posts;
using Blogifier.Shared;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

using Blogifier.Helper;
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
/*

    [Fact]
    public async Task FirstAsync_WithNonExistentPostId_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentId = 999999999;
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => postProvider.FirstAsync(nonExistentId));
        }
    }


    [Fact]
    public async Task UpdateAsync_WithUnauthorizedUser_ThrowsBlogNotIitializeException()
    {
        // Arrange
        var userId = 2;
        var postTitle = "Test Post";
        var postContent = "Test content";
        var postDescription = "Test description";
    
        using (var context = new AppDbContext(_options))
        {
            // Add a post with a different user ID
            var post = new Post
            {
                Id = 1,
                Title = postTitle,
                Content = postContent,
                Description = postDescription,
                UserId = 1,
                State = PostState.Draft,
                PostType = PostType.Post
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            var postInput = new PostEditorDto
            {
                Id = 1,
                Title = "Updated Title",
                Content = "Updated content",
                Description = "Updated description",
                PostType = PostType.Post,
                State = PostState.Draft
            };
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.UpdateAsync(postInput, userId));
        }
    }


    [Fact]
    public async Task AddAsync_WithMaliciousHtml_ContentAndDescriptionAreFiltered()
    {
        // Arrange
        var userId = 1;
        var title = "Test Post";
        var content = "<script>alert('xss')</script>";
        var description = "<img src=x onerror=alert(1)>";
    
        using (var context = new AppDbContext(_options))
        {
            var postInput = new PostEditorDto
            {
                Title = title,
                Content = content,
                Description = description,
                PostType = PostType.Post,
                State = PostState.Draft
            };
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var slug = await postProvider.AddAsync(postInput, userId);
    
            // Assert
            var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
            Assert.NotNull(savedPost);
            Assert.DoesNotContain("<script>", savedPost.Content);
            Assert.DoesNotContain("<img", savedPost.Description);
        }
    }


    [Fact]
    public async Task GetSlugFromTitle_With100DuplicateSlugs_ThrowsBlogNotIitializeException()
    {
        // Arrange
        var title = "duplicate-title";
        var slug = title.ToSlug();
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            // Add 100 posts with the same slug
            for (int i = 1; i <= 100; i++)
            {
                context.Posts.Add(new Post
                {
                    Id = i,
                    Title = title,
                    Slug = $"{slug}{i}",
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                });
            }
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.GetSlugFromTitle(title));
        }
    }


    [Fact]
    public async Task GetAsync_WithDuplicateSlugs_ReturnsFirstMatch()
    {
        // Arrange
        var userId = 1;
        var slug = "duplicate-slug";
    
        using (var context = new AppDbContext(_options))
        {
            // Add two posts with the same slug
            var post1 = new Post { Id = 1, Title = "Post 1", Slug = slug, UserId = userId, State = PostState.Release };
            var post2 = new Post { Id = 2, Title = "Post 2", Slug = slug, UserId = userId, State = PostState.Release };
            context.Posts.Add(post1);
            context.Posts.Add(post2);
            await context.SaveChangesAsync();
    
            var postToHtmlDto1 = new PostToHtmlDto { Id = 1, Title = "Post 1", Slug = slug, Views = 0 };
            var postToHtmlDto2 = new PostToHtmlDto { Id = 2, Title = "Post 2", Slug = slug, Views = 0 };
    
            _mapperMock.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                .Returns(new List<PostToHtmlDto> { postToHtmlDto1, postToHtmlDto2 }.AsQueryable().BuildMock());
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(postToHtmlDto1.Id, result.Post.Id);
        }
    }


    [Fact]
    public async Task GetAsync_WithNonExistentSlug_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentSlug = "non-existent-slug";
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => postProvider.GetAsync(nonExistentSlug));
        }
    }

FAILED TEST: **Analysis:**  
The test run failed because the build process attempted to execute `npm i` (install Node.js packages), but the `npm` command is not found on the system. This is likely due to Node.js/npm not being installed or not available in the environment's PATH.

**Recommended Fix:**  
- Install Node.js and npm on the system running the tests.  
- Alternatively, if Node.js is not required, modify the project files to remove or conditionally skip the `npm i` build commands.

    [Fact]
    public async Task AddAsync_WithMaliciousHtml_ContentAndDescriptionAreFiltered()
    {
        // Arrange
        var userId = 1;
        var title = "Test Post";
        var content = "<script>alert('xss')</script>";
        var description = "<img src=x onerror=alert(1)>";
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        var mapperMock = new Mock<IMapper>();
        var context = new AppDbContext(options);
    
        var postProvider = new PostProvider(mapperMock.Object, context);
    
        // Act
        var slug = await postProvider.AddAsync(new PostEditorDto
        {
            Title = title,
            Content = content,
            Description = description,
            PostType = PostType.Post,
            State = PostState.Draft
        }, userId);
    
        // Assert
        var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        Assert.NotNull(savedPost);
        Assert.DoesNotContain("<script>", savedPost.Content);
        Assert.DoesNotContain("<img", savedPost.Description);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the build process attempted to execute `npm i` (install Node.js packages), but the `npm` command is not found on the system. This is likely due to Node.js/npm not being installed or not available in the environment's PATH.

**Recommended Fix:**  
- Install Node.js and npm on the system running the tests.  
- Alternatively, if Node.js is not required, modify the project files to remove or conditionally skip the `npm i` build commands.

    [Fact]
    public async Task GetSearchAsync_WithEmptyTerm_ReturnsAllPostsOrThrowsException()
    {
        // Arrange
        var emptyTerm = "";
        var page = 1;
        var pageSize = 10;
    
        using (var context = new AppDbContext(_options))
        {
            // Add a test post
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Content = "Test content",
                Description = "Test description",
                UserId = 1,
                State = PostState.Release
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postItemDto = new PostItemDto { Id = 1, Title = "Test Post", Content = "Test content", Description = "Test description" };
    
            _mapperMock.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                .Returns(new List<PostItemDto> { postItemDto }.AsQueryable().BuildMock());
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetSearchAsync(emptyTerm, page, pageSize);
    
            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the build process attempted to execute `npm i` (install Node.js packages), but the `npm` command is not found on the system. This is likely due to Node.js/npm not being installed or not available in the environment's PATH.

**Recommended Fix:**  
- Install Node.js and npm on the system running the tests.  
- Alternatively, if Node.js is not required, modify the project files to remove or conditionally skip the `npm i` build commands.

    [Fact]
    public async Task GetPostsAsync_WithInvalidPageSize_ReturnsEmptyListOrThrowsException()
    {
        // Arrange
        var invalidPageSize = 0;
        var page = 1;
    
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => postProvider.GetPostsAsync(page, invalidPageSize));
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the build process attempted to execute `npm i` (install Node.js packages), but the `npm` command is not found on the system. This is likely due to Node.js/npm not being installed or not available in the environment's PATH.

**Recommended Fix:**  
- Install Node.js and npm on the system running the tests.  
- Alternatively, if Node.js is not required, modify the project files to remove or conditionally skip the `npm i` build commands.

    [Fact]
    public async Task GetAsync_WithNonExistentSlug_ReturnsNullOrThrowsException()
    {
        // Arrange
        var nonExistentSlug = "non-existent-slug";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        var mapperMock = new Mock<IMapper>();
    
        using (var context = new AppDbContext(options))
        {
            var postProvider = new PostProvider(mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => postProvider.GetAsync(nonExistentSlug));
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the build process attempted to execute `npm i` (install Node.js packages), but the `npm` command is not found on the system. This is likely due to Node.js/npm not being installed or not available in the environment's PATH.

**Recommended Fix:**  
- Install Node.js and npm on the system running the tests.  
- Alternatively, if Node.js is not required, modify the project files to remove or conditionally skip the `npm i` build commands.

    [Fact]
    public async Task GetAsync_WithDuplicateSlugs_ReturnsFirstMatch()
    {
        // Arrange
        var userId = 1;
        var slug = "duplicate-slug";
    
        using (var context = new AppDbContext(_options))
        {
            // Add two posts with the same slug
            var post1 = new Post { Id = 1, Title = "Post 1", Slug = slug, UserId = userId, State = PostState.Release };
            var post2 = new Post { Id = 2, Title = "Post 2", Slug = slug, UserId = userId, State = PostState.Release };
            context.Posts.Add(post1);
            context.Posts.Add(post2);
            await context.SaveChangesAsync();
    
            var postToHtmlDto1 = new PostToHtmlDto { Id = 1, Title = "Post 1", Slug = slug, Views = 0 };
            var postToHtmlDto2 = new PostToHtmlDto { Id = 2, Title = "Post 2", Slug = slug, Views = 0 };
    
            _mapperMock.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                .Returns(new List<PostToHtmlDto> { postToHtmlDto1, postToHtmlDto2 }.AsQueryable().BuildMock());
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(postToHtmlDto1.Id, result.Post.Id);
        }
    }

*/
/*
FAILED TEST: The test run failed because the build process attempted to execute `npm i` (install Node.js packages), but the `npm` command is not found on the system. This is likely due to Node.js/npm not being installed or not available in the environment's PATH.

**Recommended Fix:**
- Install Node.js and npm on the system running the tests.
- Alternatively, if Node.js is not required, modify the project files to remove or conditionally skip the `npm i` build commands.

    [Fact]
    public async Task FirstAsync_WithNonExistentPostId_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var nonExistentId = 999999999;
    
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => postProvider.FirstAsync(nonExistentId));
        }
    }

*/
        }
    }
}
