namespace TestRunner
{
    using System;
    using Tests;

    public class Program
    {
        public static void Main(string[] args)
        {
            var test = new SyntaxTreeWalkerTest();
            test.Method_Test();

            Console.ReadKey();
        }
    }
}
