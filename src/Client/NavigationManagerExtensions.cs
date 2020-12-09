using System;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace Blazor5Auth.Client
{

    // https://chrissainty.com/working-with-query-strings-in-blazor/
    // ParseQueryString added to HttpUtility

    public static class NavigationManagerExtensions
    {
        public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T value)
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);
            var valueFromQueryString = HttpUtility.ParseQueryString(uri.Query)[key];

            if (!string.IsNullOrEmpty(valueFromQueryString))
            {
                if (typeof(T) == typeof(int) && int.TryParse(valueFromQueryString, out var valueAsInt))
                {
                    value = (T)(object)valueAsInt;
                    return true;
                }

                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)valueFromQueryString.ToString();
                    return true;
                }

                if (typeof(T) == typeof(decimal) && decimal.TryParse(valueFromQueryString, out var valueAsDecimal))
                {
                    value = (T)(object)valueAsDecimal;
                    return true;
                }
            }

            value = default;
            return false;
        }

        // https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Core/src/Routing/UrlHelperBase.cs
        public static bool IsLocalUrl(this NavigationManager navigationManager, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            // Allows "/" or "/foo" but not "//" or "/\".
            if (url[0] == '/')
            {
                // url is exactly "/"
                if (url.Length == 1)
                {
                    return true;
                }

                // url doesn't start with "//" or "/\"
                if (url[1] != '/' && url[1] != '\\')
                {
                    return !HasControlCharacter(url.AsSpan(1));
                }

                return false;
            }

            // Allows "~/" or "~/foo" but not "~//" or "~/\".
            if (url[0] == '~' && url.Length > 1 && url[1] == '/')
            {
                // url is exactly "~/"
                if (url.Length == 2)
                {
                    return true;
                }

                // url doesn't start with "~//" or "~/\"
                if (url[2] != '/' && url[2] != '\\')
                {
                    return !HasControlCharacter(url.AsSpan(2));
                }

                return false;
            }

            return false;

            static bool HasControlCharacter(ReadOnlySpan<char> readOnlySpan)
            {
                // URLs may not contain ASCII control characters.
                for (var i = 0; i < readOnlySpan.Length; i++)
                {
                    if (char.IsControl(readOnlySpan[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static void NavigateToReturnUrl(this NavigationManager navigationManager, string fallbackUrl) {
            if (navigationManager.TryGetQueryString<string>("returnUrl", out var returnUrl))
            {
                if (navigationManager.IsLocalUrl(returnUrl)) {
                    navigationManager.NavigateTo(returnUrl);
                    return;
                }
            }

            navigationManager.NavigateTo(fallbackUrl);
        }

        public static void NavigateWithExistingReturnUrl(this NavigationManager navigationManager, string url) {
            if (navigationManager.TryGetQueryString<string>("returnUrl", out var returnUrl))
            {
                if (navigationManager.IsLocalUrl(returnUrl)) {
                    url = url + "?returnUrl=" + Uri.EscapeDataString(returnUrl);
                }
            }

            navigationManager.NavigateTo(url);
        }

        public static void NavigateWithNewReturnUrl(this NavigationManager navigationManager, string url) {

            var returnUrl = navigationManager.ToBaseRelativePath(navigationManager.Uri);

            if (navigationManager.TryGetQueryString<string>("returnUrl", out var existingReturnUrl))
            {
                if (!string.IsNullOrEmpty(existingReturnUrl) && navigationManager.IsLocalUrl(existingReturnUrl)) {
                    returnUrl = existingReturnUrl;
                }
            }

            if (!returnUrl.StartsWith("/")) returnUrl = "/" + returnUrl;

            navigationManager.NavigateTo($"{url}?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
    }

}
