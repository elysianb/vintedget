using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Infrastructure;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class LoginCommand
    {
        public void Execute(string token)
        {
            var jwt = new JwtToken(token);

            if (VintedProcessor.HasSession(jwt.UserId, jwt.TokenCookie))
            {
                System.IO.Directory.CreateDirectory(GlobalSettings.Instance.SettingsFolder);
                System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"), jwt.UserId);
                System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_cookies"), jwt.TokenCookie);

                var user = VintedProcessor.GetUser(jwt.UserId, jwt.TokenCookie);
                Console.WriteLine($"logged as {user.Login}");
            }
        }
    }
}
