using System;
using System.IO;

namespace Map_editing
{
    class Program
    {
        static void Main(string[] args)
        {
            init();
            int posx = Console.LargestWindowWidth / 2, posy = Console.LargestWindowHeight / 2;
            ConsoleKey input;
            int[,] map = new int[Console.LargestWindowHeight, Console.LargestWindowWidth];
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    map[y, x] = -1;
                }
            }
            a:
            Console.SetCursorPosition(0, 0);
            Console.SetCursorPosition(posx, posy);
            input = Console.ReadKey(true).Key;
            #region switch
            switch (input)
            {
                case ConsoleKey.Backspace:
                    map[posy, posx] = -1;
                    Console.Write(" ");
                    break;
                case ConsoleKey.Tab:
                    break;
                case ConsoleKey.Clear:
                    break;
                case ConsoleKey.Enter:
                    if (!File.Exists("map.txt")) File.Create("map.txt");
                    File.WriteAllText("map.txt", getcontents(map));
                    break;
                case ConsoleKey.Pause:
                    break;
                case ConsoleKey.Escape:
                    break;
                case ConsoleKey.Spacebar:
                    break;
                case ConsoleKey.PageUp:
                    break;
                case ConsoleKey.PageDown:
                    break;
                case ConsoleKey.End:
                    break;
                case ConsoleKey.Home:
                    break;
                case ConsoleKey.LeftArrow:
                    if (posx > 0) posx--;
                    break;
                case ConsoleKey.UpArrow:
                    if (posy > 0) posy--;
                    break;
                case ConsoleKey.RightArrow:
                    if (posx < Console.LargestWindowWidth-1) posx++;
                    break;
                case ConsoleKey.DownArrow:
                    if (posy < Console.LargestWindowHeight-2) posy++;
                    break;
                case ConsoleKey.Select:
                    break;
                case ConsoleKey.Print:
                    break;
                case ConsoleKey.Execute:
                    break;
                case ConsoleKey.PrintScreen:
                    break;
                case ConsoleKey.Insert:
                    break;
                case ConsoleKey.Delete:
                    break;
                case ConsoleKey.Help:
                    break;
                case ConsoleKey.D0:
                    Console.Write("0");
                    map[posy, posx] = 0;
                    break;
                case ConsoleKey.D1:
                    Console.Write("1");
                    map[posy, posx] = 1;
                    break;
                case ConsoleKey.D2:
                    Console.Write("2");
                    map[posy, posx] = 2;
                    break;
                case ConsoleKey.D3:
                    Console.Write("3");
                    map[posy, posx] = 3;
                    break;
                case ConsoleKey.D4:
                    Console.Write("4");
                    map[posy, posx] = 4;
                    break;
                case ConsoleKey.D5:
                    Console.Write("5");
                    map[posy, posx] = 5;
                    break;
                case ConsoleKey.D6:
                    Console.Write("6");
                    map[posy, posx] = 6;
                    break;
                case ConsoleKey.D7:
                    Console.Write("7");
                    map[posy, posx] = 7;
                    break;
                case ConsoleKey.D8:
                    Console.Write("8");
                    map[posy, posx] = 8;
                    break;
                case ConsoleKey.D9:
                    Console.Write("9");
                    map[posy, posx] = 9;
                    break;
                case ConsoleKey.A:
                    break;
                case ConsoleKey.B:
                    break;
                case ConsoleKey.C:
                    break;
                case ConsoleKey.D:
                    break;
                case ConsoleKey.E:
                    break;
                case ConsoleKey.F:
                    break;
                case ConsoleKey.G:
                    break;
                case ConsoleKey.H:
                    break;
                case ConsoleKey.I:
                    break;
                case ConsoleKey.J:
                    break;
                case ConsoleKey.K:
                    break;
                case ConsoleKey.L:
                    break;
                case ConsoleKey.M:
                    break;
                case ConsoleKey.N:
                    break;
                case ConsoleKey.O:
                    break;
                case ConsoleKey.P:
                    break;
                case ConsoleKey.Q:
                    break;
                case ConsoleKey.R:
                    break;
                case ConsoleKey.S:
                    break;
                case ConsoleKey.T:
                    break;
                case ConsoleKey.U:
                    break;
                case ConsoleKey.V:
                    break;
                case ConsoleKey.W:
                    break;
                case ConsoleKey.X:
                    break;
                case ConsoleKey.Y:
                    break;
                case ConsoleKey.Z:
                    break;
                case ConsoleKey.LeftWindows:
                    break;
                case ConsoleKey.RightWindows:
                    break;
                case ConsoleKey.Applications:
                    break;
                case ConsoleKey.Sleep:
                    break;
                case ConsoleKey.NumPad0:
                    break;
                case ConsoleKey.NumPad1:
                    break;
                case ConsoleKey.NumPad2:
                    break;
                case ConsoleKey.NumPad3:
                    break;
                case ConsoleKey.NumPad4:
                    break;
                case ConsoleKey.NumPad5:
                    break;
                case ConsoleKey.NumPad6:
                    break;
                case ConsoleKey.NumPad7:
                    break;
                case ConsoleKey.NumPad8:
                    break;
                case ConsoleKey.NumPad9:
                    break;
                case ConsoleKey.Multiply:
                    break;
                case ConsoleKey.Add:
                    break;
                case ConsoleKey.Separator:
                    break;
                case ConsoleKey.Subtract:
                    break;
                case ConsoleKey.Decimal:
                    break;
                case ConsoleKey.Divide:
                    break;
                case ConsoleKey.F1:
                    break;
                case ConsoleKey.F2:
                    break;
                case ConsoleKey.F3:
                    break;
                case ConsoleKey.F4:
                    break;
                case ConsoleKey.F5:
                    break;
                case ConsoleKey.F6:
                    break;
                case ConsoleKey.F7:
                    break;
                case ConsoleKey.F8:
                    break;
                case ConsoleKey.F9:
                    break;
                case ConsoleKey.F10:
                    break;
                case ConsoleKey.F11:
                    break;
                case ConsoleKey.F12:
                    break;
                case ConsoleKey.F13:
                    break;
                case ConsoleKey.F14:
                    break;
                case ConsoleKey.F15:
                    break;
                case ConsoleKey.F16:
                    break;
                case ConsoleKey.F17:
                    break;
                case ConsoleKey.F18:
                    break;
                case ConsoleKey.F19:
                    break;
                case ConsoleKey.F20:
                    break;
                case ConsoleKey.F21:
                    break;
                case ConsoleKey.F22:
                    break;
                case ConsoleKey.F23:
                    break;
                case ConsoleKey.F24:
                    break;
                case ConsoleKey.BrowserBack:
                    break;
                case ConsoleKey.BrowserForward:
                    break;
                case ConsoleKey.BrowserRefresh:
                    break;
                case ConsoleKey.BrowserStop:
                    break;
                case ConsoleKey.BrowserSearch:
                    break;
                case ConsoleKey.BrowserFavorites:
                    break;
                case ConsoleKey.BrowserHome:
                    break;
                case ConsoleKey.VolumeMute:
                    break;
                case ConsoleKey.VolumeDown:
                    break;
                case ConsoleKey.VolumeUp:
                    break;
                case ConsoleKey.MediaNext:
                    break;
                case ConsoleKey.MediaPrevious:
                    break;
                case ConsoleKey.MediaStop:
                    break;
                case ConsoleKey.MediaPlay:
                    break;
                case ConsoleKey.LaunchMail:
                    break;
                case ConsoleKey.LaunchMediaSelect:
                    break;
                case ConsoleKey.LaunchApp1:
                    break;
                case ConsoleKey.LaunchApp2:
                    break;
                case ConsoleKey.Oem1:
                    break;
                case ConsoleKey.OemPlus:
                    break;
                case ConsoleKey.OemComma:
                    break;
                case ConsoleKey.OemMinus:
                    break;
                case ConsoleKey.OemPeriod:
                    break;
                case ConsoleKey.Oem2:
                    break;
                case ConsoleKey.Oem3:
                    break;
                case ConsoleKey.Oem4:
                    break;
                case ConsoleKey.Oem5:
                    break;
                case ConsoleKey.Oem6:
                    break;
                case ConsoleKey.Oem7:
                    break;
                case ConsoleKey.Oem8:
                    break;
                case ConsoleKey.Oem102:
                    break;
                case ConsoleKey.Process:
                    break;
                case ConsoleKey.Packet:
                    break;
                case ConsoleKey.Attention:
                    break;
                case ConsoleKey.CrSel:
                    break;
                case ConsoleKey.ExSel:
                    break;
                case ConsoleKey.EraseEndOfFile:
                    break;
                case ConsoleKey.Play:
                    break;
                case ConsoleKey.Zoom:
                    break;
                case ConsoleKey.NoName:
                    break;
                case ConsoleKey.Pa1:
                    break;
                case ConsoleKey.OemClear:
                    break;
                default:
                    break;
            }
            #endregion
            goto a;
        }

        static string getcontents(int[,] map)
        {
            string content = "";
            for (int y = 0; y < map.GetLength(0)-1; y++)
            {
                for (int x = 0; x < map.GetLength(1)-1; x++)
                {
                    if (map[y, x] > 0) content += "Just " + map[y, x];
                    else content += "Nothing";
                    content += ",";
                }
                if (map[y, map.GetLength(1)-1] > 0) content += "Just " + map[y, map.GetLength(1)-1];
                else content += "Nothing";
                content += ".";
            }
            for (int x = 0; x < map.GetLength(1) - 1; x++)
            {
                if (map[map.GetLength(0)-1, x] > 0) content += "Just " + map[map.GetLength(0) - 1, x];
                else content += "Nothing";
                content += ",";
            }
            if (map[map.GetLength(0) - 1, map.GetLength(1) - 1] > 0) content += "Just " + map[map.GetLength(0) - 1, map.GetLength(1) - 1];
            else content += "Nothing";
            return content;
        }

        static void init()
        {
            Console.SetBufferSize(Console.LargestWindowWidth * 2, Console.LargestWindowHeight * 2);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.CursorVisible = true;
            Console.CursorSize = 100;
        }
    }
}
