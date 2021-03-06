﻿using CodingArena.Game.Entities;
using CodingArena.Player;
using CodingArena.Player.Battlefield;
using CodingArena.Player.Implement;
using CodingArena.Player.TurnActions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodingArena.Game.Internal
{
    internal sealed class BattleBot : IBattleBot
    {
        private IBotAI BotAI { get; }
        private IBattlefield Battlefield { get; set; }
        private ISettings Settings { get; }

        public BattleBot(IBotAI botAI, ISettings settings)
        {
            BotAI = botAI;
            Settings = settings;
            MaxHP = Settings.MaxHP;
            HP = MaxHP;
            MaxSP = Settings.MaxSP;
            SP = MaxSP;
            MaxEP = Settings.MaxEP;
            EP = MaxEP;
            InsideView = new InsideView(this);
            OutsideView = new OutsideView(this);
        }

        public string Name => BotAI.BotName;

        public Model Model => BotAI.Model;

        public int MaxHP { get; }

        public int HP { get; private set; }

        public int MaxEP { get; }

        public int EP { get; private set; }

        public int MaxSP { get; }

        public int SP { get; private set; }

        public IBattlefieldPlace Position => Battlefield?[this];

        public IOwnBot InsideView { get; }

        public IEnemy OutsideView { get; }

        public string DestroyedBy { get; set; }

        public IBattleBot Target { get; private set; }

        public void PositionTo(IBattlefield battlefield, int newX, int newY)
        {
            Battlefield = battlefield;
            Battlefield.Set(this, newX, newY);
        }

        public void ExecuteTurnAction(IEnumerable<IBattleBot> enemies)
        {
            Reset();
            if (HP <= 0)
            {
                Action = $"{Name} is destroyed by {DestroyedBy}.";
                return;
            }

            ITurnAction turnAction;
            try
            {
                turnAction = GetTurnAction(enemies);
            }
            catch (Exception e)
            {
                Log(e);
                Destroy("system malfunction");
                Action = $"{Name} is destroyed by {DestroyedBy}.";
                return;
            }
            if (turnAction == null)
            {
                Log("Null turn action");
                Destroy("system malfunction");
                Action = $"{Name} is destroyed by {DestroyedBy}.";
                return;
            }
            switch (turnAction)
            {
                case Move move:
                    Action = ExecuteTurnAction(move);
                    return;
                case RechargeBattery rechargeBattery:
                    Action = ExecuteTurnAction(rechargeBattery);
                    return;
                case RechargeShield rechargeShield:
                    Action = ExecuteTurnAction(rechargeShield);
                    return;
                case Attack attack:
                    Action = ExecuteTurnAction(attack, enemies);
                    return;
                case MoveTowards moveTowards:
                    Action = ExecuteTurnAction(moveTowards);
                    return;
                case MoveAwayFrom moveAwayFrom:
                    Action = ExecuteTurnAction(moveAwayFrom);
                    return;
                case Idle idle:
                    Action = $"{Name} is idle.";
                    return;
            }

            Destroy("system malfunction");
            Action = $"{Name} is destroyed by {DestroyedBy}.";
        }

        private void Reset()
        {
            Target = null;
        }

        private ITurnAction GetTurnAction(IEnumerable<IBattleBot> enemies)
        {
            ITurnAction result = null;
            var task = Task.Run(() =>
                result = BotAI.GetTurnAction(
                    InsideView, enemies.Select(e => e.OutsideView).ToList(), Battlefield));
            var timeout = TimeSpan.FromSeconds(1);
            var success = task.Wait(timeout);
            if (!success)
                throw new TimeoutException($"GetTurnAction is not complete after timeout {timeout}.");

            return result;
        }

        private string ExecuteTurnAction(Attack attack, IEnumerable<IBattleBot> enemies)
        {
            if (attack.EnergyCost > EP)
                return $"{Name} does not have enough energy to attack.";

            var ownPlace = Battlefield[this];
            var targetPlace = Battlefield[attack.Target];
            var distance = ownPlace.DistanceTo(targetPlace);
            DrainEnergy(attack.EnergyCost);
            var damage = CalculateDamage(distance);
            var enemy = enemies.FirstOrDefault(e => e.OutsideView == attack.Target);

            if (enemy == null)
                return $"{Name} wants to attack, but enemy is not found on battlefield.";

            if (enemy.HP <= 0)
                return $"{Name} attempts to attack {enemy.Name} but failed, target is already destroyed.";

            if (distance > Attack.MaxRange)
                return $"{Name} attempts to attack {enemy.Name} but failed, target is out of range.";

            if (damage <= 0)
                return $"{Name} attacks {enemy.Name} with no damage.";

            enemy.TakeDamage(damage, this);
            Target = enemy;

            return enemy.HP <= 0
                ? $"{Name} destroys {enemy.Name}."
                : $"{Name} attacks {enemy.Name} with {damage} damage.";
        }

        private int CalculateDamage(double distance)
        {
            if (distance > Attack.MaxRange) return 0;
            var chance = (Attack.MaxRange - distance + 1) / Attack.MaxRange;
            return (int)(Attack.MaxDamage * chance);
        }

        private string ExecuteTurnAction(Move move)
        {
            if (move.EnergyCost > EP)
                return $"{Name} does not have enough energy to move.";

            int newX = Position.X;
            int newY = Position.Y;

            switch (move.Direction)
            {
                case Direction.East:
                    newX = Position.X + 1;
                    break;
                case Direction.West:
                    newX = Position.X - 1;
                    break;
                case Direction.South:
                    newY = Position.Y - 1;
                    break;
                case Direction.North:
                    newY = Position.Y + 1;
                    break;
                case Direction.NorthEast:
                    newY = Position.Y + 1;
                    newX = Position.X + 1;
                    break;
                case Direction.NorthWest:
                    newY = Position.Y + 1;
                    newX = Position.X - 1;
                    break;
                case Direction.SouthEast:
                    newY = Position.Y - 1;
                    newX = Position.X + 1;
                    break;
                case Direction.SouthWest:
                    newY = Position.Y - 1;
                    newX = Position.X - 1;
                    break;
            }

            if (Battlefield.IsOutOfRange(newX, newY))
            {
                Destroy("force field");
                return $"{Name} moved into force field and exploded.";
            }

            if (Battlefield[newX, newY].IsEmpty == false)
            {
                return $"{Name} cannot move {move.Direction}, place is occupied.";
            }

            DrainEnergy(move.EnergyCost);
            PositionTo(newX, newY);
            return $"{Name} moves {move.Direction}.";
        }

        private string ExecuteTurnAction(MoveTowards moveTowards)
        {
            if (moveTowards.EnergyCost > EP)
                return $"{Name} does not have enough energy to move.";

            var nearestPlaces = GetNearestPlaces();

            if (nearestPlaces.Any())
            {
                var newPlace = nearestPlaces
                    .OrderBy(p => p.DistanceTo(moveTowards.Place))
                    .First();
                var direction = GetDirectionTo(newPlace);

                if (direction != Direction.None)
                {
                    DrainEnergy(moveTowards.EnergyCost);
                    PositionTo(newPlace.X, newPlace.Y);
                    return $"{Name} moves {direction}.";
                }
                return $"{Name} stays at current position.";
            }
            return $"{Name} cannot move in any direction.";
        }

        private string ExecuteTurnAction(MoveAwayFrom moveAwayFrom)
        {
            if (moveAwayFrom.EnergyCost > EP)
                return $"{Name} does not have enough energy to move.";

            var nearestPlaces = GetNearestPlaces();

            if (nearestPlaces.Any())
            {
                var newPlace = nearestPlaces
                    .OrderByDescending(p => p.DistanceTo(moveAwayFrom.Place))
                    .First();
                var direction = GetDirectionTo(newPlace);

                if (direction != Direction.None)
                {
                    DrainEnergy(moveAwayFrom.EnergyCost);
                    PositionTo(newPlace.X, newPlace.Y);
                    return $"{Name} moves {direction}.";
                }
                return $"{Name} stays at current position.";
            }
            return $"{Name} cannot move in any direction.";
        }

        private List<IBattlefieldPlace> GetNearestPlaces()
        {
            var nearestPlaces = new List<IBattlefieldPlace>();
            // west
            var x = Position.X - 1;
            var y = Position.Y;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }
            // east
            x = Position.X + 1;
            y = Position.Y;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }
            // south
            x = Position.X;
            y = Position.Y - 1;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }
            // north
            x = Position.X;
            y = Position.Y + 1;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }
            // north-east
            x = Position.X + 1;
            y = Position.Y + 1;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }
            // north-west
            x = Position.X - 1;
            y = Position.Y + 1;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }
            // south-east
            x = Position.X + 1;
            y = Position.Y - 1;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }
            // south-west
            x = Position.X - 1;
            y = Position.Y - 1;
            if (!Battlefield.IsOutOfRange(x, y) && Battlefield[x, y].IsEmpty)
            {
                nearestPlaces.Add(Battlefield[x, y]);
            }

            nearestPlaces.Add(Position);
            return nearestPlaces;
        }

        private Direction GetDirectionTo(IBattlefieldPlace newPlace)
        {
            var direction = Direction.None;

            if (newPlace.X == Position.X && newPlace.Y == Position.Y - 1)
                direction = Direction.South;

            if (newPlace.X == Position.X && newPlace.Y == Position.Y + 1)
                direction = Direction.North;

            if (newPlace.X == Position.X + 1 && newPlace.Y == Position.Y)
                direction = Direction.East;

            if (newPlace.X == Position.X - 1 && newPlace.Y == Position.Y)
                direction = Direction.West;

            if (newPlace.X == Position.X - 1 && newPlace.Y == Position.Y - 1)
                direction = Direction.SouthWest;

            if (newPlace.X == Position.X + 1 && newPlace.Y == Position.Y - 1)
                direction = Direction.SouthEast;

            if (newPlace.X == Position.X - 1 && newPlace.Y == Position.Y + 1)
                direction = Direction.NorthWest;

            if (newPlace.X == Position.X + 1 && newPlace.Y == Position.Y + 1)
                direction = Direction.NorthEast;

            return direction;
        }

        private void PositionTo(int newX, int newY) => PositionTo(Battlefield, newX, newY);


        private string ExecuteTurnAction(RechargeBattery rechargeBattery)
        {
            if (rechargeBattery.EnergyCost > EP)
                return $"{Name} does not have enough energy to recharge battery.";
            DrainEnergy(rechargeBattery.EnergyCost);
            EP += rechargeBattery.RechargeAmount;
            if (EP > MaxEP) EP = MaxEP;
            return $"{Name} recharges battery.";
        }

        private string ExecuteTurnAction(RechargeShield rechargeShield)
        {
            int amount;
            if (EP == 0)
            {
                return $"{Name} does not have enough energy to recharge shield.";
            }
            if (rechargeShield.EnergyCost > EP)
            {
                amount = EP;
                DrainEnergy(EP);
                if (SP + amount > MaxSP)
                {
                    amount = MaxSP - SP;
                }
                if (SP == MaxSP) return $"{Name} wants to recharge shield, but it's already full.";
                SP += amount;
                return $"{Name} recharges shield by {amount} SP.";
            }
            if (SP == MaxSP) return $"{Name} wants to recharge shield, but it's already full.";
            DrainEnergy(rechargeShield.EnergyCost);
            amount = rechargeShield.RechargeAmount;
            if (SP + amount > MaxSP)
            {
                amount = MaxSP - SP;
            }
            SP += amount;
            return $"{Name} recharges shield by {amount} SP.";
        }

        public void DrainEnergy(int energyPoints)
        {
            EP -= energyPoints;
            if (EP < 0) EP = 0;
        }

        public void TakeDamage(int damage) => TakeDamage(damage, null);


        public void TakeDamage(int damage, IBattleBot attacker)
        {
            SP -= damage;
            if (SP < 0)
            {
                HP += SP;
                SP = 0;
                if (HP <= 0)
                {
                    if (attacker != null)
                    {
                        Destroy(attacker);
                    }
                    else
                    {
                        Destroy("unknown force");
                    }
                }
            }
        }

        private void Destroy(IBattleBot attacker)
        {
            attacker.Kills++;
            Destroy(attacker.Name);
        }

        private void Destroy(string cause)
        {
            HP = 0;
            Deaths++;
            DestroyedBy = cause;
            OnExploded();
        }

        public event EventHandler Exploded;

        private void OnExploded() => Exploded?.Invoke(this, EventArgs.Empty);

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public string Action { get; set; }

        public double DistanceTo(IOwnBot insideView) => Position.DistanceTo(Battlefield[insideView]);

        public double DistanceTo(IEnemy outsideView) => Position.DistanceTo(Battlefield[outsideView]);

        public override string ToString() => Name;

        private void Log(Exception exception) => Log(exception.ToString());

        private void Log(string message)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var sourceDir = Path.Combine(baseDirectory, "Bots");
            var filePath = Path.Combine(sourceDir, $"{Name}.log");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(message);
            }
        }
    }
}