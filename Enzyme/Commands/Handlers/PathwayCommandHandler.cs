using Discord;
using Discord.WebSocket;
using static Enzyme.Misc.Verbosity;
using static Enzyme.Commands.CommandUtil;

namespace Enzyme.Commands.Handlers; 

public class PathwayCommandHandler : ICommandHandler {
    public async Task Execute(SocketSlashCommand command, (ulong guild, ulong user) id, Dictionary<string, object> param) {
        WriteDebug($"{id.user} in {id.guild} ran pathway with {param["action"]} {param["pathway"]}");
        switch ((long)param["action"]) {
            case 1:
                WriteDebug("Adding Role");
                if (GetRoleIds(id).Contains(outsiderId)) {
                    WriteDebug("outsider");
                    await command.RespondAsync(embed: BaseEmbed(id.user).WithColor(Color.Red).WithTitle("Unable To Join").WithDescription("You are registered as an outsider").Build(), ephemeral: true);
                    WriteDebug("Responded");
                    return;
                }
                await Program.client.Rest.AddRoleAsync(id.guild, id.user, GetPathway(param).id);
                WriteDebug("Added Role");
                var rIds = GetRoleIds(id);
                if (!(rIds.Contains(studentId) || rIds.Contains(teacherId))) {
                    await Program.client.Rest.AddRoleAsync(id.guild, id.user, studentId);
                    WriteDebug("Added Attending");
                }
                await command.RespondAsync(embed: BaseEmbed(id.user).WithColor(Color.Green).WithTitle("Joined Pathway").WithDescription(GetPathway(param).name).Build(), ephemeral: true);
                WriteDebug("Responded");
                break;
            case 2:
                WriteDebug("Removing role");
                if (!GetRoleIds(id).Contains(GetPathway(param).id)) {
                    WriteDebug("Not in pathway");
                    await command.RespondAsync(embed: BaseEmbed(id.user).WithColor(Color.Red).WithTitle("Not In Pathway").WithDescription(GetPathway(param).name).Build(), ephemeral: true);
                    WriteDebug("Responded");
                    return;
                }
                await Program.client.Rest.RemoveRoleAsync(id.guild, id.user, GetPathway(param).id);
                WriteDebug("Removed Role");
                if (InCatalyst(id) && GetRoleIds(id).Contains(studentId)) {
                    await Program.client.Rest.RemoveRoleAsync(id.guild, id.user, studentId);
                    WriteDebug("Removed Attending");
                }
                await command.RespondAsync(embed: BaseEmbed(id.user).WithColor(Color.Green).WithTitle("Left Pathway").WithDescription(GetPathway(param).name).Build(), ephemeral: true);
                WriteDebug("Responded");
                break;
        }
    }

    private (ulong id, string name) GetPathway(Dictionary<string, object> param) {
        return roleDict[(long)param["pathway"]];
    }
}