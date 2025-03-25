using lerXML.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lerXML.Application.Services
{
    public class JsonServices
    {
        private readonly string _jsonFilePath;
        private JsonDocument _jsonDocument;

        public JsonServices(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
            CarregarJson();
        }
        private void CarregarJson()
        {
            if (!File.Exists(_jsonFilePath))
            {
                throw new FileNotFoundException($"Arquivo JSON não encontrado: {_jsonFilePath}");
            }

            string jsonContent = File.ReadAllText(_jsonFilePath);
            _jsonDocument = JsonDocument.Parse(jsonContent);
        }

        public List<string> PegarCaminhosFiscais(string chave)
        {
            List<string> caminhos = new List<string>();

            if (_jsonDocument.RootElement.TryGetProperty("caminhosFiscais", out JsonElement caminhosFiscais))
            {
                if (caminhosFiscais.TryGetProperty(chave, out JsonElement listaCaminhos))
                {
                    foreach (JsonElement item in listaCaminhos.EnumerateArray())
                    {
                        if (item.TryGetProperty("caminhoCupom", out JsonElement caminhoCupom))
                        {
                            caminhos.Add(caminhoCupom.GetString());
                        }
                        if (item.TryGetProperty("caminhoNota", out JsonElement caminhoNota))
                        {
                            caminhos.Add(caminhoNota.GetString());
                        }
                        if (item.TryGetProperty("caminhoNFCE", out JsonElement caminhoNFCE))
                        {
                            caminhos.Add(caminhoNFCE.GetString());
                        }
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"A chave '{chave}' não foi encontrada nos caminhos fiscais.");
                }
            }
            else
            {
                throw new KeyNotFoundException("A seção 'caminhosFiscais' não foi encontrada no JSON.");
            }

            return caminhos;
        }

        public dynamic LerConfiguracaoEmail()
        {
            string jsonContent = File.ReadAllText(_jsonFilePath);
            var jsonDocument = JsonDocument.Parse(jsonContent);

            if (jsonDocument.RootElement.TryGetProperty("email", out JsonElement emailConfig))
            {
                try
                {
                    return new
                    {
                        Usuario = emailConfig.GetProperty("Usuario").GetString(),
                        Senha = emailConfig.GetProperty("Senha").GetString(),
                        ServidorSMTP = emailConfig.GetProperty("ServidorSMTP").GetString(),
                        Porta = emailConfig.GetProperty("Porta").GetInt32(),
                        Destinatario = emailConfig.GetProperty("Destinatario").GetString(),
                        Copia = emailConfig.GetProperty("Copia").GetString(),
                        Assunto = emailConfig.GetProperty("Assunto").GetString(),
                        Mensagem = emailConfig.GetProperty("Mensagem").GetString(),
                        SslMode = Convert.ToBoolean(emailConfig.GetProperty("SSL").GetString())
                    };
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Erro ao ler o arquivo Json: " + ex.Message);
                }
            }

            throw new FileNotFoundException("Configuração de e-mail não encontrada no JSON.");
        }
    }
}
