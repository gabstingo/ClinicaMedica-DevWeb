using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicaMedica.Models
{

    [Index(nameof(Email), IsUnique = true)]
    public class Usuario
    {
        //[Range(1,50, ErrorMEssage = "Mensagem para usuário")] // Para definir intervalo de valores

        public int? ID { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido")]
        public string Email { get; set; }

        [DisplayName("Senha")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Utilize ao menos 4 caracteres")]
        public string Senha { get; set; }

        [DisplayName("Senha de recuperação")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Utilize ao menos 4 caracteres")]
        public string SenhaRecuperacao { get; set; }

        [DisplayName("Tipo de usuário")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string TipoUsuario { get; set; }

        public bool? Ativo { get; set; }

        public bool? PrimeiroAcesso { get; set; }

        //Para guardarmos a data final e a hora que o email com link ainda esta valido para recuperacao de senha
        [DataType(DataType.Time)]
        public DateTime? TempoMaxRecupSenha { get; set; }

        [DisplayName("CPF")]
        [Range(1, long.MaxValue, ErrorMessage = "A quantidade mínima de números é 1 e o máximo é 19")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public long? Cpf { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [MinLength(3, ErrorMessage = "O nome deve possuir, ao menos, 3 caracteres")]
        [MaxLength(50)]
        public string Nome { get; set; }

        [DisplayName("Data de nascimento")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Date)]
        public DateTime? DataNasc { get; set; }

        //--------------------------------------------Relacionado a medico abaixo
        [DisplayName("Digite o CRM do médico ou 0 caso seja secretaria")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0, long.MaxValue, ErrorMessage = "A quantidade mínima de números é 1 e o máximo é 19")]
        public long? Crm { get; set; }
        //--------------------------------------------Relacionado a medico acima

        //--------------------------------------Não vai para o banco
        [NotMapped]
        [DisplayName("Nova senha")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Utilize ao menos 4 caracteres")]
        [MaxLength(32, ErrorMessage = "Utilize ao máximo 32 caracteres")]
        public string NovaSenha { get; set; }

        [NotMapped]
        [DisplayName("Nova senha")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Utilize ao menos 4 caracteres")]
        [MaxLength(32, ErrorMessage = "Utilize ao máximo 32 caracteres")]
        public string NovaSenha2 { get; set; }
        //--------------------------------------
    }
}
