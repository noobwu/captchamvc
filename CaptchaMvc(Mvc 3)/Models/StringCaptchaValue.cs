using System;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the base model for storing ​string ​captcha values.
    /// </summary>
    public class StringCaptchaValue : ICaptchaValue
    {
        #region Fields

        private readonly string _value;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StringCaptchaValue"/> class.
        /// </summary>
        public StringCaptchaValue(string captchaText, string value)
        {
            if (captchaText == null) throw new ArgumentNullException("captchaText");
            if (value == null) throw new ArgumentNullException("value");
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
            return _value.Equals(inputText);
        }

        #endregion
    }
}