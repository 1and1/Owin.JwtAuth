# Owin.JwtAuth

Owin.JwtAuth provides JSON Web Token authentication with X509 signing and configuration stored in `App.config`/`Web.config`.

This is a very thin wrapper around [Microsoft.Owin.Security.Jwt](https://www.nuget.org/packages/Microsoft.Owin.Security.Jwt/).

NuGet package:
* [Owin.JwtAuth](https://www.nuget.org/packages/Owin.JwtAuth/)



## Usage

 * Import the public key of the certificate used to sign the JSON Web Tokens in the machine-wide "Trusted People" store.
 * Add `app.UseJwtAuth();` to your `OwinStartup.cs` file.
 * Add this to your `App.config`/`Web.config`:
```xml
<configuration>
  <appSettings>
    <!-- The name of the service allowed to issue JSON Web Tokens for access to this service. Also used to select the issuer certificate from the system certificate store. -->
    <add key="JwtIssuer" value="auth-service" />

    <!-- The name of this service as used as an audience name in JSON Web Tokens. -->
    <add key="JwtAudience" value="my-service" />
  </appSettings>
  <!--...-->
</configuration>
```



## Certificate rollover

The library tries to validate signatures using all currently valid certificates with a matching subject name found in the "Trusted People" store.

To perform a graceful certificate rollover, e.g., in case of a certificate renewal: Import a new certificate with the same subject name before the old one expires. Then start using the new certificate to sign tokens.



## Sample project

The source code includes a sample project that uses demonstrates the usage of Owin.JwtAuth. You can build and run it using Visual Studio 2015. By default the instance will be hosted by IIS Express at `http://localhost:4358/`.
