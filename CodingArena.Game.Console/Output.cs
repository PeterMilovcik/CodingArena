﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using static System.Console;

namespace CodingArena.Game.Console
{
    [Export(typeof(IOutput))]
    internal class Output : IOutput
    {
        public ISettings Settings { get; }
        private int MatchRow { get; }
        private int RoundRow { get; set; }
        private int TurnRow { get; set; }
        private int Row { get; set; }
        private IGame Game { get; set; }

        [ImportingConstructor]
        public Output(ISettings settings)
        {
            Settings = settings;
            MatchRow = 1;
        }

        public void Observe(IGame game)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            Game.MatchStarting += OnMatchStarting;
            Game.MatchFinished += OnMatchFinished;
        }

        private void OnMatchStarting(object sender, EventArgs e)
        {
            Game.Match.RoundStarting += OnRoundStarting;
            Game.Match.RoundFinished += OnRoundFinished;
        }

        private void OnMatchFinished(object sender, EventArgs e)
        {
            Game.Match.RoundStarting -= OnRoundStarting;
            Game.Match.RoundFinished -= OnRoundFinished;
        }

        private void OnRoundStarting(object sender, EventArgs e)
        {
            Game.Match.Round.TurnStarting += OnTurnStarting;
            Game.Match.Round.TurnFinished += OnTurnFinished;
        }

        private void OnRoundFinished(object sender, EventArgs e)
        {
            Game.Match.Round.TurnStarting -= OnTurnStarting;
            Game.Match.Round.TurnFinished -= OnTurnFinished;
        }

        private void OnTurnStarting(object sender, EventArgs e)
        {
        }

        private void OnTurnFinished(object sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            Clear();
            DisplayHeader(0, "Coding Arena");
            Update(Game.Match);
            Update(Game.Match.Round);
            Update(Game.Match.Round.Turn);
        }

        private void Update(IMatch match)
        {
            DisplayHeader(MatchRow, "Match");
            int row = MatchRow + 1;
            foreach (var score in match.Scores.OrderByDescending(s => s.Kills))
            {
                DisplayScore(row, score);
                row++;
            }
            RoundRow = MatchRow + row - 1;
        }

        private void Update(IRound round)
        {
            DisplayHeader(RoundRow, $"Round ({round.Number} / {Settings.MaxRounds}) Battlefield [ {round.Battlefield.Width} x {round.Battlefield.Height} ]");
            TurnRow = RoundRow + 1;
        }

        private void Update(ITurn turn)
        {
            DisplayHeader(TurnRow, $"Turn  ({turn.Number} / {Settings.MaxTurns})");
            int row = TurnRow + 1;

            foreach (var bot in Game.Match.Round.Bots)
            {
                DisplayBot(row, bot);
                row++;
            }

            DisplayHeader(row++, "Actions");

            foreach (var pair in turn.BotActions)
            {
                DisplayRow(row, pair.Value);
                row++;
            }
        }

        private void DisplayBot(int row, IBattleBot bot) =>
            DisplayRow(row,
                $"  * {bot.Name,-30} " +
                $"[HP: {bot.HP,3:F0} SP: {bot.SP,3:F0} EP: {bot.EP,3:F0}] " +
                $"[X: {bot.Position.X,2}, Y: {bot.Position.Y,2}]");

        public void Error(string message)
        {
            var previousColor = ForegroundColor;
            ForegroundColor = ConsoleColor.Red;
            WriteLine(message);
            ForegroundColor = previousColor;
        }

        public void Update(IEnumerable<IBattleBot> bots, TurnResult turnResult)
        {
            //Row = 1;
            //DisplayHeader(Row, "Match");
            //if (Scores != null)
            //{
            //    DisplayHeader(Row, "Match ============================= K == D");
            //    Row++;
            //    foreach (var score in Scores.OrderByDescending(s => s.Kills))
            //    {
            //        DisplayScore(Row, score);
            //        Row++;
            //    }
            //}
            //DisplayHeader(Row, "Round");
        }
        
        private void DisplayScore(int row, Score score) =>
            DisplayRow(row,
                $"  * {score.BotName,-30} " +
                $"{score.Kills,4:N0} {score.Deaths,4:N0}");

        private void DisplayHeader(int rowIndex, string text)
        {
            FullRow(rowIndex, "=");
            CursorTop = rowIndex;
            CursorLeft = 1;
            WriteLine($" {text} ");
        }

        private void DisplayRow(int row, string message)
        {
            FullRow(row, " ");
            CursorTop = row;
            CursorLeft = 0;
            WriteLine(message);
        }

        private void FullRow(int row, string s)
        {
            CursorTop = row;
            CursorLeft = 0;
            WriteLine(string.Join("", Enumerable.Repeat(s, BufferWidth - 1)));
        }
    }
}