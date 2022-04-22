namespace NekoChattingBot
{
    public static class TwitchCredentials
    {
        public static readonly string[] Channels = new string[] { "pancakes" };
        public static readonly string mods = "ThatOneGuyWhoSpamsPogpega, aeonim, andros18, bennyoberwinch, bigtimemassivecash, binfy, bluecrystal004, boxbox, chase55t, chrchie, cloud9, deadrote, derpyfoxplayz, digitalhypno, dios_dong, doremy, elcheer, eleeement_, emanfman, enyoti, fossabot, happystick, honmi, intlcoco, itswinter, jamiethebull, joiechii, kaoran, kerneon, ksn24, l3lackshark, littleendu, luckfire, mikuia, mrchompysaur, mrdutchboi, mrnolife_, nightbot, olibomby, oralekin, piscator_, rainbowmeeps, ryotou, shigetora, shugi, slpchatbot, soarnathan, soran2202, streamelements, stunterletsplay, thepoon, theramu, therealzachtv, toekneered, toybickler, tru3o_o, tuonto, warpworldbot, wholewheatpete, yazzehh, ypaperr, zarrah, zonelouise, nekopavel";
        public static readonly string password = Environment.GetEnvironmentVariable("TWITCH_OAUTH", EnvironmentVariableTarget.User);
        public static readonly string botUsername = "NekoChattingBot";
    }
}
