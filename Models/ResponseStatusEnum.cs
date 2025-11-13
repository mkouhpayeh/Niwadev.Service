namespace EnergyService.Api.Models
{
    public enum ResponseStatusEnum
    {
        Success = 0,
        NotFound = 1,
        DbException = 2,
        InvalidModelState = 3,
        DuplicateValue = 4,
        Failed = 5,
        TokenExpired = 6,
        InvalidToken = 7,
        Lockedout = 8,
        Unknown = 9,
        Exception = 10,
        Error = 11,
        Unauthorized = 12,
        IsNotAllowed = 13
    }
}
