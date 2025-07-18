FROM mcr.microsoft.com/dotnet/sdk:8.0.101 AS test

WORKDIR /app

# Install Node.js and npm
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Copy solution and project files
COPY *.sln ./
COPY src/Blogifier/*.csproj ./src/Blogifier/
COPY src/Blogifier.Admin/*.csproj ./src/Blogifier.Admin/
COPY src/Blogifier.Shared/*.csproj ./src/Blogifier.Shared/
COPY src/Blogifier.Themes.Standard/*.csproj ./src/Blogifier.Themes.Standard/
COPY tests/Blogifier.Tests/*.csproj ./tests/Blogifier.Tests/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Install testing tools
RUN dotnet --info && \
    dotnet tool install -g dotnet-reportgenerator-globaltool || \
    dotnet tool update -g dotnet-reportgenerator-globaltool

# Add dotnet tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Directory for your test generation app
RUN mkdir -p /test-gen

# Run tests with code coverage
CMD ["bash", "-c", "dotnet test --collect:'XPlat Code Coverage' --results-directory ./tests/TestResults && find ./tests/TestResults -name 'coverage.cobertura.xml' -exec cp {} ./tests/TestResults/coverage.cobertura.xml \\; && echo 'Tests completed with coverage'"]
