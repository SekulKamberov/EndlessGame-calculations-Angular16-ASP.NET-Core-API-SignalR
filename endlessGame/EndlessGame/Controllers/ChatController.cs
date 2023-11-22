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
     
      [HttpGet("user/{username?}/{score?}/{gameHistory}")]
      public IActionResult User(string username, int score, string gameHistory)
      {
        var users = context.Users.AsQueryable();
        var maxScore = users.Max(s => (long?)s.Score);
        var champion = context.Users.FirstOrDefault(s => s.Score == maxScore);
        var user = users.FirstOrDefault(u => u.Username == username);

        var history = gameHistory != "|" ? gameHistory : "";
        if (score > maxScore)
        {
          var champ = new User { Username = username, Score = score, History = history };
          _hub.Clients.All.SendAsync("score", champ);
        }

        else //(champion != null)
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

        else
        { 
          context.Add(new User { Score = score, Username = username, History = history });
          var saved = context.SaveChanges();
          return Ok(score);
        } 
      }

    [HttpPost("user/")]
    public IActionResult User([FromBody] User dat)
    {
      var users = context.Users.AsQueryable();
      var maxScore = users.Max(s => (long?)s.Score);
      var champion = context.Users.FirstOrDefault(s => s.Score == maxScore);
      var user = users.FirstOrDefault(u => u.Username == dat.Username);

      var history = dat.History != "|" ? dat.History : "";
      if (dat.Score > maxScore)
      {
        var champ = new User { Username = dat.Username, Score = dat.Score, History = history };
        _hub.Clients.All.SendAsync("score", champ);
      }

      else 
      {
        _hub.Clients.All.SendAsync("score", champion);
      }

      if (user != null && user.Score >= dat.Score)
      {
        var currentUser = new User { Username = user.Username, Score = user.Score, History = user.History };
        return Ok(currentUser);
      }

      if (user != null && user.Score < dat.Score)
      {
        user.Score = dat.Score;
        user.History = user.History + history;
        var saved = context.SaveChanges();
        return Ok(dat.Score);
      }

      else
      {
        context.Add(new User { Score = dat.Score, Username = dat.Username, History = history });
        var saved = context.SaveChanges();
        return Ok(dat.Score);
      }
    }

  }
  } 
