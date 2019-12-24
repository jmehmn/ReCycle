using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReCycle.Models.Requests;
using ReCycle.Models;
using ReCycle.Services;
using ReCycle.Web.Controllers;
using ReCycle.Web.Core;
using ReCycle.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ReCycle.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using ReCycle.Models.Requests.EmailModel;
using ReCycle.Data.Providers;
using ReCycle.Services.Interfaces;
using ReCycle.Models.Requests.UserTokens;
using ReCycle.Models.Requests.Email;
using ReCycle.Models.Requests.User;

namespace ReCycle.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserApiController : BaseApiController
    {
        private SendGridConfig _sendGridConfig;
        private IUserTokenService _userTokenService;
        private IEmailService _service = null;
        private IUserService _userService;
        private IAuthenticationService<int> _authService;
        private IOptions<SecurityConfig> _options;

        public UserApiController(IUserTokenService userTokenService
            , IUserService userService
            , IAuthenticationService<int> authService
            , ILogger<TempAuthApiController> logger
            , IOptions<SecurityConfig> options
            , IOptions<SendGridConfig> sendGridConfig
            , IEmailService service) : base(logger)
        {
            _service = service;
            _sendGridConfig = sendGridConfig.Value;
            _userService = userService;
            _userTokenService = userTokenService;
            _authService = authService;
            _options = options;
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<ActionResult> Register(UserAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int id = _userService.Create(model);

                Guid guid = Guid.NewGuid();

                _userTokenService.Add(id, guid);

                EmailAddRequest emailBase = new EmailAddRequest();
                emailBase.From = model.Email;
                string email = emailBase.From;
                string directory = Environment.CurrentDirectory;
                string path = Path.Combine(directory, "EmailTemplates\\ConfirmEmail.html");
                string htmlContent = System.IO.File.ReadAllText(path);
                await _service.ConfirmEmail(email, htmlContent, _sendGridConfig.Secret, _sendGridConfig.Email, guid);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());

                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpGet("confirm"), AllowAnonymous]
        public ActionResult<SuccessResponse> Confirm(string token)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                bool verifiedToken = _userService.VerifyToken(token);
                if (verifiedToken)
                {
                    _userService.DeleteToken(token);
                    response = new SuccessResponse();
                }
                else
                {
                    iCode = 500;
                    response = new ErrorResponse("This user accountr could not be confirmed.");
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}");
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(iCode, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<User>> Get(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                User user = _userService.Get(id);

                if (user == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<User> { Item = user };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(iCode, response);
        }

        [HttpGet("email"), AllowAnonymous]
        public async Task<ActionResult<ItemResponse<User>>> GetByEmail(string email)
        {
           int iCode = 200;
           BaseResponse response = null;

            try
            {
                User user = _userService.GetByEmail(email);

                if (user == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Email not found.");
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    _userTokenService.AddForgotPassword(user.Id, guid);

                    EmailAddRequest emailBase = new EmailAddRequest();
                    emailBase.From = user.Email;
                    string userEmail = emailBase.From;
                    string directory = Environment.CurrentDirectory;
                    string path = Path.Combine(directory, "EmailTemplates\\ResetPasswordEmail.html");
                    string htmlContent = System.IO.File.ReadAllText(path);
                    await _service.ResetPasswordEmail(userEmail, htmlContent, _sendGridConfig.Secret, _sendGridConfig.Email, guid);


                    response = new ItemResponse<User> { Item = user };

                };   
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(iCode, response);
        }

        [HttpPut("resetPassword"), AllowAnonymous]
        public ActionResult<SuccessResponse> ResetPassword(UserPasswordUpdateRequest model)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
               int userId = _userService.ResetPassword(model.Token);

                if (userId >  0)
                {
                    _userService.UpdateUserPassword(userId, model.Password);
                    _userService.DeleteToken(model.Token);
                    response = new SuccessResponse();
                }
                else
                {
                    iCode = 500;
                    response = new ErrorResponse("Failed to reset password.");
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse($"Generic Error: {ex.Message}");
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(iCode, response);
        }

        [HttpPost("{login}"), AllowAnonymous]
        public async Task<ActionResult<SuccessResponse>> Login(UserLogRequest logModel)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                User model = _userService.GetByEmail(logModel.Email);

                if (model != null && model.UserStatusId != 1)
                {
                    return StatusCode(401, new ErrorResponse("Please confirm your account."));
                }
                else
                {

                    bool isLoggedIn = await _userService.LogInAsync(logModel);
                    if (isLoggedIn)
                    {
                        response = new ItemResponse<string>() { Item = logModel.Email };
                        return Ok200(response);
                    }
                    else
                    {
                        return StatusCode(401, new ErrorResponse("The information provided does not match our records. Please correct the information entered and try again."));
                    }
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(code, response);
        }

        [HttpGet("current")]
        public ActionResult<ItemResponse<IUserAuthData>> GetCurrrent()
        {
            IUserAuthData user = _authService.GetCurrentUser();
            ItemResponse<IUserAuthData> response = new ItemResponse<IUserAuthData>();
            response.Item = user;
            return Ok200(response);
        }
    }
}

