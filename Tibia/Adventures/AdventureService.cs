using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tibia.Adventures
{
    public class AdventureService : IAdventureService
    {
        public Adventure GetInitialAdventure()
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}Adventure";
            var initialAdventure = new Adventure();

            if (File.Exists($"{basePath}\\Adventure.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var initialJsonFile = directory.GetFiles("Adventure.json");

                using (StreamReader fi = File.OpenText(initialJsonFile[0].FullName))
                {
                    initialAdventure = JsonConvert.DeserializeObject<Adventure>(fi.ReadToEnd());
                }
            }

            return initialAdventure;
        }
    }
}
