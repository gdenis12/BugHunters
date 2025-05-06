namespace WebApplication1.DTO
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime BirthDay { get; set; } // тільки для студентів

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
    }
}

