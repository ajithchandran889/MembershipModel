$(document).on("click", "#submitRegister", function (event) {
    var register =
        {
            emailId: $("#emailId").val(),
            password: $("#password").val()
        };
    var dataReg = JSON.stringify(register);
    $.ajax({
        type: "POST",
        url: "/api/Account/InitialRegister/",
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
            //alert(response.access_token);
            //alert($("#emailId").val());
            //self.user($("#emailId").val());
            //sessionStorage.setItem(tokenKey, response.access_token);
            $.cookie('isAuthenticated', true, { path: '/' }); 
            $.cookie('token', response.access_token, { path: '/' });
            $.cookie('userEmail', $("#emailId").val(), { path: '/' });
            var url = $("#RedirectToDash").val();
            window.location.href = url;
            //alert($.cookie("isAuthenticated"));
        },
        error: function (x, y, z) {
            alert("error");
        }
    });
    return false;
});
$(document).on("click", "#logOutBtn", function (event) {
    $.ajax({
        type: "GET",
        url: "/api/Account/Logout/",
        contentType: "application/json; charset=utf-8",
        Accept: "application/json",
        dataType: "json",
        success: function (response) {

            $.cookie('isAuthenticated', false, { path: '/' });
            $.cookie('token', null, { path: '/' });
            $.cookie('userEmail', null, { path: '/' });
            var url = $("#RedirectToHome").val(); 
            window.location.href = url;
        },
        error: function (x, y, z) {
            alert("error");
            
        }
    });
    
});
$(document).on("click", ".accountSetting", function (event) {
    $("#accountSettings").show();
    $("#groupSettigs").hide();
    $("#userSettings").hide();
    $("img.accountSetting").attr("src", "/Content/site/account-settings-hover.png");
    $("img.userSetting").attr("src", "/Content/site/user-settings.png");
    $("img.groupSetting").attr("src", "/Content/site/group-settings.png");
});
$(document).on("click", ".userSetting", function (event) {
    $("#accountSettings").hide();
    $("#groupSettigs").hide();
    $("#userSettings").show();
    $("img.accountSetting").attr("src", "/Content/site/account-settings.png");
    $("img.userSetting").attr("src", "/Content/site/user-settings-hover.png");
    $("img.groupSetting").attr("src", "/Content/site/group-settings.png");
});
$(document).on("click", ".groupSetting", function (event) {
    $("#accountSettings").hide();
    $("#groupSettigs").show();
    $("#userSettings").hide();
    $("img.accountSetting").attr("src", "/Content/site/account-settings.png");
    $("img.userSetting").attr("src", "/Content/site/user-settings.png");
    $("img.groupSetting").attr("src", "/Content/site/group-settings-hover.png");
});


$(document).on("click", ".addNewUser", function (event) {
    var register =
        {
            emailId: $("#inputEmail").val(),
            password: $("#inputPassword").val()
        }; 
    var dataReg = JSON.stringify(register); 
    $.ajax({
        type: "POST",
        url: "/api/Account/UserRegister/",
        data: dataReg,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
            alert("success");
        },
        error: function (x, y, z) {
            alert("error")
        }
    });
    return false;
});