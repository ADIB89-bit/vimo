using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static string? waitingUser;

    private static readonly object LockObject = new();

    private static int onlineUsers = 0;


    // =========================================
    // USER CONNECTED
    // =========================================

    public override async Task OnConnectedAsync()
    {
        lock (LockObject)
        {
            onlineUsers++;
        }

        await Clients.All.SendAsync(
            "OnlineUsersChanged",
            onlineUsers
        );

        await base.OnConnectedAsync();
    }


    // =========================================
    // JOIN RANDOM CHAT
    // =========================================

    public async Task JoinChat()
    {
        string? otherUser = null;

        lock (LockObject)
        {
            if (waitingUser == null)
            {
                waitingUser = Context.ConnectionId;
            }
            else if (waitingUser != Context.ConnectionId)
            {
                otherUser = waitingUser;
                waitingUser = null;
            }
        }


        // Nobody waiting
        if (otherUser == null)
        {
            await Clients.Caller.SendAsync(
                "WaitingForStranger"
            );

            return;
        }


        // Tell first user
        await Clients.Client(otherUser).SendAsync(
            "StrangerFound",
            Context.ConnectionId
        );


        // Tell second user
        await Clients.Caller.SendAsync(
            "StrangerFound",
            otherUser
        );
    }


    // =========================================
    // WEBRTC OFFER
    // =========================================

    public async Task SendOffer(
        string targetUserId,
        string offer)
    {
        await Clients.Client(targetUserId)
            .SendAsync(
                "ReceiveOffer",
                Context.ConnectionId,
                offer
            );
    }


    // =========================================
    // WEBRTC ANSWER
    // =========================================

    public async Task SendAnswer(
        string targetUserId,
        string answer)
    {
        await Clients.Client(targetUserId)
            .SendAsync(
                "ReceiveAnswer",
                Context.ConnectionId,
                answer
            );
    }


    // =========================================
    // ICE CANDIDATE
    // =========================================

    public async Task SendIceCandidate(
        string targetUserId,
        string candidate)
    {
        await Clients.Client(targetUserId)
            .SendAsync(
                "ReceiveIceCandidate",
                Context.ConnectionId,
                candidate
            );
    }


    // =========================================
    // USER DISCONNECTED
    // =========================================

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        lock (LockObject)
        {
            if (waitingUser == Context.ConnectionId)
            {
                waitingUser = null;
            }

            if (onlineUsers > 0)
            {
                onlineUsers--;
            }
        }


        await Clients.All.SendAsync(
            "OnlineUsersChanged",
            onlineUsers
        );


        await base.OnDisconnectedAsync(exception);
    }
}