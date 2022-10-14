using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Adventures.Models;

namespace Tibia.Adventures
{
    public class Adventure
    {
        public Guid GUID;
        public string Title;
        public string Description;
        public int CompletionXPReward;
        public int CompletionGoldReward;
        public int FinaleObjective;
        public int MinLevel;
        public int MaxLevel;
        public List<Room> Rooms;

        public Adventure()
        {

        }

    }
}
