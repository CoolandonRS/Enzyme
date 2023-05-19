using Discord;
using Discord.WebSocket;
using static Enzyme.Misc.Verbosity;

namespace Enzyme.Commands.Handlers; 

public class PathwayCommandHandler : ICommandHandler {
    private static readonly ulong attendingId = 1109211805156380682;
    private static readonly Dictionary<long, (ulong id, string name)> roleDict = new() {
        {  1, (1109205041866350622, "Programming") },
        {  2, (1109205120958349332, "Cybersecurity") },
        {  3, (1109205186989269022, "Game Dev") },
        {  4, (1109205247974445117, "Web Dev") },
        {  5, (1109205274503418066, "Drones") },
        {  6, (1109205318254215178, "UAV") },
        {  7, (1109205355210211369, "Audio") },
        {  8, (1109205401200754698, "Graphic Design") },
        {  9, (1109205709834420274, "Video") },
        { 10, (1109205761629884486, "Construction") },
        { 11, (1109205799638671380, "Business") },
        { 12, (1109205851488669766, "Culinary") },
        { 13, (1109205891472949270, "Baking") },
        { 14, (1109205933025935383, "Medical Assisting") },
        { 15, (1109205993621045379, "Pharmacy") }
    };
    
    public async Task Execute(SocketSlashCommand command, (ulong guild, ulong user) id, Dictionary<string, object> param) {
        WriteDebug($"{id.user} in {id.guild} ran pathway with {param["action"]} {param["pathway"]}");
        var embed = new EmbedBuilder().WithAuthor(command.User.ToString(), command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl()).WithCurrentTimestamp();
        switch ((long)param["action"]) {
            case 1:
                WriteDebug("Adding Role");
                await Program.client.Rest.AddRoleAsync(id.guild, id.user, GetPathway(param).id);
                WriteDebug("Added Role");
                if (!GetRoleIds(id).Contains(attendingId)) {
                    await Program.client.Rest.AddRoleAsync(id.guild, id.user, attendingId);
                    WriteDebug("Added Attending");
                }
                await command.RespondAsync(embed: embed.WithColor(Color.Green).WithTitle("Joined Pathway").WithDescription(GetPathway(param).name).Build(), ephemeral: true);
                WriteDebug("Responded");
                break;
            case 2:
                WriteDebug("Removing role");
                if (!GetRoleIds(id).Contains(GetPathway(param).id)) {
                    WriteDebug("Not in pathway");
                    await command.RespondAsync(embed: embed.WithColor(Color.Red).WithTitle("Not In Pathway").WithDescription(GetPathway(param).name).Build(), ephemeral: true);
                    WriteDebug("Responded");
                    return;
                }
                await Program.client.Rest.RemoveRoleAsync(id.guild, id.user, GetPathway(param).id);
                WriteDebug("Removed Role");
                var ids = roleDict.Values.Select(v => v.id).ToArray();
                if (GetRoleIds(id).Any(rId => ids.Contains(rId)) && GetRoleIds(id).Contains(attendingId)) {
                    await Program.client.Rest.RemoveRoleAsync(id.guild, id.user, attendingId);
                    WriteDebug("Removed Attending");
                }
                await command.RespondAsync(embed: embed.WithColor(Color.Green).WithTitle("Left Pathway").WithDescription(GetPathway(param).name).Build(), ephemeral: true);
                WriteDebug("Responded");
                break;
        }
    }

    private SocketRole[] GetRoles((ulong guild, ulong user) id) {
        return Program.client.GetGuild(id.guild).GetUser(id.user).Roles.ToArray();
    }
    private ulong[] GetRoleIds((ulong guild, ulong user) id) {
        return Program.client.GetGuild(id.guild).GetUser(id.user).Roles.Select(r => r.Id).ToArray();
    }

    private (ulong id, string name) GetPathway(Dictionary<string, object> param) {
        return roleDict[(long)param["pathway"]];
    }
}