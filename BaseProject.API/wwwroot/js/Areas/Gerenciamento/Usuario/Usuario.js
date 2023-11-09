
console.log(bundle)
var UsuarioAPI = function () {
 
    var urlListar = "/Gerenciamento/Usuario/Listar/";
    var urlIndex = "/Gerenciamento/Usuario/Index/";
    var urlGetTeste = "/Gerenciamento/Usuario/GetTeste/";
    var urlPostTeste = "/Gerenciamento/Usuario/PostTeste/";
    var urlAdicionar = "/Gerenciamento/Usuario/Adicionar/";
    var urlVisualizar = "/Gerenciamento/Usuario/Visualizar/";
    var urlEditar = "/Gerenciamento/Usuario/Editar/";
    var urlDeletar = "/Gerenciamento/Usuario/Deletar/";
    var urlListarS2Empresas = "/Gerenciamento/Empresa/ListarSelect2/";

    var idUsuario = $('#HdnIdUsuario').val();
    var dataTable = null;
    var validation = null;

    var filters = {
        IdEmpresa: null,
        DataCadastro: null,
        Ativo: null,
    };

    var initDataTable = function () {

        var options = _DefaultDataTablesOptions(urlListar, filters);

        options.columns = [
            {
                data: 'Id',
                title: 'Id',
            },
            {
                data: 'Nome',
                title: 'Nome',
            },
            {
                data: 'Email',
                title: 'Email',
            },
            {
                data: 'CPF',
                title: 'CPF',
            },
            {
                data: 'NomeEmpresa',
                title: 'Empresa',
            },
            {
                data: 'DataCadastro',
                title: 'Data Cadastro',
            },
            {
                data: 'Ativo',
                title: 'Status',
                orderable: false,
                searchable: false,
                className: 'text-center',
                render: _DefaultDataTablesStatusTemplate,
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
                html: 'Realmente deseja deletar esse usuário?',
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

    var initSelect2 = function () {

        var options = _DefaultSelect2Options(urlListarS2Empresas);

        options.placeholder = "Empresa";

        $('#slc_empresa').select2(options);

    };

    var onChangeEmpresa = bundle.UI.CreateEventFunction({
        selector: '#slc_empresa',
        eventName: 'change',
        callback: function (e) {

            filters.IdEmpresa = e.val();
            dataTable.draw();

        }
    });

    var initDateRangePicker = function () {
        $('#drp_cadastro').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: false,
            locale: _DefaultDataRangePickerLocale,
            minYear: 1901,
            applyButtonClasses: 'btn btn-primary',
            cancelButtonClasses: 'btn btn-secondary',
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

            filters.Ativo = $(this).val();
            dataTable.draw();

        });
    };

    var initInputmask = function () {

        Inputmask({
            mask: '999.999.999-99',
            autoUnmask: true
        }).mask('[name="cpf"]');

    };

    var initValidation = function () {

        validation = bundle
            .FormControl
            .CreateFormObject('frmUsuario',
            {
                empresa: {},
                nome: {},
                email: {
                    ValidationType: bundle.FormControl.ValidationType.Email
                },
                cpf: {
                    ValidationType: bundle.FormControl.ValidationType.CPF
                },                
            });

    };

    var onClickAdicionar = function () {

        bundle.UI.CreateClickEvent({
            selector: '#btnAdicionar',
            callback: function (element, event) {
                validation.ValidateForm({
                    successAction: function () {
                        bundle.UI.CreateDialog({
                            html: 'Realmente deseja adicionar esse usuário?',
                            confirmAction: function () {

                                var model = {};

                                model.Id = idUsuario;
                                model.IdEmpresa = $('#slc_empresa').val();
                                model.Nome = $('[name="nome"]').val();
                                model.Email = $('[name="email"]').val();
                                model.CPF = $('[name="cpf"]').val();
                                model.Ativo = $('[name="ativo"]').is(':checked');

                                bundle.API.post({
                                    url: urlAdicionar,
                                    body: model
                                }).then(function (data) {
                                    window.location.href = urlIndex;
                                })
                            }

                        })
                    }
                });
            }
        });

    };

    var onClickEditar = function () {

        bundle.UI.CreateClickEvent({
            selector: '#btnEditar',
            callback: function (element, event) {
                validation.ValidateForm({
                    successAction: function () {
                        bundle.UI.CreateDialog({
                            html: 'Realmente deseja editar esse usuário?',
                            confirmAction: function () {

                                var model = {};

                                model.Id = idUsuario;
                                model.IdEmpresa = $('#slc_empresa').val();
                                model.Nome = $('[name="nome"]').val();
                                model.Email = $('[name="email"]').val();
                                model.CPF = $('[name="cpf"]').val();
                                model.Ativo = $('[name="ativo"]').is(':checked');

                                bundle.API.post({
                                    url: urlEditar,
                                    body: model
                                }).then(function (data) {
                                    window.location.href = urlIndex;
                                })
                            }

                        })
                    }
                });
            }
        });
        
    };

    return {
        initIndex: function () {
            initDataTable();
            initSelect2();
            onChangeEmpresa();
            initDateRangePicker();
            onChangeStatus();
        },
        initAdicionar: function () {
            initSelect2();
            initInputmask();
            initValidation();
            onClickAdicionar();
        },
        initEditar: function () {
            initSelect2();
            initInputmask();
            initValidation();
            onClickEditar();
        },
        initVisualizar: function () {
            initSelect2();
        }
    };
}();
