using Microsoft.AspNetCore.SignalR;

namespace EndlessGame.HubConfig
{
  public class ChatHub : Hub
  {
      public async Task broadcastdata(string data) =>
          await Clients.All.SendAsync("broadcastdata", 1000000);

  }
}
