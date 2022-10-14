using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Enteties.Models;

namespace Tibia.Adventures.Models
{
    public class Room
    {
        public int RoomNumber;
        public string Description;
        public Traps Traps;
        public List<Monster> Monsters;
        public Chest Chest;
        public Objective FinaleObjective;
        public List<Exit> Exits;
    }
}
