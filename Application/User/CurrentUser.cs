using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.User {
    public class CurrentUser {
        public class Query : IRequest<User> { }

        public class Handler : IRequestHandler<Query, User> {
            private readonly ILogger<CurrentUser> _logger;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            private readonly IJwtGenerator _jwtGenerator;
            public Handler (UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor, ILogger<CurrentUser> logger) {
                this._jwtGenerator = jwtGenerator;
                this._userAccessor = userAccessor;
                this._userManager = userManager;
                this._logger = logger;
            }

            public async Task<User> Handle (Query request, CancellationToken cancellationToken) {
                var user = await _userManager.FindByNameAsync (_userAccessor.GetCurrentUserName ());
                return new User {
                    UserName = user.UserName,
                        Email = user.Email,
                        Token = _jwtGenerator.CreateToken (user),
                        Image = null
                };
            }
        }
    }
}