using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Game;

namespace Tibia.Adventures.Models
{
    public class Traps
    {
        public TrapType TrapType;
        public Die DamageDie = Die.D50;
        public Die HiddenDamageDie = Die.D100;
        public bool SearcedFor = false;
        public bool TrippedOrDisarmed = false;
    }

    public enum TrapType
    {
        Poison,
        Spike,
        Fire,
        Pit,
    }
}
