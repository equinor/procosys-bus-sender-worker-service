namespace Equinor.ProCoSys.BusSender.Core.Models
{
    public class BusEventMessage
    {
        public string ProjectSchema { get; set; }
        public string ProjectName { get; set; }
        public string McPkgNo { get; set; }
        public string CommPkgNo { get; set; }
    }
}
