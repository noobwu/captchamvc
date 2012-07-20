using System;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    /// Represents the base model with information for drawing a captcha.
    /// </summary>
    public class DefaultDrawingModel : IDrawingModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDrawingModel"/> class.
        /// </summary>
        public DefaultDrawingModel(string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            Text = text;
        }

        #endregion

        #region Implementation of IDrawingModel

        /// <summary>
        /// Gets the specified text for render.
        /// </summary>
        public string Text { get; private set; }

        #endregion
    }
}