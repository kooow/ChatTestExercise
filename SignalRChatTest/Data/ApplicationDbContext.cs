using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalRChatTest.Models;

namespace SignalRChatTest.Data
{
    /// <summary>
    /// Application database context that inherits from IdentityDbContext to manage user identities and roles.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Constructor for ApplicationDbContext, used to inject the database context options.
        /// </summary>
        /// <param name="options">Database context options</param>
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CustomerConversationMessage>().HasIndex(p => p.Content);
        }

        /// <summary>
        /// Customer conversation entities in the database.
        /// </summary>
        public DbSet<CustomerConversation> CustomerConversations { get; set; }

        /// <summary>
        /// Customer conversation messages in the database.
        /// </summary>
        public DbSet<CustomerConversationMessage> CustomerConversationMessages { get; set; }
    }
}
