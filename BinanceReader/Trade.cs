namespace BinanceReader
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Trade
    {
        [DataMember(Name = "e")]
        public string EventType { get; set; } = "";

        [DataMember(Name = "E")]
        public float DispatchTime { get; set; }


        [DataMember(Name = "s")]
        public string Pair { get; set; } = "";

        [DataMember(Name = "t")]
        public float EventID { get; set; }


        [DataMember(Name = "p")]
        public string Price { get; set; }

        [DataMember(Name = "q")]
        public string Value { get; set; } = "";


        [DataMember(Name = "b")]
        public float Currency { get; set; }

        [DataMember(Name = "a")]
        public float BuyerID { get; set; }


        [DataMember(Name = "T")]
        public float SellerID { get; set; }


        [DataMember(Name = "m")]
        public bool BuyerMaker { get; set; }

        [DataMember(Name = "M")]
        public bool NotRelevant { get; set; }
    }
}
