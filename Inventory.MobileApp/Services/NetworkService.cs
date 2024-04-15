using Inventory.MobileApp.Models;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Inventory.MobileApp.Services;

public static class NetworkService
{
    private static HttpClient NetworkClient()
    {
        HttpClientHandler handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            return true;
        };
        return new(handler);
    }

    public static async Task<NetworkResponse<T>> Get<T>(string endpoint, Dictionary<string, string> parameters) 
    {
        try 
        {
            string url = $"{SessionService.APIUrl}/{endpoint}{QueryFrom(parameters)}";

            var client = NetworkClient();
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
            return new() { ErrorMessage = ex.ToString() };
        }
    }

    public static async Task<NetworkResponse<T>> Post<T>(string endpoint, object jsonBody) 
    {
        try 
        {
            string url = $"{SessionService.APIUrl}/{endpoint}";

            var client = NetworkClient();
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
            return new() { ErrorMessage = ex.ToString() };
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

    private static async Task<NetworkResponse<T>> HandleResponse<T>(HttpResponseMessage message)
    {
        string content = await message.Content.ReadAsStringAsync();
        switch (message.StatusCode)
        {
            case HttpStatusCode.OK:
                return new() { Data = JsonSerializer.Deserialize<T>(content) };
            case HttpStatusCode.InternalServerError:
                return new() { ErrorMessage = content };
            default:
                return new() { ErrorMessage = $"HTTP STATUS CODE {message.StatusCode} --- {message.RequestMessage?.RequestUri}" };
        }
    }
}
