using SQLite;

namespace SecondHandMarket.Models
{
    public class Post
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public string AuthorPhoneNumber { get; set; } // 用于关联用户
        [Ignore]
        public string AuthorNickname { get; set; } // 用于展示作者的昵称
        public DateTime CreatedAt { get; set; } // 新增属性，用于记录帖子的发布时间

        // 新增属性
        public bool IsPinned { get; set; } // 是否为置顶
        public bool IsFeatured { get; set; } // 是否为精华
        public bool IsClosed { get; set; } // 是否被关闭
        public int Weight { get; set; } // 权重值
    }
}