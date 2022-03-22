using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class WoMaterialDeleteTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string StockId { get; set; }
        public string MaterialStatus { get; set; }

        public const string TopicName = "womaterial_delete";
    }
}
