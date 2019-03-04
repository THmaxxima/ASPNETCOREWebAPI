using System;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        //public virtual DbSet<User> User { get; set; }
        //public virtual DbSet<Queue> Queues_API { get; set; }
        public virtual DbSet<Tokens> Tokens { get; set; }
    }
   

}