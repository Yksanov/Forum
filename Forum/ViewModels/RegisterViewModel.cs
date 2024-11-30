using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Forum.ViewModels;

public class RegisterViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Логин не может быть пустым")]
    [Remote(action: "CheckUserName", controller: "Validation", ErrorMessage = "Пользователь с таким именем пользователя уже существует")]
    public string UserName { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Почта в некорректном формате")]
    [Remote(action: "CheckUserEmail", controller: "Validation", ErrorMessage = "Пользователь с такой почтой уже существует", AdditionalFields = "Id")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Аватар не может быть пустым")]
    public IFormFile Avatar { get; set; }
    [Required(ErrorMessage = "Пароль не может быть пустым")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Повтор пароля не может быть пустым")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}