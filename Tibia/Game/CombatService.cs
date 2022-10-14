using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Enteties.Models;
using Tibia.Game.Interfaces;
using Tibia.Items.Models;
using Tibia.Utilities.Interfaces;

namespace Tibia.Game
{
    public class CombatService : ICombatService
    {
        private readonly IMessageHandler messageHandler;

        public CombatService(IMessageHandler MessageHandler)
        {
            messageHandler = MessageHandler;
        }

        public void RunCombat(ref Character character, List<Monster> monsters)
        {
            var monsterDescription = "You face off against : ";
            foreach (var monster in monsters)
            {
                monsterDescription += $" -- {monster.MonsterType}";
            }

            messageHandler.WriteL(monsterDescription);

            var dice = new Dice();
            var d20 = new List<Die> { Die.D20 };
            var charDmgDie = new List<Die> { (Die)character.Attack.BaseDie };

            var charInitiative = dice.RollDice(new List<Die> { Die.D20 });
            var monsterInitiative = dice.RollDice(new List<Die> { Die.D20 });

            messageHandler.WriteRead("Hit a Key to roll for Initiative!");

            messageHandler.WriteRead($"You rolled {charInitiative} and the monsters rolled {monsterInitiative}");

            while (charInitiative == monsterInitiative)
            {
                messageHandler.WriteL($"Thats a tie, lets roll again!");

                charInitiative = dice.RollDice(d20);
                monsterInitiative = dice.RollDice(d20);

                messageHandler.WriteL($"You rolled {charInitiative} and the monsters rolled {monsterInitiative}");
            }

            var monstersAreAlive = true;
            //var monsterGold = 0;
            //var monsterInventory = new List<Item>();

            var charactersTurn = (charInitiative > monsterInitiative);

            while ( monstersAreAlive)
            {
                if (charactersTurn)
                {
                    //Character Attack
                    messageHandler.WriteRead($"Hit a Key to Attack the {monsters[0].MonsterType}!\n================");

                    var attackToHitMonster = dice.RollDice(d20);
                    messageHandler.Write($"You rolled a {attackToHitMonster} against the monster's armor class of {monsters[0].ArmorClass}");

                    if (attackToHitMonster >= monsters[0].ArmorClass)
                    {
                        var damage = dice.RollDice(charDmgDie);
                        messageHandler.Write($"You swung and hit the {monsters[0].MonsterType} for {damage} damage!");

                        monsters[0].Health -= damage;
                        if (monsters[0].Health < 1)
                        {
                            messageHandler.Write($"You have killed the {monsters[0].MonsterType}");

                            if (monsters[0].Gold > 0)
                            {
                                messageHandler.Write($"It had {monsters[0].Gold} GOLD!");
                                character.Gold += monsters[0].Gold;
                            }

                            if (monsters[0].Inventory.Count > 0)
                            {
                                messageHandler.WriteL($"It also has some inventory!  You get:");

                                foreach (Item item in monsters[0].Inventory)
                                {
                                    messageHandler.WriteL(item.Description);
                                }

                                character.Inventory.AddRange(monsters[0].Inventory);
                            }

                            monsters.RemoveAt(0);
                            if (monsters.Count < 1)
                            {
                                monstersAreAlive = false;
                            }
                        }
                    }
                    else
                    {
                        messageHandler.WriteL("Swing and a miss!!");
                    }

                    charactersTurn = false;
                }
                else //Monsters Turn
                {
                    messageHandler.WriteL($"================\nThe {monsters[0].MonsterType} attacks!");

                    var attackToHitCharacter = dice.RollDice(d20);
                    messageHandler.WriteL($"The monster rolls a {attackToHitCharacter} and your AC is {character.ArmorClass}");
                    if (attackToHitCharacter >= character.ArmorClass)
                    {
                        messageHandler.WriteL("The monster hits!");
                        var damage = dice.RollDice(new List<Die> { (Die)monsters[0].Attack.BaseDie });

                        messageHandler.WriteL($"It hits you for {damage}!\n");

                        character.Health -= damage;
                        if (character.Health < 1)
                        {
                            messageHandler.WriteL($"You have died.... at the hands of a viscious {monsters[0].MonsterType}");

                        }
                    }
                    else
                    {
                        messageHandler.WriteL("Swing and a miss!!");
                    }
                    charactersTurn = true;
                }
            }
        }
    }
}
