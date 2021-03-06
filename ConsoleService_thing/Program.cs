using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleService_thing
{
    class Program
    {
        private static Process process;
        private static bool running = true;
        static void Main(string[] args)
        {
            Console.WriteLine("starting server...");
            Console.CancelKeyPress += CurrentDomain_ProcessExit;
            process = new Process();
            process.StartInfo = new ProcessStartInfo(
                    "cmd",
                    @"/c java -Xms12G -Xmx12G -jar server.jar nogui"
                );
            process.StartInfo.RedirectStandardError
                = process.StartInfo.RedirectStandardInput
                = process.StartInfo.RedirectStandardOutput
                = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            void readStreamOut(StreamReader reader)
            {
                Task.Run(() =>
                {
                    while (running)
                    {
                        Thread.Sleep(10);
                        while (!reader.EndOfStream)
                        {
                            Console.Write(
                                (char)reader.Read()
                            );
                        }
                    }
                    Console.WriteLine("reader shutting down");
                });
            }
            readStreamOut(process.StandardOutput);
            readStreamOut(process.StandardError);
            process.WaitForExit();
            Console.WriteLine("reached end of process");
            running = false;
        }

        static void CurrentDomain_ProcessExit(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
