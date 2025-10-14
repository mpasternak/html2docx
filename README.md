# html2docx-dotnet
As pandoc is failing on VMWare ESX with Core Dumped, another solution emerges...

## Installation

### Prerequisites
This application requires **.NET 8.0** or later.

### Option 1: Ubuntu Linux

1. Install .NET 8.0:
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Install .NET 8.0 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

2. Build and install:
```bash
make build
sudo make install
```

### Option 2: macOS

1. Install .NET 8.0 using Homebrew:
```bash
brew install dotnet@8
```

2. Build and install:
```bash
make build
sudo make install
```

This installs the tool to /opt/local/lib/html2docx and the wrapper script to /opt/local/bin/html2docx.

## Usage Examples (Native Installation)

After installation, you can use the `html2docx` command directly:

```bash
# Read from stdin, write to stdout
echo "<h1>Hello World</h1>" | html2docx > output.docx

# Read from file, write to stdout
html2docx input.html > output.docx

# Read from stdin, write to file
echo "<h1>Hello World</h1>" | html2docx - output.docx

# Read from file, write to file
html2docx input.html output.docx

# Show help
html2docx --help

# Convert HTML file with complex content
html2docx report.html report.docx

# Pipe HTML from another command
curl -s "https://example.com" | html2docx > webpage.docx
```

### Option 3: Docker

#### Build the Docker image:
```bash
make docker
# or manually:
docker build -t iplweb/html2docx:latest .
```

#### Push to Docker Hub:
```bash
make docker-push
# or manually:
docker push iplweb/html2docx:latest
```

#### Docker Usage Examples:

**Example 1: Basic usage with pipe**
```bash
echo "<h1>Hello World</h1><p>This is a test.</p>" | docker run --rm -i iplweb/html2docx:latest > output.docx
```

**Example 2: Interactive usage with mounted files**
```bash
docker run --rm -it -v $(pwd)/input.html:/input.html -v $(pwd)/output.docx:/output.docx iplweb/html2docx:latest /input.html /output.docx
```

**Example 3: Docker Compose usage**
Create a `docker-compose.yml` file:
```yaml
version: '3.8'
services:
  html-converter:
    image: iplweb/html2docx:latest
    volumes:
      - ./input.html:/input.html:ro
      - ./output.docx:/output.docx
    command: ["/input.html", "/output.docx"]
```

Run with:
```bash
docker-compose up
```

**Example 4: Using as a service in Docker Compose**
```yaml
version: '3.8'
services:
  web-app:
    image: your-web-app:latest
    depends_on:
      - html-converter
    # ... your web app configuration
  
  html-converter:
    image: iplweb/html2docx:latest
    # This service can be used by other containers
    # via Docker network: docker exec web-app html2docx ...
```

## Usage

The program supports the following command-line arguments:

```bash
html2docx [input_file] [output_file]
```

- **input_file**: HTML file to convert (use `-` for stdin, default: stdin)
- **output_file**: DOCX output file (default: stdout)
- **--help**: Show help message

### Examples

```bash
# Read from stdin, write to stdout
echo "<h1>Hello World</h1>" | html2docx > output.docx

# Read from file, write to stdout
html2docx input.html > output.docx

# Read from stdin, write to file
echo "<h1>Hello World</h1>" | html2docx - output.docx

# Read from file, write to file
html2docx input.html output.docx

# Show help
html2docx --help
```

