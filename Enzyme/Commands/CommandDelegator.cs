using Discord.WebSocket;
using Enzyme.Commands.Handlers;

namespace Enzyme.Commands;

public static class CommandDelegator {
    private static readonly Dictionary<string, ICommandHandler?> dict = new() {
        { "pathway", new PathwayCommandHandler() },
        { "outsider", new OutsiderCommandHandler() },
        { "reactrole", new ReactRoleCommandHandler() }
    };

    public static async Task Delegate(SocketSlashCommand command) {
        ICommandHandler? handler;
        if (!dict.TryGetValue(GuildUnformat(command.CommandName), out handler)) {
            throw new InvalidOperationException($"Command name '{command.CommandName}' not recognized");
        }

        await handler!.Execute(command, (command.GuildId!.Value, command.User.Id), command.Data.Options.ToDictionary(val => val.Name, val => val.Value));
    }

    private static string GuildUnformat(string name) {
        // I love rider. I don't know what this witchcraft syntax is but I love it.
        // And rider is what taught me it.
        return name[^2..] == "_g" ? name[..^2] : name;
    }
}