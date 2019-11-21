using Microsoft.EntityFrameworkCore;

namespace IOEdiProcessor.Data.Context
{
    /// <summary>
    /// The entity framework context
    /// </summary>
    public class IOEdiProcessorContext : DbContext
    {
        public IOEdiProcessorContext() : base() { }

        public IOEdiProcessorContext(DbContextOptions<IOEdiProcessorContext> options) : base(options) { }

        //public DbSet<Address> Address { get; set; }
        //public DbSet<Box> Box { get; set; }
        //public DbSet<Contact> Contact { get; set; }
        //public DbSet<DocHeader> DocHeader { get; set; }
        //public DbSet<Edifact> Edifact { get; set; }
        //public DbSet<Freight> Freight { get; set; }
        //public DbSet<Header> Header { get; set; }
        //public DbSet<MapInfo> MapInfo { get; set; }
        //public DbSet<Measure> Measure { get; set; }
        //public DbSet<Pallet> Pallet { get; set; }
        //public DbSet<PalletV2> PalletV2 { get; set; }
        //public DbSet<Part> Part { get; set; }
        //public DbSet<PartV2> PartV2 { get; set; }
        //public DbSet<ReqDates> ReqDates { get; set; }
        //public DbSet<Seller> Seller { get; set; }
        //public DbSet<ShipFrom> ShipFrom { get; set; }
        //public DbSet<ShipTo> ShipTo { get; set; }
        //public DbSet<Shipping_Containers> Shipping_Containers { get; set; }
        //public DbSet<SoldTo> SoldTo { get; set; }
    }
}
