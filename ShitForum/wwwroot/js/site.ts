var v = new aspnetValidation.ValidationService();
v.bootstrap();

 function onload() {
    var element = document.getElementById("SubmitButton");
    element.onclick += validate;
}
onload();

function validate(event) {
    event.preventDefault();
	if (!$("#validation-summary-error").length){
		grecaptcha.execute();
	}
}

function SubmitRegistration(data) {
    if ($("#gRecaptchaResponse").val() == '') {
        $("#gRecaptchaResponse").val(data);
    }
    document.getElementById("post-form").submit();
}  
