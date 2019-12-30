using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using IniParser;
using IniParser.Exceptions;
using IniParser.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;

namespace ShareShot {
    class ImgurLogin {

        public ImgurLogin() {

        }

        public string AuthUser() {
            IniData authData = null;
            string clientId = null;
            // Create a parser to read the auth.ini file
            var parser = new FileIniDataParser();
            try {
                authData = parser.ReadFile(Environment.ExpandEnvironmentVariables(Environment.ExpandEnvironmentVariables("%appdata%\\ShareShot\\auth.ini")));
                clientId = authData["credentials"]["client_id"];
            }
            catch (ParsingException) {
                Console.WriteLine("Error: File not found.");
                return null;
            }

            // Get the authorization url using the clientId
            var client = new ImgurClient(clientId);
            var endpoint = new OAuth2Endpoint(client);
            var authorizationUrl = endpoint.GetAuthorizationUrl(OAuth2ResponseType.Token);

            // Create a web driver that will open the authorization url in an instance of Firefox
            IWebDriver driver = new FirefoxDriver {
                Url = authorizationUrl
            };

            // Wait until the user logs in and the browser redirects
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromDays(365));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains("#access_token="));

            // Get the url of the redirected web page
            string url = driver.Url;

            // Close the IWebDriver
            driver.Close();

            // The url comes with a substring that needs to be deleted
            url = url.Replace("state=#", "");

            // Create a uri using the updated url, then turn it into a functional string that can be queried
            Uri uri = new Uri(url);
            string queryString = uri.Query;

            // Get the access and refresh tokens
            string accessToken = System.Web.HttpUtility.ParseQueryString(queryString).Get("access_token");
            string refreshToken = System.Web.HttpUtility.ParseQueryString(queryString).Get("refresh_token");

            authData["credentials"]["refresh_token"] = refreshToken;
            parser.WriteFile(Environment.ExpandEnvironmentVariables("%appdata%\\ShareShot\\auth.ini"), authData);

            return accessToken;
        }

        public string GetToken() {
            // Create a parser to read the auth.ini file
            var parser = new FileIniDataParser();
            IniData authData = parser.ReadFile(Environment.ExpandEnvironmentVariables("%appdata%\\ShareShot\\auth.ini"));
            string clientId = authData["credentials"]["client_id"];
            string clientSecret = authData["credentials"]["client_secret"];
            string refreshToken = authData["credentials"]["refresh_token"];

            // Get the authorization url using the clientId
            var client = new ImgurClient(clientId, clientSecret);
            var endpoint = new OAuth2Endpoint(client);

            // Try/Catch block, in case the refresh token is null or invalid.
            try {
                return endpoint.GetTokenByRefreshTokenAsync(refreshToken).Result.AccessToken;
            }
            catch (AggregateException) {
                return AuthUser();
            }
        }
    }
}