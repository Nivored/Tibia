using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Items.Interfaces;

namespace Tibia.Items.Models
{
    public class Item : IItem
    {
        public ItemType Name;
        public string Description;
        public int ObjectiveNumber;
        public int Weight;
        public int GoldValue;
    }

    public enum ItemType
    {
        Rope,
        Torch,
        Shovel,
        Scyth,
        Food,
        Lockpicks,
        Key,
        GoldenToken,
    }
}
