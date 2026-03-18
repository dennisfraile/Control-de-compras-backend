using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GroceryControl.API.Hubs;

[Authorize]
public class ShoppingListHub : Hub
{
    public async Task JoinList(string listId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, listId);
        await Clients.OthersInGroup(listId).SendAsync("UserJoined", Context.User?.Identity?.Name);
    }

    public async Task LeaveList(string listId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, listId);
        await Clients.OthersInGroup(listId).SendAsync("UserLeft", Context.User?.Identity?.Name);
    }

    public async Task AddItem(string listId, string productName, decimal quantity)
    {
        await Clients.OthersInGroup(listId).SendAsync("ItemAdded", new
        {
            ListId = listId,
            ProductName = productName,
            Quantity = quantity,
            AddedBy = Context.User?.Identity?.Name
        });
    }

    public async Task ToggleItem(string listId, string itemId, bool isChecked)
    {
        await Clients.OthersInGroup(listId).SendAsync("ItemToggled", new
        {
            ListId = listId,
            ItemId = itemId,
            IsChecked = isChecked,
            ToggledBy = Context.User?.Identity?.Name
        });
    }

    public async Task RemoveItem(string listId, string itemId)
    {
        await Clients.OthersInGroup(listId).SendAsync("ItemRemoved", new
        {
            ListId = listId,
            ItemId = itemId,
            RemovedBy = Context.User?.Identity?.Name
        });
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
