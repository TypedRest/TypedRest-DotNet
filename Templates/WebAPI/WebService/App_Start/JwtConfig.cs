using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using NLog;
using Owin;

namespace XProjectNamespaceX.WebService
{
    /// <summary>
    /// Configuration for JSON Web Token authentication.
    /// </summary>
    public static class JwtConfig
    {
        /// <summary>
        /// Enables authentication using JSON Web Tokens.
        /// </summary>
        public static IAppBuilder UseJwtAuthentication(this IAppBuilder app)
        {
            string issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            if (issuer == null) return app;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                LogManager.GetLogger("ApiRequest").Warn("JWT is only supported on Windows because it uses the Certificate Store.");
                return app;
            }

            return app.UseJwtBearerAuthentication(issuer, ConfigurationManager.AppSettings["JwtAudience"]);
        }

        private static IAppBuilder UseJwtBearerAuthentication(this IAppBuilder app, string issuer, string audience)
        {
            var certificate = GetCertificate(issuer);
            if (certificate == null)
            {
                LogManager.GetLogger("ApiRequest").Warn($"No certificate for {issuer} found in TrustedPeople store.");
                return app;
            }

            return app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] {audience},
                IssuerSecurityTokenProviders = new[]
                {
                    new X509CertificateSecurityTokenProvider(issuer, certificate)
                }
            });
        }

        private static X509Certificate2 GetCertificate(string issuer)
        {
            var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                return store.Certificates
                    .Find(X509FindType.FindBySubjectName, issuer, validOnly: true)
                    .Cast<X509Certificate2>().FirstOrDefault();
            }
            finally
            {
                store.Close();
            }
        }
    }
}