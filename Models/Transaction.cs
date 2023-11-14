using System;
using System.Collections.Generic;

namespace mqtt_remote_server.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }
}
