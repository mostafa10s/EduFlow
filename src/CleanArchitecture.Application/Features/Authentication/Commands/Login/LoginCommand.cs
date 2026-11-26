using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<OperationResult<string>>;


public class LoginCommandHandler
    : IRequestHandler<LoginCommand, OperationResult<string>>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<OperationResult<string>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var isValid = await _identityService.ValidateCredentialsAsync(
            request.Email,
            request.Password);

        if (!isValid)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var userId = await _identityService.GetUserIdByEmailAsync(
        request.Email);

        if (userId is null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var roles = await _identityService.GetUserRolesAsync(
            userId,
            cancellationToken);

        var token = await _tokenService.GenerateTokenAsync(
            userId,
            request.Email,
            roles);

        return OperationResult<string>.SuccessOperation(
            token,
            200,
            "Login successful.");
    }
}


public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}