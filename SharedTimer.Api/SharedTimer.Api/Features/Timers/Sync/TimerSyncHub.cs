using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using shared_timer_api.Features.Auth;

namespace shared_timer_api.Features.Timers.TimerSync;

[Authorize]
public sealed class TimerSyncHub : Hub
{
    private static readonly Dictionary<string, List<string>> _groupConnections = new();

    public async Task JoinGroup(string groupName)
    {
        Guid ownerId = Context.User.GetUserId();
        string groupKey = $"{groupName}:{ownerId}";
        
        await Groups.AddToGroupAsync(Context.ConnectionId, groupKey);
        
        if (!_groupConnections.ContainsKey(groupKey))
        {
            _groupConnections[groupKey] = new List<string>();
        }
        
        if (!_groupConnections[groupKey].Contains(Context.ConnectionId))
        {
            _groupConnections[groupKey].Add(Context.ConnectionId);
        }
        
        await Clients.Caller.SendAsync("JoinedGroup", groupName, ownerId);
    }
    
    public async Task LeaveGroup(string groupName)
    {
        Guid ownerId = Context.User.GetUserId();
        string groupKey = $"{groupName}:{ownerId}";
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupKey);
        
        if (_groupConnections.ContainsKey(groupKey))
        {
            _groupConnections[groupKey].Remove(Context.ConnectionId);
            
            if (_groupConnections[groupKey].Count == 0)
            {
                _groupConnections.Remove(groupKey);
            }
        }
    }
    
    public async Task SendTimerAction(TimerSyncMessage message)
    {
        Guid ownerId = Context.User.GetUserId();
        
        message.GroupOwnerId = ownerId;
        message.InitiatedByUserId = ownerId.ToString();
        
        string groupKey = $"{message.GroupName}:{ownerId}";
        await Clients.Group(groupKey).SendAsync("ReceiveTimerAction", message);
    }
    
    public async Task RequestCurrentState(string groupName)
    {
        Guid ownerId = Context.User.GetUserId();
        string groupKey = $"{groupName}:{ownerId}";
        
        await Clients.OthersInGroup(groupKey).SendAsync("StateRequested");
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var groupKey in _groupConnections.Keys.ToList())
        {
            if (_groupConnections[groupKey].Contains(Context.ConnectionId))
            {
                _groupConnections[groupKey].Remove(Context.ConnectionId);
                
                if (_groupConnections[groupKey].Count == 0)
                {
                    _groupConnections.Remove(groupKey);
                }
                
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupKey);
            }
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
