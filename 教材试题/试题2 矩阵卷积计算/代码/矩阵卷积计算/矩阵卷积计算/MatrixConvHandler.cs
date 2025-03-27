namespace 矩阵卷积计算
{
    internal class MatrixConvHandler
    {
        public Matrix M { get; set; }
        public Matrix N { get; set; }
        public MatrixConvHandler(Matrix m, Matrix n)
        {
            M = m; N = n;
        }

        /// <summary>
        /// 计算两种算法结果
        /// </summary>
        /// <param name="cal1Result">算法1结果</param>
        /// <param name="cal2Result">算法2结果</param>
        public void Calculate(out Matrix cal1Result, out Matrix cal2Result)
        {
            double[,] v1 = new double[N.RowCount, N.ColCount], v2 = new double[N.RowCount, N.ColCount];
            for (int I = 0; I < N.RowCount; I++)
            {
                for (int J = 0; J < N.ColCount; J++)
                {
                    double num1 = 0, num2 = 0, den = 0;//算法1分子,算法2分子，分母
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (I - i - 1 < 0 ||
                                J - j - 1 < 0 ||
                                I - i - 1 > 9 ||
                                J - j - 1 > 9) continue;

                            num1 += M[i, j] * N[I - i - 1, J - j - 1];
                            num2 += M[i, j] * N[9 - (I - i - 1), 9 - (J - j - 1)];
                            den += M[i, j];
                        }
                    }
                    v1[I, J] = num1 / den;
                    v2[I, J] = num2 / den;
                }
            }
            cal1Result = new Matrix(v1);
            cal2Result = new Matrix(v2);
        }
    }
}
