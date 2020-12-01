using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corrections
{
    internal class Matrix3x3
    {
        private double [][] matrixArray = new double[3][] { new double[3], new double[3], new double[3]};
        /// <summary>
        /// Получить матрицу поворота вокруг оси oX
        /// </summary>
        /// <param name="alpha">Угол поворота</param>
        /// <returns>Матрица поворота</returns>
        public static Matrix3x3 RotateX(double rotAngle)
        {
            double S = Math.Sin(rotAngle);
            double C = Math.Cos(rotAngle);

            Matrix3x3 U = new Matrix3x3();

            U.matrixArray[0][0] = 1.0; U.matrixArray[0][1] = 0.0; U.matrixArray[0][2] = 0.0;
            U.matrixArray[1][0] = 0.0; U.matrixArray[1][1] = +C;  U.matrixArray[1][2] = +S;
            U.matrixArray[2][0] = 0.0; U.matrixArray[2][1] = -S;  U.matrixArray[2][2] = +C;

            return U;

        }

        public  static Vector3D operator *(Matrix3x3 mat, Vector3D vec)
        {
            Vector3D res = new Vector3D();
            for (int i = 0; i < 3; i++)
            {
                double Scalp = 0.0;
                for (int j = 0; j < 3; j++)
                    Scalp += mat.matrixArray[i][j] * vec[j];
                res[i] = Scalp;
            }
            return res;
        }
    }
}
