using System;

namespace NekoChattingBot
{
    public static class TwitchCredentials
    {
        /// <summary>
        /// Channels that will be initially joined.
        /// </summary>
        public static readonly string[] Channels = new string[] { "btmc", "nekopavel", "nekochattingbot", "thatoneguywhospamspogpega" };
        /// <summary>
        /// Mods of the bot/channel, idk.
        /// </summary>
        /// I really want to change this, but I'll let it be like that for now.
        public static readonly string mods = "ThatOneGuyWhoSpamsPogpega, aeonim, andros18, bennyoberwinch, bigtimemassivecash, binfy, bluecrystal004, boxbox, chase55t, chrchie, cloud9, deadrote, derpyfoxplayz, digitalhypno, dios_dong, doremy, elcheer, eleeement_, emanfman, enyoti, fossabot, happystick, honmi, intlcoco, itswinter, jamiethebull, joiechii, kaoran, kerneon, ksn24, l3lackshark, littleendu, luckfire, mikuia, mrchompysaur, mrdutchboi, mrnolife_, nightbot, olibomby, oralekin, piscator_, rainbowmeeps, ryotou, shigetora, shugi, slpchatbot, soarnathan, soran2202, streamelements, stunterletsplay, thepoon, theramu, therealzachtv, toekneered, toybickler, tru3o_o, tuonto, warpworldbot, wholewheatpete, yazzehh, ypaperr, zarrah, zonelouise, nekopavel";
        /// <summary>
        /// Twitch OAuth key for authentication of the user.
        /// Recommended to use <see href="https://twitchapps.com/tmi/">TwitchApps</see> generator.
        /// </summary>
        public static readonly string password = Environment.GetEnvironmentVariable("TWITCH_OAUTH", EnvironmentVariableTarget.User);
        /// <summary>
        /// The username of the bot.
        /// </summary>
        public static readonly string botUsername = "NekoChattingBot";
    }
}
