using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xarcade.Application.Authentication.Models;
using Xarcade.Domain.Authentication;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Utilities;
using Xarcade.Infrastructure.Utilities.Logger;

namespace Xarcade.Application.Authentication
{
    public class XarcadeAccountService : IXarcadeAccountService
    {
        private readonly IDataAccessProximaX dataAccessAuthentication;
        private readonly IValidator validator;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private static ILogger _logger;

        public XarcadeAccountService(IDataAccessProximaX dataAccessAuthentication, 
                                     IValidator validator, 
                                     IConfiguration configuration,
                                     IEmailService emailService)
        {
            this.dataAccessAuthentication = dataAccessAuthentication;
            this.validator = validator;
            this.configuration = configuration;
            this.emailService = emailService;
        }
public async Task<bool> RegisterAccountAsync(AccountDto account, string origin)
        {
            var result = this.validator.Validate(account);
            if (!result.IsValid)
            {
                // TODO: Replace Console.WriteLine with log
                result.ErrorMessages.ForEach(e => Console.WriteLine(e));
                return false;
            }

            if (string.IsNullOrEmpty(origin))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Origin is empty");
                return false;
            }

            var searchKey = new XarcadeUserSearchKey()
            {
                Username = account.Username
            };
            var existingUser = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));
            if (existingUser != null && string.Equals(existingUser.UserID, account.UserId))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("User already exist");
                return false;
            }

            var xarcadeUser = new XarcadeUser()
            {
                UserID = string.IsNullOrEmpty(account.UserId) ? Guid.NewGuid().ToString() : account.UserId,
                UserDetails = new XarcadeUserDetails()
                {
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    Email = account.Email,
                },
                Username = account.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(account.Password),
                AcceptTerms = account.AcceptTerms,
                Created = DateTime.UtcNow,
                Verification = new VerificationDetails()
                {
                    VerificationToken = RandomTokenString()
                }
            };

            var isOk = await Task.Run(() => this.dataAccessAuthentication.SaveXarcadeUser(xarcadeUser));
            if (!isOk)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("An error occured when trying to save the user");
                return false;
            }

            var emailDetails = new Email()
            {
                To = account.Email,
                Name = $"{account.FirstName} {account.LastName}",
                Origin = origin,
                Type = EmailType.Verification,
                VerificationToken = xarcadeUser.Verification.VerificationToken,
            };

            if(!this.emailService.Send(emailDetails))
            {
                // TODO: Replace Console.WriteLine with log
                // To Evaluate: Maybe we need to expose an api to send a verification mail
                // or should this be handled by the customer service
                Console.WriteLine("Sending verification mail failed");
                return false;
            }
            else
            {
                
            }

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email, string origin)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(origin))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Invalid ForgotPasswordAsync parameters");
                return false;
            }

            var searchKey = new XarcadeUserSearchKey()
            {
                Email = email
            };
            var existingUser = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));
            if (existingUser == null)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("User does not exist");
                return false;
            }

            if (existingUser.PasswordReset == null)
            {
                existingUser.PasswordReset = new PasswordResetDetails();
                existingUser.PasswordReset.ResetToken = RandomTokenString();
                existingUser.PasswordReset.ResetTokenExpiry = DateTime.UtcNow.AddDays(24);
            }

            var isUpdated = await Task.Run(() => this.dataAccessAuthentication.UpdateXarcadeUser(existingUser));
            if (!isUpdated)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Xarcade user update failed");
                return false;
            }

            var emailDetails = new Email()
            {
                To = existingUser.UserDetails.Email,
                Name = $"{existingUser.UserDetails.FirstName} {existingUser.UserDetails.LastName}",
                Origin = origin,
                Type = EmailType.PasswordReset,
                ResetToken = existingUser.PasswordReset.ResetToken
            };

            if (!this.emailService.Send(emailDetails))
            {
                // TODO: Replace Console.WriteLine with log
                // To Evaluate: Maybe we need to expose an api to send a verification mail
                // or should this be handled by the customer service
                Console.WriteLine("Sending forgot password mail failed");
                return false;
            }

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetDetails, string userId)
        {
            var result = this.validator.Validate(resetDetails);
            if (!result.IsValid)
            {
                // TODO: Replace Console.WriteLine with log
                result.ErrorMessages.ForEach(e => Console.WriteLine(e));
                return false;
            }

            if (string.IsNullOrEmpty(userId))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("User Id is null");
                return false;
            }

            var searchKey = new XarcadeUserSearchKey()
            {
                UserID = userId
            };
            var existingUser = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));
            if (existingUser == null)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("User does not exist");
                return false;
            }

            if (string.Equals(existingUser.PasswordReset.ResetToken, resetDetails.Token) &&
                existingUser.PasswordReset.ResetTokenExpiry > DateTime.UtcNow)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Invalid reset token");
                return false;
            }

            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetDetails.Password);
            if (existingUser.PasswordReset == null)
            {
                existingUser.PasswordReset = new PasswordResetDetails();
                existingUser.PasswordReset.PasswordReset = DateTime.UtcNow;
                existingUser.PasswordReset.ResetToken = "";
                existingUser.PasswordReset.ResetTokenExpiry = default;
            }

            var isUpdated = await Task.Run(() => this.dataAccessAuthentication.UpdateXarcadeUser(existingUser));
            if (!isUpdated)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Xarcade user update failed");
                return false;
            }

            return true;
        }

        public async Task<AuthenticateDto> AuthenticateAsync(LoginDto loginDetails, string ipAddress)
        {
            var authenticationResult = new AuthenticateDto();
            var result = this.validator.Validate(loginDetails);
            if (!result.IsValid)
            {
                // TODO: Replace Console.WriteLine with log
                result.ErrorMessages.ForEach(e => Console.WriteLine(e));
                authenticationResult.AuthenticationError = AuthenticationError.InvalidParameters;
                return authenticationResult;
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("IP Address is empty");
                authenticationResult.AuthenticationError = AuthenticationError.InvalidIpAddress;
                return authenticationResult;
            }

            var searchKey = new XarcadeUserSearchKey()
            {
                Username = loginDetails.Username
            };

            var existingUser = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));

            if (existingUser == null || !existingUser.Verification.IsVerified)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Account is not verified");
                authenticationResult.AuthenticationError = AuthenticationError.AccountNotVerified;
                return authenticationResult;
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDetails.Password, existingUser.PasswordHash))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Invalid password");
                authenticationResult.AuthenticationError = AuthenticationError.InvalidPassword;
                return authenticationResult;
            }

            var jwtToken = GenerateJwtToken(existingUser);
            var refreshToken = GenerateRefreshToken(ipAddress);

            if (existingUser.RefreshTokens == null)
            {
                existingUser.RefreshTokens = new List<RefreshToken>();
                existingUser.RefreshTokens.Add(refreshToken);
            }

            var isUpdated = await Task.Run(() => this.dataAccessAuthentication.UpdateXarcadeUser(existingUser));
            if (!isUpdated)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Xarcade user update failed");
                authenticationResult.AuthenticationError = AuthenticationError.AccountUpdateFailed;
                return authenticationResult;
            }

            authenticationResult.Account = await this.GetXarcadeUserAsync(existingUser.UserID);
            authenticationResult.JwtToken = jwtToken;
            authenticationResult.RefreshToken = refreshToken;
            authenticationResult.AuthenticationError = AuthenticationError.Ok;

            return authenticationResult;
        }

        public async Task<AuthenticateDto> RefreshTokenAsync(string token, string ipAddress)
        {
            var (refreshToken, existingUser) = await Task.Run(() => GetRefreshToken(token));

            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokerIp = ipAddress;
            refreshToken.ReplacementToken = newRefreshToken.Token;
            existingUser.RefreshTokens.Add(newRefreshToken);

            var isUpdated = await Task.Run(() => this.dataAccessAuthentication.UpdateXarcadeUser(existingUser));
            if (!isUpdated)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Xarcade user update failed");
                return null;
            }

            return new AuthenticateDto()
            {
                Account = MapAccount(existingUser),
                JwtToken = GenerateJwtToken(existingUser),
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
        {
            var (refreshToken, existingUser) = await Task.Run(() => GetRefreshToken(token));

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokerIp = ipAddress;

            var isUpdated = await Task.Run(() => this.dataAccessAuthentication.UpdateXarcadeUser(existingUser));
            if (!isUpdated)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Xarcade user update failed");
                return false;
            }

            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Token is empty");
                return false;
            }

            var searchKey = new XarcadeUserSearchKey()
            {
                VerificationToken = token
            };
            var existingUser = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));

            if (existingUser == null)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Account is not verified");   
                return false;
            }

            if (existingUser.Verification.IsVerified)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Account is already verified");
                return false;
            }

            existingUser.Verification.IsVerified = true;
            existingUser.Verification.Verified = DateTime.UtcNow;
            existingUser.Verification.VerificationToken = null;

            var isUpdated = await Task.Run(() => this.dataAccessAuthentication.UpdateXarcadeUser(existingUser));
            if (!isUpdated)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Xarcade user update failed");
                return false;
            }

            return true;
        }

        public async Task<AccountDto> GetXarcadeUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("User Id is empty");
                return null;
            }

            var searchKey = new XarcadeUserSearchKey()
            {
                UserID = userId
            };
            var existingUser = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));

            return new AccountDto()
            {
                UserId = userId,
                FirstName = existingUser.UserDetails.FirstName,
                LastName = existingUser.UserDetails.LastName,
                Email = existingUser.UserDetails.Email,
                Username = existingUser.Username,
                AcceptTerms = existingUser.AcceptTerms
            };
        }

        public async Task<bool> UpdateXarcadeUserAsync(AccountDto account)
        {
            // We don't have to call our validate here since we are only updating.
            // Some fields might be empty

            var searchKey = new XarcadeUserSearchKey()
            {
                UserID = account.UserId
            };
            var existingUser = await Task.Run(() => this.dataAccessAuthentication.LoadXarcadeUser(searchKey));

            if (!string.Equals(existingUser.UserDetails.Email, account.Email))
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Email is already taken!");
                return false;
            }

            if (!existingUser.Verification.IsVerified)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Account is not yet verified!");
                return false;
            }

            // We only allow update these info for now.
            if (!string.IsNullOrEmpty(account.Password))
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(account.Password);

            if (!string.IsNullOrEmpty(account.FirstName))
                existingUser.UserDetails.FirstName = account.FirstName;

            if (!string.IsNullOrEmpty(account.LastName))
                existingUser.UserDetails.FirstName = account.LastName;

            // TODO: Need to confirm if we allow update of email
            if (!string.IsNullOrEmpty(account.Email))
                existingUser.UserDetails.Email = account.Email;

            existingUser.Modified = DateTime.UtcNow;

            var isUpdated = await Task.Run(() => this.dataAccessAuthentication.UpdateXarcadeUser(existingUser));
            if (!isUpdated)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("Xarcade user update failed");
                return false;
            }

            return true;
        }

        public AccountDto GetAuthorizedXarcadeUser(IDictionary<object, object> context, string userId)
        {
            var existingUser = (XarcadeUser)context["Account"];

            if (existingUser == null)
                return null;

            return new AccountDto()
            {
                UserId = userId,
                FirstName = existingUser.UserDetails.FirstName,
                LastName = existingUser.UserDetails.LastName,
                Email = existingUser.UserDetails.Email,
                Username = existingUser.Username,
                AcceptTerms = existingUser.AcceptTerms
            };
        }

        private string GenerateJwtToken(XarcadeUser account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.configuration["JwtTokenSettings:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", account.UserID) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private (RefreshToken, XarcadeUser) GetRefreshToken(string token)
        {
            var searchKey = new XarcadeUserSearchKey()
            {
                RefreshToken = token
            };
            var existingUser = this.dataAccessAuthentication.LoadXarcadeUser(searchKey);

            if (existingUser == null)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("User does not exist");
                return (null, null);
            }

            var refreshToken = existingUser.RefreshTokens.FirstOrDefault(x => x.Token == token);
            if (refreshToken == null || !refreshToken.IsActive)
            {
                // TODO: Replace Console.WriteLine with log
                Console.WriteLine("User does not exist");
                return (null, null);
            }
            return (refreshToken, existingUser);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expiry = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatorIp = ipAddress
            };
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private AccountDto MapAccount(XarcadeUser xarcadeUser)
        {
            return new AccountDto()
            {
                UserId = xarcadeUser.UserID,
                FirstName = xarcadeUser.UserDetails.FirstName,
                LastName = xarcadeUser.UserDetails.LastName,
                Email = xarcadeUser.UserDetails.Email,
                Username = xarcadeUser.Username,
                AcceptTerms = xarcadeUser.AcceptTerms
            };
        }
    }
}
