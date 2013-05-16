namespace Honeycomb
{
    public static class StringExtensions
    {
         public static string WithParams(this string template, params object[] args)
         {
             return string.Format(template, args);
         }
    }
}