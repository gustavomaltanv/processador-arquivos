namespace ExampleProcessor.Application
{
    internal interface IProcessor
    {
        Task ProcessarArquivo(string caminhoArquivo);
    }
}
