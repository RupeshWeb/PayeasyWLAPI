using AttributeRouting.Web.Http;
using BusinessEntities;
using BusinessServices;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthenticateController : ApiController
    {
        #region Private variable.
        private readonly ITokenServices _tokenServices;
        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public AuthenticateController(ITokenServices tokenServices)
        {
            _tokenServices = tokenServices;
        }

        #endregion

        /// <summary>
        /// Authenticates user and returns token with expiry.
        /// </summary>
        /// <returns></returns>
        [POST("authenticate")]
        public HttpResponseMessage Authenticate([FromBody] TokenRequestEntity model)
        {
            TokenResponseEntity _response = new TokenResponseEntity();
            try
            {
                if (string.IsNullOrEmpty(model.ServiceID))
                    model.ServiceID = "151";
                if (ModelState.IsValid)
                {
                    if (new[] { "151" }.Contains(model.ServiceID))
                    {
                        if (model.ServiceID == "151")
                        {
                            if (Regex.IsMatch(model.MerchantId, @"(\d*-)?\d{10}", RegexOptions.IgnoreCase))
                            {
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = "Merchant ID should be 10 digits.";
                                return Request.CreateResponse(HttpStatusCode.OK, _response);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(model.MerchantId))
                                model.MerchantId = "NA";
                        }
                        var result = _tokenServices.ValidateToken(model.Token, model.ServiceID);
                        if (result.StatusCode == clsVariables.APIStatus.Success)
                        {
                            return GetAuthToken(model);
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = result.Message;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = "Invalid service ID.";
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = string.Join(" |", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        /// <summary>
        /// Returns auth token for the validated user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private HttpResponseMessage GetAuthToken(TokenRequestEntity item)
        {
            TokenResponseEntity _response = new TokenResponseEntity();
            var result = _tokenServices.TokenGenerate(item);
            if (result.StatusCode == clsVariables.APIStatus.Success)
            {
                TokenResponseDataEntity _subResponse = new TokenResponseDataEntity()
                {
                    Token = result.Message
                };
                _response.StatusCode = clsVariables.APIStatus.Success;
                _response.Message = "Success";
                _response.Data = _subResponse;
            }
            else
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = result.Message;
            }
            var response = Request.CreateResponse(HttpStatusCode.OK, _response);
            return response;
        }

    }
}
