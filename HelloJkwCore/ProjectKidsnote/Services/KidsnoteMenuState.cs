namespace ProjectKidsnote.Services;

internal sealed class KidsnoteMenuState
{
    public event Action? Changed;

    public void NotifyChanged() => Changed?.Invoke();
}
