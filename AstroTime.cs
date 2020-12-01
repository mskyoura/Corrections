using System;
using System.Collections.Generic;
using System.Text;

namespace Corrections
{
    static internal class AstroTime
    {
        public static readonly short ETFromUT = 60; //[сек]
        public static readonly short CETFromUT = 4;

        public static double ToJulianDate(DateTime curTime)
        {
            return curTime.ToOADate() + 2415018.5;
        }

        public static double ToMJDUT(DateTimeOffset curTime)
        {
            return curTime.DateTime.ToOADate() + 15018;
        }

        public static double ToDateSinceJ1900Epoch(double curMJDUT)
        {
            //var curMJDET = ToMjdET(curMJDUT);
            //var curJD = ToJulianDate(curTime);
            //var JD2000 = 2451545;
            var MJD1900 = 15019.5;
            return (curMJDUT - MJD1900) / 36525;
        }

        public static double ToDateSinceJ2000Epoch(double curMJDUT)
        {
            var curMJDET = ToMjdET(curMJDUT);
            //var curJD = ToJulianDate(curTime);
            //var JD2000 = 2451545;
            var MJD2000 = 51544.5;
            return (curMJDET - MJD2000) / 36525;
        }

        public static double StarCorrection(DateTimeOffset dateTime)
        {
            return ((dateTime.Hour * 60 + dateTime.Minute) * 60 + dateTime.Second) / 86400.0;

        }

        public static double ToMjdET(double MJDUt)
        {
            return MJDUt + ETFromUT / 86400.0;
        }

        public static double ToGMST(double MJD)
        {
            const double Secs = 86400.0;
            double MJD0, UT, t0, t, gmst;


            MJD0 = Math.Floor(MJD);
            UT = Secs * (MJD - MJD0);     // [s]
            t0 = (MJD0 - 51544.5) / 36525.0;
            t = (MJD - 51544.5) / 36525.0;

            gmst = 24110.54841 + 8640184.812866 * t0 + 1.0027379093 * UT
                    + (0.093104 - 6.2e-6 * t) * t * t;      // [sec]

            return (2 * Math.PI / Secs) * gmst % Secs;   // [Rad]
        }

        public static double Mjd(DateTime curTime)
        {
            var month = curTime.Month;
            var year = curTime.Year;
            var day = curTime.Day;
            if (month < 3) month += 12;
            if (month > 12) year -= 1;
            if (month > 13) day += 1;
            return (year * 365) + (year / 4) - (year / 100) + (year / 400)
                   + (489 * month) / 16
                   + day - 678973;
        }

        private static double Ddd(int D, int M, double S)
        {
            //
            // Variables
            //
            double sign;


            if ((D < 0) || (M < 0) || (S < 0)) sign = -1.0; else sign = 1.0;

            return sign * (Math.Abs(D) + Math.Abs(M) / 60.0 + Math.Abs(S) / 3600.0);
        }
    }
}
