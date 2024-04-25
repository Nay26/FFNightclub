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
            { "RoundStart", " Starting a New Round!  <se.12>" },
            { "PlayerRolled", "\" #player#  Rolls a #publicRoll#\" " },
            { "OptionsRolled", "\"A test of #availableOption# presents itself.\" " },
        };
    }
}
