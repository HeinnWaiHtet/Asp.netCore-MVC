namespace Asp.netCore_MVC.ViewModels
{
    public class EmployeeEditViewModel: EmployeeCreateViewModel
    {
        /// <summary>
        /// Employee Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Existing Employee Photo Path
        /// </summary>
        public string? ExistingPhotoPath { get; set; }
    }
}
