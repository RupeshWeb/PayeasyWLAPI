/// <reference path="../system/angular.min.js" />
var rgapp = angular.module('rgapp', []);
rgapp.controller('MAINRGBS', ['$scope', '$http', function ($scope, $http) {
    resetmethod();
    Binddata();

    function Binddata() {
        $http({
            url: '/BillerCategories',
            method: 'POST'
        }).then(function successCallback(response) {
            if (response.data.StatusCode === 1) {
                $scope.categoriesdata = response.data.Data;
            } else {
                swal('', response.data.Message, 'error');
                $scope.categoriesdata = null;
            }
        }, function errorCallback(response) { alert(response.data); location.reload(); });
    }

    $scope.dobindopnamebycate = function (item) {
        $scope.subcategoriesdata = null; $scope.ErrorMessage = null;
        if (item.CategoryID) {
            jQuery('.category').removeClass('active');
            jQuery('#Temp' + item.CategoryID).addClass('active');
            $scope.stateNameList = null;
            $http({
                url: '/CategoriesBiller?categoryCode=' + item.CategoryID,
                method: 'POST'
            }).then(function successCallback(response) {
                if (response.data.StatusCode === 1) {
                    $scope.subcategoriesdata = response.data.Data;
                    $scope.stateNameList = response.data.States;
                    $scope.SelectedBillerImage = null;
                    $(".bank-details").hide();
                } else {
                    swal('', response.data.Message, 'error');
                    $scope.subcategoriesdata = null;
                }
            }, function errorCallback(response) { alert(response.data); location.reload(); });
        } else
            $scope.ErrorMessage = 'Select Category Name.';
    };

    $scope.dobindvalidateparam = function (item) {
       // alert(jQuery.find(".select2-selection__clear"));
        resetmethod();
        if (item) {
            jQuery('.subcategory').removeClass('active');
            jQuery('#Temp' + item.BillerID).addClass('active');

            $http({
                url: '/BillerParameters?billerCode=' + item.BillerID,
                method: 'POST'
            }).then(function successCallback(response) {
                if (response.data.StatusCode === 1) {
                    $scope.Billerparamdata = response.data.Data; var cnt = 0;
                    response.data.Data.forEach(function (itemdata) {
                        if (cnt === 0) {
                            $(".bank-details").show();
                            $scope.maintransnumber = itemdata.Name;
                            $scope.maintransnumbermaxlen = itemdata.MaxLength;
                            $scope.maintransnumberminlen = itemdata.MinLenght;
                            itemdata.Ismandatory === true ? (jQuery('#MainNumber').attr('required', 'required')) : (jQuery('#MainNumber').removeAttr('required'));
                            itemdata.FieldType === 'NUMERIC' ? (jQuery('#MainNumber').attr('onkeypress', 'return ValidateNumber(event);')) : (jQuery('#MainNumber').removeAttr('onkeypress'));
                            $scope.maintransnumberregpattern = itemdata.Pattern;
                            //itemdata.FieldType == 'NUMERIC' ? (jQuery('#MainNumber').attr('onkeypress', 'return ValidateNumber(event);')) : (jQuery('#MainNumber').removeAttr('onkeypress'))
                        } else if (cnt === 1) {
                            $(".bank-details").show();
                            if (!itemdata.HasGrouping) {
                                $scope.mainaccountlabel = itemdata.Name;
                                itemdata.MaxLength > 1 ? (jQuery('#MainAccount_Number').attr('maxlength', itemdata.MaxLength)) : (jQuery('#MainAccount_Number').removeAttr('maxlength'));
                                itemdata.MinLenght !== 0 ? (jQuery('#MainAccount_Number').attr('minlength', itemdata.MinLenght)) : (jQuery('#MainAccount_Number').removeAttr('minlength'));
                                itemdata.Ismandatory === true ? (jQuery('#MainAccount_Number').attr('required', 'required')) : (jQuery('#MainAccount_Number').removeAttr('required'));
                                itemdata.FieldType === 'NUMERIC' ? (jQuery('#MainAccount_Number').attr('onkeypress', 'return ValidateNumber(event);')) : (jQuery('#MainAccount_Number').removeAttr('onkeypress'));
                                itemdata.Pattern !== ' ' ? (jQuery('#MainAccount_Number').attr('pattern', itemdata.Pattern)) : (jQuery('#MainAccount_Number').removeAttr('pattern')); $scope.accountnumverview = false;
                            } else {
                                $scope.mainaccountlabels = itemdata.Name;
                                $scope.accountnumverviews = false;

                                $http({
                                    url: '/BillerParameterGrouping?billerCode=' + item.BillerID + '&billerParam=' + itemdata.RefNumber,
                                    method: 'POST'
                                }).then(function successCallback(response) {
                                    if (response.data.StatusCode === 1) {
                                        $scope.subParamterGroupdata = response.data.Data;
                                    } else {
                                        swal('', response.data.Message, 'error');
                                        $scope.subParamterGroupdata = null;
                                    }
                                }, function errorCallback(response) { alert(response.data); });
                            }
                        } else if (cnt === 2) {
                            $(".bank-details").show();
                            if (!itemdata.HasGrouping) {
                                $scope.maincyclelabel = itemdata.Name;
                                itemdata.MaxLength > 1 ? (jQuery('#MainCycle_Number').attr('maxlength', itemdata.MaxLength)) : (jQuery('#MainCycle_Number').removeAttr('maxlength'));
                                itemdata.MinLenght !== 0 ? (jQuery('#MainCycle_Number').attr('minlength', itemdata.MinLenght)) : (jQuery('#MainCycle_Number').removeAttr('minlength'));
                                itemdata.Ismandatory === true ? (jQuery('#MainCycle_Number').attr('required', 'required')) : (jQuery('#MainCycle_Number').removeAttr('required'));
                                itemdata.FieldType === 'NUMERIC' ? (jQuery('#MainCycle_Number').attr('onkeypress', 'return ValidateNumber(event);')) : (jQuery('#MainCycle_Number').removeAttr('onkeypress'));
                                itemdata.Pattern !== ' ' ? (jQuery('#MainCycle_Number').attr('pattern', itemdata.Pattern)) : (jQuery('#MainCycle_Number').removeAttr('pattern')); $scope.cyclenumbernumverview = false;
                            } else {
                                $scope.maincyclelabels = itemdata.Name;
                                $scope.cyclenumbernumverviews = false;

                                $http({
                                    url: '/BillerParameterGrouping?billerCode=' + item.BillerID + '&billerParam=' + itemdata.RefNumber,
                                    method: 'POST'
                                }).then(function successCallback(response) {
                                    if (response.data.StatusCode === 1) {
                                        $scope.subAuthenticatorGroupdata = response.data.Data;
                                    } else {
                                        swal('', response.data.Message, 'error');
                                        $scope.subAuthenticatorGroupdata = null;
                                    }
                                }, function errorCallback(response) { alert(response.data); });
                            }
                        }
                        cnt++;
                    });
                    $scope.Biller_Name = item;
                    $scope.SelectedBillerImage = item.ImageUrl;
                    if (item.StateName) {
                        //$('#billerStateName').val(item.StateName);
                        //$('#billerStateName').trigger('change');
                        //var $example = $(".billerStateName").select2().val(item.StateName);
                        //$example.val(item.StateName).trigger("select");
                        //$('#billerStateName').off('select2:select');

                        jQuery('#billerStateName').val(item.StateName);
                        $scope.billerStateName = item.StateName;
                        jQuery('#select2-billerStateName-container').html('<span class="select2-selection__clear" data-ng-click="rgclearstate()">×</span>' + item.StateName);
                        jQuery('#select2-billerStateName-container').attr('title', item.StateName);
                    } else {
                        $scope.billerStateName = null;
                        jQuery('#select2-billerStateName-container').html('<span class="select2-selection__placeholder">State</span>');
                        jQuery('#select2-billerStateName-container').removeAttr('title');
                    }
                    if ($scope.Biller_Name.BillerCode === 'BSLL') { $scope.pptypenumbernumverview = false; } if ($scope.Biller_Name.BillFetch === true) { $scope.btnpaymentprocess = true; $scope.btnfeachindvalidateinfo = false; $scope.payable_amount = true; } else { $scope.btnpaymentprocess = false; $scope.btnfeachindvalidateinfo = true; $scope.payable_amount = false; }
                } else {
                    swal('', response.data.Message, 'error');
                    $scope.Billerparamdata = null;
                    $scope.btnpaymentprocess = true; $scope.btnfeachindvalidateinfo = true;
                }
            }, function errorCallback(response) { alert(response.data); location.reload(); });
        } else
            $scope.ErrorMessage = 'Select Category or Subcategory Name.';
    };

    function resetmethod() {
        $scope.ErrorMessage = null; $scope.maintransnumber = 'Consumer Number'; $scope.payable_amount = true; $scope.maintransnumbermaxlen = '15'; $scope.maintransnumberminlen = '1'; $scope.maintransnumberregpattern = null;
        $scope.accountnumverview = true; $scope.accountnumverviews = true; $scope.cyclenumbernumverview = true; $scope.cyclenumbernumverviews = true; $scope.pptypenumbernumverview = true; $scope.confirmBillPayment = true;
        $scope.mainaccountlabel = 'Account Number'; $scope.maincyclelabel = 'Cycle';
        $scope.MainAccount_Number = null; $scope.MainCycle_Number = null; $scope.MainNumber = null;
        $scope.btnpaymentprocess = false; $scope.btnfeachindvalidateinfo = true; $scope.Pay_Amount = null; $scope.SelectedBillerImage = null; $scope.billerStateName = null;
        $scope.Retailer_MobileNumber = null; $scope.Pay_TPIN = null;
        $scope.shomainnumber = null, $scope.shopconsumername = null, $scope.shopduedate = null, $scope.shopbilldate = null, $scope.netpaymentamount = null, $scope.shopbillnumbers = null, $scope.Retailer_MobileNumber = null, $scope.shoppartial = null, $scope.netpaymentamount = null, $scope.BillerCodes = null, $scope.Pay_Amount = null, $scope.Retailer_MobileNumber = null, $scope.MainAccount_Number = null, $scope.authenticator = null, $scope.shopduedate = null, $scope.shopbilldate = null, $scope.showbillername = null, $scope.shopbillnumbers = null, $scope.refNumbers = null; $scope.confirmBillPayment = true;
        $scope.mainaccountlabels = null, $scope.maincyclelabels = null;
        $scope.subParamterGroupdata = null; $scope.subAuthenticatorGroupdata = null;
    }

    $scope.doforfeachingbalfor = function () {
        $scope.ErrorMessage = null;
        if ($scope.Biller_Name && $scope.MainNumber && $scope.Retailer_MobileNumber) {
            var currentnumber = ''; if ($scope.MainCycle_Number) { currentnumber = $scope.MainCycle_Number; } if ($scope.PostpaidLand_Type) { currentnumber = $scope.PostpaidLand_Type; }
            var accountNumber = $scope.MainAccount_Number;
            if ($scope.mainaccountlabels) {
                accountNumber = $scope.MainAccount_NumberGroup;
            }
            if ($scope.maincyclelabels) {
                currentnumber = $scope.MainCycle_NumberGroup;
            }
            swal({ title: "Processing", text: "Please wait..", imageUrl: "../Content/Plugins/img/Processing.gif", showConfirmButton: false });
            $http({
                url: '/BBPS/BillerValidate',
                method: 'POST',
                data: JSON.stringify({ billercode: $scope.Biller_Name.BillerID, customerNo: $scope.Retailer_MobileNumber, billnumber: $scope.MainNumber, billeraccount: accountNumber, billercycle: currentnumber })
            }).then(function successCallback(response) {
                if (response.data.StatusCode === 1) {
                    $scope.Pay_Amount = response.data.Data.Amount;
                    $scope.netpaymentamount = response.data.Data.Amount;
                    $scope.shomainnumber = $scope.MainNumber;
                    $scope.shopayamount = $scope.Pay_Amount;
                    $scope.shopduedate = response.data.Data.BillDueDate;
                    $scope.shopbilldate = response.data.Data.BillDate;
                    $scope.showbillername = $scope.Biller_Name.BillerName;
                    $scope.shobillerimage = '/images/Recharge/Operators/' + $scope.Biller_Name.Imageurl;
                    $scope.shopconsumername = response.data.Data.BillerName;
                    $scope.shopbillnumbers = response.data.Data.BillerNumber;
                    $scope.refNumbers = response.data.RefNumber;
                    $scope.authenticator = currentnumber;
                    $scope.BillerCodes = $scope.Biller_Name.BillerID;
                    $scope.confirmBillPayment = false;
                    $(".bank-details").hide();
                    if (response.data.Data.Partial == 'Y') {
                        jQuery('#netbillpayment').removeAttr('readonly');
                        $scope.shoppartial = 'YES';
                    } else {
                        jQuery('#netbillpayment').attr('readonly', 'readonly');
                        $scope.shoppartial = 'NO';
                    }
                    //jQuery('#Perconfirmmodal').modal('show');
                    swal.close();
                } else {
                    swal('', response.data.Message, 'error');
                }
            }, function errorCallback(response) { alert(response.data); location.reload(); });
        } else
            $scope.ErrorMessage = "All field's are required.";
    };

    $scope.dooutprocbillpay = function () {
        $scope.ErrorMessage = null;
        if ($scope.Biller_Name && $scope.MainNumber && $scope.Retailer_MobileNumber && $scope.Pay_Amount > 0) {
            var currentnumber = ''; if ($scope.MainCycle_Number) { currentnumber = $scope.MainCycle_Number; } if ($scope.PostpaidLand_Type) { currentnumber = $scope.PostpaidLand_Type; }
            var accountNumber = $scope.MainAccount_Number;
            if ($scope.mainaccountlabels) {
                accountNumber = $scope.MainAccount_NumberGroup;
            }
            if ($scope.maincyclelabels) {
                currentnumber = $scope.MainCycle_NumberGroup;
            }
            swal({ title: "Processing", text: "Please wait..", imageUrl: "../Content/Plugins/img/Processing.gif", showConfirmButton: false });
            $http({
                url: '/BBPS/BillPayment',
                method: 'POST',
                data: JSON.stringify({ billercode: $scope.Biller_Name.BillerID, billnumber: $scope.MainNumber, Amount: $scope.Pay_Amount, custmobileno: $scope.Retailer_MobileNumber, billeraccount: accountNumber, billercycle: currentnumber, payment: 'Cash', duedate: $scope.shopduedate, billdate: $scope.shopbilldate, consumername: $scope.shopconsumername, billnumbers: $scope.shopbillnumbers, referenceNo: $scope.refNumbers })
            }).then(function successCallback(response) {
                var jsonobj = response.data;
                if (jsonobj.StatusCode === 1) {
                    jQuery('#responseModal').modal('show');
                    jQuery('#printStatusImages').attr('src', '/Content/Plugins/bbps/images/bbps/success-blue.png');
                    jQuery('#printConsumerName').html('Hey, ' + $scope.shopconsumername);
                    jQuery('#printMessage').html('Your bill payment for <span class="text-primary">Rs.' + $scope.Pay_Amount + '</span> is Successful.');
                    jQuery('#printMessage1').html('Convenience fee collected Rs.25');
                    jQuery('#printMessage2').html('Your transaction ID <span class="text-primary">' + jsonobj.TxnRef + '</span>');
                    jQuery('#printMessage3').html('' + jsonobj.OperatorRef + ' ' + jsonobj.TxnDate);
                    jQuery('#btnPrint').attr('href', '/BBPS/Receipt/' + jsonobj.RefNumber);
                    jQuery('#btnPrint').html('<i class="mdi mdi-printer printer-w"></i>&nbsp;Print');
                    //swal({
                    //    title: '<small><div class="rg-alert-su">SUCCESS!</div></small>', text: '<div style="margin-left: 20px;text-align:left"> <b>Number :</b>' + $scope.MainNumber + '<br/><b>Amount :</b>' + $scope.Pay_Amount + '<br/><b>Message :</b>' + jsonobj.Message + '<br/></div>', html: true, type: 'success', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Print', closeOnConfirm: false
                    //}, function () { doshowprintforbbps(jsonobj.RefNumber); });
                    resetmethod();
                } else if (jsonobj.StatusCode === 0) {
                    jQuery('#responseModal').modal('show');
                    jQuery('#printStatusImages').attr('src', '/Content/Plugins/bbps/images/bbps/failure.png');
                    jQuery('#printConsumerName').html('Hey, ' + $scope.shopconsumername);
                    jQuery('#printMessage').html('Your bill payment for <span class="text-primary">Rs.' + $scope.Pay_Amount + '</span> is Failed.');
                    jQuery('#printMessage1').html('');
                    jQuery('#printMessage2').html('Your transaction ID <span class="text-primary">' + jsonobj.TxnRef + '</span>');
                    jQuery('#printMessage3').html('' + jsonobj.OperatorRef + ' ' + jsonobj.TxnDate);
                    jQuery('#btnPrint').attr('data-dismiss', 'modal');
                    jQuery('#btnPrint').html('Ok');
                    resetmethod();
                } else {
                    jQuery('#responseModal').modal('show');
                    jQuery('#printStatusImages').attr('src', '/Content/Plugins/bbps/images/bbps/Pending.png');
                    jQuery('#printConsumerName').html('Hey, ' + $scope.shopconsumername);
                    jQuery('#printMessage').html('Your bill payment for <span class="text-primary">Rs.' + $scope.Pay_Amount + '</span> is Pending.');
                    jQuery('#printMessage1').html('Convenience fee collected Rs.25');
                    jQuery('#printMessage2').html('Your transaction ID <span class="text-primary">' + jsonobj.TxnRef + '</span>');
                    jQuery('#printMessage3').html('' + jsonobj.Message + ' ' + jsonobj.TxnDate);
                    jQuery('#btnPrint').attr('href', '/BBPS/Receipt/' + jsonobj.RefNumber);
                    jQuery('#btnPrint').html('<i class="mdi mdi-printer printer-w"></i>&nbsp;Print');
                    resetmethod();
                }
                swal.close();
            }, function errorCallback(response) { alert(response.data); location.reload(); });
        } else
            $scope.ErrorMessage = "All field's are required.";
    };

    function doshowprintforbbps(itemvalue) {
        if (itemvalue != '')
            location.href = 'Receipt/' + itemvalue;
        //$.post("/Recharge/GetTransactiondetailsPay", { TranId: itemvalue }, function (item) {
        //    showprtreciept(item);
        //}).error(function (result) { alert(result.responseText); location.reload(); });
    }

    function showprtreciept(itemval) {
        jQuery('#prt_operatorname').html(itemval.Operator);
        jQuery('#prt_operator_number').html(itemval.Operatornumber);
        jQuery('#prt_transactionid').html(itemval.Trxnid);
        jQuery('#prt_trxn_head_id').html(itemval.Trxnid);
        jQuery('#prt_InvoiceId').html(itemval.Trxnid);
        jQuery('#prt_operatorref').html(itemval.Operatorref);
        jQuery('#prt_transaction_date').html(parseJsonDate(itemval.Trxndate));
        jQuery('#prt_trxn_amount').html(itemval.Billamount);
        jQuery('#hdamt').val(itemval.Billamount);
        jQuery('#prt_total_amount').html(itemval.Billamount);
        swal.close();
        jQuery('#myReciept').modal('show');
    }

    function validateofflineeingmbill(itemx) {
        var clength = jQuery(itemx).val();
        if (clength > 0) {
            jQuery('#bl_pay_off_view').html(clength);
            jQuery('#btn_request_blr_off').removeAttr('disabled');
        } else {
            jQuery('#btn_request_blr_off').attr('disabled', 'disabled');
        }
    }

    function parseJsonDate(jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    }

    $scope.rgclearstate = function () {
        $scope.billerStateName = null;
        jQuery('#select2-billerStateName-container').html('<span class="select2-selection__placeholder">State</span>');
        jQuery('#select2-billerStateName-container').removeAttr('title');
    }

    //$scope.stateIncludes = [];
    $scope.showSelectState = function () {
        if ($scope.searchStateName) {
            $scope.search = $scope.searchStateName;
        } else {
            $scope.search = null;
        }
    }

    //$scope.showFilterData = function () {
    //    if ($scope.mySearch) {
    //        alert($scope.mySearch);
    //        $scope.myfilters.BillerName = $scope.mySearch;
    //    } else {
    //        $scope.myfilters = null;
    //    }
    //}

}]);