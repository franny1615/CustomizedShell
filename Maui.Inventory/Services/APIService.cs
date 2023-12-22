using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using System.Text;
using System.Text.Json;

namespace Maui.Inventory.Services;

public interface IAPIService
{
    public Task<T> Get<T>(
        string endpoint,
        Dictionary<string, string> parameters) where T : new();

    public Task<T> Post<T>(
        string endpoint, 
        object jsonParams) where T : new();
}

public class APIService : IAPIService
{
    private readonly HttpClient _Client;
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<ApiUrl> _ApiUrl;

    public async Task<T> Get<T>(
        string endpoint,
        Dictionary<string, string> parameters) where T : new()
    {
        try
        {
            int apiUrlId = Preferences.Get(Constants.ApiUrlId, 1);
            var api = (await _ApiUrl.GetAll()).First(api => api.Id == apiUrlId);
            
            string accessToken = (await _UserDAL.GetAll()).First().AccessToken;

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

            string apiFull = $"{api}{endpoint}?{queryParams}";

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(apiFull);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await _Client.SendAsync(request);

            return await DealWithResponse<T>(response);
        }
        catch { /* TODO: add logging */}

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
            string accessToken = (await _UserDAL.GetAll()).First().AccessToken;
            string apiFull = $"{api}{endpoint}";

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(apiFull);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Content = new StringContent(
                JsonSerializer.Serialize(jsonParams), 
                Encoding.UTF8, 
                "application/json");

            var response = await _Client.SendAsync(request);

            return await DealWithResponse<T>(response);
        }
        catch { /* TODO: add logging */}

        return new();
    }

    private async Task<T> DealWithResponse<T>(HttpResponseMessage response) where T : new()
    {
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                var content = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(content);
            case System.Net.HttpStatusCode.Unauthorized:
            case System.Net.HttpStatusCode.Forbidden:
            default:
                // TODO: forward log out message to main app
                return new();
        }
    }
}
