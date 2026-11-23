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
        .Split("//") is var parts ? (parts[0], parts[1]) : default;

    using var handler = new HttpClientHandler
    {
        CookieContainer = new CookieContainer(),
        AutomaticDecompression = DecompressionMethods.All
    };
    using var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://www.kidsnote.com/")
    };
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));

    using var initial = await client.GetAsync("");
    initial.EnsureSuccessStatusCode();

    using var form = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["username"] = username,
        ["password"] = password,
        ["remember_me"] = "on"
    });
    using var login = await client.PostAsync("kr/login", form);
    login.EnsureSuccessStatusCode();

    using var response = await client.GetAsync("api/v1/me/info/");
    response.EnsureSuccessStatusCode();
    if (!string.Equals(response.Content.Headers.ContentType?.MediaType,
            "application/json", StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "계정 정보가 JSON으로 반환되지 않았습니다. 로그인 상태를 확인하세요.");
    }

    var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    var account = await response.Content.ReadFromJsonAsync<AccountInfo>(options);
    var children = account?.Children
        ?? throw new InvalidOperationException("계정 정보 응답에 자녀 목록이 없습니다.");

    Console.WriteLine($"등록된 자녀 수: {children.Length}");
    children.Dump("자녀 목록");
}

class AccountInfo
{
    public Child[]? Children { get; init; }
}

class Child
{
    public long Id { get; init; }
    public string? Name { get; init; }
    public Enrollment[]? Enrollment { get; init; }
}

class Enrollment
{
    public long CenterId { get; init; }
    public string? CenterName { get; init; }
    public long BelongToClass { get; init; }
    public string? ClassName { get; init; }
}
