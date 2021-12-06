using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicaMedica.Models
{
    public class Consulta
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ID { get; set; }

        //Dados FK
        [DisplayName("Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string NomePaciente { get; set; }

        [DisplayName("CPF")]
        [Range(11, long.MaxValue, ErrorMessage = "A quantidada minima e maxima é de 11")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public long? Cpf { get; set; }

        [DisplayName("CRM")]
        [Range(4, long.MaxValue, ErrorMessage = "A quantidade mínima de números é 4 e o máximo é 10")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public long? Crm { get; set; }

        //Dados p/ alimentação

        [DisplayName("Secretario(a)")]
        public string Secretario { get; set; }

        [DisplayName("Data de cadastro")]
        [DataType(DataType.Date)]
        public DateTime? dataCadastro { get; set; }

        [DisplayName("Informe o motivo da consulta")]
        public string Observacao { get; set; }

        [DisplayName("Data da Consulta")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public DateTime? dataConsulta { get; set; }

        //Dados do Pagamento

        [DisplayName("Status da Consulta")]
        public string Situacao { get; set; }

        [DisplayName("Forma de Pagamento")]
        public string Pagamento { get; set; }

        [DisplayName("Comprovante de Pagamento")]
        public string Comprovante { get; set; }

        // Aqui sera quando for realizada a confirmação 
        public bool ConfirmacaoSeVaiAconsulta { get; set; } //Não tera na dela de criação, mas na consultas já agendadas

        [DataType(DataType.Date)]
        public DateTime dataConfirmação { get; set; } //Não tera na tela de criação, mas na consultas já agendadas

        [DisplayName("Quem ligou ao Paciente? ")]
        public string secretariaCancelamento { get; set; }

        [DisplayName("Quem confirmou ?")]
        public string quemCancelou { get; set; }

        public DateTime FimConsulta { get; set; }
        public DateTime InicioConsulta { get; set; }

        [DisplayName("anotações da consulta")]
        public string Anotacoes { get; set; }



        [MinLength(8, ErrorMessage = "Utilize 8 caracteres")]
        [MaxLength(8, ErrorMessage = "Utilize ao máximo 8 caracteres")]
        [DataType(DataType.Password)]
        public string Chave { get; set; }

        public string Tipo { get; set; }

    }
}
