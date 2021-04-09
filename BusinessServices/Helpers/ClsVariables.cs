/// <summary>
/// Summary description for clsVariables
/// </summary>
public class clsVariables
{
    public struct RechargeMode
    {
        public static string API
        { get { return "API"; } }
        public static string GPRS
        { get { return "GPRS"; } }
    }

    public struct OperatorType
    {
        public static string P2P
        { get { return "P2P"; } }
        public static string P2A
        { get { return "P2A"; } }
    }

    public struct AllowTransactionType
    {
        public static string ALL
        { get { return "ALL"; } }
        public static string IMPS
        { get { return "IMPS"; } }
        public static string NEFT
        { get { return "NEFT"; } }
        public static string UPI
        { get { return "UPI"; } }
        public static string RTGS
        { get { return "RTGS"; } }
    }

    public struct PaymentType
    {
        public static string InitialBalanceDep
        { get { return "Initial Balance Deposit"; } }
        public static string PaymentReq
        { get { return "Payment Request Deposit"; } }
        public static string DmtComm
        { get { return "Money Transfer Comm."; } }
        public static string Dmt
        { get { return "Money Transfer"; } }
        public static string DmtR
        { get { return "Money Transfer - Reversal"; } }
        public static string DmtRevert
        { get { return "Money Transfer Commission - Reversal"; } }
        public static string Credit
        { get { return "Credit"; } }
        public static string Debit
        { get { return "Debit"; } }
        public static string AEPSTransaction
        { get { return "AEPS Transaction"; } }
        public static string AEPSMainWallet
        { get { return "AEPS To Main Wallet Transfer"; } }
        public static string POSTransaction
        { get { return "POS Transaction"; } }

        public static string Recharge
        { get { return "Recharge"; } }
        public static string RechargeComm
        { get { return "Recharge Comm."; } }
        public static string RechargeChar
        { get { return "Recharge Charg."; } }
        public static string RechargeR
        { get { return "Recharge - Reversal"; } }
        public static string RechargeCommRevert
        { get { return "Recharge Commission - Reversal"; } }
        public static string RechargeCharRevert
        { get { return "Recharge Surcharge - Reversal"; } }

        public static string Transaction
        { get { return "Transaction"; } }
        public static string TransactionComm
        { get { return "Transaction Comm."; } }
        public static string TransactionChar
        { get { return "Transaction Charg."; } }
        public static string TransactionR
        { get { return "Transaction - Reversal"; } }
        public static string TransactionCommRevert
        { get { return "Transaction Commission - Reversal"; } }
        public static string TransactionCharRevert
        { get { return "Transaction Surcharge - Reversal"; } }

        public static string BBPS
        { get { return "BBPS Transaction"; } }
        public static string BBPSR
        { get { return "BBPS Transaction - Reversal"; } }

        public static string PAN
        { get { return "PAN Transaction"; } }
        public static string PANR
        { get { return "PAN Transaction - Reversal"; } }
    }

    public struct RequestType
    {
        public static string Pending
        { get { return "Pending"; } }
        public static string Approved
        { get { return "Approved"; } }
        public static string Rejected
        { get { return "Rejected"; } }
    }

    public struct RecipientStatus
    {
        public static string Pending
        { get { return "Pending"; } }
        public static string Active
        { get { return "Active"; } }
    }

    public struct RechargeAPI
    {
        public static string PayTmPayout
        { get { return "PayTmPayout"; } }
        public static string PayTmBBPS
        { get { return "PayTmBBPS"; } }
        public static string PayTmDMT
        { get { return "PayTmDMT"; } }
        public static string CashOnCash
        { get { return "CashOnCash"; } }
        public static string EzyPay
        { get { return "EzyPay"; } }
        public static string Ambika
        { get { return "Ambika"; } }
        public static string KumarNew
        { get { return "KumarNew"; } }
        public static string ClickShyam
        { get { return "ClickShyam"; } }
        public static string Aadhar
        { get { return "Aadhar"; } }
        public static string MoneyOnMoney
        { get { return "MoneyOnMoney"; } }
        public static string Fino
        { get { return "Fino"; } }
        public static string Offline
        { get { return "Offline"; } }
        public static string MobileWare
        { get { return "MobileWare"; } }
        public static string UTIPan
        { get { return "UTIPan"; } }
        public static string STSMoney
        { get { return "STSMoney"; } }
        public static string Cyberplet
        { get { return "Cyberplet"; } }
        public static string CashOnCashNew
        { get { return "CashOnCashNew"; } }
        public static string TejasRecharge
        { get { return "TejasRecharge"; } }
        public static string MRobotics
        { get { return "MRobotics"; } }
        public static string Unique
        { get { return "Unique"; } }
        public static string EFast
        { get { return "EFast"; } }
        public static string VishwaMani
        { get { return "VishwaMani"; } }
        public static string ZipPay
        { get { return "ZipPay"; } }
        public static string Airtel
        { get { return "Airtel"; } }
        public static string Sonakshi
        { get { return "Sonakshi"; } }
        public static string Ezetap
        { get { return "Ezetap"; } }
        public static string QuickRech
        { get { return "QuickRech"; } }
        public static string MoneyArt
        { get { return "MoneyArt"; } }
    }

    public struct SmsAPI
    {
        public static string Bulk
        { get { return "Bulk"; } }
        public static string MobiComm
        { get { return "MobiComm"; } }
        public static string SmsVille
        { get { return "SmsVille"; } }
        public static string SmsCountry
        { get { return "SmsCountry"; } }
    }

    public struct SmsType
    {
        public static string RecipientVerify
        { get { return "RecipientVerify"; } }

        public static string RecipientRemove
        { get { return "RecipientRemove"; } }

        public static string AccountValidate
        { get { return "AccountValidate"; } }

        public static string MoneyTransfer
        { get { return "MoneyTransfer"; } }

        public static string UserLoginOTP
        { get { return "UserLoginOTP"; } }

        public static string UserRegistration
        { get { return "UserRegistration"; } }

        public static string ChangePassword
        { get { return "ChangePassword"; } }

        public static string BillPayment
        { get { return "Bill Payment"; } }

        public static string ComplaintRegistration
        { get { return "Complaint Registration"; } }
    }

    public struct ServiceType
    {
        public static string Money_Transfer
        { get { return "Money Transfer"; } }
        public static string BBPS
        { get { return "BBPS"; } }
        public static string AEPS
        { get { return "AEPS"; } }
        public static string Mobile
        { get { return "Mobile"; } }
        public static string DTH
        { get { return "DTH"; } }
        public static string DataCard
        { get { return "Data Card"; } }
        public static string Postpaid
        { get { return "Postpaid"; } }
        public static string Insurance
        { get { return "Insurance"; } }
        public static string Electricity
        { get { return "Electricity"; } }
        public static string Gas
        { get { return "Gas"; } }
        public static string Water
        { get { return "Water"; } }
        public static string PanCard
        { get { return "PanCard"; } }
        public static string MPOS
        { get { return "MPOS"; } }
    }

    public struct UserType
    {
        public static string API
        { get { return "API"; } }
        public static string Administrator
        { get { return "Administrator"; } }
    }

    public struct RechargeStatus
    {
        public static string Pending
        { get { return "Pending"; } }
        public static string Success
        { get { return "Success"; } }
        public static string Failure
        { get { return "Failure"; } }
        public static string Reversal
        { get { return "Reversal"; } }
    }

    public struct CreditDebitType
    {
        public static string Credit
        { get { return "Credit"; } }
        public static string Debit
        { get { return "Debit"; } }
    }

    public struct APIStatus
    {
        public static int Pending
        { get { return 2; } }
        public static int Success
        { get { return 1; } }
        public static int Failed
        { get { return 0; } }
        public static int Exception
        { get { return 10; } }
    }

    public struct GIFTCardStatus
    {
        public static string Accepted
        { get { return "Accepted"; } }
        public static string Activated
        { get { return "Activated"; } }
        public static string Rejected
        { get { return "Rejected"; } }
        public static string Activation
        { get { return "Activation"; } }
    }

    public struct MerchantStatus
    {
        public static string PENDING
        { get { return "PENDING"; } }
        public static string INPROCESS
        { get { return "IN PROCESS"; } }
        public static string APPROVED
        { get { return "APPROVED"; } }
        public static string REVISION
        { get { return "REVISION"; } }
        public static string REVISIONPENDING
        { get { return "REVISION PENDING"; } }
    }
    public struct TicketStatus
    {
        public static string Pending
        { get { return "Pending"; } }
        public static string Closed
        { get { return "Closed"; } }
    }
}