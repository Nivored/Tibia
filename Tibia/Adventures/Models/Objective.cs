using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tibia.Adventures.Models
{
    public class Objective
    {
        public ObjectType ObjectType;
    }

    public enum ObjectType
    {
        MonstersInRoom,
        AllMonsters,
        ItemObtained,
        LevelReached
    }
}
