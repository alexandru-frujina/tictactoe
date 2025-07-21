using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;


public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }

    // Called when a player invites another
    public async Task SendGameInvite(string targetUserId, string fromUserName, string gameId)
    {
        _logger.LogInformation($"Send game invite request {fromUserName} -> {targetUserId}...");
        try
        {
            await Clients.User(targetUserId)
                .SendAsync("ReceiveGameInvite", fromUserName, gameId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SendGameInvite error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw; // re-throw so SignalR sends error back to client
        }
    }
}