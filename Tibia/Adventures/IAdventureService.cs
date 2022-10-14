using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tibia.Adventures
{
    public interface IAdventureService
    {
        Adventure GetInitialAdventure();
    }
}
