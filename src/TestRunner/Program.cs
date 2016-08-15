using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Tests;

namespace TestRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var test = new ClassTest();
            test.Method_Test();

            Console.ReadKey();
        }
    }
}
