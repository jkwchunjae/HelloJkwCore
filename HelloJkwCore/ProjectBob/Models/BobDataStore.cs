namespace ProjectBob;

public class BobDataStore
{
    public int Version { get; set; } = 1;
    public List<BobMenu> Menus { get; set; } = [];
    public List<BobFridgeItem> FridgeItems { get; set; } = [];
    public List<BobPurchaseUnit> PurchaseUnits { get; set; } = [];
    public BobChildProfile ChildProfile { get; set; } = new();
    public List<BobCalendarEntry> CalendarEntries { get; set; } = [];
}
