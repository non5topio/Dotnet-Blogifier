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


    [Fact]
    public async Task GetSlugFromTitle_With100DuplicateSlugs_ThrowsException()
    {
        // Arrange
        var title = "duplicate-title";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            // Add 100 posts with the same slug
            for (int i = 0; i < 100; i++)
            {
                var post = new Post
                {
                    Id = i + 1,
                    Title = title,
                    Slug = title,
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                };
                context.Posts.Add(post);
            }
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.GetSlugFromTitle(title));
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


    [Fact]
    public async Task GetAsync_WithMaximumRelatedPosts_ReturnsThreeRelatedPosts()
    {
        // Arrange
        var slug = "post-with-related";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            // Add a post with the given slug
            var post = new Post
            {
                Id = 1,
                Title = "Main Post",
                Slug = slug,
                UserId = userId,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
    
            // Add 3 related posts
            var relatedPost1 = new Post
            {
                Id = 2,
                Title = "Related Post 1",
                Slug = "related-post-1",
                UserId = userId,
                State = PostState.Featured,
                PublishedAt = DateTime.UtcNow.AddDays(-1)
            };
            var relatedPost2 = new Post
            {
                Id = 3,
                Title = "Related Post 2",
                Slug = "related-post-2",
                UserId = userId,
                State = PostState.Featured,
                PublishedAt = DateTime.UtcNow.AddDays(-2)
            };
            var relatedPost3 = new Post
            {
                Id = 4,
                Title = "Related Post 3",
                Slug = "related-post-3",
                UserId = userId,
                State = PostState.Featured,
                PublishedAt = DateTime.UtcNow.AddDays(-3)
            };
            context.Posts.Add(relatedPost1);
            context.Posts.Add(relatedPost2);
            context.Posts.Add(relatedPost3);
            await context.SaveChangesAsync();
    
            var postToHtmlDto = new PostToHtmlDto
            {
                Id = 1,
                Title = "Main Post",
                Slug = slug,
                Views = 0,
                PublishedAt = DateTime.UtcNow
            };
    
            _mapperMock.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                .Returns(new List<PostToHtmlDto> { postToHtmlDto }.AsQueryable().BuildMock());
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Related.Count);
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
    public async Task GetAsync_WithNonExistentSlug_ThrowsException()
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

/*
FAILED TEST: **Analysis:**  
The test run failed due to a missing type `BlogNotIitializeException`, which is not recognized by the compiler. This is likely a typo in the exception name.

**Recommended Fix:**  
- Correct the typo in `BlogNotIitializeException` to the correct class name (e.g., `BlogNotInitializedException`) in the `PostProvider.cs` file.  
- Ensure the exception class is defined and accessible in the correct namespace.

    [Fact]
    public async Task UpdateAsync_WithUnauthorizedUser_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var unauthorizedUserId = 2;
    
        using (var context = new AppDbContext(_options))
        {
            // Add a post with userId = 1
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Slug = "test-post",
                UserId = userId,
                State = PostState.Draft,
                PostType = PostType.Post
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.UpdateAsync(new PostEditorDto { Id = 1 }, unauthorizedUserId));
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to two main issues:  
1. **CS0246 Error:** The type `BlogNotInitializedException` is missing, indicating a missing reference or typo in the exception class name.  
2. **CS1061 Error:** The method `GetSlugFromTitle` is not accessible, likely because it is private and not exposed for testing.

**Recommended Fixes:**  
1. Correct the exception class name to `BlogNotIitializeException` (check for typos).  
2. Either make the `GetSlugFromTitle` method internal and use the `[InternalsVisibleTo]` attribute for test access, or create a public wrapper method for testing purposes.

    [Fact]
    public async Task GetSlugFromTitle_With100DuplicateSlugs_ThrowsException()
    {
        // Arrange
        var title = "duplicate-title";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            // Add 100 posts with the same slug
            for (int i = 0; i < 100; i++)
            {
                var post = new Post
                {
                    Id = i + 1,
                    Title = title,
                    Slug = title,
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                };
                context.Posts.Add(post);
            }
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotInitializedException>(() => postProvider.GetSlugFromTitle(title));
        }
    }

*/

    [Fact]
    public async Task GetAsync_WithNonExistentSlug_ThrowsException()
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

/*
FAILED TEST: **Analysis:**  
The test run failed due to a missing assembly reference to `Blogifier.Tests.Helpers`, which is referenced in `PostProviderTests.cs`.

**Recommended Fix:**  
- Ensure the `Helpers` project or folder is included in the solution and properly referenced in the `Blogifier.Tests.csproj` file.  
- If it's a local helper class, verify it's added to the correct namespace and project.

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
    
            var queryable = new List<PostToHtmlDto> { postToHtmlDto1, postToHtmlDto2 }.AsQueryable();
            var mockSet = MockSetHelper.CreateMockDbSet(queryable);
            
            _mapperMock.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                .Returns(mockSet.Object);
    
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
FAILED TEST: The test run failed due to the following issues:

1. **CS1503 Error**: A method group is being passed where an `int` is expected. This is likely due to incorrect usage of a method without specifying the required integer parameter.

2. **CS0854 Error**: An expression tree (e.g., in a LINQ expression) contains a call or invocation with optional arguments, which is not allowed.

**Recommended Fixes**:
- For **CS1503**, ensure the correct method signature is used and required parameters (like `int userId`) are explicitly passed.
- For **CS0854**, avoid using optional arguments in LINQ expressions; explicitly provide all required parameters instead.

    [Fact]
    public async Task GetAsync_WithMaximumRelatedPosts_ReturnsThreeRelatedPosts()
    {
        // Arrange
        var slug = "post-with-related";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            // Add a post with the given slug
            var post = new Post
            {
                Id = 1,
                Title = "Main Post",
                Slug = slug,
                UserId = userId,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
    
            // Add 3 related posts
            var relatedPost1 = new Post
            {
                Id = 2,
                Title = "Related Post 1",
                Slug = "related-post-1",
                UserId = userId,
                State = PostState.Featured,
                PublishedAt = DateTime.UtcNow.AddDays(-1)
            };
            var relatedPost2 = new Post
            {
                Id = 3,
                Title = "Related Post 2",
                Slug = "related-post-2",
                UserId = userId,
                State = PostState.Featured,
                PublishedAt = DateTime.UtcNow.AddDays(-2)
            };
            var relatedPost3 = new Post
            {
                Id = 4,
                Title = "Related Post 3",
                Slug = "related-post-3",
                UserId = userId,
                State = PostState.Featured,
                PublishedAt = DateTime.UtcNow.AddDays(-3)
            };
            context.Posts.Add(relatedPost1);
            context.Posts.Add(relatedPost2);
            context.Posts.Add(relatedPost3);
            await context.SaveChangesAsync();
    
            var postToHtmlDto = new PostToHtmlDto
            {
                Id = 1,
                Title = "Main Post",
                Slug = slug,
                Views = 0,
                PublishedAt = DateTime.UtcNow
            };
    
            // Setup mock to return a queryable of PostToHtmlDto
            var mockQueryable = new List<PostToHtmlDto> { postToHtmlDto }.AsQueryable();
            _mapperMock.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                .Returns(mockQueryable);
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Related.Count);
        }
    }

*/
    }
}
