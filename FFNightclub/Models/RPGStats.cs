using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFNightclub.Models
{
    public class RPGStats
    {
        public Stat Fighty { get; set; }
        public Stat Sneaky { get; set; }
        public Stat Brainy { get; set; }
        public Stat Talky { get; set; }
        public Stat Lusty { get; set; }

        public List<Stat> list { get; set; } = new List<Stat>();

        public RPGStats()
        {
            Fighty = new Stat(Stats.Fighty, 0);
            list.Add(Fighty);
            Sneaky = new Stat(Stats.Sneaky, 0);
            list.Add(Sneaky);
            Brainy = new Stat(Stats.Brainy, 0);
            list.Add(Brainy);
            Talky = new Stat(Stats.Talky, 0);
            list.Add(Talky);
            Lusty = new Stat(Stats.Lusty, 0);
            list.Add(Lusty);
        }

        public float GetValue(Stats stat)
        {
            return list.Find(s => s.Name == stat).Value;
        }

        public void SetValue(Stats stat, float value)
        {
            list.Find(s => s.Name == stat).Value = value;
        }
    }

    public enum Stats
    {
        Fighty,
        Sneaky,
        Brainy,
        Talky,
        Lusty
    }

    public class Stat
    {
        public Stats Name { get; set; }
        public float Value { get; set; }

        public Stat(Stats name, float value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() {
            return $"{Name} : {Value}";
        }
    }
}
