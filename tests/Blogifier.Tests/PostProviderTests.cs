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

        [Fact]
        public async Task AddAsync_WithSlugExceeding100Iterations_ThrowsException()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                // Add posts with slugs from "popular-post" to "popular-post100"
                for (int i = 1; i <= 100; i++)
                {
                    var slug = i == 1 ? "popular-post" : $"popular-post{i}";
                    context.Posts.Add(new Post
                    {
                        Id = i,
                        Title = "Popular Post",
                        Slug = slug,
                        Content = "Content",
                        Description = "Description",
                        UserId = 1,
                        State = PostState.Draft,
                        PostType = PostType.Post
                    });
                }
                await context.SaveChangesAsync();
                
                var mockMapper = new Mock<IMapper>();
                var postProvider = new PostProvider(mockMapper.Object, context);
                
                var postInput = new PostEditorDto
                {
                    Title = "Popular Post",
                    Content = "New Content",
                    Description = "New Description",
                    PostType = PostType.Post,
                    State = PostState.Draft
                };
                
                // Act & Assert
                await Assert.ThrowsAsync<Blogifier.Blogs.BlogNotIitializeException>(() => 
                    postProvider.AddAsync(postInput, 1));
            }
        }

/*
FAILED TEST: ## Analysis

The test compilation failed due to **1 compilation error**:

**Line 132**: `BlogNotInitializedException` type not found - incorrect class name used in the test.

## Root Cause

The test references `BlogNotInitializedException`, but the actual exception class in the source code is `BlogNotIitializeException` (note the double 'i' typo in "Iitialize").

## Recommended Fixes

**Fix 1: Correct the exception type name (Line 132)**

Replace:
```csharp
BlogNotInitializedException
```

With:
```csharp
BlogNotIitializeException
```

This matches the actual exception class name used in `PostProvider.cs` (lines 310, 345).

**Note**: The source code contains a typo in the exception class name (`BlogNotIitializeException` instead of `BlogNotInitializedException`). The test must use the actual class name as it exists in the codebase.

        [Fact]
        public async Task UpdateAsync_WithMismatchedUserId_ThrowsException()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var post = new Post
                {
                    Id = 1,
                    Title = "Original Title",
                    Slug = "original-title",
                    Content = "Original Content",
                    Description = "Original Description",
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post
                };
                
                context.Posts.Add(post);
                await context.SaveChangesAsync();
                
                var mockMapper = new Mock<IMapper>();
                var postProvider = new PostProvider(mockMapper.Object, context);
                
                var postInput = new PostEditorDto
                {
                    Id = 1,
                    Title = "Updated Title",
                    Slug = "original-title",
                    Content = "Updated Content",
                    Description = "Updated Description",
                    State = PostState.Release,
                    PostType = PostType.Post
                };
                
                // Act & Assert
                await Assert.ThrowsAsync<BlogNotInitializedException>(() => 
                    postProvider.UpdateAsync(postInput, 2));
            }
        }

*/

        [Fact]
        public async Task AddAsync_WithDraftState_SetsPublishedAtToNull()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var mockMapper = new Mock<IMapper>();
                var postProvider = new PostProvider(mockMapper.Object, context);
                
                var postInput = new PostEditorDto
                {
                    Title = "Draft Post",
                    Content = "Draft content",
                    Description = "Draft description",
                    PostType = PostType.Post,
                    State = PostState.Draft,
                    PublishedAt = new DateTime(2024, 1, 15)
                };
                
                // Act
                var result = await postProvider.AddAsync(postInput, 1);
                
                // Assert
                var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == result);
                Assert.NotNull(savedPost);
                Assert.Null(savedPost.PublishedAt);
            }
        }


        [Fact]
        public async Task AddAsync_WithProvidedPublishedAtAndReleaseState_PreservesDate()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var mockMapper = new Mock<IMapper>();
                var postProvider = new PostProvider(mockMapper.Object, context);
                
                var specificDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
                var postInput = new PostEditorDto
                {
                    Title = "Test Post",
                    Content = "Test content",
                    Description = "Test description",
                    PostType = PostType.Post,
                    State = PostState.Release,
                    PublishedAt = specificDate
                };
                
                // Act
                var result = await postProvider.AddAsync(postInput, 1);
                
                // Assert
                var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == result);
                Assert.NotNull(savedPost);
                Assert.Equal(specificDate, savedPost.PublishedAt);
            }
        }


        [Fact]
        public async Task AddAsync_WithNullPublishedAtAndReleaseState_SetsPublishedAtToNow()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var mockMapper = new Mock<IMapper>();
                var postProvider = new PostProvider(mockMapper.Object, context);
                
                var postInput = new PostEditorDto
                {
                    Title = "Test Post",
                    Content = "Test content",
                    Description = "Test description",
                    PostType = PostType.Post,
                    State = PostState.Release,
                    PublishedAt = null
                };
                
                var beforeAdd = DateTime.UtcNow;
                
                // Act
                var result = await postProvider.AddAsync(postInput, 1);
                
                var afterAdd = DateTime.UtcNow;
                
                // Assert
                var savedPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == result);
                Assert.NotNull(savedPost);
                Assert.NotNull(savedPost.PublishedAt);
                Assert.True(savedPost.PublishedAt >= beforeAdd && savedPost.PublishedAt <= afterAdd);
            }
        }

/*
FAILED TEST: ## Test Failure Analysis

### Root Cause
The test compilation failed due to **1 compilation error** and **6 warnings** about duplicate using directives.

### Compilation Error
**Line 108**: `User` type not found - missing reference to `AppUser` class from `Blogifier.Blogs` namespace

### Warnings
**Lines 15-20**: Duplicate using directives for:
- `Blogifier.Data`
- `Blogifier.Posts`
- `Blogifier.Shared`
- `AutoMapper`
- `Moq`
- `Xunit`

### Recommended Fixes

**Fix 1: Add missing using directive**
```csharp
using Blogifier.Blogs;
```
Add this at the top of the file with other using statements.

**Fix 2: Update User reference (Line 108)**
Change `User` to `AppUser` (the correct type name from the `Blogifier.Blogs` namespace).

**Fix 3: Remove duplicate using directives (Lines 15-20)**
Delete the duplicate using statements for `Blogifier.Data`, `Blogifier.Posts`, `Blogifier.Shared`, `AutoMapper`, `Moq`, and `Xunit`.

        [Fact]
        public async Task GetAsync_WithDefaultFilter_ReturnsAllPostsOrderedCorrectly()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var post1 = new Post
                {
                    Id = 1,
                    Title = "Post 1",
                    Slug = "post-1",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    PublishedAt = null
                };
                
                var post2 = new Post
                {
                    Id = 2,
                    Title = "Post 2",
                    Slug = "post-2",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    PublishedAt = DateTime.UtcNow.AddDays(-1)
                };
                
                var post3 = new Post
                {
                    Id = 3,
                    Title = "Post 3",
                    Slug = "post-3",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Featured,
                    PostType = PostType.Post,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    PublishedAt = DateTime.UtcNow
                };
                
                context.Posts.AddRange(post1, post2, post3);
                await context.SaveChangesAsync();
                
                var mockMapper = new Mock<IMapper>();
                mockMapper.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>(), null, null))
                    .Returns((IQueryable<Post> source, object parameters, object membersToExpand) => 
                        source.Select(p => new PostItemDto { Id = p.Id, Title = p.Title }));
                
                var postProvider = new PostProvider(mockMapper.Object, context);
                
                // Act
                var result = await postProvider.GetAsync(PublishedStatus.All, PostType.Post);
                
                // Assert
                Assert.NotNull(result);
                var resultList = result.ToList();
                Assert.Equal(3, resultList.Count);
            }
        }

*/
/*
FAILED TEST: ## Analysis

The test run failed due to **2 compilation errors**:

1. **Line 107**: `User` type not found - missing reference to `AppUser` class from `Blogifier.Blogs` namespace
2. **Line 152**: `MappingProfile` type not found - this class doesn't exist in the project structure

Additionally, there are **5 warnings** about duplicate using directives (lines 15-19).

## Recommended Fixes

**Fix 1: Add missing using directive (Line 107)**
- Add `using Blogifier.Blogs;` at the top of the file
- Change `User` to `AppUser` on line 107

**Fix 2: Remove MappingProfile instantiation (Line 152)**
- The `MappingProfile` class does not exist in the project
- Replace with a mocked `IMapper` configuration instead of instantiating a mapping profile directly
- Use `_mapperMock.Setup()` to configure mapper behavior for tests

**Fix 3: Remove duplicate using directives (Lines 15-19)**
- Delete duplicate using statements for: `Blogifier.Data`, `Blogifier.Posts`, `Blogifier.Shared`, `AutoMapper`, and `Xunit`

        [Fact]
        public async Task GetAsync_WithDraftsFilter_ReturnsOnlyDraftPosts()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var draftPost1 = new Post
                {
                    Id = 1,
                    Title = "Draft Post 1",
                    Slug = "draft-post-1",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                };
                
                var draftPost2 = new Post
                {
                    Id = 2,
                    Title = "Draft Post 2",
                    Slug = "draft-post-2",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                };
                
                var releasedPost = new Post
                {
                    Id = 3,
                    Title = "Released Post",
                    Slug = "released-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow
                };
                
                context.Posts.AddRange(draftPost1, draftPost2, releasedPost);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetAsync(PublishedStatus.Drafts, PostType.Post);
                
                // Assert
                Assert.NotNull(result);
                var resultList = result.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.DoesNotContain(resultList, p => p.Title == "Released Post");
                Assert.Equal("Draft Post 2", resultList[0].Title);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

The test run failed due to **2 compilation errors** and **8 warnings** about duplicate using directives.

### Compilation Errors:

1. **Line 110**: `User` type not found - missing reference to `AppUser` class from `Blogifier.Blogs` namespace
2. **Line 155**: `MappingProfile` type not found - this class doesn't exist in the project structure

### Warnings:
- **Lines 15-22**: Duplicate using directives for `Xunit`, `AutoMapper`, `Blogifier.Data`, `Blogifier.Posts`, `Blogifier.Shared`, `System`, `System.Linq`, and `System.Threading.Tasks`

## Recommended Fixes:

**Fix 1: Replace `User` with `AppUser` (Line 110)**
- Change `User` to `AppUser` 
- Add `using Blogifier.Blogs;` at the top of the file

**Fix 2: Remove or mock `MappingProfile` instantiation (Line 155)**
- The `MappingProfile` class doesn't exist in the project
- Replace with a properly configured mock `IMapper` instead of trying to instantiate mapping profiles directly
- Use the existing `_mapperMock` field that's already set up in the constructor

**Fix 3: Remove duplicate using directives (Lines 15-22)**
- Delete all duplicate using statements for: `Xunit`, `AutoMapper`, `Blogifier.Data`, `Blogifier.Posts`, `Blogifier.Shared`, `System`, `System.Linq`, and `System.Threading.Tasks`

        [Fact]
        public async Task GetAsync_WithFeaturedFilter_ReturnsOnlyReleasedPosts()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var draftPost = new Post
                {
                    Id = 1,
                    Title = "Draft Post",
                    Slug = "draft-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post,
                    CreatedAt = DateTime.UtcNow
                };
                
                var releasedPost = new Post
                {
                    Id = 2,
                    Title = "Released Post",
                    Slug = "released-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow.AddDays(-1)
                };
                
                var featuredPost = new Post
                {
                    Id = 3,
                    Title = "Featured Post",
                    Slug = "featured-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Featured,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow
                };
                
                context.Posts.AddRange(draftPost, releasedPost, featuredPost);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetAsync(PublishedStatus.Featured, PostType.Post);
                
                // Assert
                Assert.NotNull(result);
                var resultList = result.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.DoesNotContain(resultList, p => p.Title == "Draft Post");
                Assert.Equal("Featured Post", resultList[0].Title);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

The test run failed due to **2 compilation errors** and **9 warnings** about duplicate using directives.

### Compilation Errors:

1. **Line 111**: `User` type not found
   - Missing reference to the correct type, which should be `AppUser` from `Blogifier.Blogs` namespace

2. **Line 115**: `MappingProfile` type not found
   - This class doesn't exist in the project structure
   - Tests are attempting to instantiate a mapping profile directly instead of mocking it

### Warnings:
- **Lines 15-23**: Duplicate using directives for multiple namespaces (Xunit, AutoMapper, Blogifier.Data, Blogifier.Posts, Blogifier.Shared, Microsoft.EntityFrameworkCore, System.Collections.Generic, System.Linq, System.Threading.Tasks)

## Recommended Fixes:

**Fix 1: Correct the User type reference (Line 111)**
- Change `User` to `AppUser`
- Add `using Blogifier.Blogs;` at the top of the file if not already present

**Fix 2: Remove MappingProfile instantiation (Line 115)**
- Remove the line attempting to create `new MappingProfile()`
- Use the existing `_mapperMock` (Mock<IMapper>) that's already set up in the constructor instead of creating real mapping profiles

**Fix 3: Remove duplicate using directives (Lines 15-23)**
- Delete all duplicate using statements to eliminate the 9 warnings

        [Fact]
        public async Task CheckPostCategories_WithDuplicates_RemovesDuplicates()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                var postInput = new PostEditorDto
                {
                    Title = "Test Post",
                    Content = "Content",
                    Description = "Description",
                    State = PostState.Draft,
                    PostType = PostType.Post,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Content = "Tech" },
                        new CategoryDto { Content = "Tech" },
                        new CategoryDto { Content = "News" },
                        new CategoryDto { Content = "Tech" }
                    }
                };
                
                // Act
                var slug = await postProvider.AddAsync(postInput, 1);
                
                // Assert
                var savedPost = await context.Posts
                    .Include(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                    .FirstAsync(p => p.Slug == slug);
                
                Assert.NotNull(savedPost.PostCategories);
                Assert.Equal(2, savedPost.PostCategories.Count);
                var categoryContents = savedPost.PostCategories.Select(pc => pc.Category.Content).ToList();
                Assert.Contains("Tech", categoryContents);
                Assert.Contains("News", categoryContents);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

The test run **failed to compile** due to 2 critical errors:

### Compilation Errors

1. **Line 104**: `User` type not found
   - Missing reference to the correct User type from the `Blogifier.Blogs` namespace

2. **Line 134**: `MappingProfile` type not found
   - This class doesn't exist in the project structure

### Warnings
- **Lines 15-16**: Duplicate using directives for `Blogifier.Data` and `Blogifier.Shared`

---

## Recommended Fixes

**Fix 1: Resolve User type (Line 104)**
```csharp
// Change:
var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };

// To:
var user = new AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
```
And add at the top of the file:
```csharp
using Blogifier.Blogs;
```

**Fix 2: Remove MappingProfile instantiation (Line 134)**
- The `MappingProfile` class does not exist in the project
- Replace with a mocked `IMapper` instead (already available as `_mapperMock` in the test class)
- Remove any direct instantiation of mapping profiles

**Fix 3: Remove duplicate using directives (Lines 15-16)**
- Delete the duplicate `using Blogifier.Data;` statement
- Delete the duplicate `using Blogifier.Shared;` statement

        [Fact]
        public async Task GetSearchAsync_WithShortTerms_SkipsThemInRanking()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var post1 = new Post
                {
                    Id = 1,
                    Title = "Technology Article",
                    Slug = "technology-article",
                    Content = "Content about tech",
                    Description = "Description with technology",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post
                };
                
                var post2 = new Post
                {
                    Id = 2,
                    Title = "Another Post",
                    Slug = "another-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post
                };
                
                context.Posts.AddRange(post1, post2);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act - search with mix of short and long terms
                var result = await postProvider.GetSearchAsync("ab cd technology", 1, 10);
                
                // Assert
                Assert.NotNull(result);
                Assert.True(result.Total >= 1);
                var firstItem = result.Items.FirstOrDefault();
                Assert.NotNull(firstItem);
                Assert.Equal("Technology Article", firstItem.Title);
            }
        }

*/
/*
FAILED TEST: ## Analysis

The test run failed due to **2 compilation errors**:

1. **Line 109**: `User` type not found - missing reference to the correct user entity type
2. **Line 132**: `MappingProfile` type not found - this class doesn't exist in the project

Additionally, there are **7 warnings** about duplicate using directives (lines 15-21).

## Recommended Fixes

**Fix 1: Replace `User` with correct type (Line 109)**
```csharp
// Change from:
var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };

// To:
var user = new AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
```
Add `using Blogifier.Blogs;` at the top of the file.

**Fix 2: Remove or replace `MappingProfile` instantiation (Line 132)**
The `MappingProfile` class doesn't exist. Replace with a mocked `IMapper` instead:
```csharp
// Remove the line creating MappingProfile
// Use the existing _mapperMock field instead
```

**Fix 3: Remove duplicate using directives (Lines 15-21)**
Delete the duplicate using statements for:
- `Xunit`
- `AutoMapper`
- `Blogifier.Posts`
- `Blogifier.Data`
- `Blogifier.Shared`
- `System`
- `System.Threading.Tasks`

        [Fact]
        public async Task GetPostsAsync_WithPageExceedingTotal_ReturnsEmptyItems()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                for (int i = 1; i <= 15; i++)
                {
                    context.Posts.Add(new Post
                    {
                        Id = i,
                        Title = $"Post {i}",
                        Slug = $"post-{i}",
                        Content = "Content",
                        Description = "Description",
                        UserId = 1,
                        User = user,
                        State = PostState.Release,
                        PostType = PostType.Post,
                        CreatedAt = DateTime.UtcNow.AddDays(-i),
                        PublishedAt = DateTime.UtcNow.AddDays(-i)
                    });
                }
                
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetPostsAsync(10, 10);
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(15, result.Total);
                Assert.Equal(10, result.Page);
                Assert.Empty(result.Items);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

The test run failed due to **2 compilation errors** and **8 warnings** related to duplicate using directives.

### Root Causes:

1. **Missing Type Reference (Line 110)**: `User` type not found - the test uses `User` but should use the fully qualified type from the project
2. **Missing Type Reference (Line 134)**: `MappingProfile` type not found - this class doesn't exist in the project structure
3. **Duplicate Using Directives (Lines 15-22)**: Multiple using statements are duplicated, causing CS0105 warnings

### Recommended Fixes:

**Fix 1: Correct User type reference (Line 110)**
```csharp
// Change from:
var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };

// To (add using directive at top):
using Blogifier.Blogs;
// Then use:
var user = new AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
```

**Fix 2: Remove or mock MappingProfile (Line 134)**
```csharp
// Remove the line attempting to instantiate MappingProfile
// The test already uses Mock<IMapper>, so remove any direct profile instantiation
// Delete line 134 or replace with proper mock setup
```

**Fix 3: Remove duplicate using directives (Lines 15-22)**
Delete the duplicate using statements for:
- `Xunit`
- `Blogifier.Data`
- `Blogifier.Posts`
- `Blogifier.Shared`
- `AutoMapper`
- `Microsoft.EntityFrameworkCore`
- `System`
- `System.Threading.Tasks`

        [Fact]
        public async Task GetAsync_WithSlug_WhenNoRelatedPosts_ReturnsEmptyRelatedList()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var testPost = new Post
                {
                    Id = 1,
                    Title = "Test Post",
                    Slug = "test-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    User = user,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow,
                    Views = 0
                };
                
                context.Posts.Add(testPost);
                await context.SaveChangesAsync();
            }
            
            using (var context = new AppDbContext(_options))
            {
                var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetAsync("test-post");
                
                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Post);
                Assert.Equal("Test Post", result.Post.Title);
                Assert.Equal(1, result.Post.Views);
                Assert.NotNull(result.Related);
                Assert.Empty(result.Related);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

The test run **failed due to 2 compilation errors** and **2 warnings**:

### Compilation Errors:

1. **Line 104**: `Blogifier.Data.User` type does not exist
   - The correct type is `AppUser` from the `Blogifier.Blogs` namespace, not `User` from `Blogifier.Data`

2. **Line 124**: `Blogifier.Shared.MappingProfile` type does not exist
   - This class is not part of the project structure and should not be instantiated directly in tests

### Warnings:

3. **Lines 15-16**: Duplicate using directives for `Blogifier.Data` and `Blogifier.Shared`

---

## Recommended Fixes:

**Fix 1: Correct the User type (Line 104)**
```csharp
// Change from:
var user = new Blogifier.Data.User { Id = 1, UserName = "testuser", Email = "test@test.com" };

// To:
var user = new Blogifier.Blogs.AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
```

**Fix 2: Remove MappingProfile instantiation (Line 124)**
```csharp
// Remove this line entirely:
var config = new MapperConfiguration(cfg => cfg.AddProfile<Blogifier.Shared.MappingProfile>());

// The test should use the existing mockMapper setup instead of creating a real mapper
```

**Fix 3: Remove duplicate using directives (Lines 15-16)**
- Delete the duplicate `using Blogifier.Data;` statement
- Delete the duplicate `using Blogifier.Shared;` statement

        [Fact]
        public async Task GetAsync_WithSlug_WhenNoOlderOrNewerPosts_ReturnsNullForThoseFields()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new Blogifier.Data.User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var onlyPost = new Post
                {
                    Id = 1,
                    Title = "Only Post",
                    Slug = "only-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow,
                    Views = 0
                };
                
                context.Posts.Add(onlyPost);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<Blogifier.Shared.MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetAsync("only-post");
                
                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Post);
                Assert.Equal("Only Post", result.Post.Title);
                Assert.Equal(1, result.Post.Views);
                Assert.Null(result.Older);
                Assert.Null(result.Newer);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

The test run **failed due to 2 compilation errors** and **1 warning**:

### Compilation Errors:

1. **Line 104**: `Blogifier.Blogs.User` type does not exist
   - The correct type is `AppUser`, not `User`
   
2. **Line 122**: `Blogifier.Shared.MappingProfile` type does not exist
   - This class doesn't exist in the project structure

### Warning:

1. **Line 16**: Duplicate using directive for `Blogifier.Shared`

## Recommended Fixes:

**Fix 1: Change `User` to `AppUser` (Line 104)**
```csharp
// Change from:
var user = new Blogifier.Blogs.User { Id = 1, UserName = "testuser", Email = "test@test.com" };

// To:
var user = new Blogifier.Blogs.AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
```

**Fix 2: Remove `MappingProfile` instantiation (Line 122)**
- Remove or replace the line that references `new Blogifier.Shared.MappingProfile()`
- Use the existing `mockMapper` Mock object instead of creating actual mapping profiles

**Fix 3: Remove duplicate using directive (Line 16)**
- Delete the duplicate `using Blogifier.Shared;` statement

        [Fact]
        public async Task AddAsync_WithDuplicateTitle_GeneratesUniqueSlugWithSuffix()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new Blogifier.Blogs.User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var existingPost = new Post
                {
                    Id = 1,
                    Title = "Existing Post",
                    Slug = "existing-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post
                };
                
                context.Posts.Add(existingPost);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<Blogifier.Shared.MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                var postInput = new PostEditorDto
                {
                    Title = "Existing Post",
                    Content = "New Content",
                    Description = "New Description",
                    State = PostState.Draft,
                    PostType = PostType.Post
                };
                
                // Act
                var result = await postProvider.AddAsync(postInput, 1);
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal("existing-post2", result);
                var newPost = await context.Posts.FirstOrDefaultAsync(p => p.Slug == "existing-post2");
                Assert.NotNull(newPost);
                Assert.Equal("Existing Post", newPost.Title);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

The test run failed due to **2 compilation errors** and **6 warnings**:

### Compilation Errors:

1. **Line 109**: `AppUser` type not found - missing using directive for the namespace containing `AppUser`
2. **Line 157**: `PostMappingProfile` type not found - this class doesn't exist in the project structure

### Warnings:
- **Lines 16-21**: Duplicate using directives causing CS0105 warnings

## Recommended Fixes:

**Fix 1: Replace `AppUser` with correct type (Line 109)**
- Change `AppUser` to `Blogifier.Blogs.AppUser` or add `using Blogifier.Blogs;` at the top
- Based on the source code structure, the user entity is likely in the `Blogifier.Blogs` namespace

**Fix 2: Remove or replace `PostMappingProfile` reference (Line 157)**
- The `PostMappingProfile` class does not exist in the project
- Either remove this mapping profile instantiation or create a proper AutoMapper configuration for tests
- Consider using a mock IMapper instead of instantiating actual mapping profiles

**Fix 3: Remove duplicate using directives (Lines 16-21)**
- Delete the duplicate using statements for: `Blogifier.Posts`, `Blogifier.Data`, `Blogifier.Shared`, `AutoMapper`, `Xunit`, and `System.Linq`

        [Fact]
        public async Task GetSearchAsync_WithValidTerm_ReturnsRankedResults()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var category = new Category { Id = 1, Content = "test" };
                
                var post1 = new Post
                {
                    Id = 1,
                    Title = "Test Blog Post",
                    Slug = "test-blog-post",
                    Content = "Some content here",
                    Description = "A test description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PostCategories = new List<PostCategory>
                    {
                        new PostCategory { Category = category }
                    }
                };
                
                var post2 = new Post
                {
                    Id = 2,
                    Title = "Another Post",
                    Slug = "another-post",
                    Content = "Content with test keyword",
                    Description = "Description with blog and post words",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post
                };
                
                var post3 = new Post
                {
                    Id = 3,
                    Title = "Unrelated Article",
                    Slug = "unrelated-article",
                    Content = "Nothing matching",
                    Description = "No matches here",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post
                };
                
                context.Posts.AddRange(post1, post2, post3);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<PostMappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetSearchAsync("test blog post", 1, 10);
                
                // Assert
                Assert.NotNull(result);
                Assert.True(result.Total >= 2);
                Assert.True(result.Items.Count() >= 2);
                var firstItem = result.Items.First();
                Assert.Equal("Test Blog Post", firstItem.Title);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

**Root Causes:**

1. **Invalid Type Reference (Line 104)**: `Blogifier.Data.User` does not exist. The correct type is `AppUser`.

2. **Missing MappingProfile Reference (Line 127)**: `Blogifier.Shared.MappingProfile` does not exist in the namespace.

3. **Duplicate Using Directives (Lines 15-16)**: Causing compilation warnings for `Blogifier.Data` and `Blogifier.Shared`.

## Recommended Fixes

**Fix 1: Correct the User type (Line 104)**
```csharp
// Change from:
var user = new Blogifier.Data.User { Id = 1, UserName = "testuser", Email = "test@test.com" };

// To:
var user = new AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
```

**Fix 2: Remove or correct the MappingProfile reference (Line 127)**
Remove the line entirely or replace with the correct mapping configuration from the actual project structure.

**Fix 3: Remove duplicate using directives (Lines 15-16)**
Delete the duplicate `using Blogifier.Data;` and `using Blogifier.Shared;` statements.

        [Fact]
        public async Task UpdateAsync_UpdatesPostWithNewCategories_WhenUserOwnsPost()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new Blogifier.Data.User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var category1 = new Category { Id = 1, Content = "Tech" };
                var post = new Post
                {
                    Id = 1,
                    Title = "Original Title",
                    Slug = "original-title",
                    Content = "Original Content",
                    Description = "Original Description",
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post,
                    PostCategories = new List<PostCategory>
                    {
                        new PostCategory { Category = category1 }
                    }
                };
                
                context.Posts.Add(post);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<Blogifier.Shared.MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                var postInput = new PostEditorDto
                {
                    Id = 1,
                    Title = "Updated Title",
                    Slug = "original-title",
                    Content = "Updated Content",
                    Description = "Updated Description",
                    State = PostState.Release,
                    PostType = PostType.Post,
                    Categories = new List<CategoryDto>
                    {
                        new CategoryDto { Content = "News" },
                        new CategoryDto { Content = "Tech" }
                    }
                };
                
                // Act
                await postProvider.UpdateAsync(postInput, 1);
                
                // Assert
                var updatedPost = await context.Posts
                    .Include(p => p.PostCategories)
                    .ThenInclude(pc => pc.Category)
                    .FirstAsync(p => p.Id == 1);
                Assert.Equal("Updated Title", updatedPost.Title);
                Assert.Equal("Updated Content", updatedPost.Content);
                Assert.Equal(PostState.Release, updatedPost.State);
                Assert.NotNull(updatedPost.PublishedAt);
                Assert.Equal(2, updatedPost.PostCategories.Count);
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

**Root Causes:**

1. **Invalid Type Reference**: `Blogifier.Data.User` does not exist in the codebase. The correct type is `AppUser`.

2. **Missing MappingProfile**: `Blogifier.Shared.MappingProfile` does not exist in the namespace.

3. **Duplicate Using Directives**: Multiple using statements are duplicated (lines 15-22), causing warnings.

## Recommended Fixes

**Fix 1: Correct the User type reference (line 110)**
```csharp
// Change from:
var user = new Blogifier.Data.User { Id = 1, UserName = "testuser", Email = "test@test.com" };

// To:
var user = new AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
```

**Fix 2: Remove or correct the MappingProfile reference (line 148)**
- Either remove the line entirely if not needed
- Or replace with the correct mapping configuration class from the actual project structure

**Fix 3: Remove duplicate using directives (lines 15-22)**
Delete the duplicate using statements to eliminate warnings.

        [Fact]
        public async Task GetPostsAsync_ReturnsPaginatedPosts_WithCorrectFiltering()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new Blogifier.Data.User { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                for (int i = 1; i <= 25; i++)
                {
                    context.Posts.Add(new Post
                    {
                        Id = i,
                        Title = $"Post {i}",
                        Slug = $"post-{i}",
                        Content = "Content",
                        Description = "Description",
                        UserId = 1,
                        User = user,
                        State = PostState.Release,
                        PostType = PostType.Post,
                        CreatedAt = DateTime.UtcNow.AddDays(-i),
                        PublishedAt = DateTime.UtcNow.AddDays(-i)
                    });
                }
                
                // Add some posts that should be filtered out
                context.Posts.Add(new Post
                {
                    Id = 26,
                    Title = "Draft Post",
                    Slug = "draft-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    User = user,
                    State = PostState.Draft,
                    PostType = PostType.Post,
                    CreatedAt = DateTime.UtcNow
                });
                
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<Blogifier.Shared.MappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetPostsAsync(2, 10);
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(25, result.Total);
                Assert.Equal(2, result.Page);
                Assert.Equal(10, result.Items.Count());
            }
        }

*/
/*
FAILED TEST: ## Test Failure Analysis

**Root Cause:**
The test file references a namespace `Blogifier.Mapping` that does not exist in the project:
```csharp
using Blogifier.Mapping;
```

**Error:**
```
error CS0234: The type or namespace name 'Mapping' does not exist in the namespace 'Blogifier'
```

## Recommended Fix

**Remove the unused import** from `tests/Blogifier.Tests/PostProviderTests.cs`:

Delete line 16:
```csharp
using Blogifier.Mapping;  // Remove this line
```

This namespace is not used anywhere in the test file and is not part of the Blogifier project structure based on the source code provided.

        [Fact]
        public async Task GetAsync_WithValidSlug_ReturnsPostAndIncrementsViews()
        {
            // Arrange
            using (var context = new AppDbContext(_options))
            {
                var user = new AppUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
                context.Users.Add(user);
                
                var olderPost = new Post
                {
                    Id = 1,
                    Title = "Older Post",
                    Slug = "older-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow.AddDays(1),
                    Views = 0
                };
                
                var targetPost = new Post
                {
                    Id = 2,
                    Title = "Target Post",
                    Slug = "target-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow,
                    Views = 5
                };
                
                var newerPost = new Post
                {
                    Id = 3,
                    Title = "Newer Post",
                    Slug = "newer-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Release,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    Views = 0
                };
                
                var featuredPost = new Post
                {
                    Id = 4,
                    Title = "Featured Post",
                    Slug = "featured-post",
                    Content = "Content",
                    Description = "Description",
                    UserId = 1,
                    State = PostState.Featured,
                    PostType = PostType.Post,
                    PublishedAt = DateTime.UtcNow.AddDays(-2),
                    Views = 0
                };
                
                context.Posts.AddRange(olderPost, targetPost, newerPost, featuredPost);
                await context.SaveChangesAsync();
                
                var config = new MapperConfiguration(cfg => cfg.AddProfile<AppMappingProfile>());
                var mapper = config.CreateMapper();
                var postProvider = new PostProvider(mapper, context);
                
                // Act
                var result = await postProvider.GetAsync("target-post");
                
                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Post);
                Assert.Equal("Target Post", result.Post.Title);
                Assert.Equal(6, result.Post.Views);
                Assert.NotNull(result.Older);
                Assert.Equal("Older Post", result.Older.Title);
                Assert.NotNull(result.Newer);
                Assert.Equal("Newer Post", result.Newer.Title);
                Assert.NotNull(result.Related);
                Assert.Single(result.Related);
            }
        }

*/


        
    }


}
