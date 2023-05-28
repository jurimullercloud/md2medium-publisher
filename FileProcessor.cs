using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Md2Medium.Functions;
public class FileProcessor {

    private const string BASE_ADDRESS =  "https://raw.githubusercontent.com";
    
    // i.e -> <username>/<reponame>
    private readonly string _repoAddress;

    private readonly string _branch;
    private readonly string _filePath;
    public FileProcessor(string repoAddress, string branch, string filePath)
    {
        _repoAddress = repoAddress;
        _branch = branch;
        _filePath = filePath;
    }


    
    // finds the relative image paths, and replaces them with raw content links.
    private string replaceRelativeLinks(string rawMd)  {
        string pattern = Regex.Escape("/_media/images");
        var pathStr = string.Empty;
        foreach(Match match in Regex.Matches(rawMd, pattern)) {
            var idx = match.Index; 
            var braceIdX = rawMd.LastIndexOf('(', idx);
            pathStr = rawMd.Substring(braceIdX + 1 , idx - braceIdX -1); 
            break;
        }

        if (!string.IsNullOrEmpty(pathStr))
           return rawMd.Replace(pathStr, $"{BASE_ADDRESS}/{_repoAddress}/{_branch}");
        return rawMd; 
    }

    
    public async Task<string> GetFileAsync() {
        
        using HttpClient httpClient = new();
        var res = await httpClient.GetStringAsync(new Uri($"{BASE_ADDRESS}/{_repoAddress}/{_branch}/{_filePath}"));
        return replaceRelativeLinks(res);
    }

}

