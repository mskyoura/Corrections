using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corrections
{
    internal class Vector3D
    {
        private double[] array = new double[3];

        public double x1
        {
            get
            {
                return array[0];
            }
            set
            {
                array[0] = value;
            }
        }

        public double x2
        {
            get
            {
                return array[1];
            }
            set
            {
                array[1] = value;
            }
        }

        public double x3
        {
            get
            {
                return array[2];
            }
            set
            {
                array[2] = value;
            }
        }

        public Vector3D(Polar polar)
        {
            array[0] = polar.radius * Math.Cos(polar.phi) * Math.Cos(polar.theta);
            array[1] = polar.radius * Math.Sin(polar.phi) * Math.Cos(polar.theta);
            array[2] = polar.radius * Math.Sin(polar.theta);
        }

        public Vector3D() { }


        public double this[int key]
        {
            get
            {
                return array[key];
            }
            set
            {
                array[key] = value;
            }
        }
        public double this[Polar.Angle angle]
        {
            get
            {
                if (angle == Polar.Angle.phi)
                {
                    if ((array[0] == 0.0) && (array[1] == 0.0))
                        return 0;
                    else
                    {
                        double phi = Math.Atan2(array[1], array[0]);
                        if (phi < 0)
                            phi += 2.0 * Math.PI;
                        return phi;
                    }
                }
                else 
                {
                    double rhoSqr = array[0] * array[0] + array[1] * array[1];
                    double rho = Math.Sqrt(rhoSqr);
                    if ((array[2] == 0.0) && (rho == 0.0))
                        return 0;
                    else
                    {
                        double theta = Math.Atan2(array[2], rho);
                        return theta;
                    }
                        
                }
            }
        }
    }
}
