namespace Gck.Application.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int SessionCount { get; set; }
    public bool IsLoyal { get; set; }
    public int SessionsRequiredForFree { get; set; }
    public int PaidSessionsCount { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = "Male";
    public bool IsLoyal { get; set; } = false;
    public int SessionsRequiredForFree { get; set; } = 0;
}

public class UpdateCustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Gender { get; set; } = string.Empty;
    public bool IsLoyal { get; set; }
    public int SessionsRequiredForFree { get; set; }
}
