$(document).on("click", "#submitRegister", function (event) {
    alert("ok");
    
    $.ajax({
        type: "POST",
        url: "api/AccountApi/Register/",
        data: "{id:2}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            alert(msg);
        },
        error: function (err) {
            alert(err.toString());
            
        }
    });
    return false;
});