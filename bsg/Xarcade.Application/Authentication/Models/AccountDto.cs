﻿using System.ComponentModel.DataAnnotations;

namespace Xarcade.Application.Authentication.Models
{
    public class AccountDto
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "FirstName is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "AcceptTerms is required.")]
        public bool AcceptTerms { get; set; }
    }
}
