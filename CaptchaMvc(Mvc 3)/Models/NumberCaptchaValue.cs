using System;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the base model for storing number ​​captcha values.
    /// </summary>
    public class NumberCaptchaValue : ICaptchaValue
    {
        #region Fields

        private readonly int _value;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberCaptchaValue"/> class.
        /// </summary>
        public NumberCaptchaValue(string captchaText, int value)
        {
            if (captchaText == null) throw new ArgumentNullException("captchaText");
            CaptchaText = captchaText;
            _value = value;
        }

        #endregion

        #region Implementation of ICaptchaValue

        /// <summary>
        /// Gets the specified captcha text.
        /// </summary>
        public string CaptchaText { get; private set; }

        /// <summary>
        /// Gets the specified captcha value.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Determines whether the current captcha value is equal for the <see cref="inputText"/>.
        /// </summary>
        /// <param name="inputText">The specified input text.</param>
        /// <returns><c>True</c> if the value is equals; otherwise, <c>false</c>.</returns>
        public bool IsEqual(string inputText)
        {
            int input;
            if (!int.TryParse(inputText, out input))
                return false;
            return input == _value;
        }

        #endregion
    }
}