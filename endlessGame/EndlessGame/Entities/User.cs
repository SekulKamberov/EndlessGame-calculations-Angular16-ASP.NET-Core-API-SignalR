using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace EndlessGame.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? History { get; set; }
        public long Score { get; set; }
    }

  public class UserBindingModel
  {
    [Key] 
    public string? Username { get; set; }
    public string? History { get; set; }
    public string Score { get; set; }
  }

  public class UserViewModel
  {
    [Key]
    public string? Username { get; set; }
    public string? History { get; set; }
    public string Score { get; set; }
  }
}
