"use strict";

var EmpresaAPI = function () {

	var urlListar = "/Gerenciamento/Empresa/Listar/";
	var urlIndex = "/Gerenciamento/Empresa/Index/";	
	var urlAdicionar = "/Gerenciamento/Empresa/Adicionar/";
	var urlVisualizar = "/Gerenciamento/Empresa/Visualizar/";
	var urlEditar = "/Gerenciamento/Empresa/Editar/";
	var urlDeletar = "/Gerenciamento/Empresa/Deletar/";

	var idEmpresa = $('#HdnIdEmpresa').val();
    var dataTable = null;
	var validation = null;
	var dropzone = null;
	var files = null;

    var filters = {
        DataCadastro: null,
        Ativa: null,
    };

    var initDataTable = function () {

        var options = _DefaultDataTablesOptions(urlListar, filters);

        options.columns = [
            {
                data: 'Id',
                title: 'Id',
            },
            {
                data: 'NomeFantasia',
                title: 'Nome',
            },
            {
                data: 'CNPJ',
                title: 'CNPJ',
            },
            {
                data: 'DataCadastro',
                title: 'Data Cadastro',
            },
            {
                data: 'Ativa',
                title: 'Status',
                orderable: false,
                searchable: false,
                className: 'text-center',
				render: function (data, type, row) {

					return `<div class="badge ${data ? 'badge-light-success' : 'badge-light-danger'}">${data ? 'Ativa' : 'Inativa'}</div>`;

				},
            },
            {
                data: null,
                title: 'Ações',
                orderable: false,
                searchable: false,
                className: 'text-center',
                render: _DefaultDataTablesActionsTemplate,
            },
        ];

        dataTable = $('#kt_datatable').DataTable(options);

		dataTable.on('draw', function () {

			KTMenu.createInstances();
			onClickEditarLinha();
			onClickVisualizarLinha();
			onClickDeletarLinha();

        });

		dataTable.on('error.dt', function () {

            Swal.fire({
                title: 'Erro',
                html: 'Erro na requisição! Atualize a página e tente novamente, caso o erro persista contate um administrador.',
                icon: "error",
                confirmButtonText: "Ok",
                customClass: {
                    confirmButton: "btn font-weight-bold btn-primary"
                }
			});

        });

		$('#kt_datatable_search').on('keyup', function (e) {

			dataTable.search(e.target.value).draw();

        });
        
	}

	var onClickEditarLinha = function () {
		$('.editar').on('click', function () {
			
			var id = $(this).data('id');
			window.location.href = urlEditar + id;

		});
	};

	var onClickVisualizarLinha = function () {
		$('.visualizar').on('click', function () {

			var id = $(this).data('id');
			window.location.href = urlVisualizar + id;

		});
	};

	var onClickDeletarLinha = function () {
		$('.deletar').on('click', function () {

			var id = $(this).data('id');

			Swal.fire({
				title: 'Tem certeza?',
				html: 'Realmente deseja deletar essa empresa?',
				icon: "warning",
				confirmButtonText: "Sim",
				showCancelButton: true,
				cancelButtonText: "Não",
				customClass: {
					confirmButton: "btn font-weight-bold btn-danger",
					cancelButton: "btn font-weight-bold btn-secondary"
				}
			}).then(async function (result) {
				if (result.isConfirmed) {

					_BlockPage();

					$.ajax({
						url: urlDeletar,
						type: 'POST',
						contentType: 'application/json',
						data: JSON.stringify(id),
						success: function (data) {

							_UnblockPage();

							if (data.Ok) {
								dataTable.draw();
							}

							Swal.fire({
								title: data.Title,
								html: data.Message,
								icon: data.Ok ? "success" : "error",
								confirmButtonText: "Ok",
								customClass: {
									confirmButton: "btn font-weight-bold btn-primary"
								}
							});

						},
						error: function () {

							_UnblockPage();

							Swal.fire({
								title: "Aviso",
								html: "Desculpe, houve um erro na requisição!",
								icon: "error",
								confirmButtonText: "Ok",
								customClass: {
									confirmButton: "btn font-weight-bold btn-primary"
								}
							}).then(function () {
								KTUtil.scrollTop();
							});

						}
					});

				}
			});

		});
	};

    var initDateRangePicker = function () {
        $('#drp_cadastro').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: false,
            locale: _DefaultDataRangePickerLocale,
            minYear: 1901,
            applyButtonClasses: 'btn btn-primary',
            cancelButtonClasses : 'btn btn-secondary',
        }).on("apply.daterangepicker", function (e, picker) {

            picker.element.val(picker.startDate.format(picker.locale.format));
            filters.DataCadastro = $(this).val();
            dataTable.draw();

        }).on('cancel.daterangepicker', function (e, picker) {

            picker.element.val('');
            filters.DataCadastro = $(this).val();
            dataTable.draw();

        });
	};

    var onChangeStatus = function () {
		$('#slc_status').on('change', function () {

            filters.Ativa = $(this).val();
			dataTable.draw();

        });
    };

	var initInputmask = function () {

        Inputmask({
			mask: '99.999.999/9999-99',
			autoUnmask: true
		}).mask('[name="cnpj"]');

	};

	var initDropzone = async function (dropzoneClass, preloadFiles = null, blockClick = false) {
		
		var options = _DefaultDropzoneOptions(dropzoneClass, preloadFiles, blockClick);

		options.maxFiles = 1;
		options.acceptedFiles = 'image/jpeg,image/png';
		
		dropzone = new Dropzone(dropzoneClass, options);

	};

	var initValidation = function () {

		var form = document.getElementById('frmEmpresa');

		validation = FormValidation.formValidation(
			form,
			{
				fields: {
					razao_social: {
						validators: {
							notEmpty: {
								message: 'A razão social é obrigatória!'
							},
						}
					},
					nome_fantasia: {
						validators: {
							notEmpty: {
								message: 'O nome fantasia é obrigatório!'
							},
						}
					},
					cnpj: {
						validators: {
							notEmpty: {
								message: 'O CNPJ é obrigatório!'
							},
							regexp: {
								regexp: /^([0-9]{14})$/,
								message: 'Esse não é um CNPJ válido!',
							}
						}
					},
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
				}
			}
		);

		_ValidateSelect2(validation);

    }

    var onClickAdicionar = function () {
		$('#btnAdicionar').on('click', function (e) {

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					Swal.fire({
						title: 'Tem certeza?',
						html: 'Realmente deseja adicionar essa empresa?',
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

							var model = {};

							model.Id = idEmpresa;
							model.RazaoSocial = $('[name="razao_social"]').val();
							model.NomeFantasia = $('[name="nome_fantasia"]').val();
							model.CNPJ = $('[name="cnpj"]').val();
							model.Ativa = $('[name="ativa"]').is(':checked');

							var files = dropzone.files;

							if (files && files.length > 0) {

								var file = files[0];
								var base64 = await _ConvertFileToBase64(file);
								
								model.EmpresaLogo = {
									Nome: file.name,
									Extensao: file.name.split('.').pop(),
									Tamanho: file.size,
									Tipo: file.type,
									Base64: base64
								};

							}

							$.ajax({
								url: urlAdicionar,
								type: 'POST',
								contentType: 'application/json',
								data: JSON.stringify(model),
								success: function (data) {

									_UnblockPage();

									Swal.fire({
										title: data.Title,
										html: data.Message,
										icon: data.Ok ? "success" : "error",
										confirmButtonText: "Ok",
										customClass: {
											confirmButton: "btn font-weight-bold btn-primary"
										}
									}).then(function () {
										if (data.Ok) {
											window.location.href = urlIndex;
										}
									});

								},
								error: function () {

									_UnblockPage();

									Swal.fire({
										title: "Aviso",
										html: "Desculpe, houve um erro na requisição!",
										icon: "error",
										confirmButtonText: "Ok",
										customClass: {
											confirmButton: "btn font-weight-bold btn-primary"
										}
									}).then(function () {
										KTUtil.scrollTop();
									});

								}
							});

						}
					});

				} else {
					Swal.fire({
						title: "Aviso",
						html: "Você deve preencher todos os campos obrigatórios!",
						icon: "error",
						confirmButtonText: "Ok",
						customClass: {
							confirmButton: "btn font-weight-bold btn-primary"
						}
					}).then(function () {
						KTUtil.scrollTop();
					});
				}
			});

		});
	};

	var onClickEditar = function () {
		$('#btnEditar').on('click', function (e) {

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					Swal.fire({
						title: 'Tem certeza?',
						html: 'Realmente deseja editar essa empresa?',
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

							var model = {};

							model.Id = idEmpresa;
							model.RazaoSocial = $('[name="razao_social"]').val();
							model.NomeFantasia = $('[name="nome_fantasia"]').val();
							model.CNPJ = $('[name="cnpj"]').val();
							model.Ativa = $('[name="ativa"]').is(':checked');

							var files = dropzone.files;

							if (files && files.length > 0) {

								var file = files[0];
								var base64 = await _ConvertFileToBase64(file);

								model.EmpresaLogo = {
									Nome: file.name,
									Extensao: file.name.split('.').pop(),
									Tamanho: file.size,
									Tipo: file.type,
									Base64: base64
								};

							}
							
							$.ajax({
								url: urlEditar,
								type: 'POST',
								contentType: 'application/json',
								data: JSON.stringify(model),
								success: function (data) {

									_UnblockPage();

									Swal.fire({
										title: data.Title,
										html: data.Message,
										icon: data.Ok ? "success" : "error",
										confirmButtonText: "Ok",
										customClass: {
											confirmButton: "btn font-weight-bold btn-primary"
										}
									}).then(function () {
										if (data.Ok) {
											window.location.href = urlIndex;
										}
									});

								},
								error: function () {

									_UnblockPage();

									Swal.fire({
										title: "Aviso",
										html: "Desculpe, houve um erro na requisição!",
										icon: "error",
										confirmButtonText: "Ok",
										customClass: {
											confirmButton: "btn font-weight-bold btn-primary"
										}
									}).then(function () {
										KTUtil.scrollTop();
									});

								}
							});

						}
					});

				} else {
					Swal.fire({
						title: "Aviso",
						html: "Você deve preencher todos os campos obrigatórios!",
						icon: "error",
						confirmButtonText: "Ok",
						customClass: {
							confirmButton: "btn font-weight-bold btn-primary"
						}
					}).then(function () {
						KTUtil.scrollTop();
					});
				}
			});

		});
	};
	
	return {
		initIndex: function () {
            initDataTable();
			initDateRangePicker();
            onChangeStatus();
		},
		initAdicionar: function () {
			initDropzone('#kt_dropzone');
			initInputmask();
			initValidation();
			onClickAdicionar();
		},
		initEditar: function (preloadFiles) {
			initDropzone('#kt_dropzone', preloadFiles);
			initInputmask();
			initValidation();
			onClickEditar();
		},
		initVisualizar: function (preloadFiles) {
			initDropzone('#kt_dropzone', preloadFiles, true);
		}
	};
}();
