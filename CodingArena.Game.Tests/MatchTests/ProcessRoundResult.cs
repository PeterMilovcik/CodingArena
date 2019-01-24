﻿using FluentAssertions;
using NUnit.Framework;

namespace CodingArena.Game.Tests.MatchTests
{
    internal class ProcessRoundResult : TestFixture
    {
        private const string WinnerName = "TestBot";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            Match.Process(RoundResult.Winner(WinnerName));
        }

        [Test]
        public void OutputUpdated() => Output.Winners[WinnerName].Should().Be(1);

        [Test]
        public void SameWinnerTwice()
        {
            Match.Process(RoundResult.Winner(WinnerName));
            Output.Winners[WinnerName].Should().Be(2);
        }
    }
}