using System;
using System.Collections.Generic;

namespace VintedGet.Infrastructure
{
    public class CommandLineInterface
    {
        private class Command
        {
            public Func<string[], bool> When { get; }
            public Action<string[]> Then { get; }

            public Command(Func<string[], bool> when, Action<string[]> then)
            {
                When = when;
                Then = then;
            }
        }

        private List<Command> _commands = new List<Command>();
        private Action<string[]> _helpCommand;

        public void RegisterCommand(string key, Action<string[]> then)
        {
            _commands.Add(new Command(x => x.HasParameter(key), then));
        }

        public void RegisterCommand(string key, string shortKey, Action<string[]> then)
        {
            _commands.Add(new Command(x => x.HasParameter(key, shortKey), then));
        }

        public void RegisterCommand(Func<string[], bool> when, Action<string[]> then)
        {
            _commands.Add(new Command(when, then));
        }

        public void RegisterHelp(Action<string[]> helpCommand)
        {
            _helpCommand = helpCommand;
        }

        public void Run(string[] args)
        {
            var commandExecuted = false;
            foreach (var command in _commands)
            {
                if (command.When(args))
                {
                    command.Then(args);
                    commandExecuted = true;
                    break;
                }
            }

            if (!commandExecuted && _helpCommand != null)
            {
                _helpCommand(args);
            }
        }
    }
}
