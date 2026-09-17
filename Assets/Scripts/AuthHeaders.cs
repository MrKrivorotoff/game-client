using System;
using System.Text;

public static class AuthHeaders
{
    public static string CreateBasicValue(string username, string password)
    {
        var bytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        return $"Basic {Convert.ToBase64String(bytes)}";
    }

    public static string CreateBearerValue(string token)
    {
        return $"Bearer {token}";
    }
}