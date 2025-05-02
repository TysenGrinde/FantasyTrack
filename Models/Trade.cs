using System;
using System.Collections.Generic;

// Models/Trade.cs
namespace FantasySports.Models
{
    public enum TradeStatus
    {
        Pending,
        Accepted,
        Declined,
        Countered,
        Expired
    }
    public class Trade
    {
        public int Id { get; set; }
        public int ProposingTeamId { get; set; }
        public int ReceivingTeamId { get; set; }
        public string OfferedAthletes { get; set; } = ""; // JSON or comma-separated IDs
        public string RequestedAthletes { get; set; } = "";
        public string? CounterOffer { get; set; } // Added CounterOffer
        public TradeStatus Status { get; set; } = TradeStatus.Pending; // Using enum
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(2); // Added ExpiryDate

        // Navigation properties
        public Team ProposingTeam { get; set; } = null!;
        public Team ReceivingTeam { get; set; } = null!;
    }
}
