using System;
using System.Collections.Generic;

namespace CPApplication.Core.Models;

public partial class Appeal
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Organization { get; set; }

    public string? AppealPurpose { get; set; }

    public int BroadcastId { get; set; }

    public virtual Broadcast Broadcast { get; set; } = null!;
}
