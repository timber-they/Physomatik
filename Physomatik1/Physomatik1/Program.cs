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
            a:
            double[] start = new double[2] { 0, 45 };
            double[] vector = new double[2] { 10, 45 };
            double t = 0;
            int sizex = Console.LargestWindowWidth, sizey = Console.LargestWindowHeight;
            Console.SetBufferSize(sizex * 10, sizey * 10);
            Console.SetWindowSize(sizex, sizey);
            Console.SetBufferSize(sizex, sizey);
            Console.SetWindowPosition(0, 0);
            Console.CursorVisible = false;
            double[,] simulated = new double[100000, 2];
            int pos = 0;
            Console.SetCursorPosition(0, 0);
            double F = Convert.ToDouble(Console.ReadLine()), a = Convert.ToDouble(Console.ReadLine()), m = 1, f = 0.1, c_w = 1, A = 0.1, s = 10;
            Console.Write("Simulating...");
            double step = 0.001;
            double[] v = new double[2] { 0, a }, posi = new double[2];
            double[,] vectors;
            while (pos < simulated.GetLength(0) - 1)
            {
                try
                {
                    if (Math.Sqrt(posi[0]*posi[0]+posi[1]*posi[1]) < s)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        vectors = Physomatik.getnewPos_SpeedatHill(f, a, m, Physomatik.g, F, step, v, c_w, A, Physomatik.Density_Air, posi);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        vectors = Physomatik.getnewPos_Speed(new double[2] { 0, 0 }, m, Physomatik.g, c_w, A, Physomatik.Density_Air, step, posi, v);
                    }
                    v[0] = vectors[1, 0];
                    v[1] = vectors[1, 1];
                    posi[0] = vectors[0, 0];
                    posi[1] = vectors[0, 1];
                    simulated[pos, 0] = posi[0];
                    simulated[pos, 1] = posi[1];
                    Console.SetCursorPosition((int)(sizex / 2 + (posi[0]) / 1), (int)(sizey / 2 - (posi[1]) / 1));
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

                    Console.SetCursorPosition((int)(sizex / 2 + (simulated[pos, 0]) / 1), (int)(sizey / 2 - (simulated[pos, 1]) / 1));
                    if (Math.Sqrt(simulated[pos,0] * simulated[pos, 0] + simulated[pos,1] * simulated[pos,1]) < s) Console.ForegroundColor = ConsoleColor.Red;
                    else Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("█");
                    System.Threading.Thread.Sleep((int)(step*1000));
                    pos++;
                }
                catch
                {
                    break;
                }
            }
            Console.ReadLine();
            Console.Clear();
            goto a;
        }
    }

    class Physomatik
    {
        public static double PI = 3.14159265359;
        public static double g = 9.81;
        public static double Density_Air = 1.2041;

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

        public static double[,] getnewPos_Speed(double[] F_Wurf, double m, double g, double c_w, double A, double P, double step, double[] oldpos, double[] oldv)
        {
            double[,] vectors = new double[3, 2] { { F_Wurf[0], F_Wurf[1] }, { getF_G(m, g), 270 }, { getF_L(c_w, A, P, oldv[0]), (oldv[1] + 180) % 360 } };
            double[] newSpeed = getnewSpeed(oldv, getresVector(vectors), step, m);
            double[] pos = new double[2] { getxPart(newSpeed) * step + oldpos[0], getyPart(newSpeed) * step + oldpos[1] };
            return new double[2,2] { { pos[0], pos[1] },{ newSpeed[0], newSpeed[1] }};
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
            return f * getF_N(angle, m, g);
        }

        public static double getnewSpeedatHill(double f, double angle, double m, double g, double F_S, double step, double[] v0, double c_w, double A, double P)
        {
            double v = v0[0]; // 0
            if (v0[1] > (angle + 180) % 360-5 && v0[1] < (angle + 180) % 360+5)
                v *= -1;
            if(v > 0)
                return v + ((F_S - getF_H(angle, m, g) - getF_R(f, angle, m, g) - getF_L(c_w, A, P, v)) / m) * step; 
            else
                return v + ((F_S - getF_H(angle, m, g) + getF_R(f, angle, m, g) + getF_L(c_w, A, P, v)) / m) * step;
        }

        public static double[] getnewPosatHill(double f, double angle, double m, double g, double F_S, double step, double[] v0, double c_w, double A, double P, double[] pos0)
        {
            double[] newpos = new double[2] { pos0[0], pos0[1] };
            double newspeed = getnewSpeedatHill(f, angle, m, g, F_S, step, v0, c_w, A, P);
            newpos[0] += getxPart(new double[] { newspeed*step, angle });
            newpos[1] += getyPart(new double[] { newspeed*step, angle });
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
            return new double[2, 2] { { getnewPosatHill(f, angle, m, g, F_S, step, v0, c_w, A, P, pos0)[0], getnewPosatHill(f, angle, m, g, F_S, step, v0, c_w, A, P, pos0)[1] },{ newSpeed[0], newSpeed[1] }};
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
