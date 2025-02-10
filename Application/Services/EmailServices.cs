using lerXML.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Application.Services
{
    public class EmailServices
    {
        private readonly Email _email;
        private readonly JsonServices _jsonServices;

        public EmailServices(JsonServices jsonServices )
        {
            _jsonServices = jsonServices;
        }

        public void EnviarEmail(Email emailConfig)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(emailConfig.ServidorSMTP)
                {
                    Port = emailConfig.Porta,
                    Credentials = new NetworkCredential(emailConfig.Usuario, emailConfig.Senha),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(emailConfig.Usuario),
                    Subject = emailConfig.Assunto,
                    Body = emailConfig.Mensagem,
                    IsBodyHtml = false
                };

                mail.To.Add(emailConfig.Destinatario);
                if (!string.IsNullOrEmpty(emailConfig.Copia))
                {
                    mail.CC.Add(emailConfig.Copia);
                }

                smtpClient.Send(mail);
                Console.WriteLine("E-mail enviado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        public async Task EnviarEmailComAnexos(string pastaArquivos, int year, int month)
        {
            try
            {
                // 🔹 Lendo configurações do JSON
                var emailConfig = _jsonServices.LerConfiguracaoEmail();

                string usuario = emailConfig.Usuario;
                string senha = emailConfig.Senha;
                string servidorSMTP = emailConfig.ServidorSMTP;
                int porta = emailConfig.Porta;
                string destinatario = emailConfig.Destinatario;
                string copia = emailConfig.Copia;
                string assunto = emailConfig.Assunto;
                string mensagem = emailConfig.Mensagem;

                try
                {
                    using (SmtpClient smtpClient = new SmtpClient(servidorSMTP, porta))
                    {
                        smtpClient.Credentials = new NetworkCredential(usuario, senha);
                        smtpClient.EnableSsl = false;

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(usuario);
                        mail.To.Add(destinatario);
                        mail.Subject = assunto;
                        mail.Body = mensagem;

                        string[] arquivos = Directory.GetFiles(pastaArquivos, "*.*")
                                             .Where(file => file.EndsWith(".pdf") || file.EndsWith(".7z"))
                                             .ToArray();

                        foreach (var arquivo in arquivos)
                        {
                            mail.Attachments.Add(new Attachment(arquivo));
                        }

                        await smtpClient.SendMailAsync(mail);
                        MessageBox.Show("Email enviado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch(Exception exe)
                {
                    MessageBox.Show($"Email não enviado. {exe}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar e-mail: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }
}
