using SimpleCsvProcessor;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(
        (context, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        var diretorioCsv = configuration.GetValue<string>("Diretorio:Csv");
        var diretorioLog = configuration.GetValue<string>("Diretorio:Log");
        var pastaBackup = configuration.GetValue<string>("Diretorio:Backup");

        services.AddSingleton<IProcessor>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<Processor>>();
            return new Processor(logger, pastaBackup);
        });

        services.AddHostedService(provider =>
        {
            var loggerWatcher = provider.GetRequiredService<ILogger<CsvWatcherService>>();
            var processor = provider.GetRequiredService<IProcessor>();

            var watcherService = new CsvWatcherService(diretorioCsv, loggerWatcher);

            watcherService.AoEncontrarCsv += processor.ProcessarArquivo;

            return watcherService;
        });
    });





var host = builder.Build();
host.Run();
