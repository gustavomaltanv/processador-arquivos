# ExampleProcessor

O **ExampleProcessor** é um serviço em .NET que monitora um diretório para chegada/alterações de arquivos, processa esses arquivos e os move para uma pasta de backup após o processamento. Ele foi projetado para ser executado como um serviço hospedado no Windows.

## Funcionalidades

- Monitora um diretório configurável para arquivos.
- Processa os arquivos encontrados (exemplo: inserção em banco de dados ou outras operações).
- Move os arquivos processados para uma pasta de backup.
- Controla a concorrência no processamento de arquivos para evitar sobrecarga.

## Configuração

### Arquivo `appsettings.json`

O arquivo `appsettings.json` contém as configurações principais de um projeto exemplo:

```json
{
  "Diretorio": {
    "Csv": "C:\\Processor\\Arquivos",
    "Backup": "C:\\Processor\\Backup",
    "Log": "C:\\Processor\\logs"
  }
}
```
* Csv: Diretório onde os arquivos CSV serão monitorados.
* Log: Diretório onde os logs serão armazenados.
* Backup: Diretório onde os arquivos processados serão movidos.

## Como Executar como Windows Service
Este projeto foi projetado para ser executado como um Windows Service. Para instalar o serviço, siga os passos abaixo:

1. Publique o projeto em um diretório específico: ```dotnet publish -c Release -o C:\ExampleProcessor```
2. Use o comando sc para criar o serviço no Windows:```sc create ExampleProcessor binPath= "C:\ExampleProcessor\ExampleProcessor.exe"```
3. Inicie o serviço: ```sc start ExampleProcessor```
