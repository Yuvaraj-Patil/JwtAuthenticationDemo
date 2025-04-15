using System;
using System.Collections.Generic;

namespace JwtAuthenticationDemo.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<RefreshSession> RefreshSessions { get; set; } = new List<RefreshSession>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
