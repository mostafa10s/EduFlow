namespace CleanArchitecture.Application.DTOs;

public record CustomerDto(int Id, string FirstName, string LastName, string Email, string FullName);

public record CreateCustomerDto(string FirstName, string LastName, string Email);
