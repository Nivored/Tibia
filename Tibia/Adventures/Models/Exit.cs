using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tibia.Adventures.Models
{
    public class Exit
    {
        public Lock Lock;
        public CompassDirection WallLocation;
        public Riddle Riddle;
        public int LeadsToRoomNumber;
    }

    public enum CompassDirection
    {
        North,
        East,
        South,
        West
    }
}
