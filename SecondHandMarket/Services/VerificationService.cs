using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondHandMarket.Services
{
    internal class VerificationService
    {
        // 保存手机号和验证码的映射关系
        private readonly Dictionary<string, string> _verificationCodes = new Dictionary<string, string>();
        
        // 生成并发送验证码
        public string GenerateAndSendVerificationCode(string phoneNumber)
        {
            // 生成随机验证码
            var random = new Random();
            var verificationCode = random.Next(1000, 9999).ToString();

            // 模拟发送验证码到用户手机
            Console.WriteLine($"发送验证码 {verificationCode} 到手机号 {phoneNumber}");

            // 保存验证码
            _verificationCodes[phoneNumber] = verificationCode;

            return verificationCode;
        }

        // 验证验证码
        public bool VerifyVerificationCode(string phoneNumber, string verificationCode)
        {
            // 验证验证码是否正确
            return _verificationCodes.ContainsKey(phoneNumber) && _verificationCodes[phoneNumber] == verificationCode;
        }
    }
}
