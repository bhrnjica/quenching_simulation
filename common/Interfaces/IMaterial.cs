using FEMCommon.Entities;
using FEMCommon.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Interfaces
{

    public interface IMData
    {
        double[] GetData(double? stateValue);
    }


}
