using System.Text;

namespace WarningUnlock;
using System.Security.Cryptography;
public class sha
{
    private string Path = "";
    public sha(string path)
    {
        Path = path;
    }
    public string GetFileSHA256()
    {
        using (FileStream stream = File.OpenRead(Path))
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hashBytes = sha.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2")); // 转16进制字符串
            return sb.ToString();
        }
    }
}