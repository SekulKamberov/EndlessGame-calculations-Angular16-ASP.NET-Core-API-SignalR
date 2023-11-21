namespace EndlessGame.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.SignalR;

  using EndlessGame.Entities;
  using EndlessGame.HubConfig; 

  [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
      private readonly IHubContext<ChatHub> _hub;
      private readonly EndlessGameDBContext context;
  
      public ChatController(IHubContext<ChatHub> hub, EndlessGameDBContext _context)
      {
        _hub = hub;
        context = _context; 
      }

      [HttpGet]
      public IActionResult Get()
      {
        var maxScore = context.Users.Max(s => (int?)s.Score);
        var champ = context.Users.FirstOrDefault(s => s.Score == maxScore);

         _hub.Clients.All.SendAsync("score", champ);
        return Ok(new { Message = "Request Completed" });
      }
     
      [HttpGet("user/{username}/{score}/{gameHistory}")]
      public IActionResult User(string username, int score, string gameHistory)
      {
        var users = context.Users.AsQueryable();
        var maxScore = users.Max(s => (int?)s.Score);
        var champion = context.Users.FirstOrDefault(s => s.Score == maxScore);
        var user = users.FirstOrDefault(u => u.Username == username);

        var history = gameHistory != "|" ? gameHistory : "";
        if (score > maxScore)
        {
          var champ = new User { Username = username, Score = score, History = history };
          _hub.Clients.All.SendAsync("score", champ);
        }

        if (champion != null)
        {
          _hub.Clients.All.SendAsync("score", champion);
        }

        if (user != null && user.Score >= score)
        {
          var currentUser = new User { Username = user.Username, Score = user.Score, History = user.History };
          return Ok(currentUser);
        }

        if (user != null && user.Score < score)
        {
          user.Score = score;
          user.History = user.History + history;
          var saved = context.SaveChanges();
          return Ok(score);
        }

        if(user == null)
        { 
          context.Add(new User { Score = score, Username = username, History = history });
          var saved = context.SaveChanges();
          return Ok(score);
        }
      return Ok();
      }

    }
  } 
