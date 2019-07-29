using Newtonsoft.Json;
using opendkp_bot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace opendkp_bot.Services
{
    class DKPService
    {
        private static DKPService fInstance;
        private static int CacheTimer = 5;
        private static string DKP_URL = "https://discordapp.com/api/oauth2/authorize?client_id=604892897921466368&permissions=522304&scope=bot";
        private static string ClientId = "UIHM6uLuogqPfHGPrnMM9rxCEx8xvUrz";

        public DKPService()
        {
            GetDKPInformation();
        }
        public static DKPService Instance
        {
            get
            {
                if (fInstance == null)
                {
                    fInstance = new DKPService();
                }
                return fInstance;
            }
        }
        public DateTime LastUpdated { get; set; }
        private List<MemberModel> fGuildRoster = new List<MemberModel>();
        public List<MemberModel> GuildRoster
        {
            get
            {
                if ( fGuildRoster.Count <= 0 || (DateTime.Now - new TimeSpan(0,CacheTimer,0)) > LastUpdated)
                {
                    GetDKPInformation();
                }
                return fGuildRoster;
            }
        }

        private void GetDKPInformation()
        {
            fGuildRoster.Clear();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DKP_URL);
            request.Headers = new WebHeaderCollection();

            //You must set the proper client id for your dkp client
            //https://4jmtrkwc86.execute-api.us-east-2.amazonaws.com/beta/client
            //Check the above URL for your client id
            request.Headers.Add("clientid", ClientId);
            
            try
            {
                try
                {
                    using (HttpWebResponse vResponse = (HttpWebResponse)request.GetResponse())
                    using (Stream vStream = vResponse.GetResponseStream())
                    using (StreamReader vStreamReader = new StreamReader(vStream))
                    {
                        string vJson = vStreamReader.ReadToEnd();
                        if ( !string.IsNullOrWhiteSpace(vJson))
                        {
                            dynamic vModels = JsonConvert.DeserializeObject(vJson);
                            foreach (var vItem in vModels.Models)
                            {
                                fGuildRoster.Add(new MemberModel
                                {
                                    Name = vItem.CharacterName,
                                    Class = vItem.CharacterClass,
                                    Rank = vItem.CharacterRank,
                                    RA_30DayPercent = vItem.Calculated_30,
                                    RA_60DayPercent = vItem.Calculated_60,
                                    RA_90DayPercent = vItem.Calculated_90,
                                    RA_LifeTimePercent = vItem.Calculated_Life,
                                    DKP_CURRENT = vItem.CurrentDKP
                                });
                            }
                            LastUpdated = DateTime.Now;
                        }
                        else
                        {
                            Console.WriteLine("Was not able to get data back");
                        }
                    }

                }
                catch (Exception vException)
                {
                    Console.WriteLine(vException);
                }
            }
            catch (Exception e)
            {
                //EXCEPTION?
            }
        }
    }
}
