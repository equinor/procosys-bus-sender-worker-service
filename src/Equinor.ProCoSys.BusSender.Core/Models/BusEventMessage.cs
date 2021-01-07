using System;
using System.Collections.Generic;
using System.Text;

namespace Equinor.ProCoSys.BusSender.Core.Models
{
    public class BusEventMessage
    {
        public string ProjectSchema { get; set; }
        public string ProjectName { get; set; }
        public string McPkgNo { get; set; }
        public string CommPkgNo { get; set; }

        //ElementNo Key for AppInsights to follow element through all modules
        public string TryGetElementNo()
        {
            if (!string.IsNullOrWhiteSpace(McPkgNo))
            {
                return McPkgNo;
            }
            if (!string.IsNullOrWhiteSpace(CommPkgNo))
            {
                return CommPkgNo;
            }

            return string.Empty;
        }
    }
}
