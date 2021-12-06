
using Microsoft.AspNetCore.Mvc;
using ClinicaMedica.Data;
using ClinicaMedica.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace ClinicaMedica.Controllers
{
    /*
        1 - Agenda - (1H) OK
        Passo 1: Criar Filtro por data (OK)
        Passo 2: Apenas Receber do medicom do crm logado e do dia de hoje ate N. Não dos dias anteriores
        Obrigando o mesmo a usar visualizar (OK)

        2 - Dentro da consulta - (3H) OK
         Passo 1: Incluir 2 Botoes, Inicio e Fim (OK)
         Passo 2: Botão Inicio ativa o relogio pegando o horario do sistema. (dd/mm/aaaa hh:mm) (+/-)
         Passo 3: Finalizar Consulta deve
            Salvar Hora (OK com erro)
            Mudar o Status (OK)
            Travar Botoes adicionados (OK)

            ideia do Gostoso
        " Verificar assim que clicado, dentro de um if se os valores realmente estão liberados para serem editados.
        Umas consulta que ainda não foi finalizada, caso for. Dar um !Alerta!
        Prezado Dr. Arrombado, essa consulta já foi finalizada"

        3 - Criar Nova Views (Visualizar) - (2H)
        Passo 1: Difernte da consulta, ela nos trará todos as consultas ja finalizadas e canceladas
        Passo 2: Criar filtro por CPF or Nome
        Passo 3: Lembrar de colocar apenas botão voltar, sem deixar editar nada
        NÃO É PRA EDITAR NADAAAAAAAAAAA
         */
    public class MedicoCOntroller : Controller
    {
        private readonly DataContext dataContext;

        public MedicoCOntroller(DataContext dc)
        {
            dataContext = dc;
        }

        public IActionResult Index(int? Id)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Medico")
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home

            Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.ID == Id);
            string consulta = " SELECT * FROM Consulta where 1 = 1 ";
            List<object> parametros = new List<object>();
            if (usuarioBanco != null)
            {
                string hoje = DateTime.Now.Date.ToString("yyyy'-'MM'-'dd");
                consulta += " AND Crm = @Crm AND dataConsulta like '%' + @hoje + '%'";
                parametros.Add(new SqlParameter("Crm", usuarioBanco.Crm));
                parametros.Add(new SqlParameter("hoje", hoje));
            }
            List<Consulta> lista = dataContext.Consulta.FromSqlRaw(consulta, parametros.ToArray()).ToList();
            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(int? Id, Consulta Agenda)
        {
            {
                if (Agenda.ID == null)
                {
                    //return NoContent();//não faz nada, retorna nada, nenhuma ação
                    return RedirectToAction("Index");
                }
                else
                {
                    //----------------------------------------------------------------------
                    string consulta = " SELECT * FROM Consulta where 1 = 1 ";
                    List<object> parametros = new List<object>();

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
                    //consulta += " order by dataCadastro ";

                    Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.ID == Id);
                    string hoje = DateTime.Now.Date.ToString("yyyy'-'MM'-'dd");
                    consulta += " AND Crm = @Crm AND dataConsulta like '%' + @hoje + '%' order by dataCadastro ";
                    parametros.Add(new SqlParameter("Crm", usuarioBanco.Crm));
                    parametros.Add(new SqlParameter("hoje", hoje));

                    List<Consulta> lista = dataContext.Consulta.FromSqlRaw(consulta, parametros.ToArray()).ToList();
                    //----------------------------------------------------------------------
                    return View(lista);
                }
            }
        }

        public IActionResult IndexHistorico(int? Id)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Medico")
            {
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IndexHistorico(Consulta Agenda)
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
                    if (usuarioBanco != null)
                    {
                        consulta += " AND Cpf like @Cpf ";
                        parametros.Add(new SqlParameter("Cpf", Agenda.Cpf));
                    }
                    else
                    {
                        ViewBag.TipoMensagem = "ERRO";
                        ViewBag.Mensagem = "CPF INVALIDO";
                        return View();
                    }
                }

                consulta += " order by dataCadastro ";

                List<Consulta> lista = dataContext.Consulta.FromSqlRaw(consulta, parametros.ToArray()).ToList();
                //----------------------------------------------------------------------
                return View(lista);
            }
        }

        public IActionResult Historico(int? ID)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Medico")
            {
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home
            }

            if (ID.HasValue)
            {
                Consulta consulta = dataContext.Consulta.FirstOrDefault(x => x.ID == ID);
                if (consulta != null)
                {
                    dataContext.SaveChanges();
                    return View(consulta);
                }
            }
            return NoContent();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Historico(Consulta consulta)
        //{

        //}

        public IActionResult Anotacoes(int? ID)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Medico")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ID.HasValue)
            {
                Consulta consulta = dataContext.Consulta.FirstOrDefault(x => x.ID == ID);
                if (consulta != null)
                {
                    dataContext.SaveChanges();
                    return View(consulta);
                }
            }
            return NoContent();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anotacoes(Consulta consulta)
        {

            Consulta Iniciando = new Consulta();
            Iniciando = consulta;
            List<Consulta> validacaoDeCpf = dataContext.Consulta.ToList();
            int i = consulta.ID.Value;
            i = i - 1;
            if (consulta.ID == validacaoDeCpf[i].ID)
            {
                Iniciando = validacaoDeCpf[i];
            }

            if (Iniciando.InicioConsulta == Convert.ToDateTime("01/01/0001 00:00:00")) //Iniciar Consulta
            {
                DateTime HoraDoShow = DateTime.Now;
                Iniciando.InicioConsulta = HoraDoShow;
                dataContext.Consulta.Update(Iniciando);
                dataContext.SaveChanges();
                return RedirectToAction();
            }
            else
            {
                DateTime horaFim = DateTime.Now;
                Iniciando.FimConsulta = horaFim;
                //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                if (consulta.Tipo == "Criptografar")
                {
                    string encrypted = Encrypt(consulta);
                    ViewBag.TipoMensagem = "SUCESSO";
                    ViewBag.Mensagem = encrypted;

                    Iniciando.Anotacoes = encrypted;

                    if (encrypted != "Erro")
                    {
                        ViewBag.TipoMensagem = "SUCESSO";
                        ViewBag.Mensagem = encrypted;
                    }
                    else
                    {
                        ViewBag.TipoMensagem = "ERRO";
                        ViewBag.Mensagem = "Erro nas strings";
                    }
                }

                if (consulta.Tipo == "Descriptografar")
                {
                    string decrypted = Decrypt(consulta);

                    consulta.Anotacoes = decrypted;

                    if (decrypted != "Erro")
                    {
                        ViewBag.TipoMensagem = "DESENCRIPTOGRAFAR";
                        ViewBag.Mensagem = decrypted;
                    }
                    else
                    {
                        ViewBag.TipoMensagem = "ERRO";
                        ViewBag.Mensagem = "Erro na desencriptação";
                    }
                    return View();
                }
                //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                if (Iniciando.Situacao == "Concluida")
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "Não é possivel editar uma consulta já concluida";
                    return View();
                }
                Iniciando.Situacao = "Concluida";
                dataContext.Consulta.Update(Iniciando);
                dataContext.SaveChanges();

                ViewBag.TipoMensagem = "SUCESSO";
                ViewBag.Mensagem = " anotações salvas";
                return View();
            }
        }

        public string Encrypt(Consulta informacoes)
        {
            try
            {
                string textToEncrypt = informacoes.Anotacoes;
                string ToReturn = "";
                string publickey = informacoes.Chave;
                string secretkey = informacoes.Chave;
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message, ex.InnerException);
                return "Erro";
            }
        }

        public string Decrypt(Consulta informacoes)
        {
            try
            {
                string textToDecrypt = informacoes.Anotacoes;
                string ToReturn = "";
                string publickey = informacoes.Chave;
                string secretkey = informacoes.Chave;
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                //throw new Exception(ae.Message, ae.InnerException);
                return "Erro";
            }
        }
    }
}
