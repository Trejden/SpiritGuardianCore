

namespace SpiritGuardianCore
{
    public class Config
    {
        //Core Function
        public string DiscordKey{get;set;}
        //public string GuildApiKey { get; set; }
        public Gw2Api ApiHandler { get; set; }
        //Behavioral Customisation
        public string WelcomeMessage { get; set; } = "Welcome Traveler {user.Mention}, at the gates of {channel.Guild.Name} " +
                $"if you are interested in joining use, please aknowledge yourself with <#232167382414524416> " +
                $"and I will notify my superiors of your presence here, I must warn you though as I cannot tell " +
                $"how much time it will take for Guild Commander or Shield Marshall to arrive." +
                $"" +
                $"if you are already member of our guild I can verify that if you tell me your name and region " +
                $"(ping the bot with 'join' + your gw2 account name and PL/ENG regions or type in '!join (your gw2 account name) (PL/ENG)')";
        
    }
}
