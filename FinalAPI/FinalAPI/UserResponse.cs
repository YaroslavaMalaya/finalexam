using Newtonsoft.Json;

namespace FinalAPI;

public class UserResponse
{
    [JsonProperty("nickname")]
    public string Nickname { get; set; }

    [JsonProperty("userId")]
    public string UserID { get; set; }
    
    [JsonProperty("lastSeenDate")]
    public string FirstSeen { get; set; }
}

public class UserData
{
    public UserResponse[] data { get; set; }
}

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly List<UserResponse> _users = new List<UserResponse>();

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync()
    {
        if (_users.Count == 0)
        {
            await FetchUsersFromApi();
        }

        return _users;
    }

    private async Task FetchUsersFromApi()
    {
        var offset = 0;
        while (true)
        {
            var apiUrl = $"https://sef.podkolzin.consulting/api/users/lastSeen?offset={offset}";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<UserData>(json);
                var userCount = userData?.data?.Length ?? 0;

                if (userCount == 0)
                {
                    break;
                }

                foreach(var user in userData.data)
                {
                    var existingUser = _users.FirstOrDefault(u => u.UserID == user.UserID);
                    if(existingUser == null)
                    {
                        _users.Add(user);
                    }
                    else if(String.CompareOrdinal(existingUser.FirstSeen, user.FirstSeen) > 0)
                    {
                        existingUser.FirstSeen = user.FirstSeen;
                    }
                }
                offset += userCount;
            }
            else
            {
                throw new Exception($"Failed to fetch users: {response.StatusCode}");
            }
        }
    }
}
