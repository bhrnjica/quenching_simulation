using FEMCommon.Entities;
using FEMCommon.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Interfaces
{

    public interface INode
    {
        public int Id { get; set; }
        public NodeType NT { get; set; }
        PointD P { get; set; }
        int GetDof();
    }


}
