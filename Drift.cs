using MathNet.Numerics;
using SignalNetwork.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using static SignalNetwork.Signals.AsyncSeries;

namespace Corrections
{
    public static class Drift
    {
        public static AsyncSeries Trend(AsyncSeries series, int order)
        {
            try
            {
                switch (series.Mode)
                {
                    case SignalMode.SampleBuffer:
                        {
                            return new AsyncSeries(series.DataProvider.Where(item => item != null).Select(item =>
                            {

                                try
                                {
                                    var samples = item as IList<Sample>;
                                    if (samples.Count < 1)
                                        return null;
                                    var firstSample = samples.First();
                                    var pol = Polynomial.Fit(samples.Where(sample => sample != null).Select(sample => (double)((sample.Timestamp - firstSample.Timestamp).TotalMilliseconds)).ToArray(),
                                        samples.Where(sample => sample != null).Select(sample => sample.Value).ToArray(), order);
                                    List<Sample> trend = new List<Sample>();
                                    for (int i = 0; i < samples.Count; i++)
                                    {
                                        var trendValue = pol.Evaluate((double)((samples[i].Timestamp - firstSample.Timestamp).TotalMilliseconds));
                                        trend.Add(new Sample(trendValue, samples[i].Timestamp));
                                    }
                                    return trend;
                                }
                                catch (Exception ex)
                                {
                                    return null;
                                }

                            }).Where(item => item != null), SignalMode.SampleBuffer, series.SampleRate, series.Name);
                        }
                    default:
                        return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public static AsyncSeries Correct(AsyncSeries series,int order)
        {
            try
            {
                switch (series.Mode)
                {
                    case SignalMode.SampleBuffer:
                        {
                            return new AsyncSeries(series.DataProvider.Where(item => item != null).Select(item =>
                            {

                                try
                                {
                                    var samples = item as IList<Sample>;
                                    if (samples.Count < 1)
                                        return null;
                                    var firstSample = samples.First();
                                    var pol = Polynomial.Fit(samples.Where(sample => sample != null).Select(sample => (double)((sample.Timestamp - firstSample.Timestamp).TotalMilliseconds)).ToArray(),
                                        samples.Where(sample => sample != null).Select(sample => sample.Value).ToArray(), order);
                                    List<Sample> driftCorrected = new List<Sample>();
                                    List<Sample> trend = new List<Sample>();
                                    for (int i = 0; i < samples.Count; i++)
                                    {
                                        var trendValue = pol.Evaluate((double)((samples[i].Timestamp - firstSample.Timestamp).TotalMilliseconds));
                                        trend.Add(new Sample(trendValue, samples[i].Timestamp));
                                        driftCorrected.Add(new Sample(samples[i].Value - trendValue,
                                            samples[i].Timestamp));
                                    }
                                    var count = driftCorrected.Where(t => Double.IsNaN(t.Value)).Count();
                                    return driftCorrected;
                                }
                                catch
                                {
                                    return null;
                                }

                            }).Where(item => item != null), SignalMode.SampleBuffer, series.SampleRate, series.Name);
                        }
                    default:
                        return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
