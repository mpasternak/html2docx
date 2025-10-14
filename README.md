# html2docx-dotnet

## Project Description

html2docx-dotnet is a .NET 8.0 command-line tool that converts HTML documents to DOCX format. This project was created as an alternative to pandoc when experiencing core dump issues on VMWare ESX environments.

The application provides a simple, efficient way to convert HTML content to Microsoft Word documents without the complexity and dependencies of larger conversion tools. It's designed to handle standard HTML markup and generate properly formatted DOCX files suitable for further editing or distribution.

### Key Features

- Lightweight .NET 8.0 application
- Command-line interface for easy integration
- Support for stdin/stdout pipeline operations
- Docker containerization for cross-platform deployment
- Direct file-to-file conversion
- Simple, dependency-light implementation

## ⚠️ Security Warning

**This software parses HTML as-is without any cleaning, validation, or sanitization.**

For some cases, this could be considered a gaping security hole. 

- IT WILL DOWNLOAD THINGS OVER THE INTERNET, IF LINKED IN HTML FILE
- It does not perform HTML validation or cleanup
- It is prone to fail when the HTML file is malformed or contains invalid markup
- It does not protect against malicious HTML content
- Use with caution and only with trusted HTML sources

TL,DR: if it fails on compilicated web page with a lot of JavaScript and CSS, use some other tool like bleach before conversion. 

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

## ⚠️ Security Warning

**This software parses HTML as-is without any cleaning, validation, or sanitization.** 

- It does not perform HTML validation or cleanup
- It is prone to fail when the HTML file is malformed or contains invalid markup
- It does not protect against malicious HTML content
- Use with caution and only with trusted HTML sources

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

