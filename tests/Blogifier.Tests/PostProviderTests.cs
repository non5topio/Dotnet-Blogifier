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
FAILED TEST: **Short Analysis:**

1. **Misspelled Exception Type**: `BlogNotIitializeException` is misspelled in `PostProvider.cs`.  
   - **Fix**: Correct to `BlogNotInitializeException`.

2. **Async EF Core In-Memory Query Issue**: `GetPostsAsync_PageSizeZero_ReturnsEmptyResults` failed due to calling `ToListAsync()` on a non-materialized `IQueryable<PostItemDto>` in an in-memory EF Core context.  
   - **Fix**: Replace with `ToList()` if async is not required, or use `AsAsyncEnumerable().ToListAsync()`.

3. **Duplicate Using Directives**: Duplicate `using` statements in `PostProviderTests.cs` cause compiler warnings.  
   - **Fix**: Remove duplicates to clean up the code.

4. **Private Method Access**: Test tries to access private method `CheckPostCategories`.  
   - **Fix**: Make it `internal` and use `[InternalsVisibleTo]` for the test assembly, or refactor into a helper method.

5. **Syntax and Code Structure Issues**: Malformed code in `PostProviderTests.cs` (e.g., incomplete method declarations, nested `using` blocks).  
   - **Fix**: Clean up and complete the test code structure.

6. **Enum Definition Issue**: `PostState.Published` is not defined.  
   - **Fix**: Use the correct enum value (e.g., `PostState.Release` or define `Published` if intended).

    [Fact]
    public async Task StateInternalAsynct_MultiplePosts_UpdatesState()
    {
        // Arrange
        var ids = new List<int> { 1, 2, 3 };
        var state = PostState.Published;
        var posts = ids.Select(id => new Post
        {
            Id = id,
            Title = $"Post {id}",
            Slug = $"post-{id}",
            UserId = 1,
            State = PostState.Draft
        }).ToList();
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            await postProvider.StateInternalAsynct(context.Posts.Where(p => ids.Contains(p.Id)), state);
    
            // Assert
            var updatedPosts = await context.Posts.Where(p => ids.Contains(p.Id)).ToListAsync();
            foreach (var post in updatedPosts)
            {
                Assert.Equal(state, post.State);
            }
        }
    }

*/
/*
FAILED TEST: **Short Analysis:**

The test `GetPostsAsync_PageSizeZero_ReturnsEmptyResults` failed because it attempted to call `ToListAsync()` on an `IQueryable<PostItemDto>` that was not materialized as an `IAsyncEnumerable<PostItemDto>`. This is not supported in EF Core when using in-memory databases, especially when AutoMapper's `ProjectTo<T>()` is involved.

**Recommended Fix:**

Replace `ToListAsync()` with `ToList()` if asynchronous behavior is not required, or ensure the query is materialized correctly by using `AsAsyncEnumerable()` before calling `ToListAsync()`. Alternatively, mock or pre-populate the query results to avoid this EF Core limitation in in-memory testing.

    [Fact]
    public async Task GetPostsAsync_PageSizeZero_ReturnsEmptyResults()
    {
        // Arrange
        var page = 1;
        var pageSize = 0;
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetPostsAsync(page, pageSize);
    
            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }
    }

*/
/*
FAILED TEST: **Short Analysis:**

1. **Misspelled Exception Type**:  
   - **Error**: `BlogNotIitializeException` is misspelled.  
   - **Fix**: Correct to `BlogNotInitializeException`.

2. **Missing Method in Test Context**:  
   - **Error**: The test calls `GetSlugFromTitle`, which is not accessible as it is private in `PostProvider`.  
   - **Fix**: Make it `internal` and use `[InternalsVisibleTo]` for the test assembly, or refactor into a helper method that can be tested separately.

3. **Duplicate Using Directives**:  
   - **Error**: Duplicate `using (var context = new AppDbContext(_options))` in test setup.  
   - **Fix**: Remove the duplicate to eliminate compiler warnings and ensure clean context setup.

4. **Async EF Core In-Memory Query Issue**:  
   - **Error**: In-memory EF Core does not support `ToListAsync()` on non-materialized queries.  
   - **Fix**: Use `ToList()` if async is not required, or ensure queries are materialized with `.ToListAsync()` on `IAsyncEnumerable`. Alternatively, mock query results.

5. **Syntax and Code Structure Issues**:  
   - **Error**: Malformed code (e.g., incomplete method declarations, nested `using` blocks).  
   - **Fix**: Clean up and complete the test code structure.

    [Fact]
    public async Task GetSlugFromTitle_MaxAttemptsExceeded_ThrowsException()
    {
        // Arrange
        var title = "Existing Title";
        var baseSlug = "existing-title";
        var postProvider = new PostProvider(_mapperMock.Object, new AppDbContext(_options));
    
        using (var context = new AppDbContext(_options))
        {
            for (int i = 1; i <= 100; i++)
            {
                context.Posts.Add(new Post
                {
                    Title = title,
                    Slug = $"{baseSlug}-{i}",
                    UserId = 1
                });
            }
            await context.SaveChangesAsync();
    
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotInitializeException>(() => postProvider.GetSlugFromTitle(title));
        }
    }

*/
/*
FAILED TEST: **Short Analysis:**

The test run failed due to the following issues:

1. **Misspelled Exception Type**:  
   - **Error**: `BlogNotIitializeException` is misspelled.  
   - **Fix**: Correct to `BlogNotInitializeException`.

2. **Async EF Core In-Memory Query Issue**:  
   - **Error**: In-memory EF Core does not support `ToListAsync()` on non-materialized queries.  
   - **Fix**: Use `ToList()` if async is not required, or ensure queries are materialized with `.ToListAsync()` on `IAsyncEnumerable`.

3. **Private Method Access**:  
   - **Error**: Test tries to access private method `CheckPostCategories`.  
   - **Fix**: Make it `internal` and use `[InternalsVisibleTo]` for test assembly, or refactor into a testable helper.

4. **Duplicate Using Directives**:  
   - **Error**: Duplicate `using` statements in `PostProviderTests.cs`.  
   - **Fix**: Remove duplicates to eliminate compiler warnings.

5. **Syntax and Code Structure Issues**:  
   - **Error**: Malformed code (e.g., incomplete method declarations, nested `using` blocks).  
   - **Fix**: Clean up and complete the test code structure.

    [Fact]
    public async Task GetAsync_DuplicateRelatedPosts_FilteredCorrectly()
    {
        // Arrange
        var slug = "post-with-duplicate-related";
        var post = new Post
        {
            Id = 1,
            Title = "Main Post",
            Slug = slug,
            Content = "Main content",
            UserId = 1,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow
        };
        var olderPost = new Post
        {
            Id = 2,
            Title = "Older Post",
            Slug = "older-post",
            Content = "Older content",
            UserId = 1,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow.AddDays(1)
        };
        var newerPost = new Post
        {
            Id = 3,
            Title = "Newer Post",
            Slug = "newer-post",
            Content = "Newer content",
            UserId = 1,
            State = PostState.Release,
            PublishedAt = DateTime.UtcNow.AddDays(-1)
        };
        var relatedPost1 = new Post
        {
            Id = 4,
            Title = "Related Post 1",
            Slug = "related-post-1",
            Content = "Related content 1",
            UserId = 1,
            State = PostState.Featured
        };
        var relatedPost2 = new Post
        {
            Id = 5,
            Title = "Related Post 2",
            Slug = "related-post-2",
            Content = "Related content 2",
            UserId = 1,
            State = PostState.Featured
        };
    
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post);
            context.Posts.Add(olderPost);
            context.Posts.Add(newerPost);
            context.Posts.Add(relatedPost1);
            context.Posts.Add(relatedPost2);
            await context.SaveChangesAsync();
    
            var postProvider = new PostProvider(_mapperMock.Object, context);
    
            // Act
            var result = await postProvider.GetAsync(slug);
    
            // Assert
            Assert.NotNull(result.Post);
            Assert.Equal(olderPost.Id, result.Older?.Id);
            Assert.Equal(newerPost.Id, result.Newer?.Id);
            Assert.Equal(2, result.Related.Count);
            Assert.DoesNotContain(result.Related, p => p.Id == olderPost.Id);
            Assert.DoesNotContain(result.Related, p => p.Id == newerPost.Id);
        }
    }

*/
/*
FAILED TEST: **Short Analysis:**
The test `GetByCategoryAsync_NonExistentCategory_ReturnsEmptyResults` failed because the in-memory EF Core database does not support `ToListAsync()` on queries that are not `IAsyncEnumerable`, which is required for async operations.

**Recommended Fix:**
Modify the query in `GetByCategoryAsync` to ensure it is materialized properly, or switch to `ToList()` if async behavior is not required. Alternatively, configure the in-memory database to support async operations or mock the query results.

    [Fact]
    public async Task GetByCategoryAsync_NonExistentCategory_ReturnsEmptyResults()
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
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }
    }

*/
/*
FAILED TEST: **Short Analysis:**

The test `GetSearchAsync_NoMatchingPosts_ReturnsEmptyResults` failed because the in-memory EF Core database does not support `ToListAsync()` on queries that are not `IAsyncEnumerable`, which is required for async operations. This is due to the use of in-memory provider with a query that is not properly materialized.

**Recommended Fix:**

Modify the test to use `ToListAsync()` with a properly materialized query or switch to `ToList()` if the async behavior is not required. Alternatively, ensure the in-memory database setup supports async operations by using a compatible configuration or mocking the query results.

    [Fact]
    public async Task GetSearchAsync_NoMatchingPosts_ReturnsEmptyResults()
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
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }
    }

*/
/*
FAILED TEST: **Analysis:**

The test run failed due to the following issues:

1. **Misspelled Exception Type**:  
   - **Error**: `BlogNotIitializeException` is misspelled.  
   - **Fix**: Correct to `BlogNotInitializeException`.

2. **Missing Method in Test Context**:  
   - **Error**: The test calls `CheckPostCategories`, which is not accessible as it is private in `PostProvider`.  
   - **Fix**: Either make `CheckPostCategories` internal and use `InternalsVisibleTo` for tests, or refactor it into a helper class that can be tested separately.

3. **Async Query Provider Exception (In-Memory EF Core)**:  
   - **Error**: In-memory EF Core does not support `FirstAsync()` in some contexts.  
   - **Fix**: Replace `FirstAsync()` with `FirstOrDefaultAsync()` and add null checks where needed.

4. **Duplicate Using Directives**:  
   - **Error**: Duplicate `using` directives in `PostProviderTests.cs`.  
   - **Fix**: Remove duplicate `using` statements to eliminate compiler warnings.

5. **Test Code Syntax Issues**:  
   - **Error**: The test file has incomplete or malformed code (e.g., nested `using` blocks, incomplete method declarations).  
   - **Fix**: Clean up and complete the test code structure.

    [Fact]
    public async Task CheckPostCategories_EmptyInput_ReturnsNull()
    {
        // Arrange
        using (var context = new AppDbContext(_options))
        {
            var postProvider = new PostProvider(_mapperMock.Object, context);
            
            // Act
            var result = await postProvider.CheckPostCategories(null);
            
            // Assert
            Assert.Null(result);
        }
    }

*/
/*
FAILED TEST: The test run failed due to two main issues:

1. **Misspelled Exception Type**:
   - **Error**: The test references `BlogNotIitializeException`, which is misspelled.
   - **Fix**: Correct the spelling to `BlogNotInitializeException`.

2. **Missing Method in Test**:
   - **Error**: The test calls `GetSlugFromTitle`, but this method is not accessible in the test context.
   - **Fix**: Either make `GetSlugFromTitle` public or create a wrapper/testable version to be used in tests.

3. **Async Query Provider Exception**:
   - **Error**: In-memory EF Core does not support certain async operations unless properly configured.
   - **Fix**: Replace `FirstAsync()` with `FirstOrDefaultAsync()` and add null checks to handle non-existent slugs gracefully.

    [Fact]
    public async Task GetSlugFromTitle_TitleExists_GeneratesUniqueSlug()
    {
        // Arrange
        var title = "Existing Title";
        var existingPost = new Post
        {
            Title = title,
            Slug = "existing-title",
            UserId = 1
        };
        
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(existingPost);
            await context.SaveChangesAsync();
            
            var postProvider = new PostProvider(_mapperMock.Object, context);
            
            // Act
            var result = await postProvider.GetSlugFromTitle(title);
            
            // Assert
            Assert.Equal("existing-title-1", result);
        }
    }

*/
/*
FAILED TEST: The test run failed because the test file `PostProviderTests.cs` references `BlogNotIitializeException`, but this exception type is misspelled and not recognized by the compiler.

**Recommended Fix:**
Correct the misspelling of `BlogNotIitializeException` to `BlogNotInitializeException` in the test file to match the actual exception type defined in the codebase.

    [Fact]
    public async Task UpdateAsync_InvalidUserId_ThrowsBlogNotInitializeException()
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
            UserId = userId,
            State = PostState.Draft,
            PostType = PostType.Post
        };
        
        using (var context = new AppDbContext(_options))
        {
            context.Posts.Add(post);
            await context.SaveChangesAsync();
            
            var postInput = new PostEditorDto
            {
                Id = post.Id,
                Title = "Updated Title",
                Content = "Updated content",
                Slug = "updated-slug"
            };
            
            var postProvider = new PostProvider(_mapperMock.Object, context);
            
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotInitializeException>(() => postProvider.UpdateAsync(postInput, invalidUserId));
        }
    }

*/
/*
FAILED TEST: **Analysis:**

The test `GetAsync_NonExistentSlug_ReturnsNullPostAndNoRelated` failed due to the following error:

> `System.InvalidOperationException: The provider for the source 'IQueryable' doesn't implement 'IAsyncQueryProvider'. Only providers that implement 'IAsyncQueryProvider' can be used for Entity Framework asynchronous operations.`

This error occurs because the test is using an in-memory database (via `UseInMemoryDatabase`) but is calling `FirstAsync` on a query that is not properly configured for async operations in that context. Specifically, the in-memory provider does not support certain EF Core async query operations when the query is not materialized or when the queryable is not correctly set up.

**Recommended Fix:**

Update the test to ensure that the in-memory database context is correctly used with async operations. Replace `FirstAsync()` with `FirstOrDefaultAsync()` and add null checks in the test to simulate non-existent slugs properly. This will avoid the async provider exception and allow the test to run as expected.

    [Fact]
    public async Task GetAsync_NonExistentSlug_ReturnsNullPostAndNoRelated()
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


        
    }


}
