namespace Gck.Application.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = string.Empty;
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = "Male";
}

public class UpdateCustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = string.Empty;
}
