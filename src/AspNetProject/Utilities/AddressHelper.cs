namespace AspNetProject.Utilities;

public static class AddressHelper
{
    public static string GetStreetName(string address)
    {
        int dotIndex = address.LastIndexOf(".");
        return address.Substring(0, dotIndex + 1);
    }
}