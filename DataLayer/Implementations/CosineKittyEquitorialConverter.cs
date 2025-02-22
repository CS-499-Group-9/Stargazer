using DataLayer.ConvertedObjects;
using DataLayer.Interfaces;
using DataLayer.RawObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Implementations
{
    public class CosineKittyEquitorialConverter : IEquitorialConverter
    {
        Func<RawCelestialBody, CelestialBody> IEquitorialConverter.Converter => (rawBody) =>
        {
            throw new NotImplementedException();
        };
     
            
    }
}
