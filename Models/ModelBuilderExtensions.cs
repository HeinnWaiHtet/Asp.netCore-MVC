using Microsoft.EntityFrameworkCore;

namespace Asp.netCore_MVC.Models
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Set Employee Deafult Row Data when create model
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee()
                {
                    Id = 1,
                    Name = "Hein Wai Htet",
                    Email = "heinwaihtet@gmail.com",
                    Department = Dept.IT,
                    PhotoPath = String.Empty
                },
                new Employee()
                {
                    Id = 2,
                    Name = "Aye Chan May",
                    Email = "acm@gmail.com",
                    Department = Dept.HR,
                    PhotoPath = String.Empty
                },
                new Employee()
                {
                    Id = 3,
                    Name = "HWH",
                    Email = "hwh@gmail.com",
                    Department = Dept.Software,
                    PhotoPath = String.Empty
                });
        }
    }
}
