"use strict";

const _DefaultSelect2Options = function (urlListarSelect2) {

	return {
		language: "pt-BR",
		allowClear: true,
		ajax: {
			url: urlListarSelect2,
			type: 'POST',
			contentType: 'application/json',
			data: function (model) {
				return JSON.stringify(model);
			},
			delay: 500
		}
	};
	
};

const _ValidateSelect2 = function (validation) {
	$('form .select2-validate').on('change', function () {

		var name = $(this).attr("name");
		validation.revalidateField(name);

	});
};