﻿using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notes.WebApi
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        // получим данные о версиях
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var apiVersion = description.ApiVersion.ToString();
                options.SwaggerDoc(description.GroupName,
                    new OpenApiInfo
                    {
                        // описание версии (можно добавить условия, лицензию и т.д.)
                        Version = apiVersion,
                        Title = $"Notes API {apiVersion}",
                        Description ="A simple example ASP NET Core Web API",
                        TermsOfService = new Uri("https://www.youtube.com/c/PlatinumTechTalks"),
                        Contact = new OpenApiContact
                        {
                            Name = " P Chat",
                            Email = string.Empty,
                            Url = new Uri("https://t.me/platinum_chat")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "P Telegram Channel",
                            Url = new Uri("https://t.me/platinum_tech_talks")
                        }
                    });

                // для возможности авторизации прямов в Swagger
                options.AddSecurityDefinition($"AuthToken {apiVersion}",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer",
                        Name = "Authorization",
                        Description = "Authorization token"
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = $"AuthToken {apiVersion}"
                            }
                        }, new string[] {}
                    }
                });

                options.CustomOperationIds(apiDescription =>
                    apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)
                       ? methodInfo.Name
                       : null);
            }
        }
    }
}
