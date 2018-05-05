
var v = new aspnetValidation.ValidationService();
v.bootstrap();

function onSubmit(a)
{
	
}

function onSubmitClick() {
	grecaptcha.execute();
	$("#post-form").submit();
}

document.getElementById("submit").onclick = onSubmitClick;
