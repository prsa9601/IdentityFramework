﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tools
{
    public class SwaggerDocument : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        public SwaggerDocument(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var item in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(item.GroupName,
      new OpenApiInfo()
      {
          Title = $"Api Version {item.ApiVersion}",
          Version = item.ApiVersion.ToString()
      });
            }


            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "این کادر برای احراز هویت (jwt) هست \r\n\r\n" +
                "برای تست بعد از 'Bearer' \r\n\r\n" +
                " و یک فاصله توکن خود را وارد کنید .\r\n\r\n" +
                "example : Bearer dl;fgkd;fgldfgjdlfkgjiogjighuieredghsdklfghsjkdfhsdkljfl",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });


            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        },
                        Name="Bearer",
                        In=ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            var pathComment = Path.Combine(AppContext.BaseDirectory, "SwaggerComment.xml");
            options.IncludeXmlComments(pathComment);
        }



    }
    
}
