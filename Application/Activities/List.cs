using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities {
    public class List {
        public class Query : IRequest<List<Activity>> { }

        public class Handler : IRequestHandler<Query, List<Activity>> {
            private readonly DataContext _context;
            private readonly ILogger<List> _logger;
            public Handler (DataContext context, ILogger<List> logger) {
                this._logger = logger;
                this._context = context;
            }

            public async Task<List<Activity>> Handle (Query request, CancellationToken cancellationToken) {
                try {
                    cancellationToken.ThrowIfCancellationRequested ();
                    var activities = await _context.Activities.ToListAsync ();
                    return activities;

                } catch (Exception exp) when (exp is TaskCanceledException) {
                    _logger.LogInformation (exp, "The request was cancelled");
                    return null;
                }
            }
        }
    }
}