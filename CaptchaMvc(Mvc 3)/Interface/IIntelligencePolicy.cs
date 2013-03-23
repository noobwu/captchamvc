using System.Web.Mvc;

namespace CaptchaMvc.Interface
{
    /// <summary>
    ///     Represents the policy which makes a captcha intelligence.
    /// </summary>
    public interface IIntelligencePolicy
    {
        /// <summary>
        ///     Determines whether the intelligence captcha is valid.
        /// </summary>
        /// <param name="controller">
        ///     The specified <see cref="ControllerBase" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     <c>True</c> if the intelligence captcha is valid; <c>false</c> not valid; <c>null</c> is not intelligence captcha.
        /// </returns>
        bool? IsValid(ControllerBase controller, IParameterContainer parameterContainer);

        /// <summary>
        ///     Makes the specified captcha "intelligent".
        /// </summary>
        /// <param name="captcha">
        ///     The specified <see cref="ICaptcha" />.
        /// </param>
        /// <param name="parameterContainer">
        ///     The specified <see cref="IParameterContainer" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="ICaptcha" />.
        /// </returns>
        ICaptcha MakeIntelligent(ICaptcha captcha, IParameterContainer parameterContainer);
    }
}