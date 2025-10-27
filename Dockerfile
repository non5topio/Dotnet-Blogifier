FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test

WORKDIR /app

# Install Node.js (minimal - needed by some projects in the solution)
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Copy all the code
COPY . .

# Run tests with code coverage - dotnet test will automatically restore dependencies
CMD ["bash", "-c", "time dotnet test ./tests/Blogifier.Tests/Blogifier.Tests.csproj --filter \"FullyQualifiedName=Blogifier.Tests.PostProviderTests.AddAsync_CreatesNewPost_ReturnsSlug\" --collect:'XPlat Code Coverage' --results-directory ./tests/TestResults --verbosity minimal && find ./tests/TestResults -name 'coverage.cobertura.xml' -exec cp {} ./tests/TestResults/coverage.cobertura.xml \\; && echo 'Tests completed with coverage'"]
