using SQLite;

namespace SecondHandMarket.Models
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; } = "temp_icon.jpg";
        public string OwnerPhoneNumber { get; set; } // 用于关联用户
        [Ignore]
        public string OwnerNickname { get; set; } // 用于展示商家用户的昵称
        public DateTime CreatedAt { get; set; } // 新增属性，用于记录商品的发布时间
        public int Weight { get; set; } // 权重值
    }
}