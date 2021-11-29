using FEMCommon.Interfaces;

namespace FEM.Quenching
{

    /// <summary>
    /// Thermal material property specific for the experiment. conductivity and specific heat are dependent to the current temperature, 
    /// so they are expressed with in the function of temperature.Specific density are constant.
    /// The method GetData can receive the parameter 'value' which is in this case temperature. In case the value is null, the starting values will be returned.
    /// Also the order of the parameters are important and they should be in this order: kx,ky,rho,cp. 
    /// </summary>
    public class MHeat : IMData
    {
        double _kx;
        double _rho;
        double _cp;
        public MHeat(double kx, double rho, double cp)
        {
            _kx = kx;
            _rho = rho;
            _cp = cp;
        }

        /// <summary>
        /// Get mechanical properties based on the temperature values.When temp is null the properties are constant.
        /// </summary>
        /// <param name="temperatureValue">temperature values</param>
        /// <returns></returns>
        public double[] GetData(double? temperatureValue)
        {
            //order of quantities
            //kx,ky,rho,cp
            if (temperatureValue == null)
                return new double[4] { _kx, _kx, _rho, _cp };
            else
            {
                var l = -2E-6 * temperatureValue.Value * temperatureValue.Value + 0.0159 * temperatureValue.Value + 14.712;
                var cp = 6E-7 * temperatureValue.Value * temperatureValue.Value * temperatureValue.Value - 0.001 * temperatureValue.Value * temperatureValue.Value + 0.642 * temperatureValue.Value + 436.57;
                return new double[4] { l,l, _rho, cp };
            }

        }
    }
}
