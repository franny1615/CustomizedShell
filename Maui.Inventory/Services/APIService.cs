using CommunityToolkit.Mvvm.Messaging;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Maui.Inventory.Services;

public class APIService : IAPIService
{
    private readonly HttpClient _Client;
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<ApiUrl> _ApiUrl;
    private readonly IDAL<Admin> _AdminDAL;

    public APIService(
        IDAL<User> userDAL, 
        IDAL<Admin> adminDAL,
        IDAL<ApiUrl> apiURL)
    {
        _UserDAL = userDAL;
        _AdminDAL = adminDAL;
        _ApiUrl = apiURL;
        _Client = new()
        {
            Timeout = TimeSpan.FromMinutes(5)
        };   
    }

    public async Task<T> Get<T>(
        string endpoint,
        Dictionary<string, string> parameters) where T : new()
    {
        try
        {
            int apiUrlId = Preferences.Get(Constants.ApiUrlId, 1);
            var api = (await _ApiUrl.GetAll()).First(api => api.Id == apiUrlId);

            string queryParams = "";
            #region BUILD QUERY PARAMETERS
            bool first = true;
            foreach(var pram in parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    queryParams += "&";
                }
                queryParams += pram.Key;
                queryParams += "=";
                queryParams += pram.Value;
            }
            #endregion

            string apiFull = $"{api.URL}{endpoint}?{queryParams}";

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(apiFull);
            
            string accessToken = await GetAccessToken();
            if (accessToken.Length > 0)
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            var response = await _Client.SendAsync(request);

            return await DealWithResponse<T>(response);
        }
        catch { /* TODO: add logging */ }

        return new();
    }

    public async Task<T> Post<T>(
        string endpoint,
        object jsonParams) where T : new()
    {
        try
        {
            int apiUrlId = Preferences.Get(Constants.ApiUrlId, 1);
            var api = (await _ApiUrl.GetAll()).First(api => api.Id == apiUrlId);
            string apiFull = $"{api.URL}{endpoint}";

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(apiFull);

            string accessToken = await GetAccessToken();
            if (accessToken.Length > 0)
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            request.Content = new StringContent(
                JsonSerializer.Serialize(jsonParams), 
                Encoding.UTF8, 
                "application/json");

            var response = await _Client.SendAsync(request);

            return await DealWithResponse<T>(response);
        }
        catch { /* TODO: add logging */ }

        return new();
    }

    private async Task<string> GetAccessToken()
    {
        var users = await _UserDAL.GetAll();
        var admins = await _AdminDAL.GetAll();
        
        string accessToken = "";
        if (users.Count > 0)
        {
            accessToken = users.First().AccessToken;
        }
        else if (admins.Count > 0)
        {
            accessToken = admins.First().AccessToken;
        }

        return accessToken;
    }

    private async Task<T> DealWithResponse<T>(HttpResponseMessage response) where T : new()
    {
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                var content = await response.Content.ReadAsStreamAsync();
                var apiResponse = await JsonSerializer.DeserializeAsync<APIResponse<T>>(content);
                // TODO: add logging for failures 
                return apiResponse.Data;
            case System.Net.HttpStatusCode.Unauthorized:
            case System.Net.HttpStatusCode.Forbidden:
            default:
                WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.AccessTokenExpired));
                return new();
        }
    }
}
