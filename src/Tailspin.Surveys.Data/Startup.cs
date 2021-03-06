﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// TODO: Remove. Will be unnecessary when bug #2357 fixed
// See https://github.com/aspnet/EntityFramework/issues/2357
// Also https://github.com/aspnet/EntityFramework/issues/2256

using System; //Needed for KeyVaultConfigurationProvider
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using Tailspin.Surveys.Data.Configuration;
using Tailspin.Surveys.Data.DataModels;

public class Startup
{
    private ConfigurationOptions _configOptions = new ConfigurationOptions();
    public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv, ILoggerFactory loggerFactory)
    {
        InitializeLogging(loggerFactory);
        var builder = new ConfigurationBuilder()
            .SetBasePath(appEnv.ApplicationBasePath)
            .AddJsonFile("../Tailspin.Surveys.Web/appsettings.json"); // path to your original configuration in Web project
        if (env.IsDevelopment())
        {
            // This reads the configuration keys from the secret store.
            // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            builder.AddUserSecrets();
        }
        //Uncomment the block of code below to use a connection string from KeyVault for migrations
//#if DNX451
//        var config = builder.Build();
//        builder.AddKeyVaultSecrets(config["ClientId"],
//            config["KeyVault:Name"],
//            config["Asymmetric:CertificateThumbprint"],
//            Convert.ToBoolean(config["Asymmetric:ValidationRequired"]),
//            loggerFactory);
//#endif
        builder.Build().Bind(_configOptions);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEntityFramework()
            .AddSqlServer()
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_configOptions.Data.SurveysConnectionString));
    }
    public void Configure() { }
    private void InitializeLogging(ILoggerFactory loggerFactory)
    {
        loggerFactory.MinimumLevel = LogLevel.Information;
        loggerFactory.AddDebug(LogLevel.Information);
    }
}