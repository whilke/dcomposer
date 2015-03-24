using GraphX;
using GraphX.Logic;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DComposer
{
    public class GraphArea : GraphArea<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>> { }

    public class Graph : BidirectionalGraph<DataVertex, DataEdge> { }

    //Logic core class
    public class GXLogicCore : GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>> { }

    //Vertex data object
    public class DataVertex : VertexBase
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public DialogLabel Page { get; set; }

        public override string ToString()
        {
            if (Page != null)
                return Page.Label;

            return String.Empty;
        }
    }

    //Edge data object
    public class DataEdge : EdgeBase<DataVertex>
    {
        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }

        public DataEdge()
            : base(null, null, 1)
        {
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}