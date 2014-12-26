using System.Linq;

namespace FileNotes.Monitor.Tool
{
    internal static class StringExtensions
    {
        public static string SafetyTake(this string str, int amount)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            return new string(str.Take(amount).ToArray());
        }
    }
}