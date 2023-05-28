using Newtonsoft.Json;

namespace Md2Medium.Models;

[JsonObject]
public class Blog {
    [JsonProperty("repoAddress")]
    public string RepoAddress {get; set;}

    [JsonProperty("branch")]
    public string Branch {get; set;}


    [JsonProperty("filePath")]
    public string FilePath {get; set;}
    
    
    [JsonProperty("title")]
    public string Title {get; set;}
}