using FEMCommon.Interfaces;
using FEMCommon.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Entities
{
    /// <summary>
    /// Implementation of Axi-symmetric node for heat transfer 
    /// </summary>
    public class Node : INode
    {
        public int Id { get; set; }
        public NodeType NT { get; set; } 
        public PointD P { get; set; }

        /// <summary>
        /// Degree of freedom for this node is 1, since only one temperature component is defined
        /// </summary>
        /// <returns></returns>
        public virtual int GetDof()
        {
            switch (NT)
            {
                case NodeType.t:
                    return 1;
            }
            return 1;
        }

        public override string ToString()
        {
            return $"{Id}:({P.X};{P.Y};{P.Y})";
        }
        public Node()
        {

        }
        public Node(int id, NodeType type, double xr, double xz)
        {
            Id = id;
            P = new PointD(xr,xz);
            NT = type;
        }
    }
}
