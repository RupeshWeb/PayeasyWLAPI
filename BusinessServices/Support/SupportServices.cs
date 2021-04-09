using AutoMapper;
using BusinessEntities;
using BusinessServices.Resource;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Configuration;

namespace BusinessServices
{
    public class SupportServices : ISupportServices
    {
        #region Private member variables.
        private readonly UnitOfWork _unitOfWork;
        private readonly string FinoClientId = ConfigurationManager.AppSettings["V1Fino_Client_ID_AEPS"];
        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public SupportServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        public UserValidateResponseEntity ValidateUser(string refName)
        {
            UserValidateResponseEntity _response = new UserValidateResponseEntity();
            try
            {
                string[] splitData = refName.Split('|');
                string token = splitData[0];
                _response.AgentID = splitData[1];
                _response.MerchantID = splitData[2];
                var user = _unitOfWork.UserTokenRepository.Get(x => x.SessionValue == token);
                if (user != null)
                    _response.UserID = user.UserID;
                else
                    _response.UserID = 0;
            }
            catch (Exception) { _response.UserID = 0; }
            return _response;
        }

        /// <summary>
        /// return aeps statement of user by date
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public MerchantActivationResponseEntity GetMerchantActivation(string mobileNo)
        {
            MerchantActivationResponseEntity _response = new MerchantActivationResponseEntity();
            try
            {
                var transaction = _unitOfWork.MerchantActivationRepository.Get(x => x.MobileNo == mobileNo);
                if (transaction != null)
                {
                    Mapper.CreateMap<MerchantActivation, MerchantActivationListEntity>();
                    var statementModel = Mapper.Map<MerchantActivation, MerchantActivationListEntity>(transaction);
                    string[] line = transaction.MerchantName.Split(' ');
                    if (line.Length > 2)
                    {
                        statementModel.FirstName = line[0].Trim();
                        statementModel.MiddleName = line[1].Trim();
                        statementModel.LastName = line[2].Trim();
                    }
                    else if (line.Length == 2)
                    {
                        statementModel.FirstName = line[0].Trim();
                        statementModel.LastName = line[1].Trim();
                    }
                    statementModel.AadharNo = transaction.Extra1;
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.Su;
                    _response.Data = statementModel;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Datanotfound;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        /// <summary>
        /// merchant activation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public APIReponseEntity AEPSActivation(int userID, MerchantActivationEntity item)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                var transaction = _unitOfWork.MerchantActivationRepository.Get(x => x.MobileNo == item.MobileNo);
                if (transaction == null)
                {
                    MerchantActivation _add = new MerchantActivation()
                    {
                        UserID = userID,
                        ClientID = FinoClientId,
                        MobileNo = item.MobileNo,
                        MerchantName = item.FirstName + " " + item.MiddleName + " " + item.LastName,
                        PanNumber = item.PanNumber,
                        PinCode = Convert.ToInt32(item.PinCode),
                        StateName = item.StateName,
                        District = item.District,
                        Document1 = item.Document1,
                        Document2 = item.Document2,
                        Document3 = item.Document3,
                        AddDate = DateTime.Now,
                        EditDate = DateTime.Now,
                        Status = clsVariables.MerchantStatus.PENDING,
                        Extra1 = item.AadharNo
                    };
                    _unitOfWork.MerchantActivationRepository.Insert(_add);
                    _unitOfWork.Save();

                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.RequestAccpted;
                }
                else if (transaction.Status == clsVariables.MerchantStatus.REVISION && transaction.UserID == userID)
                {
                    transaction.MerchantName = item.FirstName + " " + item.MiddleName + " " + item.LastName;
                    transaction.PanNumber = item.PanNumber;
                    transaction.PinCode = Convert.ToInt32(item.PinCode);
                    transaction.StateName = item.StateName;
                    transaction.District = item.District;
                    transaction.Document1 = item.Document1;
                    transaction.Document2 = item.Document2;
                    transaction.Document3 = item.Document3;
                    transaction.EditDate = DateTime.Now;
                    transaction.Status = clsVariables.MerchantStatus.REVISIONPENDING;
                    _unitOfWork.MerchantActivationRepository.Update(transaction);
                    _unitOfWork.Save();

                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = Message.RequestAccpted;
                }
                else if (transaction.Status == clsVariables.MerchantStatus.PENDING && transaction.UserID == userID)
                {
                    _response.StatusCode = clsVariables.APIStatus.Pending;
                    _response.Message = Message.TxnPending;
                }
                else if (transaction.Status == clsVariables.MerchantStatus.INPROCESS && transaction.UserID == userID)
                {
                    _response.StatusCode = clsVariables.APIStatus.Pending;
                    _response.Message = clsVariables.MerchantStatus.INPROCESS;
                }
                else if (transaction.Status == clsVariables.MerchantStatus.APPROVED && transaction.UserID == userID)
                {
                    _response.StatusCode = clsVariables.APIStatus.Success;
                    _response.Message = clsVariables.MerchantStatus.APPROVED;
                }
                else if (transaction.Status == clsVariables.MerchantStatus.REVISIONPENDING && transaction.UserID == userID)
                {
                    _response.StatusCode = clsVariables.APIStatus.Pending;
                    _response.Message = clsVariables.MerchantStatus.REVISIONPENDING;
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = Message.Mobilenoexist;
                }
            }
            catch
            {
                _response.StatusCode = clsVariables.APIStatus.Failed;
                _response.Message = Message.Errorsm;
            }
            return _response;
        }

        public string StateNameList()
        {
            string response = ClsMethods.POSTAPIConsume("https://moneyart.in/api/ProfileServices/StatesName", "");
            return response;
        }

        public string CityNameList(string stateName)
        {
            string response = ClsMethods.POSTAPIConsume("https://moneyart.in/api/ProfileServices/CitiesName?state=" + stateName, "");
            return response;
        }

        public string FindStateByPinCode(string pinCode)
        {
            string response = ClsMethods.POSTAPIConsume("https://moneyart.in/api/ProfileServices/FindByPinCode?pinCode=" + pinCode, "");
            return response;
        }
    }
}
