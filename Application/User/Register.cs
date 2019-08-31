using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Validators;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.User {
    public class Register {
        public class Command : IRequest<User> {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command> {
            public CommandValidator () {
                RuleFor (x => x.Email).NotEmpty ().EmailAddress ();
                RuleFor (x => x.UserName).NotEmpty ();
                RuleFor (x => x.Password).Password ();
            }
        }

        public class Handler : IRequestHandler<Command, User> {

            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;

            private readonly DataContext _context;
            public Handler (DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator) {
                this._context = context;

                this._jwtGenerator = jwtGenerator;
                this._userManager = userManager;
            }

            public async Task<User> Handle (Command request, CancellationToken cancellationToken) {
                if (await _context.Users.Where (x => x.Email == request.Email).AnyAsync ()) {
                    throw new Exception ("Email already exists");
                }

                if (await _context.Users.Where (x => x.UserName == request.UserName).AnyAsync ()) {
                    throw new Exception ("Username already exists");
                }

                var user = new AppUser {
                    UserName = request.UserName,
                    Email = request.Email,
                };

                var result = await _userManager.CreateAsync (user, request.Password);

                if (result.Succeeded) {
                    return new User {
                        UserName = user.UserName,
                            Token = _jwtGenerator.CreateToken (user),
                            Email = user.Email,
                            Image = null
                    };
                }
                throw new Exception ("Problem creating user");
            }
        }
    }
}