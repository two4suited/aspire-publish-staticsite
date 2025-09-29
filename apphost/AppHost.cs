var builder = DistributedApplication.CreateBuilder(args);

builder.AddViteApp("static-site", "../static-site")
    .WithNpmPackageInstallation();

builder.Build().Run();
