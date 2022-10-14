using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tibia.Adventures;
using Tibia.Enteties.Interfaces;
using Tibia.Enteties.Models;
using Tibia.Items.Models;
using Tibia.Utilities;
using Tibia.Utilities.Interfaces;

namespace Tibia.Enteties
{
    public class CharacterService : ICharacterService
    {
        private readonly IMessageHandler messageHandler;

        public CharacterService(IMessageHandler MessageHandler)
        {
            messageHandler = MessageHandler;
        }

        public Character LoadCharacter(string name)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var character = new Character();

            if (File.Exists($"{basePath}\\{name}.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var characterJsonFile = directory.GetFiles($"{name}.json");

                using (StreamReader fi = File.OpenText(characterJsonFile[0].FullName))
                {
                    character = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                }
            }
            else
            {
                throw new Exception("Character not found!");
            }

            return character;
        }

        public List<Character> GetCharacterInRange(Guid adventureGUID, int minlevel = 1, int maxlevel = 8)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var characterInRange = new List<Character>();

            try
            {
                var directory = new DirectoryInfo(basePath);
                foreach (var file in directory.GetFiles($"*.json"))
                {
                    using (StreamReader fi = File.OpenText(file.FullName))
                    {
                        var potentialCharacterInRange = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                        if (potentialCharacterInRange.Level >= minlevel && (potentialCharacterInRange.Level <= maxlevel) && !potentialCharacterInRange.AdventuresPlayed.Contains(adventureGUID))
                        {
                            characterInRange.Add(potentialCharacterInRange);
                        }
                        
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Oh Snap! {ex.Message}");
                throw;
            }

            return characterInRange;
        }

        public bool SaveCharacter(Character character)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";

                File.WriteAllText($"{basePath}\\{character.Name}.json",JsonConvert.SerializeObject(character));
                return true;
        }

        public void CreateCharacter()
        {
            messageHandler.WriteL("*********************************");
            messageHandler.WriteL("*        Create Character!      *");
            messageHandler.WriteL("*********************************\n\n");

            var newCharacter = new Character();
            newCharacter.Inventory = new List<Item>();

            messageHandler.WriteL("Characters Name Please : ");
            newCharacter.Name = messageHandler.Read();

            messageHandler.WriteL("\nClass? (K)night, (S)orcerer, (D)ruid, (P)aladin");
            switch (messageHandler.Read().ToLower())
            {
                case "k":
                    newCharacter.Class = CharacterClass.Knight;
                    newCharacter.Health = 250;
                    newCharacter.Mana = 35;
                    newCharacter.Skills.Meele = 10;
                    newCharacter.Skills.Shielding = 10;
                    newCharacter.Skills.MagicLevel = 2;
                    newCharacter.Attack = new Attack { BaseDie = 20, BonusDamage = 0 };
                    newCharacter.InventoryCap = 200;
                    break;
                case "s":
                    newCharacter.Class = CharacterClass.Sorcerer;
                    newCharacter.Health = 120;
                    newCharacter.Mana = 75;
                    newCharacter.Skills.MagicLevel = 10;
                    newCharacter.Skills.Shielding = 3;
                    newCharacter.Attack = new Attack { BaseDie = 20, BonusDamage = 0 };
                    newCharacter.InventoryCap = 150;
                    break;
                case "d":
                    newCharacter.Class = CharacterClass.Druid;
                    newCharacter.Health = 120;
                    newCharacter.Mana = 75;
                    newCharacter.Skills.MagicLevel = 10;
                    newCharacter.Skills.Shielding = 3;
                    newCharacter.Attack = new Attack { BaseDie = 20, BonusDamage = 0 };
                    newCharacter.InventoryCap = 150;
                    break;
                case "p":
                    newCharacter.Class = CharacterClass.Paladin;
                    newCharacter.Health = 200;
                    newCharacter.Mana = 50;
                    newCharacter.Skills.MagicLevel = 5;
                    newCharacter.Skills.Shielding = 5;
                    newCharacter.Skills.DistanceFighting = 10;
                    newCharacter.Skills.LockPick = 3;
                    newCharacter.Attack = new Attack { BaseDie = 20, BonusDamage = 0 };
                    newCharacter.InventoryCap = 175;
                    break;
                default:
                    Console.WriteLine("\nDumbfuq. You did not enter a valid letter");
                    break;
            }

            messageHandler.Clear();
            messageHandler.WriteL("\n======================================\n\n");
            messageHandler.WriteL("Here is your new character!");
            DisplayCharacter(newCharacter);
            messageHandler.WriteL("\n What is your choice?  (S)ave or (R)edo");
            var playerChoice = messageHandler.Read().ToLower();

            if (playerChoice == "r")
            {
                CreateCharacter();
            }
            else if (playerChoice == "s")
            {
                if (SaveCharacter(newCharacter))
                {
                    messageHandler.Clear();
                    Program.Menu();
                }
            }
        }

        private void WriteSkills(Skills skills)
        {
            messageHandler.WriteL("\nSKILLS ");
            messageHandler.WriteL("######################");
            messageHandler.WriteL($"Meele:              {skills.Meele}");
            messageHandler.WriteL($"Shielding:          {skills.Shielding}");
            messageHandler.WriteL($"Magic level:        {skills.MagicLevel}");
            messageHandler.WriteL($"Distance fighting:  {skills.DistanceFighting}");
            messageHandler.WriteL($"Lockpicking:        {skills.LockPick}");
            messageHandler.WriteL("######################");
        }

        private void DisplayCharacter(Character character)
        {
            messageHandler.WriteL($"\n\n********************************");
            messageHandler.WriteL($"NAME: {character.Name.ToUpper()}");
            messageHandler.WriteL($"LEVEL: {character.Level}");
            messageHandler.WriteL($"CLASS: {character.Class}");
            WriteSkills(character.Skills);
        }
    }
}
