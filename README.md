# CS2 Cut-The-Wire

Requires .NET 10 and CounterStrikeSharp API 1.0.374 or later. Build with `dotnet build --configuration Release` and install `bin/Release/net10.0/CutTheWirePlugin.dll` in the plugin directory. CounterStrikeSharp supplies its own API assembly; do not copy `CounterStrikeSharp.API.dll` from the build output to the server.

You are the last CT standing. The bomb has been planted and several T are protecting the bomb. With normal defusing methods, your chance of winning the round is somewhere between slim and none. What if you could rush to the bomb in true "do or die" fashion and just cut a wire by chance and win the round?

This script lets a CT try to instantly defuse the bomb by cutting a wire in either of two ways:
- Start defusing three times within one second.
- Hold the defuse button at the bomb, then press secondary attack (right mouse button by default) while actively defusing.

Both methods use the same odds. You might cut the wrong cable and instead set off the bomb! Each bomb allows one wire-cut attempt. Holding secondary attack does not cause repeated attempts, and pressing it before starting to defuse does not trigger an attempt.

Your odds (configurable) of defusing the bomb depends on if you are carrying the defuse kit or not:
- With kit: default odds are 1:4
- Without kit: default odds are 1:8

This is a CS2 port of the Sourcemod script "QuickDefuse", rewritten in C# for CounterStrikeSharp


## Credit
[pRED*] (https://forums.alliedmods.net/showthread.php?p=520661) for the initial Sourcemod plugin "QuickDefuse" which this script is based upon

