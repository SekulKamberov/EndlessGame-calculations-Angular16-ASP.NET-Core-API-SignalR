namespace EndlessGame.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.SignalR;

  using EndlessGame.Entities;
  using EndlessGame.HubConfig;
  using System.Numerics;

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
      var maxScore = context.Users.Max(s => (long?)s.Score);
      var champ = context.Users.FirstOrDefault(s => s.Score == maxScore);

      _hub.Clients.All.SendAsync("score", champ);
      return Ok(new { Message = "Request Completed" });
    }

    [HttpGet("user/{username}/{score}/{gameHistory}")]
    public IActionResult User(string username, string? score, string gameHistory)
    {
      var longScore = long.Parse(score);
      var users = context.Users.AsQueryable();
      var maxScore = users.Max(s => (long?)s.Score);
      var champion = context.Users.FirstOrDefault(s => s.Score == maxScore);
      var user = users.FirstOrDefault(u => u.Username == username);

      var history = gameHistory != "|" ? gameHistory : "";
      if (longScore > maxScore)
      {
        var champ = new User { Username = username, Score = longScore, History = history };
        _hub.Clients.All.SendAsync("score", champ);
      }

      else //(champion != null)
      {
        _hub.Clients.All.SendAsync("score", champion);
      }

      if (user != null && user.Score >= longScore)
      {
        var currentUser = new User { Username = user.Username, Score = user.Score, History = user.History };
        return Ok(currentUser);
      }

      if (user != null && user.Score < longScore)
      {
        user.Score = longScore;
        user.History = user.History + history;
        var saved = context.SaveChanges();
        return Ok(score);
      }

      else
      {
        context.Add(new User { Score = longScore, Username = username, History = history });
        var saved = context.SaveChanges();
        return Ok(score);
      }
    }

    [HttpPost("user/")]
    public IActionResult User([FromBody] UserBindingModel dat)
    {
      var longScore = long.Parse(dat.Score);
      var users = context.Users.AsQueryable();
      var maxScore = users.Max(s => (long?)s.Score);
      var champion = context.Users.FirstOrDefault(s => s.Score == maxScore);
      var user = users.FirstOrDefault(u => u.Username == dat.Username);

      var history = dat.History != "|" ? dat.History : "";
      if (longScore > maxScore)
      {
        var champ = new User { Username = dat.Username, Score = longScore, History = history };
        _hub.Clients.All.SendAsync("score", champ);
      }

      else
      {
        _hub.Clients.All.SendAsync("score", champion);
      }

      if (user != null && user.Score >= longScore)
      {
        var currentUser = new User { Username = user.Username, Score = user.Score, History = user.History };
        return Ok(currentUser);
      }

      if (user != null && user.Score < longScore)
      {
        user.Score = longScore;
        user.History = user.History + history;
        var saved = context.SaveChanges();
        return Ok(dat.Score);
      }

      else
      {
        context.Add(new User { Score = longScore, Username = dat.Username, History = history });
        var saved = context.SaveChanges();
        return Ok(dat.Score);
      }
    }

  }
}
