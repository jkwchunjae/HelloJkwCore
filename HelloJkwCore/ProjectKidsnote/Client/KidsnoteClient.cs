using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProjectKidsnote.Configuration;
using ProjectKidsnote.Models.Account;
using ProjectKidsnote.Models.Authentication;
using ProjectKidsnote.Models.Reports;

namespace ProjectKidsnote.Client;

public sealed class KidsnoteClient : IKidsnoteClient, IDisposable
{
    private readonly CookieContainer _cookieContainer = new();
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly KidsnoteOptions _options;
    private readonly SemaphoreSlim _reauthenticationLock = new(1, 1);
    private KidsnoteLoginRequest? _lastSuccessfulLogin;
    private UserInfo? _myInfo;
    private long _authenticationGeneration;

    public KidsnoteClient(KidsnoteOptions options)
    {
        _options = options;

        var baseAddress = CreateBaseAddress(options.BaseAddress);
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = _cookieContainer,
            AutomaticDecompression =
                DecompressionMethods.GZip |
                DecompressionMethods.Deflate |
                DecompressionMethods.Brotli,
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = baseAddress,
        };
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd(
            "application/json, text/plain, */*");

        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
        };
    }

    public bool IsLoggedIn { get; private set; }

    public async Task LoginAsync(
        string userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        await LoginCoreAsync(userId, password, cancellationToken);
        _lastSuccessfulLogin = new KidsnoteLoginRequest(userId, password);
    }

    public async Task<UserInfo> GetMyInfoAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureLoggedIn();

        using var response = await SendWithAutomaticReauthenticationAsync(
            token => _httpClient.GetAsync("v1/me/info/", token),
            cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        EnsureSuccess(response, responseBody, "내 정보 조회");

        _myInfo = Deserialize<UserInfo>(responseBody, "내 정보");
        return _myInfo;
    }

    public async Task<ReportsResponse> GetReportsAsync(
        string? next = null,
        CancellationToken cancellationToken = default)
    {
        var myInfo = _myInfo ?? await GetMyInfoAsync(cancellationToken);
        var child = myInfo.Children.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "키즈노트 계정에 등록된 자녀가 없습니다.");

        return await GetReportsAsync(child, next, cancellationToken);
    }

    public Task<ReportsResponse> GetReportsAsync(
        Child child,
        string? next = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(child);

        var enrollment = child.Enrollment.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"{child.Name} 자녀의 소속 반 정보가 없습니다.");

        return GetReportsAsync(
            enrollment.BelongToClass,
            child.Id,
            enrollment.CenterId,
            next,
            cancellationToken);
    }

    public async Task<ReportsResponse> GetReportsAsync(
        long classId,
        long childId,
        long centerId,
        string? next = null,
        CancellationToken cancellationToken = default)
    {
        EnsureLoggedIn();

        var query = new List<string>
        {
            $"cls={classId}",
        };

        if (!string.IsNullOrWhiteSpace(next))
        {
            query.Add($"page={Uri.EscapeDataString(next)}");
        }

        query.Add($"child={childId}");
        query.Add($"tz={Uri.EscapeDataString(_options.TimeZoneId)}");
        query.Add($"center_id={centerId}");

        var path =
            $"v1_2/children/{childId}/reports/?{string.Join("&", query)}";

        using var response = await SendWithAutomaticReauthenticationAsync(
            token => _httpClient.GetAsync(path, token),
            cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        EnsureSuccess(response, responseBody, "리포트 조회");
        return Deserialize<ReportsResponse>(responseBody, "리포트");
    }

    public void Dispose()
    {
        _reauthenticationLock.Dispose();
        _httpClient.Dispose();
    }

    private async Task LoginCoreAsync(
        string userId,
        string password,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            username = userId,
            password,
            remember_me = true,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "web/login/")
        {
            Content = JsonContent.Create(payload, options: _jsonOptions),
        };
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        EnsureSuccess(response, responseBody, "키즈노트 로그인");

        if (_cookieContainer.GetCookies(_httpClient.BaseAddress!).Count == 0)
        {
            throw new InvalidOperationException(
                "로그인은 성공했지만 응답에 세션 쿠키가 없습니다.");
        }

        _myInfo = null;
        IsLoggedIn = true;
        _authenticationGeneration++;
    }

    private static Uri CreateBaseAddress(string baseAddress)
    {
        if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps ||
            !uri.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Kidsnote:BaseAddress는 '/'로 끝나는 유효한 HTTPS 주소여야 합니다.");
        }

        return uri;
    }

    private void EnsureSuccess(
        HttpResponseMessage response,
        string responseBody,
        string operation)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.StatusCode is
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            IsLoggedIn = false;
            _myInfo = null;
        }

        throw new HttpRequestException(
            $"{operation} 실패: HTTP {(int)response.StatusCode} - {responseBody}",
            inner: null,
            response.StatusCode);
    }

    private T Deserialize<T>(string responseBody, string responseName)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(responseBody, _jsonOptions)
                ?? throw new JsonException(
                    $"{responseName} 응답 본문이 비어 있습니다.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                $"{responseName} 응답 형식을 해석하지 못했습니다.",
                exception);
        }
    }

    private async Task<HttpResponseMessage> SendWithAutomaticReauthenticationAsync(
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken)
    {
        var authenticationGeneration = _authenticationGeneration;
        var response = await sendAsync(cancellationToken);

        if (!IsSessionExpired(response.StatusCode))
        {
            return response;
        }

        response.Dispose();
        await ReauthenticateAsync(authenticationGeneration, cancellationToken);
        return await sendAsync(cancellationToken);
    }

    private async Task ReauthenticateAsync(
        long expiredAuthenticationGeneration,
        CancellationToken cancellationToken)
    {
        await _reauthenticationLock.WaitAsync(cancellationToken);

        try
        {
            if (IsLoggedIn &&
                _authenticationGeneration != expiredAuthenticationGeneration)
            {
                return;
            }

            IsLoggedIn = false;
            _myInfo = null;

            var login = GetSavedLogin();
            if (login is null)
            {
                throw new InvalidOperationException(
                    "키즈노트 로그인 세션이 만료되었습니다. 다시 로그인해주세요.");
            }

            await LoginCoreAsync(login.UserId, login.Password, cancellationToken);
        }
        finally
        {
            _reauthenticationLock.Release();
        }
    }

    private KidsnoteLoginRequest? GetSavedLogin()
    {
        if (_lastSuccessfulLogin is not null)
        {
            return _lastSuccessfulLogin;
        }

        return _options.HasCredentials
            ? new KidsnoteLoginRequest(_options.UserId!, _options.Password!)
            : null;
    }

    private static bool IsSessionExpired(HttpStatusCode statusCode) =>
        statusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

    private void EnsureLoggedIn()
    {
        if (!IsLoggedIn)
        {
            throw new InvalidOperationException(
                "키즈노트에 먼저 로그인해야 합니다.");
        }
    }

    public async Task<SingleReport> GetSingleReportAsync(long reportId, CancellationToken cancellationToken = default)
    {
        var myInfo = _myInfo ?? await GetMyInfoAsync(cancellationToken);
        var child = myInfo.Children.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "키즈노트 계정에 등록된 자녀가 없습니다.");

        var enrollment = child.Enrollment.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"{child.Name} 자녀의 소속 반 정보가 없습니다.");

        var report = await GetSingleReportAsync(
            reportId,
            enrollment.BelongToClass,
            child.Id,
            enrollment.CenterId,
            cancellationToken);

        return report;
    }

    public async Task<SingleReport> GetSingleReportAsync(long reportId, long classId, long childId, long centerId, CancellationToken cancellationToken = default)
    {
        var path =
            $"v1_2/reports/{reportId}/" +
            $"?cls={classId}" +
            $"&child={childId}" +
            $"&tz={Uri.EscapeDataString("Asia/Seoul")}" +
            $"&center_id={centerId}";

        using var response = await _httpClient.GetAsync(path);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"리포트 조회 실패: HTTP {(int)response.StatusCode} - {responseBody}");
        }

        var res = JsonSerializer.Deserialize<SingleReport>(responseBody, _jsonOptions);

        return res!;
    }
}
