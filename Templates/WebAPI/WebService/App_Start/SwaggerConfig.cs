using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace XProjectNamespaceX.WebService
{
    public static class SwaggerConfig
    {
        public static void EnableSwagger(this HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "XProjectNameX API");
                c.BasicAuth("basic").Description("Basic HTTP Authentication");
                c.IncludeXmlCommentsDir(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));
                c.OperationFilter<CancellationTokenFilter>();
            }).EnableSwaggerUi(c => { c.DisableValidator(); });
        }

        private static void IncludeXmlCommentsDir(this SwaggerDocsConfig config, string dirPath)
        {
            foreach (string filePath in Directory.GetFiles(dirPath, "*.xml"))
                config.IncludeXmlComments(filePath);
        }

        /// <summary>
        /// Hides <see cref="CancellationToken"/> parameters.
        /// </summary>
        private class CancellationTokenFilter : IOperationFilter
        {
            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                if (operation.parameters == null) return;

                foreach (var parameterDesc in apiDescription.ParameterDescriptions.Where(
                    x => x.ParameterDescriptor.ParameterType == typeof(CancellationToken)))
                {
                    operation.parameters.Remove(operation.parameters.Single(x => x.name == parameterDesc.Name));
                }
            }
        }
    }
}
