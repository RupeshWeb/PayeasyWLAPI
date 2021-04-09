using BusinessEntities;

namespace BusinessServices
{
    public interface ITokenServices
    {
        #region Interface member methods.

        /// <summary>
        /// Function to validate token againt expiry and existance in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        TokenValidateEntity ValidateToken(string tokenId, string serviceReqID);

        /// <summary>
        /// Function to get token owner againt expiry and existance in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        int GetTokenOwner(string tokenId);

        TokenGenerateResponseEntity TokenGenerate(TokenRequestEntity item);

        bool TokenValidate(string tokenID);

        TokenRequestResponseEntity ValidateRequest(string tokenID);

        #endregion
    }
}
