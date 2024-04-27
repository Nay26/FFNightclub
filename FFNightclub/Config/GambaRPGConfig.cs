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
            { "StartGame", " **  STARTING A NEW ADIVENTURE! <se.12>  **" },
            { "ExplainRules", "** Welcome to  Adiventure Quest!  An exciting journey through a fantasy world! Dare you put your hard earned gil on the line for a chance at greater treasure? <se.12> **" },
            { "TakePayments", "** Please trade your starting gil with the  Games Master!  Your potential rewards and losses will be proportional to the gil you trade! <se.12> **" },
            { "CharacterCreation", "** Your Character has 5 Stats: FIGHTY, SNEAKY, BRAINY, TALKY and LUSTY. Please send a message of of your preferred order (Most Important --> Least Important). Roll bonuses will be : (+2, +1, 0, -1, -2) <se.12> **" },
            { "NewEvent", "**  NEW EVENT HAPPENING!  <se.12> **" },
            { "NewRound", "**  NEXT ROUND!  #currentRound#/#startingRound# <se.12> **" },
            { "PlayerDecided", "**  #player#  #description# **" },
            { "OptionsRolled", "**  #player#  A test of  #availableOption#  presents itself! **" },
            { "AskPlayerOption", "**  #player#  Try to use your :  #availableOption#  ? (Match or beat #optionDC# with 1d20 #playerSkill#) (You stand to win or loose #moneyGain# Gil of your current #playerMoney# Gil). Or bind the winds of chaos to  REROLL  the event? #PlayerRerolls#/#maxRerolls# available. **" },
            { "PlayerRoll", "**  #player#  Rolled a #adjustedRoll#! (#roll# #playerSkill#) **" },
            { "OptionSuccess", "**  #player#  #optionSuccess# **" },
            { "OptionFailure", "**  #player#  #optionFailure# **" },
            { "MoneyGain", "**  #player#  has WON #moneyGain# Gil! leaving them with #playerMoney# Gil Remaining  <se.12> **" },
            { "MoneyLoss", "**  #player#  has LOST #moneyGain# Gil! leaving them with #playerMoney# Gil Remaining  <se.11> **" },
            { "Reroll", "**  #player#  Is rerolling! Good luck! <se.11> **" },
            { "GameOver", "** !GAME OVER! Thank you for playing <3 Let's see what you won! <se.11> **" }
        };

        public List<RPGEvent> Events = new List<RPGEvent>() {
            new RPGEvent()
            {
                Name = "Rumble in the Jungle",
                IntroText = "You reach a thick jungle, tall green trees surround you and twisted vines hamper your progress.",
                Options = [
                    new RPGEventOption(Stats.Fighty)
                    {
                        Description = "A Lion jumps out to attack you! He roars a fierce roar!",
                        SuccessDescription = "You easily beat the lion, killing it instantly!",
                        FailureDescription = "The Lion bites you! Ouch!",
                        SkillCheck = 11
                    },
                    new RPGEventOption(Stats.Sneaky)
                    {
                        Description = "You spy a camp of goblins in the distance, it seems you may be able to sneak around them.",
                        SuccessDescription = "You effortlesly avoid detection from the meanie goblins!",
                        FailureDescription = "The meanie goblins spot you and rummage through your pockets! You manage to escape!",
                        SkillCheck = 11
                    },
                    new RPGEventOption(Stats.Lusty)
                    {
                        Description = "An alluring figure aproaches you from the bush, she admires your adventurer physique.",
                        SuccessDescription = "You manage to seduce her with your sexy skills! She lies down to sleep, exhausted by your presence.",
                        FailureDescription = "She is too much for you to handle! You drop exhausted from the encounter, while you are out she rummages through your pockets.",
                        SkillCheck = 11
                    },
                    new RPGEventOption(Stats.Brainy)
                    {
                        Description = "A wizard asks for help with his crossword puzzle.",
                        SuccessDescription = "You are a crossword wizz! The wizard is impressed! \"Take this gold friend!\"",
                        FailureDescription = "You disapoint the wizard and he robs you for the inconvienince",
                        SkillCheck = 11
                    },
                    new RPGEventOption(Stats.Talky)
                    {
                        Description = "An ogre appears \"Hewwo I am da ogre, should I eat you?\"",
                        SuccessDescription = "\"OK\" The ogre says, then walks off. Leaving a pile of gold behind",
                        FailureDescription = "\"I think I will eat you ahahaha\" The ogre chases you away. You drop some gold as you run.",
                        SkillCheck = 11
                    }
                ]
            },
            new RPGEvent()
            {
                Name = "Dubious Desert",
                IntroText = "You reach a hot sandy desert.",
                Options = [
                    new RPGEventOption(Stats.Fighty)
                    {
                        Description = "A Lion jumps out to attack you! He roars a fierce roar!",
                        SuccessDescription = "You easily beat the lion, killing it instantly!",
                        FailureDescription = "The Lion bites you! Ouch!",
                        SkillCheck = 12
                    },
                    new RPGEventOption(Stats.Sneaky)
                    {
                        Description = "You spy a camp of goblins in the distance, it seems you may be able to sneak around them.",
                        SuccessDescription = "You effortlesly avoid detection from the meanie goblins!",
                        FailureDescription = "The meanie goblins spot you and rummage through your pockets! You manage to escape!",
                        SkillCheck = 12
                    },
                    new RPGEventOption(Stats.Lusty)
                    {
                        Description = "An alluring figure aproaches you from the bush, she admires your adventurer physique.",
                        SuccessDescription = "You manage to seduce her with your sexy skills! She lies down to sleep, exhausted by your presence.",
                        FailureDescription = "She is too much for you to handle! You drop exhausted from the encounter, while you are out she rummages through your pockets.",
                        SkillCheck = 12
                    },
                    new RPGEventOption(Stats.Brainy)
                    {
                        Description = "A wizard asks for help with his crossword puzzle.",
                        SuccessDescription = "You are a crossword wizz! The wizard is impressed!",
                        FailureDescription = "You disapoint the wizard and he robs you for the inconvienince",
                        SkillCheck = 12
                    },
                    new RPGEventOption(Stats.Talky)
                    {
                        Description = "An ogre appears \"Hewwo I am da ogre, should I eat you?\"",
                        SuccessDescription = "\"OK\" The ogre says, then walks off.",
                        FailureDescription = "\"I think I will eat you ahahaha\" The ogre chases you away.",
                        SkillCheck = 12
                    }
                ]
            },
            new RPGEvent()
            {
                Name = "Bustling City",
                IntroText = "A busy city sprawls infront of you, you decide to see what secrets you can uncover.",
                Options = [
                    new RPGEventOption(Stats.Fighty)
                    {
                        Description = "A Lion jumps out to attack you! He roars a fierce roar!",
                        SuccessDescription = "You easily beat the lion, killing it instantly!",
                        FailureDescription = "The Lion bites you! Ouch!",
                        SkillCheck = 13
                    },
                    new RPGEventOption(Stats.Sneaky)
                    {
                        Description = "You spy a camp of goblins in the distance, it seems you may be able to sneak around them.",
                        SuccessDescription = "You effortlesly avoid detection from the meanie goblins!",
                        FailureDescription = "The meanie goblins spot you and rummage through your pockets! You manage to escape!",
                        SkillCheck = 13
                    },
                    new RPGEventOption(Stats.Lusty)
                    {
                        Description = "An alluring figure aproaches you from the bush, she admires your adventurer physique.",
                        SuccessDescription = "You manage to seduce her with your sexy skills! She lies down to sleep, exhausted by your presence.",
                        FailureDescription = "She is too much for you to handle! You drop exhausted from the encounter, while you are out she rummages through your pockets.",
                        SkillCheck = 13
                    },
                    new RPGEventOption(Stats.Brainy)
                    {
                        Description = "A wizard asks for help with his crossword puzzle.",
                        SuccessDescription = "You are a crossword wizz! The wizard is impressed!",
                        FailureDescription = "You disapoint the wizard and he robs you for the inconvienince",
                        SkillCheck = 13
                    },
                    new RPGEventOption(Stats.Talky)
                    {
                        Description = "An ogre appears \"Hewwo I am da ogre, should I eat you?\"",
                        SuccessDescription = "\"OK\" The ogre says, then walks off.",
                        FailureDescription = "\"I think I will eat you ahahaha\" The ogre chases you away.",
                        SkillCheck = 13
                    }
                ]
            },
            new RPGEvent()
            {
                Name = "Dark Dungeon",
                IntroText = "You enter a dark dank dungeon.",
                Options = [
                    new RPGEventOption(Stats.Fighty)
                    {
                        Description = "A Lion jumps out to attack you! He roars a fierce roar!",
                        SuccessDescription = "You easily beat the lion, killing it instantly!",
                        FailureDescription = "The Lion bites you! Ouch!",
                        SkillCheck = 14
                    },
                    new RPGEventOption(Stats.Sneaky)
                    {
                        Description = "You spy a camp of goblins in the distance, it seems you may be able to sneak around them.",
                        SuccessDescription = "You effortlesly avoid detection from the meanie goblins!",
                        FailureDescription = "The meanie goblins spot you and rummage through your pockets! You manage to escape!",
                        SkillCheck = 14
                    },
                    new RPGEventOption(Stats.Lusty)
                    {
                        Description = "An alluring figure aproaches you from the bush, she admires your adventurer physique.",
                        SuccessDescription = "You manage to seduce her with your sexy skills! She lies down to sleep, exhausted by your presence.",
                        FailureDescription = "She is too much for you to handle! You drop exhausted from the encounter, while you are out she rummages through your pockets.",
                        SkillCheck = 14
                    },
                    new RPGEventOption(Stats.Brainy)
                    {
                        Description = "A wizard asks for help with his crossword puzzle.",
                        SuccessDescription = "You are a crossword wizz! The wizard is impressed!",
                        FailureDescription = "You disapoint the wizard and he robs you for the inconvienince",
                        SkillCheck = 14
                    },
                    new RPGEventOption(Stats.Talky)
                    {
                        Description = "An ogre appears \"Hewwo I am da ogre, should I eat you?\"",
                        SuccessDescription = "\"OK\" The ogre says, then walks off.",
                        FailureDescription = "\"I think I will eat you ahahaha\" The ogre chases you away.",
                        SkillCheck = 14
                    }
                ]
            },
            new RPGEvent()
            {
                Name = "Freezing Mountain",
                IntroText = "You are climbing a tall cold mountain, coated in snow.",
                Options = [
                    new RPGEventOption(Stats.Fighty)
                    {
                        Description = "A Lion jumps out to attack you! He roars a fierce roar!",
                        SuccessDescription = "You easily beat the lion, killing it instantly!",
                        FailureDescription = "The Lion bites you! Ouch!",
                        SkillCheck = 15
                    },
                    new RPGEventOption(Stats.Sneaky)
                    {
                        Description = "You spy a camp of goblins in the distance, it seems you may be able to sneak around them.",
                        SuccessDescription = "You effortlesly avoid detection from the meanie goblins!",
                        FailureDescription = "The meanie goblins spot you and rummage through your pockets! You manage to escape!",
                        SkillCheck = 15
                    },
                    new RPGEventOption(Stats.Lusty)
                    {
                        Description = "An alluring figure aproaches you from the bush, she admires your adventurer physique.",
                        SuccessDescription = "You manage to seduce her with your sexy skills! She lies down to sleep, exhausted by your presence.",
                        FailureDescription = "She is too much for you to handle! You drop exhausted from the encounter, while you are out she rummages through your pockets.",
                        SkillCheck = 15
                    },
                    new RPGEventOption(Stats.Brainy)
                    {
                        Description = "A wizard asks for help with his crossword puzzle.",
                        SuccessDescription = "You are a crossword wizz! The wizard is impressed!",
                        FailureDescription = "You disapoint the wizard and he robs you for the inconvienince",
                        SkillCheck = 15
                    },
                    new RPGEventOption(Stats.Talky)
                    {
                        Description = "An ogre appears \"Hewwo I am da ogre, should I eat you?\"",
                        SuccessDescription = "\"OK\" The ogre says, then walks off.",
                        FailureDescription = "\"I think I will eat you ahahaha\" The ogre chases you away.",
                        SkillCheck = 15
                    }
                ]
            }
        };
    }
}
