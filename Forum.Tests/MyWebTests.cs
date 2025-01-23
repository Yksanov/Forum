using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Forum.Tests;

public class MyWebTests
{
    private readonly IWebDriver _driver;

    public MyWebTests()
    {
        _driver = new ChromeDriver();
    }

    [Fact]
    public async void CheckMainPageTitle()
    {
        await _driver.Navigate().GoToUrlAsync("https://localhost:7231/");
        Assert.Equal("login - Forum", _driver.Title);
        Assert.Contains("Тема", _driver.PageSource);
    }

    [Fact]
    public async void LoginEmptyDataErrorMessage()
    {
        await _driver.Navigate().GoToUrlAsync("https://localhost:7231/Account/Login");
        _driver.FindElement(By.Id("password")).SendKeys("qweqwe");
        _driver.FindElement(By.Id("sumbit")).Click();
        var error = _driver.FindElement(By.ClassName("field-validation-error")).Text;
        Assert.Contains("Почта/Логин не могут быть пустыми", error);
    }
    
    [Fact]
    public async void PasswordEmptyDataErrorMessage()
    {
        await _driver.Navigate().GoToUrlAsync("https://localhost:7231/Account/Login");
        _driver.FindElement(By.Id("login")).SendKeys("qwe@qwe.qwe");
        _driver.FindElement(By.Id("sumbit")).Click();
        var error = _driver.FindElement(By.ClassName("field-validation-error")).Text;
        Assert.Contains("Пароль не может быть пустым", error);
    }
    
    [Fact]
    public async void LoginAndPasswordWrongDataErrorMessage()
    {
        await _driver.Navigate().GoToUrlAsync("https://localhost:7231/Account/Login");
        _driver.FindElement(By.Id("login")).SendKeys("eldos");
        _driver.FindElement(By.Id("password")).SendKeys("qweqwe");
        _driver.FindElement(By.Id("sumbit")).Click();
        var error = _driver.FindElement(By.ClassName("validation-summary-errors")).Text;
        Assert.Contains("Неправильные логин или пароль", error);
    }
    
    
    [Fact]
    public async void LoginAndCheckMainPageMessage()
    {
        await _driver.Navigate().GoToUrlAsync("https://localhost:7231/Account/Login");
        _driver.FindElement(By.Id("login")).SendKeys("eldos");
        _driver.FindElement(By.Id("password")).SendKeys("Eldos@12");
        _driver.FindElement(By.Id("sumbit")).Click();
        Assert.Contains("Создать тему", _driver.PageSource);
    }
    
    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}