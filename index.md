
# CodingArena
A game for programmers to compete against each other to show best skills for creativity, fast writing and more by implementing bot artificial intelligence to fight against other bots.

### Language
Current version will only supports C# programming language.

### Match
Bots are qualified at the start of the each round if players' implementations (assemblies) are present at the server's location from which the game server loads the assemblies and tries to create bots. Round results are accumulated at match level.

### Rounds
Match consists from multiple rounds. Each round bots fights against each other and gain points for destroying other bots. 

### Turn
Every turn each bot chooses one turn action to be performed for specified input parameters (your bot, enemies, battlefield).

### TurnAction
A bot chooses turn action (e.g. move, attack, ...). 

| Turn Action           | Result       | Energy Cost |
|:----------------------|-------------:|------------:|
| Move.(East, North...) | 1 place      |           2 |
| Attack (Max Range 5)  | 0-100 damage |           5 |
| Recharge.Shield       |  +X SP       |           X |
| Recharge.Battery      | +20 EP       |           5 |
| Idle                  | nothing      |           0 |

#### Order of TurnActions (not implemented yet)
Turn actions are ordered by the bot age:
> Oldest assembly file that contains implementation of bot AI goes first, newest goes last.

### Bot AI
Player (a programmer) implements artificial intelligence for their bot to fight against other bots to kill most of enemies.

### Bot
Bot acts based on the Bot AI (implemented by a player) with following starting properties:

| Max HP | Max SP | Max EP |
|-------:|-------:|-------:|
|    500 |    200 |    500 |

> HP = Health Points  
> SP = Shield Points  
> EP = Energy Points

### Battlefield

A battlefield is area where bots fight against each other.
Configured battlefield size is `50 x 50`. 
> Be careful! Battlefield boundaries are deadly!

### Game Rules
1. Don't try to beat the game, beat enemies instead.

### How to create turn actions in your BotAI implementation

* Attack enemy:
```csharp
return TurnAction.Attack(enemy);
```
* To get max attack range:
```csharp
Attack.MaxRange
```
* Move towards enemy:
```csharp
return TurnAction.Move.Towards(enemy.Position);
```
* Move away from enemy:
```csharp
return TurnAction.Move.AwayFrom(enemy.Position);
```
* Move in specific direction:
```csharp
return TurnAction.Move.East();
return TurnAction.Move.West();
return TurnAction.Move.South();
return TurnAction.Move.North();
return TurnAction.Move.SouthEast();
return TurnAction.Move.SouthWest();
return TurnAction.Move.NorthEast();
return TurnAction.Move.NorthWest();
```
* Recharge 50 shield points (energy cost 50 energy points):
```csharp
return TurnAction.Recharge.Shield(50);
```
* Recharge battery:
```csharp
return TurnAction.Recharge.Battery();
```
* Stay idle:
```csharp
return TurnAction.Idle();
```

### How to start implementing own bot artificial intelligence

1. Create new `.NET Framework Class Library` project. (Name it, e.g. CodingArena.<YourName>.csproj)
2. Manage Nu-Get Packages for this project and install latest `CodingArena.Player` package.
3. Implement `CodingArena.Player.Implement.IBotAI` interface and compile.
4. Copy project assembly to server's bot directory (Specified by Game Master).
5. Wait for next game round to start.
6. Repeat from step 3 to improve your bot's intelligence.

### Robot Models

> Hover mouse over the picture to see the robot model name.

![Hammer](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Hammer.png "Hammer")
![Juggernaut](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Juggernaut.png "Juggernaut")
![Proto](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Proto.png "Proto")
![Rust](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Rust.png "Rust")
![Scrappie](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Scrappie.png "Scrappie")
![Scyther](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Scyther.png "Scyther")
![Sparky](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Sparky.png "Sparky")
![Tinker](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Tinker.png "Tinker")
![Twobit](https://github.com/PeterMilovcik/CodingArena/raw/master/CodingArena.Game.Wpf/Images/Twobit.png "Twobit")
