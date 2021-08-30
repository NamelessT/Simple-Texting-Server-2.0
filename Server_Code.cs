using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
/*
 * ===========================================================================================
 * This is the server code for the simple messing program
 * all comms are encrypted and the server uses mulithreading for handling comms
 * up to 16 connections
 * ===========================================================================================
 * WRITEN BY JOE VOISEY
 * APRIL 11 2021
 * ===========================================================================================
 * THIS CANNOT BE USED FOR FINANCIAL GAIN, THIS CODE IS FREE AND PERMITTED FOR PERSONAL USE 
 * ===========================================================================================
 */
namespace CommandServer
{
    class user
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    //This is the class used for loging in
    class msg
    {
        public string username { get; set; }
        public string message { get; set; }
    }
    //This is the class that is used for message packets
    class Program
    {
        private static byte key_num = 0;
        //this variable is used for logging the current amount of users on the server

        private static bool[] CLIENT_KEY_ACTIVE = new bool[16];
        private static IPAddress SERVER_IP;
        private static int SERVER_PORT;
        //Server info that is passed to the program thru arguments

        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        //Generates encryption keypair
 
        private static Thread[] WRT_THREADS = new Thread[16];
        //Thread array for listening to oncoming messages
        private static Thread writ_thread = new Thread(new ThreadStart(WRIT));
        //Thread for writing to the clients
        private static Socket[] SOC_ARRAY = new Socket[16];
        //WIN_SOCK array for the client connections
        private static bool[] ACTIVE = new bool[16];
        //for handling which threads are running or terminated(so not all 16 threads are firing at once)
        private static bool[] RUNNIN = new bool[16];
        //helps with the thread management
        private static string[] username_array = new string[16];
        //array of current usernames that are logged in

        private static RSAParameters PubKey = csp.ExportParameters(false);
        private static RSAParameters Privkey = csp.ExportParameters(true);
        private static RSAParameters[] CLIENTKEY_ARRAY = new RSAParameters[16];
        //^^ Server Keys for encryption

        private static bool[] THREAD_ACT = new bool[16];
        //array to tell which threads are active
        private static XmlSerializer xs = new XmlSerializer(typeof(RSAParameters));
        //xml serializer for sending the key over to the client
        private static byte[] key_bytes = new byte[1024];
        //byte array for the public key
        static Program()
        {
            StringWriter sr = new StringWriter();
            xs.Serialize(sr, PubKey);
            string key = sr.ToString();
            key = key.Replace("utf-16", "ASCII");
            key_bytes = Encoding.ASCII.GetBytes(key);
            //parses the pub_key as a byte array for sending across the wire

        }
        private static TcpListener server = new TcpListener(SERVER_IP, SERVER_PORT);
        //TcpListner listening for connection attempts
        private static void Main(string[] args)
        {
            try
            {
                string arg = args[0];
                string[] arg_s = arg.Split(":");
                SERVER_IP = IPAddress.Parse(arg_s[0]);
                SERVER_PORT = int.Parse(arg_s[1]);
                //parses the server ip and port

                Console.WriteLine("INIT");
                for (int i = 0; i <= RUNNIN.Length - 1; i++)
                {
                    RUNNIN[i] = false;
                }
                for (int i = 0; i <= CLIENT_KEY_ACTIVE.Length - 1; i++)
                {
                    CLIENT_KEY_ACTIVE[i] = false;
                }
                for (int i = 0; i <= THREAD_ACT.Length - 1; i++)
                {
                    THREAD_ACT[i] = false;
                }
                WRT_THREADS[0] = new Thread(() => LISTN(0, SOC_ARRAY[0]));
                WRT_THREADS[1] = new Thread(() => LISTN(1, SOC_ARRAY[1]));
                WRT_THREADS[2] = new Thread(() => LISTN(2, SOC_ARRAY[2]));
                WRT_THREADS[3] = new Thread(() => LISTN(3, SOC_ARRAY[3]));
                WRT_THREADS[4] = new Thread(() => LISTN(4, SOC_ARRAY[4]));
                WRT_THREADS[5] = new Thread(() => LISTN(5, SOC_ARRAY[5]));
                WRT_THREADS[6] = new Thread(() => LISTN(6, SOC_ARRAY[6]));
                WRT_THREADS[7] = new Thread(() => LISTN(7, SOC_ARRAY[7]));
                WRT_THREADS[8] = new Thread(() => LISTN(8, SOC_ARRAY[8]));
                WRT_THREADS[9] = new Thread(() => LISTN(9, SOC_ARRAY[9]));
                WRT_THREADS[10] = new Thread(() => LISTN(10, SOC_ARRAY[10]));
                WRT_THREADS[11] = new Thread(() => LISTN(11, SOC_ARRAY[11]));
                WRT_THREADS[12] = new Thread(() => LISTN(12, SOC_ARRAY[12]));
                WRT_THREADS[13] = new Thread(() => LISTN(13, SOC_ARRAY[13]));
                WRT_THREADS[14] = new Thread(() => LISTN(14, SOC_ARRAY[14]));
                WRT_THREADS[15] = new Thread(() => LISTN(15, SOC_ARRAY[15]));
                //sets all the threads that listen for incoming messages from client connections
                               
                Thread CONN = new Thread(new ThreadStart(CONN_HANDL));
                //^^ Thread Handles connections
                Thread HAND = new Thread(new ThreadStart(ACTIVE_HANDL));
                //^^ Thread Handles the threads that handle incoming messages


                CONN.Start();

                HAND.Start();

                writ_thread.Start();
                //starts the threads
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
        //Main function primarly assigns the threads and global variables
        private static void ACTIVE_HANDL()
        {
            try
            {


                while (true)
                {
                    if (ACTIVE[0] && RUNNIN[0] == false)
                    {
                        WRT_THREADS[0].Start();
                        RUNNIN[0] = true;
                    }
                    if (ACTIVE[1] && RUNNIN[1] == false)
                    {
                        WRT_THREADS[1].Start();
                        RUNNIN[1] = true;
                    }
                    if (ACTIVE[2] && RUNNIN[2] == false)
                    {
                        WRT_THREADS[2].Start();
                        RUNNIN[2] = true;
                    }
                    if (ACTIVE[3] && RUNNIN[3] == false)
                    {
                        WRT_THREADS[3].Start();
                        RUNNIN[3] = true;
                    }
                    if (ACTIVE[4] && RUNNIN[4] == false)
                    {
                        WRT_THREADS[4].Start();
                        RUNNIN[4] = true;
                    }
                    if (ACTIVE[5] && RUNNIN[5] == false)
                    {
                        WRT_THREADS[5].Start();
                        RUNNIN[5] = true;
                    }
                    if (ACTIVE[6] && RUNNIN[6] == false)
                    {
                        WRT_THREADS[6].Start();
                        RUNNIN[6] = true;
                    }
                    if (ACTIVE[7] && RUNNIN[7] == false)
                    {
                        WRT_THREADS[7].Start();
                        RUNNIN[7] = true;
                    }
                    if (ACTIVE[8] && RUNNIN[8] == false)
                    {
                        WRT_THREADS[8].Start();
                        RUNNIN[8] = true;
                    }
                    if (ACTIVE[9] && RUNNIN[9] == false)
                    {
                        WRT_THREADS[9].Start();
                        RUNNIN[9] = true;
                    }
                    if (ACTIVE[10] && RUNNIN[10] == false)
                    {
                        WRT_THREADS[10].Start();
                        RUNNIN[10] = true;
                    }
                    if (ACTIVE[11] && RUNNIN[11] == false)
                    {
                        WRT_THREADS[11].Start();
                        RUNNIN[11] = true;
                    }
                    if (ACTIVE[12] && RUNNIN[12] == false)
                    {
                        WRT_THREADS[12].Start();
                        RUNNIN[12] = true;
                    }
                    if (ACTIVE[13] && RUNNIN[13] == false)
                    {
                        WRT_THREADS[13].Start();
                        RUNNIN[13] = true;
                    }
                    if (ACTIVE[14] && RUNNIN[14] == false)
                    {
                        WRT_THREADS[14].Start();
                        RUNNIN[14] = true;
                    }
                    if (ACTIVE[15] && RUNNIN[15] == false)
                    {
                        WRT_THREADS[15].Start();
                        RUNNIN[15] = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
        //^^ Handles the active connections(saves threads)
        private static void WRIT()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "//quit")
                {
                    Environment.Exit(0);
                }
                else
                {
                    msg message = new msg();
                    message.username = "__SERVER__";
                    message.message = msg;
                    STR(17, message);
                }
            }
        }
        //function for writing from the server to the client connections
        private static void LISTN(byte swich, Socket s)
        {
            while (true)
            {
                try
                {
                    byte[] msg_bytes = new byte[256];
                    s.Receive(msg_bytes);
                    csp.ImportParameters(Privkey);
                    msg_bytes = csp.Decrypt(msg_bytes, false);
                    string msg = System.Text.Encoding.ASCII.GetString(msg_bytes);
                    msg data_load = JsonConvert.DeserializeObject<msg>(msg);
                    EndPoint s_ep = s.RemoteEndPoint;
                    Console.WriteLine(data_load.username + " Said:\n" + data_load.message + "\n" + "________________________________________");
                    STR(swich, data_load);
                }
                catch (Exception e)
                {
                    DISCONNT(swich);
                }
            }
        }
        //Function for listening on the sockets
        private static void DISCONNT(byte swich)
        {
            try
            {
                if(key_num == 0)
                {
                    ACTIVE[swich] = false;
                    RUNNIN[swich] = false;
                    CLIENT_KEY_ACTIVE[swich] = false;
                }
                else
                {
                    key_num--;
                    ACTIVE[swich] = false;
                    RUNNIN[swich] = false;
                    CLIENT_KEY_ACTIVE[swich] = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
        //Function for handling disconnects(so the program doesnt crash on a disconnect)
        private static void STR(byte swich, msg message)
        {
            try
            {
                byte[] count = new byte[1];
                for (int i = 0; i <= 15; i++)
                {

                        if (CLIENT_KEY_ACTIVE[i])
                        {

                            try
                            {
                                string jsondata = JsonConvert.SerializeObject(message);
                                byte[] json_bytes = System.Text.Encoding.ASCII.GetBytes(jsondata);
                                csp.ImportParameters(CLIENTKEY_ARRAY[i]);
                                byte[] msg = csp.Encrypt(json_bytes, false);
                                Socket s = SOC_ARRAY[i];
                                s.Send(msg);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                Console.ReadLine();
                            }
                        }
                        if (CLIENT_KEY_ACTIVE[i] == false)
                        {
                            i++;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
        //function for sorting thru client connections streaming back a message
        private static void CONN_HANDL()
        {
            try
            {
                while (true)
                {
                    server.Start();
                    Socket s = server.AcceptSocket();
                    RSAParameters ClientKey;
                    byte[] ClientKeyBytes = new byte[1024];
                    s.Send(key_bytes);
                    s.Receive(ClientKeyBytes);
                    using (Stream str = new MemoryStream(ClientKeyBytes))
                    {
                        ClientKey = (RSAParameters)xs.Deserialize(str);
                    }
                    byte[] usr_bytes = new byte[256];
                    try
                    {
                        s.Receive(usr_bytes);
                        csp.ImportParameters(Privkey);
                        File.WriteAllBytes("C:/Server/Logs/debug.txt", usr_bytes);
                        byte[] raw_data = csp.Decrypt(usr_bytes, false);
                        File.WriteAllBytes("C:/Server/Logs/d_debug.txt", raw_data);
                        string msg = System.Text.Encoding.ASCII.GetString(raw_data);
                        user user_login = JsonConvert.DeserializeObject<user>(msg);
                        string jsonsource = File.ReadAllText("C:/Server/Users/" + user_login.username + ".json");
                        string jsondata = JsonConvert.DeserializeObject<string>(jsonsource);


                        //File.WriteAllText("C:/Server/Logs/hashed_pwd_on_server.txt",user_login.password);
                        byte[] guess_hash = System.Text.Encoding.ASCII.GetBytes(user_login.password);
                        byte[] password_hash = System.Text.Encoding.ASCII.GetBytes(jsondata);
                        //File.WriteAllBytes("C:/Server/Logs/guess_hash.txt",guess_hash);
                        //File.WriteAllBytes("C:/Server/Logs/password_hash.txt",password_hash);
                        bool checksum = IsMatch(guess_hash, password_hash);
                        username_array[key_num] = user_login.username;
                        CLIENTKEY_ARRAY[key_num] = ClientKey;
                        if (checksum)
                        {
                            Console.WriteLine("Checksum Success");
                            csp.ImportParameters(CLIENTKEY_ARRAY[key_num]);
                            byte[] info = csp.Encrypt(System.Text.Encoding.ASCII.GetBytes("auth"), false);
                            s.Send(info);
                            ACTIVE[key_num] = true;
                            SOC_ARRAY[key_num] = s;
                            CLIENT_KEY_ACTIVE[key_num] = true;
                            key_num++;
                        }
                        if (!checksum)
                        {
                            Console.WriteLine("Checksum Failure");
                            s.Close();
                        }
                    }
                    catch(SocketException s_error)
                    {
                        DISCONNT(key_num);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
        //Function for handling connections, key exchanging and loginning in
        public static bool IsMatch(byte[] guess_hash, byte[] password_hash)
        {
            try
            {
                bool[] newt = new bool[32];
                for (byte i = 0; i <= 31; i++)
                {
                    if (guess_hash[i] == password_hash[i])
                    {
                        newt[i] = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                byte checksum = 0;
                for (byte i = 0; i <= 31; i++)
                {
                    if (newt[i] == true)
                    {
                        checksum += 1;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (checksum == 32)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                string problem = e.ToString();
                Console.WriteLine(problem);
            }
            return false;
        }
    }
    //function for matching byte arrays(wrote this like 1 1/2 years ago but is works really well)
}
/*
 * =================================================================================================================
 * NOTES FOR LINUX!
 * =================================================================================================================
 * The server code has not been experimented on Android, do not recommend  
 * this code has been used with Linux using MONO C# in terminal. The System.Security.Cryptography RSA Library is slow on Linux 
 * so Windows is recommended, for exiting the application thru terminal type "//quit" in the writeline
 *==================================================================================================================
 */
