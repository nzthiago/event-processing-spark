# Find the Dockerfile for mcr.microsoft.com/azure-functions/dotnet:3.0-dotnet3-core-tools at this URL
FROM mcr.microsoft.com/azure-functions/dotnet:3.0-dotnet3-core-tools

# Avoid warnings by switching to noninteractive
ENV DEBIAN_FRONTEND=noninteractive

COPY library-scripts/bicep-debian.sh /tmp/library-scripts/
RUN bash /tmp/library-scripts/bicep-debian.sh \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts/bicep-debian.sh