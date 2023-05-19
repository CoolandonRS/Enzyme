using Discord;
using Discord.Rest;
using Discord.WebSocket;
using static Enzyme.Commands.CommandUtil;
using static Enzyme.Misc.Verbosity;

namespace Enzyme.Commands.Handlers; 

public class ReactRoleCommandHandler : ICommandHandler {
    public async Task Execute(SocketSlashCommand command, (ulong guild, ulong user) id, Dictionary<string, object> param) {
        if (!IsAuth(id, AuthLevel.Admin)) {
            WriteDebug("Unauthorized");
            await command.RespondAsync(embed: UnauthorizedEmbed(id.user), ephemeral: true);
            WriteDebug("responded");
            return;
        }
        switch ((int)param["action"]) {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
}