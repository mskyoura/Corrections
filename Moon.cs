using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Corrections
{
    internal static class Moon
    {
        private static readonly double Rad =  Math.PI / 180; // градусов в радиане
        private static readonly double Arcs = 3600.0 * 180.0 / Math.PI; //радиан в арксекунде

        /// <summary>
        /// Расчёт прямого восхождение и склонения луны с помощью аналитических рядов
        /// </summary>
        /// <param name="offsetJ2000">Время по Джулианскому календарю c 2000 эпохи</param>
        /// <param name="rightAscension">Прямое восхождение луны в рад</param>
        /// <param name="declination">Склонение луны в рад</param>

        public static void MiniModel(double T, out double rightAscension, out double declination)
        {
            double eps = 23.43929111 * Rad;     
             
            double L0 = ((0.606433 + 1336.855225 * T) % 1); // Средняя долгота

            double l = 2 * Math.PI * ((0.374897 + 1325.552410 * T) % 1);// Средняя аномалия луны
            double ls = 2 * Math.PI * ((0.993133 + 99.997361 * T) % 1);  // Средняя аномалия солнца
            double D = 2 * Math.PI * ((0.827361 + 1236.853086 * T) % 1);  // Долготная разница между луной-солнцем 
            double F = 2 * Math.PI * ((0.259086 + 1342.227825 * T) % 1);   // Расстояния от восходящего узла орбиты

            // Возмущения широты и долготы
            double dL = +22640 * Math.Sin(l) - 4586 * Math.Sin(l - 2 * D) + 2370 * Math.Sin(2 * D)
                + 769 * Math.Sin(2 * l) - 668 * Math.Sin(ls) - 412 * Math.Sin(2 * F)
                - 212 * Math.Sin(2 * l - 2 * D) - 206 * Math.Sin(l + ls - 2 * D)
                + 192 * Math.Sin(l + 2 * D) - 165 * Math.Sin(ls - 2 * D) - 125 * Math.Sin(D)
                - 110 * Math.Sin(l + ls) + 148 * Math.Sin(l - ls) - 55 * Math.Sin(2 * F - 2 * D);
            double S = F + (dL + 412 * Math.Sin(2 * F) + 541 * Math.Sin(ls)) / Arcs;
            double h = F - 2 * D;
            double N = -526 * Math.Sin(h) + 44 * Math.Sin(l + h) - 31 * Math.Sin(-l + h) - 23 * Math.Sin(ls + h)
                 + 11 * Math.Sin(-ls + h) - 25 * Math.Sin(-2 * l + F) + 21 * Math.Sin(-l + F);

            // Эклиптическая широта и долгота
            double lMoon = 2 * Math.PI * ((L0 + dL / 1296.0e3) % 1);// [рад]
            double bMoon = (18520.0 * Math.Sin(S) + N) / Arcs;   // [рад]

            // Перевод в экваториальные координаты
            var eMoon = Matrix3x3.RotateX(-eps) * new Vector3D(new Polar(lMoon, bMoon));

            rightAscension = eMoon[Polar.Angle.phi];
            declination = eMoon[Polar.Angle.theta];
        }

    }
}
