using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Enteties.Models;

namespace Tibia.Enteties.Interfaces
{
    public interface ICharacterService
    {
        public Character LoadCharacter(string name);

        public bool SaveCharacter(Character character);

        public List<Character> GetCharacterInRange(Guid adventureGUID, int minlevel = 1, int maxlevel = 8);

        public void CreateCharacter();
    }
}
