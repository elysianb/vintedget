using VintedGet.Commands;
using VintedGet.Infrastructure;
using VintedGet.Services;

namespace VintedGet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GlobalSettings.Instance.Delay = int.Parse(args.GetParameterValue("delay", "d", "500"));
            GlobalSettings.Instance.MaxRetry = int.Parse(args.GetParameterValue("max-retry", "r", "10"));
            GlobalSettings.Instance.Output = args.GetParameterValue("output", "o", ".");

            if (args.HasOnlyOneArgument())
            {
                args = new[] { "--url", args[0] };
            }

            var cli = new CommandLineInterface();

            cli.RegisterHelp(x => new HelpCommand().Execute());

            cli.RegisterCommand("version", "v",
                x => {
                    new GetVersionCommand().Execute();
                });

            cli.RegisterCommand("url", "u",
                x => {
                    var url = x.GetParameterValue("url", "u");
                    new GetItemCommand().Execute(url);
                });

            cli.RegisterCommand("login",
                x => {
                    var userId = x.GetParameterValue("userid", "uid");
                    var userSession = x.GetParameterValue("session", "s");
                    var token = args.GetParameterValue("token", "T");
                    new LoginCommand().Execute(userId, userSession);
                });

            cli.RegisterCommand("whoami",
               x => {
                   new WhoAmICommand().Execute();
               });

            cli.RegisterCommand("logout",
               x => {
                   new LogoutCommand().Execute();
               });

            cli.RegisterCommand("new-batch", "nb",
               x => {
                   var filename = args.GetParameterValue("new-batch", "nb", "vget.batch.txt");
                   new NewBatchCommand().Execute(filename);
               });

            cli.RegisterCommand("clean", "cls",
               x => {
                   var directory = args.GetParameterValue("clean", "cls", ".");
                   new CleanCommand().Execute(directory);
               });

            cli.RegisterCommand("batch", "b",
               x => {
                   var directory = args.GetParameterValue("batch", "b", "vget.batch.txt");
                   var statisticsOnly = args.HasParameter("statistics-only", "so");
                   new RunBatchCommand().Execute(directory, statisticsOnly);
               });

            cli.RegisterCommand("thread", "t",
               x => {
                   var userId = args.GetParameterValue("userid", "uid");
                   var userSession = args.GetParameterValue("session", "s");
                   var token = args.GetParameterValue("token", "T");
                   var threadId = args.GetParameterValue("thread", "t");

                   new GetThreadCommand().Execute(userId, userSession, threadId);
               });

            cli.RegisterCommand("favorites", "f",
               x => {
                   var userId = args.GetParameterValue("userid", "uid");
                   var userSession = args.GetParameterValue("session", "s");
                   var token = args.GetParameterValue("token", "T");
                   var operation = args.GetParameterValue("download", "d", "download");
                   var itemLimit = args.GetParameterValue("itemlimit", "il");

                   new GetFavoritesCommand().Execute(userId, userSession, operation, itemLimit);
               });

            cli.Run(args);
        }
    }
}
