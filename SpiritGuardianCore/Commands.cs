using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Net;
using SpiritGuardianCore;

namespace SpiritGuardian
{
    public class Commands : ModuleBase
    {
        //public Config Config { get; set; }
        private readonly Config _config;
        public Commands(Config config) => _config = config;

        /*--------------------Dailies---------------------------*/

        //~Daily -> list of dailies for a day
        [Command("Daily", RunMode = RunMode.Async), Summary("Displays Current Dailies")]
        public async Task Daily()
        {
            string result = _config.ApiHandler.CurrentDaily[0] + "\r\n" +
                          _config.ApiHandler.CurrentDaily[1] + "\r\n" +
                          _config.ApiHandler.CurrentDaily[2] + "\r\n";
            await ReplyAsync(result);
        }
        //~Daily type -> list of dailies of given type
        [Command("Daily", RunMode = RunMode.Async), Summary("Displays Current Dailies by type")]
        public async Task Daily(string type)
        {
            string result;
            switch (type)
            {
                case "pve":
                    result = _config.ApiHandler.CurrentDaily[0];
                    break;
                case "pvp":
                    result = _config.ApiHandler.CurrentDaily[1];
                    break;
                case "wvw":
                    result = _config.ApiHandler.CurrentDaily[2];
                    break;
                case "next":
                    result = result = _config.ApiHandler.NextDaily[0] + "\r\n" +
                          _config.ApiHandler.NextDaily[1] + "\r\n" +
                          _config.ApiHandler.NextDaily[2] + "\r\n";
                    break;
                default:
                    Console.WriteLine("Incorrect type");
                    result = "incorrect daily category, try again\r\n";
                    break;
            }
            Console.WriteLine("Dailies Parsed, printing response: " + result);
            await ReplyAsync(result);
        }

        /*--------------------Organisational---------------------------*/

        //~Members -> list of members
        [Command("Members", RunMode = RunMode.Async), Summary("Displays list of members")]
        public async Task Members()
        {
            var contextChannel = Context.Channel.Id;
            if (contextChannel == 342772674243854336 || contextChannel == 232167382414524416)
            {
                var caller = Context.User;
                var callId = caller.Id;
                await ReplyAsync("Nie mogę ci pokazać listy członków tutaj <@"+callId+">");
                return;
            }
            var memberList = _config.ApiHandler.MemberList;
            var printableList = memberList.Select(x => x.rank + " " + x.name).ToArray();
            string printable = string.Join("\r\n", printableList);
            await ReplyAsync(printable);
        }
        //~Join
        [Command("Join", RunMode = RunMode.Async), Summary("Verifies user in a guild")]
        public async Task Join(string id, string region)
        {
            var memberList = _config.ApiHandler.MemberList;
            var parseable = memberList.Select(x => x.name);
            var user = Context.User;
            IRole role;
            if (region == "PL")
            {
                role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "PL Praetori");
            }
            else
            {
                role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "ENG Praetori");
            }
            if (parseable.Contains(id))
            {
                await (user as IGuildUser).AddRoleAsync(role);
                await ReplyAsync("Welcome Home, Praetori");
            }
            else
            {
                await ReplyAsync("You are not member of this guild");
            }
        }
        //~Rekrut
        [Command("Rekrut"), Summary("allows self-assignment of rekrut rank")]
        public async Task Rekrut()
        {
            var user = Context.User;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Rekrut");
            await (user as IGuildUser).AddRoleAsync(role);
            await ReplyAsync("Be our guest at <#232455349473509377>");
        }

    }
}
