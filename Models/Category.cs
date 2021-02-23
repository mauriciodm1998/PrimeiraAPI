using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API.Models
{
    public class Category 
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter no máximo 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter no minimo 3 caracteres")]
        public string Title { get; set; }
    }
}