
jsonObj = [];
$(document).ready(function () {
    $(".fromDate").datepicker();
    $(".toDate").datepicker();
});
$(document).on("click", "#selectUsers", function (event) {
    jsonObj = [];
    var subscribedProductIds = [];
    var subscribedCheckBoxIds = [];
    if ($('input[name=selectProducts]:checked').length) {
        $('input[name=selectProducts]:checked').each(function () {
            var id = "#subscribeModel_" + $(this).attr("productid");
            var checkBoxId = "#product_" + $(this).attr("productid");
            subscribedProductIds[$(id).val()] = id;
            subscribedCheckBoxIds.push(checkBoxId);
            if ($(id).val() == 0)
            {
                $(id).addClass("errorMessageDropDown");
            }
            else
            {
                item = {}
                var nameid = "#productName_" + $(this).attr("productid");
                item["id"] = $(this).attr("productid");
                item["name"] = $(nameid).val();
                item["subscription"] = $(id).val();
                var priceIdPerUser = "";
                var priceIdSubsrciption = "";
                if ($(id).val() == 1)
                {
                    priceid = "#goldValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#goldMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 2)
                {
                    priceid = "#goldValuePerUserYearly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#goldYearly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 3) {
                    priceid = "#silverValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#silverMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 4) {
                    priceid = "#silverValuePerUserYearly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#silverYearly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 5) {
                    priceid = "#premiumValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#premiumMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 6) {
                    priceid = "#premiumValuePerUserYearly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#premiumYearly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 7) {
                    priceid = "#basicValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#basicMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 8) {
                    priceid = "#basicValuePerUserYearly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#basicYearly_" + $(this).attr("productid");
                }
                item["subscriptionPrice"] = $(priceIdSubsrciption).val();
                item["perUserPrice"] = $(priceid).val();
                jsonObj.push(item);
                $(id).removeClass("errorMessageDropDown");
            }
        }); 
        sessionStorage.setItem('productDetails', JSON.stringify(jsonObj));
        $.cookie("selectedUsers", null, { path: '/' });
        $.cookie('subscribedProductIds', JSON.stringify(subscribedProductIds), { path: '/' });
        $.cookie('subscribedCheckBoxIds', JSON.stringify(subscribedCheckBoxIds), { path: '/' });
        window.location.href = '/Credit/SelectUsers';
    }
    else {
        var opt = {
            autoOpen: false,
            modal: true,
            width: 550,
            title: 'Error',
            resizable: false,
            buttons: {
                "Ok": function () {
                    $(this).dialog("close");
                }
            }
        };
        var theDialog = $("#dialog-warning").dialog(opt);
        theDialog.dialog("open");
        //$("#failureMsg").hide();
       // alert("Please select a product");
        return false
    }
});

$(document).on("click", "#payment", function (event) {
    var error = 0; 
    $('input[type=text]').each(function () {
        if ($(this).hasClass('hasDatepicker')) {
            if ($(this).val() == "") {
                $(this).addClass("errorMessageDropDown");
                error = 1;
            }
            else {
                $(this).removeClass("errorMessageDropDown");
            }
        }
    });
    if(error)
    {
        return false;
    } 
    var purchaseDetails = [];
    var fromDates = [];
    var temp = sessionStorage.getItem('productDetails');
    var productDetails = $.parseJSON(temp);
    var ProdUsers = sessionStorage.getItem('jsonObjProdUsers');
    var jsonProdUsers = $.parseJSON(ProdUsers);
    $.each(productDetails, function (key, value) {
        item = {};
        item['productId'] = value.id;
        item['subsriptionType'] = value.subscription;
        var fromDateId = "#productStartDate_" + value.id;
        item['fromDate'] = $(fromDateId).val();
        fromDates[value.id] = $(fromDateId).val();

        //var toDateId = "#productEndDate_" + value.id;
        //item['toDate'] = $(toDateId).val();
        var userId = "";
        $.each(jsonProdUsers, function (key1, value1) {
            if (value1.productId == value.id) {
                if (userId != "")
                {
                    userId += "$" + value1.userId;
                }
                else
                {
                    userId = value1.userId;
                }
                
            }
        });
        item['userIds'] = userId;
        purchaseDetails.push(item);
    });
    $.cookie('fromDates', JSON.stringify(fromDates), { path: '/' });
    console.log("purchaseDetails"); console.log(purchaseDetails);
    var dataPur = JSON.stringify(purchaseDetails);
    $('#sl-loadingscreen').show();
    $.ajax({
        type: "POST",
        url: "/api/Products/PurchaseProductDetails",
        data: dataPur,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
            $("#custom").val(response);
            $("#paymentForm").submit();
            $('#sl-loadingscreen').hide();
            //alert(response);
            //$("#successMessageAddedNewUser").show();
        },
        error: function (x, y, z) {
            $('#sl-loadingscreen').hide();
            //$("#errorsMessageAddedNewUser").show();
        }
    });
});
$(document).on("click", "#summary", function (event) {
    var selectedUsers = [];
    jsonObjProdUsers = [];
    var temp = sessionStorage.getItem('productDetails');
    if ($('input[class=checkthis]:checked').length)
    {
        $('input[class=checkthis]:checked').each(function () {
            item = {};
            item["productId"] = $(this).attr("productId");
            item["userId"] = $(this).attr("userId");
            selectedUsers.push($(this).attr("id"));
            jsonObjProdUsers.push(item);
        });
        
        sessionStorage.setItem('jsonObjProdUsers', JSON.stringify(jsonObjProdUsers));
    } 
    $.cookie('selectedUsers', JSON.stringify(selectedUsers), { path: '/' });
    $.cookie("fromDates", null, { path: '/' });
    window.location.href = '/Credit/Summary';
});
$(document).ready(function () {
    var url = window.location.href; 
    var host = window.location.host; 
    if (url.indexOf('http://' + host + '/Credit') == -1 &&
        url.indexOf('http://' + host + '/Credit/SelectUsers') == -1 &&
        url.indexOf('http://' + host + '/Credit/Summary') == -1) {
        $.cookie("subscribedProductIds", null, { path: '/' });
        $.cookie("subscribedCheckBoxIds", null, { path: '/' });
    }
    if (url.indexOf('http://' + host + '/Credit') !== -1) {
        if ($.cookie('subscribedProductIds')) {
            var subscribedProductIds = $.parseJSON($.cookie('subscribedProductIds'));
            if (subscribedProductIds)
            {
                $.each(subscribedProductIds, function (key, value) {
                    if (value != null) {
                        $(value).val(key);
                    }
                });
            }
            
        }
        if ($.cookie('subscribedCheckBoxIds')) {
            var subscribedCheckBoxIds = $.parseJSON($.cookie('subscribedCheckBoxIds'));
            if (subscribedCheckBoxIds) {
                $.each(subscribedCheckBoxIds, function (key, value) {
                    if (value != null) {
                        $(value).prop("checked", true);
                    }
                });
            }

        }
    }
    if (url.indexOf('http://' + host + '/Credit/Summary') !== -1) {
        var temp = sessionStorage.getItem('productDetails');
        var productDetails = $.parseJSON(temp);
        var ProdUsers = sessionStorage.getItem('jsonObjProdUsers');
        var jsonProdUsers = $.parseJSON(ProdUsers);
        var total = 0;
        var count = 0;
        var itemCount = 0;
        var productNames = "";
        var subscriptionType = "";
        var html = '<table id="mytable" class="table table-bordred table-striped">' +
                        '<thead>' +
                        '<th>Product</th>' +
                        '<th>From</th>' +
                        '<th>Subscription Model</th>' +
                        '<th>Additional Users</th>' +
                        '<th>Total Cost =Users+Subscription</th>' +
                        '</thead>' +
                        '<tbody>'; console.log(productDetails);
                        $.each(productDetails, function (key, value) {
                                count = 0;
                                $.each(jsonProdUsers, function (key1, value1) {
                                    if (value1.productId == value.id)
                                    {
                                        count++;
                                    }
                                }); 
                                var productTotal = (count * (Math.round(value.perUserPrice * 1000) / 1000)) + (Math.round(value.subscriptionPrice * 1000) / 1000);
                                if (productNames == "")
                                {
                                    productNames = value.name;
                                }
                                else
                                {
                                    productNames +=","+ value.name;
                                }
                                if (value.subscription == "1")
                                {
                                    subscriptionType = "Gold - Monthly";
                                }
                                else if (value.subscription == "2") {
                                    subscriptionType = "Gold - Yearly";
                                }
                                else if (value.subscription == "3") {
                                    subscriptionType =  "Silver - Monthly";
                                }
                                else if (value.subscription == "4") {
                                    subscriptionType = "Silver - Yearly";
                                }
                                else if (value.subscription == "5") {
                                    subscriptionType = "Premium - Monthly";
                                }
                                else if (value.subscription == "6") {
                                    subscriptionType = "Premium - Yearly";
                                }
                                else if (value.subscription == "7") {
                                    subscriptionType = "Basic - Monthly";
                                }
                                else if (value.subscription == "8") {
                                    subscriptionType = "Basic - Yearly";
                                }
                            html += '<tr>' +

                                 '<td>' +
                                     '<div class="s_p_detail">' +
                                         '<h3>' + value.name + '</h3>' +
                                     '</div>' +
                                 '</td>' +
                                 '<td>' +
                                     '<input id="productStartDate_' + value.id + '" class="fromDate" type="text" />' +
                                 '</td>' +
                                 '<td>' +
                                     '<div class="s_p_detail">' +
                                         '<h3>' + subscriptionType + ' - $ ' + Math.round(value.subscriptionPrice * 1000) / 1000 + '</h3>' +
                                     '</div>' +
                                 '</td>' +
                                 '<td>' +
                                     '<div class="s_p_detail">' +
                                         '<h3>Additional Users -' + count + ' x $ ' + Math.round(value.perUserPrice * 1000) / 1000 +  '</h3>' +
                                     '</div>' +
                                 '</td>' +


                                 '<td>' +
                                     '<div class="s_p_detail">' +
                                         '<h3>$' + productTotal + '</h3>' +
                                     '</div>' +
                                 '</td>' +

                             '</tr>';
                            itemCount++;
                            total += productTotal;
                        }); 
                        $("#quantity").val(itemCount);
                  html+='</tbody>'+

                        '<tr>'+
                            '<td>&nbsp;</td>'+
                            '<td>'+
                                '<div class="s_p_detail">'+
                                    '<h4>Total</h4>'+
                                '</div>'+
                            '</td>'+
                            '<td>'+
                                '<div class="s_p_detail">'+
                                    '<h4>$' + total + '</h4>' +
                                '</div>'+
                            '</td>'+
                        '</tr>'+

                    '</table>';
        $("#item_name").val(productNames);
        $("#totalAmount").val(total);
        $("#summaryContent").append(html);
        $(".fromDate").datepicker({ minDate: 0 });
        if($.cookie('fromDates'))
        {
            var fromDates = $.parseJSON($.cookie('fromDates')); 
            $.each(fromDates, function (key, value) {
                if (value != null)
                {
                    var fromDateId = "#productStartDate_" + key;
                    $(fromDateId).val(value);
                }
            });
        }
       // $(".toDate").datepicker({ minDate: 0 });
    }
    if (url.indexOf('http://' + host + '/Credit/SelectUsers') !== -1) {
        var temp = sessionStorage.getItem('productDetails');
        var productDetails = $.parseJSON(temp);
        $('#sl-loadingscreen').show();
        $.ajax({
            type: "GET",
            url: "/api/User/GetUsers/",
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            Accept: "application/json",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
            },
            success: function (response) {
                var html = '<table id="mytable" class="table table-bordred table-striped">' +

                        '<thead>';

                $.each(productDetails, function (key, value) {
                    html += '<th>' + value.name + '-$' + Math.round(value.perUserPrice * 1000) / 1000 + ' Price per user</th>';
                });
                html += '</thead>' +
                           '<tbody>';
                $.each(response, function (key1, value1) {
                    html += '<tr>';
                    $.each(productDetails, function (key, value) {
                        html += '<td>' +
                                    '<div class="s_p_detail">' +
                                        '<h3><input type="checkbox" id="id_' + value.id + '_' + value1.customUserId + '" productId="' + value.id + '" userId="' + value1.customUserId + '" class="checkthis" />&nbsp;' + value1.customUserId + '</h3>' +
                                    '</div>' +
                                '</td>';
                    });
                    html += '</tr>';
                });
                html += '</tbody>' +
           '</table>';
                $("#selectUsersContent").append(html);
                $('#sl-loadingscreen').hide();
                if ($.cookie('selectedUsers')) {
                    var selectedUsers = $.parseJSON($.cookie('selectedUsers'));
                    if (selectedUsers)
                    {
                        $.each(selectedUsers, function (key, value) {
                            if (value != null) {
                                var selectedUsersId = "#" + value;
                                $(selectedUsersId).prop("checked", true);
                            }
                        });
                    }
                    
                }
            },
            error: function (x, y, z) {
                $('#sl-loadingscreen').hide();
            }
        });
        
    }
});