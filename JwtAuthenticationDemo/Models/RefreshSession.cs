using System;
using System.Collections.Generic;

namespace JwtAuthenticationDemo.Models;

public partial class RefreshSession
{
    public Guid SessionId { get; set; }

    public int UserId { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
