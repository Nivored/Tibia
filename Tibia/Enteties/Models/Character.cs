using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Items.Interfaces;
using Tibia.Items.Models;

namespace Tibia.Enteties.Models
{
    public class Character : Entity
    {
        public int Exp = 0;
        public Skills Skills = new Skills();
        public int InventoryCap;
        public List<Guid> AdventuresPlayed = new List<Guid>();
        public CharacterClass Class;
    }

    public class Skills
    {
        public int Meele;
        public int Shielding;
        public int MagicLevel;
        public int DistanceFighting;
        public int LockPick;
    }

    public enum CharacterClass
    {
        Knight,
        Sorcerer,
        Druid,
        Paladin
    }
}
