using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.User {
    public class Login {
        public class Query : IRequest<User> {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query> {
            public QueryValidator () {
                RuleFor (x => x.Email).NotEmpty ();
                RuleFor (x => x.Password).NotEmpty ();
            }
        }

        public class Handler : IRequestHandler<Query, User> {
            private readonly ILogger<Login> _logger;
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJwtGenerator _jwtGenerator;
            public Handler (UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator, ILogger<Login> logger) {
                this._jwtGenerator = jwtGenerator;
                this._signInManager = signInManager;
                this._userManager = userManager;
                this._logger = logger;
            }

            public async Task<User> Handle (Query request, CancellationToken cancellationToken) {
                var user = await _userManager.FindByEmailAsync (request.Email);

                if (user == null) {
                    throw new Exception (HttpStatusCode.Unauthorized.ToString ());
                }

                var result = await _signInManager.CheckPasswordSignInAsync (user, request.Password, false);

                if (result.Succeeded) {
                    return new User { UserName = user.UserName, Email = user.Email, Token = _jwtGenerator.CreateToken (user), Image = null };
                }

                throw new Exception (HttpStatusCode.Unauthorized.ToString ());
            }
        }
    }
}