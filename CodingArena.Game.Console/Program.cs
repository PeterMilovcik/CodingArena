﻿using System;

namespace CodingArena.Game.Console
{
    class Program
    {
        public static void Main()
        {
            try
            {
                ContainerFactory.Create().GetExportedValue<IGame>().Start();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Game is broken.");
                System.Console.WriteLine($"Error message: {e}");
            }

            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey();
        }
    }
}
