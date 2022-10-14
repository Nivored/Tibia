using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Items.Models;

namespace Tibia.Enteties.Models
{
    public abstract class Entity
    {
        public string Name;
        public int Health = 0;
        public Attack Attack;
        public int Mana = 0;
        public int Gold;
        public int Level = 1;
        public int ArmorClass;
        public List<Item> Inventory;
    }

    public class Attack
    {
        public int BaseDie;
        public int BonusDamage;
    }
}
