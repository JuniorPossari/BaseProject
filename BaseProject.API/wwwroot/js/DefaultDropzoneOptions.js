"use strict";

const _DefaultDropzoneOptions = function (dropzoneClass, preloadFiles, blockClick) {

	return {
		url: '/Service/FileCallback',
		paramName: "file",
		maxFiles: 10,
		maxFilesize: 10, // MB
		addRemoveLinks: true,
		dictDefaultMessage: "Arraste e solte os arquivos aqui ou clique para fazer upload",
		dictFallbackMessage: "Seu navegador não suporta fazer upload de arquivos com drag'n'drop!",
		dictFallbackText: "Use o formulário alternativo abaixo para fazer upload de seus arquivos como antigamente.",
		dictFileTooBig: "O arquivo é muito grande ({{filesize}}MB)! Tamanho máximo permitido: {{maxFilesize}}MB.",
		dictInvalidFileType: "Tipo de arquivo inválido!",
		dictResponseError: "Servidor respondeu com código {{statusCode}}!",
		dictCancelUpload: "Cancelar",
		dictCancelUploadConfirmation: "Tem certeza de que deseja cancelar este upload?",
		dictRemoveFile: "Remover",
		dictMaxFilesExceeded: "Não é possível carregar mais arquivos!",
		init: function () {

			$(dropzoneClass)[0].dropzone.on("complete", function (file) {

				file.previewElement.addEventListener("click", () => {

					Swal.fire({
						title: 'Tem certeza?',
						html: 'Realmente deseja baixar esse arquivo?',
						icon: "warning",
						confirmButtonText: "Sim",
						showCancelButton: true,
						cancelButtonText: "Não",
						customClass: {
							confirmButton: "btn font-weight-bold btn-primary",
							cancelButton: "btn font-weight-bold btn-secondary"
						}
					}).then(async function (result) {
						if (result.isConfirmed) {
							
							_BlockPage();

							var url = window.URL.createObjectURL(await _CreateBlob(file));
							var a = document.createElement('a');
							a.href = url;
							document.body.appendChild(a);
							a.download = file.name
							a.click();
							a.remove();

							_UnblockPage();							

						}
					});

				});

				if (blockClick) $(".dz-remove").addClass('d-none');

			});

			if (preloadFiles) {
				
				if (!Array.isArray(preloadFiles)) preloadFiles = [preloadFiles];

				for (var i = 0; i < preloadFiles.length; i++) {
					
					var mockFile = {
						name: preloadFiles[i].Nome,
						size: preloadFiles[i].Tamanho,
						type: preloadFiles[i].Tipo,
						dataURL: 'data:' + preloadFiles[i].Tipo + ';base64,' + preloadFiles[i].Base64
					};
					
					$(dropzoneClass)[0].dropzone.files.push(mockFile);
					
					$(dropzoneClass)[0].dropzone.emit("addedfile", mockFile);
					if (mockFile.type.includes('image/'))
						$(dropzoneClass)[0].dropzone.emit("thumbnail", mockFile, mockFile.dataURL);
					else
						$(dropzoneClass)[0].dropzone.emit("thumbnail", mockFile, '/img/file.png');
					$(dropzoneClass)[0].dropzone.emit("complete", mockFile);

				}

			}

			if (blockClick) {
				$(dropzoneClass)[0].dropzone.disable();
			}
		},
		addedFile: (file) => {
			console.log(file);
		},
		accept: function (file, done) {
			done();
		},
		error: (file, message) => {

			$(dropzoneClass)[0].dropzone.removeFile(file);

			Swal.fire({
				title: "Erro",
				html: message,
				icon: "error",
				confirmButtonText: "Ok",
				customClass: {
					confirmButton: "btn font-weight-bold btn-primary"
				}
			});

		}
	};
	
};

const _ValidateDropzone = function (validation) {
	$('form .dropzone-validate')[0].dropzone.on('addedfile', function () {

		var name = $(this.element).attr("name");
		validation.revalidateField(name);

	});

	$('form .dropzone-validate')[0].dropzone.on('removedfile', function () {

		var name = $(this.element).attr("name");
		validation.revalidateField(name);

	});
};