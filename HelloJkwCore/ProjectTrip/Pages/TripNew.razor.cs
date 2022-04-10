using KakaoMapBlazor.Marker;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.RegularExpressions;

namespace ProjectTrip.Pages;

public partial class TripNew : JkwPageBase
{
    [Inject]
    private IJSRuntime JS { get; set; }
    [Inject]
    private ITripService TripService { get; set; }
    [Inject]
    private UserManager<AppUser> UserManager { get; set; }


    KakaoMapComponent kakaoMapComponent;
    IKakaoMap KakaoMap => kakaoMapComponent?.Instance;
    MapCreateOption mapCreateOption = new MapCreateOption(new LatLng(36.55506321886859, 127.61013231891525))
    {
        MapTypeId = MapType.RoadMap,
        Level = 12,
    };

    MudForm _form;
    AppUser searchPartner;
    IList<AppUser> _allUsers;
    List<string> DuplicatedTripId = new();

    bool FormSuccess;
    bool FormSuccessed => FormSuccess && NewTrip.Positions.Any() && IsAuthenticated;

    Trip NewTrip = new();
    string TripTitle;
    string TripId;
    DateRange DateRange = new DateRange(DateTime.Now.Date, DateTime.Now.Date);
    List<AppUser> TripPartners = new();

    protected override Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            KakaoMap.Click += async (s, e) =>
            {
                var position = e.LatLng;
                var marker = await KakaoMap.CreateMarker(
                    new MarkerCreateOptionInMap { Position = position });
                NewTrip.Positions.Add(position);
                StateHasChanged();
                marker.Click += async (s, e) =>
                {
                    NewTrip.Positions.Remove(position);
                    StateHasChanged();
                    await marker.Close();
                };
            };
        }

        return Task.CompletedTask;
    }

    private IEnumerable<string> TripTitleValidator(string tripTitle)
    {
        if (string.IsNullOrWhiteSpace(tripTitle))
        {
            yield return "여행 이름을 입력해주세요.";
        }
    }

    private IEnumerable<string> TripIdValidator(string tripId)
    {
        var pattern = "^[a-z0-9-]*$";
        if (string.IsNullOrWhiteSpace(tripId))
        {
            yield return "여행 ID를 입력해주세요.";
        }
        else if (!Regex.IsMatch(tripId, pattern))
        {
            yield return "여행 ID는 소문자,숫자,하이픈(-) 만 가능합니다.";
        }
        else if (tripId.Length < 10)
        {
            yield return "여행 ID는 10자 이상으로 해주세요.";
        }
        else if (DuplicatedTripId.Any(id => id.Equals(tripId, StringComparison.OrdinalIgnoreCase)))
        {
            yield return "여행 ID가 중복되었습니다. 다른 ID를 정해주세요.";
        }
    }

    private async Task<IEnumerable<AppUser>> SearchPartner(string keyword)
    {
        if (!IsAuthenticated)
            return new List<AppUser>();

        if (_allUsers == null)
        {
            _allUsers = await UserManager.GetUsersInRoleAsync("all");
        }

        var filtered = _allUsers
            .Where(user => user != User)
            .Where(user => user.DisplayName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
                        || (user.Email?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ?? true))
            .Take(3)
            .ToList();

        return filtered;
    }

    private void OnPartnerSelect()
    {
        if (searchPartner != null && !TripPartners.Contains(searchPartner))
        {
            TripPartners.Add(searchPartner);
            searchPartner = null;
        }
    }

    private async Task RemovePartner(AppUser partner)
    {
        await JS.InvokeVoidAsync("console.log", "RemovePartner", partner);
        TripPartners.RemoveAll(x => x.Id == partner.Id);
    }

    private async Task CreateTrip()
    {
        await _form.Validate();

        if (FormSuccessed)
        {
            var duplicated = await TripService.ExistsTripIdAsync(new TripId(TripId));
            if (duplicated)
            {
                if (!DuplicatedTripId.Contains(TripId))
                    DuplicatedTripId.Add(TripId);
                await _form.Validate();
                return;
            }

            NewTrip.Title = new TripTitle(TripTitle);
            NewTrip.Id = new TripId(TripId);
            NewTrip.AddUser(User);
            NewTrip.AddUsers(TripPartners);
            NewTrip.BeginTime = DateRange.Start.Value;
            NewTrip.EndTime = DateRange.End.Value;

            await JS.InvokeVoidAsync("console.log", "CreateTrip2", NewTrip);
            await TripService.CreateOrUpdateTripAsync(NewTrip);
            Navi.NavigateTo($"/trip/trip/{NewTrip.Id}");
        }
    }
}
