using BusinessEntities;
using BusinessServices.Resource;
using DataModel.UnitOfWork;
using System;
using System.Linq;

namespace BusinessServices
{
    public class TokenServices : ITokenServices
    {
        #region Private member variables.
        private readonly UnitOfWork _unitOfWork;
        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public TokenServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public member methods.

        /// <summary>
        /// Method to validate token
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public TokenValidateEntity ValidateToken(string tokenId, string serviceReqID)
        {
            TokenValidateEntity _response = new TokenValidateEntity();
            try
            {
                if (string.IsNullOrEmpty(serviceReqID))
                    serviceReqID = "151";
                string requestIP = System.Web.HttpContext.Current.Request.UserHostAddress;
                var token = _unitOfWork.UserTokenRepository.Get(t => t.SessionValue == tokenId);
                if (token != null)
                {
                    var user = _unitOfWork.UserRepository.Get(u => u.ID == token.UserID);
                    if (user != null && user.ID > 0)
                    {
                        if (user.Status)
                        {
                            if (new[] { token.IPAddress, token.IPAddressOne, token.IPAddressTwo }.Contains(requestIP) || (token.IPAddress == null && token.IPAddressOne == null && token.IPAddressTwo == null))
                            {
                                short[] serviceID = { 10 };// { 10, 14, 2, 5, 6, 8, 11, 13 };
                                var service = _unitOfWork.ServiceRepository.Get(x => serviceID.Contains(x.ID) && x.IsActive);
                                if (service != null)
                                {
                                    var serviceAuth = _unitOfWork.ServiceAuthorizationRepository.Get(x => x.UserID == user.ID && x.ServiceID == service.ID && x.IsActive);
                                    if (serviceAuth != null)
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Success;
                                        _response.Message = Message.Su;
                                        _response.UserID = user.ID;
                                    }
                                    else
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = Message.ServiceDown;
                                    }
                                }
                                else
                                {
                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                    _response.Message = Message.ServiceDown;
                                }
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = Message.InvalidIP;
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = Message.Youracsuspend;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = Message.InvalidUser;
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.InavlidToken;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Method to get token owner against expiry and existence in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public int GetTokenOwner(string tokenId)
        {
            var token = _unitOfWork.UserTokenRepository.Get(t => t.SessionValue == tokenId);
            if (token != null)
            {
                return token.UserID;
            }
            return 0;
        }

        public TokenGenerateResponseEntity TokenGenerate(TokenRequestEntity item)
        {
            TokenGenerateResponseEntity _response = new TokenGenerateResponseEntity();
            try
            {
                if (string.IsNullOrEmpty(item.ServiceID))
                    item.ServiceID = "151";
                string request = item.Token + "|" + item.RetailerID + "|" + item.MerchantId + "|" + DateTime.Now + "|" + item.LogoUrl + "|" + item.Copyright + "|" + item.FirmName + "|" + item.ServiceID;
                string token = ClsMethods.GenerateHash(request);
                if (!string.IsNullOrEmpty(token))
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = token;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Errorsm;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public bool TokenValidate(string tokenID)
        {
            try
            {
                string token = ClsMethods.DecryptionHash(tokenID);
                if (!string.IsNullOrEmpty(token))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public TokenRequestResponseEntity ValidateRequest(string tokenID)
        {
            TokenRequestResponseEntity _response = new TokenRequestResponseEntity();
            try
            {
                string tokenResult = ClsMethods.DecryptionHash(tokenID);
                if (!string.IsNullOrEmpty(tokenResult))
                {
                    string[] line = tokenResult.Split('|');
                    if (DateTime.Now.Date == Convert.ToDateTime(line[3]).Date)
                    {
                        string token = line[0];
                        string merchantId = line[2];
                        string serviceId = line[7];
                        var tokens = _unitOfWork.UserTokenRepository.Get(x => x.SessionValue == token);
                        if (tokens != null && tokens.UserID > 0)
                        {
                            if (serviceId == "151")
                            {
                                #region AEPS Service Method
                                _response.Logourl = line[4];
                                _response.Copyright = line[5];
                                _response.Value = tokenResult;
                                _response.FirmName = line[6];
                                _response.ServiceId = line[7];
                                var transaction = _unitOfWork.MerchantActivationRepository.Get(x => x.MobileNo == merchantId);
                                if (transaction != null)
                                {
                                    if (transaction.Status == clsVariables.MerchantStatus.APPROVED)
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Success;
                                        _response.Message = Message.Su;
                                        _response.Status = clsVariables.APIStatus.Success;
                                    }
                                    else if (new[] { clsVariables.MerchantStatus.PENDING, clsVariables.MerchantStatus.REVISIONPENDING }.Contains(transaction.Status))
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = "Invalid merchant Id.";
                                        _response.Status = 3;
                                    }
                                    else if (transaction.Status == clsVariables.MerchantStatus.INPROCESS)
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = "Invalid merchant Id.";
                                        _response.Status = 4;
                                    }
                                    else
                                    {
                                        _response.StatusCode = clsVariables.APIStatus.Failed;
                                        _response.Message = "Invalid merchant Id.";
                                        _response.Status = 5;
                                    }
                                }
                                else
                                {
                                    _response.StatusCode = clsVariables.APIStatus.Failed;
                                    _response.Message = "Invalid merchant Id.";
                                    _response.Status = 2;
                                }
                                #endregion
                            }
                            else if (new[] { "152", "153" }.Contains(serviceId))
                            {
                                #region BBPS AND PAN Method
                                _response.StatusCode = clsVariables.APIStatus.Success;
                                _response.Message = Message.Su;
                                _response.Logourl = line[4];
                                _response.Copyright = line[5];
                                _response.Value = tokenResult;
                                _response.FirmName = line[6];
                                _response.ServiceId = line[7];
                                _response.Status = clsVariables.APIStatus.Success;
                                #endregion
                            }
                            else
                            {
                                _response.StatusCode = clsVariables.APIStatus.Failed;
                                _response.Message = Message.InavlidToken;
                                _response.Status = clsVariables.APIStatus.Failed;
                            }
                        }
                        else
                        {
                            _response.StatusCode = clsVariables.APIStatus.Failed;
                            _response.Message = Message.InavlidToken;
                            _response.Status = clsVariables.APIStatus.Failed;
                        }
                    }
                    else
                    {
                        _response.StatusCode = clsVariables.APIStatus.Failed;
                        _response.Message = Message.TokenExpired;
                        _response.Status = clsVariables.APIStatus.Failed;
                    }
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.InavlidToken;
                    _response.Status = clsVariables.APIStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
                _response.Status = clsVariables.APIStatus.Exception;
            }
            return _response;
        }

        #endregion
    }
}
