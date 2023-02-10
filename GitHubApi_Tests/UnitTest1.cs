namespace GitHubApi_Tests;

public class Tests
{
    [SetUp]
    private RestClient = client;
    private const string baseUrl = "https://api.github.com";
    private const string partialUrl = "/repos/petyazhelyazkova29/Collections/issue";

    public void Setup()
    {
        this.client = new RestClient(baseUrl);
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}