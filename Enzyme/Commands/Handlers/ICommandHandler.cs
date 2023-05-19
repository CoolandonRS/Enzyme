using Discord.WebSocket;

namespace Enzyme.Commands.Handlers; 

public interface ICommandHandler {
    public Task Execute(SocketSlashCommand command, (ulong guild, ulong user) id, Dictionary<string, object> param);
}