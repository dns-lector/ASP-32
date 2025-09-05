namespace ASP_32.Services.Auth
{
    public interface IAuthService
    {
        void SetAuth(object payload);
        object? GetAuth();
        void RemoveAuth();

    }
}
