using Corrections;
using SignalNetwork.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static SignalNetwork.Signals.AsyncSeries;

namespace Corrections
{
    public static class Tides
    {
        /// <summary>
        /// Constants
        /// </summary>

        /// <summary>
        /// Графитационная константа в Гал
        /// </summary>
        static readonly double GravitationConstant = 6.67e-08;
        /// <summary>
        /// Mасса Луны  в граммах
        /// </summary>
        static readonly double MoonMass = 7.3537e+25;
        /// <summary>
        /// Масса Солнца в граммах
        /// </summary>
        static readonly double SunMass = 1.993e+33;
        /// <summary>
        /// Расстояние между Солнцем и Луной в см
        /// </summary>
        static readonly double EarthSunDistance = 1.495e+13;
        /// <summary>
        /// Расстояние между Землёй и Луной в см
        /// </summary>
        static readonly double EarthMoonDistance = 3.84402e+10;
        /// <summary>
        /// Средний радиус Земли в см
        /// </summary>
        static readonly double EarthRadius = 6.37827e+08;
        /// <summary>
        /// Числа Лява
        /// </summary>
        static readonly double LoveNumberH = 0.612;
        static readonly double LoveNumberK = 0.303;
        /// <summary>
        /// Большая полуось Земного эллипсоида в см
        /// </summary>
        static readonly double EarthMAxis = 637813600;
        /// <summary>
        /// Квадрат эксцентриситета Земного элипсоида
        /// </summary>
        static readonly double SqrEarthEccentricity = 0.0066943662;
        /// <summary>
        /// Eccentricity of the moon's orbit
        /// </summary>
        static readonly double MoonEccentricity = 0.0549;
        /// <summary>
        /// Ratio of mean motion of the sun to that of the moon
        /// </summary>
        static readonly double ZeroPointCorrection = 0.074804;

        /// <summary>
        /// Расчёт приливных
        /// </summary>
        public static double Calculate(double lat, double lon, double alt, DateTime timeStamp)
        {
            double declinationMoon = 0;
            double hourСircleMoon = 0;
            double declinationSun = 0, hourСircleSun = 0;
            var mjdUT = AstroTime.ToMJDUT(timeStamp.ToUniversalTime());
            double t = AstroTime.ToDateSinceJ2000Epoch(mjdUT);
            Moon.MiniModel(t, out hourСircleMoon, out declinationMoon);
            Sun.MiniModel(t, out hourСircleSun, out declinationSun);
            t = AstroTime.ToGMST(mjdUT) + AngleTransform.ConvertToRadians(lon);
            //Получить косинус зенитного расстояние солнца
            var cosSm = CosGeocentricСolatitude(AngleTransform.ConvertToRadians(lat), declinationSun, t - hourСircleSun);
            //Получить косинус зенитного расстояние луны
            var cosZm = CosGeocentricСolatitude(AngleTransform.ConvertToRadians(lat), declinationMoon, t - hourСircleMoon);
            //var r = TorgeMethod(GeoPosition);
            //Расстояние до центра Земли от текущей точки
            var r = EarthMAxis * (1 - 0.5 * SqrEarthEccentricity * Math.Pow(Math.Sin(AngleTransform.ConvertToRadians(lat)), 2)) + alt * 100;
            double sl = new Polynom(new double[] { 4.720023438, 8399.7093, 0.0000440695, 0.0000000329 }).GetValue(t);
            double pl = new Polynom(new double[] { 5.835124721, 71.01800936, -0.0001805446, -0.0000002181 }).GetValue(t);
            double hl = new Polynom(new double[] { 4.881627934, 628.3319508, 0.0000052796, 0 }).GetValue(t);
            double el1 = new Polynom(new double[] { 0.01675104, -0.0000418, 0.000000126, 0 }).GetValue(t);
            double pl1 = new Polynom(new double[] { 4.908229467, 0.0300052641, 7.9024e-06, 0.0000000581 }).GetValue(t);
            double ap = 2.60930776e-11;
            double ap1 = 1 / (1.495e+13 * (1 - el1 * el1));
            double dl = 1 / (1 / EarthMoonDistance
                + ap * MoonEccentricity * Math.Cos(sl - pl)
                + ap * Math.Pow(MoonEccentricity, 2) * Math.Cos(2 * (sl - pl))
                + 1.875 * ap * ZeroPointCorrection * MoonEccentricity * Math.Cos(sl - 2 * hl + pl)
                + ap * Math.Pow(ZeroPointCorrection, 2) * Math.Cos(2 * (sl - hl)));
            double D = 1 / (1 / EarthSunDistance
                + ap1 * el1 * Math.Cos(hl - pl1));
            //Лунные
            var gm = -MoonMass * GravitationConstant * r / Math.Pow(dl, 3) * ((3 * Math.Pow(cosZm, 2) - 1) - 1.5 * r / dl * (5 * Math.Pow(cosZm, 3) - 3 * cosZm));
            //Солнечные
            var gs = -SunMass * GravitationConstant * r / Math.Pow(D, 3) * (3 * Math.Pow(cosSm, 2) - 1);
            var dgMoon = 0.05483;
            var dgSun = 0.02527;
            var k = 1.0;
            gm = dg(cosZm, dgMoon * k);
            gs = dg(cosSm, dgSun * k);
            //Учитывая что Земля упругая получаем ответ
            return (gm + gs) * (1 + LoveNumberH - 1.5 * LoveNumberK) * 1e3;
            //return (gm + gs);


        }

        private static double dg(double sinH, double k)
        {
            return (1 - 3 * Math.Pow(sinH, 2)) * k;
        }

        private static double CosGeocentricСolatitude(double latitude, double declination, double hourCircle)
        {
            return Math.Sin(latitude) * Math.Sin(declination) + Math.Cos(latitude) * Math.Cos(declination) * Math.Cos(hourCircle);
        }

        public static AsyncSeries Calculate(AsyncSeries series, double lat, double lon, double alt)
        {
            switch (series.Mode)
            {
                case SignalMode.Sample:
                    {
                        return new AsyncSeries(series.DataProvider.Where(item => item != null)
                            .Select(item =>
                            {
                                try
                                {
                                    var sample = (Sample)item;
                                    var result = Longman1959.Compute(sample.Timestamp, TimeZoneInfo.Local, lat, lon, alt);
                                    return new Sample(result, sample.Timestamp);
                                }
                                catch (Exception ex)
                                {
                                    return null;
                                }
                            }).Where(sample => sample != null), SignalMode.Sample, series.SampleRate, "Приливы в мкГал");
                        break;
                    }
                case SignalMode.SampleBuffer:
                    {
                        return new AsyncSeries(series.DataProvider.Where(item => item != null)
                            .Select(item =>
                            {
                                try
                                {
                                    var samples = (IList<Sample>)item;
                                    return samples.Select(sample => Corrections.Tides.Calculate(lat, lon, alt, sample.Timestamp));
                                }
                                catch (Exception ex)
                                {
                                    return null;
                                }
                            }).Where(samples => samples != null), SignalMode.SampleBuffer, series.SampleRate, "Приливы в мкГал");
                    }
            }
            return null;
        }
    }
}
