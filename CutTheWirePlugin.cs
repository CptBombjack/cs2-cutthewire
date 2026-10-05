using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Text.Json;


namespace CutTheWirePlugin;


public class Config
{
    //public List<ulong> admins { get; set; } = new List<ulong>();
    public int noKitOdds { get; set; }
    public int kitOdds { get; set; }
    public bool pluginEnabled {get; set;}
}


[MinimumApiVersion(129)]
public class CutTheWirePlugin : BasePlugin
{
    private const string Version = "1.1.2";
    
    public override string ModuleName => "Cut the Wire Plugin";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "CptBombjack - http://github.com/CptBombjack";
    public override string ModuleDescription => "Allows a CT to try to quick defuse the bomb by randomly cut a wire";
    private static readonly string LogPrefix = $"[Cut-the-Wire {Version}] ";
    private static readonly string MessagePrefix = $"[{ChatColors.Green}Cut the Wire{ChatColors.White}] ";

    private int tapCounter = 0;
    private Config config = new Config();
    private string configPath = "";
             
    
    public override void Load(bool hotReload)
    {
        configPath = Path.Join(ModuleDirectory, "cutTheWireConfig.json");
        if (!File.Exists(configPath))
        {
            //config.admins.Add(1234566789123456);
            config.kitOdds = 4;
            config.noKitOdds = 8;
            config.pluginEnabled = true;
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        }
        else config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));


        Console.WriteLine($"{LogPrefix}Plugin loaded");
        Logger.LogInformation($"{LogPrefix}Plugin loaded");
    }
    
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        tapCounter = 0;
        if(! config.pluginEnabled) return HookResult.Continue;

        Server.PrintToChatAll($"{MessagePrefix}  CT can trippeltap defuse button to quick defuse");
        Server.PrintToChatAll($"{MessagePrefix} With kit: odds of success is 1:{config.kitOdds}");
        Server.PrintToChatAll($"{MessagePrefix} Without kit: odds of success is 1:{config.noKitOdds}");

        return HookResult.Continue;
    }


    [GameEventHandler]
    public HookResult OnBombBeginDefuse(EventBombBegindefuse @event, GameEventInfo info)
    {
        if(! config.pluginEnabled) return HookResult.Continue;
                        
        var player = @event.Userid;

        if (player == null || !player.IsValid) return HookResult.Continue;
                
        tapCounter++;
        if (tapCounter == 3) CutTheWire(player);

        // tapCounter is reset after 1 second
        AddTimer(1.0f, () => tapCounter = 0);
                
        return HookResult.Continue;
    }
    
    private void CutTheWire(CCSPlayerController player)
    {       
        var plantedBombList = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4").ToList();
         
        if(!plantedBombList.Any()) 
        {
            Console.WriteLine($"{LogPrefix}No planted bomb found");
            return;  
        }
        var plantedBomb = plantedBombList.FirstOrDefault();


        // User has kit: 1:config.kitOdds odds of picking the right wire. No kit: 1:config.noKitOdds
        var correctWireOdds = player.PawnHasDefuser ? config.kitOdds : config.noKitOdds;

        // Cut random wire 
        var cutWire = new Random().Next(1,correctWireOdds + 1 );
        Console.WriteLine($"{LogPrefix}{player.PlayerName} cutted wire " + cutWire + ". Odds was 1:" + correctWireOdds);
        Logger.LogInformation($"{LogPrefix}{player.PlayerName} cutted wire " + cutWire + " Odds was 1:" + correctWireOdds);

        // Correct wire is always 1
        if (cutWire == 1) 
        {  
        Server.NextFrame(() =>
        {
            plantedBomb!.DefuseCountDown = 0;
            var outputText = $"{player.PlayerName} quick defused the bomb by cutting the correct wire! (1:{correctWireOdds} odds)";
            Console.WriteLine($"{LogPrefix}{outputText}");
            Server.PrintToChatAll($"{MessagePrefix}{outputText}");
        });
        } 
        else 
        {
            Server.NextFrame(() =>
            {
                plantedBomb!.C4Blow = 1.0f;
                var outputText = $"{player.PlayerName} Cut the wrong wire and set of the bomb! (1:{correctWireOdds} odds)";
                Console.WriteLine($"{LogPrefix}{outputText}");
                Server.PrintToChatAll($"{MessagePrefix}{outputText}");
            });
        }

    }

    [ConsoleCommand("ctw_kitodds")]
    public void OnCTWKitOdds(CCSPlayerController? controller, CommandInfo command)

    { 
        if (controller == null) return;
        if (Regex.IsMatch(command.GetArg(1), @"^\d+$") && 
                int.Parse(command.GetArg(1)) > 0 && 
                int.Parse(command.GetArg(1)) < 101
            )
        {
            config.kitOdds = int.Parse(command.GetArg(1));
             File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
            Console.WriteLine($"{LogPrefix}Odds when using kit set to 1:{config.kitOdds}");
            Logger.LogInformation($"{LogPrefix}Odds when using kit set to 1:{config.kitOdds}");
            Server.PrintToChatAll($"{MessagePrefix}Odds when using kit set to 1:{config.kitOdds} by {controller.PlayerName}");
        }
        else 
        {
            controller.PrintToChat($"Odds must be an integer 1 -- 100");
        }
    }

    [ConsoleCommand("ctw_nokitodds")]
    public void OnCTWNoKitOdds(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;
        if  (Regex.IsMatch(command.GetArg(1), @"^\d+$") && 
                int.Parse(command.GetArg(1)) > 0 && 
                int.Parse(command.GetArg(1)) < 101
            )
        {
            config.noKitOdds = int.Parse(command.GetArg(1));
             File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
            Console.WriteLine($"{LogPrefix}Odds not using kit set to 1:{config.noKitOdds}");
            Logger.LogInformation($"{LogPrefix}Odds not using kit set to 1:{config.noKitOdds} by {controller.PlayerName}");
            Server.PrintToChatAll($"{MessagePrefix}Odds when not using kit set to 1:{config.noKitOdds}");
        }
        else
        {
            controller.PrintToChat($"Odds must be an integer 1 -- 100");
        }
    }
     [ConsoleCommand("ctw_control")]
     public void OnCTWOnOff(CCSPlayerController? controller, CommandInfo command)
     {
        
        if (controller == null) return;
       
        config.pluginEnabled = ! config.pluginEnabled; 
        File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        
        var outputText = string.Format("Plugin turned {0}", config.pluginEnabled ? "ON" : "OFF");
        Console.WriteLine($"{LogPrefix}{outputText}");
        Logger.LogInformation($"{LogPrefix}{outputText} by {controller.PlayerName}");
        Server.PrintToChatAll($"{MessagePrefix}{outputText}");
       
     }

     [ConsoleCommand("ctw_help")]
     public void OnCTWtHelp(CCSPlayerController? controller, CommandInfo command)
     {

        if (controller == null) return;

        Logger.LogInformation($"{LogPrefix}{controller.PlayerName} asked for help");

        Server.PrintToChatAll($"{MessagePrefix} COMMAND HELP");
        Server.PrintToChatAll($"!ctw_control   - Toggles plugin on/off");
        Server.PrintToChatAll($"!ctw_kitodds   - Set odds when using kit. Default: 4");
        Server.PrintToChatAll($"!ctw_nokitodds - Set odds when NOT using kit. Default: 8");
     }

    //TODO: private checkAdmin 
    // Check that user is an admin here adn call from all console commands

}
