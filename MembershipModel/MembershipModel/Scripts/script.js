$(document).on("click", "#submitRegister", function (event) {
    alert("ok");
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