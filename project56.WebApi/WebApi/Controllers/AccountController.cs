﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Authorization.Users;
using Abp.Web.Models;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using project56.Authorization;
using project56.Authorization.Users;
using project56.MultiTenancy;
using project56.WebApi.Models;

namespace project56.WebApi.Controllers
{
    public class AccountController : project56ApiControllerBase
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        private readonly LogInManager _logInManager;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;

        static AccountController()
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
        }

        public AccountController(
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            LogInManager logInManager)
        {
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _logInManager = logInManager;
        }

        [HttpPost]
        public async Task<AjaxResponse> Authenticate(LoginModel loginModel)
        {
            var loginResult = await GetLoginResultAsync(
                loginModel.UsernameOrEmailAddress,
                loginModel.Password,
                loginModel.TenancyName
                );

            var ticket = new AuthenticationTicket(loginResult.Identity, new AuthenticationProperties());

            var currentUtc = new SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));

            return new AjaxResponse(OAuthBearerOptions.AccessTokenFormat.Protect(ticket));
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }
    }
}
