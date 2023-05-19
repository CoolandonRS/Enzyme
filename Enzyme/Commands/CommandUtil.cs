using Discord;
using Discord.WebSocket;

namespace Enzyme.Commands; 

public static class CommandUtil {
    public static readonly ulong studentId = 1109211805156380682;
    public static readonly ulong outsiderId  = 1109224098636255318;
    public static readonly ulong teacherId = 1109223971091660831;
    public static readonly Dictionary<long, (ulong id, string name)> roleDict = new() {
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

    public static EmbedBuilder BaseEmbed(ulong userId) {
        var user = Program.client.GetUser(userId);
        return new EmbedBuilder().WithAuthor(user.ToString(), user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()).WithCurrentTimestamp();
    }

    public static SocketRole[] GetRoles((ulong guild, ulong user) id) {
        return Program.client.GetGuild(id.guild).GetUser(id.user).Roles.ToArray();
    }
    public static ulong[] GetRoleIds((ulong guild, ulong user) id) {
        return Program.client.GetGuild(id.guild).GetUser(id.user).Roles.Select(r => r.Id).ToArray();
    }
}