using EVTC_2_CSV.Model;
using EVTC_2_CSV.Model.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace EVTC_2_CSV
{
    public class Program
    {
        private static string[] _logs;
        private static Parser _parser = new Parser();

        public static void Main(string[] args)
        {
            PromptBegin();
            if (LoadEVTC())
            {
                if (!ConfigurationHasFields())
                {
                    Console.WriteLine("Configuration has no fields enabled..." + Environment.NewLine);
                }
                else
                {
                    WriteCSVRows();
                }
            }
            PromptQuit();
        }

        private static void PromptBegin()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        private static bool LoadEVTC()
        {
            _logs = Directory.GetFiles("./", "*.evtc*", SearchOption.AllDirectories);
            if (_logs.Length > 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine("No .evtc files found in this directory..." + Environment.NewLine);
                return false;
            }
        }

        private static bool ConfigurationHasFields()
        {
            foreach (string field in Enum.GetNames(typeof(Field)))
            {
                if (Properties.Settings.Default[field].ToString() == "True")
                {
                    return true;
                }
            }
            return false;
        }

        private static void PromptQuit()
        {
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void WriteCSVHeader(StreamWriter f)
        {
            Console.WriteLine("Writing header...");
            StringBuilder header = new StringBuilder();
            foreach (string field in Enum.GetNames(typeof(Field)))
            {
                if (Properties.Settings.Default[field].ToString() == "True")
                {
                    header.Append(field + ",");
                }
            }
            header.Remove(header.Length - 1, 1);
            f.WriteLine(header.ToString());
        }

        private static void WriteCSVRows()
        {
            string fileName = "./" + (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + ".csv";
            using (StreamWriter f = new StreamWriter(fileName))
            {
                if (Properties.Settings.Default.WriteHeader)
                {
                    WriteCSVHeader(f);
                }
                List<string> errors = new List<string>();
                for (int i = 0; i < _logs.Length; i++)
                {
                    Console.Write("\rParsing " + (i + 1) + " of " + _logs.Length + " logs...");
                    if (_parser.Parse(_logs[i]))
                    {
                        f.Write(new Converter(_parser).ToCSV());
                    }
                    else
                    {
                        errors.Add(_logs[i]);
                    }
                }
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("CSV file written into " + fileName + Environment.NewLine);
                WriteErrors(errors);
            }
        }

        private static void WriteErrors(List<string> errors)
        {
            if (errors.Count > 0)
            {
                Console.WriteLine("Failed to parse " + errors.Count + " file(s)...");
                Console.WriteLine("Problem file paths written into error.log...");
                using (StreamWriter ew = new StreamWriter("errors.log"))
                {
                    foreach (string e in errors)
                    {
                        ew.WriteLine(e);
                    }
                }
            }
        }
    }
}
