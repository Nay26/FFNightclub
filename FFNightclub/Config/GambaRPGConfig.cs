using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFNightclub.Config
{
    public class GambaRPGConfig
    {
        public int OptionRoll { get; set; } = 5;

        public Dictionary<string, string> Messages = new Dictionary<string, string>() {
            { "TakePayments", "Please trade your starting gil with the Games Master" },
            { "NewEvent", "NEW EVENT HAPPENING! <se.12>" },
            { "OptionsRolled", "\"A test of #availableOption# presents itself.\" " },
            { "CharacterCreation", "Your Character has 5 Stats: Fighty, Sneaky, Brainy, Talky and Lusty. Please send a message of your preferred order (MostImportant -> LeastImportant)" },
            { "StartGame", "STARTING A NEW ADVENTURE!" },
            { "ExplainRules", "Welcome to RPG GAME! : Rules Explanation!" },
        };
    }
}
