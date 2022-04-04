using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCore_MVC.Models
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        #region Constructor

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        #endregion

        #region Properties

        public DbSet<Employee> Employees { get; set; }

        #endregion


        #region CreateDeafultTableRow

        /// <summary>
        /// Create Default Table Value When Create Model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            /** add delete no action when data have */
            foreach(var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                /** Change default delete on cascade to restrict */
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        #endregion
    }
}
