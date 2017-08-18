using Nexogen.Libraries.Metrics.Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Nexogen.Libraries.Metrics.Prometheus
{
    public sealed class AtomicDouble
    {
        private double value;
        public double Value
        {
            get
            {
                return Volatile.Read(ref this.value);
            }

            set
            {
                Volatile.Write(ref this.value, value);
            }
        }

        public void Add(double amount)
        {
            double res, old;
            do
            {
                old = value;
                res = Interlocked.CompareExchange(ref value, old + amount, old);
            } while (res != old);
        }

        /*
         *  represented as required by the Go strconv package (see functions ParseInt and ParseFloat).
         *  In particular, Nan, +Inf, and -Inf are valid values.
         */
        public override string ToString()
        {
            if (Double.IsNaN(value))
            {
                return "Nan";
            }
            else if (Double.IsNegativeInfinity(value))
            {
                return "-Inf";
            }
            else if (Double.IsPositiveInfinity(value))
            {
                return "+Inf";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
