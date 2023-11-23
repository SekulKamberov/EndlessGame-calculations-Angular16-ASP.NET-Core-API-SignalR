namespace EndlessGame.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.SignalR;

  using EndlessGame.Entities;
  using EndlessGame.HubConfig;
  using System.Numerics;
  using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
      //var maxScore = context.Users.Max(s => (long?)s.Score);
      var maxScore = context.Users.Select(s => BigInteger.Parse(s.Score.ToString())).ToList().Max();
      var champ = context.Users.FirstOrDefault(s => BigInteger.Parse(s.Score.ToString()) == maxScore);

      _hub.Clients.All.SendAsync("score", champ);
      return Ok(new { Message = "Request Completed" });
    }

    [HttpGet("user/{username}/{score}/{gameHistory}")]
    public IActionResult User(string username, string? score, string gameHistory)
    {
      //var a = (BigInteger.Parse("394635443963561656323946354439635616563239463544396356165632") * BigInteger.Parse("394635443963561656323946354439635616563239463544396356165632")).ToString();
      //BigInteger number1;
      //bool succeeded1 = BigInteger.TryParse("12347534159895123", out number1);
      var longScore = BigInteger.Parse(score);

      var users = context.Users.AsQueryable();
      //var maxScore = users.Max(s => (long?)s.Score);
      var maxScore2 = users.Select(s => BigInteger.Parse(s.Score.ToString())).ToList().Max();

      //var maxScore = users.Max(s => (long?)s.Score);
      //var maxScore = users.Select(s => {
      //    var current = BigInteger.Parse(s.Score.ToString());

      //}); 
      var champion = context.Users.FirstOrDefault(s => s.Score == maxScore2.ToString());

      var user2 = users.FirstOrDefault(u => u.Username == username);
      var p = BigInteger.Parse(user2.Score.ToString());
      var p2 = BigInteger.Parse(champion.Score.ToString());

      var champion2 = new UserViewModel() { Username = champion.Username, History = champion.History, Score = p2.ToString() };
      var user = new UserViewModel() { Username = user2.Username, History = user2.History, Score = p.ToString() };
      var history = gameHistory != "|" ? gameHistory : "";
      if (longScore > maxScore2)
      {
        var champ = new UserViewModel { Username = username, Score = longScore.ToString(), History = history };
        _hub.Clients.All.SendAsync("score", champ);
      }

      else
      {
        _hub.Clients.All.SendAsync("score", champion);
      }

      if (user != null && BigInteger.Parse(user2.Score.ToString()) >= longScore)
      {
        var currentUser = new UserViewModel { Username = user.Username, Score = user.Score, History = user.History };
        return Ok(currentUser);
      }

      if (user != null && BigInteger.Parse(user2.Score.ToString()) < longScore)
      {
        user.Score = longScore.ToString();
        user.History = user.History + history;
        var saved = context.SaveChanges();
        return Ok(score);
      }

      else
      {
        context.Add(new UserViewModel { Score = longScore.ToString(), Username = username, History = history });
        var saved = context.SaveChanges();
        return Ok(score);
      }
    }

    [HttpPost("user/")]
    public IActionResult User([FromBody] UserBindingModel dat)
    { 
      var longScore = BigInteger.Parse(dat.Score.ToString());
      
      var users = context.Users.AsQueryable(); 
      var maxScore2 = users.Select(s => BigInteger.Parse(s.Score.ToString())).ToList().Max();
      
      var champion = context.Users.FirstOrDefault(s => s.Score == maxScore2.ToString());

      var user2 = users.FirstOrDefault(u => u.Username == dat.Username);
      var p = BigInteger.Parse(user2.Score.ToString());
      var p2 = BigInteger.Parse(champion.Score.ToString());

      var champion2 = new UserViewModel() { Username = champion.Username, History = champion.History, Score = p2.ToString() };
      var user = new User() { Username = user2.Username, History = user2.History, Score = p.ToString() };

      var history = dat.History != "|" ? dat.History : "";
      if (longScore > maxScore2)
      {
        var champ = new UserViewModel { Username = dat.Username, Score = longScore.ToString(), History = history };
        _hub.Clients.All.SendAsync("score", champ);
      }

      else
      {
        _hub.Clients.All.SendAsync("score", champion);
      }

      if (user != null && BigInteger.Parse(user2.Score.ToString()) >= longScore)
      {
        var currentUser = new UserViewModel { Username = user.Username, Score = user.Score, History = user.History };
        return Ok(currentUser);
      }

      if (user != null && BigInteger.Parse(user2.Score.ToString()) < longScore)
      {
        user2.Score = longScore.ToString();
        user2.History = user.History + history;
        context.SaveChanges();
        return Ok(dat.Score);
      }

      else
      {
        context.Add(new User { Score = longScore.ToString(), Username = dat.Username, History = history });
         context.SaveChanges();
        return Ok(dat.Score);
      }
    }

  }
}
