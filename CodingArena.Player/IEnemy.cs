﻿namespace CodingArena.Player
{
    public interface IEnemy : IRobot
    {
        IValueState Health { get; }
        IValueState Shield { get; }
    }
}