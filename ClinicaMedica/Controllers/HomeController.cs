using ClinicaMedica.Data;
using ClinicaMedica.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaMedica.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]//para fazer com que a classe toda nao utilizem cache
    public class HomeController : Controller
    {
        private readonly DataContext dataContext;

        //construtor
        //public HomeController(DataContext dc)
        //{
        //    dataContext = dc;
        //}

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext dc)
        {
            _logger = logger;

            dataContext = dc;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CadastroUsuario()
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Administrador")
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de cadastrar usuario redirecionando para a home

            return View();
        }

        //Video caso queira tirar duvidas https://www.youtube.com/watch?v=c7SMAC6Xn90
        //Vamos usar no final do codigo para testar se está enviando 
        public static bool teste;

        public object email(string para, string assunto, string corpo)
        {
            teste = false;
            SmtpClient smpt = new SmtpClient();
            MailMessage mail = new MailMessage();
            //Não estamos usando envio de anexo. Mas caso fosse necessario 
            //Attachment anexar = new Attachment(String anexo);
            //mail.Attachments.Add(anexo)

            smpt.Host = "smtp.gmail.com";
            smpt.Port = 587;

            smpt.EnableSsl = true;

            smpt.UseDefaultCredentials = false; //Importar as Credenciais

            //Usuario e Senha do Email
            smpt.Credentials = new System.Net.NetworkCredential("testesdesitesnakamura@gmail.com", "Testesdesitesnakamur@");

            mail.From = new MailAddress("muriloarceniorosa@gmail.com"); //Necessario informar de onde vai o email

            if (!string.IsNullOrWhiteSpace(para))//Se não for nulo ou espaço em branco
            {
                mail.To.Add(new MailAddress(para));
                teste = true;
            }
            else
            {
                //Não vejo necessidade, ja que ele começa como falso.
                teste = false;
            }
            mail.Subject = assunto;
            mail.Body = corpo;

            //Variavel Principal de Conecção que enviarar
            smpt.Send(mail);
            return teste;
        }

        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult CadastroUsuario(Usuario cadastrarUsuario)
        {
            // Gera uma senha com 6 caracteres entre numeros e letras
            string chars = "abcdefghjkmnpqrstuvwxyz023456789";
            string pass = "";
            Random random = new Random();
            for (int f = 0; f < 6; f++)
            {
                pass = pass + chars.Substring(random.Next(0, chars.Length - 1), 1);
            }

            cadastrarUsuario.Senha = pass;
            cadastrarUsuario.Ativo = true;
            cadastrarUsuario.PrimeiroAcesso = true;

            if (ValidaCpf(cadastrarUsuario.Cpf) == false)
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "CPF inválido";
                return View();
            }
            //public object email(string para, string assunto, string corpo)
            email(cadastrarUsuario.Email, "Nova senha do software Clinica Medica", $" Sua senha de acesso é: {cadastrarUsuario.Senha}");

            cadastrarUsuario.Senha = ComputeSha256Hash(pass);

            //if (ModelState.IsValid == false)
            //{
            //    ViewBag.TipoMensagem = "ERRO";
            //    ViewBag.Mensagem = "Os dados preenchidos estão incorretos";
            //    return View();
            //}

            //if (cadastrarUsuario.Email != null && cadastrarUsuario.Senha != null && cadastrarUsuario.NovaSenha != null)
            if (cadastrarUsuario.TipoUsuario != null && cadastrarUsuario.Nome != null &&
                cadastrarUsuario.Crm != null && cadastrarUsuario.Email != null && cadastrarUsuario.DataNasc != null &&
                cadastrarUsuario.Cpf != null && cadastrarUsuario.Senha != null)//esse comentado serve para validar se toda a model esta correta de acordo com a modelagem
            {
                DateTime dataAtual = DateTime.Now.Date;

                if (cadastrarUsuario.DataNasc > dataAtual)
                {
                    //TempData["TipoMensagem"] = "ERRO";
                    //TempData["Mensagem"] = $"Usuário {cadastrarUsuario.Nome} não foi salvo pois não é permitido o cadastro com data de nascimento futura";
                    //return RedirectToAction("Index");

                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = $"Usuário {cadastrarUsuario.Nome} não foi salvo pois não é permitido o cadastro com data de nascimento futura";
                    return View();
                }

                try
                {
                    dataContext.Logins.Add(cadastrarUsuario);
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

                //TempData["TipoMensagem"] = "SUCESSO";
                //TempData["Mensagem"] = "Senha atualizada com sucesso";
                //Logoff();
                //return RedirectToAction("CadastroUsuario", "Home");
            }

            ViewBag.Erro = "Usuário inválido";
            return View();
        }

        public string ComputeSha256Hash(string senha)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(senha));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();//Gerando o primeiro hash
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                byte[] bytes2 = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));

                StringBuilder builder2 = new StringBuilder();//Gerando o hash duplo, fazendo a hash da primeira hash
                for (int i = 0; i < bytes2.Length; i++)
                {
                    builder2.Append(bytes2[i].ToString("x2"));
                }

                return builder2.ToString();
            }
        }

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

        public IActionResult VisualizarUsuarios()
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Administrador")
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de Visualizar usuarios redirecionando para a home

            List<Usuario> lista = dataContext.Logins.OrderBy(x => x.Nome).ToList();

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VisualizarUsuarios(Usuario usuario)
        {
            if (usuario == null)
            {
                //return NoContent();//não faz nada, retorna nada, nenhuma ação
                return RedirectToAction("Index");
            }
            else
            {
                if (usuario.Nome == null)
                    usuario.Nome = "";

                //----------------------------------------------------------------------
                string consulta = " SELECT * FROM Logins where 1 = 1 ";
                List<object> parametros = new List<object>();

                if (usuario.Nome != null)
                {
                    consulta += " AND Nome like '%' + @Nome + '%' ";
                    parametros.Add(new SqlParameter("Nome", usuario.Nome));
                }

                if (usuario.Ativo == true)
                {
                    consulta += " AND Ativo = @Ativo ";
                    parametros.Add(new SqlParameter("Ativo", usuario.Ativo));
                }

                if (usuario.Ativo == false)
                {
                    consulta += " AND Ativo = @Ativo ";
                    parametros.Add(new SqlParameter("Ativo", usuario.Ativo));
                }

                consulta += " order by Nome ";

                List<Usuario> lista = dataContext.Logins.FromSqlRaw(consulta, parametros.ToArray()).ToList();
                //----------------------------------------------------------------------
                return View(lista);
            }
        }

        public IActionResult EditarUsuario(int? Id)
        {
            if (User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value != "Administrador")
                return RedirectToAction("Index", "Home");//Caso não seja administrador nao deixa acessar a pagina de Visualizar usuarios redirecionando para a home

            if (Id.HasValue)
            {
                Usuario usuario = dataContext.Logins.FirstOrDefault(x => x.ID == Id);
                if (usuario != null)
                    return View(usuario);
            }

            return NoContent();
        }

        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult EditarUsuario(Usuario cadastrarUsuario)
        {

            Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.ID == cadastrarUsuario.ID);//retorna nulo se nao encontrar

            usuarioBanco.TipoUsuario = cadastrarUsuario.TipoUsuario;
            usuarioBanco.Ativo = cadastrarUsuario.Ativo;
            usuarioBanco.Nome = cadastrarUsuario.Nome;
            usuarioBanco.Crm = cadastrarUsuario.Crm;
            usuarioBanco.Email = cadastrarUsuario.Email;
            usuarioBanco.DataNasc = cadastrarUsuario.DataNasc;
            usuarioBanco.Cpf = cadastrarUsuario.Cpf;

            if (ValidaCpf(usuarioBanco.Cpf) == false)
            {
                ViewBag.TipoMensagem = "ERRO";
                ViewBag.Mensagem = "CPF inválido";
                return View();
            }

            if (cadastrarUsuario.ID != null && cadastrarUsuario.TipoUsuario != null && cadastrarUsuario.Ativo != null && cadastrarUsuario.Nome != null &&
                cadastrarUsuario.Crm != null && cadastrarUsuario.Email != null && cadastrarUsuario.DataNasc != null &&
                cadastrarUsuario.Cpf != null)
            {
                DateTime dataAtual = DateTime.Now.Date;

                if (usuarioBanco.DataNasc > dataAtual)
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = $"Usuário {usuarioBanco.Nome} não foi atualizado pois não é permitido o cadastro com data de nascimento futura";
                    return View();
                }

                try
                {
                    dataContext.Logins.Update(usuarioBanco);
                    dataContext.SaveChanges();
                }
                catch
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "O usuário não pode ser atualizado";
                    return View();
                }

                ViewBag.TipoMensagem = "SUCESSO";
                ViewBag.Mensagem = "Usuário atualizado com sucesso";
                return View();
            }

            ViewBag.Erro = "Usuário inválido";
            return View();
        }
        public IActionResult EditarSenha(int? Id)
        {
            if (Id.HasValue)
            {
                Usuario usuario = dataContext.Logins.FirstOrDefault(x => x.ID == Id);
                if (usuario != null)
                    return View(usuario);
            }

            return NoContent();
        }

        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult EditarSenha(Usuario cadastrarUsuario)
        {
            if (cadastrarUsuario.Senha != null && cadastrarUsuario.NovaSenha != null && cadastrarUsuario.NovaSenha2 != null)
            {
                Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.ID == cadastrarUsuario.ID);//retorna nulo se nao encontrar

                cadastrarUsuario.Senha = ComputeSha256Hash(cadastrarUsuario.Senha);

                if(usuarioBanco.Senha == cadastrarUsuario.Senha && cadastrarUsuario.NovaSenha == cadastrarUsuario.NovaSenha2)
                {
                    cadastrarUsuario.NovaSenha = ComputeSha256Hash(cadastrarUsuario.NovaSenha);

                    if (usuarioBanco.Senha != cadastrarUsuario.NovaSenha)
                    {
                        usuarioBanco.Senha = cadastrarUsuario.NovaSenha;

                        try
                        {
                            dataContext.Logins.Update(usuarioBanco);
                            dataContext.SaveChanges();
                            TempData["TipoMensagem"] = "SUCESSO";
                            TempData["Mensagem"] = "Senha atualizada com sucesso";
                            Logoff();
                            return RedirectToAction("Index", "Login");
                        }
                        catch
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "A senha não pode ser atualizada";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.TipoMensagem = "ERRO";
                        ViewBag.Mensagem = "A nova senha é igual a senha anterior, por favor, colocar uma senha diferente.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "Erro em alguma das senhas digitadas";
                    return View();
                }
            }

            ViewBag.Erro = "Usuário inválido";
            return View();
        }

        public IActionResult Logoff()
        {
            HttpContext.SignOutAsync();         //Efetiva a saída do sistema
            return RedirectToAction("Index");   //Redireciona para a ação Index do controller atual
        }
    }
}
