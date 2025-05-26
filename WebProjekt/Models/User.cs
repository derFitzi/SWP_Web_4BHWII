namespace WebProjekt.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Passwort { get; set; }
        public DateTime? Birthdate { get; set; }
        public List<Order>? Orders { get; set; }
        public string? Userrole {get; set; }

    }
}
