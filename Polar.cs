using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corrections
{
    internal class Polar
    {
        public double phi { get; private set; }

        public double theta { get; private set; }

        public double radius { get; private set; }

        public enum Angle { phi, theta};

        public Polar(double phi, double theta, double radius = 1.0)
        {
            this.phi = phi;
            this.theta = theta;
            this.radius = radius;
        }
    }
}
