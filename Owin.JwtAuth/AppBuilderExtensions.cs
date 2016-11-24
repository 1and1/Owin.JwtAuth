using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        /// <param name="suppressChallenge"><c>true</c> to prevent the <c>Bearer</c> challenge from being added to the <c>WWW-Authenticate</c> header.</param>
        public static IAppBuilder UseJwtAuth(this IAppBuilder app, bool suppressChallenge = false)
        {
            string issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            string audience = ConfigurationManager.AppSettings["JwtAudience"];
            return string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience)
                ? app
                : app.UseJwtAuth(issuer, audience, suppressChallenge);
        }

        /// <summary>
        /// Enables JSON Web Token Authentication.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="issuer">The name of the service allowed to issue JSON Web Tokens for access to this service. Also used to select the issuer certificate from the system certificate store.</param>
        /// <param name="audience">The name of this service as used as an audience name in JSON Web Tokens.</param>
        /// <param name="suppressChallenge"><c>true</c> to prevent the <c>Bearer</c> challenge from being added to the <c>WWW-Authenticate</c> header.</param>
        public static IAppBuilder UseJwtAuth(this IAppBuilder app, string issuer, string audience, bool suppressChallenge = false)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                app.CreateLogger("Owin.JwtAuth").WriteWarning("Certificate Store is only available on Windows. JWT Authentication not enabled.");
                return app;
            }

            var certificates = CertificateLoader.BySubjectName(issuer);
            if (certificates.Length == 0)
            {
                app.CreateLogger("Owin.JwtAuth").WriteWarning("No certificate for {0} found in TrustedPeople store. JWT Authentication not enabled.", issuer);
                return app;
            }

            return app.UseJwtAuth(issuer, certificates, audience, suppressChallenge);
        }

        /// <summary>
        /// Enables JSON Web Token Authentication.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="issuer">The name of the service allowed to issue JSON Web Tokens for access to this service.</param>
        /// <param name="certificates">The possible certificates used by the issuer to sign tokens.</param>
        /// <param name="audience">The name of this service as used as an audience name in JSON Web Tokens.</param>
        /// <param name="suppressChallenge"><c>true</c> to prevent the <c>Bearer</c> challenge from being added to the <c>WWW-Authenticate</c> header.</param>
        public static IAppBuilder UseJwtAuth(this IAppBuilder app, string issuer, IEnumerable<X509Certificate2> certificates, string audience, bool suppressChallenge = false)
        {
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = suppressChallenge ? AuthenticationMode.Passive : AuthenticationMode.Active,
                Realm = audience,
                AllowedAudiences = new[] {audience},
                IssuerSecurityTokenProviders =
                    certificates.Select(x => new X509CertificateSecurityTokenProvider(issuer, x))
            });
            return app;
        }
    }
}