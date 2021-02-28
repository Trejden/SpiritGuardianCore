using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpiritGuardianCore
{
    public class CommandHandler
    {
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly IServiceProvider _services;

		public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
		{
			_commands = commands;
			_services = services;
			_client = client;
		}

		public async Task InitializeAsync()
		{
			// Pass the service provider to the second parameter of
			// AddModulesAsync to inject dependencies to all modules 
			// that may require them.
			await _commands.AddModulesAsync(
				assembly: Assembly.GetEntryAssembly(),
				services: _services);
			_client.MessageReceived += HandleCommandAsync;
		}

		public async Task HandleCommandAsync(SocketMessage msg)
		{
			// ...
			// Pass the service provider to the ExecuteAsync method for
			// precondition checks.
			await _commands.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: _services);
			// ...
		}
	}
}
