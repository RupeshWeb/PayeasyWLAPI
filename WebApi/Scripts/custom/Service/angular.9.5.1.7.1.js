Bindcategories();

function Bindcategories() {
    $.post({
        url: '/BillerCategories',
        method: 'POST'
    }).then(function successCallback(response) {
        if (response.StatusCode === 1) {
            var strcat = '';
            jQuery.each(response.Data, function () {
                strcat += '<li class=""><a href="javascript:void(0);" onclick="BindSubCategory(' + this.CategoryID + ');">' + this.CategoryName + '</a></li>';
            });
            jQuery('#bbps_categories').html(strcat);
        } else {
            swal('', response.Message, 'error');
            $scope.categoriesdata = null;
        }
    }, function errorCallback(response) { alert(response.data); location.reload(); });
}

function BindSubCategory(itemval) {

    resetreqaction();
    jQuery('#prereq_category_view_tab').hide();
    $.post({
        url: '/CategoriesBiller?categoryCode=' + itemval,
        method: 'POST'
    }).then(function successCallback(response) {
        if (response.StatusCode === 1) {
            var strcat = '';
            jQuery.each(response.Data, function () {
                strcat += '<li class=""><a href="javascript:void(0);" onclick="BindValidateFields(&#039;' + this.BillerID + '|' + this.BillerName + '|' + this.BillerName + '|' + this.BillFetch + '&#039;);">' + this.BillerName + '</a></li>';//<br /><span>' + this.BillerName + '</span>
            });
            jQuery('#bbps_sub_categories').html(strcat);
            jQuery('#sub_category_view_tab').show();
            if (itemval == 8) { jQuery('#blr_paymentr_amt_view_tab').show(); jQuery('#btn_request_blr_off').show(); jQuery('#btn_request_blr').hide(); jQuery('#btn_request_blr_off').attr('disabled', 'disabled'); } else { jQuery('#blr_paymentr_amt_view_tab').hide(); jQuery('#btn_request_blr_off').hide(); jQuery('#btn_request_blr').show(); jQuery('#btn_request_blr').attr('disabled', 'disabled'); }
        } else {
            jQuery('#sub_category_view_tab').hide();
            swal('', response.Message, 'error');
        }
    }, function errorCallback(response) { alert(response.data); location.reload(); });
}

function resetreqaction() {
    jQuery('#blr_request_otherparam_view_tab').hide();
    jQuery('#blr_request_number').val('');
    jQuery('#blr_request_otherparam').val('');
    jQuery('#blr_request_mobile_number').val('');

    jQuery('#blr_request_otherparam3').val('');
    jQuery('#blr_request_otherparam4').val('');
    jQuery('#blr_request_otherparam5').val('');

    jQuery('#blr_request_amount_of').val('');
    jQuery('#blr_bill_paymt_view_tab').hide();
}


function BindValidateFields(itemval) {

    var line = itemval.split('|');
    var billername = itemval.split('|')[1];
    var billernum = itemval.split('|')[0];
    var billercode = itemval.split('|')[2];
    var Ismandatory = itemval.split('|')[3];
    jQuery('#prereq_category_view_tab').show();
    jQuery('#blr_bill_fetch_view_tab').show();
    jQuery('#blr_bill_paymt_view_tab').hide();
    jQuery('#brl_request_head_view').html(billername);
    jQuery('#brlname').val(billername);
    jQuery('#blr_request_blrnumber').val(billernum);
    jQuery('#operators').val(billercode);
    DoValidateFields(billernum, Ismandatory);
}

function DoValidateFields(itemval, Ismandatory) {
    resetreqaction();
    jQuery.ajax({
        type: 'POST',
        url: '/BillerParameters?billerCode=' + itemval,
        data: { billerCode: itemval },
        success: function (result) {
            jQuery('#blr_paymentr_mode_view_tab').hide();
            if (result.StatusCode === 1) {
                $("#bbpslogo").attr("src", "/images/bbps.jpg");
                $("#bbpslogo").show();
                if (Ismandatory == "true") {
                    jQuery('#btn_request_blr').removeAttr('disabled');
                    jQuery('#btn_request_blr_off').attr('disabled', 'disabled');
                } else {
                    jQuery('#btn_request_blr').attr('disabled', 'disabled');
                    jQuery('#btn_request_blr_off').removeAttr('disabled');
                    jQuery('#btn_request_blr').hide();
                    jQuery('#btn_request_blr_off').show();
                    jQuery('#blr_paymentr_amt_view_tab').show();
                    jQuery('#btn_request_blr_off').show();
                    jQuery('#blr_paymentr_mode_view_tab').show();
                }
                var cnt = 0;
                jQuery.each(result.Data, function () {
                    if (this.IsMandatory == true) {
                        if (cnt == 0) {
                            jQuery('#blr_request_number').attr('placeholder', this.Name);
                            jQuery('#blr_request_number_view').html(this.Name);
                            jQuery('#blr_request_number').attr('minlength', this.MinLenght);
                            if (this.MaxLength > 0) {
                                jQuery('#blr_request_number').attr('maxlength', this.MaxLength);
                                jQuery('#blr_request_number').attr('data-maxlength', this.MaxLength);
                            }
                            else
                                jQuery('#blr_request_number').attr('maxlength', 100);
                            if (this.FieldType == 'NUMERIC') { jQuery('#blr_request_number').attr('onkeypress', 'return ValidateNumber(event);'); } else if (this.FieldType == 'ALPHANUMERIC') { jQuery('#blr_request_number').removeAttr('onkeypress'); } else { jQuery('#blr_request_number').attr('onkeypress', 'return ValidateAlphabetSpace(event);'); }
                        }
                        else if (cnt == 1) {
                            jQuery('#blr_request_otherparam_view_tab').show();
                            jQuery('#blr_request_otherparam').attr('placeholder', this.Name);
                            jQuery('#blr_request_otherparam_view').html(this.Name);
                            jQuery('#blr_request_otherparam').attr('minlength', this.MinLenght);
                            if (this.MaxLength > 0)
                                jQuery('#blr_request_otherparam').attr('maxlength', this.MaxLength);
                            else
                                jQuery('#blr_request_otherparam').attr('maxlength', 100);
                            if (this.FieldType == 'NUMERIC') { jQuery('#blr_request_otherparam').attr('onkeypress', 'return ValidateNumber(event);'); } else if (this.FieldType == 'ALPHANUMERIC') { jQuery('#blr_request_otherparam').removeAttr('onkeypress'); } else { jQuery('#blr_request_otherparam').attr('onkeypress', 'return ValidateAlphabetSpace(event);'); }
                        }
                        else if (cnt == 2) {
                            jQuery('#blr_request_otherparam3_view_tab').show();
                            jQuery('#blr_request_otherparam3').attr('placeholder', this.Name);
                            jQuery('#blr_request_otherparam3_view').html(this.Name);
                            jQuery('#blr_request_otherparam3').attr('minlength', this.MinLenght);
                            if (this.MaxLength > 0)
                                jQuery('#blr_request_otherparam3').attr('maxlength', this.MaxLength);
                            else
                                jQuery('#blr_request_otherparam3').attr('maxlength', 100);
                            if (this.FieldType == 'NUMERIC') { jQuery('#blr_request_otherparam3').attr('onkeypress', 'return ValidateNumber(event);'); } else if (this.FieldType == 'ALPHANUMERIC') { jQuery('#blr_request_otherparam3').removeAttr('onkeypress'); } else { jQuery('#blr_request_otherparam3').attr('onkeypress', 'return ValidateAlphabetSpace(event);'); }
                        }
                        else if (cnt == 3) {
                            jQuery('#blr_request_otherparam4_view_tab').show();
                            jQuery('#blr_request_otherparam4').attr('placeholder', this.Name);
                            jQuery('#blr_request_otherparam4_view').html(this.Name);
                            jQuery('#blr_request_otherparam4').attr('minlength', this.MinLenght);
                            if (this.MaxLength > 0)
                                jQuery('#blr_request_otherparam4').attr('maxlength', this.MaxLength);
                            else
                                jQuery('#blr_request_otherparam4').attr('maxlength', 100);
                            if (this.FieldType == 'NUMERIC') { jQuery('#blr_request_otherparam4').attr('onkeypress', 'return ValidateNumber(event);'); } else if (this.FieldType == 'ALPHANUMERIC') { jQuery('#blr_request_otherparam4').removeAttr('onkeypress'); } else { jQuery('#blr_request_otherparam4').attr('onkeypress', 'return ValidateAlphabetSpace(event);'); }
                        }
                        else if (cnt == 4) {
                            jQuery('#blr_request_otherparam5_view_tab').show();
                            jQuery('#blr_request_otherparam5').attr('placeholder', this.Name);
                            jQuery('#blr_request_otherparam5_view').html(this.Name);
                            jQuery('#blr_request_otherparam5').attr('minlength', this.MinLenght);
                            if (this.MaxLength > 0)
                                jQuery('#blr_request_otherparam5').attr('maxlength', this.MaxLength);
                            else
                                jQuery('#blr_request_otherparam5').attr('maxlength', 100);
                            if (this.FieldType == 'NUMERIC') { jQuery('#blr_request_otherparam5').attr('onkeypress', 'return ValidateNumber(event);'); } else if (this.FieldType == 'ALPHANUMERIC') { jQuery('#blr_request_otherparam5').removeAttr('onkeypress'); } else { jQuery('#blr_request_otherparam5').attr('onkeypress', 'return ValidateAlphabetSpace(event);'); }
                        }
                        else {
                            //op
                        }
                        cnt++;
                    }
                });

            } else {
                swal('', result.Message, 'error');
            }
        }, error: function (result) {
            alert(result.responseText);
            location.reload();
        }, async: true
    });


    jQuery(function () {
        jQuery('#btn_request_blr').click(function () {

            var cnumberView = document.getElementById('blr_request_number_view').innerHTML;
            var cparamView = document.getElementById('blr_request_otherparam_view').innerHTML;
            var cparam3View = document.getElementById('blr_request_otherparam3_view').innerHTML;
            var cparam4View = document.getElementById('blr_request_otherparam4_view').innerHTML;
            var cparam5View = document.getElementById('blr_request_otherparam5_view').innerHTML;


            var cblenumber = jQuery('#blr_request_blrnumber').val();
            var cnumber = jQuery('#blr_request_number').val() + '$' + cnumberView;
            var cparam = jQuery('#blr_request_otherparam').val() + '$' + cparamView;
            var cparam3 = jQuery('#blr_request_otherparam3').val() + '$' + cparam3View;
            var cparam4 = jQuery('#blr_request_otherparam4').val() + '$' + cparam4View;
            var cparam5 = jQuery('#blr_request_otherparam5').val() + '$' + cparam5View;
            var cmobilenumber = jQuery('#blr_request_mobile_number').val();

            if (cnumber != '' && cmobilenumber.length == 10) {
                swal({ title: "Processing", text: "Please wait..", imageUrl: "../Content/Plugins/img/Processing.gif", showConfirmButton: false });
                jQuery.ajax({
                    type: 'POST',
                    url: '/AuthServices/BillerDetails?billerid=' + cblenumber + '&mobile=' + cmobilenumber + '&number=' + cnumber + '&param1=' + cparam + '&param2=' + cparam3 + '&param3=' + cparam4 + '&param4=' + cparam5 + '&param5=',
                    success: function (result) {
                        if (result.StatusCode === 1) {
                            var response = result.Data.billFetchResponse;
                            var adinfo = '';
                            jQuery.each(response.additionalInfo.info, function () {
                                adinfo += '<tr><td>' + this.infoName + '</td><td>' + this.infoValue + '</td></tr>';
                            });
                            var blinfo = '<tr><td>Amount Option</td><td><select onchange="getval(this);"><option>Select</option><option value=' + ParseToDecimal(response.billerResponse.billAmount) + '>AMOUNT PAYABLE:' + ParseToDecimal(response.billerResponse.billAmount) + '</option>';
                            jQuery.each(response.billerResponse.amountOptions.option, function () {
                                blinfo += '<option Value=' + ParseToDecimal(this.amountValue) + '>' + this.amountName + ':' + ParseToDecimal(this.amountValue) + '</option>';
                            });
                            blinfo += '</select></td></tr><tr><td>Payment Mode</td><td>Cash</td></tr>';
                            var htmlstr = '<tbody><tr><td>Customer Name</td><td>' + response.billerResponse.customerName + '</td></tr><tr><td>Customer Number</td><td>' + cmobilenumber + '</td></tr><tr><td>Bill Date</td><td>' + response.billerResponse.billDate + '</td></tr><tr><td>Bill Period</td><td>' + response.billerResponse.billPeriod + '</td></tr><tr><td>Bill Number</td><td>' + response.billerResponse.billNumber + '</td></tr><tr><td>Due Date</td><td>' + response.billerResponse.dueDate + '</td></tr><tr><td>Bill Amount</td><td>' + ParseToDecimal(response.billerResponse.billAmount) + '</td></tr><tr><td>Convenience Fees</td><td>0.00</td></tr><tr><td>Total Amount</td><td>' + ParseToDecimal(response.billerResponse.billAmount) + '</td></tr>' + blinfo + '</tbody >';
                            jQuery('#Fetch_bill_details_view').html(htmlstr);

                            jQuery('#bl_pay_view').html('0');
                            // jQuery('#fin_tran_refid').val(response.TransactionId);                  
                            jQuery('#fin_blr_amount').val(ParseToDecimal(response.billerResponse.billAmount));
                            jQuery('#number').val(cnumber);
                            jQuery('#mobileno').val(cmobilenumber);
                            jQuery('#billercode').val(cblenumber);
                            jQuery('#blr_bill_fetch_view_tab').hide();
                            jQuery('#blr_bill_paymt_view_tab').show();
                            jQuery('#btn_payment_blr').removeAttr('disabled');
                            jQuery('#param').val(cparam);
                            getTimer();
                            swal.close();
                        } else {
                            jQuery('#Fetch_bill_details_view').html('');
                            swal('', result.Message, 'error');
                        }
                    }, error: function (result) {
                        swal.close();
                        alert(result.responseText);
                        location.reload();
                    }, async: true
                });
            } else {
                swal('', 'All fields are required.', 'error');
            }
        });
    });
}