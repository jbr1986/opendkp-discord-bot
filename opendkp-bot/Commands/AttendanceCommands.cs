using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using opendkp_bot.Services;
using opendkp_bot.Models;
using System.Linq;
using System;
using System.Globalization;

namespace opendkp_bot.Commands
{
    [Group("ra", CanInvokeWithoutSubcommand = true)] // let's mark this class as a command group
    [Description("These are raid attendance related commands")] // give it a description for help purposes
    class AttendanceCommands
    {
        private static NumberFormatInfo percentageFormat = new NumberFormatInfo { PercentPositivePattern = 1, PercentNegativePattern = 1 };
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DKPService vService = DKPService.Instance;
            MemberModel vModel = null;
            var emoji = DiscordEmoji.FromName(ctx.Client, ":disappointed:");
            //If a player name is passed to the command, lets look up their dkp information
            if (!string.IsNullOrWhiteSpace(ctx.RawArgumentString))
            {
                vModel = vService.GuildRoster.SingleOrDefault<MemberModel>(s => s.Name.Equals(ctx.RawArgumentString.Trim(), StringComparison.OrdinalIgnoreCase));
                if (vModel == null)
                {
                    await ctx.RespondAsync($"I can't find any attendance associated with {ctx.RawArgumentString} {emoji}");
                }
                else
                {
                    int vMaxLength = vModel.Name.Length + 2;
                    await ctx.RespondAsync(string.Format("```"+"Name".PadRight(vMaxLength)+"30 Day".PadRight(10)+"60 Day".PadRight(10)+"90 Day".PadRight(10)+"Life"+Environment.NewLine+
                                           "{0}{1}{2}{3}{4}```",
                        vModel.Name.PadRight(vMaxLength),
                        vModel.RA_30DayPercent.ToString("P2", percentageFormat).PadRight(10),
                        vModel.RA_60DayPercent.ToString("P2", percentageFormat).PadRight(10),
                        vModel.RA_90DayPercent.ToString("P2", percentageFormat).PadRight(10),
                        vModel.RA_LifeTimePercent.ToString("P2", percentageFormat)).PadRight(10));
                }
            }
            else
            {
                await ctx.RespondAsync($"I have dkp as of {vService.LastUpdated} available");
            }

        }
    }
}
