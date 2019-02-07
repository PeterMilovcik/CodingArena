﻿using System.ComponentModel.Composition;

namespace CodingArena.Game.Tests.Doubles
{
    [Export(typeof(IOutput))]
    internal class Output : IOutput
    {
        public void Observe(IGame game)
        {
        }

        public void Error(string message) => System.Console.WriteLine($"Error: {message}");
    }
}