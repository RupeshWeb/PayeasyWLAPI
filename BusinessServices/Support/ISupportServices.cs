using BusinessEntities;

namespace BusinessServices
{
    public interface ISupportServices
    {
        UserValidateResponseEntity ValidateUser(string refName);
        MerchantActivationResponseEntity GetMerchantActivation(string mobileNo);
        APIReponseEntity AEPSActivation(int userID, MerchantActivationEntity item);

        string StateNameList();
        string CityNameList(string stateName);
        string FindStateByPinCode(string pinCode);
    }
}
