using System;
using System.Collections.Generic;

namespace CPApplication.Core.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<TvProgram> TvPrograms { get; } = new List<TvProgram>();
}
