using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Md2Medium.Functions;

public class Publisher {

    private const string BASE_ADDRESS = "https://api.medium.com/v1";
    private readonly string _integrationToken;

    public Publisher(string integrationToken)
    {
        _integrationToken = integrationToken;
    }



    private async Task<string> getUserInfoAsync(HttpClient client) {
        try {
            var json = await client.GetStringAsync($"{BASE_ADDRESS}/me?accessToken={_integrationToken}");
            return json;
        } catch (Exception ex) {
            Console.WriteLine("Failed to fetch user info", ex);
            return null;
        }

    }

    public async Task<string> PublishAsync(string title, string content) {
        HttpClient client = new();
        var json = await getUserInfoAsync(client);
        var userId = JsonConvert.DeserializeAnonymousType(json, new {data = new {id = ""}})?.data?.id;

        if (userId is null) 
            throw new Exception("User id is null, aborting..");


        var data = new {
            title = title,
            contentFormat = "markdown",
            content = content 
        };

        var req = new HttpRequestMessage();
        req.RequestUri = new Uri($"{BASE_ADDRESS}/users/{userId}/posts");
        req.Method = HttpMethod.Post;
        req.Headers.Authorization = new("Bearer", $"{_integrationToken}");
        req.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var res = await client.SendAsync(req);
        return await res.Content.ReadAsStringAsync();
    }


} 