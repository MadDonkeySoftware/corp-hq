# Corp-HQ 

## Useful Links
* [Discord](https://discord.gg/FvTw9pF) - General discussion and place to possibly talk to the developers
* [YouTrack](https://maddonkeysoftware.myjetbrains.com/youtrack/agiles/93-1/94-16) - Our bug / task tracking location


# Security:
MSDN: https://docs.microsoft.com/en-us/aspnet/core/security/
- CSRF: Cross Site Request Forgery: All controllers should be marked with the [ValidateAntiForgeryToken] to prevent CSRF, and `<form>` tags need to use tag helpers.
- HTTPS: Producation and Stage (any web-facing site) also needs 
  ```csharp
  services.Configure<MvcOptions>(options => options.Filters.Add(new RequireHttpsAttribute()));
  ``` 
  which forces all HTTP connections to request HTTPS instead.
- XSS: HTML Encode all user input (MVC does this automatically). If needed, @Html.Raw() will unencode it.
- CSP: Content-Security-Policy: script-src 'self': disables inline javascript and stops javascript from being loaded outside the current domain. See Startup.cs, Configure method:
  ```csharp
  app.UseCsp(options => options.DefaultSources(s => s.Self()) //only allow content to load from the web site's domain
            .ScriptSources(s => s.Self().CustomSources("ajax.aspnetcdn.com"))); //except for these whitelisted cdns (_ValidationScriptsPartial.cshtml uses these cdns)
  ```
  Content Security Policy (CSP) Quick Reference Guide: https://content-security-policy.com/
- ORA: (Open Redirect Attack) (e.g. bank.com/returnUrl?=bank.net then redirects back to the original site after getting username/password)
  - if a page has a returnUrl parameter (e.g. login page), the controller can first check:
    ```csharp
    if (!Url.IsLocalUrl(returnUrl))
    {
        //throw an exception, or otherwise handle unknown urls
    }
    ```
   - We will need to handle this slightly differently with the login server
   - We can add a function as follows to handle all returnUrl redirects:
   ```csharp
   private IActionResult RedirectToLocal(string returnUrl)
   {
       if (Url.IsLocalUrl(returnUrl))
       {
           return Redirect(returnUrl);
       }
       return RedirectToAction("Index", "Home"); //action, controller
   }
   ```
- Click-jacking: HTTP header X-Frame-Options
  - DENY = your site will not allow itself to be loaded to an iframe
  - SAMEORIGIN = your site can be in an iframe on the same domain
  - ALLOW-FROM https://asite.com = your site can be in an iframe on a specific domain (white-listing)
  - implemented in Startup.cs as 
    ```csharp
    app.UseXfo(options => options.Deny());
    ```
- Same Origin Policy and Cross Origin Resource Sharing (CORS): (browser sees different ports on same machine as different origins)
  - HTTP header 
    - Access-Control-Allow-Origin: http://example.com
    - Access-Control-Allow-Origin: *
  - Done via Startup.cs, Configure method: 
    ```csharp
    app.UseCors(options => 
    {
        options.WithOrigins("https://site.com:<port>"); //port is not required unless its non-standard
        options.WithOrigins("https://api.site.com:<port>");
        //OR the following to allow any CORS call to any URL
        options.AllowAnyOrigin();
    });
    ```
  - NOTE: this is primarily for API Controllers and can be done via Startup.cs, ConfigureServices method:
    ```csharp
    services.AddCors(options => 
    {
      options.AddPolicy("PolicyName", 
          c => c.WithOrigins("<protocol>://<url>:<port>"));
    });
    ```
    combined with the [RequireCors("PolicyName")] decorator on your API Controller class.