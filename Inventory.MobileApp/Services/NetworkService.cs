using System.Net;
using System.Text;
using System.Text.Json;

namespace Inventory.MobileApp.Services;

public static class NetworkService
{
    public static async Task<T?> Get<T>(string endpoint, Dictionary<string, string> parameters) 
    {
        try 
        {
            string url = $"{SessionService.APIUrl}/{endpoint}{QueryFrom(parameters)}";

            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get, url);

            string authToken = SessionService.AuthToken;
            if (!string.IsNullOrEmpty(authToken)) 
            {
                message.Headers.Add("Authorization", $"Bearer {authToken}");
            }

            HttpResponseMessage response = await client.SendAsync(message);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);

            return default(T);
        }
    }

    public static async Task<T?> Post<T>(string endpoint, object jsonBody) 
    {
        try 
        {
            string url = $"{SessionService.APIUrl}/{endpoint}";

            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Post, url);

            string authToken = SessionService.AuthToken;
            if (!string.IsNullOrEmpty(authToken)) 
            {
                message.Headers.Add("Authorization", $"Bearer {authToken}");
            }

            string content = JsonSerializer.Serialize(jsonBody);
            message.Content = new StringContent(content, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(message);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);

            return default(T);
        }
    }

    private static string QueryFrom(Dictionary<string, string> parameters)
    {
        string result = "";
        var keys = parameters.Keys.ToList();
        for (int i = 0; i < keys.Count; i++) 
        {
            var key = keys[i];
            var value = parameters[key]; 
            result += $"{key}={value}";
            if (i < keys.Count - 1) 
            {
                result += "&";
            }
        }
        if (!string.IsNullOrEmpty(result))
        {
            result = $"?{result}";   
        }
        return result;
    }

    private static async Task<T?> HandleResponse<T>(HttpResponseMessage message)
    {
        switch (message.StatusCode)
        {
            case HttpStatusCode.OK:
                try 
                {
                    string content = await message.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return default(T);
                }
            default:
                System.Diagnostics.Debug.WriteLine($"HTTP STATUS CODE {message.StatusCode} --- {message.RequestMessage?.RequestUri}");
                return default(T);
        }
    }
}
