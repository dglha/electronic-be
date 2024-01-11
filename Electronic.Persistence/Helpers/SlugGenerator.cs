using System.Text.RegularExpressions;

namespace Electronic.Persistence.Helpers;

public static class SlugGenerator
{
    private static string RemoveAccent(this string str)
    {
        var bytes = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(str);
        return System.Text.Encoding.ASCII.GetString(bytes);
    }
    
    public static string Generate(string phrase) 
    { 
        var str = phrase.RemoveAccent().ToLower(); 

        str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
        str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
        // str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it   
        str = Regex.Replace(str, @"\s", "-"); // hyphens   

        return str; 
    } 
}