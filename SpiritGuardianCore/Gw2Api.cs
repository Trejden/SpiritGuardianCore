using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace SpiritGuardianCore
{
    public class Gw2Api
    {
        protected string _key;
        protected string _guildId;

        protected string GetRequest(string URI)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        protected DateTime MemberUpdate { get; set; }
        protected DateTime DailyUpdate { get; set; }
        protected DateTime NextDailyUpdate { get; set; }

        private GuildMember[] _memberList;

        public GuildMember[] MemberList
        {
            get
            {
                if(MemberUpdate!=DateTime.Today)
                {
                    Console.WriteLine("Attempting to access GW2 API node: /guild/members \r\n");

                    string responseString = GetRequest("https://api.guildwars2.com/v2/guild/" + _guildId + "/members?access_token=" + _key);

                    Console.WriteLine("Accessed GW2 API, return string: \r\n" + responseString);

                    var guildMemberList = JsonConvert.DeserializeObject<GuildMember[]>(responseString);

                    _memberList = guildMemberList;

                    MemberUpdate = DateTime.Today;

                    return _memberList;
                }
                else
                {
                    return _memberList;
                }
            }
        }

        private string[] _currentDaily;

        public string[] CurrentDaily
        {
            get
            {
                Console.WriteLine("Last Daily update: " + DailyUpdate.ToString());
                Console.WriteLine("today is: " + DateTime.Today.ToString());
                if (DailyUpdate!=DateTime.Today)
                {
                    Console.WriteLine("Attempting to acess GW2 API node: /daily \r\n");
                    string responseString = GetRequest("https://api.guildwars2.com/v2/achievements/daily");

                    Console.WriteLine("Accessed GW2 API, succesfully");

                    var dailies = JsonConvert.DeserializeObject<Dailies>(responseString);

                    var pve = dailies.pve.Select(x => x.id).Distinct().ToArray();
                    var pvp = dailies.pvp.Select(x => x.id).Distinct().ToArray();
                    var wvw = dailies.wvw.Select(x => x.id).Distinct().ToArray();

                    string[] update = new string[3];

                    update[0] = "Current Dailies PvE: \r\n";
                    for (int a=0;a<pve.Length;a++)
                    {
                        var achiev = this.GetAchievement(pve[a]);
                        update[0] = update[0]+achiev.name+"\r\n";
                    }
                    update[1] = "Current Dailies PvP: \r\n";
                    for (int a = 0; a < pvp.Length; a++)
                    {
                        var achiev = this.GetAchievement(pvp[a]);
                        update[1] = update[1] + achiev.name + "\r\n";
                    }
                    update[2] = "Current Dailies WvW: \r\n";
                    for (int a = 0; a < wvw.Length; a++)
                    {
                        var achiev = this.GetAchievement(wvw[a]);
                        update[2] = update[2] + achiev.name + "\r\n";
                    }
                    DailyUpdate = DateTime.Today;
                    _currentDaily = update;
                    return _currentDaily;
                }
                else
                {
                    return _currentDaily;
                }
            }
        }

        private string[] _nextDaily;

        public string[] NextDaily
        {
            get
            {
                Console.WriteLine("Last Daily update: " + DailyUpdate.ToString());
                Console.WriteLine("today is: " + DateTime.Today.ToString());
                if (NextDailyUpdate != DateTime.Today)
                {
                    Console.WriteLine("Attempting to acess GW2 API node: /daily \r\n");
                    string responseString = GetRequest("https://api.guildwars2.com/v2/achievements/daily/tomorrow");

                    Console.WriteLine("Accessed GW2 API, successfully");

                    var dailies = JsonConvert.DeserializeObject<Dailies>(responseString);

                    var pve = dailies.pve.Select(x => x.id).Distinct().ToArray();
                    var pvp = dailies.pvp.Select(x => x.id).Distinct().ToArray();
                    var wvw = dailies.wvw.Select(x => x.id).Distinct().ToArray();

                    string[] update = new string[3];

                    update[0] = "Next Dailies PvE: \r\n";
                    for (int a = 0; a < pve.Length; a++)
                    {
                        var achiev = this.GetAchievement(pve[a]);
                        update[0] = update[0] + achiev.name + "\r\n";
                    }
                    update[1] = "Next Dailies PvP: \r\n";
                    for (int a = 0; a < pvp.Length; a++)
                    {
                        var achiev = this.GetAchievement(pvp[a]);
                        update[1] = update[1] + achiev.name + "\r\n";
                    }
                    update[2] = "Next Dailies WvW: \r\n";
                    for (int a = 0; a < wvw.Length; a++)
                    {
                        var achiev = this.GetAchievement(wvw[a]);
                        update[2] = update[2] + achiev.name + "\r\n";
                    }
                    NextDailyUpdate = DateTime.Today;
                    _nextDaily = update;
                    return _nextDaily;
                }
                else
                {
                    return _nextDaily;
                }
            }
        }

        public Achievement GetAchievement(int ID)
        {
            Console.WriteLine("Attempting to acess GW2 API node: /achievements?id="+ID+ " \r\n");
            string responseString = GetRequest("https://api.guildwars2.com/v2/achievements?id="+ID);

            //Console.WriteLine("Accessed GW2 API, return string: \r\n"+responseString);
            Console.WriteLine("Accessed GW2 API, succefully");

            var achievement = JsonConvert.DeserializeObject<Achievement>(responseString);

            return achievement;
        }
        
        public Gw2Api(string key, string ID)
        {
            this._key = key;
            this._guildId = ID;
        }
    }
}
