// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using FluvAuto.Data;
using FluvAuto.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FluvAuto.Areas.Identity.Pages.Account
{
    public class RegisterFModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterFModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Os dados do funcionário são obrigatórios")]
            public Funcionario Funcionario { get; set; } = new Funcionario();

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
            [EmailAddress(ErrorMessage = "Introduza um {0} válido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
            [StringLength(100, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", 
                ErrorMessage = "A password deve conter pelo menos 6 caracteres e: 1 letra minúscula, 1 maiúscula, 1 número e 1 caracter especial")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Password")]
            [Compare("Password", ErrorMessage = "A password e a confirmação da password não coincidem.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "O código secreto é obrigatório.")]
            [Display(Name = "Código secreto da oficina")]
            public string SecretCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, IFormFile fotografiaUpload = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            // Validar o modelo Funcionario manualmente para garantir que as validações são aplicadas
            if (Input.Funcionario != null)
            {
                var validationContext = new ValidationContext(Input.Funcionario);
                var validationResults = new List<ValidationResult>();
                bool isFuncionarioValid = Validator.TryValidateObject(Input.Funcionario, validationContext, validationResults, true);
                
                if (!isFuncionarioValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        foreach (var memberName in validationResult.MemberNames)
                        {
                            ModelState.AddModelError($"Input.Funcionario.{memberName}", validationResult.ErrorMessage);
                        }
                    }
                }
            }
            
            if (ModelState.IsValid)
            {
                if (Input.SecretCode != "MegaPasswordSecreta123qwe#")
                {
                    ModelState.AddModelError("Input.SecretCode", "O código secreto está incorreto.");
                    return Page();
                }

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // +++++++++++++++++++++++++++++++++++++++++++++++
                    // Guardar os dados do Funcionário na base de dados
                    // +++++++++++++++++++++++++++++++++++++++++++++++

                    bool haErro = false;

                    // Associar os dados do utilizador Identity ao Funcionário
                    // O UtilizadorId será gerado automaticamente pelo EF como chave primária
                    Input.Funcionario.UserName = Input.Email;
                    Input.Funcionario.Email = Input.Email;

                    // Processar upload da fotografia se existir
                    if (fotografiaUpload != null && fotografiaUpload.Length > 0)
                    {
                        var fotografiaBase64 = await ProcessarUploadFotografia(fotografiaUpload);
                        if (!string.IsNullOrEmpty(fotografiaBase64))
                        {
                            Input.Funcionario.Fotografia = fotografiaBase64;
                        }
                    }

                    try
                    {
                        _context.Add(Input.Funcionario);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        haErro = true;
                        // throw; // pode lançar ou tratar de outra forma
                    }

                    // Se houve erro ao guardar o Funcionário, apaga o utilizador Identity criado
                    if (haErro)
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError("", "Ocorreu um erro ao criar o utilizador. Tente novamente.");
                        return Page();
                    }

                    // Atribuir role de Funcionário
                    await _userManager.AddToRoleAsync(user, "Funcionario");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                // If we got this far, something failed, redisplay form
                return Page();
            }
            // If ModelState is not valid, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        /// <summary>
        /// Processa o upload da fotografia e converte para Base64
        /// </summary>
        /// <param name="ficheiro">ficheiro de imagem enviado</param>
        /// <returns>String Base64 da imagem</returns>
        private async Task<string> ProcessarUploadFotografia(IFormFile ficheiro)
        {
            // Validar se é uma imagem
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extensao = Path.GetExtension(ficheiro.FileName).ToLowerInvariant();
            
            if (!extensoesPermitidas.Contains(extensao))
            {
                return null;
            }

            // Validar tamanho do ficheiro (máximo 500KB)
            if (ficheiro.Length > 500 * 1024)
            {
                return null;
            }

            try
            {
                // Converter para Base64
                using var memoryStream = new MemoryStream();
                await ficheiro.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                
                // Determinar o tipo MIME da imagem
                var mimeType = extensao switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png", 
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".webp" => "image/webp",
                    _ => "image/jpeg"
                };

                // Retornar no formato data URL
                return $"data:{mimeType};base64,{base64String}";
            }
            catch
            {
                return null;
            }
        }
    }
}
