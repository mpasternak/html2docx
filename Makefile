.PHONY: help build install docker docker-push

# Default target
help:
	@echo "Available targets:"
	@echo "  help       - Show this help message"
	@echo "  build      - Build the .NET application using dotnet"
	@echo "  install    - Install the application to /opt/local/bin/"
	@echo "  docker     - Build Docker image with tag iplweb/html2docx:latest"
	@echo "  docker-push- Build and push Docker image to Docker Hub"

# Build the .NET application
build:
	@echo "Building the .NET application..."
	cd src && dotnet build -c Release

# Install the application to /opt/local/bin/
install: build
	@echo "Installing application to /opt/local/lib/..."
	@mkdir -p /opt/local/lib
	cd src && dotnet publish -c Release -o /opt/local/lib/html2docx /p:UseAppHost=false
	@echo "Creating shell script wrapper..."
	@echo '#!/bin/bash' > /opt/local/bin/html2docx
	@echo 'dotnet /opt/local/lib/html2docx/HtmlToDocx.dll "$$@"' >> /opt/local/bin/html2docx
	@chmod +x /opt/local/bin/html2docx
	@echo "Installation complete. Run with: html2docx [options]"

# Build Docker image
docker:
	@echo "Building Docker image..."
	docker build -t iplweb/html2docx:latest .

# Build and push Docker image to Docker Hub
docker-push: docker
	@echo "Pushing Docker image to Docker Hub..."
	docker push iplweb/html2docx:latest
