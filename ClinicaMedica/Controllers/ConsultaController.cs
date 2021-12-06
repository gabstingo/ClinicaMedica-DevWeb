using ClinicaMedica.Data;
using ClinicaMedica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicaMedica.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]//para fazer com que a classe toda nao utilizem cache
    public class ConsultaController : Controller
    {
        private readonly DataContext dataContext;

        //construtor da classe consulta
        public ConsultaController(DataContext dc)
        {
            dataContext = dc;
        }

        public IActionResult Index()
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Secretaria")
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de Visualizar usuarios redirecionando para a home

            List<Consulta> lista = dataContext.Consulta.OrderBy(x => x.ID).ToList();

            return View(lista);
        }
        public bool ValidaCpf(long? aux)
        {
            //Validando Botão teste
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
        public IActionResult cadastrar()
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Secretaria")
            {
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home
            }

            return View();
        }
        public IActionResult EditarConsulta(int? ID)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Secretaria")
            {
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home
            }

            if (ID.HasValue)
            {
                Consulta consulta = dataContext.Consulta.FirstOrDefault(x => x.ID == ID);
                if (consulta != null)
                    return View(consulta);
            }

            return NoContent();
        }
        public IActionResult ConfirmarConsulta(int? ID)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Secretaria")
            {
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home
            }

            if (ID.HasValue)
            {
                Consulta consulta = dataContext.Consulta.FirstOrDefault(x => x.ID == ID);
                if (consulta != null)
                    return View(consulta);
            }

            return NoContent();
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult Index(Consulta Agenda)
        {
            {
                if (Agenda == null)
                {
                    //return NoContent();//não faz nada, retorna nada, nenhuma ação
                    return RedirectToAction("Index");
                }
                else
                {
                    //----------------------------------------------------------------------
                    string consulta = " SELECT * FROM Consulta where 1 = 1 ";
                    List<object> parametros = new List<object>();

                    if (Agenda.Cpf != null)
                    {
                        Paciente usuarioBanco = dataContext.Paciente.FirstOrDefault(x => x.Cpf == Agenda.Cpf);
                        Usuario usuarioBanco2 = dataContext.Logins.FirstOrDefault(x => x.Crm == Agenda.Cpf);
                        if (usuarioBanco != null)
                        {
                            consulta += " AND Cpf like @Cpf ";
                            parametros.Add(new SqlParameter("Cpf", Agenda.Cpf));
                        }
                        else if (usuarioBanco2 != null)
                        {
                            consulta += " AND Crm like @Cpf ";
                            parametros.Add(new SqlParameter("Cpf", Agenda.Cpf));
                        }
                        else
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "CPF INVALIDO";
                            return View();
                        }
                    }

                    if (Agenda.Situacao == "Pendente")
                    {
                        consulta += " AND Situacao = 'Pendente' ";
                        //parametros.Add(new SqlParameter("Situacao", Agenda.Situacao));
                    }

                    if (Agenda.Situacao == "Confirmada")
                    {
                        consulta += " AND Situacao = 'Confirmada' ";
                        //parametros.Add(new SqlParameter("Situacao", Agenda.Situacao));
                    }

                    if (Agenda.Situacao == "Cancelada")
                    {
                        consulta += " AND Situacao = 'Cancelado' ";
                        //parametros.Add(new SqlParameter("Situacao", Agenda.Situacao));
                    }

                    if (Agenda.Situacao == "Concluida")
                    {
                        consulta += " AND Situacao = 'Concluida' ";
                        //parametros.Add(new SqlParameter("Situacao", Agenda.Situacao));
                    }

                    consulta += " order by dataCadastro ";

                    List<Consulta> lista = dataContext.Consulta.FromSqlRaw(consulta, parametros.ToArray()).ToList();
                    //----------------------------------------------------------------------
                    return View(lista);
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult cadastrar(Consulta agendarConsulta)
        {
            Paciente buscarcpf = dataContext.Paciente.FirstOrDefault(x => x.Cpf == agendarConsulta.Cpf);
            if (buscarcpf == null && agendarConsulta.NomePaciente == "")
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "Falta de dados ou dados incorretos";
                return View();
            }
            else
            {
                if(agendarConsulta.NomePaciente == buscarcpf.Nome)
                {
                    agendarConsulta.Cpf = buscarcpf.Cpf;
                    agendarConsulta.NomePaciente = buscarcpf.Nome;
                }
                else
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "Nome Invalido";
                    return View();
                }
            }
            Usuario buscarcrm = dataContext.Logins.FirstOrDefault(x => x.Crm == agendarConsulta.Crm);
            if (buscarcrm == null)
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "CRM não Encontrado no sistema";
                return View();
            }
            else
            {
                agendarConsulta.Crm = buscarcrm.Crm;
            }
            
            DateTime dataAtual = DateTime.Now;

            agendarConsulta.Secretario = User.Identity.Name;
            agendarConsulta.dataCadastro = dataAtual;
            agendarConsulta.Situacao = "Pendente";
            if(agendarConsulta.Pagamento == "SUS")
            {
                agendarConsulta.Comprovante = "Gratís";
            }

            if (agendarConsulta.Crm != null && agendarConsulta.Cpf != null && agendarConsulta.Observacao != null && agendarConsulta.dataConsulta != null &&
                agendarConsulta.Comprovante != null && agendarConsulta.Pagamento != null)
            {
                if (agendarConsulta.dataConsulta < dataAtual)
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "Não pode selecionar para agendar uma consulta que já passou";
                    return View();
                }
                string consulta = " SELECT * FROM Consulta where 1 = 1  AND Situacao = 'Pendente' OR Situacao = 'Confirmada' ";
                List<object> parametros = new List<object>();
                List<Consulta> lista = dataContext.Consulta.FromSqlRaw(consulta, parametros.ToArray()).ToList();
                //Consulta cadastros = dataContext.Consulta.FirstOrDefault(x => x.dataConsulta == agendarConsulta.dataConsulta);
                if (lista != null)
                {
                    for(int i = 0; lista.Count > i; i++){
                        if (lista[i].Crm == agendarConsulta.Crm && lista[i].Cpf == agendarConsulta.Cpf &&
                        lista[i].dataConsulta == agendarConsulta.dataConsulta)
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "Esse CPF já esta cadastrado com o CRM nesse Dia";
                            return View();
                        }
                        if (lista[i].Crm == agendarConsulta.Crm && lista[i].dataConsulta == agendarConsulta.dataConsulta)
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "Doutor já tem consulta marcada nesse horario";
                            return View();
                        }
                        if (lista[i].Cpf == agendarConsulta.Cpf && lista[i].dataConsulta == agendarConsulta.dataConsulta)
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "Paciente já tem consulta marcada nesse horario";
                            return View();
                        }
                    }
                }
                     
                try
                {
                    agendarConsulta.ID = null;
                    dataContext.Consulta.Add(agendarConsulta);
                    dataContext.SaveChanges();//ta dando erro nessa linha
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
            else
            {
                ViewBag.Erro = "Esta Faltando Dados";
                return View();
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult EditarConsulta(Consulta consulta)
        {
            Consulta consultaBanco = dataContext.Consulta.FirstOrDefault(x => x.ID == consulta.ID);//retorna nulo se nao encontrar

            consultaBanco.Comprovante = consulta.Comprovante;
            consultaBanco.ConfirmacaoSeVaiAconsulta = consulta.ConfirmacaoSeVaiAconsulta;
            consultaBanco.Cpf = consulta.Cpf;
            consultaBanco.Crm = consulta.Crm;
            consulta.Secretario = consultaBanco.Secretario;
            consultaBanco.Observacao = consulta.Observacao;
            consultaBanco.Pagamento = consulta.Pagamento;
            consultaBanco.quemCancelou = consulta.quemCancelou;
            consultaBanco.secretariaCancelamento = consulta.secretariaCancelamento;
            consultaBanco.Situacao = consulta.Situacao;
            consulta.dataCadastro = consultaBanco.dataCadastro;
            consultaBanco.dataConfirmação = consulta.dataConfirmação;
            //consultaBanco.dataConsulta = consulta.dataConsulta;

            Paciente buscarcpf = dataContext.Paciente.FirstOrDefault(x => x.Cpf == consulta.Cpf);
            if (buscarcpf == null)
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "CPF não Encontrado no sistema";
                return View();
            }
            else
            {
                consulta.Cpf = buscarcpf.Cpf;
            }
            Usuario buscarcrm = dataContext.Logins.FirstOrDefault(x => x.Crm == consulta.Crm);
            if (buscarcrm == null)
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "CRM não Encontrado no sistema";
                return View();
            }
            else
            {
                consulta.Crm = buscarcrm.Crm;
            }
            DateTime dataAtual = DateTime.Now;
            if (consulta.Crm != null && consulta.Cpf != null && consulta.Observacao != null && consulta.dataConsulta != null &&
                consulta.Comprovante != null && consulta.Pagamento != null)
            {
                if (consulta.dataConsulta < dataAtual)
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "Não pode selecionar para agendar uma consulta que já passou";
                    return View();
                }
                string consultando = " SELECT * FROM Consulta where 1 = 1  AND Situacao = 'Pendente' OR Situacao = 'Confirmada' ";
                List<object> parametros = new List<object>();
                List<Consulta> lista = dataContext.Consulta.FromSqlRaw(consultando, parametros.ToArray()).ToList();
                //Consulta cadastros = dataContext.Consulta.FirstOrDefault(x => x.dataConsulta == agendarConsulta.dataConsulta);
                if (lista != null)
                {
                    for (int i = 0; lista.Count > i; i++)
                    {
                        if (lista[i].Crm == consulta.Crm && lista[i].Cpf == consulta.Cpf &&
                        lista[i].dataConsulta == consulta.dataConsulta)
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "Esse CPF já esta cadastrado com o CRM nesse Dia";
                            return View();
                        }
                        if (lista[i].Crm == consulta.Crm && lista[i].dataConsulta == consulta.dataConsulta)
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "Doutor já tem consulta marcada nesse horario";
                            return View();
                        }
                        if (lista[i].Cpf == consulta.Cpf && lista[i].dataConsulta == consulta.dataConsulta)
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "Paciente já tem consulta marcada nesse horario";
                            return View();
                        }
                        consultaBanco.dataConsulta = consulta.dataConsulta;
                    }
                }

                try
                {
                    //consulta.ID = consultaBanco.ID;
                    dataContext.Consulta.Update(consultaBanco);
                    dataContext.SaveChanges();//ta dando erro nessa linha
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
            else
            {
                ViewBag.Erro = "Esta Faltando Dados";
                return View();
            }
        }
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult ConfirmarConsulta(Consulta consulta)
        {
            Consulta consultaBanco = dataContext.Consulta.FirstOrDefault(x => x.ID == consulta.ID);//retorna nulo se nao encontrar

            DateTime dataAtual = DateTime.Now;
            if (consultaBanco != null)
            {
                consultaBanco.ConfirmacaoSeVaiAconsulta = consulta.ConfirmacaoSeVaiAconsulta;
                consultaBanco.quemCancelou = consulta.quemCancelou;
                consultaBanco.secretariaCancelamento = consulta.secretariaCancelamento;
                consultaBanco.dataConfirmação = dataAtual.Date;

                try
                {
                    if(consultaBanco.ConfirmacaoSeVaiAconsulta == false)
                    {
                        consultaBanco.Situacao = "Cancelado";
                    }
                    else
                    {
                        consultaBanco.Situacao = "Confirmada";
                    }
                    //consulta.ID = consultaBanco.ID;
                    dataContext.Consulta.Update(consultaBanco);
                    dataContext.SaveChanges();//ta dando erro nessa linha
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
            else
            {
                ViewBag.Erro = "Esta Faltando Dados";
                return View();
            }
        }
    }
}