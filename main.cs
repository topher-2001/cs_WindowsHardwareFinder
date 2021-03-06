using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using System.IO;

namespace sysinfo_blogpost
{
    class Program
    {

        static bool ReleaseBuild = true;

        static void Main(string[] args)
        {
            if(!ReleaseBuild) {
                printAll("Win32_OperatingSystem");
            } else {
                //getPSUinfo();
                
                FileStream stream;
                StreamWriter writer;
                TextWriter oldOut = Console.Out;

                try
                {
                    stream = new FileStream("./Specs.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new StreamWriter(stream);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }

                //Get Info
                getCPUinfo();
                getGPUinfo();
                getRAMinfo();
                getMotherBoardInfo();
                getSystemInfo();
                
                Console.SetOut(writer);

                //Get Info
                getCPUinfo();
                getGPUinfo();
                getRAMinfo();
                getMotherBoardInfo();
                getSystemInfo();

                Console.SetOut(oldOut);
                writer.Close();
                stream.Close();
                Console.WriteLine("\n\nPrinted to 'Specs.txt' ");
                Console.WriteLine("Press ENTER to exit");
                Console.Read();
            }
        }

        static void printAll(string win) {
            ManagementClass myManagementClass = new ManagementClass(win);
            ManagementObjectCollection myManagementCollection = myManagementClass.GetInstances();
            foreach (ManagementObject mo in myManagementCollection)
            {
                foreach (PropertyData prop in mo.Properties)
                {
                    Console.WriteLine("{0}: {1}", prop.Name, prop.Value);
                }
            }
        }

        static void getCPUinfo()
        {
            ManagementObjectSearcher os_searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject mobj in os_searcher.Get())
            {
                Console.WriteLine("======= CPU =======");
                Console.WriteLine("CPU = " + GetValue(mobj, "Name"));
                Console.WriteLine("Thread Count = " + GetValue(mobj, "ThreadCount"));
                Console.WriteLine("Core Count = " + GetValue(mobj, "NumberOfCores"));
            }
        }

        static void getGPUinfo()
        {
            ManagementObjectSearcher os_searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject mobj in os_searcher.Get())
            {
                Console.WriteLine("======= GPU =======");
                Console.WriteLine("GPU = " + GetValue(mobj, "Description"));
                Console.WriteLine("Display = " + GetValue(mobj, "VideoModeDescription"));
                Console.WriteLine("Current Refresh Rate = " + GetValue(mobj, "CurrentRefreshRate"));
                Console.WriteLine("Max Refresh Rate = " + GetValue(mobj, "MaxRefreshRate"));
                Console.WriteLine("Min Refresh Rate = " + GetValue(mobj, "MinRefreshRate"));
                Console.WriteLine("Processor = " + GetValue(mobj, "VideoProcessor"));
            }
        }

        static void getRAMinfo()
        {
            ManagementObjectSearcher os_searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject mobj in os_searcher.Get())
            {
                Console.WriteLine("======= RAM " + GetValue(mobj, "BankLabel") + " =======");
                Console.WriteLine("Manufacturer = " + GetValue(mobj, "Manufacturer"));
                Console.WriteLine("Clock Speed = " + GetValue(mobj, "ConfiguredClockSpeed"));
                Console.WriteLine("Capacity = " + (Double.Parse(GetValue(mobj, "Capacity")) / 1024 / 1024 / 1024)+"GB");
                Console.WriteLine("SMBIOS Type = " + GetValue(mobj, "SMBIOSMemoryType"));
            }
        }

        static void getMotherBoardInfo()
        {
            ManagementObjectSearcher os_searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject mobj in os_searcher.Get())
            {
                Console.WriteLine("======= MOTHER BOARD =======");
                Console.WriteLine("Manufacturer = " + GetValue(mobj, "Manufacturer"));
                Console.WriteLine("Type = " + GetValue(mobj, "Product"));
            }
        }

        static void getSystemInfo()
        {
            ManagementObjectSearcher os_searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject mobj in os_searcher.Get())
            {
                Console.WriteLine("======= SYSTEM =======");
                Console.WriteLine("OS Caption = " + GetValue(mobj, "Caption"));
                Console.WriteLine("Version = " + GetValue(mobj, "Version"));
                Console.WriteLine("Manufacturer = " + GetValue(mobj, "Manufacturer"));
                Console.WriteLine("Architecture = " + GetValue(mobj, "OSArchitecture"));
                Console.WriteLine("Admin = " + GetValue(mobj, "RegisteredUser"));
            }
        }

        static void getPSUinfo()
        {
            ManagementObjectSearcher os_searcher = new ManagementObjectSearcher("SELECT * FROM CIM_PowerSupply");
            foreach (ManagementObject mobj in os_searcher.Get())
            {
                Console.WriteLine("======= PSU =======");
                Console.WriteLine("Computer Name = " + GetValue(mobj, "Name"));
            }
        }

        // Get a value from the ManagementObject.
        static string GetValue(ManagementObject mobj, string property_name)
        {
            string value;
            try
            {
                value = mobj[property_name].ToString();
            }
            catch (Exception ex)
            {
                value = "*** Error: " + ex.Message;
            }

            return value;
        }
    }
}