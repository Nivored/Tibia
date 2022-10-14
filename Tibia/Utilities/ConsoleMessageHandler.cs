using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibia.Utilities.Interfaces;

namespace Tibia.Utilities
{
    public class ConsoleMessageHandler : IMessageHandler
    {
        public string Read()
        {
            return Console.ReadLine();
        }

        public void WriteRead(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }

        public void Write(string message = "")
        {
            Console.Write(message);
        }
        public void WriteL(string message = "")
        {
            Console.WriteLine(message);
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
