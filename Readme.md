# Aspire Static Site with Azure Front Door

This project demonstrates how to build and deploy a static site using .NET Aspire orchestration to Azure Storage with Azure Front Door CDN. The project is based on Safia Abdalla's blog post: [Building custom deployment pipelines with Aspire](https://blog.safia.rocks/2025/09/07/aspire-deploy/).

## Architecture

The deployment architecture consists of:

- **Vite Static Site**: A TypeScript-based static website built with Vite
- **Azure Storage**: Hosts the static website files with public blob access enabled
- **Azure Front Door**: Provides CDN, routing, and SSL termination
- **.NET Aspire AppHost**: Orchestrates local development and cloud deployment

## Key Features

- Custom deployment pipeline using Aspire's `DeployingCallbackAnnotation`
- Infrastructure as Code using Bicep templates through Aspire's Azure CDK
- Automated build and deployment of static assets
- Integration between Azure Storage and Front Door for optimal performance
- Local development orchestration with production deployment capabilities

## Prerequisites

- .NET 9 SDK or later
- Node.js and npm
- Azure subscription
- Aspire CLI (dev builds recommended)

### Install Aspire CLI

```bash
curl -sSL https://aspire.dev/install.sh | bash -s -- -q dev
```

## Project Structure

```
├── static-site/           # Vite application
├── aspire-apphost/        # .NET Aspire AppHost
│   ├── AppHost.cs        # Main orchestration logic
│   └── front-door.bicep  # Azure Front Door Bicep template
└── README.md
```

## Getting Started

### 1. Create the Vite Application

```bash
npm create vite@latest static-site -- --template vanilla-ts
```

### 2. Create the Aspire AppHost

```bash
aspire new aspire-apphost
```

### 3. Add Node.js Extensions

```bash
cd aspire-apphost
aspire add nodejs
aspire add communitytoolkit-nodejs-extensions
```

### 4. Local Development

Run the application locally with Aspire dashboard:

```bash
aspire run
```

This will start:
- The Vite development server
- Aspire dashboard for monitoring

## Deployment

### Deploy to Azure

```bash
aspire deploy
```

The deployment process:

1. **Build Phase**: Runs `npm run build` to create optimized static assets
2. **Infrastructure Phase**: Provisions Azure Storage and Front Door resources
3. **Configuration Phase**: Enables static website hosting on Azure Storage
4. **Upload Phase**: Uploads built assets to the `$web` container
5. **Completion**: Displays the Front Door endpoint URL

## Key Implementation Details

### Execution Modes

The AppHost operates in different modes:
- **RunMode**: Active during `aspire run` for local development
- **PublishMode**: Active during `aspire publish` and `aspire deploy`

### Custom Azure Resources

Since Azure Front Door doesn't have first-class Aspire integration yet, this project uses a custom `AzureBicepResource` wrapper:

```csharp
class AzureFrontDoorResource(string name) : AzureBicepResource(name, templateFile: "front-door.bicep") { }
```

### Deployment Pipeline

The custom deployment logic uses three main steps:

1. `TryBuildStaticSite`: Builds the Vite site locally
2. `TryConfigureStaticWebsite`: Configures Azure Storage for static hosting
3. `TryUploadStaticFiles`: Uploads assets to the `$web` container

### Infrastructure Configuration

The project uses `ConfigureInfrastructure` API to modify Bicep templates:

```csharp
var storage = builder.AddAzureStorage($"{name}-storage")
    .ConfigureInfrastructure(infra =>
    {
        var storage = infra.GetProvisionableResources().OfType<StorageAccount>().Single();
        storage.AllowBlobPublicAccess = true; // Required for static website support
    });
```

## Learning Resources

- [Original Blog Post](https://blog.safia.rocks/2025/09/07/aspire-deploy/) by Safia Abdalla
- [Reference Implementation](https://github.com/captainsafia/aspire-static-site-deploy)
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Azure CDK Documentation](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/provisioning)

## Key Takeaways

1. **Leverage the AppHost**: Use Aspire's AppHost for both local development and deployment orchestration
2. **Custom Deployment Callbacks**: The `DeployingCallbackAnnotation` enables custom deployment pipelines
3. **Infrastructure as Code**: Use `ConfigureInfrastructure` API to customize Azure resources beyond default integrations
4. **Execution Context Awareness**: Different behavior for RunMode vs PublishMode ensures proper local/cloud separation

## Future Enhancements

Potential areas for extension:
- Custom domain support
- Private blob access with Front Door origin configuration
- Enhanced deployment progress reporting
- Multi-environment deployment strategies
- Automated testing pipeline integration

## Contributing

This project serves as a reference implementation for deploying static sites with Aspire. Feel free to adapt the patterns for your specific use cases and contribute improvements back to the community.