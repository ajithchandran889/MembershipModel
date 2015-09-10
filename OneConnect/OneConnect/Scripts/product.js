
jsonObj = [];
$(document).ready(function () {
    $(".fromDate").datepicker();
    $(".toDate").datepicker();
});
$(document).on("click", "#selectUsers", function (event) {
    jsonObj = [];
    
    if ($('input[name=selectProducts]:checked').length) {
        $('input[name=selectProducts]:checked').each(function () {
            var id = "#subscribeModel_" + $(this).attr("productid");
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
                if ($(id).val() == 2)
                {
                    priceid = "#goldValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#goldMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 3)
                {
                    priceid = "#goldValuePerUserYearly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#goldYearly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 4) {
                    priceid = "#silverValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#silverMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 5) {
                    priceid = "#silverValuePerUserYearly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#silverYearly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 6) {
                    priceid = "#premiumValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#premiumMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 7) {
                    priceid = "#premiumValuePerUserYearly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#premiumYearly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 8) {
                    priceid = "#basicValuePerUserMonthly_" + $(this).attr("productid");
                    priceIdSubsrciption = "#basicMonthly_" + $(this).attr("productid");
                }
                else if ($(id).val() == 9) {
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
        window.location.href = '/Credit/SelectUsers';
    }
    else {
        alert("Please select a product");
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
        var toDateId = "#productEndDate_" + value.id;
        item['toDate'] = $(toDateId).val();
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
    jsonObjProdUsers = [];
    var temp = sessionStorage.getItem('productDetails');
    if ($('input[class=checkthis]:checked').length)
    {
        $('input[class=checkthis]:checked').each(function () {
            item = {};
            item["productId"] = $(this).attr("productId");
            item["userId"] = $(this).attr("userId");
            jsonObjProdUsers.push(item);
        });
        
        sessionStorage.setItem('jsonObjProdUsers', JSON.stringify(jsonObjProdUsers));
    }
    window.location.href = '/Credit/Summary';
});
$(document).ready(function () {
    var url = window.location.href; 
    var host = window.location.host; 
    if (url.indexOf('http://' + host + '/Credit/Summary') !== -1) {
        var temp = sessionStorage.getItem('productDetails');
        var productDetails = $.parseJSON(temp);
        var ProdUsers = sessionStorage.getItem('jsonObjProdUsers');
        var jsonProdUsers = $.parseJSON(ProdUsers);
        console.log("productDetails:"); console.log(productDetails);
        console.log("jsonProdUsers:"); console.log(jsonProdUsers);
        var total = 0;
        var count=0;
        var html = '<table id="mytable" class="table table-bordred table-striped">' +
                        '<thead>' +
                        '<th>Product (Product description)</th>' +
                        '<th>From</th>' +
                        '<th>To</th>' +
                        '<th>Aditional Users</th>' +
                        '<th>Total Cost =Subscription+Users</th>' +
                        '</thead>' +
                        '<tbody>'; 
                        $.each(productDetails, function (key, value) {
                                count = 0;
                                $.each(jsonProdUsers, function (key1, value1) {
                                    if (value1.productId == value.id)
                                    {
                                        count++;
                                    }
                                });
                                var productTotal = (count * (Math.round(value.perUserPrice * 1000) / 1000)) + (Math.round(value.subscriptionPrice * 1000) / 1000);
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
                                     '<input id="productEndDate_' + value.id + '" class="toDate" type="text" />' +
                                 '</td>' +
                                 '<td>' +
                                     '<div class="s_p_detail">' +
                                         '<h3>Aditional Users -'+count+' x $' + Math.round(value.perUserPrice * 1000) / 1000 + '</h3>' +
                                     '</div>' +
                                 '</td>' +


                                 '<td>' +
                                     '<div class="s_p_detail">' +
                                         '<h3>$' + productTotal + '</h3>' +
                                     '</div>' +
                                 '</td>' +

                             '</tr>';
                            total += productTotal;
                        });
                  html+='</tbody>'+

                        '<tr>'+
                            '<td>&nbsp;</td>'+
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
        $("#totalAmount").val(total);
        $("#summaryContent").append(html);
        $(".fromDate").datepicker();
        $(".toDate").datepicker();
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
                                        '<h3><input type="checkbox" productId="' + value.id + '" userId="' + value1.customUserId + '" class="checkthis" />&nbsp;' + value1.customUserId + '</h3>' +
                                    '</div>' +
                                '</td>';
                    });
                    html += '</tr>';
                });
                html += '</tbody>' +
           '</table>';
                $("#selectUsersContent").append(html);
                $('#sl-loadingscreen').hide();
            },
            error: function (x, y, z) {
                $('#sl-loadingscreen').hide();
            }
        });
        
    }
});