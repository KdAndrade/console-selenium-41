using System.Security.Cryptography;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests;

[TestClass]
[DoNotParallelize]
public sealed class WebFormularioDesafioTest
{
    private IWebDriver? driver;
    private WebDriverWait espera = null!;

    private IWebDriver Navegador => driver
        ?? throw new InvalidOperationException("O navegador ainda não foi iniciado.");

    [TestInitialize]
    public void AbrirFormulario()
    {
        var opcoes = new ChromeOptions();
        if (!string.Equals(Environment.GetEnvironmentVariable("SELENIUM_HEADLESS"), "false",
                StringComparison.OrdinalIgnoreCase))
        {
            opcoes.AddArgument("--headless=new");
        }

        opcoes.AddArgument("--window-size=1366,900");
        if (Environment.GetEnvironmentVariable("SELENIUM_NO_SANDBOX") == "true")
        {
            opcoes.AddArgument("--no-sandbox");
        }

        driver = new ChromeDriver(opcoes);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
        espera = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        driver.Navigate().GoToUrl(ConfiguracaoFormulario.Url);
        espera.Until(navegador => navegador.FindElement(By.Name("my-text")).Displayed);
    }

    [TestCleanup]
    public void FecharNavegador()
    {
        try
        {
            driver?.Quit();
        }
        finally
        {
            driver?.Dispose();
            driver = null;
        }
    }

    [TestMethod]
    public void CadastroCompleto_DevePreencherCamposEExibirReceived()
    {
        Navegador.FindElement(By.Name("my-text")).SendKeys(ConfiguracaoFormulario.NomeCompleto);
        // Senha descartável: o formulário de demonstração envia os campos pela URL (GET).
        var senha = $"Aa1!{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}";
        Navegador.FindElement(By.Name("my-password")).SendKeys(senha);
        Navegador.FindElement(By.Name("my-textarea")).SendKeys(ConfiguracaoFormulario.Experiencia);

        Assert.AreEqual(ConfiguracaoFormulario.NomeCompleto,
            Navegador.FindElement(By.Name("my-text")).GetDomProperty("value"));
        Assert.AreEqual(senha, Navegador.FindElement(By.Name("my-password")).GetDomProperty("value"));
        Assert.AreEqual(ConfiguracaoFormulario.Experiencia,
            Navegador.FindElement(By.Name("my-textarea")).GetDomProperty("value"));
        Assert.AreEqual("Readonly input",
            Navegador.FindElement(By.Name("my-readonly")).GetDomProperty("value"));

        EnviarFormularioEValidarSucesso();
    }

    [TestMethod]
    public void SeletoresEListas_DeveSelecionarTwoEPreencherSeattle()
    {
        var seletor = new SelectElement(Navegador.FindElement(By.Name("my-select")));
        seletor.SelectByText("Two");
        Assert.AreEqual("2", seletor.SelectedOption.GetDomAttribute("value"));

        var cidadeDisponivel = Navegador.FindElements(By.CssSelector("#my-options option"))
            .Any(opcao => opcao.GetDomAttribute("value") == ConfiguracaoFormulario.Cidade);
        Assert.IsTrue(cidadeDisponivel, "Seattle deve estar entre as cidades do datalist.");

        var datalist = Navegador.FindElement(By.Name("my-datalist"));
        datalist.SendKeys(ConfiguracaoFormulario.Cidade);
        Assert.AreEqual(ConfiguracaoFormulario.Cidade, datalist.GetDomProperty("value"));

        EnviarFormularioEValidarSucesso();
    }

    [TestMethod]
    public void EstadosECor_DeveMarcarCheckboxAlternarRadioEAlterarCor()
    {
        Assert.IsTrue(Navegador.FindElement(By.Id("my-check-1")).Selected,
            "O primeiro checkbox deve estar marcado por padrão.");

        var segundoCheckbox = Navegador.FindElement(By.Id("my-check-2"));
        Assert.IsFalse(segundoCheckbox.Selected);
        segundoCheckbox.Click();
        Assert.IsTrue(segundoCheckbox.Selected);

        var segundoRadio = Navegador.FindElement(By.Id("my-radio-2"));
        segundoRadio.Click();
        Assert.IsTrue(segundoRadio.Selected);
        Assert.IsFalse(Navegador.FindElement(By.Id("my-radio-1")).Selected);

        var seletorCor = Navegador.FindElement(By.Name("my-colors"));
        // O diálogo nativo de cores fica fora do DOM; alteramos o input e emitimos seus eventos.
        ((IJavaScriptExecutor)Navegador).ExecuteScript("""
            arguments[0].value = arguments[1];
            arguments[0].dispatchEvent(new Event('input', { bubbles: true }));
            arguments[0].dispatchEvent(new Event('change', { bubbles: true }));
            """, seletorCor, ConfiguracaoFormulario.Cor);
        Assert.AreEqual(ConfiguracaoFormulario.Cor, seletorCor.GetDomProperty("value"));

        EnviarFormularioEValidarSucesso();
    }

    private void EnviarFormularioEValidarSucesso()
    {
        Navegador.FindElement(By.CssSelector("button[type='submit']")).Click();
        var mensagem = espera.Until(navegador =>
        {
            var elemento = navegador.FindElement(By.Id("message"));
            return elemento.Displayed ? elemento : null;
        });
        Assert.AreEqual("Received!", mensagem.Text);
        Assert.AreEqual("/selenium/web/submitted-form.html", new Uri(Navegador.Url).AbsolutePath);
    }
}
