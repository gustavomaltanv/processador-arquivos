namespace SimpleCsvProcessor
{
    internal class Processor : IProcessor
    {
        private readonly ILogger<Processor> _logger;
        private readonly string _pastaBackup;

        public Processor(ILogger<Processor> logger, string pastaBackup)
        {
            _logger = logger;
            _pastaBackup = pastaBackup;
        }
        public async Task ProcessarArquivo(string caminhoArquivo)
        {
            try
            {
                _logger.LogInformation("Processando arquivo: {Arquivo}", caminhoArquivo);

                // await -> insercao no banco de dados

                var nomeArquivo = Path.GetFileName(caminhoArquivo);
                var destino = Path.Combine(_pastaBackup, nomeArquivo);

                File.Move(caminhoArquivo, destino, overwrite: true);

                _logger.LogInformation("Arquivo {Arquivo} processado e movido para Backup.", caminhoArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar arquivo {Arquivo}", caminhoArquivo);
            }
        }
    }
}
