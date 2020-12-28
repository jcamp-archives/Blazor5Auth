# Blazor5Auth
A Blazor Wasm App with Authentication/Authorization without IdentityServer

The goal of this is to build an alternative Blazor Wasm authentication system that does not use Identity Server.

Two reasons:
1. Identity Server is serious overkill for many small sites and adds significant complexity if you are not looking to allow multiple platforms / apps to login (ie don't need the OpenID/OAuth2 flows)
2. Identity Server will no longer be free / open source after version 4 and the commercial license, at the time of writing this, starts at $1500/year.  As such, I firm believe an alternative is needed to Identity Server and Blazor Wasm.   There have been some comments from the asp.net team that a solution is being worked on, but no timelines have been given, and I needed something sooner.

I put a full copy of the IdentityServer-based code into this repo as a reference and also to have a copy of the fully scaffolded pages for Identity.

I did change it to use Sqlite so it could run cross-platform.

## MongoDb Branch
Uses MongoFramework with MongoDb for data storage

## References / Thanks
Blueprint and much code came from Chris Sainty's blog article, [Authentication with client-side Blazor using WebAPI and ASP.NET Core Identity](https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/)

[API / Email Service Example](https://github.com/cornflourblue/aspnet-core-3-signup-verification-api)
[Documentation on Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-5.0)


### SMTP Settings
Use secrets to store these without commiting
- dotnet user-secrets set "SmtpSettings:Server" "smtp.mailtrap.io"
- dotnet user-secrets set "SmtpSettings:Port" "2525"
- dotnet user-secrets set "SmtpSettings:Username" ""
- dotnet user-secrets set "SmtpSettings:Password" ""
