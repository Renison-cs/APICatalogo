
namespace APICatalogo.Logging
{
    public class CustomerLogger : ILogger
    {
        readonly string loggerName;
        readonly CustomLoggerProviderConfiguration loggerConfig;
        public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
        {
            loggerName = name;
            loggerConfig = config;

        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == loggerConfig.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";
            EscreverTextoNoArquivo(mensagem);
        }
        private void EscreverTextoNoArquivo(string mensagem)
        {
            string caminhoArquivoLog = @"c:\dados\log\Macoratti_log.txt";
            using (StreamWriter sw = new StreamWriter(caminhoArquivoLog, true))
            {
                try
                {
                    sw.WriteLine(mensagem);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao escrever no arquivo de log: {ex.Message}");
                }
            }
        }
    }
}

