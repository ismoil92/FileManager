using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Utils
{
    public static class WindowsAPI
    {
        #region FIELDS
        const uint _COMMMANDS = 0x00000000;
        const uint _MAXIMSIZE = 0xF030;
        public const uint MAX_PATH = 255;
        #endregion

        #region METHODS

        [DllImport("user32.dll")]
        static extern int DeleteMenu(IntPtr hWnd, uint position, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool revert);

        [DllImport("kernel32.dll", BestFitMapping =true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpszLongPath,
            [MarshalAs(UnmanagedType.LPTStr)]
            StringBuilder lpszShortPath,
            uint cchBuffer);

        public static void ConfigurationConsole() => DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), _MAXIMSIZE, _COMMMANDS);
        #endregion
    }
}
