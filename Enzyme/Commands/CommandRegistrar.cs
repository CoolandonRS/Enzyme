using System.Text;
using Discord;
using Discord.Net;
using Newtonsoft.Json;
using static Enzyme.Misc.Verbosity;

namespace Enzyme.Commands; 

public static class CommandRegistrar {
    public static async Task RegisterAll() {
        WriteDebug("Declaring Global Commands");
        WriteDebug("Finished Global Declarations");
    }

    public static async Task RegisterGuild(string strId) {
        var id = ulong.Parse(strId);
        WriteDebug($"Declaring Commands to guild {strId}");
        await AssignPathway(id);
        await Outsider(id);
        WriteDebug($"Finished declaring commands to guild {strId}");
    }

    public static async Task RegisterAllGuild(string strId) {
        var id = ulong.Parse(strId);
        WriteDebug($"Declaring Global Commands to guild {strId}");
        WriteDebug($"Finished declaring Global commands to guild {strId}");
    }

    public static async Task RemoveAll() {
        WriteDebug("Deleting all global commands");
        await Program.client.Rest.BulkOverwriteGlobalCommands(Array.Empty<ApplicationCommandProperties>());
        WriteDebug("Finished deleting all global commands");
    }

    public static async Task RemoveAllGuild(string strId) {
        WriteDebug($"Deleting guild commands for guild {strId}");
        await Program.client.Rest.BulkOverwriteGuildCommands(Array.Empty<ApplicationCommandProperties>(), ulong.Parse(strId));
        WriteDebug($"Finished deleting guild commands for guild {strId}");
    }
    public static async Task AssignPathway(ulong? id = null) {
        if (id == null) throw new InvalidOperationException("AssignPathway cannot be declared globally");
        var command = new SlashCommandBuilder().WithName("pathway").WithDescription("Join or leave a pathway")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("action").WithDescription("join/leave pathway").WithRequired(true)
                .AddChoice("join", 1)
                .AddChoice("leave", 2)
                .WithType(ApplicationCommandOptionType.Integer)
            ).AddOption(new SlashCommandOptionBuilder()
                .WithName("pathway").WithDescription("Name of pathway").WithRequired(true)
                .AddChoice("Programming", 1)
                .AddChoice("Cybersecurity", 2)
                .AddChoice("Game Dev", 3)
                .AddChoice("Web Dev", 4)
                .AddChoice("Drones", 5)
                .AddChoice("UAV", 7)
                .AddChoice("Audio", 8)
                .AddChoice("Graphic Design", 9)
                .AddChoice("Video", 10)
                .AddChoice("Construction", 11)
                .AddChoice("Business", 12)
                .AddChoice("Culinary", 13)
                .AddChoice("Baking", 14)
                .AddChoice("Medical Assisting", 15)
                .AddChoice("Pharmacy", 16)
                .WithType(ApplicationCommandOptionType.Integer)
            ).Build();

        FullRegister(command, id);
    }

    public static async Task Outsider(ulong? id = null) {
        if (id == null) throw new InvalidOperationException("AssignPathway cannot be declared globally");
        var command = new SlashCommandBuilder().WithName("outsider").WithDescription("Register as an outsider").Build();
        FullRegister(command, id);
    }

    private static void FullRegister(SlashCommandProperties? command, ulong? id) {
        try {
            Register(command, id);
        } catch (HttpException e) {
            WriteError("Failed to declare command");
            if (showErrorData()) {
                var json = JsonConvert.SerializeObject(e.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
            throw e; // rethrow after printing
        }
    }

    private static void Register(SlashCommandProperties? command, ulong? id) {
        if (id == null) {
            Program.client.Rest.CreateGlobalCommand(command);
        } else {
            Program.client.Rest.CreateGuildCommand(command, id.Value);
        }
    }
}