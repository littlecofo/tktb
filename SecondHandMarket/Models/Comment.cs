using SQLite;
using System;

namespace SecondHandMarket.Models
{
    public class Comment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorPhoneNumber { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        [Ignore]
        public string AuthorNickname { get; set; }
        [Ignore]
        public string AuthorProfilePhoto { get; set; }
        [Ignore]
        public int FloorNumber { get; set; }
    }
}