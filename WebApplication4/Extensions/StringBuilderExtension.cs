using System.Text;

namespace WebApplication4.Extensions
{  
    internal static class StringBuilderExtension
    {
        public static int IndexOf(this StringBuilder sb, char value)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == value)
                    return i;
            }

            return -1;
        }

        public static StringBuilder Remove(this StringBuilder sb, int startIndex)
        {
            for (int i = sb.Length - 1; i >= startIndex; i--)
            {
                sb.Remove(i, 1);
            }

            return sb;
        }
    }
}
