using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace User_Gen
{
    class user
    {
        public byte[] one = new byte[32];   
    }
    class index
    {
        public string uname = "";
        public byte indx = 0;
    }
    class Program
    {
        private static byte user_count = 0;
        private static HashAlgorithm hash = SHA256.Create();

        private static void Main(string[] args)
        {

            string usrname = args[0];
            string passwd = args[1];
            byte[] password = System.Text.Encoding.ASCII.GetBytes(passwd);
            byte[] hashed_bytes = hash.ComputeHash(password);
            string jsondata = JsonConvert.SerializeObject(hashed_bytes);
            File.WriteAllText("C:/Server/Users/" + usrname + ".json",jsondata);
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
