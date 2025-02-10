using lerXML.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Application.Services
{
    public class ConfiguracaoService
    {
        private readonly string _configFilePath;

        public ConfiguracaoService(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public Configuracao CarregarConfiguracao()
        {
            if (!File.Exists(_configFilePath))
            {
                throw new FileNotFoundException("Arquivo de configuração não encontrado.");
            }

            string json = File.ReadAllText(_configFilePath);
            return JsonConvert.DeserializeObject<Configuracao>(json) ?? new Configuracao();
        }

        public void SalvarConfiguracao(Configuracao configuracao)
        {
            try
            {
                string json = JsonConvert.SerializeObject(configuracao, Formatting.Indented);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar configurações: {ex.Message}");
            }
        }
    }
}
