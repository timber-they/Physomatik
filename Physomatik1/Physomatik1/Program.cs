using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Physomatik1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            a:
            Map_editing.Main_Map(args);
            editData("data.txt");
            Console.CursorVisible = false;
            Process p = new Process();
            p.StartInfo.FileName = "Haskell_calcs.exe";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            p.Close();
            Console.Clear();
            int cx = 0, cy = 0;
            if(File.Exists("Physomatik_betterVisualisation.exe"))
            {
                Process p2 = new Process();
                p2.StartInfo.FileName = "Physomatik_betterVisualisation.exe";
                p2.Start();
            }
            else if (File.Exists("map.txt") && File.Exists("data.txt") && File.Exists("end.txt"))
            {
                List<double[]> data = Physomatik.getsimulatedPossesFromFile("end.txt");
                foreach (double[] item in data)
                {
                    if (cx != (int)Math.Round(item[0]) || cy != Console.WindowHeight - (int)Math.Round(item[1]) - 1)
                    {
                        cx = (int)Math.Round(item[0]);
                        cy = Console.WindowHeight - (int)Math.Round(item[1]) - 1;
                        Console.SetCursorPosition(cx, cy);
                        Console.Write("█");
                    }
                    System.Threading.Thread.Sleep(1);
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) break;
                }
            }
            Console.ReadLine();
            goto a;
        }

        static void editData(string filePath)
        {
            Console.Clear();
            string content = File.ReadAllText(filePath);
            string[] splitted = content.Split(':');
            string[] newdata = new string[splitted.Length];
            string[] text = new string[] { "F_throw", "angle_throw", "t_throw", "f_floor", "cw", "p", "A", "v_old", "pos_old", "step", "m", "g", "t", "max", "filePath", "divider0", "divider1", "(-)bounce Factor" };
            for (int i = 0; i < splitted.Length; i++)
            {
                Console.WriteLine("Der Wert von " + text[i] + " beträgt " + splitted[i] + ". Zum Ändern neuen Wert eingeben, ansonsten nichts eingeben!");
                string input = Console.ReadLine();
                if (input == "") newdata[i] = splitted[i];
                else
                {
                    newdata[i] = input;
                }
            }
            string final = "";
            for (int i = 0; i < newdata.Length-1; i++)
            {
                final += newdata[i] + ":";
            }
            File.WriteAllText(filePath, final + newdata.Last());
        }
    }

    class Physomatik
    {
        public static double PI = 3.14159265359;
        public static double g = 9.81;
        public static double Density_Air = 1.2041;

        public static List<double[]> getsimulatedPossesFromFile(string FilePath)
        {
            string content = File.ReadAllText(FilePath);
            List<char> listi = content.ToList();
            listi.Remove('[');
            listi.Remove(']');
            content = string.Join("", listi);
            List<string> TupleStrings = ToListByBrackets(listi);
            List<double[]> final = new List<double[]>();
            foreach (string item in TupleStrings)
            {
                final.Add(stringtoDoubles(item));
            }
            return final;
        }

        public static double[] stringtoDoubles(string content)
        {
            string[] splitted = content.Split(',');
            return new double[2] { Convert.ToDouble(splitted[0]), Convert.ToDouble(splitted[1]) };
        }

        public static List<string> ToListByBrackets(List<char> listi)
        {
            List<int> open = getIndexesOfX(listi, '(');
            List<int> close = getIndexesOfX(listi, ')');
            List<string> final = new List<string>();
            while (listi.Count > 0 && close.Count > 0 && open.Count > 0)
            {
                final.Add(string.Join("", (listi.GetRange(open.First() + 1, close.First() - open.First() - 1))));
                open.Remove(open.First());
                close.Remove(close.First());
            }
            return final;
        }

        public static List<int> getIndexesOfX(List<char> listi, char x)
        {
            char[] arri = listi.ToArray();
            List<int> ints = new List<int>();
            for (int i = 0; i < arri.Length; i++)
            {
                if (arri[i] == x) ints.Add(i);
            }
            return ints;
        }

        #region resVector
        public static double[] getresVector(double[,] vectors)
        {
            double xsum = 0, ysum = 0;
            double[] vector = new double[2];
            for (int i = 0; i < vectors.GetLength(0); i++)
            {
                vector[0] = vectors[i, 0];
                vector[1] = vectors[i, 1];
                xsum += getxPart(vector); //get the parts...
                ysum += getyPart(vector);
            }
            vector[0] = Math.Sqrt(xsum * xsum + ysum * ysum); //and add the parts
            vector[1] = getresAngle(xsum, ysum); //and get the angle
            if (vector[1] < 0) vector[1] = 360 + vector[1];
            vector[1] %= 360;
            return vector;
        }

        public static double getresAngle(double x, double y)
        {
            if (x > 0) //this is ugly code. But I think, theres no better way to do this
                return ToDegree(Math.Atan(y / x));
            if (x == 0)
                return Math.Sign(y) * 90;
            if (y == 0)
                return 180;
            if (y > 0)
                return 180 - ToDegree(Math.Atan(Math.Abs(y) / Math.Abs(x)));
            else
                return 180 + ToDegree(Math.Atan(Math.Abs(y) / Math.Abs(x)));
        }
        #endregion

        #region Shot
        public static double[] getSpeedatShot(double F_Wurf, double angle_Wurf, double m, double g, double c_w, double A, double P, double t, double t_throw, double step) //you should use this part if you forgot your current position/speed
        {
            double F_G = getF_G(m, g), F_L = 0;
            double[] v = new double[2] { 0, 0 }, F_res = new double[2];
            double[,] Fs;
            for (double i = 0; i < t; i += step)
            {
                F_L = getF_L(c_w, A, P, v[0]);
                if (i <= t_throw)
                    Fs = new double[3, 2] { { F_G, 270 }, { F_Wurf, angle_Wurf }, { F_L, (v[1] + 180) % 360 } };
                else
                    Fs = new double[2, 2] { { F_G, 270 }, { F_L, (v[1] + 180) % 360 } };
                F_res = getresVector(Fs);
                v = getnewSpeed(v, F_res, step, m);
            }
            return v;
        }

        public static double[] getPosatShot(double F_Wurf, double angle_Wurf, double m, double g, double c_w, double A, double P, double t, double t_throw, double step)
        {
            double[] pos = new double[2], speed = new double[2];
            for (double i = 0; i < t; i += step)
            {
                speed = getSpeedatShot(F_Wurf, angle_Wurf, m, g, c_w, A, P, i, t_throw, step);
                pos[0] += getxPart(speed) * step;
                pos[1] += getyPart(speed) * step;
            }
            return pos;
        } //as well as this

        public static double[,] getnewPos_Speed(double[] F_Wurf, double m, double g, double c_w, double A, double P, double step, double[] oldpos, double[] oldv) //this gives you a new position/speed - it's stepwise because air is bad
        {
            double[,] vectors = new double[3, 2] { { F_Wurf[0], F_Wurf[1] }, { getF_G(m, g), 270 }, { getF_L(c_w, A, P, oldv[0]), (oldv[1] + 180) % 360 } };
            double[] newSpeed = getnewSpeed(oldv, getresVector(vectors), step, m);
            double[] pos = new double[2] { getxPart(newSpeed) * step + oldpos[0], getyPart(newSpeed) * step + oldpos[1] };
            return new double[2, 2] { { pos[0], pos[1] }, { newSpeed[0], newSpeed[1] } };
        }

        public static double[] getPosatShotwithS(double F_Wurf, double angle_Wurf, double m, double g, double c_w, double A, double P, double t, double s_throw, double step)
        {
            double[] pos = new double[2], speed = new double[2];
            double t_throw = t;
            for (double i = 0; i < t; i += step)
            {
                if (Math.Sqrt(pos[0] * pos[0] + pos[1] * pos[1]) > s_throw && t_throw == t) t_throw = i;
                speed = getSpeedatShot(F_Wurf, angle_Wurf, m, g, c_w, A, P, i, t_throw, step);
                pos[0] += getxPart(speed) * step;
                pos[1] += getyPart(speed) * step;
            }
            return pos;
        } //like this one

        public static double getSpeedwithAir(double A, double F, double P, double c_w, double t, double m) //this formula only works with positive or not too negative F
        {
            return ((Math.Sqrt(2 * c_w * P * t * t * A * F + m * m) - m) / c_w * P * t * A);
        }

        public static double getSpeedwithAirandStart(double A, double F, double P, double c_w, double t, double v0, double m) //...just like this
        {
            return ((Math.Sqrt(2 * A * F * P * c_w * t * t + 2 * A * P * c_w * v0 * m * t + m * m) - m) / c_w * P * t * A);
        }
        #endregion

        public static double getDeltaSpeed(double a, double dt)
        {
            return a * dt;
        } //this one is obvious

        public static double[] getnewSpeedafterImpactm(double[] v0, double step, double c_w, double A, double P, double m, double g, double t, double f)
        {
            double vx = getxPart(v0), vy = getyPart(v0);
            double F_N = getF_G(m, g) + getF(vy, t, m);
            if (vx > 0)
                vx += geta(-f * F_N - getF_L(c_w, A, P, vx), m) * step;
            else
                vx += geta(+f * F_N + getF_L(c_w, A, P, vx), m) * step;
            return getresVector(new double[2, 2] { { vx, 0 }, { 0, 90 } });
        }

        public static double[] getnewSpeedafterImpact(double[] v0, double step, double c_w, double A, double P, double m, double g, double t, double f, double angle)
        {
            double vx = getxPart(v0), vy = getyPart(v0);
            double F = getF(getParts(v0[0], (v0[1] + 180) % 360, (angle + 90) % 360, angle)[0, 0], t, m);
            double F_N = getF_N(angle, m, g) + getParts(F, (v0[1] + 180) % 360, (angle + 90) % 360, angle)[0, 0];
            double F_H = getF_H(angle, m, g);
            double F_L = getF_L(c_w, A, P, v0[0]);
            double F_R = getF_R(f, F_N);
            double[] F_Resa = getresVector(new double[3, 2] { { F_H, (angle + 180) % 360 }, { F_L, (v0[1] + 180) % 360 }, { F_R, (v0[1] + 180) % 360 } });
            double[] F_Res = new double[2] { getParts(F_Resa[0], F_Resa[1], angle, angle + 90)[0, 0], getParts(F_Resa[0], F_Resa[1], angle, angle + 90)[0, 1] };
            vx += getxPart(F_Res) / m * step;
            vy += getyPart(F_Res) / m * step;
            return getresVector(new double[2, 2] { { vx, 0 }, { vy, 90 } });
        }

        public static double[] getnewPos(double[] v, double[] pos, double step)
        {
            return new double[2] { pos[0] + getxPart(v) * step, pos[1] + getyPart(v) * step };
        }

        #region getParts
        public static double getonexPart(double res, double resangle, double otherangle, double ownangle)
        {
            double A = ToRadian(otherangle), B = ToRadian(resangle), C = ToRadian(ownangle);
            return (res * Math.Tan(A) * Math.Cos(B) - res * Math.Sin(B)) / (Math.Tan(A) - Math.Tan(C));
        }

        public static double[,] getxParts(double res, double resangle, double angle1, double angle2)
        {
            return new double[2, 2] { { getonexPart(res, resangle, angle2, angle1), angle1 }, { getonexPart(res, resangle, angle1, angle2), angle2 } };
        }

        public static double[,] getParts(double res, double resangle, double angle1, double angle2)
        {
            double[,] xParts = getxParts(res, resangle, angle1, angle2);
            return new double[2, 2] { { xParts[0, 0] / Math.Cos(ToRadian(xParts[0, 1])), xParts[0, 1] }, { xParts[1, 0] / Math.Cos(ToRadian(xParts[1, 1])), xParts[1, 1] } };
        }
        #endregion

        #region getF
        public static double getF_G(double m, double g) //return the fitting Fs
        {
            return m * g;
        }

        public static double getF_L(double c_w, double A, double P, double v)
        {
            return 0.5 * c_w * A * P * v * v;
        }

        public static double getF_N(double angle, double m, double g)
        {
            return Math.Cos(ToRadian(angle)) * getF_G(m, g);
        }

        public static double getF_H(double angle, double m, double g)
        {
            return Math.Sin(ToRadian(angle)) * getF_G(m, g);
        }

        public static double getF_R(double f, double angle, double m, double g)
        {
            return getF_R(f, getF_N(angle, m, g));
        }

        public static double getF_R(double f, double F_N)
        {
            return f * F_N;
        }
        #endregion

        #region Hill
        public static double getnewSpeedatHill(double f, double angle, double m, double g, double F_S, double step, double[] v0, double c_w, double A, double P) //it's nearly the same as the shot - you just can't fall down and have more resistance
        {
            double v = v0[0];
            if (v0[1] > (angle + 180) % 360 - 5 && v0[1] < (angle + 180) % 360 + 5)
                v *= -1;
            if (v > 0)
                return v + ((F_S - getF_H(angle, m, g) - getF_R(f, angle, m, g) - getF_L(c_w, A, P, v)) / m) * step;
            else
                return v + ((F_S - getF_H(angle, m, g) + getF_R(f, angle, m, g) + getF_L(c_w, A, P, v)) / m) * step;
        }

        public static double[] getnewPosatHill(double f, double angle, double m, double g, double F_S, double step, double[] v0, double c_w, double A, double P, double[] pos0)
        {
            double[] newpos = new double[2] { pos0[0], pos0[1] };
            double newspeed = getnewSpeedatHill(f, angle, m, g, F_S, step, v0, c_w, A, P);
            newpos[0] += getxPart(new double[] { newspeed * step, angle });
            newpos[1] += getyPart(new double[] { newspeed * step, angle });
            return newpos;
        }

        public static double[,] getnewPos_SpeedatHill(double f, double angle, double m, double g, double F_S, double step, double[] v0, double c_w, double A, double P, double[] pos0)
        {
            double[] newSpeed = new double[2] { getnewSpeedatHill(f, angle, m, g, F_S, step, v0, c_w, A, P), angle };
            if (newSpeed[0] < 0)
            {
                newSpeed[0] *= -1;
                newSpeed[1] = (newSpeed[1] + 180) % 360;
            }
            return new double[2, 2] { { getnewPosatHill(f, angle, m, g, F_S, step, v0, c_w, A, P, pos0)[0], getnewPosatHill(f, angle, m, g, F_S, step, v0, c_w, A, P, pos0)[1] }, { newSpeed[0], newSpeed[1] } };
        }
        #endregion

        #region newSpeed
        public static double[] getnewSpeed(double[] vectorv, double[] F, double dt, double m) //returns the new Speed in relativity to the old one
        {
            double[,] vectors = new double[2, 2] { { getnewSpeed(getxPart(vectorv), getxPart(F), dt, m), 0 }, { getnewSpeed(getyPart(vectorv), getyPart(F), dt, m), 90 } };
            return getresVector(vectors);
        }

        public static double getnewSpeed(double v0, double F, double dt, double m)
        {
            return v0 + getDeltaSpeed(geta(F, m), dt);
        }

        public static double getnewSpeed(double v0, double a, double dt)
        {
            return v0 + getDeltaSpeed(a, dt);
        }

        #endregion

        #region FAM
        public static double getF(double v, double t, double m)
        {
            return (v / t) * m;
        }

        public static double geta(double F, double m)
        {
            return F / m;
        }
        #endregion

        #region getPart
        public static double getyPart(double[] vector) //returns the y-/x-Part
        {
            return Math.Sin(ToRadian(vector[1])) * vector[0];
        }

        public static double getxPart(double[] vector)
        {
            return Math.Cos(ToRadian(vector[1])) * vector[0];
        }
        #endregion

        #region ToRadian/Degree
        public static double ToRadian(double degree) //why does C# use radians?
        {
            return degree * PI / 180;
        }

        public static double ToDegree(double radian)
        {
            return radian * 180 / PI;
        }
        #endregion
    }

    class Map_editing
    {
        public static void Main_Map(string[] args)
        {
            int startx = -1, starty = -1;
            init();
            int posx = Console.LargestWindowWidth / 2, posy = Console.LargestWindowHeight / 2;
            ConsoleKey input;
            int[,] map = loadMap("map.txt");
            a:
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
                    return;
                case ConsoleKey.Spacebar:
                    startx = posx;
                    starty = posy;
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
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    break;
                case ConsoleKey.DownArrow:
                    if (posy < Console.LargestWindowHeight - 2) posy++;
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
                #region Numbers
                case ConsoleKey.D0:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("0");
                                map[y, x] = 0;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("0");
                            map[posy, x] = 0;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("0");
                            map[y, posx] = 0;
                        }
                    }
                    else
                    {
                        Console.Write("0");
                        map[posy, posx] = 0;
                    }
                    break;
                case ConsoleKey.D1:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("1");
                                map[y, x] = 1;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("1");
                            map[posy, x] = 1;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("1");
                            map[y, posx] = 1;
                        }
                    }
                    else
                    {
                        Console.Write("1");
                        map[posy, posx] = 1;
                    }
                    break;
                case ConsoleKey.D2:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("2");
                                map[y, x] = 2;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("2");
                            map[posy, x] = 2;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("2");
                            map[y, posx] = 2;
                        }
                    }
                    else
                    {
                        Console.Write("2");
                        map[posy, posx] = 2;
                    }
                    break;
                case ConsoleKey.D3:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("3");
                                map[y, x] = 3;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("3");
                            map[posy, x] = 3;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("3");
                            map[y, posx] = 3;
                        }
                    }
                    else
                    {
                        Console.Write("3");
                        map[posy, posx] = 3;
                    }
                    break;
                case ConsoleKey.D4:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("4");
                                map[y, x] = 4;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("4");
                            map[posy, x] = 4;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("4");
                            map[y, posx] = 4;
                        }
                    }
                    else
                    {
                        Console.Write("4");
                        map[posy, posx] = 4;
                    }
                    break;
                case ConsoleKey.D5:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("5");
                                map[y, x] = 5;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("5");
                            map[posy, x] = 5;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("5");
                            map[y, posx] = 5;
                        }
                    }
                    else
                    {
                        Console.Write("5");
                        map[posy, posx] = 5;
                    }
                    break;
                case ConsoleKey.D6:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("6");
                                map[y, x] = 6;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("6");
                            map[posy, x] = 6;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("6");
                            map[y, posx] = 6;
                        }
                    }
                    else
                    {
                        Console.Write("6");
                        map[posy, posx] = 6;
                    }
                    break;
                case ConsoleKey.D7:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("7");
                                map[y, x] = 7;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("7");
                            map[posy, x] = 7;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("7");
                            map[y, posx] = 7;
                        }
                    }
                    else
                    {
                        Console.Write("7");
                        map[posy, posx] = 7;
                    }
                    break;
                case ConsoleKey.D8:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("8");
                                map[y, x] = 8;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("8");
                            map[posy, x] = 8;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("8");
                            map[y, posx] = 8;
                        }
                    }
                    else
                    {
                        Console.Write("8");
                        map[posy, posx] = 8;
                    }
                    break;
                case ConsoleKey.D9:
                    if (posx < Console.LargestWindowWidth - 1) posx++;
                    if (startx >= 0 && starty >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            for (int y = starty; y < posy; y++)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write("9");
                                map[y, x] = 9;
                            }
                        }
                    }
                    else if (startx >= 0)
                    {
                        for (int x = startx; x < posx; x++)
                        {
                            Console.SetCursorPosition(x, posy);
                            Console.Write("9");
                            map[posy, x] = 9;
                        }
                    }
                    else if (starty >= 0)
                    {
                        for (int y = starty; y < posy; y++)
                        {
                            Console.SetCursorPosition(posx, y);
                            Console.Write("9");
                            map[y, posx] = 9;
                        }
                    }
                    else
                    {
                        Console.Write("9");
                        map[posy, posx] = 9;
                    }
                    break;
                #endregion
                case ConsoleKey.A:
                    break;
                case ConsoleKey.B:
                    break;
                case ConsoleKey.C:
                    for (int y = 0; y < map.GetLength(0); y++)
                    {
                        for (int x = 0; x < map.GetLength(1); x++)
                        {
                            map[y, x] = -1;
                        }
                    }
                    Console.Clear();
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

        public static int[,] loadMap(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                string content = File.ReadAllText(FilePath);
                string[] splitteda = content.Split(';');
                string[][] splittedb = new string[splitteda.Length][];
                for (int i = 0; i < splitteda.Length; i++)
                {
                    splittedb[i] = splitteda[i].Split(',');
                }
                int[,] final = new int[Console.LargestWindowHeight, Console.LargestWindowWidth];
                for (int y = 0; y < final.GetLength(0); y++)
                {
                    for (int x = 0; x < final.GetLength(1); x++)
                    {
                        if (splittedb.Length > y && splittedb[y].Length > x && splittedb[y][x].StartsWith("Just "))
                        {
                            final[y, x] = Convert.ToInt32(splittedb[y][x].Substring(5));
                            Console.SetCursorPosition(x, y);
                            Console.Write(final[y, x]);
                        }
                        else
                            final[y, x] = -1;
                    }
                }
                return final;
            }
            else
            {
                int[,] final = new int[Console.LargestWindowHeight, Console.LargestWindowWidth];
                for (int y = 0; y < final.GetLength(0); y++)
                {
                    for (int x = 0; x < final.GetLength(1); x++)
                    {
                        final[y, x] = -1;
                    }
                }
                return final;
            }
        }

        public static string getcontents(int[,] map)
        {
            string content = "";
            for (int y = 0; y < map.GetLength(0) - 1; y++)
            {
                for (int x = 0; x < map.GetLength(1) - 1; x++)
                {
                    if (map[y, x] >= 0)
                        content += "Just " + map[y, x];
                    else content += "Nothing";
                    content += ",";
                }
                if (map[y, map.GetLength(1) - 1] >= 0) content += "Just " + map[y, map.GetLength(1) - 1];
                else content += "Nothing";
                content += ";";
            }
            for (int x = 0; x < map.GetLength(1) - 1; x++)
            {
                if (map[map.GetLength(0) - 1, x] >= 0) content += "Just " + map[map.GetLength(0) - 1, x];
                else content += "Nothing";
                content += ",";
            }
            if (map[map.GetLength(0) - 1, map.GetLength(1) - 1] >= 0) content += "Just " + map[map.GetLength(0) - 1, map.GetLength(1) - 1];
            else content += "Nothing";
            return content;
        }

        public static void init()
        {
            Console.SetCursorPosition(0, 0);
            Console.SetBufferSize(Console.LargestWindowWidth * 2, Console.LargestWindowHeight * 2);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.CursorVisible = true;
            Console.CursorSize = 100;
            Console.Clear();
        }
    }
}
