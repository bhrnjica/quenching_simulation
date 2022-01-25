using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Types
{
    public enum NodeType
    {
        //uv = 1,//u,v - displacement 
        //uvuf,//u,v - displacement and rotation related to u
        //uvvf,//u,v - displacement and rotation related to v
        //uvufvf,//u,v - displacement and rotation related to u , v 
        t//heat transfer node type
    }
    public enum ElementType
    {
        line=1,//line with two node
        tline=2,//line with three nodes
        triangle = 3,//three node triangle finite element
        quad=4,//four node rectangle finite element
        ltriangle=5,//four node triangle element
        quadquad=6,// eight node rectangle element
    }
    public enum PlaneType
    {
        stress = 1,//plane stress type
        strain,//plane strain type
        heat,
    }
}
