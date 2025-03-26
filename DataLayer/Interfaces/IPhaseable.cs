using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Provides the ability to calculate a phase between 0° and 360°
    /// </summary>
    public interface IPhaseable
    {
        /// <summary>
        /// The phase in degrees (angle formed between the object and the earth and the object and the sun.
        /// </summary>
        public double Phase { get; set; }
    }
}
