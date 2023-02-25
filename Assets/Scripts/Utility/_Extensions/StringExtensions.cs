using System;
using System.Linq;

public static class StringExtensions
{
    /// <summary>
    /// listの中に共通する文字列が含まれているか
    /// </summary>
    public static bool IncludeAny(this string self,params string[] list)
    {
        return list.Any(c => self.Contains(c));
    }
}