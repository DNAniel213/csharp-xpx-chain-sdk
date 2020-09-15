using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Xarcade.Application.Authentication;
using Xarcade.Application.Authentication.Models;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities;

namespace Authentication.Test
{
    public class Tests
    {
        private IXarcadeAccountService xarcadeAccountServie;

        [SetUp]
        public void Setup()
        {
            var validator = new XarcadeValidator();
            var dataAccessAuthentication = new DataAccessProximaX();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var emailService = new EmailService(config);
            xarcadeAccountServie = new XarcadeAccountService(dataAccessAuthentication, validator, config, emailService);
        }

        [Test]
        public void RegisterAccountAsync_Should_Return_True_After_Successful_Registration()
        {
            var accountDto = new AccountDto()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@doe.com",
                Password = "123456",
                AcceptTerms = true,
                Username = "john@doe.com"
            };

            var result = this.xarcadeAccountServie.RegisterAccountAsync(accountDto, "Test").GetAwaiter().GetResult();
            Assert.IsTrue(result);
        }

        [Test]
        public void RegisterAccountAsync_Should_Return_False_While_Passing_Invalid_Null_Account()
        {
            var result = this.xarcadeAccountServie.RegisterAccountAsync(null, "Test").GetAwaiter().GetResult();
            Assert.IsFalse(result);
        }

        [Test]
        public void RegisterAccountAsync_Should_Return_False_While_Passing_Invalid_Empty_Origin()
        {
            var accountDto = new AccountDto()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@doe.com",
                Password = "123456",
                AcceptTerms = true,
                Username = "john@doe.com"
            };
            var result = this.xarcadeAccountServie.RegisterAccountAsync(accountDto, "").GetAwaiter().GetResult();
            Assert.IsFalse(result);
        }
    }
}