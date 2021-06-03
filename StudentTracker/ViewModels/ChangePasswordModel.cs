

using System.ComponentModel.DataAnnotations;

namespace StudentTracker.ViewModels
{
    public class ChangePasswordModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указана старый пароль")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Не указан новый пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "Пароль введен неверно")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
