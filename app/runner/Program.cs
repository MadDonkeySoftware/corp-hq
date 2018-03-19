// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;

    /// <summary>
    /// The class containing the main entry point for the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">Optional arguments for the program</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
        }
    }
}
