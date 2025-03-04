using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondHandMarket.Requests
{
    //发送注册请求时包含的信息
    public class RegisterRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public string Code { get; set; }
    }
}
