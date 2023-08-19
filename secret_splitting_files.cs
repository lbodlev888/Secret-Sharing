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
      Console.WriteLine();
      byte[] data = File.ReadAllBytes(file_path);
      string[] pieces = new string[n-1];
      using(var rng = new RNGCryptoServiceProvider())
      {
        for(int i = 0; i < n-1; i++)
        {
          byte[] temp = new byte[data.Length];
          rng.GetBytes(temp);
          pieces[i] = Convert.ToBase64String(temp);
          Console.WriteLine(pieces[i]);
        }
      }
      data = XOR(data, Convert.FromBase64String(pieces[0]));
      for(int i = 1; i < n-1; i++)
        data = XOR(data, Convert.FromBase64String(pieces[i]));
      Console.WriteLine(Convert.ToBase64String(data));
    }
  }
}
