using System.ComponentModel.DataAnnotations;
using CaptchaMVC.Properties;

namespace CaptchaMVC.Models
{
    /// <summary>
    /// Model to describe the data Captcha.
    /// </summary>
    public class CaptchaModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "CaptchaRequired")]
        [DataType(DataType.Password)]
        public string CaptchaInputText { get; set; }
        public string CaptchaDeText { get; set; }
    }

    /// <summary>
    /// Model to describe the data for encryption.
    /// </summary>
    internal class EncryptorModel
    {
        public string Password { get; set; }

        public byte[] Salt { get; set; }
    }

    internal class RefreshModel
    {
        public string Image { get; set; }

        public string Code { get; set; }
    }
}
