using EVTC_2_CSV.Model;
using EVTC_2_CSV.Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EVTC_2_CSV
{
    public class Program
    {
        #region Members
        private static string[] _logs;
        private static Parser _parser = new Parser();
        #endregion

        #region Main
        public static void Main(string[] args)
        {
            PromptBegin();
            if (LoadEVTC())
            {
                if (ConfigurationHasFields())
                {
                    string fName = "./" + (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + ".csv";
                    using (StreamWriter f = new StreamWriter(fName))
                    {
                        if (Properties.Settings.Default.WriteHeader)
                        {
                            WriteCSVHeader(f);
                        }
                        List<string> err = WriteCSVRows(f);
                        Console.WriteLine("CSV file written into " + fName + Environment.NewLine);
                        if (err.Count > 0)
                        {
                            WriteErrors(err);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Configuration has no fields enabled..." + Environment.NewLine);
                }
            }
            PromptQuit();
        }
        #endregion

        #region Private Methods
        private static void PromptBegin()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        private static bool LoadEVTC()
        {
            _logs = Directory.EnumerateFiles("./", "*", SearchOption.AllDirectories).Where(f => f.EndsWith(".evtc") || f.EndsWith(".evtc.zip")).ToArray();
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
            foreach (string e in Enum.GetNames(typeof(Field)))
            {
                if ((bool)Properties.Settings.Default[e])
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
            StringBuilder sb = new StringBuilder();
            foreach (string e in Enum.GetNames(typeof(Field)))
            {
                if ((bool)Properties.Settings.Default[e])
                {
                    sb.Append(e + ",");
                }
            }
            sb.Remove(sb.Length - 1, 1);
            f.WriteLine(sb.ToString());
        }

        private static List<string> WriteCSVRows(StreamWriter f)
        {
            List<string> err = new List<string>();
            for (int i = 0; i < _logs.Length; i++)
            {
                Console.Write("\rParsing " + (i + 1) + " of " + _logs.Length + " logs...");
                if (_parser.Parse(_logs[i]))
                {
                    f.Write(new Converter(_parser).ToCSV());
                }
                else
                {
                    err.Add(_logs[i]);
                }
                //break;
            }
            Console.WriteLine(Environment.NewLine);
            return err;
        }

        private static void WriteErrors(List<string> err)
        {
            Console.WriteLine("Failed to parse " + err.Count + " file(s)...");
            Console.WriteLine("Problem file paths written into error.log...");
            using (StreamWriter f = new StreamWriter("errors.log"))
            {
                foreach (string e in err)
                {
                    f.WriteLine(e);
                }
            }
        }
        #endregion
    }
}
