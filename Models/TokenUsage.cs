using System;

namespace WileyBudgetManagement.Models
{
    public class TokenUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public decimal EstimatedCost { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string ModelUsed { get; set; } = string.Empty;
    }
}
public string ModelUsed { get; set; } = string.Empty;
    }
}
