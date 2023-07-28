using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RSAExample
{
    class Program
    {
        static bool IsPrime(BigInteger number)
        {
            if (number <= 1)
                return false;

            for (BigInteger i = 2; i * i <= number; i++)
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }

        static Tuple<BigInteger, BigInteger> GenerateKeyPair(BigInteger p, BigInteger q)
        {
            if (!(IsPrime(p) && IsPrime(q)))
                throw new ArgumentException("p ve q değerleri asal olmadılır.");
            else if (p == q)
                throw new ArgumentException("p ve q değerleri birbirinden farklı olmalıdır.");

            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);

            // phi ile asal olan bir e değeri bulunana kadar rastgele değerler seçilir.
            Random rand = new Random();
            BigInteger e;
            do
            {
                e = new BigInteger(rand.Next(2, (int)phi));
            } while (BigInteger.GreatestCommonDivisor(e, phi) != 1);

            // Öklidyen genişletilmiş algoritması kullanılarak d hesabı yapılır.
            BigInteger d = BigInteger.ModPow(e, -1, phi);

            return Tuple.Create(e, n);
        }

        static BigInteger[] Encrypt(BigInteger[] publicKey, string plaintext)
        {
            BigInteger e = publicKey[0];
            BigInteger n = publicKey[1];

            List<BigInteger> cipher = new List<BigInteger>();

            foreach (char c in plaintext)
            {
                BigInteger m = (BigInteger)c;
                BigInteger crypted = BigInteger.ModPow(m, e, n);
                cipher.Add(crypted);
            }

            return cipher.ToArray();
        }

        static string Decrypt(BigInteger privateKey, BigInteger[] ciphertext)
        {
            BigInteger d = privateKey;
            BigInteger n = ciphertext[1];

            List<char> plain = new List<char>();

            foreach (BigInteger c in ciphertext)
            {
                BigInteger decrypted = BigInteger.ModPow(c, d, n);
                plain.Add((char)decrypted);
            }

            return new string(plain.ToArray());
        }

        static void Main(string[] args)
        {
            BigInteger p = 61;
            BigInteger q = 53;

            var keyPair = GenerateKeyPair(p, q);
            Console.WriteLine("Public Key: ({0}, {1})", keyPair.Item1, keyPair.Item2);

            BigInteger publicKey = keyPair.Item1;

            string plaintext = "Merhaba, dünya!";
            Console.WriteLine("Açık Mesaj: " + plaintext);

            BigInteger[] ciphertext = Encrypt(new BigInteger[] { publicKey, keyPair.Item2 }, plaintext);
            Console.WriteLine("Şifreli Mesaj: " + string.Join(" ", ciphertext));

            string decryptedMessage = Decrypt(keyPair.Item1, ciphertext);
            Console.WriteLine("Şifre Çözülmüş Mesaj: " + decryptedMessage);
        }
    }
}
