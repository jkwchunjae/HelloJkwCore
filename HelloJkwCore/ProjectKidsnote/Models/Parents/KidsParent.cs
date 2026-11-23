using Common;

namespace ProjectKidsnote.Models.Parents;

/// <summary>
/// 키즈노트 부모님의 정보를 저장한다.
/// UserRole.Kidsnote 권한이 있는 사람이 객체를 생성 하게 된다.
/// 
/// Azure, Dropbox에 저장한다.
/// 키즈노트 인증정보도 저장해야 하는데, 암호화 해서 저장한다.
/// </summary>
internal class KidsParent
{
    /// <summary>
    /// 처음으로 내 홈페이지에 아이를 등록한 사람
    /// </summary>
    public UserId User { get; init; } = default!;
    public long Revision { get; set; }
    public bool CanManage(UserId id) => User == id || Partners.Contains(id);
    public bool CanRead(UserId id, Kid kid) => CanManage(id) || AllowedList.Contains(id) || kid.AllowedList.Contains(id);
    /// <summary>
    /// 위 User가 등록한 배우자.
    /// UserRole.Kidsnote 권한이 없어도 권한이 있는 것 처럼 동작한다.
    /// </summary>
    public List<UserId> Partners { get; init; } = new();
    /// <summary>
    /// 키즈노트 서비스 인증 정보
    /// </summary>
    public KidsnoteAuth Auth { get; set; } = default!;
    /// <summary>
    /// 홈페이지에서 서비스할 아이 정보
    /// </summary>
    public List<Kid> Kids { get; init; } = new();
    /// <summary>
    /// 내 아이 모두를 볼 수 있는 사람 목록.
    /// 여기에 등록되면 내 모든 아이의 알림장을 읽을 수 있다.
    /// </summary>
    public List<UserId> AllowedList { get; set; } = new();
}

/// <summary>
/// 키즈노트 서비스 인증 정보
/// </summary>
internal class KidsnoteAuth
{
    /// <summary>
    /// 키즈노트 서비스 아이디/비밀번호를 암호화해서 저장한다.
    /// </summary>
    public string? EncryptedAuthInfo { get; set; }
}

/// <summary>
/// 키즈노트 아이 한 명에 대한 데이터
/// </summary>
internal class Kid
{
    /// <summary>
    /// URL에 사용할 값.
    /// https://hellojkw.com/kidsnote/{NicknameForUrl} 로 접근 가능하다.
    /// 예를 들면
    /// https://hellojkw.com/kidsnote/jkw 로 접근 가능하다.
    /// Azure, dropbox에 저장할 때에도 폴더 명으로 사용한다.
    /// 알파벳 소문자, 숫자만 허용한다. 특수문자, 한글은 허용하지 않는다.
    /// </summary>
    public string NicknameForUrl { get; set; } = default!;
    /// <summary>
    /// 이 아이를 볼 수 있는 사람 목록.
    /// </summary>
    public List<UserId> AllowedList { get; init; } = new();

    public long ChildId { get; init; }
    public string Name { get; init; } = "";
    public long CenterId { get; init; }
}
