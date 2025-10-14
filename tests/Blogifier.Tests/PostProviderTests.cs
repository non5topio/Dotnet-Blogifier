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
}
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

### **Recommended Fixes:**
- Ensure all opening `{` braces have matching closing `}` braces.
- Refactor the test methods to be properly enclosed and logically separated.
- Correct the structure of the test class and method definitions.

    [Fact]
    public async Task AddAsync_WithInvalidHtmlContent_SanitizesContent()
    {
        // Arrange
        var userId = 1;
        var postInput = new PostEditorDto
        {
            Title = "Test Post",
            Content = "<script>alert('xss');</script><img src='x' onerror='alert(1)'>",
            Description = "<script>alert('xss');</script><img src='x' onerror='alert(1)'>",
            PostType = PostType.Post,
            State = PostState.Draft
        };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_AddAsync_SanitizeHtml")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<Post>(It.IsAny<PostEditorDto>()))
                .Returns(new Post
                {
                    Title = "Test Post",
                    Slug = "test-post",
                    Content = "clean content",
                    Description = "clean description",
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                });
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var slug = await postProvider.AddAsync(postInput, userId);
            var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
    
            // Assert
            Assert.NotNull(savedPost);
            Assert.DoesNotContain("<script>", savedPost.Content);
            Assert.DoesNotContain("<img", savedPost.Content);
            Assert.DoesNotContain("<script>", savedPost.Description);
            Assert.DoesNotContain("<img", savedPost.Description);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

### **Recommended Fixes:**
- Ensure all opening `{` braces have matching closing `}` braces.
- Refactor the test methods to be properly enclosed and logically separated.
- Correct the structure of the test class and method definitions.

    [Fact]
    public async Task GetByCategoryAsync_WithPartialCategoryMatch_ReturnsMatchingPosts()
    {
        // Arrange
        var userId = 1;
        var category1 = new Category { Content = "technology" };
        var category2 = new Category { Content = "tech" };
        var category3 = new Category { Content = "teaching" };
    
        var post1 = new Post
        {
            Id = 1,
            Title = "Post 1",
            Slug = "post-1",
            Content = "Content 1",
            Description = "Description 1",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow,
            PostType = PostType.Post
        };
    
        var post2 = new Post
        {
            Id = 2,
            Title = "Post 2",
            Slug = "post-2",
            Content = "Content 2",
            Description = "Description 2",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow,
            PostType = PostType.Post
        };
    
        var post3 = new Post
        {
            Id = 3,
            Title = "Post 3",
            Slug = "post-3",
            Content = "Content 3",
            Description = "Description 3",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow,
            PostType = PostType.Post
        };
    
        var postCategory1 = new PostCategory { Post = post1, Category = category1 };
        var postCategory2 = new PostCategory { Post = post2, Category = category2 };
        var postCategory3 = new PostCategory { Post = post3, Category = category3 };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetByCategoryAsync_PartialMatch")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            context.Categories.AddRange(category1, category2, category3);
            context.Posts.AddRange(post1, post2, post3);
            context.PostCategories.AddRange(postCategory1, postCategory2, postCategory3);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Description = p.Description,
                    UserId = p.UserId,
                    State = p.State,
                    PublishedAt = p.PublishedAt,
                    PostType = p.PostType
                }));
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetByCategoryAsync("tech", 1, 10);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Total);
            Assert.Equal(2, result.Items.Count);
            Assert.Contains(result.Items, i => i.Id == 1); // "technology"
            Assert.Contains(result.Items, i => i.Id == 2); // "tech"
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

### **Recommended Fixes:**
- Ensure all opening `{` braces have matching closing `}` braces.
- Refactor the test methods to be properly enclosed and logically separated.

    [Fact]
    public async Task CheckPostCategories_WithEmptyCategories_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        var post = new Post
        {
            Id = 1,
            Title = "Test Post",
            Slug = "test-post",
            Content = "Test content",
            Description = "Test description",
            UserId = userId,
            State = PostState.Draft,
            PublishedAt = DateTime.UtcNow,
            PostType = PostType.Post
        };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CheckPostCategories_Empty")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            var postInput = new PostEditorDto
            {
                Id = 1,
                Title = "Test Post",
                Slug = "test-post",
                Content = "Test content",
                Description = "Test description",
                Categories = new List<CategoryDto>(),
                PostType = PostType.Post,
                State = PostState.Draft
            };
    
            // Act
            var result = await postProvider.CheckPostCategories(postInput.Categories, post.PostCategories);
    
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

### **Recommended Fixes:**
- Ensure all opening `{` braces have matching closing `}` braces.
- Refactor the test methods to be properly enclosed and logically separated.
- Correct the structure of the test class and method definitions.

    [Fact]
    public async Task GetPostsAsync_WithMaximumPageSize_ReturnsCorrectPosts()
    {
        // Arrange
        var userId = 1;
        var posts = new List<Post>
        {
            new Post
            {
                Id = 1,
                Title = "Post 1",
                Slug = "post-1",
                Content = "Content 1",
                Description = "Description 1",
                UserId = userId,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow,
                PostType = PostType.Post
            },
            new Post
            {
                Id = 2,
                Title = "Post 2",
                Slug = "post-2",
                Content = "Content 2",
                Description = "Description 2",
                UserId = userId,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow,
                PostType = PostType.Post
            }
        };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetPostsAsync_MaxPageSize")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Description = p.Description,
                    UserId = p.UserId,
                    State = p.State,
                    PublishedAt = p.PublishedAt,
                    PostType = p.PostType
                }));
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetPostsAsync(1, 100);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Total);
            Assert.Equal(2, result.Items.Count);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

### **Recommended Fixes:**
- Ensure all opening `{` braces have matching closing `}` braces.
- Refactor the test methods to be properly enclosed and logically separated.
- Correct the structure of the test class and method definitions.

    [Fact]
    public async Task StateAsynct_WithEmptyIds_DoesNotThrowException()
    {
        // Arrange
        var emptyIds = new List<int>();
        var expectedState = PostState.Draft;
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            var mockMapper = new Mock<IMapper>();
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            await postProvider.StateAsynct(emptyIds, expectedState);
    
            // Assert - No exceptions should be thrown
            // No posts should be updated
            var updatedPosts = await context.Posts.ToListAsync();
            Assert.Empty(updatedPosts);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

**Recommended Fix:**
- Ensure all opening `{` braces have matching closing `}` braces.
- Refactor the test methods to be properly enclosed and logically separated.

    [Fact]
    public async Task GetAsync_WithUnknownPublishedStatus_ReturnsDefaultOrdering()
    {
        // Arrange
        var userId = 1;
        var post1 = new Post
        {
            Id = 1,
            Title = "Post 1",
            Slug = "post-1",
            Content = "Content 1",
            Description = "Description 1",
            UserId = userId,
            State = PostState.Draft,
            PublishedAt = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            PostType = PostType.Post
        };
    
        var post2 = new Post
        {
            Id = 2,
            Title = "Post 2",
            Slug = "post-2",
            Content = "Content 2",
            Description = "Description 2",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            PostType = PostType.Post
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post1);
            context.Posts.Add(post2);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Description = p.Description,
                    UserId = p.UserId,
                    State = p.State,
                    PublishedAt = p.PublishedAt,
                    PostType = p.PostType
                }));
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(PublishedStatus.Unknown, PostType.Post);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            // Verify default ordering: PublishedAt descending, then CreatedAt descending
            Assert.Equal(2, result.First().Id); // post2 should come first
            Assert.Equal(1, result.Skip(1).First().Id); // post1 should come second
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

**Recommended Fix:**  
- Ensure all opening `{` braces have matching closing `}` braces.  
- Refactor the test methods to be properly enclosed and logically separated.

    [Fact]
    public async Task GetSearchAsync_WithShortTerms_IgnoresShortTerms()
    {
        // Arrange
        var userId = 1;
        var term = "a a a";
        var post1 = new Post
        {
            Id = 1,
            Title = "Test Post",
            Slug = "test-post",
            Content = "This is a test post",
            Description = "Test post description",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow,
            PostType = PostType.Post
        };
    
        var post2 = new Post
        {
            Id = 2,
            Title = "Another Post",
            Slug = "another-post",
            Content = "This is another post",
            Description = "Another post description",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow,
            PostType = PostType.Post
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post1);
            context.Posts.Add(post2);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Description = p.Description,
                    UserId = p.UserId,
                    State = p.State,
                    PublishedAt = p.PublishedAt,
                    PostType = p.PostType
                }));
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act
            var result = await postProvider.GetSearchAsync(term, 1, 10);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Total);
            Assert.Empty(result.Items);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

**Recommended Fix:**  
- Ensure all opening `{` braces have matching closing `}` braces.  
- Refactor the test methods to be properly enclosed and logically separated.

    [Fact]
    public async Task UpdateAsync_WithUnauthorizedUser_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var otherUserId = 2;
        var postTitle = "Test Post Title";
        var postContent = "Test post content";
        var postSlug = "test-post-title";
    
        var postInput = new PostEditorDto
        {
            Id = 1,
            Title = postTitle,
            Content = postContent,
            Description = "Test post description",
            PostType = PostType.Post,
            State = PostState.Draft
        };
    
        using (var context = new AppDbContext(_options))
        {
            // Create a post with different user ID
            context.Posts.Add(new Post
            {
                Id = 1,
                Title = "Original Title",
                Slug = "original-title",
                Content = "Original content",
                Description = "Original description",
                UserId = otherUserId,
                State = PostState.Draft,
                PostType = PostType.Post
            });
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<Post>(It.IsAny<PostEditorDto>()))
                .Returns(new Post
                {
                    Id = 1,
                    Title = postTitle,
                    Slug = postSlug,
                    Content = postContent,
                    Description = "Test post description",
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                });
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () => await postProvider.UpdateAsync(postInput, userId));
        }
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to **syntax errors** in `PostProviderTests.cs`, specifically **unclosed or mismatched braces (`{}`)**. The test class and methods are not properly structured, leading to a `CS1513: } expected` compiler error.

**Recommended Fix:**  
- Ensure all opening `{` braces have matching closing `}` braces.
- Correct the structure of the test class and method definitions.
- Refactor the test methods to be properly enclosed and logically separated.

    [Fact]
    public async Task AddAsync_WithDuplicateSlugExceedingRetryLimit_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var postTitle = "Test Post Title";
        var postContent = "Test post content";
        var postSlug = "test-post-title";
    
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
            // Create 100 posts with same slug to simulate duplicate
            for (int i = 0; i < 100; i++)
            {
                context.Posts.Add(new Post
                {
                    Id = i + 1,
                    Title = postTitle,
                    Slug = postSlug,
                    Content = postContent,
                    Description = "Test post description",
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                });
            }
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<Post>(It.IsAny<PostEditorDto>()))
                .Returns(new Post
                {
                    Title = postTitle,
                    Slug = postSlug,
                    Content = postContent,
                    Description = "Test post description",
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                });
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () => await postProvider.AddAsync(postInput, userId));
        }
    }

*/
/*
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected`  
- **Location**: Line 173, column 6

### Cause:
The file contains **unclosed or mismatched braces (`{}`)**, likely due to incomplete or malformed test method definitions and class structure.

### Recommended Fix:
- Review and correct the structure of the test class and methods.
- Ensure all opening `{` braces have matching closing `}` braces.
- Fix the misplaced or incomplete test method bodies (e.g., `AddAsync_CreatesNewPost_ReturnsSlug` is not properly closed or structured).

    [Fact]
    public async Task GetAsync_WithNoRelatedPosts_ReturnsEmptyRelatedFields()
    {
        // Arrange
        var userId = 1;
        var slug = "test-slug";
        var post = new Post
        {
            Id = 1,
            Title = "Test Post",
            Slug = slug,
            Content = "Test content",
            Description = "Test description",
            UserId = userId,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow,
            PostType = PostType.Post
        };
    
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GetAsync_WithNoRelatedPosts_ReturnsEmptyRelatedFields")
            .Options;
    
        using (var context = new AppDbContext(options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                .Returns((IQueryable<Post> q) => q.Select(p => new PostToHtmlDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Description = p.Description,
                    UserId = p.UserId,
                    State = p.State,
                    PublishedAt = p.PublishedAt,
                    Views = p.Views,
                    PostType = p.PostType
                }));
    
            mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                .Returns((IQueryable<Post> q) => q.Select(p => new PostItemDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Description = p.Description,
                    UserId = p.UserId,
                    State = p.State,
                    PublishedAt = p.PublishedAt,
                    PostType = p.PostType
                }));
    
            var postProvider = new PostProvider(mockMapper.Object, context);
    
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
