using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintedGet.Services;

namespace VintedGet.Commands
{
    internal class LoginCommand
    {
        public void Execute(string userId, string userSession)
        {
            if (VintedProcessor.HasSession(userId, userSession))
            {
                System.IO.Directory.CreateDirectory(GlobalSettings.Instance.SettingsFolder);
                System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, ".v_uid"), userId);
                System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.Instance.SettingsFolder, ".vinted_session"), userSession);

                var user = VintedProcessor.GetUser(userId, userSession);
                Console.WriteLine($"logged as {user.Login}");
            }
        }
    }
}
