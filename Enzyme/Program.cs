using CoolandonRS.consolelib;
using Discord;
using Discord.WebSocket;
using Enzyme.Commands;
using Microsoft.VisualBasic;
using static Enzyme.Misc.Verbosity;

namespace Enzyme;

internal static class Program {
    public static ArgHandler argHandler { get; private set; } = new ArgHandler(new Dictionary<string, ArgData>() {
        { "verbosity", new ArgData(new ArgDesc("--verbosity==[int]", "How verbose is the logging"), new ArgValue("1")) },
        { "guild-id", new ArgData(new ArgDesc("--guild-id=[id]", "The Guild ID to use in Guild Only mode (-G)"))}
    }, new Dictionary<char, FlagData>() {
        { 'C', new FlagData(new ArgDesc("-C", "(Re)Declare global commands")) },
        { 'c', new FlagData(new ArgDesc("-c", "Delete all global commands")) },
        { 'G', new FlagData(new ArgDesc("-G", "Guild Only Mode. Requires --guild-id")) },
        { 'g', new FlagData(new ArgDesc("-g", "Delete a guilds previous commands. Requires --guild-id"))}
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
        
        client.Ready += ClientReady;
        client.SlashCommandExecuted += CommandDelegator.Delegate;

        if (showDebug()) client.Log += async msg => ConsoleUtil.WriteColoredLine($"DISCORD.NET: {msg.ToString()}", ConsoleColor.DarkGray);

        await Task.Delay(-1);
    }

    public static async Task ClientReady() {
        if (argHandler.GetFlag('c')) {
            await CommandRegistrar.RemoveAll();
        }

        if (argHandler.GetFlag('C')) {
            await CommandRegistrar.RegisterAll();
        }

        var guildId = argHandler.GetValue("guild-id");
        
        if (argHandler.GetFlag('g')) {
            if (!guildId.IsSet()) throw new InvalidOperationException("-g without --guild-id");
            await CommandRegistrar.RemoveAllGuild(guildId.AsString());
        }

        if (guildId.IsSet()) {
            await CommandRegistrar.RegisterGuild(guildId.AsString());
        }

        if (argHandler.GetFlag('G')) {
            if (!guildId.IsSet()) throw new InvalidOperationException("-G without --guild-id");
            await CommandRegistrar.RegisterAllGuild(guildId.AsString());
        }
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