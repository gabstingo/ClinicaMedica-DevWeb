using ClinicaMedica.Data;
using ClinicaMedica.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaMedica.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext dataContext;

        //construtor
        public LoginController(DataContext dc)
        {
            dataContext = dc;
        }

        //Este método é chamado para exibir a página
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) // Este if retorna true se estiver logado ou false se nao estiver.
                return RedirectToAction("Index", "Home");//Caso esteje logado nao deixa acessar a pagina de login redirecionando para a home

            return View();
        }

        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult Index(Usuario login)
        {
            login.Senha = ComputeSha256Hash(login.Senha);

            //if (ModelState.IsValid)//esse comentado serve para validar se toda a model esta correta de acordo com a modelagem
            if (login.Email != null && login.Senha != null)
            {

                login.Ativo = true;//Esse login é colocado como ativo para poder conferir se o usuario do banco é igual ativo tbm.

                //bool fazerLogin = dataContext.Logins.Any(x => x.Email == login.Email && x.Senha == login.Senha);//outra opcao que nao usei
                Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.Email == login.Email && x.Senha == login.Senha && x.Ativo == login.Ativo);//retorna nulo se nao encontrar

                if (usuarioBanco != null)
                {
                    if (usuarioBanco.PrimeiroAcesso == true && usuarioBanco.TipoUsuario == "Administrador")
                    {
                        List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, usuarioBanco.ID.ToString()),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Name, usuarioBanco.Nome),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Role, usuarioBanco.TipoUsuario),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Email, usuarioBanco.Email)
                    };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                            IsPersistent = true,
                        };

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        return RedirectToAction("PrimeiroAcesso", "Login");
                    }
                    else if (usuarioBanco.PrimeiroAcesso == true && usuarioBanco.TipoUsuario == "Medico")
                    {
                        List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, usuarioBanco.ID.ToString()),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Name, usuarioBanco.Nome),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Role, usuarioBanco.TipoUsuario),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Email, usuarioBanco.Email)
                    };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                            IsPersistent = true,
                        };

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        return RedirectToAction("PrimeiroAcesso", "Login");
                    }
                    else if (usuarioBanco.PrimeiroAcesso == true && usuarioBanco.TipoUsuario == "Secretaria")
                    {
                        List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, usuarioBanco.ID.ToString()),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Name, usuarioBanco.Nome),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Role, usuarioBanco.TipoUsuario),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Email, usuarioBanco.Email)
                    };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                            IsPersistent = true,
                        };

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        return RedirectToAction("PrimeiroAcesso", "Login");
                    }
                    else
                    {
                        List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, usuarioBanco.ID.ToString()),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Name, usuarioBanco.Nome),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Role, usuarioBanco.TipoUsuario),//formas de trazer os dados para caso precise usar
                        new Claim(ClaimTypes.Email, usuarioBanco.Email)
                    };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                            IsPersistent = true,
                        };

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Erro = "Usuário e/ou senha inválidos";
            return View();
        }

        public IActionResult Logoff()
        {
            HttpContext.SignOutAsync();         //Efetiva a saída do sistema
            return RedirectToAction("Index");   //Redireciona para a ação Index do controller atual
        }

        public IActionResult PrimeiroAcesso()
        {
            return View();
        }

        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult PrimeiroAcesso(Usuario novoLogin)
        {
            novoLogin.Senha = ComputeSha256Hash(novoLogin.Senha);
            novoLogin.NovaSenha = ComputeSha256Hash(novoLogin.NovaSenha);

            int IDUsuarioBanco = int.Parse(User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Sid).Value);

            string TipoUsuarioBanco = User.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value;

            Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.ID == IDUsuarioBanco);//retorna nulo se nao encontrar

            //if (ModelState.IsValid)//esse comentado serve para validar se toda a model esta correta de acordo com a modelagem
            if (novoLogin.Email != null && novoLogin.Senha != null && novoLogin.NovaSenha != null)
            {
                if (usuarioBanco.Senha != novoLogin.NovaSenha)
                {
                    if (usuarioBanco.PrimeiroAcesso == true && usuarioBanco.TipoUsuario == TipoUsuarioBanco)
                    {
                        if (novoLogin.Senha == novoLogin.NovaSenha)
                        {
                            usuarioBanco.Email = novoLogin.Email;
                            usuarioBanco.Senha = novoLogin.Senha;
                            usuarioBanco.PrimeiroAcesso = false;
                        }
                        else
                        {
                            ViewBag.TipoMensagem = "ERRO";
                            ViewBag.Mensagem = "As senhas não são iguais";
                            return View();
                        }

                        if (dataContext.Logins.Any(x => x.ID == usuarioBanco.ID))
                        {
                            try
                            {
                                dataContext.Logins.Update(usuarioBanco);
                                dataContext.SaveChanges();
                            }
                            catch
                            {
                                ViewBag.TipoMensagem = "ERRO";
                                ViewBag.Mensagem = "O login não pode ser atualizado";
                                return View();
                            }

                            //ViewBag.TipoMensagem = "SUCESSO";
                            //ViewBag.Mensagem = "Senha atualizada com sucesso";
                            //return View();

                            TempData["TipoMensagem"] = "SUCESSO";
                            TempData["Mensagem"] = "Senha atualizada com sucesso";
                            //Logoff();

                        }
                        Logoff();
                        return RedirectToAction("Index", "Login");
                    }
                }
                else
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "A nova senha é igual a senha anterior, por favor, colocar uma senha diferente.";
                    return View();
                }
            }

            ViewBag.Erro = "Usuário e/ou senha inválidos";
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

        //Este método é chamado para exibir a página
        public IActionResult RecuperarSenha()
        {
            if (User.Identity.IsAuthenticated) // Este if retorna true se estiver logado ou false se nao estiver.
                return RedirectToAction("Index", "Home");//Caso esteje logado nao deixa acessar a pagina de login redirecionando para a home

            return View();
        }

        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult RecuperarSenha(Usuario login)
        {

            //if (ModelState.IsValid)//esse comentado serve para validar se toda a model esta correta de acordo com a modelagem
            if (login.Email != null)
            {

                login.Ativo = true;

                //bool fazerLogin = dataContext.Logins.Any(x => x.Email == login.Email && x.Senha == login.Senha);//outra opcao que nao usei
                Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.Email == login.Email && x.Ativo == login.Ativo);//retorna nulo se nao encontrar

                if (usuarioBanco != null)
                {
                    // Gera uma senha com 6 caracteres entre numeros e letras
                    string chars = "abcdefghjkmnpqrstuvwxyz023456789";
                    string pass = "";
                    Random random = new Random();
                    for (int f = 0; f < 6; f++)
                    {
                        pass = pass + chars.Substring(random.Next(0, chars.Length - 1), 1);
                    }

                    string pagina = "https://localhost:44375/Login/PaginaDeLinkRecuperacaoSenha";

                    usuarioBanco.TempoMaxRecupSenha = DateTime.Now.AddMinutes(1); //@@@@@@@@@@@lembrar de colocar aqui o .date e dar um tempo de 1 dia @@@@@@@@@@@@@

                    usuarioBanco.SenhaRecuperacao = ComputeSha256Hash(pass);

                    dataContext.Logins.Update(usuarioBanco);
                    dataContext.SaveChanges();

                    email(usuarioBanco.Email, "Link para recuperar senha do sistema Clinica Medica", $"O link da pagina é: {pagina}, e a senha de recuperação é: {pass}");

                    ViewBag.TipoMensagem = "SUCESSO";
                    ViewBag.Mensagem = "O link foi enviado com sucesso para o email digitado.";

                    return View();
                }
            }

            ViewBag.Erro = "Email não encontrado e/ou não está ativo no sistema.";
            return View();
        }

        public IActionResult PaginaDeLinkRecuperacaoSenha()
        {
            return View();
        }

        //Requisição do botão "submit"
        [HttpPost]//Para poder receber envios por método post
        [ValidateAntiForgeryToken]//Para garantir que apenas a nossa view criada tenha acesso a esse método index
        public IActionResult PaginaDeLinkRecuperacaoSenha(Usuario cadastrarUsuario)
        {
            if (cadastrarUsuario.SenhaRecuperacao != null && cadastrarUsuario.NovaSenha != null && cadastrarUsuario.NovaSenha2 != null)
            {
                cadastrarUsuario.SenhaRecuperacao = ComputeSha256Hash(cadastrarUsuario.SenhaRecuperacao);
                
                Usuario usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.Email == cadastrarUsuario.Email && x.SenhaRecuperacao == cadastrarUsuario.SenhaRecuperacao);//retorna nulo se nao encontrar

                if (usuarioBanco != null)
                {
                    if (DateTime.Now < usuarioBanco.TempoMaxRecupSenha)
                    {
                        if (cadastrarUsuario.NovaSenha == cadastrarUsuario.NovaSenha2)
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
                    else
                    {
                        ViewBag.TipoMensagem = "ERRO";
                        ViewBag.Mensagem = "Link de recuperação de senha expirado.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.TipoMensagem = "ERRO";
                    ViewBag.Mensagem = "Informações incorretas.";
                    return View();
                }
            }

            ViewBag.Erro = "Usuário inválido";
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
    }
}
