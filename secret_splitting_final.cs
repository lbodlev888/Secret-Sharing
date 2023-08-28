using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Secret
{
  internal class Program
  {
    static string input_file = "", out_file = "";
    static bool outFile = false, binary = true, split = true, default_names = true;
    static uint nr_pieces = 0;

    static bool InformationGathering(string[] args)
    {
      for(int i = 0; i < args.Length; i++)
      {
        if(args[i] == "-in" && i+1 < args.Length) {
          input_file = args[i+1];
          i++;
        }
        else if(args[i] == "-out" && i+1 < args.Length) {
          out_file = args[i+1];
          outFile = true;
          i++;
        }
        else if(args[i] == "-n" && i+1 < args.Length) {
          bool result = uint.TryParse(args[i+1], out nr_pieces);
          if(!result) {
            Console.WriteLine("Invalid usage of command line\nUse -h for help");
            return false;
          }
          i++;
        }
        else if(args[i] == "-no_default") default_names = false;
        else if(args[i] == "-armor") binary = false;
        else if(args[i] == "-d") split = false;
        else {
            Console.WriteLine("Invalid usage of command line\nUse -h for help");
            return false;
        }
      }
      return true;
    }

    public static byte[] XOR(byte[] msg, byte[] key)
    {
      if(msg.Length != key.Length)
        return Encoding.UTF8.GetBytes("error");
      byte[] data = new byte[msg.Length];
      for(int i = 0; i < msg.Length; i++)
        data[i] = (byte)(msg[i] ^ key[i]);
      return data;
    }

    public static void Encrypt()
    {
      byte[] data = File.ReadAllBytes(input_file);
      using(var rng = new RNGCryptoServiceProvider())
      {
        for(int i = 0; i < nr_pieces-1; i++)
        {
          byte[] temp = new byte[data.Length];
          rng.GetBytes(temp);
          if(outFile) {
            if(binary) File.WriteAllBytes($"{out_file}{i+1}.bin", temp);
            else File.WriteAllText($"{out_file}{i+1}.txt", Convert.ToBase64String(temp));
          }
          else Console.WriteLine(Convert.ToBase64String(temp));
          data = XOR(data, temp);
        }
      }
      if(outFile) {
        if(binary) File.WriteAllBytes($"{out_file}{nr_pieces}.bin", data);
        else File.WriteAllText($"{out_file}{nr_pieces}.txt", Convert.ToBase64String(data));
      }
      else Console.WriteLine(Convert.ToBase64String(data));
    }

    public static void Decrypt()
    {
      string[] pieces = new string[nr_pieces];
      if(default_names) {
        if(binary) {
          for(int i = 0; i < nr_pieces; i++)
            pieces[i] = Convert.ToBase64String(File.ReadAllBytes($"piece{i+1}.bin"));
        }
        else {
          for(int i = 0; i < nr_pieces; i++)
            pieces[i] = File.ReadAllText($"piece{i+1}.txt");
        }
        byte[] data = Convert.FromBase64String(pieces[0]);
        for(int i = 1; i < nr_pieces; i++)
        {
          byte[] temp = XOR(data, Convert.FromBase64String(pieces[i]));
          if(Encoding.UTF8.GetString(temp) == "error") {
            Console.WriteLine("error");
            return;
          }
          data = temp;
        }
        if(outFile) File.WriteAllBytes(out_file, data);
        else Console.WriteLine(Encoding.UTF8.GetString(data));
      }
      else {
        if(binary) {
          for(int i = 0; i < nr_pieces; i++)
          {
            Console.Write($"File {(i+1).ToString()}: ");
            string tempFileName = Console.ReadLine();
            pieces[i] = Convert.ToBase64String(File.ReadAllBytes(tempFileName));
          }
        }
        else {
          for(int i = 0; i < nr_pieces; i++)
          {
            Console.Write($"File {(i+1).ToString()}: ");
            string tempFileName = Console.ReadLine();
            pieces[i] = File.ReadAllText(tempFileName);
          }
        }
        byte[] data = Convert.FromBase64String(pieces[0]);
        for(int i = 1; i < nr_pieces; i++)
        {
          byte[] temp = XOR(data, Convert.FromBase64String(pieces[i]));
          if(Encoding.UTF8.GetString(temp) == "error") {
            Console.WriteLine("error");
            return;
          }
          data = temp;
        }
        if(outFile) File.WriteAllBytes(out_file, data);
        else Console.WriteLine(Encoding.UTF8.GetString(data));
      }
    }

    static void Main(string[] args)
    {
      if(args[0] == "-h") {
        Console.WriteLine("help:\n");
        Console.WriteLine("By default it splits file in binary format to STDOUT");
        Console.WriteLine("-in              Input file");
        Console.WriteLine("-out             Output file");
        Console.WriteLine("-n               Number of pieces");
        Console.WriteLine("-d               Reconstruct file");
        Console.WriteLine("-no_default      When reconstruct, use if file doesn't have the default name");
        Console.WriteLine("-armor           ASCII output");
        return;
      }
      if(!InformationGathering(args)) return;
      if(split) Encrypt();
      else Decrypt();
    }
  }
}
