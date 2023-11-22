using System.ComponentModel.DataAnnotations;

namespace EndlessGame.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? History { get; set; }
        public long? Score { get; set; }
    }
}
