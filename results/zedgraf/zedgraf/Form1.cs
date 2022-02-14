using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace zedgraf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            zedGraphControl1.GraphPane.CurveList.Clear();
            PointPairList type11 = new PointPairList();
            PointPairList type12 = new PointPairList();
            PointPairList type2 = new PointPairList();

            PointPairList type31 = new PointPairList();
            PointPairList type32 = new PointPairList();

            System.IO.StreamReader textFile1 = new System.IO.StreamReader(@"C:\...\tmp\" + "1Stooge2000" + ".txt");
            System.IO.StreamReader textFile2 = new System.IO.StreamReader(@"C:\...\tmp\" + "2Stooge2000" + ".txt");

            int n = 1000; // 

            int[] garbage11 = new int[n];
            int[] garbage12 = new int[n];

            for (int number = 10; number < n; number++)
            {
                double y = Convert.ToDouble(textFile1.ReadLine());
                garbage11[number] = Convert.ToInt32(y);
                type11.Add(number, y);
            }
            for (int number = 10; number < n; number++)
            {
                double y = Convert.ToDouble(textFile2.ReadLine());
                garbage12[number] = Convert.ToInt32(y);
                type12.Add(number, y);
            }

            double[] garbage2 = filtration.LeastSquareMethod3(garbage12);
            double[] garbage3 = filtration.NoiseCalculation(garbage12);

            for (int number = 0; number < n; number++)
            {
                double y = garbage2[0] * Math.Pow(number, 2) + garbage2[1] * number + garbage2[2];
                type2.Add(number, y);
                type31.Add(number, y - garbage3[0]);
                type32.Add(number, y + garbage3[0]);
            }



            Console.WriteLine();
            Console.WriteLine(garbage2[0]);
            Console.WriteLine();
            Console.WriteLine(garbage2[1]);
            Console.WriteLine();
            Console.WriteLine(garbage2[2]);
            Console.WriteLine();
            Console.WriteLine(garbage2[3]);

            Console.WriteLine();
            Console.WriteLine(garbage3[0]);
            Console.WriteLine();
            Console.WriteLine(garbage3[1]);


            LineItem Qline11 = zedGraphControl1.GraphPane.AddCurve("Тип 1", type11, Color.Gray, SymbolType.None);
            LineItem Qline12 = zedGraphControl1.GraphPane.AddCurve("Тип 1", type12, Color.Black, SymbolType.None);
            LineItem Qline2 = zedGraphControl1.GraphPane.AddCurve("Тип 2", type2, Color.Red, SymbolType.None);
            LineItem Qline31 = zedGraphControl1.GraphPane.AddCurve("Тип 3", type31, Color.Green, SymbolType.None);
            LineItem Qline32 = zedGraphControl1.GraphPane.AddCurve("Тип 3", type32, Color.Green, SymbolType.None);
            Qline11.Line.Width = 2;
            Qline12.Line.Width = 2;
            Qline2.Line.Width = 2;
            Qline31.Line.Width = 2;
            Qline32.Line.Width = 2;
            Qline11.Symbol.IsVisible = false;

            zedGraphControl1.RestoreScale(zedGraphControl1.GraphPane);



        }
    }
}
