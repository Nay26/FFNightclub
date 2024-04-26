using FFNightclub.Models;
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
        public int PlayerRoll { get; set; } = 20;

        public Dictionary<string, string> Messages = new Dictionary<string, string>() {
            { "StartGame", " **  STARTING A NEW ADIVENTURE! <se.12>  **" },
            { "ExplainRules", "** Welcome to  Adiventure Quest!  An exciting journey through a fantasy world! Dare you put your hard earned gil on the line for a chance at greater treasure? **" },
            { "TakePayments", "** Please trade your starting gil with the  Games Master!  Your potential rewards and losses will be proportional to the gil you trade! **" },
            { "CharacterCreation", "** Your Character has 5 Stats: Fighty, Sneaky, Brainy, Talky and Lusty. Please send a message of of your preferred order (Most Important --> Least Important). Roll bonuses will be : (+2, +1, 0, -1, -2) **" },
            { "GameBegins" , "**  Time to begin your quests!  <se.12> **" },
            { "NewEvent", "**  NEW EVENT HAPPENING!  <se.12> **" },
            { "PlayerDecided", "**  #player#  #description# **" },
            { "OptionsRolled", "**  #player#!  A test of  #availableOption#  presents itself! **" },
            { "AskPlayerOption", "** Try to use your :  #availableOption#  ? (Beat : #optionDC# with 1d20 #playerSkill#). Or bind the winds of chaos to  reroll  the event? #PlayerRerolls#/#maxRerolls# available. **" },
            { "PlayerRoll", "**  #player#!  Rolled a #adjustedRoll#! (#roll# #playerSkill#) **" },
            { "OptionSuccess", "**  #player#!  #optionSuccess# **" },
            { "OptionFailure", "**  #player#!  #optionFailure# **" }
        };

        public List<RPGEvent> Events = new List<RPGEvent>() {
            new RPGEvent()
            {
                Name = "Rumble in the Jungle",
                IntroText = "You reach a thick jungle thicket, tall green trees surround you and twisted vines hamper your progress.",
                Options = [
                    new RPGEventOption(Stats.Fighty)
                    {
                        Description = "A Lion jumps out to attack you! He roars a fierce roar!",
                        SuccessDescription = "You easily beat the lion, killing it instantly!",
                        FailureDescription = "The Lion bites you! Ouch!",
                        SkillCheck = 10
                    },
                    new RPGEventOption(Stats.Sneaky)
                    {
                        Description = "You spy a camp of goblins in the distance, it seems you may be able to sneak around them.",
                        SuccessDescription = "You effortlesly avoid detection from the meanie goblins!",
                        FailureDescription = "The meanie goblins spot you and rummage through your pockets! You manage to escape!",
                        SkillCheck = 10
                    },
                    new RPGEventOption(Stats.Lusty)
                    {
                        Description = "An alluring figure aproaches you from the bush, she admires your adventurer physique.",
                        SuccessDescription = "You manage to seduce her with your sexy skills! She lies down to sleep, exhausted by your presence.",
                        FailureDescription = "She is too much for you to handle! You drop exhausted from the encounter, while you are out she rummages through your pockets.",
                        SkillCheck = 10
                    },
                    new RPGEventOption(Stats.Brainy)
                    {
                        Description = "A wizard asks for help with his crossword puzzle.",
                        SuccessDescription = "You are a crossword wizz! The wizard is impressed!",
                        FailureDescription = "You disapoint the wizard and he robs you for the inconvienince",
                        SkillCheck = 10
                    },
                    new RPGEventOption(Stats.Talky)
                    {
                        Description = "An ogre appears \"Hewwo I am da ogre, should I eat you?\"",
                        SuccessDescription = "\"OK\" The ogre says, then walks off.",
                        FailureDescription = "\"I think I will eat you ahahaha\" The ogre chases you away.",
                        SkillCheck = 10
                    }
                ]
            }
        };
    }
}
