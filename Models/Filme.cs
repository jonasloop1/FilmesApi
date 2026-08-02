using System.ComponentModel.DataAnnotations;

namespace FilmesApi.Models;

public class Filme
{
    //Data Annotations podem ser usadas para validação futura
    [Key]
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "O Titulo do filme é obrigatório")]
    public string Titulo { get; set; }
    [Required(ErrorMessage = "O Gênero do filme é obrigatório")]
    [StringLength(15, ErrorMessage = "O gênero não pode ter mais de 15 caracteres")]
    public string Genero { get; set; }
    [Required(ErrorMessage = "A duração do filme é obrigatória")]
    [Range(70, 600, ErrorMessage = "A duração deve ser entre 70 e 600 minutos")]
    public int Duracao { get; set; }
}
