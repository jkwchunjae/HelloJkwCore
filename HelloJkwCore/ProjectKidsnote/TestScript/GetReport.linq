<Query Kind="Program">
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
    var (username, password) = Util.GetPassword("kidsnote-summer-auth")
        .Split("//")  is var parts ? (parts[0], parts[1]) : default;

    var cookieContainer = new CookieContainer();

    using var handler = new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = cookieContainer,
        AllowAutoRedirect = true,
        AutomaticDecompression = DecompressionMethods.All
    };

    using var client = new HttpClient(handler);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

    // 최초 페이지에서 필요한 초기 쿠키가 있다면 CookieContainer에 저장됩니다.
    using var initial = await client.GetAsync("https://www.kidsnote.com/");
    initial.EnsureSuccessStatusCode();

    using var form = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["username"] = username,
        ["password"] = password,
        ["remember_me"] = "on"
    });

    using var login = await client.PostAsync(
        "https://www.kidsnote.com/kr/login", form);

    login.EnsureSuccessStatusCode();

    const string reportsUrl =
        "https://www.kidsnote.com/api/v1_2/children/5243355/reports/?page=cD0yMDI1LTEwLTE2KzAzJTNBMjglM0EzMC44MTUwMzglMkIwMCUzQTAw";

    using var reportsResponse = await client.GetAsync(reportsUrl);
    reportsResponse.EnsureSuccessStatusCode();

    var contentType = reportsResponse.Content.Headers.ContentType?.MediaType;
    if (!string.Equals(contentType, "application/json",
            StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "JSON 응답이 아닙니다. 로그인 실패 또는 로그인 절차 변경 가능성이 있습니다.");
    }

    var reports = await reportsResponse.Content
        .ReadFromJsonAsync<ReportsResponse>(_options)
        ?? throw new InvalidOperationException("알림장 응답을 읽지 못했습니다.");

    Console.WriteLine($"총 알림장 수: {reports.Count}");
    reports.Dump(1);
    
    var report = reports.Results![0];
    var singleResponse = await GetSingleReportAsync(client, report.Id, report.cls, 5243355, report.Center, default);

    singleResponse.Dump(1);
}

JsonSerializerOptions _options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    PropertyNameCaseInsensitive = true
};

public async Task<SingleReport> GetSingleReportAsync(
    HttpClient _httpClient,
    long reportId,
    long classId,
    long childId,
    long centerId,
    CancellationToken cancellationToken = default)
{
    if (reportId <= 0)
    {
        throw new ArgumentOutOfRangeException(
            nameof(reportId),
            "알림장 번호는 0보다 커야 합니다.");
    }

    var path =
        $"https://www.kidsnote.com/api/v1_2/reports/{reportId}" +
        $"";

    using var response = await SendWithAutomaticReauthenticationAsync(
        token => _httpClient.GetAsync(path, token),
        cancellationToken);
    var report = await response.Content.ReadFromJsonAsync<SingleReport>(_options);
    return report;
}
private async Task<HttpResponseMessage> SendWithAutomaticReauthenticationAsync(
    Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
    CancellationToken cancellationToken)
{
    var response = await sendAsync(cancellationToken);

    response.Dispose();
    return await sendAsync(cancellationToken);
}


static string RequiredEnvironmentVariable(string name) =>
    Environment.GetEnvironmentVariable(name)
    ?? throw new InvalidOperationException($"{name} 환경 변수가 없습니다.");



class ReportsResponse
{
    public int Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
    public SingleReport[]? Results { get; set; }
}

public class SingleReport
{
    public long Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public DateOnly DateWritten { get; set; }
    public string AuthorName { get; set; } = default!;
    public int Center { get; set; }
    public int cls { get; set; }
    public string ClassName { get; set; } = default!;
    public string ChildName { get; set; } = default!;
    public string? Content { get; set; }
    public string? Weather { get; set; }
    public KidsnoteImage[]? AttachedImages { get; set; }
}

public class KidsnoteImage
{
    public long Id { get; set; }
    public string AccessKey { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Original { get; set; } = default!;
    public string Large { get; set; } = default!;
    public string LargeResize { get; set; } = default!;
    public string Small { get; set; } = default!;
    public string SmallResize { get; set; } = default!;
}