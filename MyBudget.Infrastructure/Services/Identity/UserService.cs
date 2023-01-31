using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Infrastructure.Specifications;
using MyBudget.Application.Exceptions;
using MyBudget.Application.Extensions;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Interfaces.Services.Identity;
using MyBudget.Application.Requests.Identity;
using MyBudget.Application.Requests.Mail;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Constants.Role;
using MyBudget.Shared.Wrapper;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace MyBudget.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMailService _mailService;
        private readonly IStringLocalizer<UserService> _localizer;
        private readonly IExcelService _excelService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            RoleManager<ApplicationRole> roleManager,
            IMailService mailService,
            IStringLocalizer<UserService> localizer,
            IExcelService excelService,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _mailService = mailService;
            _localizer = localizer;
            _excelService = excelService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<UserResponse>>> GetAllAsync()
        {
            List<ApplicationUser> users = await _userManager.Users.ToListAsync();
            List<UserResponse> result = _mapper.Map<List<UserResponse>>(users);
            return await Result<List<UserResponse>>.SuccessAsync(result);
        }

        public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
        {
            ApplicationUser userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                return await Result.FailAsync(string.Format(_localizer["Username {0} is already taken."], request.UserName));
            }
            ApplicationUser user = new()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.ActivateUser,
                EmailConfirmed = request.AutoConfirmEmail
            };

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                ApplicationUser? userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                if (userWithSamePhoneNumber != null)
                {
                    return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
                }
            }

            ApplicationUser userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                IdentityResult result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    _ = await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
                    if (!request.AutoConfirmEmail)
                    {
                        string verificationUri = await SendVerificationEmail(user, origin);
                        MailRequest mailRequest = new()
                        {
                            From = "mail@codewithmukesh.com",
                            To = user.Email,
                            Body = string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], verificationUri),
                            Subject = _localizer["Confirm Registration"]
                        };
                        _ = BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
                        return await Result<string>.SuccessAsync(user.Id.ToString(), string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"], user.UserName));
                    }
                    return await Result<string>.SuccessAsync(user.Id.ToString(), string.Format(_localizer["User {0} Registered."], user.UserName));
                }
                else
                {
                    return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
                }
            }
            else
            {
                return await Result.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
            }
        }

        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string route = "api/identity/user/confirm-email/";
            Uri endpointUri = new(string.Concat($"{origin}/", route));
            string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }

        public async Task<IResult<UserResponse>> GetAsync(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            UserResponse result = _mapper.Map<UserResponse>(user);
            return await Result<UserResponse>.SuccessAsync(result);
        }

        public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            ApplicationUser? user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
            bool isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
            if (isAdmin)
            {
                return await Result.FailAsync(_localizer["Administrators Profile's Status cannot be toggled"]);
            }
            if (user != null)
            {
                user.IsActive = request.ActivateUser;
                IdentityResult identityResult = await _userManager.UpdateAsync(user);
            }
            return await Result.SuccessAsync();
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
        {
            List<UserRoleModel> viewModel = new();
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            List<ApplicationRole> roles = await _roleManager.Roles.ToListAsync();

            foreach (ApplicationRole? role in roles)
            {
                UserRoleModel userRolesViewModel = new()
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description,
                    Selected = await _userManager.IsInRoleAsync(user, role.Name)
                };
                viewModel.Add(userRolesViewModel);
            }
            UserRolesResponse result = new() { UserRoles = viewModel };
            return await Result<UserRolesResponse>.SuccessAsync(result);
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(request.UserId);
            if (user.Email == "mukesh@blazorhero.com")
            {
                return await Result.FailAsync(_localizer["Not Allowed."]);
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            List<UserRoleModel> selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

            ApplicationUser currentUser = await _userManager.FindByNameAsync(_currentUserService.UserName);
            if (!await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
            {
                bool tryToAddAdministratorRole = selectedRoles
                    .Any(x => x.RoleName == RoleConstants.AdministratorRole);
                bool userHasAdministratorRole = roles.Any(x => x == RoleConstants.AdministratorRole);
                if ((tryToAddAdministratorRole && !userHasAdministratorRole) || (!tryToAddAdministratorRole && userHasAdministratorRole))
                {
                    return await Result.FailAsync(_localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
                }
            }

            IdentityResult result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
            return await Result.SuccessAsync(_localizer["Roles Updated"]);
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded
                ? (IResult<string>)await Result<string>.SuccessAsync(user.Id.ToString(), string.Format(_localizer["Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.Email))
                : throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string route = "account/reset-password";
            var url = string.Concat($"{origin ?? string.Empty}/", route);
            Uri endpointUri = new Uri(url);
            string passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            MailRequest mailRequest = new()
            {
                Body = string.Format(_localizer["Please reset your password by <a href='{0}'>clicking here</a>."], HtmlEncoder.Default.Encode(passwordResetURL)),
                Subject = _localizer["Reset Password"],
                To = request.Email
            };
            _ = BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
            return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            return result.Succeeded
                ? await Result.SuccessAsync(_localizer["Password Reset Successful!"])
                : await Result.FailAsync(_localizer["An Error has occured!"]);
        }

        public async Task<int> GetCountAsync()
        {
            int count = await _userManager.Users.CountAsync();
            return count;
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            UserFilterSpecification userSpec = new(searchString);
            List<ApplicationUser> users = await _userManager.Users
                .Specify(userSpec)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
            string result = await _excelService.ExportAsync(users, sheetName: _localizer["Users"],
                mappers: new Dictionary<string, Func<ApplicationUser, object>>
                {
                    { _localizer["Id"], item => item.Id },
                    { _localizer["FirstName"], item => item.FirstName },
                    { _localizer["LastName"], item => item.LastName },
                    { _localizer["UserName"], item => item.UserName },
                    { _localizer["Email"], item => item.Email },
                    { _localizer["EmailConfirmed"], item => item.EmailConfirmed },
                    { _localizer["PhoneNumber"], item => item.PhoneNumber },
                    { _localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                    { _localizer["IsActive"], item => item.IsActive },
                    { _localizer["CreatedOn (Local)"], item => DateTime.SpecifyKind(item!.CreatedOn??DateTime.UtcNow, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["CreatedOn (UTC)"], item => item!.CreatedOn?.ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["ProfilePictureDataUrl"], item => item.ProfilePictureDataUrl },
                });

            return result;
        }
    }
}
