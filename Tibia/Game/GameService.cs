using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tibia.Adventures;
using Tibia.Adventures.Models;
using Tibia.Enteties.Interfaces;
using Tibia.Enteties.Models;
using Tibia.Game.Interfaces;
using Tibia.Items.Interfaces;
using Tibia.Items.Models;
using Tibia.Utilities.Interfaces;

namespace Tibia.Game
{
    public class GameService : IGameService
    {
        private IAdventureService adventureService;
        private ICharacterService characterService;
        private IMessageHandler messageHandler;
        private ICombatService combatService;

        private Character character;
        private Adventure gameAdventure;
        private bool gameWon = false;
        private string gameWiningDescription;

        public GameService(IAdventureService AdventureService, ICharacterService CharacterService, IMessageHandler MessageHandler, ICombatService CombatService)
        {
            adventureService = AdventureService;
            characterService = CharacterService;
            messageHandler = MessageHandler;
            combatService = CombatService;
        }

        public bool StartGame(Adventure adventure = null)
        {
            gameAdventure = adventure;
            if (gameAdventure == null)
            {
                gameAdventure = adventureService.GetInitialAdventure();
            }

            CreateTitleBanner(gameAdventure.Title);

            CreateDescriptionBanner(gameAdventure);

            var charactersInRange = characterService.GetCharacterInRange(gameAdventure.GUID, gameAdventure.MinLevel, gameAdventure.MaxLevel);

            if (charactersInRange.Count == 0)
            {
                messageHandler.WriteL("Sorry, you don't have any characters that fit this adventure");
                return false;
            }
            else
            {
                messageHandler.WriteL("Who do you wish to use on this journey?");
                var cahracterCount = 0;
                foreach (var character in charactersInRange)
                {
                    messageHandler.WriteL($"#{cahracterCount} {character.Name} Level - {character.Level} {character.Class}");
                    cahracterCount++;
                }
            }

            character = characterService.LoadCharacter(charactersInRange[Convert.ToInt32(messageHandler.Read())].Name);

            var rooms = gameAdventure.Rooms;
            RoomProcessor(rooms[0]);

            return true;
        }

        private void CreateDescriptionBanner(Adventure adventure)
        {
            messageHandler.WriteL($"\n{adventure.Description.ToUpper()}");
            messageHandler.WriteL($"\nFor Level Between {adventure.MinLevel} - {adventure.MaxLevel}");
            messageHandler.WriteL($"\nCompletion Rewards = {adventure.CompletionGoldReward} gold and {adventure.CompletionXPReward} EXP");
            messageHandler.WriteL();
        }

        private void CreateTitleBanner(string title)
        {
            messageHandler.Clear();
            messageHandler.WriteL();

            //Create title banner
            for (int i = 0; i <= title.Length + 3; i++)
            {
                messageHandler.Write("*");
                if (i == title.Length + 3)
                {
                    messageHandler.Write("\n");
                }
            }

            messageHandler.WriteL($"| {title} |");
            for (int i = 0; i <= title.Length + 3; i++)
            {
                messageHandler.Write("*");
                if (i == title.Length + 3)
                {
                    messageHandler.Write("\n");
                }
            }
        }

        private void RoomProcessor(Room room)
        {
            RoomDescription(room);
            RoomOptions(room);
        }


        private void RoomDescription(Room room)
        {
            messageHandler.Clear();
            messageHandler.WriteL("******************************");

            messageHandler.WriteL($"{room.RoomNumber} {room.Description}");

            if (room.Exits.Count == 1)
            {
                messageHandler.WriteL($"There is an exit on this room on the {room.Exits[0].WallLocation} wall.");
            }
            else
            {
                var exitDescription = "";
                foreach (var exit in room.Exits)
                {
                    exitDescription += $"{exit.WallLocation}, ";
                }

                messageHandler.WriteL(
                    $"This room has exits on the {exitDescription.Remove(exitDescription.Length - 2)} walls.");
            }

            if (room.Chest != null)
            {
                messageHandler.WriteL($"There is a chest in the room!");
            }

            if (room.Monsters != null)
            {
                messageHandler.WriteL($"Monsters are attacking you!");
                combatService.RunCombat(ref character, room.Monsters);
            }
        }

        private void RoomOptions(Room room)
        {
            WriteRoomOptions(room);

            var playerDecision = messageHandler.Read().ToLower();
            var exitRoom = false;
            while (exitRoom == false)
            {
                switch (playerDecision)
                {
                    case "l":
                    case "c":
                        CheckForTraps(room);
                        if (gameWon)
                        {
                            GameOver();
                        }
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    case "o":
                        if (room.Chest != null)
                        {
                            OpenChest(room.Chest);
                            WriteRoomOptions(room);
                            playerDecision = messageHandler.Read().ToLower();
                        }
                        else
                        {
                            messageHandler.WriteL("There is no chest here!");
                        }
                        break;
                    case "n":
                    case "s":
                    case "e":
                    case "w":
                        var wallLocation = CompassDirection.North;
                        if (playerDecision == "s") wallLocation = CompassDirection.South;
                        else if (playerDecision == "w") wallLocation = CompassDirection.West;
                        else if (playerDecision == "e") wallLocation = CompassDirection.East;

                        if(room.Exits.FirstOrDefault(x => x.WallLocation == wallLocation) != null)
                        {
                            ExitRoom(room, wallLocation);
                        }
                        else
                        {
                            messageHandler.WriteL("\nEhm... That is just a wall!\n");
                            WriteRoomOptions(room);
                            playerDecision = messageHandler.Read().ToLower();
                        }
                        break;
                }
            }
        }

        private void WriteRoomOptions(Room room)
        {
            messageHandler.WriteL("What would you like to do?");
            messageHandler.WriteL("----------------------------------");
            messageHandler.WriteL("(L)ook for traps");
            if (room.Chest != null)
            {
                messageHandler.WriteL("(O)pen the chest");
                messageHandler.WriteL("(C)heck chest for traps");
            }
            messageHandler.WriteL("Use an Exit");
            foreach (var exit in room.Exits)
            {
                messageHandler.WriteL($"({exit.WallLocation.ToString().Substring(0, 1)}){exit.WallLocation.ToString().Substring(1)}");
            }

        }

        private void CheckForTraps(Room room)
        {
            if (room.Traps != null)
            {
                if (room.Traps.TrippedOrDisarmed)
                {
                    messageHandler.WriteL("You've already found and disarmed this trap... or tripped it (ouch)");
                    return;
                }

                if (room.Traps.SearcedFor)
                {
                    messageHandler.WriteL("You have already search for a trap... DumbDumb ");
                    return;
                }

                var trapBonus = 0 + character.Skills.MagicLevel;
                if (character.Class == CharacterClass.Druid || character.Class == CharacterClass.Sorcerer)
                {
                    trapBonus += 3;
                }
                else
                {
                    trapBonus -= 2;
                }

                var dice = new Dice();
                var findTrapRoll = dice.RollDice(new List<Die>{Die.D20}) + trapBonus;

                if (findTrapRoll < 12)
                {
                    messageHandler.WriteL("You find no traps!");
                    room.Traps.SearcedFor = true;
                    return;
                }

                messageHandler.WriteL("You find a trap, and you try to disarm it!");

                var disarmTrapRoll = dice.RollDice(new List<Die> { Die.D20 }) + trapBonus;

                if (disarmTrapRoll < 11)
                {
                    ProcessTrapMsgAndDmg(room.Traps);
                }
                else
                {
                    messageHandler.WriteL("WOHOO!!  You disarmed the trap!");
                }

                room.Traps.TrippedOrDisarmed = true;
                return;
            }
            messageHandler.WriteL("You find no traps!");
            return;
        }

        private void ProcessTrapMsgAndDmg(Traps traps)
        {
            var dice = new Dice();
            messageHandler.WriteL("BOOOM!   You fail and suffer from it!");
            var trapDamage = dice.RollDice(new List<Die>() { traps.DamageDie });
            var health = character.Health - trapDamage;
            messageHandler.WriteL($"You where damage for {trapDamage} health by the trap! and have {health} left!!");
            if (health < 1)
            {
                messageHandler.WriteL("Dead!");
            }
            
        }

        private void OpenChest(Chest chest)
        {
            if (chest.Lock == null || !chest.Lock.Locked)
            {
                if (chest.Traps != null && !chest.Traps.TrippedOrDisarmed)
                {
                    ProcessTrapMsgAndDmg(chest.Traps);
                    chest.Traps.TrippedOrDisarmed = true;
                }
                else
                {
                    messageHandler.WriteL("You open the chest..");
                    if (chest.Gold > 0)
                    {
                        character.Gold += chest.Gold;
                        messageHandler.WriteL($"You find {chest.Gold} gold in this chest and have now {character.Gold} gold!");
                        chest.Gold = 0;
                    }

                    if (chest.Item != null && chest.Item.Count > 0)
                    {
                        messageHandler.WriteL($"You find {chest.Item.Count} items in this chest! and they are");

                        foreach (var item in chest.Item)
                        {
                            messageHandler.Write(item.Name.ToString());

                            if (item.ObjectiveNumber == gameAdventure.FinaleObjective)
                            {
                                gameWon = true;
                                character.Gold += gameAdventure.CompletionGoldReward;
                                character.Exp += gameAdventure.CompletionXPReward;
                                character.AdventuresPlayed.Add(gameAdventure.GUID);
                            }
                        }
                        messageHandler.WriteL("\n");

                        character.Inventory.AddRange(chest.Item);
                        chest.Item = new List<Item>();

                        if (gameWon == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            messageHandler.WriteL("\t************************************************************");
                            messageHandler.WriteL("	* You done the finale objective!                            *");
                            messageHandler.WriteL("\t************************************************************\n\n");
                            messageHandler.WriteL("Exp Reward = " + gameAdventure.CompletionXPReward + ", " + gameAdventure.CompletionGoldReward +  " gold");
                            messageHandler.WriteL(character.Name + " Now has " + character.Exp + " Exp and " + character.Gold + " Gold");

                            Console.ResetColor();
                        }
                        return;
                    }

                    if (chest.Gold > 0 && (chest.Item == null || chest.Item.Count == 0))
                    {
                        messageHandler.WriteL("you Find Nothing in this shitty chest!");
                    }
                }
            }
            else
            {
                if (TryUnlock(chest.Lock))
                {
                    OpenChest(chest);
                    if (gameWon)
                    {
                        GameOver();
                    }
                }
            }
            
        }

        private void ExitRoom(Room room, CompassDirection wallLocation)
        {
            var dice = new Dice();
            if(room.Traps != null && room.Traps.TrippedOrDisarmed == false)
            {
                var triggerTrap = dice.RollDice(new List<Die>() { room.Traps.HiddenDamageDie });
                var health = character.Health - triggerTrap;
                messageHandler.WriteL($"you hit the hidden trap and took {triggerTrap} damage! leaving you with {health}");
            }
            messageHandler.Read();
            var exit = room.Exits.FirstOrDefault(x => x.WallLocation == wallLocation);
            if (exit == null)
            {
                throw new Exception("This room dont have that exception");
            }
            var newRoom = gameAdventure.Rooms.FirstOrDefault(x => x.RoomNumber == exit.LeadsToRoomNumber);

            if (newRoom == null)
            {
                throw new Exception("The room that this previous room was supose to lead too does not exist!? bad game design!");
            }

            if ((exit.Lock == null || !exit.Lock.Locked) || TryUnlock(exit.Lock))
            {
                RoomProcessor(newRoom);
            }
            else
            {
                RoomProcessor(room);
            }

            messageHandler.WriteL("You either cannot proceed or your walking away from this locked door.... mabye find a key?");
            RoomProcessor(newRoom);



            room.Traps.TrippedOrDisarmed = true;
        }

        private bool TryUnlock(Lock theLock)
        {
            if (!theLock.Locked) return true;

            var hasOptions = true;
            var dice = new Dice();
            var theLocalLock = theLock;

            while (hasOptions)
            {
                if (!theLock.Attempted)
                {
                    messageHandler.WriteL("Locked! Would you like to attempt to unlock it? \n" + "(K)ey (L)ockpick (B)ash (W)alk away");
                    var playerDecision = messageHandler.Read().ToLower();
                    switch (playerDecision)
                    {
                        case "k":
                            if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Key && x.ObjectiveNumber == theLocalLock.KeyNumber) != null) 
                            {
                                messageHandler.WriteRead("You have the right key! it unlocks the lock! \n");
                                theLock.Locked = false;
                                return true;
                            }
                            else
                            {
                                messageHandler.WriteL("You do not have a key for this chest \n");
                                break;
                            }
                        case "l":
                            if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Lockpicks) == null)
                            {
                                messageHandler.WriteL("You don't have lockpicks! \n");
                                break;
                            }
                            else
                            {
                                var lockpickBonus = 0 + character.Skills.LockPick;
                                if (character.Class == CharacterClass.Paladin ||
                                    character.Class == CharacterClass.Knight)
                                {
                                    lockpickBonus += 3;
                                }

                                var pickRoll = (dice.RollDice(new List<Die> { Die.D20 }) + lockpickBonus);
                                if (pickRoll >= 12)
                                {
                                    messageHandler.WriteRead($"You manage the skill of lockpicking \n" + $"Your lockpick roll was {pickRoll} and you needed 12! \n");
                                    theLock.Locked = false;
                                    theLock.Attempted = true;
                                }
                                messageHandler.WriteRead($"Snap! the lock doesnt budge! \n" + $"Your lockpick roll was {pickRoll} and you need 12!\n");
                                theLock.Attempted = true;
                                break;
                            }
                        case "b":
                            var bashBonus = 0 + character.Skills.Meele;
                            if (character.Class == CharacterClass.Knight)
                            {
                                bashBonus += 3;

                            }

                            var bashRoll = (dice.RollDice(new List<Die> { Die.D20 }) + bashBonus);
                            if (bashRoll >= 16)
                            {
                                messageHandler.WriteRead($"you muster the strenght That Bash taht silly lock into submission! \n" + $"You bashRoll was {bashRoll} and you needed 16! \n");
                                theLock.Locked = false;
                                theLock.Attempted = true;
                                return true;
                            }
                            messageHandler.WriteRead($"Ouch! the lock doesnt budge! \n" + $"Your bash roll was {bashRoll} and you needed 16! \n");
                            theLock.Attempted=true;
                            break;

                        default:
                            return false;
                    }
                }
                else
                {
                    if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Key && x.ObjectiveNumber == theLocalLock.KeyNumber) != null)
                    {
                        messageHandler.WriteL("you've tried bashing or picking to no avail. But you have the right key! unlocked! \n");
                        theLock.Locked = false;
                        return true;
                    }
                    else
                    {
                        messageHandler.WriteL("You've cannot try to bash or pick this lock again and you do not currently have a key! \n");
                        return false;
                    }
                }
            }

            return false;
        }

        private void GameOver()
        {
            characterService.SaveCharacter(character);
            character = new Character();
            messageHandler.WriteRead("The game is over! Press enter to return to the main menu!");
            messageHandler.Clear();
            Program.Menu();
        }
    }
}