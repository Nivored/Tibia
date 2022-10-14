using System.Media;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Tibia.Adventures;
using Tibia.Enteties;
using Tibia.Game;
using Tibia.Utilities;

namespace Tibia
{
    internal class Program
    {
        private static readonly AdventureService adventureService = new AdventureService();
        private static readonly ConsoleMessageHandler consoleMessageHandler = new ConsoleMessageHandler();
        private static readonly CharacterService characterService = new CharacterService(consoleMessageHandler);
        private static readonly CombatService combatService = new CombatService(consoleMessageHandler);
        private static GameService gameService = new GameService(adventureService, characterService, consoleMessageHandler, combatService);

        static void Main(string[] args)
        {
            Header();
            using (SoundPlayer player = new SoundPlayer($"{AppDomain.CurrentDomain.BaseDirectory}/Sound/shortIntro.wav"))
            {
                player.Play();
                Menu(player);
            }
            
            
        }


        private static void Header()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\t************************************************************");
            Console.WriteLine(@"	* _________  ________   _______    ________  ________      *");
            Console.WriteLine(@"	*/________/\/_______/\/_______/\  /_______/\/_______/\     *");
            Console.WriteLine(@"	*\__.::.__\/\__.::._\/\::: _  \ \ \__.::._\/\::: _  \ \    *");
            Console.WriteLine(@"	*   \::\ \     \::\ \  \::(_)  \/_   \::\ \  \::(_)  \ \   *");
            Console.WriteLine(@"	*    \::\ \    _\::\ \__\::  _  \ \  _\::\ \__\:: __  \ \  *");
            Console.WriteLine(@"	*     \::\ \  /__\::\__/\\::(_)  \ \/__\::\__/\\:.\ \  \ \ *");
            Console.WriteLine(@"	*      \__\/  \________\/ \_______\/\________\/ \__\/\__\/ *");
            Console.WriteLine(@"	*                                                          *");
            Console.WriteLine("\t************************************************************\n\n");
            Console.ResetColor();
        }
        public static void Menu(SoundPlayer player = null)
        {
            MenuText();
            var inputValid = false;
            try
            {
                while (!inputValid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    var menuInput = Console.ReadLine().ToUpper();
                    switch (menuInput)
                    {
                        case "S":
                            player.Stop();
                            gameService.StartGame();
                            inputValid = true;
                            break;
                        case "L":
                            //LoadGame();
                            inputValid = true;
                            break;
                        case "C":
                            Console.Clear();
                            characterService.CreateCharacter();
                            inputValid = true;
                            break;
                        default:
                            Console.WriteLine("\nDumbfuq. You did not enter a valid letter");
                            MenuText();
                            break;
                    }

                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dumbfuq, Somthing went wrong! {ex.Message}");
            }
            
        }

        private static void MenuText()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("(S)tart a new game");
            Console.WriteLine("(L)oad a game");
            Console.WriteLine("(C)reate new character");
            Console.ResetColor();
        }


    }
}