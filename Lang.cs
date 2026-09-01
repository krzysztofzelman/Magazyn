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
        { "Cancel", "Anuluj" },
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
        { "Date", "Data" },
        {"ServerError", "Błąd serwera" },
        {"NotFound", "Nie znaleziono" },
        {"Unauthorized", "Nieautoryzowany" },
        {"Forbidden", "Zabroniony" },
        {"BadRequest", "Nieprawidłowe żądanie" },
        {"Conflict", "Konflikt" },
        {"InternalServerError", "Błąd wewnętrzny serwera" },
        {"NotEnoughStock", "Za mało towaru" },
        {"EnterQuantity", "Podaj ilość" }
    };

    private static Dictionary<string, string> en = new Dictionary<string, string>
    {
    { "Add", "Add" },
        { "Save", "Save" },
        { "Delete", "Delete" },
        { "Edit", "Edit" },
        { "Dispatch", "Dispatch" },
        { "Cancel", "Cancel" },
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
        { "Date", "Date" },
        {"ServerError", "Server Error" },
        {"NotFound", "Not Found" },
        {"Unauthorized", "Unauthorized" },
        {"Forbidden", "Forbidden" },
        {"BadRequest", "Bad Request" },
        {"Conflict", "Conflict" },
        {"InternalServerError", "Internal Server Error"  },
        {"NotEnoughStock", "Not enough stock!" },
        {"EnterQuantity", "Enter quantity" },
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