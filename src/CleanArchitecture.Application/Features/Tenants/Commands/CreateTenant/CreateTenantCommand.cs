using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Enums;
using FluentValidation;
using MediatR;

namespace CleanArchitecture.Application.Features.Tenants.Commands.CreateTenant;

public record CreateTenantCommand(
    string Name,
    TenantType Type
) : IRequest<OperationResult<long>>;


public class CreateTenantCommandHandler
    : IRequestHandler<CreateTenantCommand, OperationResult<long>>
{
    private readonly ITenantService _tenantService;
    private readonly ISlugService _slugService;
    private readonly ICurrentUserService _currentUser;

    public CreateTenantCommandHandler(
        ITenantService tenantService,
        ISlugService slugService,
        ICurrentUserService currentUser)
    {
        _tenantService = tenantService;
        _slugService = slugService;
        _currentUser = currentUser;
    }

    public async Task<OperationResult<long>> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated ||
            string.IsNullOrWhiteSpace(_currentUser.UserId))
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        var slug = await _slugService.GenerateUniqueSlugAsync(
            request.Name,
            cancellationToken);

        var tenantId = await _tenantService.CreateTenantAsync(
            request.Name,
            slug,
            request.Type,
            _currentUser.UserId,
            cancellationToken);

        return OperationResult<long>.SuccessOperation(
            tenantId,
            201,
            "Tenant created successfully.");
    }
}


public class CreateTenantCommandValidator
    : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .IsInEnum()
            .Must(type =>
                type == TenantType.IndependentTeacher ||
                type == TenantType.Center)
            .WithMessage("Invalid tenant type.");
    }
}