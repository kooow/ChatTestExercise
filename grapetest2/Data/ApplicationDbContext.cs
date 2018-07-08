using System;
using System.Collections.Generic;
using System.Text;
using grapetest2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace grapetest2.Data
{
  /// <summary>
  /// 
  /// </summary>
  public class ApplicationDbContext : IdentityDbContext<IdentityUser>
  {

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<CustomerConversationMessage>().HasIndex(p => p.Content);
    }

    public DbSet<CustomerConversation> CustomerConversations { get; set; }

    public DbSet<CustomerConversationMessage> CustomerConversationMessages { get; set; }

  }
}
