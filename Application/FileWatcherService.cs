namespace ExampleProcessor.Application
{
    public delegate Task EventHandler(string caminhoArquivo);
    internal class FileWatcherService : BackgroundService
    {
        private readonly string _diretorioCsv;
        private readonly ILogger<FileWatcherService> _logger;
        private Timer? _timer;
        private readonly TimeSpan _intervaloVerificacao = TimeSpan.FromSeconds(60);
        private readonly SemaphoreSlim _semaforo;
        private readonly int _limiteConcorrencia = 5;

        public event EventHandler? AoEncontrarCsv;
        public FileWatcherService(string diretorioCsv, ILogger<FileWatcherService> logger)
        {
            _diretorioCsv = diretorioCsv;
            _logger = logger;
            _semaforo = new SemaphoreSlim(_limiteConcorrencia);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!Directory.Exists(_diretorioCsv))
            {
                throw new DirectoryNotFoundException($"O diretório {_diretorioCsv} não foi encontrado.");
            }

            _logger.LogInformation("Iniciando monitoramento do diretório {Diretorio}", _diretorioCsv);

            while (!stoppingToken.IsCancellationRequested)
            {
                await VerificarArquivosAsync(stoppingToken);

                await Task.Delay(_intervaloVerificacao, stoppingToken);
            }
        }

        private async Task VerificarArquivosAsync(CancellationToken stoppingToken)
        {
            try
            {
                var arquivosCsv = Directory.GetFiles(_diretorioCsv, "*.csv", SearchOption.AllDirectories);

                var tasks = new List<Task>();

                foreach (var arquivo in arquivosCsv)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    _logger.LogInformation("Verificando arquivo {Arquivo}", arquivo);

                    if (EsperarArquivoDisponivel(arquivo))
                    {
                        if (AoEncontrarCsv != null)
                        {
                            // Cria a task de processamento controlada pelo semáforo
                            var task = ProcessarArquivoComControleAsync(arquivo, stoppingToken);
                            tasks.Add(task);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Arquivo {Arquivo} ainda não está disponível para leitura. Nova tentativa na próxima rodada.", arquivo);
                    }
                }

                // Aguarda todas as tasks finalizarem
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar arquivos CSV.");
            }
        }

        private async Task ProcessarArquivoComControleAsync(string caminhoArquivo, CancellationToken cancellationToken)
        {
            await _semaforo.WaitAsync(cancellationToken); // Respeita o limite

            try
            {
                await AoEncontrarCsv!.Invoke(caminhoArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar arquivo {Arquivo}", caminhoArquivo);
            }
            finally
            {
                _semaforo.Release(); // Libera o semáforo
            }
        }

        private bool EsperarArquivoDisponivel(string caminhoArquivo, int tentativas = 5, int intervaloMs = 1000)
        {
            for (int i = 0; i < tentativas; i++)
            {
                try
                {
                    using (var stream = new FileStream(caminhoArquivo, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        if (stream.Length > 0)
                        {
                            return true;
                        }
                    }
                }
                catch (IOException)
                {
                    _logger.LogWarning($"Arquivo ainda não está disponível para leitura.");

                    Thread.Sleep(intervaloMs);
                }
            }

            return false;
        }

        public override void Dispose()
        {
            base.Dispose();
            _semaforo.Dispose();
        }
    }
}
