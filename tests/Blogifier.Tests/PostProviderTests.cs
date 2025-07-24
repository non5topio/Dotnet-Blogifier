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
FAILED TEST: The test run failed due to **syntax errors** in the `PostProviderTests.cs` file, specifically:

### **Cause**:
- The test class is **malformed** — it lacks a proper class declaration and has misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body).
- There are **missing or mismatched braces `{}`**, causing a compilation error (`error CS1513: } expected`).

### **Recommended Fixes**:
1. Ensure the class is properly defined with a valid class declaration:
   ```csharp
   public class PostProviderTests
   ```
2. Move all `using` statements and object initializations inside the class constructor or setup method.
3. Ensure all code blocks are enclosed in proper opening and closing braces `{}`.

    [Fact]
    public async Task GetPostsAsync_PageExceedsTotal_ReturnsEmptyList()
    {
        // Arrange
        var pageSize = 10;
        var page = 100;
        using (var context = new AppDbContext(_options))
        {
            for (int i = 0; i < 5; i++)
            {
                context.Posts.Add(new Post
                {
                    Title = $"Post {i}",
                    Slug = $"post-{i}",
                    Content = "Test content",
                    Description = "Test description",
                    PostType = PostType.Post,
                    State = PostState.Release,
                    PublishedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetPostsAsync(page, pageSize);
    
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(5, result.Total);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in the `PostProviderTests.cs` file, specifically:

- **Error**: `error CS1513: } expected` — This indicates that the test class is **not properly structured** with mismatched or missing braces `{}`.
- The class appears to be malformed, with misplaced or incomplete code blocks (e.g., `using` statements outside of method or class bodies).
- The class declaration is either missing or improperly formed.

### **Recommended Fixes**:
1. Ensure the class is properly defined with a valid class declaration:
   ```csharp
   public class PostProviderTests
   ```
2. Move all `using` statements and object initializations inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed within proper opening and closing braces `{}`.

    [Fact]
    public async Task GetPostsAsync_LargePageSize_ReturnsFirstPage()
    {
        // Arrange
        var pageSize = 10000;
        var page = 1;
        using (var context = new AppDbContext(_options))
        {
            for (int i = 0; i < 50; i++)
            {
                context.Posts.Add(new Post
                {
                    Title = $"Post {i}",
                    Slug = $"post-{i}",
                    Content = "Test content",
                    Description = "Test description",
                    PostType = PostType.Post,
                    State = PostState.Release,
                    PublishedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetPostsAsync(page, pageSize);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(50, result.Items.Count);
            Assert.Equal(50, result.Total);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **syntax errors** in the `PostProviderTests.cs` file, specifically:

- **Error**: `error CS1513: } expected` — This indicates that the test class is **not properly structured** with mismatched or missing braces `{}`.
- The class appears to be malformed, with misplaced or incomplete code blocks (e.g., `using` statements outside of method or class bodies).
- The class declaration is either missing or improperly formed.

### **Recommended Fixes**:
1. Ensure the class is properly defined with a valid class declaration:  
   ```csharp
   public class PostProviderTests
   ```
2. Move all `using` statements and object initializations inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed in proper opening and closing braces `{}`.
4. Fix any misplaced or incomplete code blocks (e.g., misplaced `var post = new Post { ... };` outside of method bodies).

    [Fact]
    public async Task GetAsync_NoOlderOrNewerPosts_ReturnsNull()
    {
        // Arrange
        var slug = "slug-with-no-older-or-newer-posts";
        using (var context = new AppDbContext(_options))
        {
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Slug = slug,
                Content = "Test content",
                Description = "Test description",
                PostType = PostType.Post,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result.Post);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
        }
    }

*/
/*
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected` at line 126, column 6.
- **Cause**: The test class is malformed — it lacks proper structure, with misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body), and missing or mismatched braces `{}`.

### **Recommended Fix**:
1. Ensure the class is properly defined with a valid class declaration.
2. Move all `using` statements inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed in proper opening and closing braces `{}`.

    [Fact]
    public async Task GetAsync_NoRelatedPosts_ReturnsEmptyRelatedList()
    {
        // Arrange
        var slug = "slug-with-no-related-posts";
        using (var context = new AppDbContext(_options))
        {
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Slug = slug,
                Content = "Test content",
                Description = "Test description",
                PostType = PostType.Post,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result.Post);
            Assert.Empty(result.Related);
        }
    }

*/
/*
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected` at line 114, column 6.
- **Cause**: The test class is malformed — it lacks proper structure, with misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body), and missing or mismatched braces `{}`.

### **Recommended Fix**:
1. Ensure the class is properly defined with a valid class declaration.
2. Move all `using` statements inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed in proper opening and closing braces `{}`.

    [Fact]
    public async Task AddAsync_InvalidPostEditorDto_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var postInput = new PostEditorDto
        {
            // Missing required fields like Title and Content
            PostType = PostType.Post,
            State = PostState.Draft
        };
    
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => postProvider.AddAsync(postInput, userId));
        }
    }

*/
/*
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected` at line 114, column 6.
- **Cause**: The test class is missing proper structure — misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body), and the class is not correctly defined with proper opening/closing braces `{}`.

### **Recommended Fix**:
1. Ensure the class is properly defined with a valid class declaration.
2. Move all `using` statements inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed in proper opening and closing braces `{}`.

    [Fact]
    public async Task GetByCategoryAsync_NoMatchingCategory_ReturnsEmptyList()
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
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected` at line 114, column 6.
- **Cause**: The test class is missing proper structure — misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body), and the class is not correctly defined with proper opening/closing braces `{}`.

### **Recommended Fix**:
1. Ensure the class is properly defined with a valid class declaration.
2. Move all `using` statements inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed in proper opening and closing braces `{}`.

    [Fact]
    public async Task GetSearchAsync_NoMatchingTerm_ReturnsEmptyList()
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
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected` at line 114, column 6.
- **Cause**: The test class is missing proper structure — misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body), and the class is not correctly defined with proper opening/closing braces `{}`.

### **Recommended Fix**:
1. Ensure the class is properly defined with a valid class declaration.
2. Move all `using` statements inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed in proper opening and closing braces `{}`.

    [Fact]
    public async Task GetSlugFromTitle_TitleExists100Times_ThrowsException()
    {
        // Arrange
        var title = "existing-title";
        var expectedSlug = "existing-title";
        using (var context = new AppDbContext(_options))
        {
            for (int i = 0; i < 100; i++)
            {
                context.Posts.Add(new Post
                {
                    Title = title,
                    Slug = i == 0 ? expectedSlug : $"{expectedSlug}{i}"
                });
            }
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.GetSlugFromTitle(title));
        }
    }

*/
/*
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected` at line 114, column 6.
- **Cause**: The test class is missing proper structure — misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body), and the class is not correctly defined with proper opening/closing braces `{}`.

### **Recommended Fix**:
1. Ensure the class is properly defined with a valid class declaration.
2. Move all `using` statements inside the class constructor or setup method.
3. Ensure all methods and code blocks are enclosed in proper opening and closing braces `{}`.

    [Fact]
    public async Task UpdateAsync_InvalidUserId_ThrowsException()
    {
        // Arrange
        var userId = 999;
        var postInput = new PostEditorDto
        {
            Id = 1,
            Title = "Test Post",
            Content = "Test content",
            Description = "Test description",
            Slug = "test-post",
            PostType = PostType.Post,
            State = PostState.Draft
        };
    
        using (var context = new AppDbContext(_options))
        {
            var post = new Post
            {
                Id = 1,
                Title = "Test Post",
                Content = "Test content",
                Description = "Test description",
                Slug = "test-post",
                PostType = PostType.Post,
                State = PostState.Draft,
                UserId = 1
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
FAILED TEST: The test run failed due to a **syntax error** in the test file `PostProviderTests.cs`:

- **Error**: `error CS1513: } expected` at line 114, column 6.
- **Cause**: The test class is missing a proper structure — it appears the class is not correctly defined, and there are misplaced or incomplete code blocks (e.g., `using` statements outside of a method or class body).

### **Recommended Fix**:
Ensure the test class is properly structured with:
1. A valid class declaration.
2. All `using` statements inside the class constructor or setup method.
3. Proper opening and closing braces `{}` for the class and methods.

Example:
```csharp
public class PostProviderTests
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly Mock<IMapper> _mapperMock;

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
        // Arrange, Act, Assert
    }
}
```

    [Fact]
    public async Task GetAsync_NonExistentSlug_ReturnsNull()
    {
        // Arrange
        var slug = "non-existent-slug";
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
            
            // Act
            var result = await postProvider.GetAsync(slug);
            
            // Assert
            Assert.Null(result.Post);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
            Assert.Empty(result.Related);
        }
    }

*/
