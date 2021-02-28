using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SpiritGuardianCore
{
    public class Program
    {
        private CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            Console.WriteLine("App entered Main-async");
            CreateHostBuilder(args).Build().Run();
            Console.WriteLine("CreateHostbuilder Completed");

            Config mainConfig = new Config();

            Console.WriteLine("Attempting to build configs");

            mainConfig.DiscordKey = Environment.GetEnvironmentVariable("DiscordApiKey");

            string gw2Key = Environment.GetEnvironmentVariable("GW2ApiKey");

            string guildID = Environment.GetEnvironmentVariable("GuildId");

            Console.WriteLine("Finished reading Configs");

            mainConfig.ApiHandler = new Gw2Api(gw2Key, guildID);

            _services = new ServiceCollection()
                .AddSingleton(mainConfig)
                .BuildServiceProvider();

            Console.WriteLine("services built");

            var config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(config);

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.MessageUpdated += MessageUpdated;
            _client.UserJoined += AnnounceJoinedUser;

            await InstallCommands();

            var token = mainConfig.DiscordKey;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null) return;
            if (message.Author.IsBot) return;
            var channel = _client.GetChannel(342780866445049867) as SocketTextChannel;
            var user = messageParam.Author;
            await channel.SendMessageAsync("user " + user.Username + " posted a message \r\n" + message.Content + "\r\nin channel: <#" + message.Channel.Name + ">\r\n message id: " + message.Id);

            Task.Run(() => Responder(message.Content, message.Channel));
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user) //Welcomes the new user
        {
            var channel = _client.GetChannel(342772674243854336) as SocketTextChannel;
            //_gm = _client.
            await channel.SendMessageAsync($"Welcome Traveler {user.Mention}, at the gates of {channel.Guild.Name} " +
                $"if you are interested in joining use, please aknowledge yourself with <#232167382414524416> " +
                $"and I will notify my superiors of your presence here, I must warn you though as I cannot tell " +
                $"how much time it will take for Guild Commander or Shield Marshall to arrive." +
                $"" +
                $"if you are already member of our guild I can verify that if you tell me your name and region " +
                $"(ping the bot with 'join' + your gw2 account name and PL/ENG regions or type in '!join (your gw2 account name) (PL/ENG)')"); //Welcomes the new user
            var rules = _client.GetChannel(342780866445049867) as SocketTextChannel;
            await rules.SendMessageAsync("user " + user.Username + " " + user.Id + " joined server");

        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var message = await before.GetOrDownloadAsync();
            //Console.WriteLine($"{message} -> {after}");
            var logChannel = _client.GetChannel(342780866445049867) as SocketTextChannel;
            var user = after.Author;
            await logChannel.SendMessageAsync("Message by: " + user + " in channel: <#" + message.Channel.Name + "> was edited. \r\n status before edit: \r\n" + message.Content + "\r\n status after edit: \r\n" + after.Content);
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        public void Responder(String message, ISocketMessageChannel channel)
        {
            var words = message.Split().ToArray();
            if (words.Contains("\\o/"))
            {
                channel.SendMessageAsync(" \\ [T] / ");
            }
            if (words.Contains("\\o\\"))
            {
                channel.SendMessageAsync(" / [T] / ");
            }
            if (words.Contains("/o/"))
            {
                channel.SendMessageAsync(" \\ [T] \\ ");
            }
        }
    }
}
