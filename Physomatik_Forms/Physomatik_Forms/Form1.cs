using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Physomatik_Forms
{
    public partial class Form1 : Form
    {
        double PI = 3.14159265359;
        bool marked = false;
        int[,] Feld;
        List<float[]> marks = new List<float[]>();
        int nauflösung = 5;

        int pos = 0;
        int fac = 5;
        double[][] data;
        int[,] map;

        bool visualisation = false;
        bool data_edit = false;

        int height = 0;
        int width = 0;

        Pen[] colors = new Pen[10] { Pens.LightGreen,  Pens.YellowGreen, Pens.LimeGreen,Pens.Green, Pens.SeaGreen, Pens.Blue, Pens.BlueViolet, Pens.DarkRed, Pens.IndianRed, Pens.Red };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            height = Form1.ActiveForm.Size.Height;
            width = Form1.ActiveForm.Size.Width;
            Feld = new int[height / nauflösung, width / nauflösung];
            for (int i = 0; i < Feld.GetLength(0); i++)
            {
                for (int u = 0; u < Feld.GetLength(1); u++)
                {
                    Feld[i, u] = -1;
                }
            }
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            data = getsimulatedPossesFromFile("end.txt").ToArray();
            map = loadMap("map.txt");
            pos = data.GetLength(0) - 6;
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!data_edit)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int x = 0; x < map.GetLength(1); x++)
                    {
                        if (map[y, x] >= 0)
                            e.Graphics.FillRectangle(Brushes.Red, x * fac, y * fac, fac, fac);
                    }
                }
                if (!visualisation)
                {
                    button5.Enabled = true;
                    button5.Visible = true;
                    for (int i = 0; i < marks.Count - 1; i += 2)
                    {
                        e.Graphics.DrawLine(Pens.Black, marks.ElementAt(i)[0], marks.ElementAt(i)[1], marks.ElementAt(i + 1)[0], marks.ElementAt(i + 1)[1]);
                    }
                    if (marks.Count % 2 != 0) e.Graphics.DrawEllipse(Pens.Black, marks.Last()[0], marks.Last()[1], 1, 1);
                    else if (marks.Count > 0) actFeld(marks.ElementAt(marks.LastIndexOf(marks.Last()) - 1), marks.Last());
                }
                else
                {
                    button5.Enabled = false;
                    button5.Visible = false;
                    pos += 4;
                    e.Graphics.DrawString(pos.ToString(), new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, 100, 100);
                    if (File.Exists("end.txt") && pos < data.GetLength(0) - 1)
                    {
                        for (int i = 0; i < pos; i++)
                        {
                            e.Graphics.DrawLine(colors[(int)((double)(colors.Length*i/pos))], fac * (float)data[i][0], height - (float)(fac * data[i][1]), fac * (float)data[i + 1][0], height - (float)(fac * data[i + 1][1]));
                        }
                        e.Graphics.DrawString((fac * data[pos][0]).ToString() + " " + (fac * data[pos][1]).ToString(), new Font(FontFamily.GenericMonospace, 10), Brushes.Black, 10, 10);
                    }
                }
            }
            else
            {
                if(!label1.Enabled)enable_dataStuff();
                button5.Enabled = false;
                button5.Visible = false;
            }
        }

        void enable_dataStuff()
        {
            label1.Enabled = true;
            label1.Visible = true;
            label2.Enabled = true;
            label2.Visible = true;
            label3.Enabled = true;
            label3.Visible = true;
            label4.Enabled = true;
            label4.Visible = true;
            label5.Enabled = true;
            label5.Visible = true;
            label6.Enabled = true;
            label6.Visible = true;
            label7.Enabled = true;
            label7.Visible = true;
            label8.Enabled = true;
            label8.Visible = true;
            label9.Enabled = true;
            label9.Visible = true;
            label10.Enabled = true;
            label10.Visible = true;
            label11.Enabled = true;
            label11.Visible = true;
            label12.Enabled = true;
            label12.Visible = true;
            label13.Enabled = true;
            label13.Visible = true;
            label14.Enabled = true;
            label14.Visible = true;
            label15.Enabled = true;
            label15.Visible = true;
            label16.Enabled = true;
            label16.Visible = true;
            label17.Enabled = true;
            label17.Visible = true;
            label18.Enabled = true;
            label18.Visible = true;
            F_Throw.Enabled = true;
            F_Throw.Visible = true;
            Angle_Throw.Enabled = true;
            Angle_Throw.Visible = true;
            t_throw.Enabled = true;
            t_throw.Visible = true;
            f_Floor.Enabled = true;
            f_Floor.Visible = true;
            cw.Enabled = true;
            cw.Visible = true;
            p.Enabled = true;
            p.Visible = true;
            A.Enabled = true;
            A.Visible = true;
            v_old.Enabled = true;
            v_old.Visible = true;
            pos_old.Enabled = true;
            pos_old.Visible = true;
            step.Enabled = true;
            step.Visible = true;
            m.Enabled = true;
            m.Visible = true;
            g.Enabled = true;
            g.Visible = true;
            t.Enabled = true;
            t.Visible = true;
            t_max.Enabled = true;
            t_max.Visible = true;
            filePath.Enabled = true;
            filePath.Visible = true;
            divider0.Enabled = true;
            divider0.Visible = true;
            divider1.Enabled = true;
            divider1.Visible = true;
            bouncefac.Enabled = true;
            bouncefac.Visible = true;
            button1.Enabled = true;
            button1.Visible = true;
        }

        void disable_dataStuff()
        {
            label1.Enabled = false;
            label1.Visible = false;
            label2.Enabled = false;
            label2.Visible = false;
            label3.Enabled = false;
            label3.Visible = false;
            label4.Enabled = false;
            label4.Visible = false;
            label5.Enabled = false;
            label5.Visible = false;
            label6.Enabled = false;
            label6.Visible = false;
            label7.Enabled = false;
            label7.Visible = false;
            label8.Enabled = false;
            label8.Visible = false;
            label9.Enabled = false;
            label9.Visible = false;
            label10.Enabled = false;
            label10.Visible = false;
            label11.Enabled = false;
            label11.Visible = false;
            label12.Enabled = false;
            label12.Visible = false;
            label13.Enabled = false;
            label13.Visible = false;
            label14.Enabled = false;
            label14.Visible = false;
            label15.Enabled = false;
            label15.Visible = false;
            label16.Enabled = false;
            label16.Visible = false;
            label17.Enabled = false;
            label17.Visible = false;
            label18.Enabled = false;
            label18.Visible = false;
            F_Throw.Enabled = false;
            F_Throw.Visible = false;
            Angle_Throw.Enabled = false;
            Angle_Throw.Visible = false;
            t_throw.Enabled = false;
            t_throw.Visible = false;
            f_Floor.Enabled = false;
            f_Floor.Visible = false;
            cw.Enabled = false;
            cw.Visible = false;
            p.Enabled = false;
            p.Visible = false;
            A.Enabled = false;
            A.Visible = false;
            v_old.Enabled = false;
            v_old.Visible = false;
            pos_old.Enabled = false;
            pos_old.Visible = false;
            step.Enabled = false;
            step.Visible = false;
            m.Enabled = false;
            m.Visible = false;
            g.Enabled = false;
            g.Visible = false;
            t.Enabled = false;
            t.Visible = false;
            t_max.Enabled = false;
            t_max.Visible = false;
            filePath.Enabled = false;
            filePath.Visible = false;
            divider0.Enabled = false;
            divider0.Visible = false;
            divider1.Enabled = false;
            divider1.Visible = false;
            bouncefac.Enabled = false;
            bouncefac.Visible = false;
            button1.Enabled = false;
            button1.Visible = false;
        }

        #region Visualisation
        int[,] loadMap(string FilePath)
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
                int[,] final = new int[height, width];
                for (int y = 0; y < final.GetLength(0); y++)
                {
                    for (int x = 0; x < final.GetLength(1); x++)
                    {
                        if (splittedb.Length > y && splittedb[y].Length > x && splittedb[y][x].StartsWith("Just "))
                        {
                            final[y, x] = Convert.ToInt32(splittedb[y][x].Substring(5));
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

        double[] stringtoDoubles(string content)
        {
            string[] splitted = content.Split(',');
            return new double[2] { Convert.ToDouble(splitted[0]), Convert.ToDouble(splitted[1]) };
        }

        List<double[]> getsimulatedPossesFromFile(string FilePath)
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

        List<string> ToListByBrackets(List<char> listi)
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

        List<int> getIndexesOfX(List<char> listi, char x)
        {
            char[] arri = listi.ToArray();
            List<int> ints = new List<int>();
            for (int i = 0; i < arri.Length; i++)
            {
                if (arri[i] == x) ints.Add(i);
            }
            return ints;
        }
        #endregion
        #region Map_editing
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!visualisation && !data_edit)
            {
                marks.Add(new float[2] { e.X, e.Y });
                marked = !marked;
                Refresh();
            }
        }

        void actFeld(float[] firstt, float[] secondt)
        {
            float[] first = new float[2] { firstt[0], height - firstt[1] }, second = new float[2] { secondt[0], height - secondt[1] };
            float mg = (second[1] - first[1]) / (second[0] - first[0]);
            int a = (int)(ToDegree(Math.Atan((second[1] - first[1]) / (second[0] - first[0]))));
            while (a >= 360) a -= 360;
            while (a < 0) a += 360;
            if (first[0] < second[0])
                for (int x = (int)first[0]; x < second[0]; x++)
                {
                    Feld[(int)(mg * (x - first[0]) + first[1]) / nauflösung, x / nauflösung] = a;
                }
            else
                for (int x = (int)first[0]; x > second[0]; x--)
                {
                    Feld[(int)(mg * (x - second[0]) + second[1]) / nauflösung, x / nauflösung] = a;
                }
            if (first[1] < second[1])
                for (int y = (int)first[1]; y < second[1] && a > 0; y++)
                {
                    Feld[y / nauflösung, (int)((y - first[1]) / mg + first[0]) / nauflösung] = a;
                }
            else
                for (int y = (int)first[1]; y > second[1] && a > 0; y--)
                {
                    Feld[y / nauflösung, (int)((y - second[1]) / mg + second[0]) / nauflösung] = a;
                }
        }

        string getcontents(int[,] map)
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

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!data_edit)
                if (e.KeyChar == (char)13)
                {
                    if (!File.Exists("map.txt")) File.Create("map.txt");
                    File.WriteAllText("map.txt", getcontents(umdrehen(Feld)));
                }
                else if (e.KeyChar == (char)32)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "Haskell_calcs.exe";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit();
                    p.Close();
                    visualisation = true;
                    data_edit = false;
                    disable_dataStuff();
                    data = getsimulatedPossesFromFile("end.txt").ToArray();
                    Refresh();
                }
                else if (e.KeyChar == 'e')
                {
                    data_edit = true;
                    Refresh();
                    edit_data("data.txt");
                }
        }

        int[,] umdrehen(int[,] Feld)
        {
            int[,] newFeld = new int[Feld.GetLength(0), Feld.GetLength(1)];
            for (int i = 0; i < Feld.GetLength(1); i++)
            {
                for (int u = Feld.GetLength(0) - 1; u > 0; u--)
                {
                    newFeld[(Feld.GetLength(0) - 1) - u, i] = Feld[u, i];
                }
            }
            return newFeld;
        }

        double ToDegree(double radian)
        {
            return radian * 180 / PI;
        }
        #endregion;
        #region useless actions
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            data_edit = false;
            visualisation = true;
            disable_dataStuff();
            Refresh();
            pos = data.GetLength(0) - 6;
            string content = File.ReadAllText("data.txt");
            string[] splitted = content.Split(':');
            string[] newdata = new string[splitted.Length];
            TextBox[] boxes = new TextBox[] { F_Throw, Angle_Throw, t_throw, f_Floor, cw, p, A, v_old, pos_old, step, m, g, t, t_max, filePath, divider0, divider1, bouncefac };
            for (int i = 0; i < boxes.Length; i++)
            {
                newdata[i] = boxes[i].Text;
            }
            string final = "";
            for (int i = 0; i < newdata.Length - 1; i++)
            {
                final += newdata[i] + ":";
            }
            File.WriteAllText("data.txt", final + newdata.Last());
            Process p2 = new Process();
            p2.StartInfo.FileName = "Haskell_calcs.exe";
            p2.StartInfo.CreateNoWindow = true;
            p2.Start();
            p2.WaitForExit();
            p2.Close();
            data = getsimulatedPossesFromFile("end.txt").ToArray();
            Refresh();
        }
        void edit_data(string file_Path)
        {
            string content = File.ReadAllText(file_Path);
            string[] splitted = content.Split(':');
            TextBox[] boxes = new TextBox[] { F_Throw, Angle_Throw, t_throw, f_Floor, cw, p, A, v_old, pos_old, step, m, g, t, t_max, filePath, divider0, divider1, bouncefac };
            for (int i = 0; i < boxes.Length && i < splitted.Length; i++)
            {
                boxes[i].Text = splitted[i];
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(data_edit)
            {
                data_edit = false;
                disable_dataStuff();
                Refresh();
            }
            else if(visualisation)
            {
                visualisation = false;
                Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            enable_dataStuff();
            data_edit = true;
            visualisation = false;
            Refresh();
            edit_data("data.txt");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            visualisation = true;
            if (data_edit)
            {
                data_edit = false;
                disable_dataStuff();
                Refresh();
            }
            Process p2 = new Process();
            p2.StartInfo.FileName = "Haskell_calcs.exe";
            p2.StartInfo.CreateNoWindow = true;
            p2.Start();
            p2.WaitForExit();
            data = getsimulatedPossesFromFile("end.txt").ToArray();
            pos = data.GetLength(0) - 6;
            Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            if (!File.Exists("map.txt")) File.Create("map.txt");
            File.WriteAllText("map.txt", getcontents(umdrehen(Feld)));
            button5.Enabled = true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            label19.Text = (e.X/fac).ToString() + " " + ((height-e.Y)/fac).ToString();
        }
    }
}
