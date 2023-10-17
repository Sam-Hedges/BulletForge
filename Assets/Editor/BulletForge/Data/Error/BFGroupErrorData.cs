using System.Collections.Generic;

namespace BulletForge.Data.Error
{
    using Elements;

    public class BFGroupErrorData
    {
        public BFErrorData ErrorData { get; set; }
        public List<BFGroup> Groups { get; set; }

        public BFGroupErrorData()
        {
            ErrorData = new BFErrorData();
            Groups = new List<BFGroup>();
        }
    }
}