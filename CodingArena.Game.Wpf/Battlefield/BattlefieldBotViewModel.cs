using CodingArena.Game.Entities;
using CodingArena.Game.Wpf.Common;
using CodingArena.Player;
using System.Windows;

namespace CodingArena.Game.Wpf.Battlefield
{
    internal class BattlefieldBotViewModel : ViewModel
    {
        private readonly int myBattlefieldWidth;
        private readonly int myBattlefieldHeight;
        private readonly IBattlefield myBattlefield;
        private string myBotName;
        private int myX;
        private int myY;
        private string myImageSource;
        private int myHP;
        private int myMaxHP;
        private int myMaxSP;
        private int mySP;
        private int myMaxEP;
        private int myEP;
        private Visibility myProgressBarVisibility;
        private Visibility myAttackVisibility;
        private int myAttackX1;
        private int myAttackX2;
        private int myAttackY1;
        private int myAttackY2;
        private Visibility myShieldVisibility;
        private Visibility myBatteryVisibility;

        public BattlefieldBotViewModel(IBattleBot battleBot, int battlefieldWidth, int battlefieldHeight, IBattlefield battlefield)
        {
            myBattlefieldWidth = battlefieldWidth;
            myBattlefieldHeight = battlefieldHeight;
            myBattlefield = battlefield;
            Width = 40;
            Height = 50;
            UpdateFrom(battleBot);
        }

        public string BotName
        {
            get => myBotName;
            private set
            {
                if (value == myBotName) return;
                myBotName = value;
                OnPropertyChanged();
            }
        }

        public int Width { get; }
        public int Height { get; }

        public int X
        {
            get => myX;
            private set
            {
                if (value == myX) return;
                myX = value;
                OnPropertyChanged();
            }
        }

        public int Y
        {
            get => myY;
            private set
            {
                if (value == myY) return;
                myY = value;
                OnPropertyChanged();
            }
        }

        public string ImageSource
        {
            get => myImageSource;
            set
            {
                if (value == myImageSource) return;
                myImageSource = value;
                OnPropertyChanged();
            }
        }

        public int HP
        {
            get => myHP;
            set
            {
                if (value == myHP) return;
                myHP = value;
                OnPropertyChanged();
            }
        }

        public int MaxHP
        {
            get => myMaxHP;
            set
            {
                if (value == myMaxHP) return;
                myMaxHP = value;
                OnPropertyChanged();
            }
        }

        public int MaxSP
        {
            get => myMaxSP;
            set
            {
                if (value == myMaxSP) return;
                myMaxSP = value;
                OnPropertyChanged();
            }
        }

        public int SP
        {
            get => mySP;
            set
            {
                if (value == mySP) return;
                mySP = value;
                OnPropertyChanged();
            }
        }

        public int MaxEP
        {
            get => myMaxEP;
            set
            {
                if (value == myMaxEP) return;
                myMaxEP = value;
                OnPropertyChanged();
            }
        }

        public int EP
        {
            get => myEP;
            set
            {
                if (value == myEP) return;
                myEP = value;
                OnPropertyChanged();
            }
        }

        public Visibility ProgressBarVisibility
        {
            get => myProgressBarVisibility;
            set
            {
                if (value == myProgressBarVisibility) return;
                myProgressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility AttackVisibility
        {
            get => myAttackVisibility;
            set
            {
                if (value == myAttackVisibility) return;
                myAttackVisibility = value;
                OnPropertyChanged();
            }
        }

        public int AttackX1
        {
            get => myAttackX1;
            set
            {
                if (value == myAttackX1) return;
                myAttackX1 = value;
                OnPropertyChanged();
            }
        }

        public int AttackY1
        {
            get => myAttackY1;
            set
            {
                if (value == myAttackY1) return;
                myAttackY1 = value;
                OnPropertyChanged();
            }
        }

        public int AttackX2
        {
            get => myAttackX2;
            set
            {
                if (value == myAttackX2) return;
                myAttackX2 = value;
                OnPropertyChanged();
            }
        }

        public int AttackY2
        {
            get => myAttackY2;
            set
            {
                if (value == myAttackY2) return;
                myAttackY2 = value;
                OnPropertyChanged();
            }
        }

        public Visibility ShieldVisibility
        {
            get => myShieldVisibility;
            set
            {
                if (value == myShieldVisibility) return;
                myShieldVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility BatteryVisibility
        {
            get => myBatteryVisibility;
            set
            {
                if (value == myBatteryVisibility) return;
                myBatteryVisibility = value;
                OnPropertyChanged();
            }
        }

        public void UpdateFrom(IBattleBot bot)
        {
            BotName = bot.Name;
            UpdateCoordinates(bot);
            ImageSource = bot.HP > 0
                ? $"../Images/{bot.Model}.png"
                : $"../Images/{bot.Model}_dead.png";
            if (bot.Model == Model.Twobit && bot.HP > 0)
            {
                ImageSource = $"../Images/{bot.Model}.gif";
            }

            ShieldVisibility = bot.Action != null && bot.Action.Contains("recharges shield")
                ? Visibility.Visible
                : Visibility.Hidden;

            BatteryVisibility = bot.Action != null && bot.Action.Contains("recharges battery")
                ? Visibility.Visible
                : Visibility.Hidden;

            MaxHP = bot.MaxHP;
            HP = bot.HP;
            MaxSP = bot.MaxSP;
            SP = bot.SP;
            MaxEP = bot.MaxEP;
            EP = bot.EP;
            ProgressBarVisibility = HP > 0 ? Visibility.Visible : Visibility.Hidden;
            UpdateAttack(bot);
        }

        private void UpdateAttack(IBattleBot bot)
        {
            if (bot.Target != null)
            {
                int offsetX = -Width / 2;
                int offsetY = -Height / 2;
                AttackX1 = 25;
                AttackY1 = 10;

                var enemyX = bot.Target.Position.X * (myBattlefieldWidth / myBattlefield.Width) + offsetX;
                var enemyY = myBattlefieldHeight - bot.Target.Position.Y * (myBattlefieldHeight / myBattlefield.Height) - (myBattlefieldHeight / myBattlefield.Height) + offsetY;

                AttackX2 = enemyX - X + 25;
                AttackY2 = enemyY - Y + 25;
                AttackVisibility = Visibility.Visible;
            }
            else
            {
                AttackVisibility = Visibility.Hidden;
            }
        }

        private void UpdateCoordinates(IBattleBot bot)
        {
            int offsetX = -Width / 2;
            int offsetY = -Height / 2;
            X = bot.Position.X * (myBattlefieldWidth / myBattlefield.Width) + offsetX;
            Y = myBattlefieldHeight - bot.Position.Y * (myBattlefieldHeight / myBattlefield.Height) - (myBattlefieldHeight / myBattlefield.Height) + offsetY;
        }
    }
}