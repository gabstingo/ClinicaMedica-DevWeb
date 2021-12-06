using ClinicaMedica.Data;
using ClinicaMedica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicaMedica.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]//para fazer com que a classe toda nao utilizem cache
    public class PacienteController : Controller
    {
        private readonly DataContext dataContext;

        //construtor
        public PacienteController(DataContext dc)
        {
            dataContext = dc;
        }

        public IActionResult Index()
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Secretaria")
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de Visualizar usuarios redirecionando para a home

            List<Paciente> lista = dataContext.Paciente.OrderBy(x => x.ID).ToList();

            return View(lista);
        }

        public IActionResult cadastrar()
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Secretaria")
            {
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home
            }

            return View();
        }
        //----------------------------------------------------------------------------------------------------------------------
        //Funções


        // *******************
        // *** Validar CPF ***
        // *******************
        public bool ValidaCpf(long? aux)
        {
            //Validando Botão
            string vrCPF = Convert.ToString(aux);
            //Apaga esse txtBox1 e coloca como o nome da variavel que recebe
            // Se estiver vindo inteiro, coloca Convert.ToString(Nome)
            string cpf = vrCPF.Replace(".", "");
            cpf = cpf.Replace("-", "");

            if (cpf.Length != 11) // caso n tenha a quantidade necessaria
            {
                return false;
            }
            switch (cpf)
            {
                case "11111111111":
                    return false;
                case "00000000000":
                    return false;
                case "2222222222":
                    return false;
                case "33333333333":
                    return false;
                case "44444444444":
                    return false;
                case "55555555555":
                    return false;
                case "66666666666":
                    return false;
                case "77777777777":
                    return false;
                case "88888888888":
                    return false;
                case "99999999999":
                    return false;
            }
            //variaveis necessarias para a validação
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf;

            string digito;

            int soma;

            int resto;
            //Isso aqui já é necessario para a validação
            tempCpf = cpf.Substring(0, 9);

            soma = 0;

            for (int i = 0; i < 9; i++)

                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;

            if (resto < 2)

                resto = 0;

            else

                resto = 11 - resto;

            digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;

            for (int i = 0; i < 10; i++)

                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            //Apos termos os 2 ultimos finais e não sendo valores como 11111, 2222 e etc
            //vamos validar se esta batendo o que é e o que deveria ser

            string validadorCPF = cpf.Substring(cpf.Length - 2, 2);

            if (Convert.ToInt32(validadorCPF) == Convert.ToInt32(digito))
            {
                //MessageBox.Show("Valido");
                // retorn 1
                return true;
            }
            else
            {
                //MessageBox.Show("Invalido");
                // retorn 0
                return false;
            }
        }
        //----------------------------------------------------------------------------------------------------
        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult cadastrar(Paciente cadastrarpaciente)
        {
            // Chamando a função valida cpf
            if (ValidaCpf(cadastrarpaciente.Cpf) == false)
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "CPF inválido";
                return View();
            }
            //Não deveria permitir 
            //Paciente usuarioBanco = dataContext.Paciente.FirstOrDefault(x => x.Cpf == cadastrarpaciente.Cpf);//retorna nulo se nao encontrar
            //if (usuarioBanco != null)
            //{
            //    ViewBag.TipoMensagem = "ERRO";
            //    ViewBag.Mensagem = "CPF já cadastrado";
            //    return View();
            //}



            //esse comentado serve para validar se toda a model esta correta de acordo com a modelagem
            if (cadastrarpaciente.Nome != null && cadastrarpaciente.Cpf != null &&
                cadastrarpaciente.DataNasc != null && cadastrarpaciente.Endereco != null && cadastrarpaciente.Email != null &&
                cadastrarpaciente.Profissao != null && cadastrarpaciente.Sexo != null)
            {
                DateTime dataAtual = DateTime.Now.Date;

                if (cadastrarpaciente.DataNasc > dataAtual)
                {
                    //TempData["TipoMensagem"] = "ERRO";
                    //TempData["Mensagem"] = $"Usuário {cadastrarUsuario.Nome} não foi salvo pois não é permitido o cadastro com data de nascimento futura";
                    //return RedirectToAction("Index");

                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = $"Usuário {cadastrarpaciente.Nome} não foi salvo pois não é permitido o cadastro com data de nascimento futura";
                    return View();
                }
                try
                {
                    cadastrarpaciente.Ativo = true;
                    dataContext.Paciente.Add(cadastrarpaciente);
                    dataContext.SaveChanges();
                }
                catch
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "O cadastro não pode ser efetuado";
                    return View();
                }

                ViewBag.TipoMensagem = "SUCESSO";
                ViewBag.Mensagem = "Cadastro efetuado com sucesso";
                return View();
            }

            ViewBag.Erro = "Usuário inválido";
            return View();
        }
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult Index(Paciente paciente)
        {
            {
                if (paciente == null)
                {
                    //return NoContent();//não faz nada, retorna nada, nenhuma ação
                    return RedirectToAction("Index");
                }
                else
                {
                    //----------------------------------------------------------------------
                    string consulta = " SELECT * FROM Paciente where 1 = 1 ";
                    List<object> parametros = new List<object>();

                    if (paciente.Nome != null)
                    {

                        Paciente usuarioBanco2 = dataContext.Paciente.FirstOrDefault(x => x.Nome == paciente.Nome);
                        if (usuarioBanco2 != null)
                        {
                            consulta += " AND Nome like @Cpf ";
                            parametros.Add(new SqlParameter("Cpf", paciente.Nome));
                        }
                        else
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "Valor digitado Invalido";
                            return View();
                        }
                    }

                    consulta += " order by DataNasc ";

                    List<Paciente> lista = dataContext.Paciente.FromSqlRaw(consulta, parametros.ToArray()).ToList();
                    //----------------------------------------------------------------------
                    return View(lista);
                }
            }
        }
        public IActionResult EditarPaciente(int? ID)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Secretaria")
            {
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home
            }

            if (ID.HasValue)
            {
                Paciente pessoa = dataContext.Paciente.FirstOrDefault(x => x.ID == ID);
                if (pessoa != null)
                    return View(pessoa);
            }

            return NoContent();
        }
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult EditarPaciente(Paciente pessoa)
        {
            Paciente consultaBanco = dataContext.Paciente.FirstOrDefault(x => x.ID == pessoa.ID);//retorna nulo se nao encontrar

            consultaBanco.Nome = pessoa.Nome;
            consultaBanco.Cpf = pessoa.Cpf;
            consultaBanco.Ativo = pessoa.Ativo;
            consultaBanco.DataNasc = pessoa.DataNasc;
            consultaBanco.Observacao = pessoa.Observacao;
            consultaBanco.Email = pessoa.Email;
            consultaBanco.Endereco = pessoa.Endereco;
            consultaBanco.Profissao = pessoa.Profissao;
            consultaBanco.Sexo = pessoa.Sexo;
            consultaBanco.Telefone = pessoa.Telefone;


            if (ValidaCpf(consultaBanco.Cpf) == false)
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "CPF inválido";
                return View();
            }

            if (consultaBanco.Nome != null && consultaBanco.Cpf != null &&
             consultaBanco.DataNasc != null && consultaBanco.Endereco != null && consultaBanco.Email != null &&
             consultaBanco.Profissao != null && consultaBanco.Sexo != null)
            {
                DateTime dataAtual = DateTime.Now.Date;

                if (consultaBanco.DataNasc > dataAtual)
                {
                    //TempData["TipoMensagem"] = "ERRO";
                    //TempData["Mensagem"] = $"Usuário {cadastrarUsuario.Nome} não foi salvo pois não é permitido o cadastro com data de nascimento futura";
                    //return RedirectToAction("Index");

                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = $"Usuário {consultaBanco.Nome} não foi salvo pois não é permitido o cadastro com data de nascimento futura";
                    return View();
                }
                try
                {
                    dataContext.Paciente.Update(consultaBanco);
                    dataContext.SaveChanges();
                }
                catch
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "O cadastro não pode ser efetuado";
                    return View();
                }

                ViewBag.TipoMensagem = "SUCESSO";
                ViewBag.Mensagem = "Cadastro efetuado com sucesso";
                return View();
            }
            ViewBag.Erro = "Usuário inválido";
            return View();
        }
    }
}
