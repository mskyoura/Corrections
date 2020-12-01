using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corrections
{
    internal class Sun
    {
        private static readonly double Rad = Math.PI / 180; // градусов в радиане

        /// <summary>
        /// Расчёт прямого восхождение и склонения солнца с помощью аналитических  рядов
        /// </summary>
        /// <param name="offsetJ2000">Время по Джулианскому календарю c 2000 эпохи</param>
        /// <param name="rightAscension">Прямое восхождение солнца</param>
        /// <param name="declination">Склонение солнца</param>
        public static void MiniModel(double offsetJ2000, out double rightAscension, out double declination)
        {
            double eps = 23.43929111 * Rad;
            double L, M;
            Vector3D e_Sun;


            // Mean anomaly and ecliptic longitude  
            M = 2 * Math.PI * ((0.993133 + 99.997361 * offsetJ2000) % 1);
            L = 2 * Math.PI * ((0.7859453 + M / (2 * Math.PI) +
                              (6893.0 * Math.Sin(M) + 72.0 * Math.Sin(2.0 * M) + 6191.2 * offsetJ2000) / 1296.0e3) % 1);

            // Equatorial coordinates
            e_Sun = Matrix3x3.RotateX(-eps) * new Vector3D(new Polar(L, 0.0));

            rightAscension = e_Sun[Polar.Angle.phi];
            declination = e_Sun[Polar.Angle.theta];
        }
    }
}
