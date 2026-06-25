using Blazored.LocalStorage;

namespace pRHosaApp1;

public class SessionService
{
    private const string UserEmailStorageKey = "usuario_email";
    private readonly ILocalStorageService _localStorage;

    public SessionService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<string?> GetUserEmailAsync()
    {
        var email = await _localStorage.GetItemAsync<string>(UserEmailStorageKey);
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }

    public Task<bool> HasSessionAsync()
    {
        return HasSessionInternalAsync();
    }

    public async Task SignInAsync(string email)
    {
        await _localStorage.SetItemAsync(UserEmailStorageKey, email);
    }

    public async Task SignOutAsync()
    {
        await _localStorage.RemoveItemAsync(UserEmailStorageKey);
    }

    private async Task<bool> HasSessionInternalAsync()
    {
        return !string.IsNullOrWhiteSpace(await GetUserEmailAsync());
    }
}
