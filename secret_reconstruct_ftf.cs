using System;
using System.IO;
using System.Text;

namespace Reconstruct
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
      Console.Write("How many pieces: ");
      int n = int.Parse(Console.ReadLine());
      string[] pieces = new string[n];
      Console.Write("Do the files have the default names?[y/n]"); char default_name = char.Parse(Console.ReadLine());
      if(default_name == 'y') {
        for(int i = 0; i < n; i++) pieces[i] = File.ReadAllText($"piece{i+1}.txt");
      }
      else if(default_name == 'n') {
        for(int i = 0; i < n; i++)
        {
          Console.Write($"File {(i+1).ToString()}: ");
          string tempFileName = Console.ReadLine();
          pieces[i] = File.ReadAllText(tempFileName);
        }
      }
      else {
        Console.WriteLine("Invalid Option");
        return;
      }
      
      byte[] data = Convert.FromBase64String(pieces[0]);
      for(int i = 1; i < n; i++)
      {
        byte[] temp = XOR(data, Convert.FromBase64String(pieces[i]));
        if(Encoding.UTF8.GetString(temp) == "error") {
          Console.WriteLine("error");
          return;
        }
        data = temp;
      }
      Console.Write("File Name: "); string file_name = Console.ReadLine();
      File.WriteAllBytes(file_name, data);
    }
  }
}
