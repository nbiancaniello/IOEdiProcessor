using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data.Context
{
    /// <inheritdoc />
    /// <summary>
    /// The entity framework context
    /// </summary>
    public class PrismContext : DbContext
    {
        public PrismContext(DbContextOptions<PrismContext> options) : base(options)
        {
        }

        //public DbSet<Organization> Organizations { get; set; }
    }
}
