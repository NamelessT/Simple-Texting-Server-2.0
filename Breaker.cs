using System;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

class file_data
{
	public string filename;
	public int filesize;
}
class Program
{
	private static file_data file_dat = new file_data();
	static void Main(string[] args)
	{
		string filename = args[0];
		file_dat.filename = filename;
		int j = 0;
		byte[] file = File.ReadAllBytes(filename);
		file_dat.filesize = file.Length;
		byte[] bytes = new byte[247];
		while(true)
		{
			int i;
			for(i = 0; i <= 246; i++)
			{
				try
				{
					bytes[i] = file[j * 247 + i];
					if(i == 246)
					{
						j++;
					}
				}
				catch(IndexOutOfRangeException e)
				{
					Console.WriteLine("Done!");
					return;
				}
			}
			string jsoninfo = JsonConvert.SerializeObject(file_dat);
			File.WriteAllText("file/fileinfo.json",jsoninfo);
			File.WriteAllBytes("file/"+j+".bin",bytes);
		}
	}
}
