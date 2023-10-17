using System.Collections.Generic;

namespace BulletForge.Data.Error
{
    using Elements;

    public class BFNodeErrorData
    {
        public BFErrorData ErrorData { get; set; }
        public List<BFNode> Nodes { get; set; }

        public BFNodeErrorData()
        {
            ErrorData = new BFErrorData();
            Nodes = new List<BFNode>();
        }
    }
}