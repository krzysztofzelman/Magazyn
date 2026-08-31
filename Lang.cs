namespace MagazynWeb;

public static class Lang
{
    public static string Current = "pl";

    private static Dictionary<string, string> pl = new Dictionary<string, string>
    {
        { "Add", "Dodaj" },
        { "Save", "Zapisz" },
        { "Delete", "Usuń" },
        { "Edit", "Edytuj" },
        { "Dispatch", "Wydaj" },
        { "Loading", "Ładowanie..." },
        { "Name", "Nazwa" },
        { "Quantity", "Ilość" },
        { "Price", "Cena" },
        { "Search", "Szukana nazwa" },
        { "Value", "Wartość" },
        { "Actions", "Akcje" },
        { "TotalValue", "Wartość magazynu" },
        { "TotalQuantity", "Liczba sztuk" },
        { "TotalItems", "Liczba pozycji" },
        { "Warehouse", "Magazyn" },
        { "Batches", "Partie" },
        { "BatchNo", "Nr partii" },
        { "ItemId", "Id artykułu" },
        { "Item", "Artykuł" },
        { "Date", "Data" }
    };

    private static Dictionary<string, string> en = new Dictionary<string, string>
    {
    { "Add", "Add" },
        { "Save", "Save" },
        { "Delete", "Delete" },
        { "Edit", "Edit" },
        { "Dispatch", "Dispatch" },
        { "Loading", "Loading..." },
        { "Name", "Name" },
        { "Quantity", "Quantity" },
        { "Price", "Price" },
        { "Search", "Search name" },
        { "Value", "Value" },
        { "Actions", "Actions" },
        { "TotalValue", "Total value" },
        { "TotalQuantity", "Total quantity" },
        { "TotalItems", "Total items" },
        { "Warehouse", "Warehouse" },
        { "Batches", "Batches" },
        { "BatchNo", "Batch no." },
        { "ItemId", "Item id" },
        { "Item", "Item" },
        { "Date", "Date" }
    };

    public static string T(string key)
    {
        if (Current == "en")
        {
            return en[key];
        }
        return pl[key];
    }

    public static void Toggle()
    {
        if (Current == "pl")
        {
            Current = "en";
        }
        else
        {
            Current = "pl";
        }
    }

    public static void Set(string lang)
    {
        Current = lang;
    }
}