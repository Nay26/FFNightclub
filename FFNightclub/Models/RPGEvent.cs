using FFNightclub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFNightclub.Models
{
    public class RPGEvent
    {
        public string Name { get; set; }
        public string IntroText { get; set; }
        public RPGEventOption[] Options { get; set; }

        public RPGEvent()
        {
           Name = "Generic Event";
           IntroText = "You arrive at a cool location some options appear before you :";
           Options = new RPGEventOption[] {
                new RPGEventOption(Stats.Fighty),
                new RPGEventOption(Stats.Sneaky),
                new RPGEventOption(Stats.Lusty),
                new RPGEventOption(Stats.Brainy),
                new RPGEventOption(Stats.Talky),
            };
        }
    }

    public class RPGEventOption
    {
        public Stats Stat { get; set; }
        public string Description { get; set; }
        public string SuccessDescription { get; set; }
        public string FailureDescription { get; set; }
        public float SkillCheck { get; set; }

        public RPGEventOption(Stats stat)
        {
            Stat = stat;
            Description = $"Do a thing related to your {stat}";
            SkillCheck = 10;
            SuccessDescription = $"You use your {stat} to succeed!";
            FailureDescription = $"You use your {stat} to fail Miserabley!";
        }

    }
}
