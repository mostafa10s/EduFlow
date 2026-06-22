using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var c = await _unitOfWork.Customers.GetByIdAsync(id, ct);
        return c is null ? null : new CustomerDto(c.Id, c.FirstName, c.LastName, c.Email, c.FullName);
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken ct = default)
    {
        var customers = await _unitOfWork.Customers.GetAllAsync(ct);
        return customers.Select(c => new CustomerDto(c.Id, c.FirstName, c.LastName, c.Email, c.FullName))
                        .ToList();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default)
    {
        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        await _unitOfWork.Customers.AddAsync(customer, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CustomerDto(customer.Id, customer.FirstName, customer.LastName,
                               customer.Email, customer.FullName);
    }
}
