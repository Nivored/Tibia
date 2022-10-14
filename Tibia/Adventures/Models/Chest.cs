using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Items.Interfaces;
using Tibia.Items.Models;

namespace Tibia.Adventures.Models
{
    public class Chest
    {
        public Lock Lock;
        public Traps Traps;
        public List<Item> Item;
        public int Gold;
    }
}
