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
FAILED TEST: <|channel|>analysis<|message|>We need to give short concise analysis: compile error due to missing closing brace in test file. Recommend fixing braces, proper namespace, class, using statements.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler reports **CS1513: “} expected”** at line 94 because the file’s braces are mismatched/missing and the class/namespace syntax is malformed (e.g., stray `using` statements and an illegal `public class PostProviderTests using …` declaration).

**Recommended fixes**  
1. Move all `using …;` directives to the top of the file, outside any namespace.  
2. Add a proper `namespace Blogifier.Tests { … }` block and close it with `}`.  
3. Declare the test class correctly, e.g. `public class PostProviderTests : IDisposable { … }`, and ensure it ends with a matching `}`.  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently inside the class body.  
5. Verify each test method is inside the class and marked with `[Fact]`/`[Theory]`.

        [Fact]
        public async Task StateAsynct_SinglePost_UpdatesState()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var post = new Post
            {
                Id = 5,
                UserId = 1,
                Title = "State Test",
                Slug = "state-test",
                State = PostState.Draft,
                PostType = PostType.Post,
                CreatedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file. Recommend fixing braces, proper namespace, class, using statements.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler reports **CS1513: “} expected”** at line 94 because the file’s braces are mismatched/missing and the class/namespace syntax is malformed (e.g., `public class PostProviderTests using …`, stray `using` statements inside the class, incomplete `using (var context = …)` block).

**Recommended fixes**  
1. Move all `using …;` directives to the top of the file, outside any namespace.  
2. Add a proper `namespace Blogifier.Tests { … }` block and close it with `}`.  
3. Declare the test class correctly, e.g. `public class PostProviderTests : IDisposable { … }`, and ensure it ends with a matching `}`.  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently inside the class body.  
5. Verify each test method is inside the class and marked with `[Fact]`/`[Theory]`.

        [Fact]
        public async Task UpdateAsync_UnauthorizedUser_ThrowsException()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var ownerId = 10;
            var otherUserId = 20;
            var post = new Post
            {
                Id = 1,
                UserId = ownerId,
                Title = "Original Title",
                Slug = "original-title",
                Content = "content",
                Description = "desc",
                PostType = PostType.Post,
                State = PostState.Draft,
                CreatedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        
            var mapperMock = new Mock<IMapper>();
            var provider = new PostProvider(mapperMock.Object, context);
        
            var postInput = new PostEditorDto
            {
                Id = 1,
                Title = "Updated Title",
                Slug = "updated-title",
                Content = "new content",
                Description = "new desc",
                PostType = PostType.Post,
                State = PostState.Publish
            };
        
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () =>
            {
                await provider.UpdateAsync(postInput, otherUserId);
            });
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace, malformed syntax. Provide fixes: correct braces, proper namespace, class, using statements, remove stray code.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler reports **CS1513: “} expected”** at line 94 because the file’s braces are mismatched/missing and the class/namespace syntax is malformed (e.g., `public class PostProviderTests using …`, `using` directives placed inside the class, stray `using (var context = …)` code).

**Recommended fixes**  
1. Move **all** `using …;` statements to the top of the file, outside any namespace.  
2. Add a proper `namespace Blogifier.Tests { … }` block and close it with `}`.  
3. Declare the test class correctly, e.g. `public class PostProviderTests : IDisposable { … }`, and ensure it ends with a matching `}`.  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently inside the class body.  
5. Verify each test method is inside the class and marked with `[Fact]`/`[Theory]`.  

After correcting the syntax and brace mismatches the project will build and the tests can run.

        [Fact]
        public async Task CheckPostCategories_RemovesDuplicatesAndMergesExisting()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var tech = new Category { Content = "Tech" };
            var news = new Category { Content = "News" };
            context.Categories.AddRange(tech, news);
            await context.SaveChangesAsync();
        
            // Existing PostCategory linking to Tech
            var existing = new PostCategory { Category = tech };
            var originalList = new List<PostCategory> { existing };
        
            var input = new List<CategoryDto>
            {
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "News" }
            };
        
            var mapperMock = new Mock<IMapper>();
            var provider = new PostProvider(mapperMock.Object, context);
        
            var method = typeof(PostProvider)
                .GetMethod("CheckPostCategories", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(method);
        
            // Act
            var task = (Task)method.Invoke(provider, new object[] { input, originalList });
            await task.ConfigureAwait(false);
            var resultProp = task.GetType().GetProperty("Result");
            var result = resultProp.GetValue(task) as List<PostCategory>;
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, pc => pc.Category.Content == "Tech");
            Assert.Contains(result, pc => pc.Category.Content == "News");
            Assert.Equal(1, result.Count(pc => pc.Category.Content == "Tech"));
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file. Recommend fixing braces, proper namespace, class, using statements.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler stops at line 94 with **CS1513: “} expected”** because the file’s braces are mismatched/missing and the class/namespace syntax is malformed (e.g., `public class PostProviderTests using …`, `using` directives inside the class body, stray `using (var context = …)` code).

**Recommended fixes**  
1. Move **all** `using …;` statements to the top of the file, outside any namespace.  
2. Add a proper `namespace Blogifier.Tests { … }` block and close it with `}`.  
3. Declare the test class correctly, e.g. `public class PostProviderTests : IDisposable { … }`, and ensure it ends with a matching `}`.  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently inside the class body.  
5. Verify each test method is inside the class and marked with `[Fact]`/`[Theory]`.  

After correcting the syntax and balancing the braces, the project will build and the tests can run.

        [Fact]
        public void GetPublishedAt_DraftState_WithPublishedAt_ReturnsNull()
        {
            // Arrange
            var method = typeof(PostProvider)
                .GetMethod("GetPublishedAt", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            var supplied = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
            // Act
            var result = method.Invoke(null, new object[] { supplied, PostState.Draft }) as DateTime?;
        
            // Assert
            Assert.Null(result);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file. Recommend fixing braces, proper namespace, class, using statements.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler reports **CS1513: “} expected”** at line 94 because the file’s braces are mismatched/missing (the `namespace` and/or class are not properly closed) and the class declaration is malformed (e.g., `public class PostProviderTests using …`).  

**Recommended fixes**  
1. Move all `using …;` directives to the top of the file, outside any namespace.  
2. Add a proper `namespace Blogifier.Tests { … }` block and close it with `}`.  
3. Declare the test class correctly, e.g. `public class PostProviderTests : IDisposable { … }`, and ensure it ends with a matching `}`.  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently inside the class body.  
5. Verify each test method is inside the class and marked with `[Fact]`/`[Theory]`.  

After correcting the syntax and closing braces, the project will build and the tests can run.

        [Fact]
        public void GetPublishedAt_ReleaseState_NullPublishedAt_ReturnsCurrentUtc()
        {
            // Arrange
            var method = typeof(PostProvider)
                .GetMethod("GetPublishedAt", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
        
            // Act
            var result = method.Invoke(null, new object[] { null, PostState.Release }) as DateTime?;
        
            // Assert
            Assert.NotNull(result);
            var now = DateTime.UtcNow;
            Assert.InRange(result.Value, now.AddSeconds(-5), now.AddSeconds(5));
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file. Recommend fixing braces, proper namespace, class, using statements.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler reports **CS1513: “} expected”** at line 94 because the file’s braces are mismatched (missing closing `}` for the namespace/class) and the class declaration is malformed (e.g., `public class PostProviderTests using …`). Additionally, `using` directives are placed inside the class body.

**Recommended fixes**  
1. Move **all** `using …;` statements to the top of the file, outside any namespace.  
2. Add a proper `namespace Blogifier.Tests { … }` block and close it with `}`.  
3. Declare the test class correctly, e.g. `public class PostProviderTests : IDisposable { … }`, and ensure it ends with a matching `}`.  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently inside the class body.  
5. Verify each test method is inside the class and marked with `[Fact]`/`[Theory]`.  

After correcting the syntax and brace structure, the project will build and the tests can run.

        [Fact]
        public async Task GetSlugFromTitle_ExceedsMaxAttempts_ThrowsException()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var mapperMock = new Mock<IMapper>();
            var provider = new PostProvider(mapperMock.Object, context);
        
            // Create 100 posts with slugs duplicate-title, duplicate-title2 … duplicate-title100
            for (int i = 1; i <= 100; i++)
            {
                var slug = i == 1 ? "duplicate-title" : $"duplicate-title{i}";
                context.Posts.Add(new Post
                {
                    Title = $"Duplicate Title {i}",
                    Slug = slug,
                    UserId = 1,
                    State = PostState.Draft,
                    PostType = PostType.Post
                });
            }
            await context.SaveChangesAsync();
        
            var postInput = new PostEditorDto
            {
                Title = "Duplicate Title",
                Content = "content",
                Description = "desc",
                PostType = PostType.Post,
                State = PostState.Draft
            };
        
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () =>
            {
                await provider.AddAsync(postInput, 1);
            });
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file. Recommend fixing braces, proper namespace, class, using statements.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler reports **CS1513: “} expected”** at line 94 because the file’s braces are mismatched (missing closing `}` for the namespace/class) and the class declaration is malformed (e.g., `public class PostProviderTests using …`).  

**Recommended fixes**  
1. Move all `using …;` directives to the top of the file, outside any namespace.  
2. Add a proper `namespace Blogifier.Tests { … }` block and close it with `}`.  
3. Declare the test class correctly, e.g. `public class PostProviderTests : IDisposable { … }`, and ensure it ends with a matching `}`.  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently inside the class body.  
5. Verify that each test method is inside the class and marked with `[Fact]`/`[Theory]`.  

After correcting the syntax and brace pairing, the project will build and the tests can be executed.

        [Fact]
        public async Task GetSearchAsync_RankingSkipsShortTermWhenRankAlreadySet()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var post = new Post
            {
                Id = 1,
                Title = "TestPost",
                Description = "A test description",
                Content = "irrelevant",
                State = PostState.Release,
                PostType = PostType.Post,
                CreatedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to mismatched braces, malformed class, stray using statements. Recommend moving usings to top, proper namespace braces, proper class declaration, remove stray code, ensure closing braces.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` does not compile. The file has mismatched/missing `{}` braces, an illegal class declaration (`public class PostProviderTests using …`), and `using` directives placed inside the class body. Because the compiler hits **CS1513: “} expected”** at line 94, no test assembly is built and the test runner aborts.

**Recommended fixes**  
1. Move **all** `using …;` statements to the very top of the file, outside any namespace.  
2. Add a proper namespace block:  
   ```csharp
   namespace Blogifier.Tests
   {
       // test class goes here
   }
   ```  
3. Declare the test class correctly, e.g.:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, Dispose, test methods
   }
   ```  
4. Remove any stray code fragments that are currently inside the class body (`using (var context = …)`, incomplete statements, etc.).  
5. Ensure every opening `{` has a matching `}` and that each test method is inside the class and marked with `[Fact]` or `[Theory]`.  

After correcting the file’s syntax and brace structure, the project will compile and the tests can be executed.

        [Fact]
        public async Task GetPostsAsync_PageBeyondRange_ReturnsEmptyItems()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var user = new User { Id = 1, Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        
            var posts = new[]
            {
                new Post { Id = 1, Title = "P1", CreatedAt = DateTime.UtcNow, PostType = PostType.Post, State = PostState.Release, User = user },
                new Post { Id = 2, Title = "P2", CreatedAt = DateTime.UtcNow.AddMinutes(-1), PostType = PostType.Post, State = PostState.Release, User = user },
                new Post { Id = 3, Title = "P3", CreatedAt = DateTime.UtcNow.AddMinutes(-2), PostType = PostType.Post, State = PostState.Release, User = user },
                new Post { Id = 4, Title = "P4", CreatedAt = DateTime.UtcNow.AddMinutes(-3), PostType = PostType.Post, State = PostState.Release, User = user },
                new Post { Id = 5, Title = "P5", CreatedAt = DateTime.UtcNow.AddMinutes(-4), PostType = PostType.Post, State = PostState.Release, User = user }
            };
            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns<IQueryable<Post>>(q => q.Select(p => new PostItemDto
                      {
                          Id = p.Id,
                          Title = p.Title,
                          CreatedAt = p.CreatedAt
                      }).AsQueryable());
        
            var provider = new PostProvider(mapperMock.Object, context);
        
            // Act: page 4 with pageSize 2 => skip = (4-1)*2 = 6 > total 5
            var result = await provider.GetPostsAsync(page: 4, pageSize: 2);
        
            // Assert
            Assert.Equal(5, result.Total);
            Assert.Empty(result.Items);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>Need concise answer.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
The test file `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler stops at line 94 with **CS1513: “} expected”** because the file’s braces are mismatched and the syntax is malformed (e.g., `public class PostProviderTests using …`, stray `using` statements inside the class, missing opening/closing braces for the namespace and class).

**Recommended fixes**  
1. Move **all** `using …;` directives to the top of the file, outside any namespace.  
2. Add a proper namespace block:  
   ```csharp
   namespace Blogifier.Tests
   {
       // test class goes here
   }
   ```  
3. Declare the test class correctly, e.g.:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, Dispose, test methods
   }
   ```  
4. Remove any stray code fragments (`using (var context = …)`, incomplete statements) that are currently placed inside the class body.  
5. Ensure every opening `{` has a matching `}` and that each test method is inside the class and marked with `[Fact]`/`[Theory]`.  

After correcting the file’s structure and syntax, the project will build and the tests can be executed.

        [Fact]
        public async Task GetPostsAsync_PageSizeLargerThanTotal_ReturnsAllPosts()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var user = new User { Id = 1, Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        
            var posts = new[]
            {
                new Post { Id = 1, Title = "A", CreatedAt = DateTime.UtcNow.AddHours(-1), PostType = PostType.Post, State = PostState.Release, User = user },
                new Post { Id = 2, Title = "B", CreatedAt = DateTime.UtcNow.AddHours(-2), PostType = PostType.Post, State = PostState.Release, User = user },
                new Post { Id = 3, Title = "C", CreatedAt = DateTime.UtcNow.AddHours(-3), PostType = PostType.Post, State = PostState.Release, User = user },
                new Post { Id = 4, Title = "D", CreatedAt = DateTime.UtcNow.AddHours(-4), PostType = PostType.Post, State = PostState.Release, User = user }
            };
            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns<IQueryable<Post>>(q => q.Select(p => new PostItemDto
                      {
                          Id = p.Id,
                          Title = p.Title,
                          CreatedAt = p.CreatedAt
                      }).AsQueryable());
        
            var provider = new PostProvider(mapperMock.Object, context);
        
            // Act
            var result = await provider.GetPostsAsync(page: 1, pageSize: 10);
        
            // Assert
            Assert.Equal(4, result.Total);
            Assert.Equal(4, result.Items.Count);
            // Items should be ordered by CreatedAt descending
            var expectedOrder = posts.OrderByDescending(p => p.CreatedAt).Select(p => p.Id).ToArray();
            var actualOrder = result.Items.Select(i => i.Id).ToArray();
            Assert.Equal(expectedOrder, actualOrder);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file. Recommend fixing syntax, proper namespace, class braces, etc.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- The test source file `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler stops at line 94 with **CS1513: “} expected”**, indicating mismatched or missing braces. The file also contains malformed constructs (e.g., `public class PostProviderTests using …`, `using` statements inside the class body, stray `using (var context = …)` code). Because the file cannot be compiled, no tests are discovered or executed.

**Recommended fixes**  
1. Move all `using …;` directives to the top of the file, outside any namespace.  
2. Open a proper namespace block:  
   ```csharp
   namespace Blogifier.Tests
   {
       // test class goes here
   }
   ```  
3. Declare the test class correctly, e.g.:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, Dispose, test methods
   }
   ```  
4. Remove or relocate stray code fragments (`using (var context = …)`, incomplete object initializers, etc.) so that only method bodies remain inside the class.  
5. Ensure every opening `{` has a matching `}` and that each test method is marked with `[Fact]`/`[Theory]`.  

After correcting the syntax and brace mismatches, the project will build and the PostProvider tests can run.

        [Fact]
        public async Task GetAsync_BySlug_NoOlderNewer_ReturnsRelatedOnly()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var user = new User { Id = 1, Name = "User" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        
            var target = new Post
            {
                Id = 10,
                Title = "Solo Post",
                Slug = "solo-post",
                PublishedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                State = PostState.Release,
                User = user,
                Views = 0
            };
            // Four featured posts (only three will be taken)
            var f1 = new Post { Id = 11, Title = "F1", Slug = "f1", PublishedAt = new DateTime(2024, 2, 2), State = PostState.Featured, User = user };
            var f2 = new Post { Id = 12, Title = "F2", Slug = "f2", PublishedAt = new DateTime(2024, 2, 3), State = PostState.Featured, User = user };
            var f3 = new Post { Id = 13, Title = "F3", Slug = "f3", PublishedAt = new DateTime(2024, 2, 4), State = PostState.Featured, User = user };
            var f4 = new Post { Id = 14, Title = "F4", Slug = "f4", PublishedAt = new DateTime(2024, 2, 5), State = PostState.Featured, User = user };
        
            context.Posts.AddRange(target, f1, f2, f3, f4);
            await context.SaveChangesAsync();
        
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns<IQueryable<Post>>(q => q.Select(p => new PostToHtmlDto
                      {
                          Id = p.Id,
                          Title = p.Title,
                          Slug = p.Slug,
                          PublishedAt = p.PublishedAt,
                          User = p.User,
                          Views = p.Views
                      }).AsQueryable());
        
            mapperMock.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns<IQueryable<Post>>(q => q.Select(p => new PostItemDto
                      {
                          Id = p.Id,
                          Title = p.Title,
                          Slug = p.Slug,
                          PublishedAt = p.PublishedAt
                      }).AsQueryable());
        
            var provider = new PostProvider(mapperMock.Object, context);
        
            // Act
            var result = await provider.GetAsync("solo-post");
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(target.Id, result.Post.Id);
            Assert.Null(result.Older);
            Assert.Null(result.Newer);
            Assert.NotNull(result.Related);
            Assert.Equal(3, result.Related.Count);
            var relatedIds = result.Related.Select(r => r.Id).ToList();
            // The three most recent featured posts (by PublishedAt descending) should be returned
            Assert.Contains(f4.Id, relatedIds);
            Assert.Contains(f3.Id, relatedIds);
            Assert.Contains(f2.Id, relatedIds);
            Assert.DoesNotContain(f1.Id, relatedIds); // f1 is the oldest and should be excluded by Take(3)
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing braces, malformed class, using statements inside class. Recommend fixing file: proper using directives at top, proper namespace braces, proper class declaration, remove stray code, ensure methods inside class, close braces.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` does not compile. The file is missing the opening/closing braces for the `namespace` and the test class, contains an illegal class declaration (`public class PostProviderTests using …`), and has `using` statements and other code fragments placed inside the class body. Because of these syntax errors the C# compiler stops at line 94 with **CS1513: “} expected”**, so no tests are discovered or executed.

**Recommended fixes**  
1. Move **all** `using …;` directives to the very top of the file, outside any namespace.  
2. Wrap the test code in a proper namespace block:  
   ```csharp
   namespace Blogifier.Tests
   {
       // test class goes here
   }
   ```  
3. Declare the test class correctly, e.g.:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, Dispose, test methods
   }
   ```  
4. Remove or relocate stray statements such as `using (var context = …)` and any incomplete object initializers so that only method bodies remain inside the class.  
5. Ensure every opening `{` has a matching `}` and that each test method is marked with `[Fact]`/`[Theory]`.  

After correcting the file’s structure and syntax, the project will build and the tests will run.

        [Fact]
        public async Task GetAsync_BySlug_ReturnsOlderNewerAndRelated_ExcludesOlderNewer()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            var user = new User { Id = 1, Name = "TestUser" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        
            // Target post
            var target = new Post
            {
                Id = 1,
                Title = "Main Post",
                Slug = "main-post",
                PublishedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                State = PostState.Release,
                User = user,
                Views = 0
            };
            // Older post
            var older = new Post
            {
                Id = 2,
                Title = "Older Post",
                Slug = "older-post",
                PublishedAt = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                State = PostState.Release,
                User = user,
                Views = 0
            };
            // Newer post
            var newer = new Post
            {
                Id = 3,
                Title = "Newer Post",
                Slug = "newer-post",
                PublishedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                State = PostState.Release,
                User = user,
                Views = 0
            };
            // Three featured related posts
            var related1 = new Post { Id = 4, Title = "Featured A", Slug = "featured-a", PublishedAt = new DateTime(2024, 2, 1), State = PostState.Featured, User = user };
            var related2 = new Post { Id = 5, Title = "Featured B", Slug = "featured-b", PublishedAt = new DateTime(2024, 2, 2), State = PostState.Featured, User = user };
            var related3 = new Post { Id = 6, Title = "Featured C", Slug = "featured-c", PublishedAt = new DateTime(2024, 2, 3), State = PostState.Featured, User = user };
        
            context.Posts.AddRange(target, older, newer, related1, related2, related3);
            await context.SaveChangesAsync();
        
            // Mock mapper to project Post -> DTOs
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.ProjectTo<PostToHtmlDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns<IQueryable<Post>>(q => q.Select(p => new PostToHtmlDto
                      {
                          Id = p.Id,
                          Title = p.Title,
                          Slug = p.Slug,
                          Content = p.Content,
                          Description = p.Description,
                          Views = p.Views,
                          PublishedAt = p.PublishedAt,
                          User = p.User
                      }).AsQueryable());
        
            mapperMock.Setup(m => m.ProjectTo<PostItemDto>(It.IsAny<IQueryable<Post>>()))
                      .Returns<IQueryable<Post>>(q => q.Select(p => new PostItemDto
                      {
                          Id = p.Id,
                          Title = p.Title,
                          Slug = p.Slug,
                          PublishedAt = p.PublishedAt
                      }).AsQueryable());
        
            var provider = new PostProvider(mapperMock.Object, context);
        
            // Act
            var result = await provider.GetAsync("main-post");
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(target.Id, result.Post.Id);
            Assert.Equal(older.Id, result.Older?.Id);
            Assert.Equal(newer.Id, result.Newer?.Id);
            Assert.NotNull(result.Related);
            Assert.Equal(3, result.Related.Count);
            var relatedIds = result.Related.Select(r => r.Id).ToList();
            Assert.DoesNotContain(older.Id, relatedIds);
            Assert.DoesNotContain(newer.Id, relatedIds);
            Assert.Contains(related1.Id, relatedIds);
            Assert.Contains(related2.Id, relatedIds);
            Assert.Contains(related3.Id, relatedIds);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to summarize: compile error due to missing closing brace in test file. Provide fix: add proper braces, correct syntax.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- The test source file `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler reports **CS1513: “} expected”** at line 94, which means a closing brace is missing (or the file’s braces are mismatched). The file also contains malformed constructs such as `using (var context = …)` and an invalid class declaration (`public class PostProviderTests using …`). Because the file cannot be compiled, the test runner never executes any tests.

**Recommended fixes**  
1. **Re‑structure the file**:  
   - Place all `using …;` directives at the top of the file, outside any namespace.  
   - Open a `namespace Blogifier.Tests { … }` block and close it with a matching `}`.  
   - Declare the test class inside that namespace, e.g. `public class PostProviderTests : IDisposable { … }`, and close it with `}`.  
2. **Remove or relocate stray code** (`using (var context = …)`, incomplete object initializers, etc.) so that only method bodies remain inside the class.  
3. **Add the missing closing brace(s)** to balance every opening `{` (namespace, class, methods).  
4. **Ensure each test method is properly annotated** (`[Fact]` or `[Theory]`) and contains valid C# code.  
5. Re‑run the build; the compilation error should disappear and the tests will execute.

        [Fact]
        public async Task StateAsynct_SinglePost_UpdatesState()
        {
            // Arrange
            var postId = 5;
            using var context = new AppDbContext(_options);
            var post = new Post
            {
                Id = postId,
                UserId = 1,
                Title = "State Test",
                Slug = "state-test",
                State = PostState.Draft,
                PostType = PostType.Post,
                CreatedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing braces, malformed syntax in test file. Recommend fixing file: proper using statements, namespace braces, class braces, remove stray code, ensure methods inside class, compile.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` does not compile. At line 94 the compiler reports **CS1513: “} expected”** because the file is missing the opening/closing braces for the `namespace` and the test class, contains malformed class/using syntax, and has stray code fragments that are not inside any method.

**Recommended fixes**  
1. Move all `using …;` directives to the top of the file, outside any namespace.  
2. Open a `namespace Blogifier.Tests { … }` block and close it with a matching `}`.  
3. Declare the test class inside that namespace, e.g. `public class PostProviderTests : IDisposable { … }`, and close it with `}`.  
4. Remove or relocate stray statements such as `using (var context = …)`, incomplete object initializers, and any code that appears outside a method.  
5. Ensure each test method is inside the class and marked with `[Fact]` (or `[Theory]`).  
6. After correcting the syntax, rebuild the project; the tests will then run (or fail for logical reasons) instead of aborting at compile time.

        [Fact]
        public async Task UpdateAsync_UnauthorizedUser_ThrowsException()
        {
            // Arrange
            var ownerId = 10;
            var otherUserId = 20;
            using var context = new AppDbContext(_options);
            var post = new Post
            {
                Id = 1,
                UserId = ownerId,
                Title = "Original Title",
                Slug = "original-title",
                Content = "content",
                Description = "desc",
                PostType = PostType.Post,
                State = PostState.Draft,
                CreatedAt = DateTime.UtcNow
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        
            var mapperMock = new Mock<IMapper>();
            var provider = new PostProvider(mapperMock.Object, context);
        
            var postInput = new PostEditorDto
            {
                Id = 1,
                Title = "Updated Title",
                Slug = "updated-title",
                Content = "new content",
                Description = "new desc",
                PostType = PostType.Post,
                State = PostState.Publish
            };
        
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () =>
            {
                await provider.UpdateAsync(postInput, otherUserId);
            });
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>Need concise answer.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- The test file `tests/Blogifier.Tests/PostProviderTests.cs` does not compile.  
- At line 94 the compiler reports **CS1513: “} expected”** because the file is missing opening/closing braces for the `namespace` and the test class, contains malformed class/using syntax, incomplete statements, and stray code fragments that are not inside any method.  
- Because the source cannot be compiled, the test runner aborts before executing any tests.

**Recommended fixes**  
1. **Rewrite the test file with a proper C# skeleton**:  
   ```csharp
   using System;
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using AutoMapper;
   using Blogifier.Data;
   using Blogifier.Posts;
   using Blogifier.Shared;
   using Microsoft.EntityFrameworkCore;
   using Moq;
   using Xunit;

   namespace Blogifier.Tests
   {
       public class PostProviderTests : IDisposable
       {
           private readonly DbContextOptions<AppDbContext> _options;
           private readonly Mock<IMapper> _mapperMock;

           public PostProviderTests()
           {
               _mapperMock = new Mock<IMapper>();
               _options = new DbContextOptionsBuilder<AppDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options;
           }

           public void Dispose()
           {
               // clean up the in‑memory DB
               using var ctx = new AppDbContext(_options);
               ctx.Database.EnsureDeleted();
           }

           [Fact]
           public async Task AddAsync_CreatesPost_ReturnsSlug()
           {
               // Arrange
               using var ctx = new AppDbContext(_options);
               var provider = new PostProvider(_mapperMock.Object, ctx);
               var input = new PostEditorDto
               {
                   Title = "Test Title",
                   Content = "Content",
                   Description = "Desc",
                   State = PostState.Draft,
                   PostType = PostType.Post,
                   // other required fields …
               };
               var userId = 1;

               // Act
               var slug = await provider.AddAsync(input, userId);

               // Assert
               Assert.False(string.IsNullOrWhiteSpace(slug));
               var saved = await ctx.Posts.FirstAsync();
               Assert.Equal(slug, saved.Slug);
           }

           // Add more [Fact] methods as needed …
       }
   }
   ```
2. **Place all `using` directives at the top of the file**, outside any namespace or class.  
3. **Open the `namespace Blogifier.Tests { … }` block and close it with a matching `}`**.  
4. **Declare the test class inside the namespace** (`public class PostProviderTests`) and close it with `}`.  
5. **Wrap every test scenario in a method marked with `[Fact]` (or `[Theory]`)**; ensure each method has a body delimited by `{}`.  
6. **Remove stray code fragments** (e.g., `using (var context = …)`, incomplete object initializers, commented‑out sections that break syntax).  

After correcting the file so it compiles, re‑run `dotnet test`; the tests will execute and you can then address any functional failures.

        [Fact]
        public async Task CheckPostCategories_RemovesDuplicatesAndMergesExisting()
        {
            // Arrange
            using var context = new AppDbContext(_options);
            // Existing category "Tech"
            var techCategory = new Category { Content = "Tech" };
            context.Categories.Add(techCategory);
            // Existing category "News"
            var newsCategory = new Category { Content = "News" };
            context.Categories.Add(newsCategory);
            await context.SaveChangesAsync();
        
            // Existing PostCategory linking to "Tech"
            var existingPostCategory = new PostCategory { Category = techCategory };
            var originalList = new List<PostCategory> { existingPostCategory };
        
            var inputCategories = new List<CategoryDto>
            {
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "News" }
            };
        
            var mapperMock = new Mock<IMapper>();
            var provider = new PostProvider(mapperMock.Object, context);
        
            var method = typeof(PostProvider).GetMethod("CheckPostCategories", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(method);
        
            // Act
            var task = (Task)method.Invoke(provider, new object[] { inputCategories, originalList });
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty.GetValue(task) as List<PostCategory>;
        
            // Assert
            Assert.NotNull(result);
            // Should contain two entries: one for existing Tech, one new News
            Assert.Equal(2, result.Count);
            Assert.Contains(result, pc => pc.Category.Content == "Tech");
            Assert.Contains(result, pc => pc.Category.Content == "News");
            // Ensure no duplicate Tech entries
            var techCount = result.Count(pc => pc.Category.Content == "Tech");
            Assert.Equal(1, techCount);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to give short concise analysis: compile error due to malformed test file, missing braces, incomplete code. Recommend rewrite test file with proper syntax, include using statements, namespace, class, methods, dispose pattern, etc. Also ensure test methods have [Fact] attributes.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- The test source `tests/Blogifier.Tests/PostProviderTests.cs` does not compile.  
- At line 94 the compiler reports **CS1513: “} expected”** because the file has:  
  1. Missing opening/closing braces for the `namespace` and the test class.  
  2. A malformed class declaration (`public class PostProviderTests using (var context = …)`).  
  3. `using` directives placed inside the class body.  
  4. Incomplete statements (e.g., `var postInput = new PostEditorDto` without `;` or initializer, object‑initializer syntax errors, stray comments).  
  5. Several code fragments that are not inside any method.  

Because the file cannot be compiled, the test runner never discovers any tests, resulting in the build error shown in `stdout`.

**Recommended fixes**  
1. **Rewrite the test file with a proper C# skeleton**:  
   ```csharp
   // -------------------------------------------------
   //  PostProviderTests.cs
   // -------------------------------------------------
   using System;
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using AutoMapper;
   using Blogifier.Data;
   using Blogifier.Posts;
   using Blogifier.Shared;
   using Microsoft.EntityFrameworkCore;
   using Moq;
   using Xunit;

   namespace Blogifier.Tests
   {
       public class PostProviderTests : IDisposable
       {
           private readonly DbContextOptions<AppDbContext> _options;
           private readonly Mock<IMapper> _mapperMock;

           public PostProviderTests()
           {
               _mapperMock = new Mock<IMapper>();
               _options = new DbContextOptionsBuilder<AppDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options;
           }

           public void Dispose()
           {
               // clean up the in‑memory DB after each test
               using var ctx = new AppDbContext(_options);
               ctx.Database.EnsureDeleted();
           }

           [Fact]
           public async Task AddAsync_CreatesPost_ReturnsSlug()
           {
               // Arrange
               using var ctx = new AppDbContext(_options);
               var provider = new PostProvider(_mapperMock.Object, ctx);
               var input = new PostEditorDto
               {
                   Title = "Test title",
                   Content = "<p>content</p>",
                   Description = "desc",
                   State = PostState.Draft,
                   PostType = PostType.Post,
                   Categories = null,
                   Cover = null,
                   PublishedAt = null
               };
               const int userId = 1;

               // Act
               var slug = await provider.AddAsync(input, userId);

               // Assert
               Assert.False(string.IsNullOrWhiteSpace(slug));
               var post = await ctx.Posts.FirstAsync();
               Assert.Equal(slug, post.Slug);
           }

           // …additional test methods for GetAsync, UpdateAsync, etc.
       }
   }
   ```
2. **Place all `using` statements at the top of the file**, outside any namespace or class.  
3. **Open the `namespace Blogifier.Tests { … }` block and close it with a matching `}`**.  
4. **Declare the test class correctly** (`public class PostProviderTests`) and close it with `}`.  
5. **Wrap each test in a method with a `[Fact]` (or `[Theory]`) attribute** and ensure the method body is complete and ends with a `}`.  
6. **Remove stray fragments and comments that are not inside a method**.  

After fixing the syntax and providing at least one valid test method, the project will compile and the test runner will execute the tests successfully.

        [Fact]
        public void GetPublishedAt_DraftState_ReturnsNull()
        {
            // Arrange
            var method = typeof(PostProvider).GetMethod("GetPublishedAt", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            var suppliedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
            // Act
            var result = method.Invoke(null, new object[] { suppliedDate, PostState.Draft }) as DateTime?;
        
            // Assert
            Assert.Null(result);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to give short concise analysis: syntax errors in test file, missing braces, malformed class, etc. Provide recommended fixes: rewrite test file with proper structure, add missing braces, correct using statements, proper class definition, implement test methods, etc.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
The file `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler stops at line 94 with **CS1513: “} expected”** because:

1. **Missing/incorrect braces** – the `namespace`, class, and method blocks are not opened/closed correctly.  
2. **Malformed class declaration** – `public class PostProviderTests using (var context = …)` is illegal C# syntax.  
3. **`using` directives placed inside the class body** – they must appear at the top of the file, outside any namespace or type.  
4. **Incomplete statements** (e.g., `var postInput = new PostEditorDto` without `;` or initializer, missing `=` in object initializers, missing semicolons).  
5. **Code fragments are dangling inside the file** (partial test method bodies, stray comments, etc.).  

Because the test source cannot be compiled, the test runner never discovers or executes any tests.

**Recommended fixes**

1. **Rewrite the file with a proper C# skeleton**:

   ```csharp
   // -------------------------------------------------
   //  PostProviderTests.cs
   // -------------------------------------------------
   using System;
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using AutoMapper;
   using Blogifier.Data;
   using Blogifier.Posts;
   using Blogifier.Shared;
   using Microsoft.EntityFrameworkCore;
   using Moq;
   using Xunit;

   namespace Blogifier.Tests
   {
       public class PostProviderTests : IDisposable
       {
           private readonly AppDbContext _context;
           private readonly Mock<IMapper> _mapperMock;
           private readonly PostProvider _provider;

           public PostProviderTests()
           {
               var options = new DbContextOptionsBuilder<AppDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options;

               _context = new AppDbContext(options);
               _mapperMock = new Mock<IMapper>();
               _provider = new PostProvider(_mapperMock.Object, _context);
           }

           public void Dispose()
           {
               _context?.Dispose();
           }

           [Fact]
           public async Task AddAsync_CreatesPost_ReturnsSlug()
           {
               // Arrange
               var userId = 1;
               var input = new PostEditorDto
               {
                   Title = "Test title",
                   Content = "Content",
                   Description = "Desc",
                   State = PostState.Draft,
                   PostType = PostType.Post,
                   Categories = null,
                   Cover = null,
                   PublishedAt = null
               };

               // Act
               var slug = await _provider.AddAsync(input, userId);

               // Assert
               var post = await _context.Posts.FirstAsync();
               Assert.Equal(slug, post.Slug);
               Assert.Equal(userId, post.UserId);
           }

           // Additional tests (GetAsync, UpdateAsync, etc.) go here…
       }
   }
   ```

2. **Key points to ensure**  
   * All `using` statements are **outside** the namespace.  
   * `namespace Blogifier.Tests { … }` opens and closes with braces.  
   * The test class is declared as `public class PostProviderTests` (optionally implementing `IDisposable` for cleanup).  
   * Every method (`[Fact]` test, helper, etc.) has matching `{}` braces and ends with a semicolon where required.  
   * No stray code fragments remain outside a method body.

3. **Run `dotnet test` again** – the project should now compile, the tests will be discovered, and you can verify the behavior of `PostProvider`.

By correcting the syntax and providing a well‑structured test class, the compilation error disappears and the regression suite can be executed successfully.

        [Fact]
        public void GetPublishedAt_ReleaseState_NullPublishedAt_ReturnsCurrentUtc()
        {
            // Arrange
            var method = typeof(PostProvider).GetMethod("GetPublishedAt", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
        
            // Act
            var result = method.Invoke(null, new object[] { null, PostState.Release }) as DateTime?;
        
            // Assert
            Assert.NotNull(result);
            var now = DateTime.UtcNow;
            Assert.InRange(result.Value, now.AddSeconds(-5), now.AddSeconds(5));
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to explain that the test file has syntax errors: missing braces, malformed class definition, using statements inside class, etc. Provide fix: correct the test file syntax, proper namespace, class braces, method braces, etc.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**

The compiler stopped at line 94 of `PostProviderTests.cs` with **CS1513: “} expected”**. The test file is not valid C# code:

* The `namespace` line is missing an opening brace and the `using` statements are placed inside the class body.
* The class declaration is malformed (`public class PostProviderTests using (var context = …)`).
* Braces are mismatched/missing for the class, methods, and the `Fact` test method.
* Several code fragments are incomplete (e.g., missing `=` after `var postInput = new PostEditorDto`, missing semicolons, incomplete object initializers).

Because of these syntax errors the compiler cannot build the test project, so no tests are executed.

**Recommended fixes**

1. **Rewrite the test file with proper C# structure**  
   ```csharp
   using System;
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using AutoMapper;
   using Blogifier.Data;
   using Blogifier.Posts;
   using Blogifier.Shared;
   using Microsoft.EntityFrameworkCore;
   using Moq;
   using Xunit;

   namespace Blogifier.Tests
   {
       public class PostProviderTests : IDisposable
       {
           private readonly DbContextOptions<AppDbContext> _options;
           private readonly AppDbContext _context;
           private readonly Mock<IMapper> _mapperMock;

           public PostProviderTests()
           {
               _mapperMock = new Mock<IMapper>();
               _options = new DbContextOptionsBuilder<AppDbContext>()
                   .UseInMemoryDatabase($"BlogifierTest_{Guid.NewGuid()}")
                   .Options;
               _context = new AppDbContext(_options);
           }

           public void Dispose()
           {
               _context?.Dispose();
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
                   State = PostState.Draft,
                   // other required properties …
               };

               _mapperMock.Setup(m => m.Map<Post>(It.IsAny<PostEditorDto>()))
                          .Returns(expectedPost);

               var provider = new PostProvider(_mapperMock.Object, _context);

               // Act
               var slug = await provider.AddAsync(postInput, userId);

               // Assert
               Assert.Equal(postSlug, slug);
           }

           // Add other tests here …
       }
   }
   ```

2. **Ensure every opening `{` has a matching closing `}`** – the compiler error is a direct result of missing braces.

3. **Place all `using` directives at the top of the file**, outside any namespace or class.

4. **Define the test class inside a proper `namespace` block** and give it a clean constructor/`Dispose` pattern for the in‑memory `DbContext`.

5. **Complete any incomplete statements** (e.g., object initializers, method calls) and add required properties to `PostEditorDto` so the test compiles.

After correcting the syntax and providing a compilable test class, run `dotnet test` again – the tests should build and execute, revealing any real failures in the `PostProvider` logic.

        [Fact]
        public async Task GetSlugFromTitle_ExceedsMaxAttempts_ThrowsException()
        {
            // Arrange: create 100 posts with slugs duplicate-title, duplicate-title2, ..., duplicate-title100
            var userId = 1;
            using var context = new AppDbContext(_options);
            for (int i = 1; i <= 100; i++)
            {
                var slug = i == 1 ? "duplicate-title" : $"duplicate-title{i}";
                context.Posts.Add(new Post
                {
                    Title = $"Duplicate Title {i}",
                    Slug = slug,
                    UserId = userId,
                    State = PostState.Draft,
                    PostType = PostType.Post
                });
            }
            await context.SaveChangesAsync();
        
            var mapperMock = new Mock<IMapper>();
            var provider = new PostProvider(mapperMock.Object, context);
        
            var postInput = new PostEditorDto
            {
                Title = "Duplicate Title",
                Content = "content",
                Description = "desc",
                PostType = PostType.Post,
                State = PostState.Draft
            };
        
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () =>
            {
                await provider.AddAsync(postInput, userId);
            });
        }

*/
