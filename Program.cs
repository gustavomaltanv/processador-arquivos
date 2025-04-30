using Serilog;
using ExampleProcessor.Application;
using ExampleProcessor.Util;

var logDirectory = Configuration.Instance.GetValue("Diretorio:Log");

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(Path.Combine(logDirectory, "log-.txt"), rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog() 
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        var diretorioCsv = Configuration.Instance.GetValue("Diretorio:Csv");
        var pastaBackup = Configuration.Instance.GetValue("Diretorio:Backup");

        services.AddSingleton<IProcessor>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<Processor>>();
            return new Processor(logger, pastaBackup);
        });

        services.AddHostedService(provider =>
        {
            var loggerWatcher = provider.GetRequiredService<ILogger<FileWatcherService>>();
            var processor = provider.GetRequiredService<IProcessor>();

            var watcherService = new FileWatcherService(diretorioCsv, loggerWatcher);

            watcherService.AoEncontrarCsv += processor.ProcessarArquivo;

            return watcherService;
        });
    });

var host = builder.Build();
host.Run();