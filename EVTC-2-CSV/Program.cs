using EVTC_2_CSV.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
                WriteCSV();
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

        private static void PromptQuit()
        {
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void WriteCSV()
        {
            string fileName = "./" + (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + ".csv";
            using (StreamWriter f = new StreamWriter(fileName))
            {
                WriteCSVHeader(f);

                List<string> errors = new List<string>();
                for (int i = 0; i < _logs.Length; i++)
                {
                    Console.Write("\rParsing " + (i + 1) + " of " + _logs.Length + " logs...");
                    if (_parser.Parse(_logs[i]))
                    {
                        f.Write(new Converter(_parser).CSV());
                    }
                    else
                    {
                        errors.Add(_logs[i]);
                    }
                }

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Output: " + fileName + Environment.NewLine);

                if (errors.Count > 0)
                {
                    Console.WriteLine("Failed to parse " + errors.Count + " file(s)...");
                    Console.WriteLine("Output: error.log...");
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

        private static void WriteCSVHeader(StreamWriter f)
        {
            if (Properties.Settings.Default.WriteHeaders)
            {
                StringBuilder header = new StringBuilder();
                if (Properties.Settings.Default.WriteARC)
                {
                    header.Append("Arc,");
                }
                if (Properties.Settings.Default.WriteDate)
                {
                    header.Append("Date,");
                }
                if (Properties.Settings.Default.WriteBuild)
                {
                    header.Append("Build,");
                }
                if (Properties.Settings.Default.WriteSpecies)
                {
                    header.Append("Species,");
                }
                if (Properties.Settings.Default.WriteTarget)
                {
                    header.Append("Target,");
                }
                if (Properties.Settings.Default.WriteTime)
                {
                    header.Append("Time,");
                }
                if (Properties.Settings.Default.WriteAccount)
                {
                    header.Append("Account,");
                }
                if (Properties.Settings.Default.WriteCharacter)
                {
                    header.Append("Character,");
                }
                if (Properties.Settings.Default.WriteProfession)
                {
                    header.Append("Profession,");
                }
                if (Properties.Settings.Default.WriteGear)
                {
                    header.Append("Gear,");
                }
                if (Properties.Settings.Default.WriteDPS)
                {
                    header.Append("DPS,");
                }
                if (Properties.Settings.Default.WriteCritical)
                {
                    header.Append("Critical,");
                }
                if (Properties.Settings.Default.WriteScholar)
                {
                    header.Append("Scholar,");
                }
                if (Properties.Settings.Default.WriteFlank)
                {
                    header.Append("Flank,");
                }
                if (Properties.Settings.Default.WriteMoving)
                {
                    header.Append("Moving,");
                }
                if (Properties.Settings.Default.WriteDodge)
                {
                    header.Append("Dodge,");
                }
                if (Properties.Settings.Default.WriteSwap)
                {
                    header.Append("Swap,");
                }
                if (Properties.Settings.Default.WriteResurrect)
                {
                    header.Append("Resurrect,");
                }
                if (Properties.Settings.Default.WriteDowned)
                {
                    header.Append("Downed,");
                }
                if (Properties.Settings.Default.WriteDied)
                {
                    header.Append("Died,");
                }
                header.Remove(header.Length - 1, 1);
                f.WriteLine(header.ToString());
            }
        }
    }
}
