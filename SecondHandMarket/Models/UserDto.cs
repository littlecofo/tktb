namespace SecondHandMarket.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Nickname { get; set; }
        public string ProfilePhoto { get; set; }
        public string Bio { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string UserIP { get; set; }
        public int Experience { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
    }
}