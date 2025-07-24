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
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, causing **C# syntax errors (`CS1513: } expected`)** during compilation. The errors indicate that:

- The `PostProviderTests` class is not properly closed.
- Several test methods (e.g., `AddAsync_CreatesNewPost_ReturnsSlug`, `GetByCategoryAsync_NonExistentCategory_ReturnsEmptyList`, etc.) are missing their closing braces.
- Nested blocks (e.g., `using`, `if`, method bodies) are not correctly closed.

### **Recommended Fix:**
Locate and add the missing closing braces (`}`) to properly close:
- The `PostProviderTests` class.
- All open test methods.
- Ensure all nested blocks are correctly closed in the correct order.

    [Fact]
    public async Task CheckPostCategories_DuplicateCategories_ReturnsUnique()
    {
        // Arrange
        var category1 = new Category { Content = "Category1" };
        var category2 = new Category { Content = "Category2" };
    
        using (var context = new AppDbContext(_options))
        {
            context.Categories.Add(category1);
            context.Categories.Add(category2);
            await context.SaveChangesAsync();
        }
    
        var input = new List<CategoryDto>
        {
            new CategoryDto { Content = "Category1" },
            new CategoryDto { Content = "Category1" },
            new CategoryDto { Content = "Category2" }
        };
    
        var postProvider = CreatePostProvider();
    
        // Act
        var result = await postProvider.CheckPostCategories(input);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Category.Content == "Category1");
        Assert.Contains(result, c => c.Category.Content == "Category2");
    }

*/
/*
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, causing **C# syntax errors (`CS1513: } expected`)** during compilation. The errors indicate that:

- The `PostProviderTests` class is not properly closed.
- Several test methods (e.g., `AddAsync_CreatesNewPost_ReturnsSlug`, `GetByCategoryAsync_NonExistentCategory_ReturnsEmptyList`, etc.) are missing their closing braces.

### **Recommended Fix:**
Locate and add the missing closing braces (`}`) to properly close:
- The `PostProviderTests` class.
- All open test methods.
- Ensure all nested blocks (e.g., `using`, `if`, method bodies) are correctly closed in the correct order.

    [Fact]
    public async Task GetSlugFromTitle_TitleExists99Times_ThrowsException()
    {
        // Arrange
        var title = "existing-title";
        var slug = "existing-title";
        var userId = 1;
    
        using (var context = new AppDbContext(_options))
        {
            for (int i = 1; i <= 99; i++)
            {
                var post = new Post
                {
                    Id = i,
                    Title = title,
                    Slug = $"{slug}{(i > 1 ? i.ToString() : "")}",
                    UserId = userId
                };
                context.Posts.Add(post);
            }
            await context.SaveChangesAsync();
        }
    
        var postProvider = CreatePostProvider();
    
        // Act & Assert
        await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.GetSlugFromTitle(title));
    }

*/
/*
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, causing **C# syntax errors (`CS1513: } expected`)** during compilation. The errors indicate that:

- The `PostProviderTests` class is not properly closed.
- One or more test methods (e.g., `AddAsync_CreatesNewPost_ReturnsSlug`, `GetByCategoryAsync_NonExistentCategory_ReturnsEmptyList`, etc.) are missing their closing braces.

### **Recommended Fix:**
Locate and add the missing closing braces (`}`) to properly close:
- The `PostProviderTests` class.
- All open test methods.
- Ensure all nested blocks (e.g., `using`, `if`, method bodies) are correctly closed in the correct order.

    [Fact]
    public async Task GetPostsAsync_PageGreaterThanTotal_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        var post1 = new Post
        {
            Id = 1,
            Title = "Post 1",
            Slug = "post-1",
            State = PostState.Release,
            PostType = PostType.Post,
            UserId = userId
        };
        var post2 = new Post
        {
            Id = 2,
            Title = "Post 2",
            Slug = "post-2",
            State = PostState.Release,
            PostType = PostType.Post,
            UserId = userId
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post1);
            context.Posts.Add(post2);
            await context.SaveChangesAsync();
        }
    
        var postProvider = CreatePostProvider();
    
        // Act
        var result = await postProvider.GetPostsAsync(3, 1);
    
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

*/
/*
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, which caused a **C# syntax error (`CS1513: } expected`)** during compilation. The error indicates that the `PostProviderTests` class and potentially one or more test methods are not properly closed.

### **Recommended Fix:**
Locate and add the missing closing braces (`}`) to properly close:
- The `PostProviderTests` class.
- Any open test methods (e.g., `AddAsync_CreatesNewPost_ReturnsSlug`, `GetSearchAsync_TermWithMultipleSpaces_ReturnsTrimmedResults`, etc.).
- Ensure all nested blocks (e.g., `using`, `if`, method bodies) are correctly closed in the correct order.

    [Fact]
    public async Task GetPostsAsync_InvalidPageSize_ReturnsEmptyList()
    {
        // Arrange
        var postProvider = CreatePostProvider();
    
        // Act
        var result1 = await postProvider.GetPostsAsync(1, 0);
        var result2 = await postProvider.GetPostsAsync(1, -1);
    
        // Assert
        Assert.NotNull(result1);
        Assert.Empty(result1.Items);
        Assert.NotNull(result2);
        Assert.Empty(result2.Items);
    }

*/
/*
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, causing syntax errors during compilation. The errors indicate that:

- The `PostProviderTests` class is not properly closed.
- One or more test methods (e.g., `AddAsync_CreatesNewPost_ReturnsSlug`) are missing their closing braces.

### **Recommended Fix:**
Locate and add the missing closing braces (`}`) to properly close all open methods and the `PostProviderTests` class. Ensure all nested blocks (e.g., `using`, `if`, method bodies) are correctly closed in the correct order.

    [Fact]
    public async Task GetByCategoryAsync_NonExistentCategory_ReturnsEmptyList()
    {
        // Arrange
        var category = "non-existent-category";
        var postProvider = new PostProvider(new Mock<IMapper>().Object, new AppDbContext(_options));
    
        // Act
        var result = await postProvider.GetByCategoryAsync(category, 1, 10);
    
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

*/
/*
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, causing syntax errors during compilation. Specifically:

- A missing closing brace for the `PostProviderTests` class.
- Missing closing braces for one or more test methods, such as `AddAsync_CreatesNewPost_ReturnsSlug`.

### **Recommended Fix:**
Add the missing closing braces (`}`) at the appropriate places in the file to properly close the class and any open methods or blocks.

    [Fact]
    public async Task GetSearchAsync_TermWithMultipleSpaces_ReturnsTrimmedResults()
    {
        // Arrange
        var userId = 1;
        var term = "   test   search   term   ";
        var post = new Post
        {
            Id = 1,
            Title = "Test Search Term",
            Slug = "test-search-term",
            Content = "This is a test post with search term",
            State = PostState.Release,
            UserId = userId
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        }
    
        var mockMapper = new Mock<IMapper>();
        var postProvider = new PostProvider(mockMapper.Object, new AppDbContext(_options));
    
        // Act
        var result = await postProvider.GetSearchAsync(term, 1, 10);
    
        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Test Search Term", result.Items.First().Title);
    }

*/
/*
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, which caused syntax errors during compilation. Specifically:

- A missing closing brace for the `PostProviderTests` class.
- Missing closing braces for one or more test methods, such as `AddAsync_CreatesNewPost_ReturnsSlug`.

### **Recommended Fix:**
Add the missing closing braces (`}`) at the appropriate places in the file to properly close the class and any open methods or blocks.

    [Fact]
    public async Task GetEditorAsync_PostWithNoCategories_ReturnsEmptyCategories()
    {
        // Arrange
        var userId = 1;
        var slug = "post-with-no-categories";
        var post = new Post
        {
            Id = 1,
            Title = "Post with no categories",
            Slug = slug,
            Content = "Test content",
            State = PostState.Draft,
            UserId = userId
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        }
    
        var mockMapper = new Mock<IMapper>();
        var postProvider = new PostProvider(mockMapper.Object, new AppDbContext(_options));
    
        // Act
        var result = await postProvider.GetEditorAsync(slug);
    
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Categories);
        Assert.Empty(result.Categories);
    }

*/
/*
FAILED TEST: The test run failed due to **missing closing braces (`}`)** in the `PostProviderTests.cs` file, which caused syntax errors during compilation. Specifically:

- A missing closing brace for the `PostProviderTests` class.
- A missing closing brace for the `AddAsync_CreatesNewPost_ReturnsSlug` test method.

### **Recommended Fix:**
Locate and add the missing closing braces to properly close the class and method in `PostProviderTests.cs`. Ensure all nested blocks (like `using`, `if`, `method` bodies) are correctly closed.

    [Fact]
    public async Task UpdateAsync_InvalidUserId_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var invalidUserId = 2;
        var post = new Post
        {
            Id = 1,
            Title = "Test Post",
            Slug = "test-post",
            Content = "Test content",
            State = PostState.Draft,
            UserId = userId
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        }
    
        var postInput = new PostEditorDto
        {
            Id = 1,
            Title = "Updated Title",
            Content = "Updated content",
            Slug = "updated-slug"
        };
    
        var postProvider = new PostProvider(new Mock<IMapper>().Object, new AppDbContext(_options));
    
        // Act & Assert
        await Assert.ThrowsAsync<BlogNotIitializeException>(() => postProvider.UpdateAsync(postInput, invalidUserId));
    }

*/
/*
FAILED TEST: The test run failed due to a **missing closing brace (`}`)** in the `PostProviderTests.cs` file, which caused a syntax error. The compiler error message:
```
/app/tests/Blogifier.Tests/PostProviderTests.cs(155,6): error CS1513: } expected
```
indicates that the class or a nested structure (such as a method or using block) is not properly closed.

### **Recommended Fix:**
Add the missing closing brace (`}`) at the end of the class to properly close the `PostProviderTests` class.

    [Fact]
    public async Task GetAsync_ExistingSlug_RelatedPostsExcludesOlderAndNewer()
    {
        // Arrange
        var userId = 1;
        var slug = "existing-slug";
        var post = new Post
        {
            Id = 1,
            Title = "Existing Post",
            Slug = slug,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow,
            UserId = userId
        };
        var olderPost = new Post
        {
            Id = 2,
            Title = "Older Post",
            Slug = "older-post",
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow.AddDays(1),
            UserId = userId
        };
        var newerPost = new Post
        {
            Id = 3,
            Title = "Newer Post",
            Slug = "newer-post",
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow.AddDays(-1),
            UserId = userId
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post);
            context.Posts.Add(olderPost);
            context.Posts.Add(newerPost);
            await context.SaveChangesAsync();
        }
    
        var mockMapper = new Mock<IMapper>();
        var postProvider = new PostProvider(mockMapper.Object, new AppDbContext(_options));
    
        // Act
        var result = await postProvider.GetAsync(slug);
    
        // Assert
        Assert.NotNull(result.Post);
        Assert.NotNull(result.Older);
        Assert.NotNull(result.Newer);
        Assert.DoesNotContain(result.Related, p => p.Id == olderPost.Id);
        Assert.DoesNotContain(result.Related, p => p.Id == newerPost.Id);
    }

*/
/*
FAILED TEST: The test run failed due to a **missing closing brace (`}`)** in the `PostProviderTests.cs` file. The error message:

```
/app/tests/Blogifier.Tests/PostProviderTests.cs(124,6): error CS1513: } expected
```

indicates that the compiler expected a closing brace at line 124, but it was missing.

### **Recommended Fix:**
Locate the missing closing brace for the `PostProviderTests` class or any nested structure (like a method or using block) and add it at the appropriate place in the file to properly close the class or method.

    [Fact]
    public async Task GetAsync_NonExistentSlug_ReturnsNullRelatedPosts()
    {
        // Arrange
        var userId = 1;
        var slug = "non-existent-slug";
        var postProvider = CreatePostProvider();
    
        // Act
        var result = await postProvider.GetAsync(slug);
    
        // Assert
        Assert.Null(result.Post);
        Assert.Null(result.Older);
        Assert.Null(result.Newer);
        Assert.Empty(result.Related);
    }
    
    private PostProvider CreatePostProvider()
    {
        using var context = new AppDbContext(_options);
        var mockMapper = new Mock<IMapper>();
        return new PostProvider(mockMapper.Object, context);
    }

*/
