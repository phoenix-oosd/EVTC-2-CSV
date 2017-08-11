using EVTC_2_CSV.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace EVTC_2_CSV
{
    public class Program
    {
        private static Parser _parser = new Parser();

        public static void Main(string[] args)
        {
            PromptBegin();
            WriteCSV();
            PromptQuit();
        }

        private static void PromptBegin()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        private static void PromptQuit()
        {
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void WriteCSV()
        {
            // Create CSV using Epoch Timestamp
            string fileName = "./" + (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + ".csv";
            using (StreamWriter f = new StreamWriter(fileName))
            {
                // Load all EVTC in CWD
                string[] logs = Directory.GetFiles("./", "*.evtc*", SearchOption.AllDirectories);
                List<string> failed = new List<string>();

                // Parse all EVTC
                for (int i = 0; i < logs.Length; i++)
                {
                    // Prompt Progress
                    Console.Write("\rParsing " + (i + 1) + " of " + logs.Length + " logs...");

                    // Try Parse
                    bool isSucessful = _parser.Parse(logs[i]);

                    // Write to CSV
                    if (isSucessful)
                    {
                        // Instantiate Converter
                        Converter converter = new Converter(_parser);
                        f.Write(converter.CSV());
                    }
                    else
                    {
                        failed.Add(logs[i]);
                    }
                    break;
                }
                // Prompt completion and CSV location
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Done - CSV saved at " + fileName + Environment.NewLine);

                // Prompt errors
                if (failed.Count > 0)
                {
                    Console.WriteLine("Failed to parse " + failed.Count + " file(s)...");
                    Console.WriteLine("Saved file paths in error.log...");
                    using (StreamWriter e = new StreamWriter("errors.log"))
                    {
                        foreach (string fail in failed)
                        {
                            e.WriteLine(fail);
                        }
                    }
                }
            }
        }
    }
}
