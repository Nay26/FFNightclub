using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FFNightclub.Models;
using FFNightClub.Config;
using static Dalamud.Interface.Utility.Raii.ImRaii;

namespace FFNightClub.Models
{
    public class RPGPlayer
    {
        public int ID;
        public string Name = "";
        public string Alias = "";
        public float HP = 100;
        public float Money = 0;
        public string StartingMoney = "";
        public bool hasPaid = false;
        public int Roll;
        public RPGStats RPGStats = new RPGStats();

        public int Stat1 = 0;
        public int Stat2 = 1;
        public int Stat3 = 2;
        public int Stat4 = 3;
        public int Stat5 = 4;
        public List<int> statStartingList = new List<int>();

        private enum ChatNameDisplayTypes
        { FullName, SurnameAbbrv, ForenameAbbrv, Initials }

        private unsafe ChatNameDisplayTypes ChatNameDisplayType
        { get { return ChatNameDisplayTypes.FullName; } }

        public RPGPlayer()
        {
            HP = 100;
            Money = 0;
        }

        public RPGPlayer(int id, string name = "", string alias = "")
        {
            ID = id;
            Name = name;
            Alias = alias;
            HP = 100;
            Money = 0;
        }

        public string GetAlias(NameMode nameMode)
        {
            return GetAlias(Name, nameMode);
        }

        public string GetAlias(string name, NameMode nameMode)
        {
            switch (nameMode)
            {
                case NameMode.First:
                    return !name.Contains(' ') ? name : name.Substring(0, name.IndexOf(" ")).Trim();

                case NameMode.Last:
                    return !name.Contains(' ') ? name : name.Substring(name.IndexOf(" ")).Trim();
            }

            return name;
        }

        public unsafe string GetNameFromDisplayType(string name)
        {
            if (name.Contains(' '))
            {
                var displayType = ChatNameDisplayType;

                if (displayType != ChatNameDisplayTypes.FullName)
                {
                    string[] n = name.Split(' ');
                    switch (displayType)
                    {
                        case ChatNameDisplayTypes.ForenameAbbrv:
                            return $"{n[0].Substring(0, 1)}. {n[1]}";

                        case ChatNameDisplayTypes.SurnameAbbrv:
                            return $"{n[0]} {n[1].Substring(0, 1)}.";

                        case ChatNameDisplayTypes.Initials:
                            return $"{n[0].Substring(0, 1)}. {n[1].Substring(0, 1)}.";
                    }
                }
            }

            return name;
        }
    }
}
