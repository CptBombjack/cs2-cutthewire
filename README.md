# CS2 Cut-The-Wire

You are the last CT standing. The bomb has been planted and several T are protecting the bomb. With normal defusing methods, your chance of winning the round is somewhere between slim and none. What if you could rush to the bomb in true "do or die" fashion and just cut a wire by chance and win the round?

This script lets a CT try to instantly defuse the bomb by cutting a wire in either of two ways:
- Start defusing three times within one second.
- Start defusing, then press primary attack (left mouse button by default) while actively defusing.

Your odds (configurable) of defusing the bomb depends on if you are carrying the defuse kit or not:
- With kit: default odds is 1:3
- Without kit: default odds is 1:6

This is a CS2 port of the Sourcemod script "QuickDefuse", rewritten in C# for CounterStrikeSharp

Requires .NET 10 and CounterStrikeSharp API 1.0.374 or later.

## Credit
[pRED*] (https://forums.alliedmods.net/showthread.php?p=520661) for the initial Sourcemod plugin "QuickDefuse" which this script is based upon

