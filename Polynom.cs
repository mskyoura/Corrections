using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corrections
{
    class Polynom
    {
            private List<double> _coefficientList = new List<double>();
            public Polynom(double[] coefficients)
            {
                for (var i = 0; i < coefficients.Count(); i++)
                    _coefficientList.Add(coefficients[i]);
            }
            public double GetValue(double point)
            {
                double value = 0;
                for (int i = 0; i < _coefficientList.Count; i++)
                    value += _coefficientList[i] * Math.Pow(point, i);
                return value;
            }
    }
}
