
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword
) : IRequest<OperationResult<string>>;


public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, OperationResult<string>>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<OperationResult<string>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var userId = await _identityService.CreateUserAsync(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Password,
            cancellationToken);

        return OperationResult<string>.SuccessOperation(
            userId,
            201,
            "Account registered successfully.");
    }
}


public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Password and confirmation password do not match.");
    }
}
