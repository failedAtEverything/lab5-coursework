using System;
using System.Collections.Generic;

namespace CPApplication.Core.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Position { get; set; }

    public virtual ICollection<Broadcast> Broadcasts { get; } = new List<Broadcast>();
}
