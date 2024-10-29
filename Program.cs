using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x86AssemblyPlayground.Core;

namespace x86AssemblyPlayground
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            AssemblyPlayground playground = new AssemblyPlayground();
            playground.Run();
            Console.ReadKey();
        }
    }
}
