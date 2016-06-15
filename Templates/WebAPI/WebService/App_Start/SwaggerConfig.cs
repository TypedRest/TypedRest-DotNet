using System;
using System.IO;
using System.Web.Http;
using Swashbuckle.Application;

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Configuration for Swagger API documentation.
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Enables and configures Swagger API documentation.
        /// </summary>
        public static void ConfigureSwagger(this HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "XProjectNameX API");
                c.IncludeXmlCommentsDir(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath));
                c.DescribeAllEnumsAsStrings();
            }).EnableSwaggerUi(c => { c.DisableValidator(); });
        }

        private static void IncludeXmlCommentsDir(this SwaggerDocsConfig config, string dirPath)
        {
            foreach (string filePath in Directory.GetFiles(dirPath, "*.xml"))
                config.IncludeXmlComments(filePath);
        }
    }
}
