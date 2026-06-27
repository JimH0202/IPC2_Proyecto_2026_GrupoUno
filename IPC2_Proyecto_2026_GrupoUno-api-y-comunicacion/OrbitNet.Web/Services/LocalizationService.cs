using Microsoft.AspNetCore.Http;
using System;

namespace OrbitNet.Web.Services;

public static class LocalizationService
{
    public const string CookieName = ".AspNetCore.Culture";

    public static void SetLanguage(HttpResponse response, string culture)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            Path = "/",
            HttpOnly = false
        };

        response.Cookies.Append(CookieName, $"c={culture}|uic={culture}", cookieOptions);
    }

    public static string GetCurrentLanguage(HttpContext context)
    {
        var culture = context.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>()?.RequestCulture.Culture.Name ?? "es";
        return culture;
    }
}
