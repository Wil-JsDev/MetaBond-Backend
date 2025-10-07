using System.Security.Cryptography;

namespace MetaBond.Application.Utils;

public static class TokenGenerator
{
    public static string GenerateNumericToken(int digits = 6)
    {
        if (digits is <= 0 or > 9)
            throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be between 1 and 9.");

        var max = (int)Math.Pow(10, digits); // 1000000 for 6 digits
        var min = (int)Math.Pow(10, digits - 1); // 100000 for 6 digits

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4]; // int = 4 bytes

        int number;
        do
        {
            rng.GetBytes(bytes);

            number = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
        } while (number < min || number >= max);

        return number.ToString();
    }
}