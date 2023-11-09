"use strict";

var AccountAPI = function () {

	var urlIndex = "/Dashboard/Index/";
	var urlSignUp = "/Account/SignUp/";
	var urlSignIn = "/Account/SignIn/";
	var urlForgotPassword = "/Account/ForgotPassword/";
	var urlResetPassword = "/Account/ResetPassword/";
	var urlChangePassword = "/Account/ChangePassword/";
	var urlProfile = "/Account/Profile/";
	var urlChangeAvatar = "/Account/ChangeAvatar/";
	var urlGetAvatar = "/Account/GetAvatar/";
	var urlListarLogAcessoUsuario = "/Account/ListarLogAcessoUsuario/";
	var urlTwoSteps = "/Account/TwoSteps/";
	var urlResendSecurityCode = "/Account/ResendSecurityCode/";

	var dataTable = null;

	var _SignInForm = function () {
		var validation;
		var form = document.getElementById('kt_sign_in_form');
				
		validation = FormValidation.formValidation(
			form,
			{
				fields: {
					email: {
						validators: {
							notEmpty: {
								message: 'O endereço de email é obrigatório!'
							},
							regexp: {
								regexp: /^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$/,
								message: 'Esse não é um endereço de email válido!',
							}
						}
					},
					password: {
						validators: {
							notEmpty: {
								message: 'A senha é obrigatória!'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
				}
			}
		);

		_ValidateSelect2(validation);

		$('#kt_sign_in_submit').on('click', function (e) {
			e.preventDefault();

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					_BlockPage();

					var email = $('#kt_sign_in_form input[name="email"]').val();
					var password = $('#kt_sign_in_form input[name="password"]').val();

					var model = {};

					model.Email = email;
					model.Password = password;

					$.ajax({
						url: urlSignIn,
						type: 'POST',
						contentType: 'application/json',
						data: JSON.stringify(model),
						success: function (data) {
							
							if (data.Ok) {
								window.location.href = urlTwoSteps + email;
							}
							else {
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
									KTUtil.scrollTop();
								});
							}

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

	}

	var _SignInTwoStepsForm = function () {
		var validation;
		var form = document.getElementById('kt_sing_in_two_steps_form');

		validation = FormValidation.formValidation(
			form,
			{
				fields: {
					securitycode: {
						validators: {
							notEmpty: {
								message: 'O código é obrigatório!'
							},
							regexp: {
								regexp: /^([0-9]{6})$/,
								message: 'O código é inválido!',
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
				}
			}
		);

		_ValidateSelect2(validation);

		Inputmask({
			mask: '999999',
			autoUnmask: true
		}).mask('#securitycode');

		$('#kt_sing_in_two_steps_submit').on('click', function (e) {
			e.preventDefault();

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					_BlockPage();
					
					var urlRedirect = getCookie('redirectUrl');

					var userName = $('#hdn-user-name').val();
					var code = $('#securitycode').val();

					var model = {};

					model.UserName = userName;
					model.Code = code;

					$.ajax({
						url: urlTwoSteps,
						type: 'POST',
						contentType: 'application/json',
						data: JSON.stringify(model),
						success: function (data) {

							if (data.Ok) {
								if (urlRedirect) window.location.href = urlRedirect;
								else window.location.href = urlIndex;
							}
							else {
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
									KTUtil.scrollTop();
								});
							}

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

		$('#btn-resend-security-code').on('click', function (e) {
			e.preventDefault();
			
			Swal.fire({
				title: 'Tem certeza?',
				html: 'Realmente deseja reenviar o código de segurança?',
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

					var userName = $('#hdn-user-name').val();

					var model = {};

					model.UserName = userName;

					$.ajax({
						url: urlResendSecurityCode,
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

	}

	var _SignUpForm = function (e) {
		var validation;
		var form = document.getElementById('kt_sign_up_form');
				
		validation = FormValidation.formValidation(
			form,
			{
				fields: {
					nome: {
						validators: {
							notEmpty: {
								message: 'O nome é obrigatório!'
							}
						}
					},
					cpf: {
						validators: {
							notEmpty: {
								message: 'O CPF é obrigatório!'
							},
							regexp: {
								regexp: /^([0-9]{11})$/,
								message: 'Esse não é um CPF válido!',
							}
						}
					},
					email: {
						validators: {
							notEmpty: {
								message: 'O endereço de email é obrigatório!'
							},
							regexp: {
								regexp: /^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$/,
								message: 'Esse não é um endereço de email válido!',
							}
						}
					},
					password: {
						validators: {
							notEmpty: {
								message: 'A senha é obrigatória!'
							},
							regexp: {
								regexp: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$*&@#!?%\-\=\\\/\[\]\{\}\(\)])[0-9a-zA-Z$*&@#!?%\-\=\\\/\[\]\{\}\(\)]{8,16}$/,
								message: 'A senha deve ter:<br /> <li>Entre 8 e 16 caracteres</li> <li>Pelo menos 1 letra maiúscula e minúscula</li> <li>Pelo menos 1 número e 1 símbolo</li>',
							}
						}
					},
					cpassword: {
						validators: {
							notEmpty: {
								message: 'A confirmação da senha é obrigatória!'
							},
							identical: {
								compare: function () {
									return form.querySelector('[name="password"]').value;
								},
								message: 'A senha não confere!'
							}
						}
					},
					agree: {
						validators: {
							notEmpty: {
								message: 'Você deve aceitar os termos e condições!'
							}
						}
					},
				},
				plugins: {
					declarative: new FormValidation.plugins.Declarative({
						html5Input: true,
					}),
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
				}
			}
		);

		_ValidateSelect2(validation);

		Inputmask({
			mask: '999.999.999-99',
			autoUnmask: true
		}).mask('#cpf');

		$('#kt_sign_up_submit').on('click', function (e) {
			e.preventDefault();

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					_BlockPage();

					var idEmpresa = $('#slcEmpresa').val();
					var email = $('#kt_sign_up_form input[name="email"]').val();
					var nome = $('#kt_sign_up_form input[name="nome"]').val();
					var cpf = $('#kt_sign_up_form input[name="cpf"]').val();
					var password = $('#kt_sign_up_form input[name="password"]').val();
					var confirmPassword = $('#kt_sign_up_form input[name="cpassword"]').val();

					var model = {};

					model.IdEmpresa = idEmpresa;
					model.Email = email;
					model.Nome = nome;
					model.CPF = cpf;
					model.Password = password;
					model.ConfirmPassword = confirmPassword;

					$.ajax({
						url: urlSignUp,
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
									window.location.href = urlSignIn;
								}
								else {
									KTUtil.scrollTop();
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

	}

	var _ForgotForm = function (e) {
		var validation;
		var form = document.getElementById('kt_forgot_form');
		
		validation = FormValidation.formValidation(
			form,
			{
				fields: {
					email: {
						validators: {
							notEmpty: {
								message: 'O endereço de email é obrigatório!'
							},
							regexp: {
								regexp: /^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$/,
								message: 'Esse não é um endereço de email válido!',
							}
						}
					}					
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
				}
			}
		);

		_ValidateSelect2(validation);

		$('#kt_forgot_submit').on('click', function (e) {
			e.preventDefault();

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					_BlockPage();

					var email = $('#kt_forgot_form input[name="email"]').val();

					var model = {};

					model.Email = email;

					$.ajax({
						url: urlForgotPassword,
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
									window.location.href = urlSignIn;
								}
								else {
									KTUtil.scrollTop();
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

	}

	var _ResetForm = function (e) {
		var validation;
		var form = document.getElementById('kt_reset_form');
				
		validation = FormValidation.formValidation(
			form,
			{
				fields: {
					password: {
						validators: {
							notEmpty: {
								message: 'A senha é obrigatória!'
							},
							regexp: {
								regexp: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$*&@#!?%\-\=\\\/\[\]\{\}\(\)])[0-9a-zA-Z$*&@#!?%\-\=\\\/\[\]\{\}\(\)]{8,16}$/,
								message: 'A senha deve ter:<br /> <li>Entre 8 e 16 caracteres</li> <li>Pelo menos 1 letra maiúscula e minúscula</li> <li>Pelo menos 1 número e 1 símbolo</li>',
							}
						}
					},
					cpassword: {
						validators: {
							notEmpty: {
								message: 'A confirmação da senha é obrigatória!'
							},
							identical: {
								compare: function () {
									return form.querySelector('[name="password"]').value;
								},
								message: 'A senha não confere!'
							}
						}
					}
				},
				plugins: {
					declarative: new FormValidation.plugins.Declarative({
						html5Input: true,
					}),
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
				}
			}
		);

		_ValidateSelect2(validation);

		$('#kt_reset_submit').on('click', function (e) {
			e.preventDefault();

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					_BlockPage();

					var email = $('#kt_reset_form input[name="email"]').val();
					var password = $('#kt_reset_form input[name="password"]').val();
					var confirmPassword = $('#kt_reset_form input[name="cpassword"]').val();
					var token = $('#kt_reset_form input[name="token"]').val();

					var model = {};

					model.Email = email;
					model.Password = password;
					model.ConfirmPassword = confirmPassword;
					model.Token = token;

					$.ajax({
						url: urlResetPassword,
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
									window.location.href = urlSignIn;
								}
								else {
									KTUtil.scrollTop();
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
		
	}

	var _ChangeForm = function (e) {
		var validation;
		var form = document.getElementById('kt_change_form');

		validation = FormValidation.formValidation(
			form,
			{
				fields: {
					password: {
						validators: {
							notEmpty: {
								message: 'A senha atual é obrigatória!'
							}
						}
					},
					npassword: {
						validators: {
							notEmpty: {
								message: 'A nova senha é obrigatória!'
							},
							regexp: {
								regexp: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[$*&@#!?%\-\=\\\/\[\]\{\}\(\)])[0-9a-zA-Z$*&@#!?%\-\=\\\/\[\]\{\}\(\)]{8,16}$/,
								message: 'A senha deve ter:<br /> <li>Entre 8 e 16 caracteres</li> <li>Pelo menos 1 letra maiúscula e minúscula</li> <li>Pelo menos 1 número e 1 símbolo</li>',
							}
						}
					},
					cpassword: {
						validators: {
							notEmpty: {
								message: 'A confirmação da senha é obrigatória!'
							},
							identical: {
								compare: function () {
									return form.querySelector('[name="npassword"]').value;
								},
								message: 'A senha não confere!'
							}
						}
					}
				},
				plugins: {
					declarative: new FormValidation.plugins.Declarative({
						html5Input: true,
					}),
					trigger: new FormValidation.plugins.Trigger,
					bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row" })
				}
			}
		);

		_ValidateSelect2(validation);

		$('#kt_change_submit').on('click', function (e) {
			e.preventDefault();

			validation.validate().then(function (status) {
				if (status == 'Valid') {

					Swal.fire({
						title: 'Tem certeza?',
						html: 'Realmente deseja editar sua senha?',
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

							var password = $('#kt_change_form input[name="password"]').val();
							var newPassword = $('#kt_change_form input[name="npassword"]').val();
							var confirmPassword = $('#kt_change_form input[name="cpassword"]').val();

							var model = {};

							model.CurrentPassword = password;
							model.NewPassword = newPassword;
							model.ConfirmNewPassword = confirmPassword;

							$.ajax({
								url: urlChangePassword,
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
											window.location.href = urlProfile;
										}
										else {
											KTUtil.scrollTop();
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

	}

	var _ProfileForm = function (e) {

		var model = null;
		var modelOriginal = null;

		var data = $.ajax({
			async: false,
			url: urlGetAvatar,
			type: 'GET',			
		});

		var foto = data.responseJSON.Foto;

		if (foto) {
			modelOriginal = {
				IdUsuario: 0,
				Nome: foto.Nome,
				Extensao: foto.Extensao,
				Tamanho: foto.Tamanho,
				Tipo: foto.Tipo,
				Base64: foto.Base64
			}

			model = modelOriginal;
		}

		var imageInputElement = document.querySelector("#kt_image_input_control");
		var imageInput = KTImageInput.getInstance(imageInputElement);

		imageInput.on("kt.imageinput.changed", async function (element) {

			var input = element.getInputElement();
			var file = input.files[0];
			
			model = {
				IdUsuario: 0,
				Nome: file.name,
				Extensao: file.name.split('.')[1],
				Tamanho: file.size,
				Tipo: file.type,
				Base64: await _ConvertFileToBase64(file)
			};
			
		});

		imageInput.on("kt.imageinput.canceled", function () {

			model = modelOriginal;

		});

		imageInput.on("kt.imageinput.removed", function () {

			model = null;

		});

		$('#kt_profile_submit').on('click', function (e) {
			e.preventDefault();

			Swal.fire({
				title: 'Tem certeza?',
				html: 'Realmente deseja editar sua foto do perfil?',
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

					$.ajax({
						url: urlChangeAvatar,
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
									window.location.href = urlProfile;
								}
								else {
									KTUtil.scrollTop();
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

		if ($('#Acessos')) {

			var options = _DefaultDataTablesOptions(urlListarLogAcessoUsuario);

			options.order = [[5, "desc"]]

			options.columns = [
				{
					data: 'EnderecoIP',
					title: 'Endereço IP',
					orderable: false,
					searchable: false,
				},
				{
					data: 'Dispositivo',
					title: 'Dispositivo',
					orderable: false,
					searchable: false,
				},
				{
					data: 'Plataforma',
					title: 'Plataforma',
					orderable: false,
					searchable: false,
				},
				{
					data: 'Navegador',
					title: 'Navegador',
					orderable: false,
					searchable: false,
				},
				{
					data: 'Status',
					title: 'Status',
					orderable: false,
					searchable: false,
					className: 'text-center',
					render: function (data, type, row) {

						return `<div class="badge ${data ? 'badge-light-success' : 'badge-light-danger'}">${data ? 'Sucesso' : 'Erro'}</div>`;

					},
				},
				{
					data: 'Data',
					title: 'Data',
				},			
			];

			dataTable = $('#kt_datatable').DataTable(options);

			dataTable.on('draw', function () {

				KTMenu.createInstances();

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

		};

	}

	function getCookie(cname) {
		let name = cname + "=";
		let decodedCookie = decodeURIComponent(document.cookie);
		let ca = decodedCookie.split(';');
		for (let i = 0; i < ca.length; i++) {
			let c = ca[i];
			while (c.charAt(0) == ' ') {
				c = c.substring(1);
			}
			if (c.indexOf(name) == 0) {
				return c.substring(name.length, c.length);
			}
		}
		return "";
	}

	return {
		initSignIn: function () {
			_SignInForm();
		},
		initSignInTwoSteps: function () {
			_SignInTwoStepsForm();
		},
		initSignUp: function () {
			_SignUpForm();
		},
		initForgot: function () {
			_ForgotForm();
		},
		initReset: function () {
			_ResetForm();
		},
		initChange: function () {
			_ChangeForm();
		},
		initProfile: function () {
			_ProfileForm();
		}
	};
}();
