# Console Selenium 41

Atividade de Garantia da Qualidade de Software: automação do [formulário web do Selenium](https://www.selenium.dev/selenium/web/web-form.html) com C#, .NET 10, MSTest e Selenium.

Aluno: **Kauan de Andrade Oliveira**.

## Estrutura

- `ConsoleSelenium41.slnx`: solução com os dois projetos solicitados.
- `ConsoleSelenium`: aplicação de console que centraliza a URL e os dados compartilhados em `ConfiguracaoFormulario.cs`.
- `ConsoleSelenium.Tests`: projeto MSTest, com referência ao console e aos pacotes `MSTest`, `Selenium.WebDriver` e `Selenium.Support`.
- `ConsoleSelenium.Tests/WebFormularioDesafioTest.cs`: classe com um `[TestMethod]` para cada exercício.

## Cenários implementados

| Exercício | Ações e verificações |
| --- | --- |
| 1 - Cadastro completo | Preenche nome completo, senha e descrição da prática de testes. Confere os valores preenchidos e o valor exato `Readonly input`. Envia e valida `Received!`. |
| 2 - Seletores e listas | Usa `SelectElement` para selecionar `Two` e verifica o valor `2`. Confirma que `Seattle` existe no datalist, preenche a cidade e valida o envio. |
| 3 - Estados e cor | Confirma o primeiro checkbox marcado por padrão, marca o segundo, seleciona `my-radio-2` e confere a desmarcação do primeiro rádio. Altera a cor para `#1a73e8`, confere o valor e valida o envio. |

Os três testes abrem o formulário real, clicam em Submit e verificam a mensagem `Received!` e o endereço da página de sucesso. Cada cenário usa uma sessão independente, encerrada em `TestCleanup`. As esperas são explícitas, sem pausas fixas.

O color picker é alterado por `IJavaScriptExecutor`, com eventos `input` e `change`, pois seu diálogo nativo fica fora do DOM.

## Como executar

Requisitos: SDK do **.NET 10 ou superior compatível com net10.0**, Google Chrome instalado e acesso à internet para restaurar pacotes, obter o ChromeDriver e acessar o formulário. O Selenium Manager gerencia o driver automaticamente.

```powershell
git clone https://github.com/KdAndrade/console-selenium-41.git
cd console-selenium-41
dotnet restore ConsoleSelenium41.slnx
dotnet test ConsoleSelenium41.slnx
```

Por padrão, o Chrome executa sem janela. Para acompanhar os testes visualmente no PowerShell:

```powershell
$env:SELENIUM_HEADLESS = "false"
dotnet test ConsoleSelenium41.slnx
```

Para gerar um relatório de execução:

```powershell
dotnet test ConsoleSelenium41.slnx --logger "trx;LogFileName=atividade-41.trx" --results-directory TestResults
```

O console também pode ser executado com `dotnet run --project ConsoleSelenium` para exibir a URL e o comando dos testes.

Em ambientes isolados que impedem o Chrome de iniciar, a opção de compatibilidade `SELENIUM_NO_SANDBOX=true` pode ser usada somente para executar esta demonstração. Ela vem desativada por padrão.

Os dados da atividade estão em `ConfiguracaoFormulario.cs`. A senha é forte, gerada em memória e descartável a cada execução, porque o formulário de demonstração usa GET.

## Entrega

Validação realizada em 13/11/2026 com .NET 10 e Chrome sem janela: **3 testes aprovados, 0 falhas e 0 ignorados**, usando o formulário público real. Neste ambiente isolado, foi utilizada a opção de compatibilidade descrita acima.

Repositório público: [KdAndrade/console-selenium-41](https://github.com/KdAndrade/console-selenium-41).
