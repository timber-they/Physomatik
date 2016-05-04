using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physomatik1
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] start = new double[2] { 0, 45 };
            double[] vector = new double[2] { 10, 45 };
            double t = 0;
            int sizex = Console.LargestWindowWidth, sizey = Console.LargestWindowHeight;
            Console.SetBufferSize(sizex * 10, sizey * 10);
            Console.SetWindowSize(sizex, sizey);
            Console.SetBufferSize(sizex, sizey);
            Console.SetWindowPosition(0, 0);
            Console.CursorVisible = false;
            double[,] simulated = new double[500, 2];
            int pos = 0;
            Console.SetCursorPosition(0, 0);
            Console.Write("Simulating...");
            double step = 0.005;
            while(pos < simulated.GetLength(0) - 1)
            {
                try
                {

                    vector = Physomatik.getPosatShotwithS(200, 30, 1, Physomatik.g, 1, 0.01, Physomatik.Dichte_Luft, t, 1, step);
                    simulated[pos, 0] = vector[0];
                    simulated[pos, 1] = vector[1];
                    Console.SetCursorPosition((int)(vector[0] / 0.1), sizey / 2 - (int)(vector[1] / 0.1));
                    if (Math.Sqrt(vector[0]*vector[0] + vector[1]*vector[1]) <= 1) Console.ForegroundColor = ConsoleColor.Red;
                    else Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("█");
                    pos++;
                    t += step;
                }
                catch
                {
                    break;
                }
            }
            pos = 0;
            Console.Clear();
            while(pos < simulated.GetLength(0)-1)
            {
                try
                {

                    Console.SetCursorPosition((int)(simulated[pos, 0] / 0.1), sizey / 2 - (int)(simulated[pos, 1] / 0.1));
                    if (Math.Sqrt(simulated[pos,0] * simulated[pos, 0] + simulated[pos, 1] * simulated[pos, 1]) <= 1) Console.ForegroundColor = ConsoleColor.Red;
                    else Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("█");
                    System.Threading.Thread.Sleep(10);
                    pos++;
                }
                catch
                {
                    break;
                }
            }
            Console.ReadLine();
        }
    }

    class Physomatik
    {
        public static double PI = 3.14159265359;
        public static double g = 9.81;
        public static double Dichte_Luft = 1.2041;

        #region resVector
        public static double[] getresVector(double[,] vectors)
        {
            double xsum = 0, ysum = 0;
            double[] vector = new double[2];
            for (int i = 0; i < vectors.GetLength(0); i++)
            {
                vector[0] = vectors[i, 0];
                vector[1] = vectors[i, 1];
                xsum += getxPart(vector);
                ysum += getyPart(vector);
            }
            vector[0] = Math.Sqrt(xsum * xsum + ysum * ysum);
            vector[1] = getresAngle(xsum, ysum);
            if (vector[1] < 0) vector[1] = 360 + vector[1];
            vector[1] %= 360;
            return vector;
        }

        public static double getresAngle(double x, double y)
        {
            if (x > 0)
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

        public static double[] getSpeedatShot(double F_Wurf,double angle_Wurf, double m, double g, double c_w, double A, double P, double t, double t_throw, double step)
        {
            double F_G = getF_G(m, g), F_L = 0;
            double[] v = new double[2] { 0, 0 }, F_res = new double[2];
            double[,] Fs;
            for (double i = 0; i < t; i+=step)
            {
                F_L = getF_L(c_w, A, P, v[0]);
                if (i <= t_throw)
                    Fs = new double[3, 2] { { F_G, 270 }, { F_Wurf, angle_Wurf }, { F_L, (v[1] + 180)%360 } };
                else
                    Fs = new double[2, 2] { { F_G, 270 }, { F_L, (v[1] + 180)%360 } };
                F_res = getresVector(Fs);
                v = getnewSpeed(v, F_res, step, m);
            }
            return v;
        }

        public static double getSpeedwithAir(double A, double F, double P, double c_w, double t, double m)
        {
            return ((Math.Sqrt(2 * c_w * P * t * t * A * F + m * m) - m) / c_w * P * t * A);
        }

        public static double getSpeedwithAirandStart(double A, double F, double P, double c_w, double t, double v0, double m)
        {
            return ((Math.Sqrt(2 * A * F * P * c_w * t * t + 2 * A * P * c_w * v0 * m * t + m * m) - m) / c_w * P * t * A);
        }

        public static double[] getPosatShot(double F_Wurf, double angle_Wurf, double m, double g, double c_w, double A, double P, double t, double t_throw, double step)
        {
            double[] pos = new double[2], speed = new double[2];
            for (double i = 0; i < t; i+=step)
            {
                speed = getSpeedatShot(F_Wurf, angle_Wurf, m, g, c_w, A, P, i, t_throw, step);
                pos[0] += getxPart(speed)*step;
                pos[1] += getyPart(speed)*step;
            }
            return pos;
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
        }

        public static double getDeltaSpeed(double a, double dt)
        {
            return a * dt;
        }

        public static double getF_G(double m, double g)
        {
            return m * g;
        }

        public static double getF_L(double c_w, double A, double P, double v)
        {
            return 0.5 * c_w * A * P * v * v;
        }

        #region newSpeed
        public static double[] getnewSpeed(double[] vectorv, double[] F, double dt, double m)
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

        public static double geta(double F, double m)
        {
            return F / m;
        }

        #region getPart
        public static double getyPart(double[] vector)
        {
            return Math.Sin(ToRadian(vector[1])) * vector[0];
        }

        public static double getxPart(double[] vector)
        {
            return Math.Cos(ToRadian(vector[1])) * vector[0];
        }
        #endregion

        public static double ToRadian(double degree)
        {
            return degree * PI / 180;
        }

        #region ToRadian/Degree
        public static double ToDegree(double radian)
        {
            return radian * 180 / PI;
        }
        #endregion
    }
}
