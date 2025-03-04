using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace SecondHandMarket.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string UserIP { get; set; }
        public string Nickname { get; set; } = "太科学子";
        public string ProfilePhoto { get; set; } = "default_profile_photo.jpg";
        public string Bio { get; set; } = "未设置简介";

        // 新增属性
        public int Experience { get; set; } = 0;
        public string Title { get; set; } = "新手";
        public int Points { get; set; } = 0;

        // 计算等级属性
        [Ignore]
        public int Level
        {
            get
            {
                if (Experience < 0)
                    return -1;
                else if (Experience < 100)
                    return 0;
                else if (Experience < 200)
                    return 1;
                else if (Experience < 350)
                    return 2;
                else if (Experience < 500)
                    return 3;
                else if (Experience < 700)
                    return 4;
                else if (Experience < 1000)
                    return 5;
                else
                    return 6;
            }
        }
    }
}