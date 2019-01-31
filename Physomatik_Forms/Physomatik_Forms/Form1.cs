using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Physomatik_Forms
{
    public partial class Form1 : Form
    {
        private const double Pi = 3.14159265359;
        private static bool _marked;
        private static int[,] _field;
        private static readonly List<float[]> Marks = new List<float[]>();
        private static List<float[]> _vectors = new List<float[]>();
        private const int NResolution = 5;

        private static int _pos;
        private const int Fac = 5;
        private static double[][] _data;

        private static bool _visualisation;
        private static bool _dataEdit;

        private static int _height;
        private static int _width;

        private readonly Pen[] _colors =
        {
            Pens.LightGreen, Pens.YellowGreen, Pens.LimeGreen, Pens.Green, Pens.SeaGreen, Pens.Blue,
            Pens.BlueViolet, Pens.DarkRed, Pens.IndianRed, Pens.Red
        };

        public Form1()
        {
            InitializeComponent();
        }

        static Form1() => _width = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            _height = ActiveForm?.Size.Height ?? 0;
            _width = ActiveForm?.Size.Width ?? 0;
            _field = new int[_height / NResolution, _width / NResolution];
            for (var i = 0; i < _field.GetLength(0); i++)
            for (var u = 0; u < _field.GetLength(1); u++)
                _field[i, u] = -1;

            var customCulture =
                (System.Globalization.CultureInfo) System.Threading.Thread.CurrentThread
                    .CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            _vectors = LoadVectors("map.txt");
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!_dataEdit)
            {
                if (_vectors != null)
                    for (var i = 0; i < _vectors.Count - 1; i += 2)
                        e.Graphics.DrawLine(Pens.Black, _vectors.ElementAt(i)[0],
                            _vectors.ElementAt(i)[1], _vectors.ElementAt(i + 1)[0],
                            _vectors.ElementAt(i + 1)[1]);

                if (!_visualisation)
                {
                    button5.Enabled = true;
                    button5.Visible = true;
                    for (var i = 0; i < Marks.Count - 1; i += 2)
                        e.Graphics.DrawLine(Pens.Black, Marks.ElementAt(i)[0],
                            Marks.ElementAt(i)[1], Marks.ElementAt(i + 1)[0],
                            Marks.ElementAt(i + 1)[1]);

                    if (Marks.Count % 2 != 0)
                        e.Graphics.DrawEllipse(Pens.Black, Marks.Last()[0], Marks.Last()[1], 1, 1);
                    else if (Marks.Count > 0)
                        ActField(Marks.ElementAt(Marks.LastIndexOf(Marks.Last()) - 1),
                            Marks.Last());
                }
                else
                {
                    button5.Enabled = false;
                    button5.Visible = false;
                    _pos += 4;
                    e.Graphics.DrawString(_pos.ToString(),
                        new Font(FontFamily.GenericSansSerif, 10),
                        Brushes.Black, 100, 100);
                    if (_data == null)
                    {
                        _data = GetSimulatedPossesFromFile("end.txt").ToArray();
                        _pos = _data.GetLength(0) - 6;
                    }

                    if (!File.Exists("end.txt") || _pos >= _data.GetLength(0) - 1)
                        return;
                    for (var i = 0; i < _pos; i++)
                    {
                        e.Graphics.DrawLine(
                            _colors[_colors.Length * i / _pos],
                            Fac * (float) _data[i][0], _height - (float) (Fac * _data[i][1]),
                            Fac * (float) _data[i + 1][0],
                            _height - (float) (Fac * _data[i + 1][1]));
                        e.Graphics.DrawRectangle(
                            _colors[_colors.Length * i / _pos],
                            Fac * (float) _data[i][0], _height - (float) (Fac * _data[i][1]),
                            (float) Math.Sqrt(0.1) / 2, (float) Math.Sqrt(0.1) / 2);
                    }

                    e.Graphics.DrawString(
                        Fac * _data[_pos][0] + " " +
                        Fac * _data[_pos][1],
                        new Font(FontFamily.GenericMonospace, 10), Brushes.Black, 10, 10);
                }
            }
            else
            {
                if (!label1.Enabled) enable_dataStuff();
                button5.Enabled = false;
                button5.Visible = false;
            }
        }

        private void enable_dataStuff()
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

        private void disable_dataStuff()
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

        private static List<float[]> LoadVectors(string filePathVectors)
        {
            var final = new List<float[]>();
            if (!File.Exists(filePathVectors))
                return null;
            var content = File.ReadAllText(filePathVectors);
            if (content == "")
                return null;
            var splitted = content.Split(';');
            final.AddRange(splitted.Select(item => item.Split(',')).Select(temp => new[]
            {
                (float) Convert.ToDouble(temp[0]) * Fac,
                (float) (_height - Convert.ToDouble(temp[1]) * Fac)
            }));

            return final;
        }

        private static double[] StringToDoubles(string content)
        {
            var splitted = content.Split(',');
            return new[] {Convert.ToDouble(splitted[0]), Convert.ToDouble(splitted[1])};
        }

        private List<double[]> GetSimulatedPossesFromFile(string filePathVectors)
        {
            var listI = File.ReadAllText(filePathVectors).ToList();
            listI.Remove('[');
            listI.Remove(']');
            var tupleStrings = ToListByBrackets(listI);

            return tupleStrings.Select(item => StringToDoubles(item)).ToList();
        }

        private IEnumerable<string> ToListByBrackets(List<char> listI)
        {
            var open = GetIndexesOfX(listI, '(').ToList();
            var close = GetIndexesOfX(listI, ')').ToList();
            while (listI.Count > 0 && close.Count > 0 && open.Count > 0)
            {
                yield return string.Join("",
                    listI.GetRange(open.First() + 1, close.First() - open.First() - 1));
                open.Remove(open.First());
                close.Remove(close.First());
            }
        }

        private static IEnumerable<int> GetIndexesOfX(List<char> listI, char x)
        {
            var arri = listI.ToArray();
            for (var i = 0; i < arri.Length; i++)
            {
                if (arri[i] == x)
                    yield return i;
            }
        }

        #endregion

        #region Map_editing

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_visualisation || _dataEdit)
                return;
            Marks.Add(new float[] {e.X, e.Y});
            _marked = !_marked;
            Refresh();
        }

        private void ActField(IReadOnlyList<float> firstT, IReadOnlyList<float> secondT)
        {
            float[] first = {firstT[0], _height - firstT[1]},
                second = {secondT[0], _height - secondT[1]};
            var mg = (second[1] - first[1]) / (second[0] - first[0]);
            var a = (int) (ToDegree(Math.Atan(mg)));
            while (a >= 360) a -= 360;
            while (a < 0) a += 360;
            if (first[0] < second[0])
                for (var x = (int) first[0]; x < second[0]; x++)
                    _field[(int) (mg * (x - first[0]) + first[1]) / NResolution, x / NResolution] =
                        a;
            else
                for (var x = (int) first[0]; x > second[0]; x--)
                    _field[(int) (mg * (x - second[0]) + second[1]) / NResolution,
                        x / NResolution] = a;

            if (first[1] < second[1])
                for (var y = (int) first[1]; y < second[1] && a > 0; y++)
                    _field[y / NResolution, (int) ((y - first[1]) / mg + first[0]) / NResolution] =
                        a;
            else
                for (var y = (int) first[1]; y > second[1] && a > 0; y--)
                    _field[y / NResolution,
                        (int) ((y - second[1]) / mg + second[0]) / NResolution] = a;
        }

        private static string GetContents(int[,] map)
        {
            var content = "";
            for (var y = 0; y < map.GetLength(0) - 1; y++)
            {
                for (var x = 0; x < map.GetLength(1) - 1; x++)
                {
                    content += map[y, x] >= 0 ? "Just " + map[y, x] : "Nothing";
                    content += ",";
                }

                content += map[y, map.GetLength(1) - 1] >= 0
                    ? "Just " + map[y, map.GetLength(1) - 1]
                    : "Nothing";
                content += ";";
            }

            for (var x = 0; x < map.GetLength(1) - 1; x++)
            {
                content += map[map.GetLength(0) - 1, x] >= 0
                    ? "Just " + map[map.GetLength(0) - 1, x]
                    : "Nothing";
                content += ",";
            }

            content += map[map.GetLength(0) - 1, map.GetLength(1) - 1] >= 0
                ? "Just " + map[map.GetLength(0) - 1, map.GetLength(1) - 1]
                : "Nothing";

            return content;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_dataEdit)
                return;
            switch (e.KeyChar)
            {
                case (char) 13:
                {
                    if (!File.Exists("map.txt"))
                        File.Create("map.txt");
                    File.WriteAllText("map.txt", GetContents(TurnAround(_field)));
                    break;
                }
                case (char) 32:
                {
                    var process = new Process
                    {
                        StartInfo = {FileName = "Haskell_calcs.exe", CreateNoWindow = true}
                    };
                    process.Start();
                    process.WaitForExit();
                    process.Close();
                    _visualisation = true;
                    _dataEdit = false;
                    disable_dataStuff();
                    _data = GetSimulatedPossesFromFile("end.txt").ToArray();
                    Refresh();
                    break;
                }
                case 'e':
                    _dataEdit = true;
                    Refresh();
                    edit_data("data.txt");
                    break;
                default:
                    Console.WriteLine(@"Unknown");
                    break;
            }
        }

        private static int[,] TurnAround(int[,] field)
        {
            var newField = new int[field.GetLength(0), field.GetLength(1)];
            for (var i = 0; i < field.GetLength(1); i++)
            for (var u = field.GetLength(0) - 1; u > 0; u--)
                newField[field.GetLength(0) - 1 - u, i] = field[u, i];

            return newField;
        }

        static double ToDegree(double radian) => radian * 180 / Pi;

        #endregion;

        #region useless actions

        // ReSharper disable once UnusedMember.Local
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
            var content = File.ReadAllText("data.txt");
            var splitted = content.Split(':');
            var newData = new string[splitted.Length];
            var boxes = new[]
            {
                F_Throw, Angle_Throw, t_throw, f_Floor, cw, p, A, v_old, pos_old, step, m, g, t,
                t_max, filePath, divider0, divider1, bouncefac
            };
            for (var i = 0; i < boxes.Length; i++)
                newData[i] = boxes[i].Text;

            var final = "";
            for (var i = 0; i < newData.Length - 1; i++)
                final += newData[i] + ":";

            File.WriteAllText("data.txt", final + newData.Last());
        }

        private void edit_data(string filePathData)
        {
            var content = File.ReadAllText(filePathData);
            var splitted = content.Split(':');
            var boxes = new[]
            {
                F_Throw, Angle_Throw, t_throw, f_Floor, cw, p, A, v_old, pos_old, step, m, g, t,
                t_max, filePath, divider0, divider1, bouncefac
            };
            for (var i = 0; i < boxes.Length && i < splitted.Length; i++)
                boxes[i].Text = splitted[i];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_dataEdit)
            {
                _dataEdit = false;
                disable_dataStuff();
                Refresh();
            }
            else if (_visualisation)
            {
                _visualisation = false;
                Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            enable_dataStuff();
            _dataEdit = true;
            _visualisation = false;
            Refresh();
            edit_data("data.txt");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_dataEdit || label1.Enabled)
            {
                disable_dataStuff();
                _dataEdit = false;
            }

            _visualisation = true;
            var p2 = new Process
            {
                StartInfo = {FileName = "Haskell_calcs.exe", CreateNoWindow = true}
            };
            p2.Start();
            p2.WaitForExit();
            _data = GetSimulatedPossesFromFile("end.txt").ToArray();
            _pos = _data.GetLength(0) - 6;
            Refresh();
            disable_dataStuff();
        }

        private string GetVectorData(IEnumerable<float[]> marks)
        {
            var final = marks.Aggregate("",
                (current, item) =>
                    current + "" + item[0] / Fac + "," + (_height - item[1]) / Fac + ";");

            return WithoutLast(final);
        }

        private static string WithoutLast(string a)
        {
            var b = a.ToCharArray();
            if (b.Length <= 0)
                return a;
            var c = new char[a.Length - 1];
            for (var i = 0; i < c.Length; i++)
                c[i] = b[i];

            return c.Aggregate("", (current, item) => current + ("" + item));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            if (!File.Exists("map.txt")) File.Create("map.txt");
            File.WriteAllText("map.txt", GetVectorData(Marks));
            button5.Enabled = true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e) =>
            label19.Text = e.X / Fac + @" " + (_height - e.Y) / Fac;

        private void button6_Click(object sender, EventArgs e) => Refresh();
    }
}