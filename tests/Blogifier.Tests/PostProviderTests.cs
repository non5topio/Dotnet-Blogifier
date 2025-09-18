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
FAILED TEST: <|channel|>analysis<|message|>We need to produce short concise analysis: test file syntax error missing closing brace. Recommend fixing test file: proper using statements, namespace, class braces, complete test methods, ensure compilation. Also ensure file ends with proper braces. Provide actionable steps.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
`PostProviderTests.cs` does not compile – the file opens a class (and possibly a namespace) but never closes it. `using` directives are placed inside the class body and there are stray/unfinished statements (`await using var context = …`). The compiler reaches line 94 expecting a `}` (CS1513).

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file (outside any namespace or class).  
2. Add a proper `namespace Blogifier.Tests { … }` wrapper if it’s missing.  
3. Define the test class with matching braces, e.g.  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // test methods
   }
   ```  
4. Remove the duplicate/unfinished `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every opened `{` has a corresponding `}` and that at least one `[Fact]`/`[Theory]` test method is present.  

After correcting the file structure and closing the missing brace, the project will build and the tests can run.

        [Fact]
        public async Task GetAsync_BySlug_Excludes_Older_Newer_From_Related_And_Increments_Views()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"SlugFull_{Guid.NewGuid()}")
                .Options;
            await using var context = new AppDbContext(options);

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to produce short concise analysis: test file has syntax errors, missing closing brace, misplaced using statements, incomplete class definition. Recommend fixing test file: proper namespace, using statements at top, close braces, implement test methods, ensure compilation. Provide actionable steps.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
`PostProviderTests.cs` does not compile – the file opens a class (and possibly a namespace) but never provides the matching closing `}`. `using` directives are placed inside the class body and there are stray/unfinished statements (`await using var context = …`). The compiler reaches the end of the file still expecting a `}` (CS1513).

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file (outside any namespace or class).  
2. Add a proper namespace wrapper, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching braces:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // test methods
   }
   ```  
4. Remove the duplicate/unfinished `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every opened `{` (namespace, class, method, block) has a corresponding `}` and that at least one `[Fact]`/`[Theory]` test method is present.  

After correcting the file structure and closing the missing brace, the project will build and the tests can run.

        [Fact]
        public async Task UpdateAsync_Throws_When_User_Not_Owner()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"UpdateAuth_{Guid.NewGuid()}")
                .Options;
            await using var context = new AppDbContext(options);
            var post = new Post
            {
                Id = 1,
                UserId = 10,
                Title = "Original",
                Slug = "original",
                Content = "content",
                Description = "desc",
                PostType = PostType.Post,
                State = PostState.Draft,
                PublishedAt = null
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            var updateDto = new PostEditorDto
            {
                Id = 1,
                Title = "Updated",
                Slug = "updated",
                Content = "new content",
                Description = "new desc",
                PostType = PostType.Post,
                State = PostState.Draft,
                PublishedAt = null,
                Categories = null
            };
        
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () =>
                await provider.UpdateAsync(updateDto, userId: 20)); // different user
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: test file syntax error missing closing brace. Recommend fixing test file: proper namespace, using statements at top, close braces, ensure class definition, add test methods. Also ensure compilation.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
`PostProviderTests.cs` does not compile – the file opens a namespace/class but never closes it. `using` directives are placed inside the class body and there are stray/unfinished statements (`await using var context = …`). The compiler reaches the end of the file still expecting a `}` (error CS1513 at line 94).

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file (outside any namespace or class).  
2. Add a proper namespace wrapper, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching braces:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // test methods
   }
   ```  
4. Remove the duplicate/unfinished `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every opened `{` has a corresponding `}` and that at least one `[Fact]`/`[Theory]` test method exists.  

After correcting the file’s structure and closing the missing brace, the project will compile and the tests can run.

        [Fact]
        public void GetPublishedAt_Returns_Correct_Value_Based_On_State()
        {
            // Arrange
            var method = typeof(PostProvider).GetMethod(
                "GetPublishedAt",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        
            // Case A: state >= Release, publishedAt null -> returns non‑null DateTime
            var resultA = (DateTime?)method!.Invoke(null, new object[] { null, PostState.Release });
            Assert.NotNull(resultA);
            // The returned value should be close to now (within a minute)
            Assert.True((DateTime.UtcNow - resultA!.Value).TotalMinutes < 1);
        
            // Case B: state < Release (Draft), publishedAt provided -> returns null
            var someDate = new DateTime(2023, 1, 1);
            var resultB = (DateTime?)method.Invoke(null, new object[] { someDate, PostState.Draft });
            Assert.Null(resultB);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file, malformed code. Recommend fixing test file: proper namespace, using statements at top, close braces, ensure class definition, implement tests. Also ensure file compiles.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` is not valid C# code. It opens a class (and possibly a namespace) but never provides the matching closing `}`.  
- Several `using` directives are placed inside the class body.  
- There are stray/unfinished statements (`await using var context = …`) and incomplete method bodies, leaving the compiler waiting for a `}` at line 94.  

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file, outside any namespace or class.  
2. Add a proper namespace wrapper, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching opening and closing braces:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // test methods
   }
   ```  
4. Remove the duplicate/unfinished `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every method, block, and the class itself is closed with `}` so the file compiles.

        [Fact]
        public async Task CheckPostCategories_Deduplicates_And_Merges_With_Existing()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"CheckCat_{Guid.NewGuid()}")
                .Options;
            await using var context = new AppDbContext(options);
            // Seed categories that exist in DB
            context.Categories.AddRange(
                new Category { Content = "Tech" },
                new Category { Content = "News" },
                new Category { Content = "Science" });
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            // Original PostCategories (Tech, News)
            var original = new List<PostCategory>
            {
                new PostCategory { Category = new Category { Content = "Tech" } },
                new PostCategory { Category = new Category { Content = "News" } }
            };
        
            // Input CategoryDto list with duplicate Tech and new Science
            var input = new List<CategoryDto>
            {
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "Science" }
            };
        
            // Act
            var method = typeof(PostProvider).GetMethod(
                "CheckPostCategories",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var task = (Task<List<PostCategory>>)method!
                .Invoke(provider, new object[] { input, original })!;
            var result = await task;
        
            // Assert
            Assert.NotNull(result);
            // Should contain three categories: Tech, News, Science
            Assert.Equal(3, result.Count);
            var contents = result.Select(pc => pc.Category!.Content).ToList();
            Assert.Contains("Tech", contents);
            Assert.Contains("News", contents);
            Assert.Contains("Science", contents);
            // No duplicate Tech
            Assert.Equal(1, contents.Count(c => c == "Tech"));
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file, malformed code, using statements inside class, incomplete class definition. Recommend fixing test file: proper namespace, using statements at top, close braces, implement test methods, ensure compilation.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` is not valid C# code. It opens a class (and possibly a namespace) but never provides the matching closing `}`.  
- Several `using` directives are placed inside the class body.  
- There are stray/unfinished statements (`await using var context = …`) and incomplete method bodies, leaving the compiler waiting for a `}` at line 94.  

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file, outside any namespace or class.  
2. Add a proper namespace wrapper, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching opening and closing braces (`public class PostProviderTests { … }`).  
4. Remove the duplicate/unfinished `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every method and block inside the class is closed with `}` and that the file contains at least one `[Fact]`/`[Theory]` test method.  

After correcting the braces, relocating the `using` directives, and cleaning up the stray code, the test project will compile and the tests can run.

        [Fact]
        public async Task GetSlugFromTitle_Throws_When_MaxAttemptsExceeded()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"SlugMax_{Guid.NewGuid()}")
                .Options;
            await using var context = new AppDbContext(options);
            // Create 100 conflicting slugs: duplicate, duplicate2 … duplicate100
            for (int i = 1; i <= 100; i++)
            {
                var slug = i == 1 ? "duplicate" : $"duplicate{i}";
                context.Posts.Add(new Post { Title = $"Duplicate {i}", Slug = slug, UserId = 1 });
            }
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            // Act
            var method = typeof(PostProvider).GetMethod(
                "GetSlugFromTitle",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var task = (Task<string>)method!.Invoke(provider, new object[] { "Duplicate" })!;
        
            // Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () => await task);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to produce short concise analysis: compile error due to missing closing brace, malformed test file. Provide recommended fixes: restructure file, proper using statements, namespace, class braces, remove stray code, ensure test methods, compile.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
The test file `PostProviderTests.cs` does not compile – it is syntactically malformed. At line 94 the compiler reports “} expected” because:

* `using` directives are placed inside the class body.  
* The `public class PostProviderTests` (and the surrounding namespace) open a block that is never closed.  
* There are stray/unfinished statements (`await using var context = …`) and incomplete method bodies, leaving the parser inside an open block.

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file, outside any namespace or class.  
2. Add a proper namespace wrapper, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching braces:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, test methods, Dispose()
   }
   ```  
4. Remove the duplicated/unfinished `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every opened `{` has a corresponding `}` and that each test method is decorated with `[Fact]`/`[Theory]` and returns `Task` if async.  

After correcting the file structure and closing the missing braces, the project will build and the tests can run.

        [Fact]
        public async Task GetSlugFromTitle_Returns_UniqueSlug_When_ConflictExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"SlugConflict_{Guid.NewGuid()}")
                .Options;
            await using var context = new AppDbContext(options);
            // Existing post with the conflicting slug
            context.Posts.Add(new Post { Title = "Existing Title", Slug = "existing-title", UserId = 1 });
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            // Act
            var method = typeof(PostProvider).GetMethod(
                "GetSlugFromTitle",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var task = (Task<string>)method!.Invoke(provider, new object[] { "Existing Title" })!;
            var result = await task;
        
            // Assert
            Assert.Equal("existing-title2", result);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace, malformed test file. Recommend moving using statements, adding namespace, closing braces, fixing class definition, ensuring test methods, etc.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` is not valid C# code. The file ends with an unmatched `{` (line 94) so the compiler reports **CS1513: `}` expected**.  
- `using` directives are placed inside the class body.  
- The `public class PostProviderTests` declaration never gets a matching closing `}` (and the surrounding namespace is also incomplete).  
- There are stray `await using var context = …` statements and incomplete method bodies that leave the parser inside an open block.

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file, outside any namespace or class.  
2. Wrap the test code in a proper namespace, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching braces, e.g. `public class PostProviderTests : IDisposable { … }`.  
4. Remove the stray `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every opened `{` has a corresponding `}` and that all test methods are complete and marked with `[Fact]`/`[Theory]`.  

After correcting the syntax and closing the missing brace, the project will compile and the tests can run.

        [Fact]
        public async Task GetPostsAsync_Returns_Empty_When_Page_Out_Of_Range()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"PaginationTest_{Guid.NewGuid()}")
                .Options;

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace, malformed test file. Provide recommended fixes.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` does not compile. At line 94 the compiler reports “} expected”. The file has:  
  * `using` directives placed inside the class body.  
  * An opened `public class PostProviderTests` (and possibly a namespace) that never gets a matching closing `}`.  
  * Stray/unfinished code fragments (`await using var context = …`, incomplete method bodies) that leave the parser inside an open block.  

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file, outside any namespace or class.  
2. Wrap the test code in a proper namespace, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching braces:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, test methods
   }
   ```  
4. Remove the stray `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the constructor or inside each test’s `using` block.  
5. Ensure every opened `{` has a corresponding `}` and that each test method is complete and marked with `[Fact]`/`[Theory]`.  

After correcting the braces and structure the file will compile and the tests can run.

        [Fact]
        public async Task GetSearchAsync_Wildcard_Returns_Empty_When_No_Match()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"WildcardSearchTest_{Guid.NewGuid()}")
                .Options;
        
            await using var context = new AppDbContext(options);
        
            // Seed a user
            var user = new User { Id = 1, Name = "User1" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        
            // Add posts with normal titles
            context.Posts.AddRange(
                new Post { Id = 1, UserId = user.Id, Title = "First Post", State = PostState.Release },
                new Post { Id = 2, UserId = user.Id, Title = "Second Post", State = PostState.Release }
            );
            await context.SaveChangesAsync();
        
            // Minimal mapper configuration (Post -> PostItemDto)
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Post, PostItemDto>());
            var mapper = mapperConfig.CreateMapper();
        
            var provider = new PostProvider(mapper, context);
        
            // Act
            var result = await provider.GetSearchAsync("*");
        
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: compile error due to missing closing brace in test file, malformed code, using statements inside class, incomplete class definition. Recommend fixing test file: proper namespace, using statements at top, close braces, implement test methods, ensure compilation.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- `PostProviderTests.cs` does not compile. At line 94 the compiler reports “} expected” because the file is missing closing braces and contains malformed structure:  
  * `using` directives are placed inside the class body.  
  * The `public class PostProviderTests` declaration is never closed.  
  * There are stray `await using var context = …` statements and incomplete method bodies, leaving the parser inside an open block.  

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file (outside any namespace or class).  
2. Wrap the test code in a proper namespace, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching `{` and `}` and close every opened block.  
4. Remove the stray `await using var context = …` lines; create a single in‑memory `AppDbContext` instance in the test constructor or inside each test’s `using` block.  
5. Complete any partially‑written test methods (add `[Fact]`/`[Theory]`, proper async signatures, and assertions).  

After correcting the braces and structure so the file compiles, the test run will proceed.

        [Fact]
        public async Task GetSearchAsync_Ranks_Results_And_Skips_Short_Terms()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"SearchRankTest_{Guid.NewGuid()}")
                .Options;
        
            await using var context = new AppDbContext(options);
        
            // Seed a user (required for Post.User navigation)
            var user = new User { Id = 1, Name = "TestUser" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        
            // Seed categories
            var betaCategory = new Category { Id = 1, Content = "beta" };
            context.Categories.Add(betaCategory);
            await context.SaveChangesAsync();
        
            // Helper to create PostCategory link
            PostCategory LinkCategory(Post post, Category cat) => new PostCategory { Post = post, Category = cat };
        
            // Post 1: Title contains "Alpha" and "Beta", has category "beta"
            var post1 = new Post
            {
                Id = 1,
                UserId = user.Id,
                Title = "Alpha Beta",
                Description = "Gamma",
                Content = "",
                PostType = PostType.Post,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow,
                PostCategories = new List<PostCategory>()
            };
            post1.PostCategories.Add(LinkCategory(post1, betaCategory));
            context.Posts.Add(post1);
        
            // Post 2: Title does not contain "Alpha", description contains two "Alpha"
            var post2 = new Post
            {
                Id = 2,
                UserId = user.Id,
                Title = "Delta",
                Description = "Alpha Alpha",
                Content = "",
                PostType = PostType.Post,
                State = PostState.Release,
                PublishedAt = DateTime.UtcNow.AddHours(-1),
                PostCategories = new List<PostCategory>()
            };
            context.Posts.Add(post2);
        
            await context.SaveChangesAsync();
        
            // Configure AutoMapper for the needed projections
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostItemDto>()
                   .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                   .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                   .ForMember(d => d.Categories, opt => opt.MapFrom(s => s.PostCategories.Select(pc => pc.Category)));
                cfg.CreateMap<Category, CategoryDto>();
            });
            var mapper = mapperConfig.CreateMapper();
        
            var provider = new PostProvider(mapper, context);
        
            // Act
            var result = await provider.GetSearchAsync("Alpha be", page: 1, pageSize: 10);
        
            // Assert
            Assert.NotNull(result);
            // Expect two items ordered by rank descending: Post1 (rank 10) then Post2 (rank 6)
            var items = result.Items.ToList();
            Assert.Equal(2, items.Count);
            Assert.Equal("Alpha Beta", items[0].Title);
            Assert.Equal("Delta", items[1].Title);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to produce short concise analysis: compile error due to missing closing brace in test file, malformed code, using statements inside class, incomplete class definition. Recommend fixing test file: proper namespace, using statements at top, close braces, correct class definition, implement tests. Also ensure file compiles.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
The test file `PostProviderTests.cs` does not compile. At line 94 the compiler reports “} expected” because the file has:

* Missing opening/closing braces for the namespace and the `PostProviderTests` class.  
* `using` directives placed **inside** the class body.  
* Duplicate/unfinished `using var context = …` statements and other stray code fragments.  
* Incomplete method/property definitions and truncated code, leaving the parser waiting for a closing `}`.

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file (outside any namespace).  
2. Wrap the tests in a proper namespace block, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching `{` and `}` and implement the required test methods (decorate with `[Fact]`/`[Theory]`).  
4. Remove the stray `using var context = …` lines; create a single in‑memory `AppDbContext` instance in the test constructor or inside each test’s `using` block.  
5. Ensure every opened `{` is closed – especially after the class, namespace, and each method.  

After correcting the syntax and completing the test class, the project will build and the tests can run.

        [Fact]
        public async Task GetAsync_BySlug_Excludes_Older_Newer_From_Related_And_Increments_Views()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"SlugFullTest_{Guid.NewGuid()}")
                .Options;
            using var context = new AppDbContext(options);
            var user = new User { Id = 1, Name = "TestUser" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output short concise analysis: test file has syntax errors, missing braces, misplaced using statements, incomplete class definition. Recommend fixing file: proper namespace, using statements at top, close braces, implement test methods, ensure compilation.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
- The test file `PostProviderTests.cs` is not valid C# code. It contains stray `using` directives inside the class, duplicate/unfinished `using var context = …` statements, and a missing closing brace (`}`) at line 94, causing a compile‑time CS1513 error.  

**Recommended fixes**  
1. Move **all** `using` statements to the top of the file (outside any namespace or class).  
2. Wrap the tests in a proper namespace block, e.g. `namespace Blogifier.Tests { … }`.  
3. Define the test class with matching opening and closing braces:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, Dispose, test methods
   }
   ```  
4. Remove the duplicated/unfinished `using var context = new AppDbContext(_options);` lines. Create a single in‑memory `AppDbContext` instance in the constructor (or inside each test’s `using` block).  
5. Ensure every method, property, and class has its closing `}`. After fixing the braces the file should compile.  
6. Add the missing `[Fact]`/`[Theory]` test methods (or uncomment existing ones) so the test runner has something to execute.  

Once the file is syntactically correct and all braces are balanced, the project will build and the tests can run.

        [Fact]
        public async Task UpdateAsync_Throws_When_User_Not_Owner()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"UpdateAuthTest_{Guid.NewGuid()}")
                .Options;
            using var context = new AppDbContext(options);
            var post = new Post
            {
                Id = 1,
                UserId = 10,
                Title = "Original",
                Slug = "original",
                Content = "content",
                Description = "desc",
                PostType = PostType.Post,
                State = PostState.Draft,
                PublishedAt = null
            };
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            var updateDto = new PostEditorDto
            {
                Id = 1,
                Title = "Updated",
                Slug = "updated",
                Content = "new content",
                Description = "new desc",
                PostType = PostType.Post,
                State = PostState.Draft,
                PublishedAt = null,
                Categories = null
            };
        
            // Act & Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () =>
                await provider.UpdateAsync(updateDto, userId: 20)); // different user
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to output concise analysis: test file has syntax errors, missing braces, incomplete methods, using statements inside class, etc. Provide recommended fixes: rewrite test file with proper namespace, class, using directives at top, close braces, implement test methods, ensure compilation.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
The test source `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler stops at line 94 with **CS1513: `}` expected** because the file contains multiple structural problems:

1. **Missing opening/closing braces** for the `namespace` and the `PostProviderTests` class.  
2. **`using` directives placed inside the class body** (e.g., `using System;` after the class declaration).  
3. **Method signatures without bodies** (e.g., `public async Task AddAsync_CreatesNewPost_ReturnsSlug` has no `{ … }`).  
4. **Incomplete object initializers and control blocks** that leave the parser inside an open block when the file ends.  
5. **Duplicate/unfinished `using var context = …` statements** and stray code fragments scattered throughout the file.

Because the file cannot be compiled, no tests are discovered and the build fails.

**Recommended fixes**  

1. **Re‑structure the file** – place all `using` statements at the very top, outside any namespace or class.  
2. Add a proper namespace block:  
   ```csharp
   namespace Blogifier.Tests
   {
       // test class here
   }
   ```  
3. Define the test class with opening and closing braces:  
   ```csharp
   public class PostProviderTests : IDisposable
   {
       // fields, ctor, Dispose, test methods
   }
   ```  
4. Ensure every method (including the test methods) has a complete body surrounded by `{ … }`.  
5. Remove the stray `using var context = new AppDbContext(_options);` lines; instead create the in‑memory `AppDbContext` inside the constructor or inside each test’s `using` block.  
6. Close all opened `{` with matching `}` – especially after object initializers, `if`/`foreach` blocks, and the class itself.  
7. Add `[Fact]` (or `[Theory]`) attributes to each test method and make them `public async Task …()` (or `Task` for sync tests).  
8. Implement any missing logic or replace placeholder comments with real code so the file compiles.

After correcting the syntax and providing complete method bodies, the project will build and the tests can be executed.

        [Fact]
        public void GetPublishedAt_Returns_Correct_Value_Based_On_State()
        {
            // Arrange
            var method = typeof(PostProvider).GetMethod("GetPublishedAt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            // Case A: state >= Release, publishedAt null -> returns non‑null DateTime
            var resultA = (DateTime?)method!.Invoke(null, new object[] { null, PostState.Release });
            Assert.NotNull(resultA);
            // The returned value should be close to now (within a minute)
            Assert.True((DateTime.UtcNow - resultA!.Value).TotalMinutes < 1);
        
            // Case B: state < Release (Draft), publishedAt provided -> returns null
            var someDate = new DateTime(2023, 1, 1);
            var resultB = (DateTime?)method.Invoke(null, new object[] { someDate, PostState.Draft });
            Assert.Null(resultB);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to produce short concise analysis: test file syntax errors, missing braces, malformed code, using statements inside class, incomplete methods. Recommend fixing file: proper namespace, class, using statements at top, add missing braces, complete method bodies, ensure test methods have [Fact] and async Task, compile. Also remove duplicate using var context lines. Ensure file compiles.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
The test source `tests/Blogifier.Tests/PostProviderTests.cs` does not compile. The compiler stops at line 94 with “`}` expected” because the file contains many structural problems:

1. **Missing opening/closing braces** for the `namespace` and the `PostProviderTests` class.  
2. **`using` directives placed inside the class body** (e.g., `using System;` after the class declaration).  
3. **Method signatures without bodies** (e.g., `public async Task AddAsync_CreatesNewPost_ReturnsSlug` has no `{ … }`).  
4. **Duplicate/unfinished variable declarations** (`using var context = …` appears twice and is not closed).  
5. **Incomplete object initializers and control blocks** that leave the parser inside an open block when the file ends.  

Because the file cannot be compiled, no tests are discovered and the build fails with CS1513.

**Recommended fixes**

1. **Re‑structure the file**  
   ```csharp
   // top of file
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
   using Blogifier.Helper;

   namespace Blogifier.Tests
   {
       public class PostProviderTests : IDisposable
       {
           // fields, ctor, Dispose, test methods …
       }
   }
   ```
2. **Add the missing `{` and `}`** for the namespace, class, and every method.  
3. **Remove duplicate/invalid `using var context …` lines** and create a single in‑memory `AppDbContext` instance in the test constructor (or in each test with a `using` block).  
4. **Complete each test method**: give it a body, mark it with `[Fact]` (or `[Theory]`), and return a `Task` if async.  
5. **Close all object initializers** (`new PostEditorDto { … }`) and any `if`, `foreach`, etc., with proper braces.  
6. **Run the build again** – the file should now compile and the tests will be discovered and executed.

        [Fact]
        public async Task CheckPostCategories_Deduplicates_And_Merges_With_Existing()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"CheckCatTest_{Guid.NewGuid()}")
                .Options;
            using var context = new AppDbContext(options);
            // Seed categories that exist in DB
            context.Categories.AddRange(
                new Category { Content = "Tech" },
                new Category { Content = "News" },
                new Category { Content = "Science" });
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            // Original PostCategories (Tech, News)
            var original = new List<PostCategory>
            {
                new PostCategory { Category = new Category { Content = "Tech" } },
                new PostCategory { Category = new Category { Content = "News" } }
            };
        
            // Input CategoryDto list with duplicate Tech and new Science
            var input = new List<CategoryDto>
            {
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "Tech" },
                new CategoryDto { Content = "Science" }
            };
        
            // Act
            var method = typeof(PostProvider).GetMethod("CheckPostCategories",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task<List<PostCategory>>)method!
                .Invoke(provider, new object[] { input, original })!;
            var result = await task;
        
            // Assert
            Assert.NotNull(result);
            // Should contain three categories: Tech, News, Science
            Assert.Equal(3, result!.Count);
            var contents = result.Select(pc => pc.Category!.Content).ToList();
            Assert.Contains("Tech", contents);
            Assert.Contains("News", contents);
            Assert.Contains("Science", contents);
            // No duplicate Tech
            Assert.Equal(1, contents.Count(c => c == "Tech"));
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to provide short concise analysis: test run failed due to syntax errors in test file, missing braces, malformed code. Recommend fixing the test file: proper namespace, class, using statements, method bodies, close braces, compile. Also ensure test methods are correctly attributed and async. Possibly remove duplicate using statements. Provide actionable steps.<|end|><|start|>assistant<|channel|>final<|message|>**Why the test run failed**  
The test project does not compile because `tests/Blogifier.Tests/PostProviderTests.cs` is syntactically invalid. At line 94 the compiler reports “`}` expected”, which is a symptom of many earlier problems:

* `using` directives are placed inside the class body.  
* The `namespace` and `class` declarations are missing opening/closing braces.  
* Method signatures (e.g., `public async Task AddAsync_CreatesNewPost_ReturnsSlug`) have no `{` and contain incomplete bodies.  
* Several `using` statements, variable declarations, and control‑flow blocks are left dangling, leaving the parser inside an open block when it reaches the end of the file.  

Because the file cannot be compiled, no tests are discovered or executed, resulting in the build error.

**Recommended fixes (concise)**

1. **Re‑create a valid test file**  
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
   using Blogifier.Helper;

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
                   .UseInMemoryDatabase($"BlogifierTest_{Guid.NewGuid()}")
                   .Options;
           }

           public void Dispose()
           {
               // clean up the in‑memory DB if needed
           }

           [Fact]
           public async Task AddAsync_CreatesNewPost_ReturnsSlug()
           {
               // Arrange
               var userId = 1;
               var postInput = new PostEditorDto
               {
                   Title = "Test Post",
                   Content = "Content",
                   Description = "Desc",
                   State = PostState.Draft,
                   PostType = PostType.Post,
                   Categories = null
               };

               await using var context = new AppDbContext(_options);
               var provider = new PostProvider(_mapperMock.Object, context);

               // Act
               var slug = await provider.AddAsync(postInput, userId);

               // Assert
               Assert.False(string.IsNullOrWhiteSpace(slug));
               var saved = await context.Posts.FirstAsync(p => p.Slug == slug);
               Assert.Equal(postInput.Title, saved.Title);
           }

           // …additional test methods (GetAsync, GetSlugFromTitle, etc.)…
       }
   }
   ```

2. **Key points to ensure compilation**
   * All `using` statements are **outside** any namespace/class.  
   * The `namespace` block opens with `{` and closes with `}`.  
   * The test class opens with `{` and closes with `}`.  
   * Every method has a complete body surrounded by `{ … }`.  
   * Add the `[Fact]` (or `[Theory]`) attribute to each test method.  
   * Implement `IDisposable` (or use `IAsyncLifetime`) to clean up the in‑memory DB if needed.

3. **Run the tests again**  
   After fixing the file, restore and rebuild the solution (`dotnet test`). The compilation error will disappear and the actual unit tests for `PostProvider` will execute.

By correcting the syntax and providing proper test method implementations, the test suite will compile and run, allowing you to verify the behavior of `PostProvider`.

        [Fact]
        public async Task GetSlugFromTitle_Throws_When_MaxAttemptsExceeded()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"SlugMaxTest_{Guid.NewGuid()}")
                .Options;
            using var context = new AppDbContext(options);
            // Create 100 conflicting slugs: duplicate, duplicate2 ... duplicate100
            for (int i = 1; i <= 100; i++)
            {
                var slug = i == 1 ? "duplicate" : $"duplicate{i}";
                context.Posts.Add(new Post { Title = $"Duplicate {i}", Slug = slug, UserId = 1 });
            }
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            // Act
            var method = typeof(PostProvider).GetMethod("GetSlugFromTitle",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task<string>)method!.Invoke(provider, new object[] { "Duplicate" })!;
        
            // Assert
            await Assert.ThrowsAsync<BlogNotIitializeException>(async () => await task);
        }

*/
/*
FAILED TEST: <|channel|>analysis<|message|>We need to analyze the test file. The compile error at line 94, column 2: "} expected". The test file is malformed: missing braces, incorrect syntax. The file seems truncated and has many syntax errors: missing class braces, using statements inside class, etc. Need to rewrite the test file with proper structure.

Provide summary: test file has syntax errors, missing braces, incorrect using statements, incomplete method bodies, missing brackets. Fix by correcting the test class definition, adding proper braces, completing test methods, ensuring using directives at top, etc.

Also ensure namespace braces.

Thus answer: compile error due to malformed test file; fix by correcting syntax.<|end|><|start|>assistant<|channel|>final<|message|>**Why the tests failed**

The compilation error `CS1513: } expected` comes from `PostProviderTests.cs`. The file is not valid C# – it contains:

* Missing opening/closing braces for the namespace and class.
* `using` statements placed inside the class body.
* Incomplete/incorrect method signatures (e.g., `public async Task AddAsync_CreatesNewPost_ReturnsSlug` without `{` and missing `Fact` attribute body).
* Unbalanced braces throughout the file, causing the compiler to reach the end of the file while still expecting a closing `}`.

Because of these syntax errors the test project cannot compile, so no tests are executed.

**Recommended fixes**

1. **Re‑structure the file**  
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
   using Blogifier.Helper;

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
                   .UseInMemoryDatabase($"BlogifierTest_{Guid.NewGuid()}")
                   .Options;
           }

           public void Dispose()
           {
               // clean up the in‑memory DB if needed
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

               _mapperMock.Setup(m => m.Map<Post>(It.IsAny<PostEditorDto>()))
                          .Returns(expectedPost);

               await using var context = new AppDbContext(_options);
               var provider = new PostProvider(_mapperMock.Object, context);

               var postInput = new PostEditorDto
               {
                   Title = postTitle,
                   Content = postContent,
                   Description = "Test post description",
                   PostType = PostType.Post,
                   State = PostState.Draft,
                   Categories = null,
                   Cover = null,
                   PublishedAt = null
               };

               // Act
               var slug = await provider.AddAsync(postInput, userId);

               // Assert
               Assert.Equal(postSlug, slug);
           }

           // Add any other test methods here, each with proper braces and attributes
       }
   }
   ```

2. **Ensure every opening `{` has a matching `}`** – IDEs or `dotnet format` can help spot mismatches.

3. **Place all `using` directives at the top of the file**, outside any namespace or class.

4. **Complete each test method** (add the missing body, proper `Fact` attribute, and any required setup/teardown).

5. **Run `dotnet build`** after fixing the file to confirm the project compiles before re‑running the tests.

Once the syntax errors are resolved and the test methods are correctly implemented, the test suite will compile and execute, allowing you to see real test results.

        [Fact]
        public async Task GetSlugFromTitle_Returns_UniqueSlug_When_ConflictExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"SlugTest_{Guid.NewGuid()}")
                .Options;
            using var context = new AppDbContext(options);
            // Existing post with conflicting slug
            context.Posts.Add(new Post { Title = "Existing Title", Slug = "existing-title", UserId = 1 });
            await context.SaveChangesAsync();
        
            var provider = new PostProvider(new Mock<IMapper>().Object, context);
        
            // Act
            var method = typeof(PostProvider).GetMethod("GetSlugFromTitle",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task<string>)method!.Invoke(provider, new object[] { "Existing Title" })!;
            var result = await task;
        
            // Assert
            Assert.Equal("existing-title2", result);
        }

*/
