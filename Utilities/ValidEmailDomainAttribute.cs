using System.ComponentModel.DataAnnotations;

namespace Asp.netCore_MVC.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        #region Properties

        private readonly string allowedDomain;
        #endregion

        #region Constructor

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }
        #endregion

        #region ValidateEmailDomain

        /// <summary>
        /// Validate Email Domain by user request domain
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            string[] data = value?.ToString()?.Split('@');
            return data?[1].ToUpper() == allowedDomain.ToUpper();
        }
        #endregion
    }
}
