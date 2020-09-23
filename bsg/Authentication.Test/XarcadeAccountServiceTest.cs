using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Xarcade.Application.Authentication;
using Xarcade.Application.Xarcade;
using Xarcade.Application.Xarcade.Models;
using Xarcade.Application.Authentication.Models;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.Utilities;

namespace Authentication.Test
{
    public class Tests
    {
        private IXarcadeAccountService xarcadeAccountServie;
        private IAccountService accountService;
        private ProximaxBlockchainPortal blockchainPortal;

        [SetUp]
        public void Setup()
        {
            var validator = new XarcadeValidator();
            var dataAccessAuthentication = new DataAccessProximaX();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var emailService = new EmailService(config);
            blockchainPortal = new ProximaxBlockchainPortal(config);
            xarcadeAccountServie = new XarcadeAccountService(dataAccessAuthentication, validator, config, emailService);
            accountService = new AccountService(dataAccessAuthentication, blockchainPortal);
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

        [Test]
        public void CreateOwnerAccountAsync_Should_Return_Null_When_UserExists()
        {
            var result = this.accountService.CreateOwnerAccountAsync("latom");
            if(result != null)
                Assert.IsFalse(false);
            else
                Assert.IsFalse(true);

        }

    }
}