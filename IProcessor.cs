namespace SimpleCsvProcessor
{
    internal interface IProcessor
    {
        Task ProcessarArquivo(string caminhoArquivo);
    }
}
