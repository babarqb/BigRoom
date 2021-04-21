﻿using BigRoom.Repository.Contexts;
using BigRoom.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BigRoom.Service.Service
{
    public class UserManager : IUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserProfileService _profileService;

        public UserManager(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,IUserProfileService profileService)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this._profileService = profileService;
        }
        public bool UserIsSignIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }
        public async Task<string> GetRole(ClaimsPrincipal user)
        {
            var appUser = await GetApplicationUserAsync(user);
            return (await this._userManager.GetRolesAsync(appUser))?.FirstOrDefault();
        }
        public async Task<(IdentityResult result, string role)> CreateAsync(ApplicationUser user, string password,string roleId)
        {
           
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                await _userManager.AddToRoleAsync(user, role.Name);
                await _profileService.AddUserProfileAsync(user.Id);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return (result, role.Name);
            }
            return (result, null);
        }

        public async Task<ApplicationUser> GetApplicationUserAsync(ClaimsPrincipal user)
        {
            return await this._userManager.GetUserAsync(user);
        }

        public async Task<IList<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<(SignInResult result, string role)> SignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure = false)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                var role = await _userManager.GetRolesAsync(user);
                return (result, role.FirstOrDefault());
            }
            return (result, null);
        }
    }
}