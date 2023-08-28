using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Secret
{
  internal class Program
  {
    public static byte[] XOR(byte[] msg, byte[] key)
    {
      if(msg.Length != key.Length)
        return Encoding.UTF8.GetBytes("error");
      byte[] data = new byte[msg.Length];
      for(int i = 0; i < msg.Length; i++)
        data[i] = (byte)(msg[i] ^ key[i]);
      return data;
    }

    static void Main()
    {
      Console.Write("Input your file path: ");
      string file_path = Console.ReadLine();
      Console.Write("How many pieces: ");
      int n = int.Parse(Console.ReadLine());
      byte[] data = File.ReadAllBytes(file_path);
      using(var rng = new RNGCryptoServiceProvider())
      {
        for(int i = 0; i < n-1; i++)
        {
          byte[] temp = new byte[data.Length];
          rng.GetBytes(temp);
          File.WriteAllText($"piece{i+1}.txt", Convert.ToBase64String(temp));
          data = XOR(data, temp);
        }
      }
      File.WriteAllText($"piece{n}.txt", Convert.ToBase64String(data));
    }
  }
}
