﻿var bundle = function (config) {
    var the = this;

    the._version = config.version;
    the._frameworkType = config.framework;
    the._sessionPath = config.sessionPath;

    the.RequestType = {
        GET: 'GET',
        POST: 'POST',
        PUT: 'PUT',
        DELETE: 'DELETE',
        PATCH: 'PATCH'
    }

    the.ResponseType = {
        HTML: 'text',
        JSON: 'json'
    }

    the._Helpers = function () {

        var _GetFormattedDate = function (date) {
            if (date !== undefined && date != null && date != '') {
                let dateArray = date.split('/');
                date = dateArray[2] + '-' + dateArray[1] + '-' + dateArray[0];
                return date;
            }
            return null;

        }

        var _InitDateTimePicker = function () {

            $(".bundle-datepicker").datepicker({
                format: 'dd/mm/yyyy',
                width: '100%',
                language: 'pt-BR'
            });
        }

        var _VerifyDateTimeRange = function (firstDateSelector, secondDateSelector) {
            var StartDateObject = Date.parse(_GetFormattedDate($(firstDateSelector).val()));
            var EndDateObject = Date.parse(_GetFormattedDate($(secondDateSelector).val()));

            let isRangeInvalid = (isNaN(StartDateObject) && isNaN(EndDateObject))
            //return true;

            return {
                IsRangeInvalid: isRangeInvalid,
                IsStartValid: StartDateObject < EndDateObject,
                IsEndValid: EndDateObject > StartDateObject

            }

        }

        var _ConvertFileToBase64Syncronous = function (file) {
            var url = URL.createObjectURL(file);//Create Object URL
            var xhr = new XMLHttpRequest();
            xhr.open("GET", url, false);//Synchronous XMLHttpRequest on Object URL
            xhr.overrideMimeType("text/plain; charset=x-user-defined");//Override MIME Type to prevent UTF-8 related errors
            xhr.send();
            URL.revokeObjectURL(url);
            var returnText = "";
            for (var i = 0; i < xhr.responseText.length; i++) {
                returnText += String.fromCharCode(xhr.responseText.charCodeAt(i) & 0xff);
            };//remove higher byte
            return "data:" + file.type + ";base64," + btoa(returnText);

        }

        var _ConvertFileToBase64Asyncronous = async function (file) {

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

        var _CreateBlob = async function (file) {
            var str = await _ConvertFileToBase64(file);
            var s = window.atob(str);
            var bytes = new Uint8Array(s.length);

            for (var i = 0; i < s.length; i++) {
                bytes[i] = s.charCodeAt(i);
            }

            return new Blob([bytes], { type: file.type });

        };

        var _Uint8ToString = function (file) {
            var CHUNK_SZ = 0x8000;
            var c = [];

            for (var i = 0; i < u8a.length; i += CHUNK_SZ) {
                c.push(String.fromCharCode.apply(null, u8a.subarray(i, i + CHUNK_SZ)));
            }
            return c.join("");

        };

        return {
            ConvertFileToBase64Asyncronous: _ConvertFileToBase64Asyncronous,
            ConvertFileToBase64Syncronous: _ConvertFileToBase64Syncronous,
            CreateBlob: _CreateBlob,
            Uint8ToString: _Uint8ToString,
            VerifyDateTimeRange: _VerifyDateTimeRange,
            InitDateTimePicker: _InitDateTimePicker,
            getFormatedDate: _GetFormattedDate,
        }
    }();

    the._UIHandler = function () {
        const DefaultSweetAlertOptions = {

            title: "Aviso",
            html: "",
            icon: "warning",
            confirmButtonText: "OK!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-primary"
            }
        }

        const DefaultSweetAlertCofirmationOptions = {
            title: 'Tem certeza?',
            html: 'Realmente deseja confirmar essa ação?',
            icon: "warning",
            confirmButtonText: "Sim",
            showCancelButton: true,
            cancelButtonText: "Não",
            customClass: {
                confirmButton: "btn font-weight-bold btn-primary",
                cancelButton: "btn font-weight-bold btn-secondary"
            }

        }

        let _blockUI = {};

        var _CreateEvent = function (eventOptions) {
            return new Promise((resolve, reject) => {
                if (eventOptions.selector) {
                    $(eventOptions.selector).off(eventOptions.eventName);
                    $(eventOptions.selector).on(eventOptions.eventName, function (event) {
                        
                        if (eventOptions.callback) {
                            eventOptions.callback($(this), event);
                            resolve({
                                element: $(this),
                                event: event
                            })
                        }
                    })
                }
            })
        }
        var _CreateEventFunction = function (eventOptions) {
            return function () {
                return _CreateEvent(eventOptions);
            }
        }

        var _CreateClickEvent = function (eventOptions) {
            eventOptions.eventName = 'click';
            return _CreateEvent(eventOptions)
        }

        var _CreateChangeEvent = function (eventOptions) {
            eventOptions.eventName = 'change';
            return _CreateEvent(eventOptions)
        }

        var _CreateAlertMessage = function (alertOptions) {
            return new Promise((resolve, reject) => {
                Swal.fire({
                    title: (alertOptions.title || DefaultSweetAlertOptions.title),
                    html: (alertOptions.html || DefaultSweetAlertOptions.html),
                    icon: (alertOptions.icon || DefaultSweetAlertOptions.icon),
                    allowOutsideClick: false,
                    confirmButtonText: (alertOptions.confirmButtonText || DefaultSweetAlertOptions.confirmButtonText),
                    customClass: (alertOptions.customClass || DefaultSweetAlertOptions.customClass)
                }).then(function () {
                    resolve("Mensagem exibida com sucesso!")
                });
            })
        }

        var _CreateConfirmationMessage = function (alertOptions) {
            return new Promise((resolve, reject) => {
                Swal.fire({
                    title: (alertOptions.title || DefaultSweetAlertCofirmationOptions.title),
                    html: (alertOptions.html || DefaultSweetAlertCofirmationOptions.html),
                    icon: (alertOptions.icon || DefaultSweetAlertCofirmationOptions.icon),
                    allowOutsideClick: false,

                    showCancelButton: true,
                    confirmButtonText: (alertOptions.confirmButtonText || DefaultSweetAlertCofirmationOptions.confirmButtonText),
                    cancelButtonText: (alertOptions.cancelButtonText || DefaultSweetAlertCofirmationOptions.cancelButtonText),
                    customClass: (alertOptions.customClass || DefaultSweetAlertCofirmationOptions.customClass)
                }).then(function (result) {
                    if (result.isConfirmed) {


                        if (alertOptions.confirmAction)
                            alertOptions.confirmAction();
                        resolve("Mensagem exibida com sucesso!")
                    }
                });
            })

        }

        var _BlockPage = function () {
            if (the._version == '8')
                _BlockPageMetronic8();
            else
                _BlockPageMetronic7();
        }

        var _UnblockPage = function () {
            if (the._version == '8')
                _UnblockPageMetronic8();
            else
                _UnblockPageMetronic7();

        }

        var _UnblockPageMetronic8 = function () {
            if (_blockUI.isBlocked())
                _blockUI.release(); _blockUI.destroy();

        }

        var _UnblockPageMetronic7 = function () {

            KTApp.unblockPage();

        }

        var _BlockPageMetronic8 = function () {

            let blockUI = new KTBlockUI(document.querySelector('#kt_body'), {
                message: '<div class="blockui-message"><span class="spinner-border text-primary"></span> Carregando...</div>',
            });

            _blockUI = blockUI;
            _blockUI.block();
        }

        var _BlockPageMetronic7 = function () {
            KTApp.blockPage({
                overlayColor: 'white',
                opacity: 0.1,
                state: 'primary' // a bootstrap color
            });
        }

        return {
            CreateMessage: _CreateAlertMessage,
            CreateDialog: _CreateConfirmationMessage,
            UnblockPage: _UnblockPage,
            BlockPage: _BlockPage,
            CreateClickEvent: _CreateClickEvent,
            CreateChangeEvent: _CreateChangeEvent,
            CreateEvent: _CreateEvent,
            CreateEventFunction: _CreateEventFunction
        }


    }();

    the._FormControl = function () {

        const ValidationType = {
            CPF: 'cpf',
            Email: 'email',
            DateRange: 'DateRange'
        }

        let initInnerValidations = function () {
            document.addEventListener("DOMContentLoaded", function (event) {
                FormValidation.validators.checkCPF = checkCPF;

            });
        }();

        const ValidateSelect2 = function (validation) {
            $('form .select2-validate').on('change', function () {

                var name = $(this).attr("name");
                validation.revalidateField(name);

            });
        };

        const _emailRegularExpression = {
            regexp: /^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$/,
            message: 'Esse não é um endereço de email válido!',

        }

        const _cpfRegularExpression = {
            regexp: /^([0-9]{11})$/,
            message: 'Esse não é um CPF válido!',
        }

        const checkCPF = function () {
            return {
                validate: function (element) {
                    let cpf = element.value;
                    if (typeof cpf !== "string") return false
                    cpf = cpf.replace(/[\s.-]*/igm, '')
                    if (
                        !cpf ||
                        cpf.length != 11 ||
                        cpf == "00000000000" ||
                        cpf == "11111111111" ||
                        cpf == "22222222222" ||
                        cpf == "33333333333" ||
                        cpf == "44444444444" ||
                        cpf == "55555555555" ||
                        cpf == "66666666666" ||
                        cpf == "77777777777" ||
                        cpf == "88888888888" ||
                        cpf == "99999999999"
                    ) {
                        return {
                            valid: false,
                        };
                    }
                    var soma = 0
                    var resto
                    for (var i = 1; i <= 9; i++)
                        soma = soma + parseInt(cpf.substring(i - 1, i)) * (11 - i)
                    resto = (soma * 10) % 11
                    if ((resto == 10) || (resto == 11)) resto = 0
                    if (resto != parseInt(cpf.substring(9, 10)))
                        return {
                            valid: false,
                        };
                    soma = 0
                    for (var i = 1; i <= 10; i++)
                        soma = soma + parseInt(cpf.substring(i - 1, i)) * (12 - i)
                    resto = (soma * 10) % 11
                    if ((resto == 10) || (resto == 11)) resto = 0
                    if (resto != parseInt(cpf.substring(10, 11)))
                        return {
                            valid: false,
                        };
                    return {
                        valid: true,

                    };
                }
            }
        }

        const AssignFormField = function (formParam, keyName) {
            let validators = {
                notEmpty: { message: (formParam.message || 'Este campo é obrigatório') }
            }

            if (formParam.callback) validators.callback = formParam.callback;

            if (formParam.ValidationType) {
                switch (formParam.ValidationType) {
                    case ValidationType.CPF:
                        validators.regexp = _cpfRegularExpression;
                        validators.checkCPF = { message: 'É necssário inserir um CPF válido!' };
                        break;

                    case ValidationType.Email:
                        validators.regexp = _emailRegularExpression;
                        break;

                    case ValidationType.DateRange:
                        validators.callback = {
                            message: formParam.DateRangeConfig.EndField ? 'A data inicial deve ser menor que data final' : 'A data final deve ser maior que a data inicial',
                            callback: function () {
                                let dateCompareResult = the._Helpers.VerifyDateTimeRange(
                                    formParam.DateRangeConfig.EndField ? '[name="' + keyName + '"]' : '[name="' + formParam.DateRangeConfig.StartField + '"]',
                                    formParam.DateRangeConfig.StartField ? '[name="' + keyName + '"]' : '[name="' + formParam.DateRangeConfig.EndField + '"]'
                                )
                                if (dateCompareResult.IsRangeInvalid)
                                    return true;

                                return (formParam.DateRangeConfig.EndField ? dateCompareResult.IsStartValid : dateCompareResult.IsEndValid);
                            }
                        }
                        break;

                }
            }
            return validators
        }


        var CreateFormValidationObject = function (elementId, formParams, customPlugins) {

            let formConfiguration = {};
            let excludedConfigurations = [];

            Object.keys(formParams).forEach(function (key) {
                formConfiguration[key] = { validators: AssignFormField(formParams[key], key) };
            })


            let validation = FormValidation.formValidation(
                document.getElementById(elementId),
                {
                    fields: formConfiguration,
                    plugins: the._version == '8' ? {
                        trigger: new FormValidation.plugins.Trigger,
                        bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
                    } :
                        {
                            trigger: new FormValidation.plugins.Trigger(),
                            bootstrap: new FormValidation.plugins.Bootstrap(),
                            submitButton: new FormValidation.plugins.SubmitButton(),
                        }
                });


            let validationObject = function () {
                let formValidation = validation;

                let InvokeValidationMethod = function (callback) {
                    formValidation
                        .validate()
                        .then(function (status) {

                            if (status == 'Valid') {
                                if (callback && callback.successAction)
                                    callback.successAction();
                            }
                            else
                                if (callback && callback.rejectAction)
                                    callback.rejectAction();
                        });
                }


                return {
                    form: formValidation,
                    ValidateForm: InvokeValidationMethod
                }



            }();

            ValidateSelect2(validationObject.form);
            return validationObject;

        }
        return {
            CreateFormObject: CreateFormValidationObject,
            ValidationType: ValidationType
        }



    }();

    //MÉTODOS DE CONTROE RESFUL
    the._restFul = function () {


/*        const _DefaultSessionURL = the._version == '8' ? '/Service/DefineSession/' : '/Home/DefineSession/';*/

        const _DefaultHeaderOptions = {
            'Accept': 'application/json, text/plain, */*',
            'Content-Type': 'application/json; charset=utf-8',
            'Response-Type': the.ResponseType.JSON // PICO - usando o camelCase, o fetch não funciona
        };
        const _DeafultRequestOptions = {
            url: "",
            params: {},
            responseType: the.ResponseType.JSON,
            requestType: the.RequestType.GET,
            headers: _DefaultHeaderOptions,
            body: "",
            requestData: "",
        };

        const CreateHeaderOptions = function (headerConfig) {
            const _options = _DefaultHeaderOptions;
            if (headerConfig) {

                Object.keys(headerConfig).forEach(function (key) {
                    _options[key] = !_options[key] || key == 'Response-Type' ? headerConfig[key] : _options[key] + "," + headerConfig[key];
                })
            }
            return _options

        }

        const _buildParams = function (params) {

            const _params = new URLSearchParams();
            if (params) {
                Object.entries(params).map(([key, value]) => {

                    if (Array.isArray(value)) {
                        value.map((_value) => _params.append(key, _value));
                    } else {
                        _params.append(key, value);
                    }
                });
            }
            return _params;
        };

        var CreateFetchOptions = function (fetchOptions) {

            //requestType, body, headers, mode = 'cors', credentials = 'include', cache = 'default', isSessionRequest = false) {

            let options = {
                method: (fetchOptions.method ?? the.RequestType.GET),
                headers: CreateHeaderOptions(fetchOptions.headers),
                mode: (fetchOptions.mode ?? 'cors'),
                credentials: (fetchOptions.credentials ?? 'include'),
                cache: (fetchOptions.cache ?? 'default')
            };

            if (fetchOptions.requestData) options.body = JSON.stringify({ jsonData: JSON.stringify(fetchOptions.requestData) })

            if (fetchOptions.body) options.body = fetchOptions.body;

            return options;

        }

        var CreateRequest = function (url, params, requestType, body, headers, requestData) {

            return new Promise((resolve, reject) => {
                let _fetchOptions = CreateFetchOptions({
                    method: requestType,
                    body: body,
                    headers: headers
                })
                /// 06/02 - A atribuição dessa variavel é necessária; a propriedade headers, no ambiente publicado, tem seus valores nulados.
                let headersConfig = _fetchOptions["headers"];

                _defineSession(requestData, headersConfig)
                    .then(function (data) {
                        the._UIHandler.BlockPage();
                        fetch(url + '?' + _buildParams(params),//isso aqui nao vai ter mais
                            _fetchOptions
                        )
                            .then(function (response) {
                                switch (headersConfig["Response-Type"]) {
                                    case ResponseType.JSON:
                                        return response.json();
                                        break;

                                    case ResponseType.HTML:
                                        return response.text();
                                        break;

                                    default:
                                        return null;
                                        break;

                                }
                            })
                            .then(response => {

                                the._UIHandler.UnblockPage();
                                // PICO (18/11) - esta parte seria a rejeição LÓGICA das requisições, ou seja, fisicamente o status é marcado como 200 (ok), mas o fluxo considera a requisição como fracassada
                                if (response.IsRequestSuccessful) {
                                    switch (headersConfig["Response-Type"]) {
                                        case ResponseType.JSON:
                                            if (response.ShowSuccessMessage) {
                                                the._UIHandler.CreateMessage({
                                                    html: response.Message,
                                                    icon: "success",
                                                }).then(function () {
                                                    resolve(response.Payload);
                                                })
                                            }
                                            else
                                                resolve(response.Payload);
                                            break;

                                        case ResponseType.HTML:
                                            resolve(response)
                                            break;

                                    }
                                }
                                else {
                                    switch (headersConfig["Response-Type"]) {
                                        case ResponseType.JSON:
                                            // PICO (18/11) - esta parte seria a rejeição LÓGICA das requisições, ou seja, fisicamente o status é marcado como 200 (ok), mas o fluxo considera a requisição como fracassada
                                            the._UIHandler.CreateMessage({
                                                html: response.ErrorMessage,
                                                icon: "error",
                                            }).then(function () {
                                                reject(response.Payload);
                                            })
                                            break;

                                        case ResponseType.HTML:
                                            reject(response);
                                            break;

                                    }
                                }
                            })
                            .catch(error => {
                                the._UIHandler.UnblockPage();
                                console.log('Request failed', error);
                                the._UIHandler.CreateMessage({
                                    html: "Ocorreu um erro ao processar essa requisição",
                                    icon: "error",
                                }).then(function () {
                                    reject(error);
                                })

                            });
                    });



            });
        }


        var _defineSession = function (requestData, headers) {

            return new Promise((resolve, reject) => {
                let _fetchOptions = CreateFetchOptions({
                    method: the.RequestType.POST,
                    headers: headers ?? {},
                    requestData: requestData ?? _DeafultRequestOptions.params,
                });

                fetch(the._sessionPath,
                    _fetchOptions
                ).then(data => {
                    console.log(data.json());
                    resolve(data);

                }).catch(error => {
                    the._UIHandler.CreateMessage({
                        html: "Ocorreu um erro ao processar a sessão da aplicação, tente novamente mais tarde!",
                        icon: "error",
                    }).then(function () {
                        reject(error);
                    })

                })



            })
        }

        var _get = function (requestOptions) {

            return CreateRequest(
                (requestOptions.url ?? ''),
                (requestOptions.params ?? _DeafultRequestOptions.params),
                (requestOptions.requestType ?? _DeafultRequestOptions.requestType),
                null,
                (requestOptions.headers ?? {}),
                (requestOptions.data ?? {}),

            )

        }

        var _post = function (requestOptions = _DeafultRequestOptions) {

            return CreateRequest(
                (requestOptions.url ?? ''),
                (requestOptions.params ?? _DeafultRequestOptions.params),
                the.RequestType.POST,
                JSON.stringify(requestOptions.body),
                (requestOptions.headers ?? {}),
                (requestOptions.data ?? {})
            )

        }

        var _delete = function (requestOptions = _DeafultRequestOptions) {

            return CreateRequest(
                requestOptions.url,
                requestOptions.params,
                the.RequestType.DELETE,
                null,
                requestOptions.responseType,
                requestOptions.headers
            )

        }

        var _put = function (requestOptions = _DeafultRequestOptions) {

            return CreateRequest(
                requestOptions.url,
                requestOptions.params,
                the.RequestType.PUT,
                JSON.stringify(requestOptions.body),
                requestOptions.responseType,
                requestOptions.headers
            )

        }

        var _patch = function (requestOptions = _DeafultRequestOptions) {

            return CreateRequest(
                requestOptions.url,
                requestOptions.params,
                the.RequestType.PATCH,
                JSON.stringify(requestOptions.body),
                requestOptions.responseType,
                requestOptions.headers
            )
        }

        return {

            get: _get,
            post: _post,
            put: _put,
            delete: _delete,
            patch: _patch,

        }

    }();

    the._IO = function () {



    }();



    return {
        API: the._restFul,
        UI: the._UIHandler,
        Util: the._Helpers,
        FormControl: the._FormControl,

    }

}({
    version: '8',
    framework: 'dotNet5',
    sessionPath: '/Service/DefineSession/'
});


$(document).ready(function () {
    bundle.Util.InitDateTimePicker();

});