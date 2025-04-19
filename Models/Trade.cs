using System;
using System.Collections.Generic;

namespace FantasySports.Models
{
    public class Trade
    {
        public int Id { get; set; }
        public int ProposingTeamId { get; set; }
        public Team ProposingTeam { get; set; } = null!;
        public int ReceivingTeamId { get; set; }
        public Team ReceivingTeam { get; set; } = null!;
        public string OfferedAthletes { get; set; } = ""; // Comma-separated athlete IDs
        public string RequestedAthletes { get; set; } = "";
        public string? CounterOffer { get; set; }
        public TradeStatus Status { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public enum TradeStatus { Pending, Accepted, Declined, Countered, Expired }
}
