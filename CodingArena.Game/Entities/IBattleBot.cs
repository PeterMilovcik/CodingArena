﻿using System;
using System.Collections.Generic;
using CodingArena.Player;
using CodingArena.Player.Battlefield;

namespace CodingArena.Game.Entities
{
    public interface IBattleBot : IBot
    {
        int MaxHP { get; }
        int HP { get; }
        int MaxEP { get; }
        int EP { get; }
        int MaxSP { get; }
        int SP { get; }
        IBattlefieldPlace Position { get; }
        IOwnBot InsideView { get; }
        IEnemy OutsideView { get; }
        string DestroyedBy { get; set; }
        void PositionTo(IBattlefield battlefield, int x, int y);
        string ExecuteTurnAction(IEnumerable<IBattleBot> enemies);
        void DrainEnergy(int energyPoints);
        void TakeDamage(int damage);
        void TakeDamage(int damage, IBattleBot attacker);
        event EventHandler Exploded;
        int Kills { get; set; }
        int Deaths { get; set; }
        double DistanceTo(IOwnBot insideView);
        double DistanceTo(IEnemy outsideView);
    }
}