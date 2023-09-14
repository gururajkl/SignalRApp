using Microsoft.AspNetCore.SignalR;

namespace BlazorServer.Hubs;

/// <summary>
/// SignalRHub class.
/// </summary>
public class ChatHub : Hub
{
    /// <summary>
    /// SignalR method can be invoked by the client.
    /// </summary>
    /// <param name="user">Username input.</param>
    /// <param name="message">Message from the user.</param>
    /// <returns></returns>
    public Task SendMessage(string user, string message)
    {
        // Get all the clients and send the data to it.
        return Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
