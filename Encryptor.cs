using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.json;
using System.Threading;

class file_data
{
    string filename;
    int filesize;
    string file_keypair;
}

class Program
{
    private static Thread[] THR = new Thread[20];

    private static RSAParameters privkey;
    private static RSAParameters pubkey;

    private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

    private static file_data file_dat = new file_data();

    static Program()
    {
        string jsondat = File.ReadAllText("file/fileinfo.json");
        file_dat = JsonConvert.DeserializeObject<file_dat>(jsondat);

        privkey = csp.ExportParameters(true);
        pubkey = csp.ExportParameters(false);

        THR[0] = new Thread(() => ENC(1));
        THR[1] = new Thread(() => ENC(2));
        THR[2] = new Thread(() => ENC(3));
        THR[3] = new Thread(() => ENC(4));
        THR[4] = new Thread(() => ENC(5));
        THR[5] = new Thread(() => ENC(6));
        THR[6] = new Thread(() => ENC(7));
        THR[7] = new Thread(() => ENC(8));
        THR[8] = new Thread(() => ENC(9));
        THR[9] = new Thread(() => ENC(10));
        THR[10] = new Thread(() => ENC(11));
        THR[11] = new Thread(() => ENC(12);
        THR[12] = new Thread(() => ENC(13));
        THR[13] = new Thread(() => ENC(14));
        THR[14] = new Thread(() => ENC(15));
        THR[15] = new Thread(() => ENC(16));
        THR[16] = new Thread(() => ENC(17);
        THR[17] = new Thread(() => ENC(18));
        THR[18] = new Thread(() => ENC(19));
        THR[19] = new Thread(() => ENC(20));
    }

    static void main(string[] args)
    {
        THR[0].Start();
        THR[1].Start();
        THR[2].Start();
        THR[3].Start();
        THR[4].Start();
        THR[5].Start();
        THR[6].Start();
        THR[7].Start();
        THR[8].Start();
        THR[9].Start();
        THR[10].Start();
        THR[11].Start();
        THR[12].Start();
        THR[13].Start();
        THR[14].Start();
        THR[15].Start();
        THR[16].Start();
        THR[17].Start();
        THR[18].Start();
        THR[19].Start();
        
    }
    private static void ENC(int inx)
    {   
        for(int i = 0; i <= file_dat.filesize; i++)
        {
            try
            {
                byte packet = File.ReadAllBytes("file/" + i * inx + ".bin");
                packet = csp.Encrypt(packet, false);
                File.WriteAllBytes("file/" + i * inx + ".bin",packet);
            }
            catch(IndexOutOfRangeException e)
            {
                return;
            }
        }
    }
}
