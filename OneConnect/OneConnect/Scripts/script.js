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
            },
            error: function (x, y, z) {
                $("#errorMessage").show();
            }
        });
        return false;

    }
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
            $.cookie('userId', null, { path: '/' });
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
$(document).on("click", "#AddNewGroup", function (event) {
    $("#member-group-master").hide();
   
    $("#account-settings-btn").hide();
    
    var url = $("img.addNewGroups").attr("data-member");

    $("#member-group-details").load(url);

    $("#member-group-details").show();

    /*if ($("#AddNewGroup").hasClass("selected")) {
        $("img.addNewGroups").attr("src", "/Content/site/pluse.png");
        $("span.addNewGroups").html("Add New Group");
        $("#AddNewGroup").removeClass("selected");
        

    } else {
        $("img.addNewGroups").attr("src", "/Content/site/minus.png");
        $("span.addNewGroups").html("Go To Group List");
        $("#AddNewGroup").addClass("selected");
        $("#member-group-master").hide();
        $("#member-group-details").show();
    }*/

});

$(document).on("click", "#HideAndShowInActive", function (event) {
    

    if ($("#HideAndShowInActive").hasClass("selected")) {
        $("img.HideAndShowInActive").attr("src", "/Content/site/minus.png");
        $("span.HideAndShowInActive").html("Hide Inactive Groups");
        $("#HideAndShowInActive").removeClass("selected");
        

    } else {
        $("img.HideAndShowInActive").attr("src", "/Content/site/pluse.png");
        $("span.HideAndShowInActive").html("Show Inactive Groups");
        $("#HideAndShowInActive").addClass("selected");
    }

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
            $("#successMessageAddedNewUser").show();
        },
        error: function (x, y, z) {
            $("#errorsMessageAddedNewUser").show();
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
            },
            error: function (x, y, z) {
                $("#errorMessagePasswordChange").show();
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
            },
            error: function (x, y, z) {
                $("#errorMessageEmailChange").show();
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
var somethingChanged = false;
$(document).ready(function () {
    $('#accountEditForm input').change(function () {
        somethingChanged = true;
    });
});
$(document).on("click", ".saveAccountInfoEdit", function () {
    
    if (somethingChanged)
    {
        var status = false;
        if ($("#checkboxAccountStatus").is(':checked')) {
            status = true;
        }
        var postData =
        {
            name: $("#accountName").val(),
            company: $("#companyName").val(),
            address: $("#userAddress").val(),
            contact: $("#userContact").val(),
            status: status
        };
        var postDataJSON = JSON.stringify(postData);
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
                $("#successMessageEditAccount").show();
            },
            error: function (x, y, z) {
                alert("error");
            }
        });
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
                },
                error: function (x, y, z) {
                    var errorMsg = x.responseText;
                    $("#failureMessage").text(errorMsg.replace(/"/g, ''));
                    $("#errorMessage").show();
                    $("#successMessage").hide();
                    $("#username-pass").show();
                    $("#username-pass-redirect").hide();
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
            },
            error: function (x, y, z) {
                var errorMsg = x.responseText;
                $("#failureMessage").text(errorMsg.replace(/"/g, ''));
                $("#errorMessage").show();
                $("#successMessage").hide();
                $("#username-pass").show();
                $("#username-pass-redirect").hide();
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
        },
        error: function (x, y, z) {
            alert("error");
            $("#errorMessage").show();
            $("#successMessage").hide();
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
        },
        error: function (x, y, z) {
            alert("error");
            $("#errorMessage").show();
            $("#successMessage").hide();
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





$("#addNewGroupForm").submit(function (e) {
    e.preventDefault();
}).validate({
    rules: {
        inputGroupName: { required: true }
    },
    messages: {
        inputGroupName: {
            required: "Please enter group name"
        }
    },
    tooltip_options: {
        inputGroupName: { trigger: 'focus' }
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    submitHandler: function () {

        var forgotPassword =
       {
           groupName: $("#inputGroupName").val()
       };
        var dataforgotPassword = JSON.stringify(forgotPassword);
        //$.ajax({
        //    type: "POST",
        //    url: "/api/Account/ForgotPassword/",
        //    data: dataforgotPassword,
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    success: function (response) {
        //        $("#errorMessage").hide();
        //        //$("#successMessage").show();
        //        $("#userId").val("");
        //        $("#emailId").val("");
        //        grecaptcha.reset();
        //        $("#username-pass").hide();
        //        $("#username-pass-redirect").show();
        //    },
        //    error: function (x, y, z) {
        //        var errorMsg = x.responseText;
        //        $("#failureMessage").text(errorMsg.replace(/"/g, ''));
        //        $("#errorMessage").show();
        //        $("#successMessage").hide();
        //        $("#username-pass").show();
        //        $("#username-pass-redirect").hide();
        //    }
        //});
        return false;

    }
});

$(document).on("click", "#saveAddUser", function (event) {
    alert(1);
    $("#addNewGroupForm").submit();
    alert(2);

});
