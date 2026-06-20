using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace UnlockWarning;

public static class LicenseManager
{
    private const string RegPath = @"SOFTWARE\CJY\License";
    private const string SerialValueName = "Serial";
    private const string ExpiryValueName = "Expiry";
    private static readonly byte[] SecretKey = Encoding.UTF8.GetBytes("CJY@AnquanWeishi#License2025");

    public static string GetMachineCode()
    {
        StringBuilder sb = new StringBuilder();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                sb.Append(obj["ProcessorId"]?.ToString() ?? "");
                break;
            }
        }
        catch { }
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
            foreach (var obj in searcher.Get())
            {
                sb.Append(obj["SerialNumber"]?.ToString() ?? "");
                break;
            }
        }
        catch { }
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE Index=0");
            foreach (var obj in searcher.Get())
            {
                sb.Append(obj["SerialNumber"]?.ToString() ?? "");
                break;
            }
        }
        catch { }

        if (sb.Length == 0)
            sb.Append(Environment.MachineName);

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
        StringBuilder result = new StringBuilder();
        foreach (byte b in hash)
            result.Append(b.ToString("x2"));
        return result.ToString();
    }

    public static (bool valid, DateTime? expiry) VerifyLicense(string serial)
    {
        try
        {
            serial = serial.Trim();
            string[] parts = serial.Split(':');
            if (parts.Length != 2)
                return (false, null);

            string payloadBase64 = parts[0];
            string hmacBase64 = parts[1];

            byte[] payload = Convert.FromBase64String(payloadBase64);
            string payloadStr = Encoding.UTF8.GetString(payload);
            string[] data = payloadStr.Split('|');
            if (data.Length != 2)
                return (false, null);

            string machineCode = data[0];
            if (!long.TryParse(data[1], out long ticks))
                return (false, null);

            byte[] expectedHmac = HMACSHA256.HashData(SecretKey, payload);
            string expectedHmacBase64 = Convert.ToBase64String(expectedHmac);

            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(hmacBase64),
                    Encoding.UTF8.GetBytes(expectedHmacBase64)))
                return (false, null);

            if (machineCode != GetMachineCode())
                return (false, null);

            DateTime expiry = new DateTime(ticks, DateTimeKind.Utc);
            if (DateTime.UtcNow <= expiry)
                return (true, expiry);
            else
                return (false, expiry);

        }
        catch
        {
            return (false, null);
        }
    }

    public static void SaveActivation(string serial, DateTime expiry)
    {
        try
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(RegPath);
            key.SetValue(SerialValueName, serial);
            key.SetValue(ExpiryValueName, expiry.Ticks.ToString());
        }
        catch { }
    }

    public static (bool activated, string? serial, DateTime? expiry) LoadActivation()
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegPath);
            if (key == null)
                return (false, null, null);

            string? serial = key.GetValue(SerialValueName) as string;
            string? expiryStr = key.GetValue(ExpiryValueName) as string;

            if (string.IsNullOrEmpty(serial) || string.IsNullOrEmpty(expiryStr))
                return (false, null, null);

            if (!long.TryParse(expiryStr, out long ticks))
                return (false, null, null);

            DateTime expiry = new DateTime(ticks, DateTimeKind.Utc);
            if (DateTime.UtcNow <= expiry)
            {
                var (valid, _) = VerifyLicense(serial);
                if (valid)
                    return (true, serial, expiry);
            }

            return (false, serial, expiry);
        }
        catch
        {
            return (false, null, null);
        }
    }
}
