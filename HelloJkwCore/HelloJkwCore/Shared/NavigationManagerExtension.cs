namespace HelloJkwCore;

public static class NavigationManagerExtension
{
    public static void GoLoginPage(this NavigationManager navi)
    {
        navi.NavigateTo("/login");
    }
}