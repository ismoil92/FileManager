using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Utils;

namespace FileManager
{
    public class FileManager
    {
        #region FIELDS
        public static int WIDTH = 120;
        public static int HEIGHT = 30;
        static string currentDirectory = Directory.GetCurrentDirectory();
        const string fileTree = "tree.txt";
        static string _command = "";
        #endregion

        #region METHODS

        /// <summary>
        /// Метод, для рисование консольное окно
        /// </summary>
        /// <param name="x">начальное координата по оси х</param>
        /// <param name="y">начальное координата по оси у</param>
        /// <param name="width">ширина окна</param>
        /// <param name="height">высота окна</param>
        public static void DrawWindows(int x, int y, int width, int height)
        {

            ///header windows draw
            Console.SetCursorPosition(x, y);
            Console.Write("╔");
            for (int i = 0; i < width - 2; i++)
                Console.Write("═");
            Console.Write("╗");

            ///body windows draw
            Console.SetCursorPosition(x, y + 1);
            for (int i = 0; i < height - 2; i++)
            {
                Console.Write("║");
                for (int j = x + 1; j < x + width - 1; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("║");
            }



            ///footer windows draw
            Console.Write("╚");
            for (int i = 0; i < width - 2; i++)
                Console.Write("═");
            Console.Write("╝");
            Console.SetCursorPosition(x, y);
        }

        /// <summary>
        /// Метод, для возрата расположение курсора
        /// </summary>
        /// <returns>возращает курсор по оси х и у</returns>
         static (int leftCursor, int topCursor) GetCursorPosition() => (Console.CursorLeft, Console.CursorTop);

        /// <summary>
        /// Метод, для возврата короткого пути каталога и файлов
        /// </summary>
        /// <param name="path">Текущий длинный путь каталога и файлов</param>
        /// <returns>возращает короткий путь каталогов и файлов</returns>
        static string GetShortPath(string path)
        {
            StringBuilder shortPath = new StringBuilder((int)WindowsAPI.MAX_PATH);
            WindowsAPI.GetShortPathName(path, shortPath, WindowsAPI.MAX_PATH);
            return shortPath.ToString();
        }

        /// <summary>
        /// Метод, ввода командного строку
        /// </summary>
        /// <param name="width">ширина консоли</param>
         static void ProcessEnterCommand(int width)
        {
            (int left, int top) = GetCursorPosition();
            StringBuilder command = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            char key;
            do
            {
                keyInfo = Console.ReadKey();
                key = keyInfo.KeyChar;
                if(keyInfo.Key!= ConsoleKey.Enter && keyInfo.Key!= ConsoleKey.Backspace && keyInfo.Key!= ConsoleKey.UpArrow)
                {
                    command.Append(key);
                }
                (int currentLeft, int currentTop) = GetCursorPosition();
                if(currentLeft== width-2)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);
                }

                if(keyInfo.Key == ConsoleKey.Backspace)
                {
                    if(command.Length>0)
                    {
                        command.Remove(command.Length - 1, 1);
                    }
                    if(currentLeft>=left)
                    {
                        Console.SetCursorPosition(currentLeft, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);
                    }
                    else
                    {
                        command.Clear();
                        Console.SetCursorPosition(left, top);
                    }
                }

                //Условие для ввода стрелки клавиатуры, чтоб не передвегался 
                if(keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.DownArrow 
                    || keyInfo.Key== ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.RightArrow)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);


                    /// Условие если стрелка вверх нажата,  предыдущий команда выводиться  
                    if(keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        if (!string.IsNullOrEmpty(_command))
                        {
                            DrawConsole2(_command);
                        }
                        //Вернуть последную консольную команду
                    }
                }
            } while (keyInfo.Key!= ConsoleKey.Enter);
            if (command.Length > 0)
            {
                _command = command.ToString();
            }
            ParseCommandString(command.ToString());
        }

        /// <summary>
        /// Второй метод, для рисование консоли, метод для ввода вверхную стрелку клавиатуру чтоб вернул последную команду
        /// </summary>
        /// <param name="command">последная команда которая вводил пользователь </param>
        static void DrawConsole2(string command)
        {
            DrawConsole(GetShortPath(currentDirectory), 0, 26, WIDTH, 3);
            Console.Write(command);
        }

        /// <summary>
        /// Метод, для преобразования командную строку для выполнение команд
        /// </summary>
        /// <param name="command">команда для консоли</param>
         static void ParseCommandString(string command)
        {
            string[] commandParams = command.ToLower().Split(' ');
            if(commandParams.Length>0)
            {
                switch(commandParams[0])
                {
                    case "cd": //Переход на другую директорию
                        if(commandParams.Length>1)
                        {
                            if (Directory.Exists(commandParams[1]) && commandParams[1] !="..")
                            {
                                currentDirectory = commandParams[1];
                            }
                            else if (commandParams[1] == "..")
                            {
                                // Переход на уровень выше каталога. Пока еще не до конца работает
                                DirectoryInfo dir = Directory.GetParent(currentDirectory);
                              currentDirectory = dir.ToString();
                                
                            }
                        }
                        break;
                    case "ls": //Пейджинг т.е. страничный режим
                        if(commandParams.Length>1 && Directory.Exists(commandParams[1]))
                        {
                            if(commandParams.Length>3 && commandParams[2]=="-p" && int.TryParse(commandParams[3], out int n))
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), n);
                            }
                            else
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), 1);
                            }
                        }
                        break;
                    case "cp": // Копирование каталогов и файлов
                        if(commandParams.Length>2 && Directory.Exists(commandParams[1]))
                        {
                            if (Directory.Exists(commandParams[1]) && !Directory.Exists(commandParams[2]))
                            {
                                // Здесь будеть копировать каталогов
                                CopyDirectory(commandParams[1], commandParams[2]);
                            }
                        }
                        else if(commandParams.Length > 2 && File.Exists(commandParams[1]))
                        {
                            CopyFile(commandParams[1], commandParams[2]);
                        }
                        break;
                    case "rm": // Удаление каталогов и файлов
                        if (commandParams.Length>1 && Directory.Exists(commandParams[1]))
                        {
                            Directory.Delete(commandParams[1], true);
                        }
                        else if(commandParams.Length > 1 && File.Exists(commandParams[1]))
                        {
                            File.Delete(commandParams[1]);
                        }
                        break;
                    case "mv": // Перемещение каталогов и файлов
                        if (commandParams.Length>2)
                        {
                            if (Directory.Exists(commandParams[1]) && !Directory.Exists(commandParams[2]))
                            {
                                Directory.Move(commandParams[1], commandParams[2]);
                            }
                            else if (File.Exists(commandParams[1]) && !File.Exists(commandParams[2]))
                            {
                                File.Move(commandParams[1], commandParams[2]);
                            }
                        }
                        break;
                    case "file":  //Получить информацию о файле
                        if (commandParams.Length>1 && File.Exists(commandParams[1]))
                        {
                            ///Информация о файле ...
                        }
                        ///Информация о файле
                        break;
                    case "exit":
                        return;
                }
            }
            UpdateConsole();
        }

        /// <summary>
        /// Метод, для обновление консоли
        /// </summary>
        public static void UpdateConsole()
        {
            DrawConsole(GetShortPath(currentDirectory), 0, 26, WIDTH, 3);
            ProcessEnterCommand(WIDTH);
        }

        /// <summary>
        /// Метод, для рисование консоль со строкой
        /// </summary>
        /// <param name="dir">текущий путь директории</param>
        /// <param name="x">начальная координата по оси х</param>
        /// <param name="y">начальная координата по оси у</param>
        /// <param name="width">ширина консоли</param>
        /// <param name="height">высота консоли</param>
        static void DrawConsole(string dir, int x, int y, int width, int height)
        {
            DrawWindows(x, y, width, height);
            Console.SetCursorPosition(x + 1, y+height / 2);
            Console.Write($"{dir}>");
        }

        /// <summary>
        /// Метод, для рисование дерево каталогов
        /// </summary>
        /// <param name="dir">текущий директорий</param>
        /// <param name="page">страница консольного экрана</param>
         static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            GetTree(tree, dir, "", true);
            File.WriteAllText(fileTree, tree.ToString());
            DrawWindows(0, 0, WIDTH, 18);
            string[] lines = tree.ToString().Split('\n');
            (int currentLeft, int currentTop) = GetCursorPosition();
            int pageLines = 16;
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            if(page>pageTotal)
            {
                page = pageTotal;
            }
            for(int i=(page-1)*pageLines, counter=0; i<(page*pageLines);i++, counter++)
            {
                if(lines.Length-1>i)
                {
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    Console.WriteLine(lines[i]);
                }
            }
            // отрисовка footer
            string footer = $"╡ {page} of {pageTotal} ╞";
            Console.SetCursorPosition(WIDTH / 2 - footer.Length / 2, 17);
            Console.WriteLine(footer);
        }

        /// <summary>
        /// Метод, для получение дерево каталогов
        /// </summary>
        /// <param name="tree">дерево каталогов</param>
        /// <param name="directory">текущий директорий</param>
        /// <param name="indent">отступ от начальной консольного экрана</param>
        /// <param name="lastDirectory">проверяет последный ли директорий, если true, то последный, если false, значить не последный</param>
         static void GetTree(StringBuilder tree, DirectoryInfo directory, string indent, bool lastDirectory)
        {
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += "  ";
            }
            else
            {
                tree.Append("├─");
                indent += "│ ";
            }
            tree.Append($"{directory.Name}\n");
            FileInfo[] files = directory.GetFiles();
            DirectoryInfo[] directories = directory.GetDirectories();
            for(int i=0;i<files.Length;i++)
            {
                if(i==files.Length-1)
                {
                    tree.Append($"{indent}└─{files[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{files[i].Name}\n");
                }
            }
            for(int i=0;i<directories.Length;i++)
            {
                GetTree(tree, directories[i], indent, i==directories.Length-1);
            }
        }

        /// <summary>
        /// Метод, копирование каталогов
        /// </summary>
        /// <param name="sourceDirectoryName">Путь копирование каталогов</param>
        /// <param name="destinationDirectoryName">Путь целевого каталогов</param>
        static void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName)
        {
            Directory.CreateDirectory(destinationDirectoryName);
            foreach(string s1 in Directory.GetFiles(sourceDirectoryName))
            {
                string s2 = destinationDirectoryName + "\\" + Path.GetFileName(s1);
                File.Copy(s1, s2);
            }
            foreach(var s in Directory.GetDirectories(sourceDirectoryName))
            {
                CopyDirectory(s, destinationDirectoryName + "\\" + Path.GetFileName(s));
            }

        }

        /// <summary>
        /// Метод, копирование файлов
        /// </summary>
        /// <param name="sourceFileName">Путь копирование файлов</param>
        /// <param name="destinationFileName">Путь целевого файлов</param>
        static void CopyFile(string sourceFileName, string destinationFileName)=>File.Copy(sourceFileName, Path.Combine(destinationFileName+"\\"+ Path.GetFileName(sourceFileName)));


        #endregion
    }
}