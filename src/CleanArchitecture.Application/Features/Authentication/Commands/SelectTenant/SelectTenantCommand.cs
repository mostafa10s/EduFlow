
    using CleanArchitecture.Application.Common.Models;
    using CleanArchitecture.Application.Interfaces;
    using FluentValidation;
    using global::CleanArchitecture.Application.Common.Exceptions;
    using MediatR;

    namespace CleanArchitecture.Application.Features.Authentication.Commands.SelectTenant;

    public record SelectTenantCommand(
        long TenantId
    ) : IRequest<OperationResult<string>>;


    public class SelectTenantCommandHandler
        : IRequestHandler<SelectTenantCommand, OperationResult<string>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public SelectTenantCommandHandler(
            ICurrentUserService currentUser,
            IIdentityService identityService,
            ITokenService tokenService)
        {
            _currentUser = currentUser;
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<OperationResult<string>> Handle(
            SelectTenantCommand request,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated ||
                string.IsNullOrWhiteSpace(_currentUser.UserId))
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            var tenantRole = await _identityService.GetTenantRoleAsync(
                _currentUser.UserId,
                request.TenantId,
                cancellationToken);

            if (tenantRole is null)
            {
                throw new ForbiddenException(
                    "You are not an active member of this tenant.");
            }

            var globalRoles = await _identityService.GetUserRolesAsync(
                _currentUser.UserId,
                cancellationToken);

            var token = await _tokenService.GenerateTokenAsync(
                _currentUser.UserId,
                _currentUser.Email ?? string.Empty,
                globalRoles,
                request.TenantId,
                tenantRole);

            return OperationResult<string>.SuccessOperation(
                token,
                200,
                "Tenant selected successfully.");
        }
    }


    public class SelectTenantCommandValidator
        : AbstractValidator<SelectTenantCommand>
    {
        public SelectTenantCommandValidator()
        {
            RuleFor(x => x.TenantId)
                .GreaterThan(0)
                .WithMessage("Invalid tenant ID.");
        }
    }

