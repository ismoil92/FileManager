using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Utils;

namespace FileManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConfigurationMainMethod();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Метод, для настройки главного входного метода Main();
        /// </summary>
        static void ConfigurationMainMethod()
        {
            WindowsAPI.ConfigurationConsole();
            Console.Title = "FileManager";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            FileManager.DrawWindows(0, 0, FileManager.WIDTH, 18);
            FileManager.DrawWindows(0, 18, FileManager.WIDTH, 8);
            FileManager.UpdateConsole();
        }
    }
}
