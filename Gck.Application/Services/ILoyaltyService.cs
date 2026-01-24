namespace Gck.Application.Services;

public interface ILoyaltyService
{
    Task<bool> CanCustomerGetFreeSession(int customerId);
    Task IncrementPaidSessions(int customerId);
    Task ResetPaidSessionsCount(int customerId);
}
