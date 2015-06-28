﻿$(document).on("click", "#submitRegister", function (event) {
    var register =
        {
            emailId: $("#emailId").val(),
            password: $("#password").val()
        };
    var dataReg = JSON.stringify(register);
    $.ajax({
        type: "POST",
        url: "/api/AccountApi/Register/",
        data: dataReg,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            alert("success");
        },
        error: function (x, y, z) {
            
            var response = null;
            var errors = [];
            var errorsString = "";
            if (x.status == 400) {
                try {
                    response = JSON.parse(x.responseText);
                }
                catch (e) { 
                }
            }
            if (response != null) {
                var modelState = response.ModelState;
                for (var key in modelState) {
                    if (modelState.hasOwnProperty(key)) {
                        errorsString = (errorsString == "" ? "" : errorsString + "<br/>") + modelState[key]; 
                        errors.push(modelState[key]);
                    }
                }
            } 
            if (errorsString != "") {
                //$("#errDiv").html(errorsString);
            }
        }
    });
    return false;
});

$(document).on("click", "#submitLogin", function (event) {
    
    $.ajax({
        type: "POST",
        url: "/token",
        data: "grant_type=password&username=" + $("#emailId").val() + "&password=" + $("#password").val() + "",
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        Accept: "application/json",
        dataType: "json",
        success: function (response) {
            alert(response.access_token);
            alert($("#emailId").val());
            self.user($("#emailId").val());
            sessionStorage.setItem(tokenKey, response.access_token);
            
        },
        error: function (x, y, z) {
            alert("error");
        }
    });
    return false;
});