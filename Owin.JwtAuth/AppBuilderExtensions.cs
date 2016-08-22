using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;

namespace Owin.JwtAuth
{
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Enables JSON Web Token Authentication using configuration from <see cref="ConfigurationManager.AppSettings"/>.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        public static IAppBuilder UseJwtAuth(this IAppBuilder app)
        {
            string issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            string audience = ConfigurationManager.AppSettings["JwtAudience"];
            return string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience)
                ? app
                : app.UseJwtAuth(issuer, audience);
        }

        /// <summary>
        /// Enables JSON Web Token Authentication.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="issuer">The name of the service allowed to issue JSON Web Tokens for access to this service. Also used to select the issuer certificate from the system certificate store.</param>
        /// <param name="audience">The name of this service as used as an audience name in JSON Web Tokens.</param>
        public static IAppBuilder UseJwtAuth(this IAppBuilder app, string issuer, string audience)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                app.CreateLogger("JwtAuth").WriteWarning("Certificate Store is only available on Windows. JWT Authentication not enabled.");
                return app;
            }

            var certificate = CertificateLoader.BySubjectName(issuer);
            if (certificate == null)
            {
                app.CreateLogger("JwtAuth").WriteWarning("No certificate for {0} found in TrustedPeople store. JWT Authentication not enabled.", issuer);
                return app;
            }

            return app.UseJwtAuth(issuer, certificate, audience);
        }

        /// <summary>
        /// Enables JSON Web Token Authentication.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="issuer">The name of the service allowed to issue JSON Web Tokens for access to this service.</param>
        /// <param name="certificate">The certificate used by the issuer to sign tokens.</param>
        /// <param name="audience">The name of this service as used as an audience name in JSON Web Tokens.</param>
        public static IAppBuilder UseJwtAuth(this IAppBuilder app, string issuer, X509Certificate2 certificate, string audience)
        {
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
    }
}