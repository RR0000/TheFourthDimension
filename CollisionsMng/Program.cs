﻿using CommonFiles;
using MarioKart.MK7;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CollisionsMng
{
    class Program
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;        
        const int STD_INPUT_HANDLE = -10;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        static void Main(string[] args)
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
            uint consoleMode;
            GetConsoleMode(consoleHandle, out consoleMode);
            consoleMode &= ~ENABLE_QUICK_EDIT;
            SetConsoleMode(consoleHandle, consoleMode);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "CollisionsMng";
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("-----3D Land collision importer by exelix11-----");
            Console.WriteLine("------------------Version 1.2-------------------");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Thanks Gericom for Every File Explorer's KCL importer");
            Console.WriteLine("");
            try
            {
                if (args.Length == 0 || args.Length > 3) { WriteUsage(); Console.ForegroundColor = ConsoleColor.White; return; }
                string FileName = args[0];
                if (args.Length == 1)
                {
                    if (Path.GetExtension(FileName).ToLower() == ".obj")
                    {
                        MakeKCLandPA(FileName, false);
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    else if (Path.GetExtension(FileName).ToLower() == ".kcl")
                    {
                        KCL k = new KCL(File.ReadAllBytes(FileName));
                        k.convert(0, FileName + ".obj");
                        Console.WriteLine("DONE !");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    else if (Path.GetExtension(FileName).ToLower() == ".pa")
                    {
                        Console.WriteLine(Pa_format.LoadFile(File.ReadAllBytes(FileName)).ToString());
                        Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    else
                    {
                        Console.WriteLine("extension " + Path.GetExtension(FileName).ToLower() + " not supported");
                        WriteUsage();
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
                else if (args[1].ToLower() == "zero")
                {
                    MakeKCLandPA(FileName, true);
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                else if (args[1].ToLower() == "unknown")
                {
                    Console.WriteLine(Pa_format.LoadFile(File.ReadAllBytes(FileName)).ToString(true));
                    Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                else { WriteUsage(); Console.ForegroundColor = ConsoleColor.White; return; }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error: ");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("");
                Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void MakeKCLandPA(string input, bool zero)
        {
            try
            {
                Console.WriteLine("Creating KCL...");
                KCL k = new KCL();
                List<String> Materials = k.CreateFromFile(File.ReadAllBytes(input));
                Console.WriteLine("\r\nWriting KCL...");
                File.WriteAllBytes(input + ".kcl", k.Write());
                Console.WriteLine("Creating PA...");
                Pa_format pa = new Pa_format(true);
                for (int i = 0; i < Materials.Count; i++)
                {
                    if (zero) pa.entries.Add(0);
                    else
                    {
                        Console.WriteLine("-Data for material : " + Materials[i]);
                        Console.Write(" |Enter value for Sound_code [0]: ");
                        string tmp = Console.ReadLine();
                        uint SoundCode;
                        if (tmp.Trim() == "") SoundCode = 0; else SoundCode = uint.Parse(tmp);
                        Console.Write(" |Enter value for Floor_code [0]: ");
                        tmp = Console.ReadLine();
                        uint FloorCode;
                        if (tmp.Trim() == "") FloorCode = 0; else FloorCode = uint.Parse(tmp);
                        Console.Write(" |Enter value for Wall_code [0]: ");
                        tmp = Console.ReadLine();
                        uint WallCode;
                        if (tmp.Trim() == "") WallCode = 0; else WallCode = uint.Parse(tmp);
                        uint Unknown = 0;
                        uint CameraThrought = 0;
                        SoundCode = SoundCode << pa.Fields[0].Shift;
                        FloorCode = FloorCode << pa.Fields[1].Shift;
                        Unknown = Unknown << pa.Fields[2].Shift;
                        WallCode = WallCode << pa.Fields[3].Shift;
                        CameraThrought = CameraThrought << pa.Fields[4].Shift;

                        SoundCode &= pa.Fields[0].Bitmask;
                        FloorCode &= pa.Fields[1].Bitmask;
                        Unknown &= pa.Fields[2].Bitmask;
                        WallCode &= pa.Fields[3].Bitmask;
                        CameraThrought &= pa.Fields[4].Bitmask;

                        pa.entries.Add(SoundCode + FloorCode + Unknown + WallCode + CameraThrought);
                        Console.WriteLine("");
                    }
                }
                Console.WriteLine("Writing PA...");
                File.WriteAllBytes(input + ".pa", pa.MakeFile());
                Console.WriteLine("DONE !");
                Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error: ");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("");
                Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void WriteUsage()
        {
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Usage:");
            Console.WriteLine("CollisionsMng *obj file path* [-zero]: ");
            Console.WriteLine("             Converts an obj to Kcl and Pa add -zero parametrer to set every flag to 0");
            Console.WriteLine("CollisionsMng *kcl file path* :");
            Console.WriteLine("             Converts a kcl to obj");
            Console.WriteLine("CollisionsMng *pa file path* :");
            Console.WriteLine("             Displays materials flags from a Pa file");
            Console.WriteLine("Parametrers are not case sensitive");
            Console.WriteLine("------------------------------------------------");
            Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}