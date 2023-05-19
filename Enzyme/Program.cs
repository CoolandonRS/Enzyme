using CoolandonRS.consolelib;
using Discord;
using Discord.WebSocket;

namespace Enzyme;

internal static class Program {
    public static ArgHandler argHandler { get; private set; } = new ArgHandler(new Dictionary<string, ArgData>() {
        { "verbosity", new ArgData(new ArgDesc("--verbosity==[int]", "How verbose is the logging"), new ArgValue("1")) }
    }, new Dictionary<char, FlagData>() {
        { 'c', new FlagData(new ArgDesc("-c", "(Re)Declare slash commands")) }
    });
    
    public static DiscordSocketClient client { get; private set; }
    
    public static async Task Main(string[] args) {
        argHandler.ParseArgs(args);
        client = new DiscordSocketClient();
        var token = Environment.GetEnvironmentVariable("ENZYME_SECRET")!;
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        AppDomain.CurrentDomain.ProcessExit += Shutdown;
        Console.CancelKeyPress += Shutdown;

        await client.SetStatusAsync(UserStatus.Online);
        await client.SetActivityAsync(new Game("With Enzymes"));
        
        await Task.Delay(-1);
    }

    private static bool shutdown = false;
    private static object shutdownLock = new object();
    public static async void Shutdown(object? sender, EventArgs? e) {
        lock (shutdownLock) {
            if (shutdown) return;
            shutdown = true;
        }

        await client.SetActivityAsync(new Game("Shutting Down", ActivityType.Watching));
        await client.SetStatusAsync(UserStatus.DoNotDisturb);
        await client.StopAsync();
        await client.Rest.LogoutAsync();
        await client.LogoutAsync();
        await client.DisposeAsync();
    }
}