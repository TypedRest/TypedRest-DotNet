using System;
using System.IO;
using System.Web.Http;
using Swashbuckle.Application;

namespace XProjectNamespaceX.WebService
{
    public static class SwaggerConfig
    {
        public static void EnableSwagger(this HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "XProjectNameX API");
                c.IncludeXmlCommentsDir(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));
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
