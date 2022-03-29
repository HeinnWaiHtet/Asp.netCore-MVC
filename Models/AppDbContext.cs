using Microsoft.EntityFrameworkCore;

namespace Asp.netCore_MVC.Models
{
    public class AppDbContext: DbContext
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
            modelBuilder.Seed();
        }
        #endregion
    }
}
