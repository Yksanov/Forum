using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Forum.Tests;

public class MyWebTests : IClassFixture<ChromeDriver>
{
    private readonly IWebDriver _driver;

    public MyWebTests()
    {
        _driver = new ChromeDriver();
    }

    [Fact]
    public async void CheckMainPageTitle()
    {
        await _driver.Navigate().GoToUrlAsync("http://localhost:5199");
        Assert.Equal("Thema", _driver.Title);
        Assert.Contains("Тема", _driver.PageSource);
    }
}