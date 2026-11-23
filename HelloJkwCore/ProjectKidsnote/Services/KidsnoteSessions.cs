using Common;
using ProjectKidsnote.Client;
using ProjectKidsnote.Configuration;

namespace ProjectKidsnote.Services;

internal sealed class KidsnoteSessions(KidsnoteOptions options, KidsnoteCredentials credentials) : IAsyncDisposable
{
    private readonly Dictionary<(UserId Owner, string Credential), KidsnoteClient> _clients = [];
    private readonly SemaphoreSlim _gate = new(1);
    private bool _disposed;

    public async Task<KidsnoteClient> GetAsync(UserId owner, string encrypted, CancellationToken token = default)
    {
        await _gate.WaitAsync(token);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var key = (owner, encrypted);
            if (_clients.TryGetValue(key, out var client) && client.IsLoggedIn)
                return client;

            var login = credentials.Open(owner, encrypted);
            if (client is null)
            {
                client = new KidsnoteClient(options);
                try
                {
                    await client.LoginAsync(login.UserId, login.Password, token);
                }
                catch
                {
                    client.Dispose();
                    throw;
                }
                _clients.Add(key, client);
            }
            else
            {
                await client.LoginAsync(login.UserId, login.Password, token);
            }
            return client;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync();
        try
        {
            _disposed = true;
            foreach (var client in _clients.Values)
                client.Dispose();
            _clients.Clear();
        }
        finally
        {
            _gate.Release();
        }
    }
}
