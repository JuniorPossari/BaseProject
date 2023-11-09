// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var page = null;
var content = null;

var _BlockPage = function (message = "Carregando...") {

	if (page == null) {

		var target = document.querySelector("#kt_body");

		page = new KTBlockUI(target, {
			zIndex: 9999,
			overlayClass: "bg-dark bg-opacity-25",
			message: '<div class="blockui-message"><span class="spinner-border text-primary"></span>' + message + '</div>'
		});

		if (!page.isBlocked()) {
			page.block();
		}

	}

};

var _UnblockPage = function () {

	if (page != null) {

		if (page.isBlocked()) {
			page.release();
		}

		page.destroy();
		page = null;

	}

};

var _BlockContent = function (message = "Carregando...") {

	if (content == null) {

		var target = document.querySelector("#kt_content");

		content = new KTBlockUI(target, {
			zIndex: 9998,
			message: '<div class="blockui-message"><span class="spinner-border text-primary"></span>' + message + '</div>'
		});

		if (!content.isBlocked()) {
			content.block();
		}

	}

};

var _UnblockContent = function () {

	if (content != null) {

		if (content.isBlocked()) {
			content.release();
		}

		content.destroy();
		content = null;

	}

};

async function _ConvertFileToBase64(file) {

	var result_base64 = '';

	var dataURL = file.dataURL;

	if (dataURL && !dataURL.includes('file.png')) {

		dataURL = dataURL.replace(' ', '');

		result_base64 = dataURL.split(',')[1];

		return result_base64;

	}

	result_base64 = await new Promise(async (resolve) => {

		let fileReader = new FileReader();

		fileReader.onload = function (e) {
			
			var arrayBuffer = e.target.result,
				array = new Uint8Array(arrayBuffer),
				base64String = btoa(_Uint8ToString(array));

			resolve(base64String);

		};

		fileReader.readAsArrayBuffer(file);

	});

	return result_base64;
};

async function _CreateBlob(file) {

	var str = await _ConvertFileToBase64(file);

	var s = window.atob(str);

	var bytes = new Uint8Array(s.length);

	for (var i = 0; i < s.length; i++) {
		bytes[i] = s.charCodeAt(i);
	}

	return new Blob([bytes], { type: file.type });

};

function _Uint8ToString(u8a) {

	var CHUNK_SZ = 0x8000;

	var c = [];

	for (var i = 0; i < u8a.length; i += CHUNK_SZ) {
		c.push(String.fromCharCode.apply(null, u8a.subarray(i, i + CHUNK_SZ)));
	}

	return c.join("");

};
