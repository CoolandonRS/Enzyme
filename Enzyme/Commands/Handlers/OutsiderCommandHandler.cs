using Discord;
using Discord.WebSocket;
using static Enzyme.Misc.Verbosity;
using static Enzyme.Commands.CommandUtil;

namespace Enzyme.Commands.Handlers; 

public class OutsiderCommandHandler : ICommandHandler {
    public async Task Execute(SocketSlashCommand command, (ulong guild, ulong user) id, Dictionary<string, object> param) {
        WriteDebug($"{id.user} in {id.guild} ran outsider");
        var ids = GetRoleIds(id);
        if (ids.Contains(studentId) || ids.Contains(teacherId)) {
            WriteDebug("Nonoutsider");
            await command.RespondAsync(embed: BaseEmbed(id.user).WithColor(Color.Red).WithTitle("Failure").WithDescription("Already registered as non-outsider").Build(), ephemeral: true);
            WriteDebug("Responded");
            return;
        }
        if (ids.Contains(outsiderId)) {
            WriteDebug("Already outsider");
            await command.RespondAsync(embed: BaseEmbed(id.user).WithColor(Color.Red).WithTitle("Failure").WithDescription("Already registered as outsider").Build(), ephemeral: true);
            WriteDebug("Responded");
            return;
        }
        WriteDebug("Adding role");
        await Program.client.Rest.AddRoleAsync(id.guild, id.user, outsiderId);
        WriteDebug("Role added");
        await command.RespondAsync(embed: BaseEmbed(id.user).WithColor(Color.Green).WithTitle("Success").WithDescription("Registered as outsider").Build(), ephemeral: true);
        WriteDebug("Responded");
    }
}