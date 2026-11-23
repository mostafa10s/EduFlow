using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password
) : IRequest<Guid>;

