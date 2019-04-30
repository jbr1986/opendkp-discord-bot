using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus;
using opendkp_bot.Services;
using opendkp_bot.Models;
using System.Linq;
using System;
using System.Globalization;

namespace opendkp_bot.Commands
{
    class BasicUnGroupedCommands
    {
        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }
    }

    [Group("dkp",CanInvokeWithoutSubcommand = true)] // let's mark this class as a command group
    [Description("These are DKP related commands")] // give it a description for help purposes
    class DKPGroupedCommands
    {

        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DKPService vService = DKPService.Instance;
            MemberModel vModel = null;
            var emoji = DiscordEmoji.FromName(ctx.Client, ":disappointed:");
            //If a player name is passed to the command, lets look up their dkp information
            if ( !string.IsNullOrWhiteSpace(ctx.RawArgumentString) )
            {
                vModel = vService.GuildRoster.SingleOrDefault<MemberModel>(s => s.Name.Equals(ctx.RawArgumentString.Trim(), StringComparison.OrdinalIgnoreCase));
                if (vModel == null)
                {
                    await ctx.RespondAsync($"I can't find any DKP associated with {ctx.RawArgumentString} {emoji}");
                }
                else
                {
                    await ctx.RespondAsync(string.Format("{0}'s dkp as of {1} is {2}",
                        vModel.Name,
                        vService.LastUpdated,
                        vModel.DKP_CURRENT.ToString("N0", CultureInfo.InvariantCulture)));
                    //await ctx.RespondAsync($"{vModel.Name}'s dkp as of {vService.LastUpdated} is {vModel.DKP_CURRENT}");
                }
            }
            else
            {
                await ctx.RespondAsync($"I have dkp as of {vService.LastUpdated} available");
            }
            
        }

        [Command("top")]
        [Description("Usage: !dkp top number class || i.e. !dkp top 5 war")] // this will be displayed to tell users what this command does when they invoke help
        public async Task TopCommand(CommandContext ctx) // this command takes no arguments
        {
            DKPService vService = DKPService.Instance;
            string[] vArgs = ctx.RawArgumentString.Trim().Split(' ');
            try
            {
                int vResults = int.Parse(vArgs[0]);
                if ( vResults > 25 )
                {
                    await ctx.RespondAsync("Limiting your results to 25");
                    vResults = 25;
                }
                if ( vArgs.Length > 1 )
                {
                    string vClass = vArgs[1].ToLower().Trim();

                    if (EQConfig.EQClasses.ContainsKey(vClass))
                    {
                        var vTopFive = vService.GuildRoster
                            .Where(y => y.Class.Equals(EQConfig.EQClasses[vClass], StringComparison.InvariantCultureIgnoreCase))
                            .OrderByDescending(x => x.DKP_CURRENT)
                            .Take(vResults);

                        int vMaxLength = vTopFive.Max(x => x.Name.Length) + 1;

                        string vResponse = string.Format("```Here are the top {2} highest {3}'s as of {0}: {1}", vService.LastUpdated, Environment.NewLine, vResults, EQConfig.EQClasses[vClass]);
                        foreach (MemberModel vMember in vTopFive)
                        {
                            vResponse += string.Format("{0}{1}{2}", 
                                vMember.Name.PadRight(vMaxLength), 
                                vMember.DKP_CURRENT.ToString("N0", CultureInfo.InvariantCulture), 
                                Environment.NewLine);
                        }
                        vResponse += "```";

                        await ctx.RespondAsync(vResponse);
                    }
                    else
                    {
                        await ctx.RespondAsync(string.Format("Sorry, I don't know what class ${0} is.", vClass));
                    }
                }
                else
                {
                    var vTopFive = vService.GuildRoster
                        .OrderByDescending(x => x.DKP_CURRENT)
                        .Take(vResults);

                    int vMaxLength = vTopFive.Max(x => x.Name.Length) + 1;

                    string vResponse = string.Format("```Here are the top 5 highest DKP members as of {0}: {1}", vService.LastUpdated, Environment.NewLine);
                    foreach (MemberModel vMember in vTopFive)
                    {
                        vResponse += string.Format("{0}{1}{2}", 
                            vMember.Name.PadRight(vMaxLength), 
                            vMember.DKP_CURRENT.ToString("N0", CultureInfo.InvariantCulture), 
                            Environment.NewLine);
                    }
                    vResponse += "```";

                    await ctx.RespondAsync(vResponse);
                }
            }
            catch (Exception vException)
            {
                Console.WriteLine(vException.Message);
                await ctx.RespondAsync($"Sorry I was not able to understand what you were asking for.");
            }
        }
    }
}
