using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicaMedica.Models
{
    [Index(nameof(Cpf), IsUnique = true)]
    public class Paciente
    {
        [DisplayName("Identificador")]
        public int ID { get; set; }
        //endereco
        [DisplayName("Endereço")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(200)]
        public string Endereco { get; set; }

        //telefone
        [DisplayName("Telefone")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Telefone { get; set; }

        //profissao
        [DisplayName("Profissão")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Profissao { get; set; }

        //sexo
        [DisplayName("Sexo")]
        [MaxLength(20)]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Sexo { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [MinLength(3, ErrorMessage = "O nome deve possuir, ao menos, 3 caracteres")]
        [MaxLength(50)]
        public string Nome { get; set; }

        [DisplayName("Email")]
        [MaxLength(100)]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Email { get; set; }

        [DisplayName("Data de nascimento")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Date)]
        public DateTime? DataNasc { get; set; }

        [DisplayName("CPF")]
        [Range(1, long.MaxValue, ErrorMessage = "A quantidade mínima de números é 1 e o máximo é 19")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public long? Cpf { get; set; }

        public bool? Ativo { get; set; }

        [DisplayName("Observação")]
        public string Observacao { get; set; }
    }
}
