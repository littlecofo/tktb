using System.ComponentModel.DataAnnotations;

namespace SecondHandMarket.Requests
{
    public class UpdateUserProfileRequest
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 1, ErrorMessage = "昵称需为1到8个字之间")]
        public string Nickname { get; set; }

        [StringLength(20, ErrorMessage = "个人简介不能超过20个字")]
        public string Bio { get; set; }

        public string ProfilePhoto { get; set; }
    }
}