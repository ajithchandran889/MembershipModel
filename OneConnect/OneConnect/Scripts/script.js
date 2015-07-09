﻿$(document).on("click", "#submitRegister", function (event) {
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
$(document).on("click", ".changePassword", function (event) {
    var changePassowrd =
        {
            OldPassword: $("#oldPassword").val(),
            NewPassword: $("#newPassword").val(),
            ConfirmPassword: $("#confirmPassword").val()
        };
    var dataChangePwd = JSON.stringify(changePassowrd);
    $.ajax({
        type: "POST",
        url: "/api/Account/ChangePassword/",
        data: dataChangePwd,
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
$(document).on("click", ".changeEmail", function (event) {
    
    var ChangeEmail =
        {
            password: $("#currentUserPassword").val(),
            emailId: $("#newEmailId").val()
        };
    var dataChangePwd = JSON.stringify(ChangeEmail);
    $.ajax({
        type: "POST",
        url: "/api/Account/ChangeEmailWithPassword/",
        data: dataChangePwd,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
            alert("success");
        },
        error: function (x, y, z) {
            alert("error");
        }
    });
    return false;
});
$('.checkboxid').change(function () {
    var userId = $(this).attr("userId");
    if ($(this).is(":checked")) {
        var status = 1;
    }
    else
    {
        var status = 0;
    }
    var usrAct =
        {
            userId: userId,
            status: status
        };
    var dataUsrAct = JSON.stringify(usrAct);
    $.ajax({
        type: "POST",
        url: "/api/Account/DisableEnableUser/",
        data: dataUsrAct,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
        },
        error: function (x, y, z) {
            alert("error");
        }
    });
    return false;
});
$(document).on("click", ".editUser", function () {
    var userId = $(this).attr("userId");
    var editBtnId = "#editUser_" + userId;
    var saveBtnId = "#saveChanges_" + userId;
    var emailId = "#"+userId;
    var editEmailId = "#email_" + userId;
    $(editBtnId).hide();
    $(emailId).hide();
    $(saveBtnId).show();
    $(editEmailId).show();
    return false;
});
$(document).on("click", ".saveChanges", function () {
    var userId = $(this).attr("userId");
    var editBtnId = "#editUser_" + userId;
    var saveBtnId = "#saveChanges_" + userId;
    var emailId = "#"+userId;
    var editEmailId = "#email_" + userId;
    if(isValidEmailAddress($(editEmailId).val()))
    {
        var usrEmail =
        {
            userId: userId,
            emailId: $(editEmailId).val()
        };
        var dataUsrEmail = JSON.stringify(usrEmail);
        $.ajax({
            type: "POST",
            url: "/api/Account/ChangeEmail/",
            data: dataUsrEmail,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
            },
            success: function (response) {
                $(emailId).text($(editEmailId).val());
                $(editBtnId).show();
                $(emailId).show();
                $(saveBtnId).hide();
                $(editEmailId).hide();
            },
            error: function (x, y, z) {
                alert("error");
            }
        });
        
    }
    else
    {
        alert("Invalid email address");
        
    }
    return false;
});
function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailAddress);
};