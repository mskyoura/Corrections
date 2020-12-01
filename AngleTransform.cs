using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corrections
{
    internal static class AngleTransform
    {
        /// <summary>
        /// Пересчитать угол из градусов в радианы
        /// </summary>
        /// <param name="angleDeg">Угол в градусах</param>
        public static double ConvertToRadians(double angleDeg)
        {
            return angleDeg * Math.PI / 180;
        }

        /// <summary>
        /// Пересчитать угол из радианов в градусов
        /// </summary>
        /// <param name="angleRad">Угол в радианах</param>
        public static double ConvertToDegrees(double angleRad)
        {
            return angleRad * 180 / Math.PI;
        }

        /// <summary>
        /// Пересчитать угол из шестидесятиричной системы в десятичную
        /// </summary>
        /// <param name="deg">градусы</param>
        /// <param name="min">минуты</param>
        /// <param name="sec">секунды</param>
        public static double ToDecimalFormat(double deg, double min, double sec)
        {
            if (min >= 0 && min <= 60 && sec >= 0 && sec <= 60)
            {
                if (deg < 0)
                    return deg - min / 60 - sec / 3600;
                else
                    return deg + min / 60 + sec / 3600;
            }
            else
            {
                throw new FormatException();
            }
        }
    }
}
