namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class WoMaterialTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string WoNo { get; set; }
        public string WoId { get; set; }
        public string ItemNo { get; set; }
        public string TagNo { get; set; }
        public string TagId { get; set; }
        public string TagRegisterId { get; set; }
        public string StockId { get; set; }
        public string Quantity { get; set; }
        public string UnitName { get; set; }
        public string UnitDescription { get; set; }
        public string AdditionalInformation { get; set; }
        public string RequiredDate { get; set; }
        public string EstimatedAvailableDate { get; set; }
        public string Available { get; set; }
        public string MaterialStatus { get; set; }
        public string StockLocation { get; set; }
        public string LastUpdated { get; set; }

        public const string TopicName = "womaterial";
    }
}
