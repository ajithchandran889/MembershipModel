var loading = $("#sl-loadingscreen");
var minDelay = 500;
var start = new Date();
var loaderTime;
$(document).ajaxStart(function () {
    loaderTime = setTimeout(function () { showLoader(); }, 1000);
});
function showLoader() {

    minDelay = 500;
    start = new Date();
    loading.fadeIn("slow");
}
$(document).ajaxStop(function () {
    var end = new Date();
    var timeInMilliseconds = (end - start);
    if (timeInMilliseconds < minDelay) {
        setTimeout(function () { callback(); }, minDelay - timeInMilliseconds);
    }
    else callback();
});
function callback() {
    clearTimeout(loaderTime);
    loading.fadeOut("slow");
}
$("#registerForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        emailId: { required: true, email: true },
        password: { required: true }
    },
    messages: {
        emailId: {
            required: "Please enter your email"
        },
        password: {
            required: "Please enter your password"
        }
    },
    tooltip_options: {
        emailId: { trigger: 'focus' },
        password: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    submitHandler: function () {
        $("#errDiv").html($("#g-recaptcha-response").val());
        var register =
        {
            emailId: $("#emailId").val(),
            password: $("#password").val(),
            captchaResponse: $("#g-recaptcha-response").val()
        };

        var dataReg = JSON.stringify(register);
        //$('#sl-loadingscreen').show();
        $.ajax({
            type: "POST",
            url: "/api/Account/InitialRegister/",
            data: dataReg,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#registerFailure").hide();
                //$("#registerSuccess").show();
                $("#emailId").val("");
                $("#password").val("");
                grecaptcha.reset();
                $("#username-pass").hide();
                $("#username-pass-redirect").show();
                //$('#sl-loadingscreen').hide();
            },
            error: function (x, y, z) {
                var errorMsg = x.responseText;
                $("#failureMessage").text(errorMsg.replace(/"/g, ''));
                $("#registerFailure").show();
                $("#successMessage").hide();
                $("#username-pass").show();
                $("#username-pass-redirect").hide();
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
                //$('#sl-loadingscreen').hide();
            }
        });
        return false;
    }
});

$("#loginForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        userId: { required: true },
        password: { required: true }
    },
    messages: {
        userId: {
            required: "Please enter your userId"
        },
        password: {
            required: "Please enter your password"
        }
    },
    tooltip_options: {
        userId: { trigger: 'focus' },
        password: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    submitHandler: function () {
        //$('#sl-loadingscreen').show();
        $.ajax({
            type: "POST",
            url: "/token",
            data: "grant_type=password&username=" + $("#userId").val() + "&password=" + $("#password").val() + "",
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            Accept: "application/json",
            dataType: "json",
            success: function (response) {
                $.cookie('isAuthenticated', true, { path: '/' });
                $.cookie('token', response.access_token, { path: '/' });
                $.cookie('userId', $("#userId").val(), { path: '/' });
                var url = $("#RedirectToDash").val();
                window.location.href = url;
                //$('#sl-loadingscreen').hide();
            },
            error: function (response) {

                var errormsg = response.responseText;
                var temp = $.parseJSON(errormsg);
                var html = "<span>" + temp.error_description + "</span>";
                $("#errorMessage").empty();
                $("#errorMessage").append(html);
                $("#errorMessage").show();
                //$('#sl-loadingscreen').hide();
            }
        });
        return false;

    }
});

$(document).on("click", "#logOutBtn", function (event) {
    //$('#sl-loadingscreen').show();
    $.ajax({
        type: "GET",
        url: "/api/Account/Logout/",
        contentType: "application/json; charset=utf-8",
        Accept: "application/json",
        dataType: "json",
        success: function (response) {

            $.cookie('isAuthenticated', false, { path: '/' });
            $.cookie('token', null, { path: '/' });
            $.cookie('userId', null, { path: '/' });
            var url = $("#RedirectToHome").val();
            //$('#sl-loadingscreen').hide();
            window.location.href = url;
        },
        error: function (x, y, z) {
            //$('#sl-loadingscreen').hide();
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


$("#addNewUserForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        emailId: { required: true },
        password: { required: true }
    },
    messages: {
        emailId: {
            required: "Please enter your email Id"
        },
        password: {
            required: "Please enter your password"
        }
    },
    tooltip_options: {
        emailId: { trigger: 'focus' },
        password: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertBefore(element);
    },
    submitHandler: function () {
        var register =
        {
            emailId: $("#inputEmail").val(),
            password: $("#inputPassword").val()
        };
        var dataReg = JSON.stringify(register);
        //$('#sl-loadingscreen').show();
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
                url = "/User/UserListPartial";
                $("#userListDIv").load(url);
                $("#successMessageAddedNewUser").show();
                //$('#sl-loadingscreen').hide();

            },
            error: function (x, y, z) {
                $("#errorsMessageAddedNewUser").show();
                //$('#sl-loadingscreen').hide();
            }
        });
        return false;

    }
});

$(document).on("click", ".addNewUser", function (event) {
    $("#addNewUserForm").submit();

});
$("#changePasswordForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        oldPassword: { required: true },
        newPassword: { required: true },
        confirmPassword: { required: true, equalTo: "#newPassword" }
    },
    messages: {
        oldPassword: {
            required: "Please enter your old password"
        },
        newPassword: {
            required: "Please enter your new password"
        },
        confirmPassword: {
            required: "Please confirm your new password"
        }

    },
    tooltip_options: {
        oldPassword: { trigger: 'focus' },
        newPassword: { trigger: 'focus' },
        confirmPassword: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertBefore(element);
    },
    submitHandler: function () {
        var changePassowrd =
       {
           OldPassword: $("#oldPassword").val(),
           NewPassword: $("#newPassword").val(),
           ConfirmPassword: $("#confirmPassword").val()
       };
        var dataChangePwd = JSON.stringify(changePassowrd);
        //$('#sl-loadingscreen').show();
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
                $("#successMessagePasswordChange").show();
                //$('#sl-loadingscreen').hide();
            },
            error: function (x, y, z) {
                $("#errorMessagePasswordChange").show();
                //$('#sl-loadingscreen').hide();
            }
        });
        return false;


    }
});
$(document).on("click", ".changePassword", function (event) {
    $("#changePasswordForm").submit();
});
$("#changeEmailForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        userId: { required: true },
        oldEmailId: { required: true },
        newEmailId: { required: true },
        currentUserPassword: { required: true }
    },
    messages: {
        userId: {
            required: "Please enter your user id"
        },
        oldEmailId: {
            required: "Please enter your old email"
        },
        newEmailId: {
            required: "Please confirm your new email"
        },
        currentUserPassword: {
            required: "Please confirm your password"
        }

    },
    tooltip_options: {
        userId: { trigger: 'focus' },
        oldEmailId: { trigger: 'focus' },
        newEmailId: { trigger: 'focus' },
        currentUserPassword: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertBefore(element);
    },
    submitHandler: function () {
        var ChangeEmail =
         {
             userId: $("#userId").val(),
             oldEmailId: $("#oldEmailId").val(),
             password: $("#currentUserPassword").val(),
             emailId: $("#newEmailId").val(),
             hostName: window.location.origin,
         };
        var dataChangePwd = JSON.stringify(ChangeEmail);
        //$('#sl-loadingscreen').show();
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
                $("#successMessageEmailChange").show();
                //$('#sl-loadingscreen').hide();
            },
            error: function (x, y, z) {
                $("#errorMessageEmailChange").show();
                //$('#sl-loadingscreen').hide();
            }
        });
        return false;


    }
});
$(document).on("click", ".changeEmail", function (event) {

    $("#changeEmailForm").submit();
});
$('.checkboxid').change(function () {

    var userId = $(this).attr("userId");
    if ($(this).is(":checked")) {
        var status = 1;
    }
    else {
        var status = 0;
    }
    var usrAct =
        {
            userId: userId,
            status: status
        };
    var dataUsrAct = JSON.stringify(usrAct);
    //showLoader = setTimeout("$('#sl-loadingscreen').show()", 100);
    ////$('#sl-loadingscreen').show();
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
            // //$('#sl-loadingscreen').hide();
        },
        error: function (x, y, z) {
            //$('#sl-loadingscreen').hide();
            alert("error");
        }
    });
    return false;
});
$(document).on("click", ".editUser", function () {
    var userId = $(this).attr("userId");
    var editBtnId = "#editUser_" + userId;
    var saveBtnId = "#saveChanges_" + userId;
    var cancelBtnId = "#cancelEdit_" + userId;
    //var emailId = "#" + userId;
    //var editEmailId = "#email_" + userId;
    var nameId = "#nameLabel_" + userId;
    var editnameId = "#name_" + userId;
    var addressId = "#addressLabel_" + userId;
    var editAddressId = "#address_" + userId;
    var contactId = "#contactLabel_" + userId;
    var editContactId = "#contact_" + userId;
    var statusId = "#statusLabel_" + userId;
    var editStatusId = "#status_" + userId;
    
    $(editnameId).text($(nameId).val());
    $(editAddressId).text($(addressId).val());
    $(editContactId).text($(contactId).val());
    alert($(statusId).attr("userstatus"));
    if ($(statusId).attr("userstatus") == "true") {
        alert($(editStatusId));
        alert($(editStatusId).checked);
        $(editStatusId).attr('checked', true);
        alert($(editStatusId).checked);
    } else {
        alert($(editStatusId).checked);
        $(editStatusId).attr('checked', false);
        alert($(editStatusId).checked);
    }
    


    $(editBtnId).hide();
    $(saveBtnId).show();
    $(cancelBtnId).show();
    $(nameId).hide();
    $(editnameId).show();
    $(addressId).hide();
    $(editAddressId).show();
    $(contactId).hide();
    $(editContactId).show();
    $(statusId).hide();
    $(editStatusId).show();
    
    return false;
});
$(document).on("click", ".cancelEdit", function () {
    var userId = $(this).attr("userId");
    var editBtnId = "#editUser_" + userId;
    var saveBtnId = "#saveChanges_" + userId;
    var cancelBtnId = "#cancelEdit_" + userId;
    //var emailId = "#" + userId;
    //var editEmailId = "#email_" + userId;
    var nameId = "#nameLabel_" + userId;
    var editnameId = "#name_" + userId;
    var addressId = "#addressLabel_" + userId;
    var editAddressId = "#address_" + userId;
    var contactId = "#contactLabel_" + userId;
    var editContactId = "#contact_" + userId;
    var statusId = "#statusLabel_" + userId;
    var editStatusId = "#status_" + userId;

   

    $(editBtnId).show();
    $(saveBtnId).hide();
    $(cancelBtnId).hide();
    $(nameId).show();
    $(editnameId).hide();
    $(addressId).show();
    $(editAddressId).hide();
    $(contactId).show();
    $(editContactId).hide();
    $(statusId).show();
    $(editStatusId).hide();
    return false;
});
$(document).on("click", ".saveChanges", function () {
    var userId = $(this).attr("userId");
    var editBtnId = "#editUser_" + userId;
    var saveBtnId = "#saveChanges_" + userId;
    var cancelBtnId = "#cancelEdit_" + userId;
    //var emailId = "#" + userId;
    //var editEmailId = "#email_" + userId;
    var nameId = "#nameLabel_" + userId;
    var editnameId = "#name_" + userId;
    var addressId = "#addressLabel_" + userId;
    var editAddressId = "#address_" + userId;
    var contactId = "#contactLabel_" + userId;
    var editContactId = "#contact_" + userId;
    var statusId = "#statusLabel_" + userId;
    var editStatusId = "#status_" + userId;
    var status = false
    if ($(editStatusId).is(':checked')) {
        status = true;
    }

    //if ($(editnameId).val() != "" || $(editAddressId).val() != "" || $(editContactId).val() != "") {
    var UserEditInfo =
        {
            userId: userId,
            name: $(editnameId).val(),
            address: $(editAddressId).val(),
            contact: $(editContactId).val(),
            status: status
        };
    var dataUserEditInfo = JSON.stringify(UserEditInfo);
    alert(JSON.stringify(dataUserEditInfo));
    //$('#sl-loadingscreen').show();
    $.ajax({
        type: "POST",
        url: "/api/Account/ChangeUserInfo/",
        data: dataUserEditInfo,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
            $(nameId).text($(editnameId).val());
            $(addressId).text($(editAddressId).val());
            $(contactId).text($(editContactId).val());
            if ($(editStatusId).is(':checked')) {
                $(statusId).attr('userstatus', true);

                $(statusId).find('img').attr('src', '/Content/site/role-checked.png');
            } else {
                $(statusId).find('img').attr('src', '/Content/site/role-unchecked.png');
                $(statusId).attr('userstatus', false);
            }

            $(editBtnId).show();
            $(saveBtnId).hide();
            $(cancelBtnId).hide();
            $(nameId).show();
            $(editnameId).hide();
            $(addressId).show();
            $(editAddressId).hide();
            $(contactId).show();
            $(editContactId).hide();
            $(statusId).show();
            $(editStatusId).hide();
            //$('#sl-loadingscreen').hide();
        },
        error: function (x, y, z) {
            //$('#sl-loadingscreen').hide();
            alert("error");
        }
    });

    /*}
    else {
        $(editBtnId).show();
        $(saveBtnId).hide();
        $(cancelBtnId).hide();
        $(nameId).show();
        $(editnameId).hide();
        $(addressId).show();
        $(editAddressId).hide();
        $(contactId).show();
        $(editContactId).hide();
        $(statusId).show();
        $(editStatusId).hide();
    }*/
    return false;
});
function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailAddress);
};
var somethingChanged = false;
$(document).ready(function () {
    $('#accountEditForm input').change(function () {
        somethingChanged = true;
    });
});
$(document).on("click", ".saveAccountInfoEdit", function () {

    if (somethingChanged) {
        var status = false;
        if ($("#checkboxAccountStatus").is(':checked')) {
            status = true;
        }
        var confirmed = 0;

        if (status == false) {
            if (confirm("Once you deactivate your account groups,user and member product access right deactivated! Are you sure?") == true) {
                confirmed = 1;
            } else {
                confirmed = 0;
            }
        } else {
            confirmed = 1;
        }
        if (confirmed == 1) {
            var postData =
            {
                name: $("#accountName").val(),
                company: $("#companyName").val(),
                address: $("#userAddress").val(),
                contact: $("#userContact").val(),
                status: status,
                captchaResponse: $("#g-recaptcha-response").val()
            };
            var postDataJSON = JSON.stringify(postData);
            //$('#sl-loadingscreen').show();
            $.ajax({
                type: "POST",
                url: "/api/Account/EditAccountInfo/",
                data: postDataJSON,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
                },
                success: function (response) {
                    if (status == false) {
                        $("#logOutBtn").click();
                    }
                    $("#successMessageEditAccount").show();
                    $("#failureMessageEditAccount").hide();
                    $("#gpsSuccessMessage").text("Account info updated successfully");
                    grecaptcha.reset();
                    //$('#sl-loadingscreen').hide();
                },
                error: function (x, y, z) {
                    var errorMsg = x.responseText;
                    $("#successMessageEditAccount").hide();
                    $("#failureMessageEditAccount").show();
                    $("#gpsFailureMessage").text(errorMsg.replace(/"/g, ''));
                    grecaptcha.reset();
                    //$('#sl-loadingscreen').hide();
                }
            });
        }
    }
    return false;
});
$("#forgotUserIdForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        email: { required: true, email: true },
    },
    messages: {
        email: {
            required: "Please enter your email"
        }
    },
    tooltip_options: {
        email: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    submitHandler: function () {
        //$('#sl-loadingscreen').show();
        var forgotUserId =
           {
               emailId: $("#emailId").val(),
               captchaResponse: $("#g-recaptcha-response").val()
           };
        var dataforgotUserId = JSON.stringify(forgotUserId);
        $.ajax({
            type: "POST",
            url: "/api/Account/ForgotUserId/",
            data: dataforgotUserId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#errorMessage").hide();
                //$("#successMessage").show();
                $("#emailId").val("");
                grecaptcha.reset();
                $("#username-pass").hide();
                $("#username-pass-redirect").show();
                //$('#sl-loadingscreen').hide();
            },
            error: function (x, y, z) {
                var errorMsg = x.responseText;
                $("#failureMessage").text(errorMsg.replace(/"/g, ''));
                $("#errorMessage").show();
                $("#successMessage").hide();
                $("#username-pass").show();
                $("#username-pass-redirect").hide();
                //$('#sl-loadingscreen').hide();
            }
        });
        return false;

    }
});

$("#forgotPasswordForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        userId: { required: true },
        emailId: { required: true, email: true }
    },
    messages: {
        userId: {
            required: "Please enter your user Id"
        },
        emailId: {
            required: "Please enter your email"
        }
    },
    tooltip_options: {
        userId: { trigger: 'focus' },
        emailId: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    submitHandler: function () {
        //$('#sl-loadingscreen').show();
        var forgotPassword =
       {
           userId: $("#userId").val(),
           emailId: $("#emailId").val(),
           hostName: window.location.origin,
           captchaResponse: $("#g-recaptcha-response").val()
       };
        var dataforgotPassword = JSON.stringify(forgotPassword);
        $.ajax({
            type: "POST",
            url: "/api/Account/ForgotPassword/",
            data: dataforgotPassword,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $("#errorMessage").hide();
                //$("#successMessage").show();
                $("#userId").val("");
                $("#emailId").val("");
                grecaptcha.reset();
                $("#username-pass").hide();
                $("#username-pass-redirect").show();
                //$('#sl-loadingscreen').hide();
            },
            error: function (x, y, z) {
                var errorMsg = x.responseText;
                $("#failureMessage").text(errorMsg.replace(/"/g, ''));
                $("#errorMessage").show();
                $("#successMessage").hide();
                $("#username-pass").show();
                $("#username-pass-redirect").hide();
                //$('#sl-loadingscreen').hide();
            }
        });
        return false;

    }
});

$(document).on("click", "#recoverPassword", function () {
    var recoverPassword =
       {
           newPassword: $("#new").val(),
           recoveryToken: $("#recoveryToken").val()
       };
    //$('#sl-loadingscreen').show();
    var dataRecoverPassword = JSON.stringify(recoverPassword);
    $.ajax({
        type: "POST",
        url: "/api/Account/RceoverPassword/",
        data: dataRecoverPassword,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            alert("success");
            $("#errorMessage").hide();
            $("#successMessage").show();
            //$('#sl-loadingscreen').hide();
        },
        error: function (x, y, z) {
            alert("error");
            $("#errorMessage").show();
            $("#successMessage").hide();
            //$('#sl-loadingscreen').hide();
        }
    });
    return false;
});
$(document).on("click", "#resetEmail", function () {
    var changeEmail =
       {
           password: $("#confirm").val(),
           token: $("#emailToken").val()
       };
    var datachangeEmail = JSON.stringify(changeEmail);
    //$('#sl-loadingscreen').show();
    $.ajax({
        type: "POST",
        url: "/api/Account/changeEmailConfirmation/",
        data: datachangeEmail,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            alert("success");
            $("#errorMessage").hide();
            $("#successMessage").show();
            //$('#sl-loadingscreen').hide();
        },
        error: function (x, y, z) {
            alert("error");
            $("#errorMessage").show();
            $("#successMessage").hide();
            //$('#sl-loadingscreen').hide();
        }
    });
    return false;
});
$(document).on("click", "#clearChanges", function (event) {
    $("#accountEditForm")[0].reset();
    return false;
});
$(document).on("click", "#changePwdClearChanges", function (event) {
    $("#changePasswordForm")[0].reset();
    return false;
});
$(document).on("click", "#chnageEmailClearForm", function (event) {
    $("#changeEmailForm")[0].reset();
    return false;
});
$(document).on("click", "#cancelAddUser", function (event) {
    $("#addNewUserForm")[0].reset();
    return false;
});
$(document).on("click", ".goToLoginPage", function (event) {
    var url = $("#RedirectToLogin").val();
    window.location.href = url;
});


/******************GROUP*************************/
$(document).on("click", ".groupSetting", function (event) {
    $("#accountSettings").hide();
    $("#groupSettigs").show();
    $("#userSettings").hide();
    $("img.accountSetting").attr("src", "/Content/site/account-settings.png");
    $("img.userSetting").attr("src", "/Content/site/user-settings.png");
    $("img.groupSetting").attr("src", "/Content/site/group-settings-hover.png");

    $("img.addNewGroups").attr("src", "/Content/site/pluse.png");
    $("span.addNewGroups").html("Add New Group");
    $("#AddNewGroup").removeClass("selected");
    $("#member-group-details").hide();

    $("#HideAndShowInActive").show();

    var url = "";


    if ($("#HideAndShowInActive").hasClass("selected")) {
        url = "/User/GroupListPartial?isActiveOnly=" + true;
        $("#member-group-master").load(url);

    } else {
        url = "/User/GroupListPartial?isActiveOnly=" + false;
        $("#member-group-master").load(url);

    }
    $("img.addNewGroups").attr("data-member", "/User/AddGroupPartial")


    $("#member-group-master").show();

});
$(document).on("click", "#AddNewGroup", function (event) {


    if ($("#AddNewGroup").hasClass("selected")) {
        $("img.addNewGroups").attr("src", "/Content/site/pluse.png");
        $("span.addNewGroups").html("Add New Group");
        $("#AddNewGroup").removeClass("selected");
        $("#member-group-details").hide();

        $("#HideAndShowInActive").show();

        var url = $("img.addNewGroups").attr("data-member");

        $("#member-group-master").load(url);


        $("#member-group-master").show();

        $("img.addNewGroups").attr("data-member", "/User/AddGroupPartial")


    } else {
        $("img.addNewGroups").attr("src", "/Content/site/minus.png");
        $("span.addNewGroups").html("Go To Group List");
        $("#AddNewGroup").addClass("selected");
        $("#member-group-master").hide();

        $("#HideAndShowInActive").hide();

        var url = $("img.addNewGroups").attr("data-member");

        $("#member-group-details").load(url);

        $("#member-group-details").show();

        if ($("#HideAndShowInActive").hasClass("selected")) {

            $("img.addNewGroups").attr("data-member", "/User/GroupListPartial?isActiveOnly=" + true)

        } else {

            $("img.addNewGroups").attr("data-member", "/User/GroupListPartial?isActiveOnly=" + false)

        }


    }

});

$(document).on("click", "#HideAndShowInActive", function (event) {


    if ($("#HideAndShowInActive").hasClass("selected")) {
        $("img.HideAndShowInActive").attr("src", "/Content/site/minus.png");
        $("span.HideAndShowInActive").html("Hide Inactive Groups");
        $("#HideAndShowInActive").removeClass("selected");

        var url = "/User/GroupListPartial?isActiveOnly=" + false;

        $("#member-group-master").load(url);

        $("#member-group-master").show();


    } else {
        $("img.HideAndShowInActive").attr("src", "/Content/site/pluse.png");
        $("span.HideAndShowInActive").html("Show Inactive Groups");
        $("#HideAndShowInActive").addClass("selected");


        var url = "/User/GroupListPartial?isActiveOnly=" + true;

        $("#member-group-master").load(url);

        $("#member-group-master").show();
    }

});

$(document).on("click", "#addNewGroupFormSave", function (event) {
    $("#addNewGroupForm").submit();

});



$(document).on("submit", "#addNewGroupForm", function (e) {

    var status = 0;
    if ($("#cbxGroupActive").is(':checked')) {
        status = 1;
    }

    var confirmed = 0;

    if (status == 0) {
        if (confirm("Once you deactivate a group all products and member access right deleted! Are you sure?") == true) {
            confirmed = 1;
        } else {
            confirmed = 0;
        }
    } else {
        confirmed = 1;
    }
    if (confirmed == 1) {
        var groupDetails =
        {
            groupId: $("#hfdGroupId").val(),
            groupName: $("#txtGroupName").val(),
            description: $("#txtGroupDescription").val(),
            isActive: status,
            groupAdmin: $("#ddlGroupAdmin :selected").val()
        };
        var groupDetailsJSON = JSON.stringify(groupDetails);
        //$('#sl-loadingscreen').show();
        $.ajax({
            type: "POST",
            url: "/api/Group/SaveGroup/",
            data: groupDetailsJSON,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
            },
            success: function (response) {
                var url = "/User/EditGroupPartial?groupId=" + response;
                $("#member-group-details").load(url);
                $("#successMessageGroupInfoSave").show();
                //$('#sl-loadingscreen').hide();
            },
            error: function (x, y, z) {
                //$('#sl-loadingscreen').hide();
                alert("error");
            }
        });
    }
    return false;
});

$(document).on("click", "#addNewGroupFormCancel", function (e) {

    var groupId = $("#hfdGroupId").val();
    var url = "";
    if (groupId == 0) {
        url = "/User/AddGroupPartial";


    } else {
        url = "/User/EditGroupPartial?groupId=" + groupId;
    }
    $("#member-group-details").load(url);
    return false;
});
//$(document).on("click", "#addNewGroupFormDelete", function (e) {

//    var groupDetails =
//        {
//            groupId: $("#hfdGroupId").val()
//        };
//    var groupDetailsJSON = JSON.stringify(groupDetails);
//    $.ajax({
//        type: "POST",
//        url: "/api/Group/DeleteGroup/",
//        data: groupDetailsJSON,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        beforeSend: function (xhr) {
//            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
//        },
//        success: function (response) {
//            $("img.addNewGroups").attr("src", "/Content/site/pluse.png");
//            $("span.addNewGroups").html("Add New Group");
//            $("#AddNewGroup").removeClass("selected");
//            $("#member-group-details").hide();

//            $("#HideAndShowInActive").show();

//            var url = $("img.addNewGroups").attr("data-member");
//            $("#member-group-master").load(url);

//            $("#member-group-master").show();

//            $("#member-group-details").load("/User/AddGroupPartial");

//            $("img.addNewGroups").attr("data-member", "/User/AddGroupPartial");


//        },
//        error: function (x, y, z) {

//            alert("error");
//        }
//    });
//    return false;
//});
$(document).on("click", ".editGroup", function () {

    var groupId = $(this).attr("groupId");
    var editBtnId = "#editGroup_" + groupId;



    $("img.addNewGroups").attr("src", "/Content/site/minus.png");
    $("span.addNewGroups").html("Go To Group List");
    $("#AddNewGroup").addClass("selected");
    $("#member-group-master").hide();

    $("#HideAndShowInActive").hide();
    var url = $(this).attr("data-member");
    $("#member-group-details").load(url);

    $("#member-group-details").show();

    if ($("#HideAndShowInActive").hasClass("selected")) {

        $("img.addNewGroups").attr("data-member", "/User/GroupListPartial?isActiveOnly=" + true)

    } else {

        $("img.addNewGroups").attr("data-member", "/User/GroupListPartial?isActiveOnly=" + false)

    }



    return false;
});


$(document).on("change", ".groupProductCheckbox", function (e) {
    //if (event.stopPropagation) {    // standard
    //    event.stopPropagation();
    //} else {    // IE6-8
    //    event.cancelBubble = true;
    //}

    var productId = $(this).attr("productId");
    var groupId = $(this).attr("groupId");
    var chkLabelId = "#chk_grp_prd_label_" + productId;
    var status = 0;
    if ($(this).is(":checked")) {

        status = 1;
    }
    else {
        status = 0;
    }

    var grpPrdDetails =
        {
            productId: productId,
            groupId: groupId,
            isSubscribed: status
        };
    var dataGrpPrdDetails = JSON.stringify(grpPrdDetails);
    //$('#sl-loadingscreen').show();
    $.ajax({
        type: "POST",
        url: "/api/Group/GroupProductSubscription/",
        data: dataGrpPrdDetails,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
            if (response == "unsubscribed") {

                $(chkLabelId).html("Unsubscribe");
                var currGroupId = $("#hfdGroupId").val();
                var url = "/User/EditGroupPartial?groupId=" + currGroupId;
                $("#member-group-details").load(url);
                $("#groupProductSubscriptionSuccess").show();
                $("#groupProductSubscriptionFailure").hide();
                $("#gpsSuccessMessage").text("Product unsubscribed from this group");

            } else if (response == "subscribed") {

                $(chkLabelId).html("Subscribe");
                var currGroupId = $("#hfdGroupId").val();
                var url = "/User/EditGroupPartial?groupId=" + currGroupId;
                $("#member-group-details").load(url);
                $("#groupProductSubscriptionSuccess").show();
                $("#groupProductSubscriptionFailure").hide();
                $("#gpsSuccessMessage").text("Product subscribed to this group");

            } else {

                e.preventDefault();
                $("#groupProductSubscriptionFailure").show();
                $("#groupProductSubscriptionSuccess").hide();
                $("#gpsFailureMessage").text("Operation failed");

            }
            //$('#sl-loadingscreen').hide();
        },
        error: function (x, y, z) {
            var errorMsg = x.responseText;
            event.stopPropagation();
            event.preventDefault();
            $("#groupProductSubscriptionFailure").show();
            $("#groupProductSubscriptionSuccess").hide();
            $("#gpsFailureMessage").text(errorMsg.replace(/"/g, ''));
            //$('#sl-loadingscreen').hide();
            return false;
        }
    });

    return false;
});
$(document).on("change", ".groupUserCheckbox", function (event) {

    var userId = $(this).attr("userId");
    var groupId = $(this).attr("groupId");
    var chkLabelId = "#chk_grp_usr_label_" + userId;
    var status = 0;
    if ($(this).is(":checked")) {

        status = 1;
    }
    else {
        status = 0;
    }

    var grpUsrDetails =
        {
            userId: userId,
            groupId: groupId,
            isSubscribed: status
        };
    var dataGrpUsrDetails = JSON.stringify(grpUsrDetails);
    //$('#sl-loadingscreen').show();
    $.ajax({
        type: "POST",
        url: "/api/Group/GroupMemberSubscription/",
        data: dataGrpUsrDetails,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
            if (response == "unsubscribed") {

                $(chkLabelId).html("Unsubscribe");
                var currGroupId = $("#hfdGroupId").val();
                var url = "/User/EditGroupPartial?groupId=" + currGroupId;
                $("#member-group-details").load(url);
                $("#groupMemberSubscriptionSuccess").show();
                $("#groupMemberSubscriptionFailure").hide();
                $("#gpmSuccessMessage").text("User unsubscribed from this group");

            } else if (response == "subscribed") {

                $(chkLabelId).html("Subscribe");
                var currGroupId = $("#hfdGroupId").val();
                var url = "/User/EditGroupPartial?groupId=" + currGroupId;
                $("#member-group-details").load(url);
                $("#groupMemberSubscriptionSuccess").show();
                $("#groupMemberSubscriptionFailure").hide();
                $("#gpmSuccessMessage").text("User subscribed to this group");

            } else {

                e.preventDefault();
                $("#groupMemberSubscriptionFailure").show();
                $("#groupMemberSubscriptionSuccess").hide();
                $("#gpmFailureMessage").text("Operation failed");

            }
            //$('#sl-loadingscreen').hide();
        },
        error: function (x, y, z) {
            var errorMsg = x.responseText;
            event.stopPropagation();
            event.preventDefault();
            $("#groupMemberSubscriptionFailure").show();
            $("#groupMemberSubscriptionSuccess").hide();
            $("#gpmFailureMessage").text(errorMsg.replace(/"/g, ''));
            //$('#sl-loadingscreen').hide();
            return false;
        }
    });

    return false;
});

$(document).on("click", "#groupMemberRoleSave", function (event) {
    var groupMemberRoleDetails = [];

    $('#groupMemberRoleTable').find('tr').each(function () {
        var row = $(this);
        row.find('input[type="checkbox"]').each(function () {

            if (this.checked) {

                var groupMemberRole =
                                {
                                    groupMemberId: $(this).attr("groupMemberId"),
                                    groupProductId: $(this).attr("groupProductId"),
                                    roleId: $(this).attr("roleId"),
                                    isSubscribed: 1
                                };
                groupMemberRoleDetails.push(groupMemberRole);
            }
        })
    });

    var groupMemberRoleDetailsSave =
        {
            groupId: $("#hfdGroupId").val(),
            groupMemberRoleDetails: groupMemberRoleDetails
        }

    var groupMemberRoleDetailsSaveJSON = JSON.stringify(groupMemberRoleDetailsSave);
    //$('#sl-loadingscreen').show();
    $.ajax({
        type: "POST",
        url: "/api/Group/SaveGroupMemberRole/",
        data: groupMemberRoleDetailsSaveJSON,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + $.cookie('token'));
        },
        success: function (response) {
            var currGroupId = $("#hfdGroupId").val();
            var url = "/User/EditGroupPartial?groupId=" + currGroupId;
            $("#member-group-details").load(url);
            //$('#sl-loadingscreen').hide();
            //$("#successMessageGroupInfoSave").show();
        },
        error: function (x, y, z) {
            //$('#sl-loadingscreen').hide();
            alert("error");
        }
    });
    return false;

});
$(document).on("click", "#groupMemberRoleCancel", function (e) {

    var groupId = $("#hfdGroupId").val();
    var url = "";
    if (groupId == 0) {
        url = "/User/AddGroupPartial";


    } else {
        url = "/User/EditGroupPartial?groupId=" + groupId;
    }
    $("#member-group-details").load(url);
    return false;
});