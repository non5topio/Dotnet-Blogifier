FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test

WORKDIR /app

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

# Install testing tools with specific version and added retry mechanism
RUN dotnet tool update -g dotnet-reportgenerator-globaltool --version 5.1.13 || \
    (sleep 5 && dotnet tool update -g dotnet-reportgenerator-globaltool --version 5.1.13)

# Add dotnet tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Directory for your test generation app
RUN mkdir -p /test-gen

# This is where you'll add your binary app using a docker cp command after building this image
# For example: docker cp your-test-gen-app.dll container-id:/test-gen/

# Mount point for test generation configuration
VOLUME ["/test-gen-config"]

# Run tests with your custom configuration
CMD ["bash", "-c", "dotnet test && echo 'Running test generation tool' && dotnet /test-gen/your-test-gen-app.dll /test-gen-config"]
