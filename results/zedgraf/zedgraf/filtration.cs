using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//zedGraphControl1.GraphPane.CurveList.Clear();
//PointPairList type1 = new PointPairList();
//PointPairList type2 = new PointPairList();
//PointPairList type3 = new PointPairList();

//PointPairList type411 = new PointPairList();
//PointPairList type412 = new PointPairList();
//PointPairList type421 = new PointPairList();
//PointPairList type422 = new PointPairList();

//Random random = new Random();
//int number = 1000;

//int[] garbage1 = new int[number];
//double a = 0.45;
//double b = 5;

//int v1 = 10;
//int v2 = 10;
//int v3 = 100;

//for (int x = 0; x < number; x++)
//{
//    double y = a * x + b;
//    type1.Add(x, y);
//    y += random.Next(-1 * v1, v1);
//    if (random.Next(0, v2) == 0)
//    {
//        y += random.Next(-1 * v3, v3);
//    }
//    type2.Add(x, y);
//    garbage1[x] = Convert.ToInt32(y);

//    Console.WriteLine(x);
//}

//double[] garbage2 = filtration.AlphaBetaFilter(garbage1);

//for (int x = 0; x < number - 10; x++)
//{
//    type3.Add(x, garbage2[x]);

//    //double y = a * x + b;
//    //type411.Add(x, y + garbage2[1]);
//    //type412.Add(x, y - garbage2[1]);

//    //type421.Add(x, y + garbage2[0]);
//    //type422.Add(x, y - garbage2[0]);
//}

////Console.WriteLine();
////Console.WriteLine(garbage2[0]);
////Console.WriteLine(garbage2[1]);



//LineItem Qline1 = zedGraphControl1.GraphPane.AddCurve("Идеальный", type1, Color.Black, SymbolType.None);
//LineItem Qline2 = zedGraphControl1.GraphPane.AddCurve("Зашумленый", type2, Color.Gray, SymbolType.None);
//LineItem Qline3 = zedGraphControl1.GraphPane.AddCurve("фильтр", type3, Color.Red, SymbolType.None);
////LineItem Qline411 = zedGraphControl1.GraphPane.AddCurve("фильтр", type411, Color.Red, SymbolType.None);
////LineItem Qline412 = zedGraphControl1.GraphPane.AddCurve("фильтр", type412, Color.Red, SymbolType.None);
////LineItem Qline421 = zedGraphControl1.GraphPane.AddCurve("фильтр", type421, Color.Red, SymbolType.None);
////LineItem Qline422 = zedGraphControl1.GraphPane.AddCurve("фильтр", type422, Color.Red, SymbolType.None);
//Qline1.Line.Width = 2;
//Qline2.Line.Width = 2;
//Qline3.Line.Width = 2;
//Qline1.Symbol.IsVisible = false;

//zedGraphControl1.RestoreScale(zedGraphControl1.GraphPane);

namespace zedgraf
{
    public class Matrix
    {
        public double[,] Args { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        public Matrix(double[] x)
        {
            Row = x.Length;
            Col = 1;
            Args = new double[Row, Col];
            for (int i = 0; i < Args.GetLength(0); i++)
                for (int j = 0; j < Args.GetLength(1); j++)
                    Args[i, j] = x[i];
        }

        public Matrix(double[,] x)
        {
            Row = x.GetLength(0);
            Col = x.GetLength(1);
            Args = new double[Row, Col];
            for (int i = 0; i < Args.GetLength(0); i++)
                for (int j = 0; j < Args.GetLength(1); j++)
                    Args[i, j] = x[i, j];
        }

        public Matrix(Matrix other)
        {
            this.Row = other.Row;
            this.Col = other.Col;
            Args = new double[Row, Col];
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Col; j++)
                    this.Args[i, j] = other.Args[i, j];
        }

        public override string ToString()
        {
            string s = string.Empty;
            for (int i = 0; i < Args.GetLength(0); i++)
            {
                for (int j = 0; j < Args.GetLength(1); j++)
                {
                    s += string.Format("{0} ", Args[i, j]);
                }
                s += "\n";
            }
            return s;
        }

        public Matrix Transposition()
        {
            double[,] t = new double[Col, Row];
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Col; j++)
                    t[j, i] = Args[i, j];
            return new Matrix(t);
        }

        public static Matrix operator *(Matrix m, double k)
        {
            Matrix ans = new Matrix(m);
            for (int i = 0; i < ans.Row; i++)
                for (int j = 0; j < ans.Col; j++)
                    ans.Args[i, j] = m.Args[i, j] * k;
            return ans;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Col != m2.Row) throw new ArgumentException("Multiplication of these two matrices can't be done!");
            double[,] ans = new double[m1.Row, m2.Col];
            for (int i = 0; i < m1.Row; i++)
            {
                for (int j = 0; j < m2.Col; j++)
                {
                    for (int k = 0; k < m2.Row; k++)
                    {
                        ans[i, j] += m1.Args[i, k] * m2.Args[k, j];
                    }
                }
            }
            return new Matrix(ans);
        }

        private Matrix getMinor(int row, int column)
        {
            if (Row != Col) throw new ArgumentException("Matrix should be square!");
            double[,] minor = new double[Row - 1, Col - 1];
            for (int i = 0; i < this.Row; i++)
            {
                for (int j = 0; j < this.Col; j++)
                {
                    if ((i != row) || (j != column))
                    {
                        if (i > row && j < column) minor[i - 1, j] = this.Args[i, j];
                        if (i < row && j > column) minor[i, j - 1] = this.Args[i, j];
                        if (i > row && j > column) minor[i - 1, j - 1] = this.Args[i, j];
                        if (i < row && j < column) minor[i, j] = this.Args[i, j];
                    }
                }
            }
            return new Matrix(minor);
        }

        public static double Determ(Matrix m)
        {
            if (m.Row != m.Col) throw new ArgumentException("Matrix should be square!");
            double det = 0;
            int length = m.Row;

            if (length == 1) det = m.Args[0, 0];
            if (length == 2) det = m.Args[0, 0] * m.Args[1, 1] - m.Args[0, 1] * m.Args[1, 0];

            if (length > 2)
                for (int i = 0; i < m.Col; i++)
                    det += Math.Pow(-1, 0 + i) * m.Args[0, i] * Determ(m.getMinor(0, i));

            return det;
        }

        public Matrix MinorMatrix()
        {
            double[,] ans = new double[Row, Col];

            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Col; j++)
                    ans[i, j] = Math.Pow(-1, i + j) * Determ(this.getMinor(i, j));

            return new Matrix(ans);
        }

        public Matrix InverseMatrix()
        {
            if (Math.Abs(Determ(this)) <= 0.000000001) throw new ArgumentException("Inverse matrix does not exist!");

            double k = 1 / Determ(this);

            Matrix minorMatrix = this.MinorMatrix();

            return minorMatrix * k;
        }
    }

    public class LSM
    {
        // Массивы значений Х и У задаются как свойства
        public double[] X { get; set; }
        public double[] Y { get; set; }

        // Искомые коэффициенты полинома в данном случае, а в общем коэфф. при функциях
        private double[] coeff;
        public double[] Coeff { get { return coeff; } }

        // Среднеквадратичное отклонение
        public double? Delta { get { return getDelta(); } }

        // Конструктор класса. Примает 2 массива значений х и у
        // Длина массивов должна быть одинакова, иначе нужно обработать исключение
        public LSM(double[] x, double[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException("X and Y arrays should be equal!");
            X = new double[x.Length];
            Y = new double[y.Length];

            for (int i = 0; i < x.Length; i++)
            {
                X[i] = x[i];
                Y[i] = y[i];
            }
        }

        // Собственно, Метод Наименьших Квадратов
        // В качестве базисных функций используются степенные функции y = a0 * x^0 + a1 * x^1 + ... + am * x^m
        public void Polynomial(int m)
        {
            if (m <= 0) throw new ArgumentException("Порядок полинома должен быть больше 0");
            if (m >= X.Length) throw new ArgumentException("Порядок полинома должен быть на много меньше количества точек!");

            // массив для хранения значений базисных функций
            double[,] basic = new double[X.Length, m + 1];

            // заполнение массива для базисных функций
            for (int i = 0; i < basic.GetLength(0); i++)
                for (int j = 0; j < basic.GetLength(1); j++)
                    basic[i, j] = Math.Pow(X[i], j);

            // Создание матрицы из массива значений базисных функций(МЗБФ)
            Matrix basicFuncMatr = new Matrix(basic);

            // Транспонирование МЗБФ
            Matrix transBasicFuncMatr = basicFuncMatr.Transposition();

            // Произведение транспонированного  МЗБФ на МЗБФ
            Matrix lambda = transBasicFuncMatr * basicFuncMatr;

            // Произведение транспонированого МЗБФ на следящую матрицу 
            Matrix beta = transBasicFuncMatr * new Matrix(Y);

            // Решение СЛАУ путем умножения обратной матрицы лямбда на бету
            Matrix a = lambda.InverseMatrix() * beta;

            // Присвоение значения полю класса 
            coeff = new double[a.Row];
            for (int i = 0; i < coeff.Length; i++)
            {
                coeff[i] = a.Args[i, 0];
            }
        }

        // Функция нахождения среднеквадратичного отклонения
        private double? getDelta()
        {
            if (coeff == null) return null;
            double[] dif = new double[Y.Length];
            double[] f = new double[X.Length];
            for (int i = 0; i < X.Length; i++)
            {
                for (int j = 0; j < coeff.Length; j++)
                {
                    f[i] += coeff[j] * Math.Pow(X[i], j);
                }
                dif[i] = Math.Pow((f[i] - Y[i]), 2);
            }
            return Math.Sqrt(dif.Sum() / X.Length);
        }
    }
    
    class filtration
    {
        public static double[] NoiseCalculation(int[] array)
        {
            double noise = 0;
            double release = 0;
            int number1 = 0;
            int number2 = 0;

            double[] indicators = new double[2];

            for (int i = 1; i < array.Length; i++)
            {
                if (Math.Abs(array[i] - array[i - 1]) > (noise / number2) * 1.5)
                {
                    release += Math.Abs(array[i] - array[i - 1]);
                    number1++;
                }
                else 
                {
                    noise += Math.Abs(array[i] - array[i - 1]);
                    number2++;
                }
                
            }

            indicators[0] = release / number1;
            indicators[1] = noise / number2;

            return indicators;
        }

        //-------------- int --------------
        public static double[] Average(int[] array) 
        {
            int core = 10;
            double[] garbage = new double[array.Length - core];
            for (int i = 0; i < array.Length - core; i++)
            {
                int sum = 0;
                for (int j = i; j < i + core; j++)
                {
                    sum += array[j];
                }
                garbage[i] = sum / core;
            }
            return garbage;
        }
        public static double[] StretchedSelection(int[] array)
        {
            byte counter = 0;     // счётчик
            float prevResult = 0; // хранит предыдущее готовое значение
            float sum = 0;  // сумма
            int NUM_READ = 10;

            double[] garbage = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];   // суммируем новое значение
                counter++;       // счётчик++
                if (counter == NUM_READ)
                {      // достигли кол-ва измерений
                    prevResult = sum / NUM_READ;  // считаем среднее
                    sum = 0;                      // обнуляем сумму
                    counter = 0;                  // сброс счётчика
                }
                garbage[i] = prevResult;
            }

            return garbage;
        }
        public static double[] RunningAverage(int[] array)
        {
            int NUM_READ = 10;

            int t = 0;
            int[] vals = new int[NUM_READ];
            int average = 0;

            double[] garbage = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                if (++t >= NUM_READ) t = 0; // перемотка t
                average -= vals[t];         // вычитаем старое
                average += array[i];          // прибавляем новое
                vals[t] = array[i];           // запоминаем в массив
                garbage[i] = ((float)average / NUM_READ);
            }

            return garbage;
        }
        public static double[] ExponentialRunningAverage(int[] array)
        {
            float k = 0.03f;  // коэффициент фильтрации, 0.0-1.0
            float filVal = 0;

            double[] garbage = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                filVal += (array[i] - filVal) * k;
                garbage[i] = filVal;
            }

            return garbage;
        }
        public static double[] AdaptiveFactor(int[] array)
        {
            float filVal = 0;

            double[] garbage = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                float k;
                // резкость фильтра зависит от модуля разности значений
                if (Math.Abs(array[i] - filVal) > 1.5) {
                    k = 0.03f;
                }
                else 
                { 
                    k = 0.9f;
                }

                filVal += (array[i] - filVal) * k;
                garbage[i] = filVal;
            }

            return garbage;
        }
        public static double[] MedianFilter(int[] array)
        {
            int NUM_READ = 10;
            float[] buffer = new float[NUM_READ];
            byte count = 0;

            double[] garbage = new double[array.Length];

            for (int j = 0; j < array.Length; j++)
            {
                buffer[count] = array[j];
                if ((count < NUM_READ - 1) && (buffer[count] > buffer[count + 1])) {
                for (int i = count; i < NUM_READ - 1; i++)
                {
                    if (buffer[i] > buffer[i + 1])
                    {
                        float buff = buffer[i];
                        buffer[i] = buffer[i + 1];
                        buffer[i + 1] = buff;
                    }
                }
            } else
            {
                if ((count > 0) && (buffer[count - 1] > buffer[count])) {
                    for (int i = count; i > 0; i--)
                    {
                        if (buffer[i] < buffer[i - 1])
                        {
                            float buff = buffer[i];
                            buffer[i] = buffer[i - 1];
                            buffer[i - 1] = buff;
                        }
                    }
                }
            }
            if (++count >= NUM_READ) count = 0;
            garbage[j] = buffer[(int)NUM_READ / 2];
            }

            return garbage;
        }
        public static double[] LeastSquareMethod2(int[] array)
        {
            double[] garbage = new double[3];

            double[] x = new double[array.Length];
            double[] y = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                x[i] = Convert.ToDouble(i);
                y[i] = Convert.ToDouble(array[i]);
            }

            LSM myReg = new LSM(x, y);
            myReg.Polynomial(1);

            garbage[0] = myReg.Coeff[1];
            garbage[1] = myReg.Coeff[0];
            garbage[2] = Convert.ToDouble(myReg.Delta);

            return garbage;
        }
        public static double[] LeastSquareMethod3(int[] array)
        {
            double[] garbage = new double[4];

            double[] x = new double[array.Length];
            double[] y = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                x[i] = Convert.ToDouble(i);
                y[i] = Convert.ToDouble(array[i]);
            }

            LSM myReg = new LSM(x, y);
            myReg.Polynomial(2);

            garbage[0] = myReg.Coeff[2];
            garbage[1] = myReg.Coeff[1];
            garbage[2] = myReg.Coeff[0];
            garbage[3] = Convert.ToDouble(myReg.Delta);

            return garbage;
        }
        public static double[] SimpleKalman(int[] array)
        {
            float _err_measure = 10f;  // примерный шум измерений
            float _q = 1f;   // скорость изменения значений 0.001-1, варьировать самому

            float _err_estimate = _err_measure;
            float _last_estimate = 0;

            double[] garbage = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                float _kalman_gain, _current_estimate;

                _kalman_gain = (float)_err_estimate / (_err_estimate + _err_measure);
                _current_estimate = _last_estimate + (float)_kalman_gain * (array[i] - _last_estimate);
                _err_estimate = (float)((1.0 - _kalman_gain) * _err_estimate + (float)Math.Abs(_last_estimate - _current_estimate) * _q);
                _last_estimate = _current_estimate;
                garbage[i] = _current_estimate;
            }
            return garbage;
        }
        public static double[] SimpleKalman(int[] array, float _err_measure, float _q)
        {
            float _err_estimate = _err_measure;
            float _last_estimate = 0;

            double[] garbage = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                float _kalman_gain, _current_estimate;

                _kalman_gain = (float)_err_estimate / (_err_estimate + _err_measure);
                _current_estimate = _last_estimate + (float)_kalman_gain * (array[i] - _last_estimate);
                _err_estimate = (float)((1.0 - _kalman_gain) * _err_estimate + (float)Math.Abs(_last_estimate - _current_estimate) * _q);
                _last_estimate = _current_estimate;
                garbage[i] = _current_estimate;
            }
            return garbage;
        }
        public static double[] AlphaBetaFilter(int[] array)
        {
            // период дискретизации (измерений), process variation, noise variation
            float dt = 0.02f;
            float sigma_process = 3.0f;
            float sigma_noise = 0.7f;

             float xk_1 = 0, vk_1 = 0, a, b;
             float xk, vk, rk;
             float xm;

            double[] garbage = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                float lambda = (float)sigma_process * dt * dt / sigma_noise;
                float r = (4 + lambda - (float)Math.Sqrt(8 * lambda + lambda * lambda)) / 4;
                a = (float)1 - r * r;
                b = (float)2 * (2 - a) - 4 * (float)Math.Sqrt(1 - a);
                xm = array[i];
                xk = xk_1 + ((float)vk_1 * dt);
                vk = vk_1;
                rk = xm - xk;
                xk += (float)a * rk;
                vk += (float)(b * rk) / dt;
                xk_1 = xk;
                vk_1 = vk;

                garbage[i] = xk_1;
            }
            return garbage;
        }
    }
}
