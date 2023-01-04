using System;
using System.Collections.Generic;

namespace CPApplication.Core.Models;

public partial class TvProgram
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Length { get; set; }

    public double? Rating { get; set; }

    public int GenreId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Broadcast> Broadcasts { get; } = new List<Broadcast>();

    public virtual Genre Genre { get; set; } = null!;
}
