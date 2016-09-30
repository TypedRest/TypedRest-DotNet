using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

namespace XProjectNamespaceX.Service
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
                c.IncludeXmlCommentsDir(AppDomain.CurrentDomain.BaseDirectory);
                c.DescribeAllEnumsAsStrings();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                c.RootUrl(req => req.RequestUri.GetLeftPart(UriPartial.Authority) + req.GetRequestContext().VirtualPathRoot);
            }).EnableSwaggerUi(c => { c.DisableValidator(); });
        }

        private static void IncludeXmlCommentsDir(this SwaggerDocsConfig config, string dirPath)
        {
            foreach (string filePath in Directory.GetFiles(dirPath, "*.xml"))
                config.IncludeXmlComments(filePath);
        }
    }
}
