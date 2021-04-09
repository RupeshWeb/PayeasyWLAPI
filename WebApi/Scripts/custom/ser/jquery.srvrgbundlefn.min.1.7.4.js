BindData(); BindsData();
function Demo() {

    var GetPIStringstr = '';
    var GetPAStringstr = '';
    var GetPFAStringstr = '';

    if (GetPI() == true) {
        GetPIStringstr = '<Pi ' + GetPIString + ' />';
        //alert(GetPIStringstr);
    }
    else {
        GetPIString = '';
    }

    if (GetPA() == true) {
        GetPAStringstr = '<Pa ' + GetPAString + ' />';
        //alert(GetPAStringstr);
    }
    else {
        GetPAString = '';
    }

    if (GetPFA() == true) {
        GetPFAStringstr = '<Pfa ' + GetPFAString + ' />';
        //alert(GetPFAStringstr);
    }
    else {
        GetPFAString = '';
    }

    if (GetPI() == false && GetPA() == false && GetPFA() == false) {
        //alert("Fill Data!");
        DemoFinalString = '';
    }
    else {
        DemoFinalString = '<Demo>' + GetPIStringstr + ' ' + GetPAStringstr + ' ' + GetPFAStringstr + ' </Demo>';
        //alert(DemoFinalString)
    }
}

function GetPI() {
    return false;
    var Flag = false;
    GetPIString = '';

    if ($("#txtName").val().trim().length > 0) {
        Flag = true;
        GetPIString += "name=" + "\"" + $("#txtName").val().trim() + "\"";
    }

    if ($("#drpMatchValuePI").val() > 0 && Flag) {
        Flag = true;
        GetPIString += " mv=" + "\"" + $("#drpMatchValuePI").val().trim() + "\"";
    }

    if ($('#rdExactPI').is(':checked') && Flag) {
        Flag = true;
        GetPIString += " ms=" + "\"E\"";
    }
    else if ($('#rdPartialPI').is(':checked') && Flag) {
        Flag = true;
        GetPIString += " ms=" + "\"P\"";
    }
    else if ($('#rdFuzzyPI').is(':checked') && Flag) {
        Flag = true;
        GetPIString += " ms=" + "\"F\"";
    }
    if ($("#txtLocalNamePI").val().trim().length > 0) {
        Flag = true;
        GetPIString += " lname=" + "\"" + $("#txtLocalNamePI").val().trim() + "\"";
    }

    if ($("#txtLocalNamePI").val().trim().length > 0 && $("#drpLocalMatchValuePI").val() > 0) {
        Flag = true;
        GetPIString += " lmv=" + "\"" + $("#drpLocalMatchValuePI").val().trim() + "\"";
    }

    //< !-- if ($("#drpGender").val() > 0)-->
    //< !--{ -->

    if ($("#drpGender").val().trim() == "MALE") {
        Flag = true;
        GetPIString += " gender=" + "\"M\"";
    }
    else if ($("#drpGender").val().trim() == "FEMALE") {
        Flag = true;
        GetPIString += " gender=" + "\"F\"";
    }
    else if ($("#drpGender").val().trim() == "TRANSGENDER") {
        Flag = true;
        GetPIString += " gender=" + "\"T\"";
    }
    //}
    if ($("#txtDOB").val().trim().length > 0) {
        Flag = true;
        GetPIString += " dob=" + "\"" + $("#txtDOB").val().trim() + "\"";
    }

    if ($("#drpDOBType").val() != "0") {
        Flag = true;
        GetPIString += " dobt=" + "\"" + $("#drpDOBType").val().trim() + "\"";
    }

    if ($("#txtAge").val().trim().length) {
        Flag = true;
        GetPIString += " age=" + "\"" + $("#txtAge").val().trim() + "\"";
    }

    if ($("#txtPhone").val().trim().length > 0 || $("#txtEmail").val().trim().length > 0) {
        Flag = true;
        GetPIString += " phone=" + "\"" + $("#txtPhone").val().trim() + "\"";
    }
    if ($("#txtEmail").val().trim().length > 0) {
        Flag = true;
        GetPIString += " email=" + "\"" + $("#txtEmail").val().trim() + "\"";
    }

    //alert(GetPIString);
    return Flag;
}

function GetPA() {
    return false;
    var Flag = false;
    GetPAString = '';

    if ($("#txtCareOf").val().trim().length > 0) {
        Flag = true;
        GetPAString += "co=" + "\"" + $("#txtCareOf").val().trim() + "\"";
    }
    if ($("#txtLandMark").val().trim().length > 0) {
        Flag = true;
        GetPAString += " lm=" + "\"" + $("#txtLandMark").val().trim() + "\"";
    }
    if ($("#txtLocality").val().trim().length > 0) {
        Flag = true;
        GetPAString += " loc=" + "\"" + $("#txtLocality").val().trim() + "\"";
    }
    if ($("#txtCity").val().trim().length > 0) {
        Flag = true;
        GetPAString += " vtc=" + "\"" + $("#txtCity").val().trim() + "\"";
    }
    if ($("#txtDist").val().trim().length > 0) {
        Flag = true;
        GetPAString += " dist=" + "\"" + $("#txtDist").val().trim() + "\"";
    }
    if ($("#txtPinCode").val().trim().length > 0) {
        Flag = true;
        GetPAString += " pc=" + "\"" + $("#txtPinCode").val().trim() + "\"";
    }
    if ($("#txtBuilding").val().trim().length > 0) {
        Flag = true;
        GetPAString += " house=" + "\"" + $("#txtBuilding").val().trim() + "\"";
    }
    if ($("#txtStreet").val().trim().length > 0) {
        Flag = true;
        GetPAString += " street=" + "\"" + $("#txtStreet").val().trim() + "\"";
    }
    if ($("#txtPOName").val().trim().length > 0) {
        Flag = true;
        GetPAString += " po=" + "\"" + $("#txtPOName").val().trim() + "\"";
    }
    if ($("#txtSubDist").val().trim().length > 0) {
        Flag = true;
        GetPAString += " subdist=" + "\"" + $("#txtSubDist").val().trim() + "\"";
    }
    if ($("#txtState").val().trim().length > 0) {
        Flag = true;
        GetPAString += " state=" + "\"" + $("#txtState").val().trim() + "\"";
    }
    if ($('#rdMatchStrategyPA').is(':checked') && Flag) {
        Flag = true;
        GetPAString += " ms=" + "\"E\"";
    }
    return Flag;
}

function GetPFA() {
    return false;
    var Flag = false;
    GetPFAString = '';

    if ($("#txtAddressValue").val().trim().length > 0) {
        Flag = true;
        GetPFAString += "av=" + "\"" + $("#txtAddressValue").val().trim() + "\"";
    }

    if ($("#drpMatchValuePFA").val() > 0 && $("#txtAddressValue").val().trim().length > 0) {
        Flag = true;
        GetPFAString += " mv=" + "\"" + $("#drpMatchValuePFA").val().trim() + "\"";
    }

    if ($('#rdExactPFA').is(':checked') && Flag) {
        Flag = true;
        GetPFAString += " ms=" + "\"E\"";
    }
    else if ($('#rdPartialPFA').is(':checked') && Flag) {
        Flag = true;
        GetPFAString += " ms=" + "\"P\"";
    }
    else if ($('#rdFuzzyPFA').is(':checked') && Flag) {
        Flag = true;
        GetPFAString += " ms=" + "\"F\"";
    }

    if ($("#txtLocalAddress").val().trim().length > 0) {
        Flag = true;
        GetPFAString += " lav=" + "\"" + $("#txtLocalAddress").val().trim() + "\"";
    }

    if ($("#drpLocalMatchValue").val() > 0 && $("#txtLocalAddress").val().trim().length > 0) {
        Flag = true;
        GetPFAString += " lmv=" + "\"" + $("#drpLocalMatchValue").val().trim() + "\"";
    }
    //alert(GetPIString);
    return Flag;
}

var MethodCapture = '', finalUrl = '';
function getHttpError(jqXHR) {
    var err = "Unhandled Exception";
    if (jqXHR.status === 0) {
        err = 'Service Unavailable';
    } else if (jqXHR.status == 404) {
        err = 'Requested page not found';
    } else if (jqXHR.status == 500) {
        err = 'Internal Server Error';
    } else if (thrownError === 'parsererror') {
        err = 'Requested JSON parse failed';
    } else if (thrownError === 'timeout') {
        err = 'Time out error';
    } else if (thrownError === 'abort') {
        err = 'Ajax request aborted';
    } else {
        err = 'Unhandled Error';
    }
    return err;
}

function BindData() {
    jQuery.post("/AEPS/BindBankList", function (datas) {
        if (datas.StatusCode === 1) {
            var BankText = "<option value=''>Select</option>";
            jQuery.each(datas.Data, function (item, DD) {
                BankText += "<option value='" + DD.Id + "'>" + DD.BankName + "</option>";
            });
            jQuery('#InqBankName').html(BankText); jQuery('#withBankName').html(BankText); jQuery('#MiniBankName').html(BankText);
        } else {
            swal('', datas.Message, 'error');
        }
    });
}

function BindsData() {
    var htmlStr = '';
    jQuery.post("/AEPS/LastOrdersDetails", function (datas) {
        if (datas.StatusCode === 1) {
            jQuery.each(datas.Data, function (rows, item) {
                var htmlStatus = item.Status == 'Success' ? 'text-success' : item.Status == 'Pending' ? 'text-pending' : 'text-failure';
                var receiptStr = item.Status == 'Success' ? '<a href="Receipt/' + item.TransRef + '"><i class="mdi mdi-printer"></i></a>' : '';

                htmlStr += '<li class="br1 mt-0"><i class="fe fe-check bg-lt text-white product-icon primary-dropshadow"></i>'
                    + '<a target="_blank" href="#" class="font-weight-semibold mb-4 fs-15">TXN ID:' + item.TransRef + '</a>'
                    + '<a href="#" class="float-right fs-12 text-muted">' + item.TransactionDate + '</a>'
                    + '<p class="mb-0 mt-2 text-muted"></p>'
                    + '<div class="row">'
                    + '<div class="col-lg-4 col-md-6">'
                    + '<span>RRN:</span><span class="f-12">' + item.RRN + '</span></div>'
                    + '<div class="col-lg-5 col-md-6">'
                    + '<span class="label label-success">Bank Name: <span class="f-12">' + item.BankName + '</span></span></div>'
                    + '<div class="col-lg-3 col-md-6">'
                    + '<span>AMT:<span class="f-12"><i class="mdi mdi-currency-inr"></i>' + item.Amount + '</span></span></span>'
                    + '</div>'
                    + '</div>'
                    + '<div class="row">'
                    + '<div class="col-lg-4 col-md-6">'
                    + '<span class="f-12">' + item.AadharNo + '</span><br>'
                    + '</div>'
                    + '<div class="col-lg-5 col-md-6">'
                    + '<span class="label label-success">Number: <span class="f-12">' + item.Number + '</span></span>'
                    + '</div>'
                    + '<div class="col-lg-3 col-md-6">'
                    + '<span class="' + htmlStatus + '"><i class="mdi mdi-checkbox-marked-circle-outline"></i>' + item.Status + '</span>' + receiptStr
                    + '</div>'
                    + '</div>'
                    + '</li>';
            });
        } else {
            htmlStr = '<li><h4 colspan="6">DATA NOT AVAILABLE</h4></li>'
        }
        jQuery('#reportBody').html(htmlStr);
    });
}

function ValidateCustomerRequest() {
    jQuery('#withErrorMessage').html('');
    var bankName = jQuery('#withBankName').val();
    var mobileNo = jQuery('#withMobileNumber').val();
    var amount = jQuery('#withAmount').val();
    var deviceName = jQuery('#withDeviceName').val();
    var agentTC = jQuery('#withAgenttc').prop('checked');
    var customerTC = jQuery('#withCustomertc').prop('checked');
    var aadharNo = '';
    for (i = 0; i < 12; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (bankName != '' && aadharNo != '' && aadharNo.length === 12 && mobileNo.length === 10 && amount != '' && deviceName != '') {
        if (amount > 0 && amount <= 10000) {
            if (agentTC && customerTC) {
                swal({ title: "Processing", text: "Please wait..", imageUrl: "/Content/Plugins/img/Processing.gif", showConfirmButton: false });
                jQuery.post("/AEPS/CashValidateRequest", { mobileNumber: mobileNo, bankName: bankName, aadharNumber: aadharNo, amount: amount }, function (result) {
                    if (result.StatusCode === 1) {
                        jQuery('#myModal').modal('show'); swal.close();
                        jQuery('#transactionAmount').html(result.Data.Amount);
                    } else if (result.StatusCode === 0) {
                        swal('', result.Message, 'error')
                    } else {
                        swal('', result.Message, 'warning')
                    }
                });
            } else {
                jQuery('#withErrorMessage').html("Terms and condition are required.");
            }
        } else {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000.");
        }
    } else {
        jQuery('#withErrorMessage').html("All field's are required.");
    }
}

function CaptureAvdmRequest() {
    jQuery('#withErrorMessage').html('');
    var bankName = jQuery('#withBankName').val();
    var mobileNo = jQuery('#withMobileNumber').val();
    var amount = jQuery('#withAmount').val();
    var deviceName = jQuery('#withDeviceName').val();
    var agentTC = jQuery('#withAgenttc').prop('checked');
    var customerTC = jQuery('#withCustomertc').prop('checked');
    var aadharNo = '';
    for (i = 0; i < 12; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (bankName != '' && aadharNo != '' && aadharNo.length === 12 && mobileNo.length === 10 && amount != '' && deviceName != '') {
        if (amount > 0 && amount <= 10000) {
            if (agentTC && customerTC) {
                jQuery('#withPidData').val(''), jQuery('#withPidDataOption').val('');
                jQuery('#confirmationbox').hide(); jQuery('#confirmationboxfirnger').show();
                if (deviceName === 'MANTRA') {
                    var SuccessFlag = 0;
                    var primaryUrl = "http://127.0.0.1:";
                    try {
                        var protocol = window.location.href;
                        if (protocol.indexOf("https") >= 0) {
                            primaryUrl = "http://127.0.0.1:";
                        }
                    } catch (e) { }
                    url = "";
                    for (var i = 11100; i <= 11120; i++) {
                        if (primaryUrl == "https://127.0.0.1:" && OldPort == true) {
                            i = "8005";
                        }
                        var verb = "RDSERVICE";
                        var err = "";
                        SuccessFlag = 0;

                        $.support.cors = true;
                        var httpStaus = false;
                        var jsonstr = "";
                        var data = new Object();
                        var obj = new Object();
                        $.ajax({
                            type: "RDSERVICE",
                            async: false,
                            url: primaryUrl + i.toString(),
                            contentType: "text/xml; charset=utf-8",
                            processData: false,
                            cache: false,
                            crossDomain: true,
                            success: function (data) {
                                httpStaus = true;
                                res = { httpStaus: httpStaus, data: data };
                                finalUrl = primaryUrl + i.toString();
                                var $doc = $.parseXML(data);
                                var CmbData1 = $($doc).find('RDService').attr('status');
                                var CmbData2 = $($doc).find('RDService').attr('info');
                                if (RegExp('\\b' + 'Mantra' + '\\b').test(CmbData2) == true) {
                                    //$("#txtDeviceInfo").val(data);

                                    if ($($doc).find('Interface').eq(0).attr('path') == "/rd/capture") {
                                        MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                                    }
                                    if ($($doc).find('Interface').eq(1).attr('path') == "/rd/capture") {
                                        MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                                    }
                                    if ($($doc).find('Interface').eq(0).attr('path') == "/rd/info") {
                                        MethodInfo = $($doc).find('Interface').eq(0).attr('path');
                                    }
                                    if ($($doc).find('Interface').eq(1).attr('path') == "/rd/info") {
                                        MethodInfo = $($doc).find('Interface').eq(1).attr('path');
                                    }

                                    SuccessFlag = 1;
                                    jQuery('#btnFingerConfirmation').hide(), jQuery('#btnFingerDismiss').hide();
                                    CaptureAvdmcasRequest();
                                    return;
                                }
                            },
                            error: function (jqXHR, ajaxOptions, thrownError) {
                                if (i == "8005" && OldPort == true) {
                                    OldPort = false;
                                    i = "11099";
                                }
                            },

                        });
                        if (SuccessFlag == 1) {
                            break;
                        }
                    }
                    if (SuccessFlag == 0) {
                        alert("Connection failed Please try again."); jQuery('#myModal').modal('hide');
                        jQuery('#confirmationbox').show(); jQuery('#confirmationboxfirnger').hide();
                    }
                } else {
                    CaptureAvdmRequestv();
                }
            } else {
                jQuery('#withErrorMessage').html("Terms and condition are required.");
            }
        } else {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000.");
        }
    } else {
        jQuery('#withErrorMessage').html("All field's are required.");
    }
}

function CaptureAvdmcasRequest() {
    var strWadh = "";
    var strOtp = "";
    Demo();
    var XML = '<?xml version="1.0"?> <PidOptions ver="1.0"> <Opts fCount="1" fType="0" iCount="0" pCount="0" pgCount="2"' + strOtp + ' format="0" pidVer="2.0" timeout="10000" pTimeout="20000"' + strWadh + ' posh="UNKNOWN" env="p" /> ' + DemoFinalString + '<CustOpts><Param name="mantrakey" value="" /></CustOpts> </PidOptions>';
    var verb = "CAPTURE";
    var err = "";
    var res;
    $.support.cors = true;
    var httpStaus = false;
    var jsonstr = "";
    $.ajax({
        type: "CAPTURE",
        async: true,
        crossDomain: true,
        url: finalUrl + MethodCapture,
        data: XML,
        contentType: "text/xml; charset=utf-8",
        processData: false,
        success: function (data) {
            httpStaus = true;
            res = { httpStaus: httpStaus, data: data };
            jQuery('#withPidData').val(data);
            jQuery('#withPidDataOption').val(XML);

            var $doc = $.parseXML(data);
            var Message = $($doc).find('Resp').attr('errInfo');
            if (Message == 'Success') {
                CaptureAvdmcasfinalRequest();
            } else {
                alert(Message); location.reload();
            }
        },
        error: function (jqXHR, ajaxOptions, thrownError) {
            alert(thrownError);
            res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
        },
    });

    return res;
}

function CaptureAvdmRequestv() {
    var url = "http://127.0.0.1:11100";
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    jQuery('#btnFingerConfirmation').hide(), jQuery('#btnFingerDismiss').hide();
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }
    xhr.open('RDSERVICE', url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;
            if (status == 200) {
                var data = xhr.responseText;
                finalUrl = 'http:/';
                var $doc = $.parseXML(data);
                var CmbData2 = $($doc).find('RDService').attr('info');
                if (RegExp('\\b' + 'Morpho_RD_Service' + '\\b').test(CmbData2) == true) {
                    if ($($doc).find('Interface').eq(0).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                    }
                    if ($($doc).find('Interface').eq(1).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                    }

                    CaptureAvdmcasRequestv();

                } else {
                    alert("Connection failed Please try again."); location.reload();
                }
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send();
}

function CaptureAvdmcasRequestv() {
    var url = finalUrl + MethodCapture;
    var PIDOPTS = '<PidOptions ver=\"1.0\">' + '<Opts fCount=\"1\" fType=\"0\" iCount=\"\" iType=\"\" pCount=\"\" pType=\"\" format=\"0\" pidVer=\"2.0\" timeout=\"10000\" otp=\"\" wadh=\"\" posh=\"\"/>' + '</PidOptions>';
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }

    xhr.open('CAPTURE', url, true);
    xhr.setRequestHeader("Content-Type", "text/xml");
    xhr.setRequestHeader("Accept", "text/xml");

    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;
            if (status == 200) {
                jQuery('#withPidData').val(xhr.responseText);
                jQuery('#withPidDataOption').val(PIDOPTS);
                CaptureAvdmcasfinalRequest();
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send(PIDOPTS);
}

function CaptureAvdmcasfinalRequest() {
    var bankName = jQuery('#withBankName').val();
    var mobileNo = jQuery('#withMobileNumber').val();
    var amount = jQuery('#withAmount').val();
    var pidData = jQuery('#withPidData').val();
    jQuery('#responsePrint').show();
    var aadharNo = '';
    for (i = 0; i < 12; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    jQuery.post("/AEPS/CashWithdrawalRequest", { mobileNumber: mobileNo, bankName: bankName, aadharNumber: aadharNo, biometricData: pidData, amount: amount }, function (result) {
        if (result.StatusCode === 1) {
            jQuery('#myModal').modal('hide');
            jQuery('#ResponseModal').modal('show');
            jQuery('#responseIcon').attr('src', '/Content/Plugins/img/success.png');
            jQuery('#responseStatus').html('SUCCESS');
            jQuery('#responseMessage').html(result.Message);
            jQuery('#responseNumber').html(mobileNo);
            jQuery('#responseAmount').html(amount);
            jQuery('#responseRRN').html(result.Data.RRN);
            jQuery('#responseUID').html(result.Data.AdhaarNo);
            jQuery('#responseAvBalance').html(result.Data.AvailableBalance);
            jQuery('#responsePrint').attr('href', '/AEPS/Receipt/' + result.RefNumber);
        } else if (result.StatusCode === 0) {
            jQuery('#myModal').modal('hide');
            jQuery('#ResponseModal').modal('show');
            jQuery('#responseIcon').attr('src', '/Content/Plugins/img/fail.png');
            jQuery('#responseStatus').html('FAILED');
            jQuery('#responseMessage').html(result.Message);
            jQuery('#responseNumber').html(mobileNo);
            jQuery('#responseAmount').html(amount);
            jQuery('#responseRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            jQuery('#responseAvBalance').html(result.Data == null ? '' : result.Data.AvailableBalance);
            jQuery('#responsePrint').hide();
        } else {
            jQuery('#myModal').modal('hide');
            jQuery('#ResponseModal').modal('show');
            jQuery('#responseIcon').attr('src', '/Content/Plugins/img/pending.png');
            jQuery('#responseStatus').html('PENDING');
            jQuery('#responseMessage').html(result.Message);
            jQuery('#responseNumber').html(mobileNo);
            jQuery('#responseAmount').html(amount);
            jQuery('#responseRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            jQuery('#responseAvBalance').html(result.Data == null ? '' : result.Data.AvailableBalance);
            jQuery('#responsePrint').hide();
        }
        ResetWithData(); BindsData();
    });
}

function ResetWithData() {
    jQuery('#select2-withBankName-container').html('<span class="select2-selection__placeholder">Select Bank Name</span>'), jQuery('#withBankName').val(''), jQuery('#withMobileNumber').val(''), jQuery('#withAmount').val(''), jQuery('#withPidData').val(''), jQuery('#withPidDataOption').val(''), jQuery('#1').val(''), jQuery('#2').val(''), jQuery('#3').val(''), jQuery('#4').val(''), jQuery('#5').val(''), jQuery('#6').val(''), jQuery('#7').val(''), jQuery('#8').val(''), jQuery('#9').val(''), jQuery('#10').val(''), jQuery('#11').val(''), jQuery('#12').val('');
    jQuery('#withAgenttc').prop('checked', false), jQuery('#withCustomertc').prop('checked', false);
    jQuery('#btnFingerConfirmation').show(), jQuery('#btnFingerDismiss').show(); jQuery('#myModal').modal('hide'); jQuery('#confirmationbox').show(); jQuery('#confirmationboxfirnger').hide();
}

function ValidateCustomerInqRequest() {
    jQuery('#InqErrorMessage').html('');
    var bankName = jQuery('#InqBankName').val();
    var mobileNo = jQuery('#InqMobileNumber').val();
    var deviceName = jQuery('#InqDeviceName').val();
    var agentTC = jQuery('#InqAgenttc').prop('checked');
    var customerTC = jQuery('#InqCustomertc').prop('checked');
    var aadharNo = '';
    for (i = 20; i < 32; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (bankName != '' && aadharNo != '' && aadharNo.length === 12 && mobileNo.length === 10 && deviceName != '') {
        if (agentTC && customerTC) {
            jQuery('#InqPidData').val(''), jQuery('#InqPidDataOption').val('');
            jQuery('#confirmationinqbox').show(); jQuery('#confirmationinqboxfirnger').hide();
            jQuery('#btnInquiryConfirmation').show(), jQuery('#btnFingerInqDismiss').show();
            jQuery('#myInquiryModal').modal('show');
            jQuery('#inquiryAadhar').html('XXXXXXXX' + aadharNo.substring(8, 12))
        } else {
            jQuery('#InqErrorMessage').html("Terms and condition are required.");
        }
    } else {
        jQuery('#InqErrorMessage').html("All field's are required.");
    }
}

function CaptureInfoAvdmRequest() {
    jQuery('#InqErrorMessage').html('');
    var bankName = jQuery('#InqBankName').val();
    var mobileNo = jQuery('#InqMobileNumber').val();
    var deviceName = jQuery('#InqDeviceName').val();
    var agentTC = jQuery('#InqAgenttc').prop('checked');
    var customerTC = jQuery('#InqCustomertc').prop('checked');
    var aadharNo = '';
    for (i = 20; i < 32; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (bankName != '' && aadharNo != '' && aadharNo.length === 12 && mobileNo.length === 10 && deviceName != '') {
        if (agentTC && customerTC) {
            jQuery('#InqPidData').val(''), jQuery('#InqPidDataOption').val('');
            jQuery('#confirmationinqbox').hide(); jQuery('#confirmationinqboxfirnger').show();
            if (deviceName === 'MANTRA') {
                var SuccessFlag = 0;
                var primaryUrl = "http://127.0.0.1:";
                try {
                    var protocol = window.location.href;
                    if (protocol.indexOf("https") >= 0) {
                        primaryUrl = "http://127.0.0.1:";
                    }
                } catch (e) { }
                url = "";
                for (var i = 11100; i <= 11120; i++) {
                    if (primaryUrl == "https://127.0.0.1:" && OldPort == true) {
                        i = "8005";
                    }
                    var verb = "RDSERVICE";
                    var err = "";
                    SuccessFlag = 0;
                    $.support.cors = true;
                    var httpStaus = false;
                    var jsonstr = "";
                    var data = new Object();
                    var obj = new Object();
                    $.ajax({
                        type: "RDSERVICE",
                        async: false,
                        url: primaryUrl + i.toString(),
                        contentType: "text/xml; charset=utf-8",
                        processData: false,
                        cache: false,
                        crossDomain: true,
                        success: function (data) {
                            httpStaus = true;
                            res = { httpStaus: httpStaus, data: data };
                            finalUrl = primaryUrl + i.toString();
                            var $doc = $.parseXML(data);
                            var CmbData1 = $($doc).find('RDService').attr('status');
                            var CmbData2 = $($doc).find('RDService').attr('info');
                            if (RegExp('\\b' + 'Mantra' + '\\b').test(CmbData2) == true) {

                                if ($($doc).find('Interface').eq(0).attr('path') == "/rd/capture") {
                                    MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                                }
                                if ($($doc).find('Interface').eq(1).attr('path') == "/rd/capture") {
                                    MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                                }
                                if ($($doc).find('Interface').eq(0).attr('path') == "/rd/info") {
                                    MethodInfo = $($doc).find('Interface').eq(0).attr('path');
                                }
                                if ($($doc).find('Interface').eq(1).attr('path') == "/rd/info") {
                                    MethodInfo = $($doc).find('Interface').eq(1).attr('path');
                                }

                                SuccessFlag = 1;
                                jQuery('#btnInquiryConfirmation').hide(), jQuery('#btnFingerInqDismiss').hide();
                                CaptureAvdminqRequest();
                                return;
                            }
                        },
                        error: function (jqXHR, ajaxOptions, thrownError) {
                            if (i == "8005" && OldPort == true) {
                                OldPort = false;
                                i = "11099";
                            }
                        },
                    });
                    if (SuccessFlag == 1) {
                        break;
                    }
                }
                if (SuccessFlag == 0) {
                    alert("Connection failed Please try again."); jQuery('#myInquiryModal').modal('hide');
                    jQuery('#confirmationinqbox').show(); jQuery('#confirmationinqboxfirnger').hide();
                }
            } else {
                CaptureInfoAvdmRequestv();
            }
        } else {
            jQuery('#InqErrorMessage').html("Terms and condition are required.");
        }
    } else {
        jQuery('#InqErrorMessage').html("All field's are required.");
    }
}

function CaptureAvdminqRequest() {
    var strWadh = "";
    var strOtp = "";
    Demo();
    var XML = '<?xml version="1.0"?> <PidOptions ver="1.0"> <Opts fCount="1" fType="0" iCount="0" pCount="0" pgCount="2"' + strOtp + ' format="0" pidVer="2.0" timeout="10000" pTimeout="20000"' + strWadh + ' posh="UNKNOWN" env="p" /> ' + DemoFinalString + '<CustOpts><Param name="mantrakey" value="" /></CustOpts> </PidOptions>';
    var verb = "CAPTURE";
    var err = "";
    var res;
    $.support.cors = true;
    var httpStaus = false;
    var jsonstr = "";
    $.ajax({
        type: "CAPTURE",
        async: true,
        crossDomain: true,
        url: finalUrl + MethodCapture,
        data: XML,
        contentType: "text/xml; charset=utf-8",
        processData: false,
        success: function (data) {
            httpStaus = true;
            res = { httpStaus: httpStaus, data: data };
            jQuery('#InqPidData').val(data);
            jQuery('#InqPidDataOption').val(XML);
            var $doc = $.parseXML(data);
            var Message = $($doc).find('Resp').attr('errInfo');
            if (Message == 'Success') {
                CaptureAvdminqfinalRequest();
            } else {
                alert(Message); location.reload();
            }
        },
        error: function (jqXHR, ajaxOptions, thrownError) {
            alert(thrownError);
            res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
        },
    });

    return res;
}

function CaptureInfoAvdmRequestv() {
    var url = "http://127.0.0.1:11100";
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    jQuery('#btnInquiryConfirmation').hide(), jQuery('#btnFingerInqDismiss').hide();
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }
    xhr.open('RDSERVICE', url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;
            if (status == 200) {
                var data = xhr.responseText;
                finalUrl = 'http:/';
                var $doc = $.parseXML(data);
                var CmbData2 = $($doc).find('RDService').attr('info');
                if (RegExp('\\b' + 'Morpho_RD_Service' + '\\b').test(CmbData2) == true) {
                    if ($($doc).find('Interface').eq(0).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                    }
                    if ($($doc).find('Interface').eq(1).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                    }

                    CaptureAvdminqRequestv();

                } else {
                    alert("Connection failed Please try again."); location.reload();
                }
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send();
}

function CaptureAvdminqRequestv() {
    var url = finalUrl + MethodCapture;
    var PIDOPTS = '<PidOptions ver=\"1.0\">' + '<Opts fCount=\"1\" fType=\"0\" iCount=\"\" iType=\"\" pCount=\"\" pType=\"\" format=\"0\" pidVer=\"2.0\" timeout=\"10000\" otp=\"\" wadh=\"\" posh=\"\"/>' + '</PidOptions>';
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }

    xhr.open('CAPTURE', url, true);
    xhr.setRequestHeader("Content-Type", "text/xml");
    xhr.setRequestHeader("Accept", "text/xml");

    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;
            if (status == 200) {
                var $doc = $.parseXML(xhr.responseText);
                var errorCode = $($doc).find('Resp').attr('errCode');
                if (errorCode == '0') {
                    jQuery('#InqPidData').val(xhr.responseText);
                    jQuery('#InqPidDataOption').val(PIDOPTS);
                    CaptureAvdminqfinalRequest();
                } else {
                    var errorMessage = $($doc).find('Resp').attr('errInfo');
                    alert(errorMessage); jQuery('#myInquiryModal').modal('hide');
                }
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send(PIDOPTS);
}

function CaptureAvdminqfinalRequest() {
    var bankName = jQuery('#InqBankName').val();
    var mobileNo = jQuery('#InqMobileNumber').val();
    var pidData = jQuery('#InqPidData').val();
    var aadharNo = '';
    for (i = 20; i < 32; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    jQuery.post("/AEPS/BalanceInquiryRequest", { mobileNumber: mobileNo, bankName: bankName, aadharNumber: aadharNo, biometricData: pidData }, function (result) {
        if (result.StatusCode === 1) {
            jQuery('#myInquiryModal').modal('hide');
            jQuery('#InqResponseModal').modal('show');
            jQuery('#responseInqIcon').attr('src', '/Content/Plugins/img/success.png');
            jQuery('#responseInqStatus').html('SUCCESS');
            jQuery('#responseInqMessage').html(result.Message);
            jQuery('#responseInqNumber').html(mobileNo);
            jQuery('#responseInqRRN').html(result.Data.RRN);
            jQuery('#responseInqUID').html(result.Data.AdhaarNo);
            jQuery('#responseInqAvBalance').html(result.Data.AvailableBalance);
        } else if (result.StatusCode === 0) {
            jQuery('#myInquiryModal').modal('hide');
            jQuery('#InqResponseModal').modal('show');
            jQuery('#responseInqIcon').attr('src', '/Content/Plugins/img/fail.png');
            jQuery('#responseInqStatus').html('FAILED');
            jQuery('#responseInqMessage').html(result.Message);
            jQuery('#responseInqNumber').html(mobileNo);
            jQuery('#responseInqRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseInqUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            jQuery('#responseInqAvBalance').html(result.Data == null ? '' : result.Data.AvailableBalance);
        } else {
            jQuery('#myInquiryModal').modal('hide');
            jQuery('#InqResponseModal').modal('show');
            jQuery('#responseInqIcon').attr('src', '/Content/Plugins/img/pending.png');
            jQuery('#responseInqStatus').html('PENDING');
            jQuery('#responseInqMessage').html(result.Message);
            jQuery('#responseInqNumber').html(mobileNo);
            jQuery('#responseInqRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseInqUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            jQuery('#responseInqAvBalance').html(result.Data == null ? '' : result.Data.AvailableBalance);
        }
        ResetInqData();
    });
}

function ResetInqData() {
    jQuery('#select2-InqBankName-container').html('<span class="select2-selection__placeholder">Select Bank Name</span>'), jQuery('#InqBankName').val(''), jQuery('#InqMobileNumber').val(''), jQuery('#InqPidData').val(''), jQuery('#InqPidDataOption').val(''), jQuery('#inquiryAadhar').html(''), jQuery('#21').val(''), jQuery('#22').val(''), jQuery('#23').val(''), jQuery('#24').val(''), jQuery('#25').val(''), jQuery('#26').val(''), jQuery('#27').val(''), jQuery('#28').val(''), jQuery('#29').val(''), jQuery('#30').val(''), jQuery('#31').val(''), jQuery('#32').val('');
    jQuery('#InqAgenttc').prop('checked', false), jQuery('#InqCustomertc').prop('checked', false);
    jQuery('#btnInquiryConfirmation').show(), jQuery('#btnFingerInqDismiss').show(); jQuery('#myInquiryModal').modal('hide'); jQuery('#confirmationinqbox').show(); jQuery('#confirmationinqboxfirnger').hide();
}

function getDateFromAspNetFormat(jsonDate) {
    var nowDate = new Date(parseInt(jsonDate.substr(6)));
    return nowDate.toLocaleDateString() + ' ' + nowDate.toLocaleTimeString();
}

function ValidateCustomerMiniRequest() {
    jQuery('#MiniErrorMessage').html('');
    var bankName = jQuery('#MiniBankName').val();
    var mobileNo = jQuery('#MiniMobileNumber').val();
    var deviceName = jQuery('#MiniDeviceName').val();
    var agentTC = jQuery('#MiniAgenttc').prop('checked');
    var customerTC = jQuery('#MiniCustomertc').prop('checked');
    var aadharNo = '';
    for (i = 40; i < 52; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (bankName != '' && aadharNo != '' && aadharNo.length === 12 && mobileNo.length === 10 && deviceName != '') {
        if (agentTC && customerTC) {
            jQuery('#MiniPidData').val(''), jQuery('#MiniPidDataOption').val('');
            jQuery('#myMiniModal').modal('show');
            jQuery('#MiniAadhar').html('XXXXXXXX' + aadharNo.substring(8, 12))
        } else {
            jQuery('#MiniErrorMessage').html("Terms and condition are required.");
        }
    } else {
        jQuery('#MiniErrorMessage').html("All field's are required.");
    }
}

function CaptureMiniAvdmRequest() {
    jQuery('#MiniErrorMessage').html('');
    var bankName = jQuery('#MiniBankName').val();
    var mobileNo = jQuery('#MiniMobileNumber').val();
    var deviceName = jQuery('#MiniDeviceName').val();
    var agentTC = jQuery('#MiniAgenttc').prop('checked');
    var customerTC = jQuery('#MiniCustomertc').prop('checked');
    var aadharNo = '';
    for (i = 40; i < 52; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (bankName != '' && aadharNo != '' && mobileNo.length === 10 && deviceName != '') {
        if (agentTC && customerTC) {
            jQuery('#MiniPidData').val(), jQuery('#MiniPidDataOption').val();
            jQuery('#confirmationminibox').hide(); jQuery('#confirmationminiboxfirnger').show();
            if (deviceName === 'MANTRA') {
                var SuccessFlag = 0;
                var primaryUrl = "http://127.0.0.1:";
                try {
                    var protocol = window.location.href;
                    if (protocol.indexOf("https") >= 0) {
                        primaryUrl = "http://127.0.0.1:";
                    }
                } catch (e) { }
                url = "";
                for (var i = 11100; i <= 11120; i++) {
                    if (primaryUrl == "https://127.0.0.1:" && OldPort == true) {
                        i = "8005";
                    }
                    var verb = "RDSERVICE";
                    var err = "";
                    SuccessFlag = 0;

                    $.support.cors = true;
                    var httpStaus = false;
                    var jsonstr = "";
                    var data = new Object();
                    var obj = new Object();
                    $.ajax({
                        type: "RDSERVICE",
                        async: false,
                        url: primaryUrl + i.toString(),
                        contentType: "text/xml; charset=utf-8",
                        processData: false,
                        cache: false,
                        crossDomain: true,
                        success: function (data) {
                            httpStaus = true;
                            res = { httpStaus: httpStaus, data: data };
                            finalUrl = primaryUrl + i.toString();
                            var $doc = $.parseXML(data);
                            var CmbData1 = $($doc).find('RDService').attr('status');
                            var CmbData2 = $($doc).find('RDService').attr('info');
                            if (RegExp('\\b' + 'Mantra' + '\\b').test(CmbData2) == true) {
                                if ($($doc).find('Interface').eq(0).attr('path') == "/rd/capture") {
                                    MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                                }
                                if ($($doc).find('Interface').eq(1).attr('path') == "/rd/capture") {
                                    MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                                }
                                if ($($doc).find('Interface').eq(0).attr('path') == "/rd/info") {
                                    MethodInfo = $($doc).find('Interface').eq(0).attr('path');
                                }
                                if ($($doc).find('Interface').eq(1).attr('path') == "/rd/info") {
                                    MethodInfo = $($doc).find('Interface').eq(1).attr('path');
                                }

                                SuccessFlag = 1;
                                jQuery('#btnMiniConfirmation').hide(), jQuery('#btnFingerMiniDismiss').hide();
                                CaptureAvdmminRequest();
                                return;
                            }
                        },
                        error: function (jqXHR, ajaxOptions, thrownError) {
                            if (i == "8005" && OldPort == true) {
                                OldPort = false;
                                i = "11099";
                            }
                        },
                    });
                    if (SuccessFlag == 1) {
                        break;
                    }
                }
                if (SuccessFlag == 0) {
                    alert("Connection failed Please try again."); jQuery('#myMiniModal').modal('hide');
                    jQuery('#confirmationminibox').show(); jQuery('#confirmationminiboxfirnger').hide();
                }
            } else {
                CaptureMiniAvdmRequestv();
            }
        } else {
            jQuery('#MiniErrorMessage').html("Terms and condition are required.");
        }
    } else {
        jQuery('#MiniErrorMessage').html("All field's are required.");
    }
}

function CaptureAvdmminRequest() {
    var strWadh = "";
    var strOtp = "";
    Demo();
    var XML = '<?xml version="1.0"?> <PidOptions ver="1.0"> <Opts fCount="1" fType="0" iCount="0" pCount="0" pgCount="2"' + strOtp + ' format="0" pidVer="2.0" timeout="10000" pTimeout="20000"' + strWadh + ' posh="UNKNOWN" env="p" /> ' + DemoFinalString + '<CustOpts><Param name="mantrakey" value="" /></CustOpts> </PidOptions>';
    var verb = "CAPTURE";
    var err = "";
    var res;
    $.support.cors = true;
    var httpStaus = false;
    var jsonstr = "";
    $.ajax({
        type: "CAPTURE",
        async: true,
        crossDomain: true,
        url: finalUrl + MethodCapture,
        data: XML,
        contentType: "text/xml; charset=utf-8",
        processData: false,
        success: function (data) {
            httpStaus = true;
            res = { httpStaus: httpStaus, data: data };
            jQuery('#MiniPidData').val(data);
            jQuery('#MiniPidDataOption').val(XML);

            var $doc = $.parseXML(data);
            var Message = $($doc).find('Resp').attr('errInfo');
            if (Message == 'Success') {
                CaptureAvdmminifinalRequest();
            } else {
                alert(Message); location.reload();
            }
        },
        error: function (jqXHR, ajaxOptions, thrownError) {
            alert(thrownError);
            res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
        },
    });

    return res;
}

function CaptureMiniAvdmRequestv() {
    var url = "http://127.0.0.1:11100";
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    jQuery('#btnMiniConfirmation').hide(), jQuery('#btnFingerMiniDismiss').hide();
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }
    xhr.open('RDSERVICE', url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;
            if (status == 200) {
                var data = xhr.responseText;
                finalUrl = 'http:/';
                var $doc = $.parseXML(data);
                var CmbData2 = $($doc).find('RDService').attr('info');
                if (RegExp('\\b' + 'Morpho_RD_Service' + '\\b').test(CmbData2) == true) {
                    if ($($doc).find('Interface').eq(0).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                    }
                    if ($($doc).find('Interface').eq(1).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                    }

                    CaptureAvdmminRequestv();

                } else {
                    alert("Connection failed Please try again."); location.reload();
                }
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send();
}

function CaptureAvdmminRequestv() {
    var url = finalUrl + MethodCapture;
    var PIDOPTS = '<PidOptions ver=\"1.0\">' + '<Opts fCount=\"1\" fType=\"0\" iCount=\"\" iType=\"\" pCount=\"\" pType=\"\" format=\"0\" pidVer=\"2.0\" timeout=\"10000\" otp=\"\" wadh=\"\" posh=\"\"/>' + '</PidOptions>';
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }
    xhr.open('CAPTURE', url, true);
    xhr.setRequestHeader("Content-Type", "text/xml");
    xhr.setRequestHeader("Accept", "text/xml");

    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;

            if (status == 200) {
                jQuery('#MiniPidData').val(xhr.responseText);
                jQuery('#MiniPidDataOption').val(PIDOPTS);
                CaptureAvdmminifinalRequest();
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send(PIDOPTS);
}

function CaptureAvdmminifinalRequest() {
    var bankName = jQuery('#MiniBankName').val();
    var mobileNo = jQuery('#MiniMobileNumber').val();
    var pidData = jQuery('#MiniPidData').val();
    var bankNames = jQuery("#MiniBankName option:selected").text();
    var aadharNo = '';
    for (i = 40; i < 52; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    jQuery.post("/AEPS/MiniStatementRequest", { mobileNumber: mobileNo, bankName: bankName, aadharNumber: aadharNo, biometricData: pidData }, function (result) {
        if (result.StatusCode === 1) {
            jQuery('#myMiniModal').modal('hide');
            jQuery('#MiniResponseModal').modal('show');
            jQuery('#responseMiniIcon').attr('src', '/Content/Plugins/img/statement.png');
            jQuery('#responseMiniStatus').html('AEPS MiniStatement');
            jQuery('#responseMiniMessage').html(result.Message);
            jQuery('#responseMiniDate').html(result.Data.localDate + ' ' + result.Data.localTime);
            jQuery('#responseMiniBankName').html(bankNames);
            jQuery('#responseMiniNumber').html(mobileNo);
            jQuery('#responseMiniAmount').html(result.Data.Balance);
            jQuery('#responseMiniRRN').html(result.Data.RRN);
            jQuery('#responseMiniUID').html(result.Data.AdhaarNo);
            jQuery('#responseMiniPrint').attr('href', '/AEPS/Statement/' + result.RefNumber);
            jQuery('#responseMiniPrint').show();
        } else if (result.StatusCode === 0) {
            jQuery('#myMiniModal').modal('hide');
            jQuery('#MiniResponseModal').modal('show');
            jQuery('#responseMiniIcon').attr('src', '/Content/Plugins/img/fail.png');
            jQuery('#responseMiniStatus').html('FAILED');
            jQuery('#responseMiniMessage').html(result.Message);
            jQuery('#responseMiniDate').html(result.Data == null ? '' : (result.Data.localDate + ' ' + result.Data.localTime));
            jQuery('#responseMiniBankName').html(bankNames);
            jQuery('#responseMiniNumber').html(mobileNo);
            jQuery('#responseMiniAmount').html(result.Data == null ? '' : result.Data.Balance);
            jQuery('#responseMiniRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseMiniUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            jQuery('#responseMiniPrint').hide();
        } else {
            jQuery('#myMiniModal').modal('hide');
            jQuery('#MiniResponseModal').modal('show');
            jQuery('#responseMiniIcon').attr('src', '/Content/Plugins/img/pending.png');
            jQuery('#responseMiniStatus').html('PENDING');
            jQuery('#responseMiniMessage').html(result.Message);
            jQuery('#responseMiniDate').html(result.Data == null ? '' : (result.Data.localDate + ' ' + result.Data.localTime));
            jQuery('#responseMiniBankName').html(bankNames);
            jQuery('#responseMiniNumber').html(mobileNo);
            jQuery('#responseMiniAmount').html(result.Data == null ? '' : result.Data.Balance);
            jQuery('#responseMiniRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseMiniUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            jQuery('#responseMiniPrint').hide();
        }
        ResetMiniData();
    });
}

function ResetMiniData() {
    jQuery('#select2-MiniBankName-container').html('<span class="select2-selection__placeholder">Select Bank Name</span>'), jQuery('#MiniBankName').val(''), jQuery('#MiniMobileNumber').val(''), jQuery('#MiniPidData').val(''), jQuery('#MiniPidDataOption').val(''), jQuery('#41').val(''), jQuery('#42').val(''), jQuery('#43').val(''), jQuery('#44').val(''), jQuery('#45').val(''), jQuery('#46').val(''), jQuery('#47').val(''), jQuery('#48').val(''), jQuery('#49').val(''), jQuery('#50').val(''), jQuery('#51').val(''), jQuery('#52').val('');
    jQuery('#MiniAgenttc').prop('checked', false), jQuery('#MiniCustomertc').prop('checked', false);
    jQuery('#btnMiniConfirmation').show(), jQuery('#btnFingerMiniDismiss').show(); jQuery('#myMiniModal').modal('hide'); jQuery('#confirmationminibox').show(); jQuery('#confirmationminiboxfirnger').hide();
}

function ValidatePasskey(tb, e) {
    var evt = (e) ? e : window.event;
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    if (charCode == 8) {
        if (tb.value.length == 0 && tb.id !== '1') {
            document.getElementById(parseInt(tb.id) - 1).focus();
        }
    } else {
        if (tb.value.length == 1 && tb.id !== '12') {
            document.getElementById(parseInt(tb.id) + 1).focus();
        }
    }
    //alert(charCode);
}

function showRequestDetails() {
    var mode = jQuery('#withRequestType').val();
    jQuery('#withAmountViews').hide();
    jQuery('#withAmountShortcutViews').hide();
    if (mode == '51') {
        jQuery('#withAmountViews').show();
        jQuery('#withAmountShortcutViews').show();
    }
}

function ValidateCustomerv2Request() {
    jQuery('#withErrorMessage').html('');
    var bankName = jQuery('#withBankName').val();
    var mobileNo = jQuery('#withMobileNumber').val();
    var amount = jQuery('#withAmount').val();
    var deviceName = jQuery('#withDeviceName').val();
    var agentTC = jQuery('#withAgenttc').prop('checked');
    var customerTC = jQuery('#withCustomertc').prop('checked');
    var mode = jQuery('#withRequestType').val();
    var modeText = jQuery("#withRequestType option:selected").text();
    var aadharNo = '';
    for (i = 0; i < 12; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (mode == '51') {
        if (amount == '') {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000."); return false;
        } else if (amount <= 0 || amount > 10000) {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000."); return false;
        }
    } else {
        jQuery('#withAmount').val('');
        amount = '0';
    }
    if (bankName != '' && aadharNo != '' && aadharNo.length === 12 && mobileNo.length === 10 && deviceName != '' && mode != '') {
        if (agentTC && customerTC) {
            swal({ title: "Processing", text: "Please wait..", imageUrl: "/Content/Plugins/img/Processing.gif", showConfirmButton: false });
            jQuery.post("/AEPS/CashValidateV2Request", { mobileNumber: mobileNo, bankName: bankName, aadharNumber: aadharNo, amount: amount, mode: mode }, function (result) {
                if (result.StatusCode === 1) {
                    jQuery('#myModal').modal('show'); swal.close();
                    jQuery('#viewtransactiontype').html(modeText);
                    jQuery('#WithAadharNo').html('XXXXXXXX' + aadharNo.substring(8, 12));
                    if (mode == '51') {
                        jQuery('#transactionAmount').html(result.Data.Amount);
                        jQuery('#transactionAmountViews').show();
                    } else {
                        jQuery('#transactionAmountViews').hide();
                        jQuery('#transactionAmount').html('');
                    }
                } else if (result.StatusCode === 0) {
                    swal('', result.Message, 'error')
                } else {
                    swal('', result.Message, 'warning')
                }
            });
        } else {
            jQuery('#withErrorMessage').html("Terms and condition are required.");
        }
    } else {
        jQuery('#withErrorMessage').html("All field's are required.");
    }
}
function CaptureAvdmv2Request() {
    jQuery('#withErrorMessage').html('');
    var bankName = jQuery('#withBankName').val();
    var mobileNo = jQuery('#withMobileNumber').val();
    var amount = jQuery('#withAmount').val();
    var deviceName = jQuery('#withDeviceName').val();
    var agentTC = jQuery('#withAgenttc').prop('checked');
    var customerTC = jQuery('#withCustomertc').prop('checked');
    var mode = jQuery('#withRequestType').val();
    var aadharNo = '';
    for (i = 0; i < 12; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (mode == '51') {
        if (amount == '') {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000."); return false;
        } else if (amount <= 0 || amount > 10000) {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000."); return false;
        }
    } else {
        jQuery('#withAmount').val('');
    }
    if (bankName != '' && aadharNo != '' && aadharNo.length === 12 && mobileNo.length === 10 && deviceName != '' && mode != '') {
        if (agentTC && customerTC) {
            jQuery('#withPidData').val(''), jQuery('#withPidDataOption').val('');
            jQuery('#confirmationbox').hide(); jQuery('#confirmationboxfirnger').show();
            if (deviceName === 'MANTRA') {
                var SuccessFlag = 0;
                var primaryUrl = "http://127.0.0.1:";
                try {
                    var protocol = window.location.href;
                    if (protocol.indexOf("https") >= 0) {
                        primaryUrl = "http://127.0.0.1:";
                    }
                } catch (e) { }
                url = "";
                for (var i = 11100; i <= 11120; i++) {
                    if (primaryUrl == "https://127.0.0.1:" && OldPort == true) {
                        i = "8005";
                    }
                    var verb = "RDSERVICE";
                    var err = "";
                    SuccessFlag = 0;

                    $.support.cors = true;
                    var httpStaus = false;
                    var jsonstr = "";
                    var data = new Object();
                    var obj = new Object();
                    $.ajax({
                        type: "RDSERVICE",
                        async: false,
                        url: primaryUrl + i.toString(),
                        contentType: "text/xml; charset=utf-8",
                        processData: false,
                        cache: false,
                        crossDomain: true,
                        success: function (data) {
                            httpStaus = true;
                            res = { httpStaus: httpStaus, data: data };
                            finalUrl = primaryUrl + i.toString();
                            var $doc = $.parseXML(data);
                            var CmbData1 = $($doc).find('RDService').attr('status');
                            var CmbData2 = $($doc).find('RDService').attr('info');
                            if (RegExp('\\b' + 'Mantra' + '\\b').test(CmbData2) == true) {
                                //$("#txtDeviceInfo").val(data);

                                if ($($doc).find('Interface').eq(0).attr('path') == "/rd/capture") {
                                    MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                                }
                                if ($($doc).find('Interface').eq(1).attr('path') == "/rd/capture") {
                                    MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                                }
                                if ($($doc).find('Interface').eq(0).attr('path') == "/rd/info") {
                                    MethodInfo = $($doc).find('Interface').eq(0).attr('path');
                                }
                                if ($($doc).find('Interface').eq(1).attr('path') == "/rd/info") {
                                    MethodInfo = $($doc).find('Interface').eq(1).attr('path');
                                }

                                SuccessFlag = 1;
                                jQuery('#btnFingerConfirmation').hide(), jQuery('#btnFingerDismiss').hide();
                                CaptureAvdmcasv2Request();
                                return;
                            }
                        },
                        error: function (jqXHR, ajaxOptions, thrownError) {
                            if (i == "8005" && OldPort == true) {
                                OldPort = false;
                                i = "11099";
                            }
                        },

                    });
                    if (SuccessFlag == 1) {
                        break;
                    }
                }
                if (SuccessFlag == 0) {
                    alert("Connection failed Please try again."); jQuery('#myModal').modal('hide');
                    jQuery('#confirmationbox').show(); jQuery('#confirmationboxfirnger').hide();
                }
            } else {
                CaptureAvdmv2Requestv();
            }
        } else {
            jQuery('#withErrorMessage').html("Terms and condition are required.");
        }
    } else {
        jQuery('#withErrorMessage').html("All field's are required.");
    }
}
function CaptureAvdmcasv2Request() {
    var strWadh = "";
    var strOtp = "";
    Demo();
    var XML = '<?xml version="1.0"?> <PidOptions ver="1.0"> <Opts fCount="1" fType="0" iCount="0" pCount="0" pgCount="2"' + strOtp + ' format="0" pidVer="2.0" timeout="10000" pTimeout="20000"' + strWadh + ' posh="UNKNOWN" env="p" /> ' + DemoFinalString + '<CustOpts><Param name="mantrakey" value="" /></CustOpts> </PidOptions>';
    var verb = "CAPTURE";
    var err = "";
    var res;
    $.support.cors = true;
    var httpStaus = false;
    var jsonstr = "";
    $.ajax({
        type: "CAPTURE",
        async: true,
        crossDomain: true,
        url: finalUrl + MethodCapture,
        data: XML,
        contentType: "text/xml; charset=utf-8",
        processData: false,
        success: function (data) {
            httpStaus = true;
            res = { httpStaus: httpStaus, data: data };
            jQuery('#withPidData').val(data);
            jQuery('#withPidDataOption').val(XML);

            var $doc = $.parseXML(data);
            var Message = $($doc).find('Resp').attr('errInfo');
            if (Message == 'Success') {
                CaptureAvdmcasfinalv2Request();
            } else {
                alert(Message); location.reload();
            }
        },
        error: function (jqXHR, ajaxOptions, thrownError) {
            alert(thrownError);
            res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
        },
    });

    return res;
}

function CaptureAvdmv2Requestv() {
    var url = "http://127.0.0.1:11100";
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    jQuery('#btnFingerConfirmation').hide(), jQuery('#btnFingerDismiss').hide();
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }
    xhr.open('RDSERVICE', url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;
            if (status == 200) {
                var data = xhr.responseText;
                finalUrl = 'http:/';
                var $doc = $.parseXML(data);
                var CmbData2 = $($doc).find('RDService').attr('info');
                if (RegExp('\\b' + 'Morpho_RD_Service' + '\\b').test(CmbData2) == true) {
                    if ($($doc).find('Interface').eq(0).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                    }
                    if ($($doc).find('Interface').eq(1).attr('path') == "/127.0.0.1:11100/capture") {
                        MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                    }

                    CaptureAvdmcasv2Requestv();

                } else {
                    alert("Connection failed Please try again."); location.reload();
                }
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send();
}
function CaptureAvdmcasv2Requestv() {
    var url = finalUrl + MethodCapture;
    var PIDOPTS = '<PidOptions ver=\"1.0\">' + '<Opts fCount=\"1\" fType=\"0\" iCount=\"\" iType=\"\" pCount=\"\" pType=\"\" format=\"0\" pidVer=\"2.0\" timeout=\"10000\" otp=\"\" wadh=\"\" posh=\"\"/>' + '</PidOptions>';
    var xhr;
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        xhr = new ActiveXObject("Microsoft.XMLHTTP");
    } else {
        xhr = new XMLHttpRequest();
    }

    xhr.open('CAPTURE', url, true);
    xhr.setRequestHeader("Content-Type", "text/xml");
    xhr.setRequestHeader("Accept", "text/xml");

    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            var status = xhr.status;
            if (status == 200) {
                jQuery('#withPidData').val(xhr.responseText);
                jQuery('#withPidDataOption').val(PIDOPTS);
                CaptureAvdmcasfinalv2Request();
            } else {
                alert(xhr.response); location.reload();
            }
        }
    };
    xhr.send(PIDOPTS);
}
function CaptureAvdmcasfinalv2Request() {
    var bankName = jQuery('#withBankName').val();
    var mobileNo = jQuery('#withMobileNumber').val();
    var amount = jQuery('#withAmount').val();
    var pidData = jQuery('#withPidData').val();
    var mode = jQuery('#withRequestType').val();
    var modeText = jQuery("#withRequestType option:selected").text();
    jQuery('#responsePrint').show();
    var aadharNo = '';
    for (i = 0; i < 12; i++) {
        aadharNo += document.getElementById(parseInt(i) + 1).value;
    }
    if (mode == '51') {
        if (amount == '') {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000."); return false;
        } else if (amount <= 0 || amount > 10000) {
            jQuery('#withErrorMessage').html("Amount should be between 1 to 10000."); return false;
        }
    } else {
        jQuery('#withAmount').val('');
        amount = '0';
    }
    jQuery.post("/AEPS/CashWithdrawalv2Request", { mobileNumber: mobileNo, bankName: bankName, aadharNumber: aadharNo, biometricData: pidData, amount: amount, mode: mode }, function (result) {
        if (result.StatusCode === 1) {
            if (mode == '51') {
                jQuery('#myModal').modal('hide');
                jQuery('#ResponseModal').modal('show');
                jQuery('#responseIcon').attr('src', '/Content/Plugins/img/success.png');
                jQuery('#responseStatus').html('SUCCESS');
                jQuery('#responseMessage').html(result.Message);
                jQuery('#responseNumber').html(mobileNo);
                jQuery('#responseAmount').html(amount);
                jQuery('#responseRRN').html(result.Data.RRN);
                jQuery('#responseUID').html(result.Data.AdhaarNo);
                jQuery('#responseAvBalance').html(result.Data.AvailableBalance);
                jQuery('#responsePrint').attr('href', '/AEPS/Receipt/' + result.RefNumber);
                ResetWithData();
            } else if (mode == '52') {
                jQuery('#myModal').modal('hide');
                jQuery('#InqResponseModal').modal('show');
                jQuery('#responseInqIcon').attr('src', '/Content/Plugins/img/success.png');
                jQuery('#responseInqStatus').html('SUCCESS');
                jQuery('#responseInqMessage').html(result.Message);
                jQuery('#responseInqNumber').html(mobileNo);
                jQuery('#responseInqRRN').html(result.Data.RRN);
                jQuery('#responseInqUID').html(result.Data.AdhaarNo);
                jQuery('#responseInqAvBalance').html(result.Data.AvailableBalance);
                jQuery('#withPidData').val(''), jQuery('#withPidDataOption').val(''); jQuery('#confirmationbox').show(); jQuery('#confirmationboxfirnger').hide(); jQuery('#btnFingerConfirmation').show(), jQuery('#btnFingerDismiss').show();
            } else if (mode == '53') {
                jQuery('#myModal').modal('hide');
                jQuery('#MiniResponseModal').modal('show');
                jQuery('#responseMiniIcon').attr('src', '/Content/Plugins/img/statement.png');
                jQuery('#responseMiniStatus').html('AEPS MiniStatement');
                jQuery('#responseMiniMessage').html(result.Message);
                jQuery('#responseMiniDate').html(result.Data.localDate + ' ' + result.Data.localTime);
                jQuery('#responseMiniBankName').html(bankNames);
                jQuery('#responseMiniNumber').html(mobileNo);
                jQuery('#responseMiniAmount').html(result.Data.Balance);
                jQuery('#responseMiniRRN').html(result.Data.RRN);
                jQuery('#responseMiniUID').html(result.Data.AdhaarNo);
                jQuery('#responseMiniPrint').attr('href', '/AEPS/Statement/' + result.RefNumber);
                jQuery('#responseMiniPrint').show();
                ResetWithData();
            } else {
                //
            }
        } else if (result.StatusCode === 0) {
            jQuery('#myModal').modal('hide');
            jQuery('#ResponseModal').modal('show');
            jQuery('#responseIcon').attr('src', '/Content/Plugins/img/fail.png');
            jQuery('#responseStatus').html('FAILED');
            jQuery('#responseMessage').html(result.Message);
            jQuery('#responseNumber').html(mobileNo);
            if (mode == '51') {
                jQuery('#responseAmount').html(amount);
                jQuery('#responseAmountViews').show();
            } else {
                jQuery('#responseAmount').html('');
                jQuery('#responseAmountViews').hide();
            }
            jQuery('#responseRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            if (mode == '53') {
                jQuery('#responseAvBalance').html(result.Data == null ? '' : result.Data.Balance);
            } else {
                jQuery('#responseAvBalance').html(result.Data == null ? '' : result.Data.AvailableBalance);
            }
            jQuery('#responsePrint').hide();
            ResetWithData();
        } else {
            jQuery('#myModal').modal('hide');
            jQuery('#ResponseModal').modal('show');
            jQuery('#responseIcon').attr('src', '/Content/Plugins/img/pending.png');
            jQuery('#responseStatus').html('PENDING');
            jQuery('#responseMessage').html(result.Message);
            jQuery('#responseNumber').html(mobileNo);
            if (mode == '51') {
                jQuery('#responseAmount').html(amount);
                jQuery('#responseAmountViews').show();
            } else {
                jQuery('#responseAmount').html('');
                jQuery('#responseAmountViews').hide();
            }
            jQuery('#responseRRN').html(result.Data == null ? '' : result.Data.RRN);
            jQuery('#responseUID').html(result.Data == null ? '' : result.Data.AdhaarNo);
            if (mode == '53') {
                jQuery('#responseAvBalance').html(result.Data == null ? '' : result.Data.Balance);
            } else {
                jQuery('#responseAvBalance').html(result.Data == null ? '' : result.Data.AvailableBalance);
            }
            jQuery('#responsePrint').hide();
            ResetWithData();
        }
        BindsData();
    });
}