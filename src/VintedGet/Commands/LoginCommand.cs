using System;
using VintedGet.Infrastructure;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class LoginCommand
    {
        public void Execute(string accessToken, string refreshToken)
        {
            var jwtAccess = new JwtToken(accessToken);
            var jwtRefresh = new JwtToken(refreshToken);

            if (!string.IsNullOrEmpty(jwtAccess.UserId) && !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var cookies = $"access_token_web={accessToken}; refresh_token_web={refreshToken}";
                System.IO.Directory.CreateDirectory(GlobalSettings.Instance.SettingsFolder);
                System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"), jwtAccess.UserId);
                System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_cookies"), cookies);
            }

            var session = VintedProcessor.GetSession();
            if (session != null)
            {
                var user = VintedProcessor.GetUser(session.UserId, session.Cookies);
                Console.WriteLine($"logged as {user.Login}");
            }
        }
    }
}
