using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubTests
{
    public class Tests
    {
        private RestClient client;
        private const string baseUrl = "https://api.github.com";
        private const string partialUrl = "/repos/petyazhelyazkova29/Collections/issues";
        private const string username = "petyazhelyazkova29";
        private const string password = "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";
        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
            this.client.Authenticator = new HttpBasicAuthenticator(username, password);
        }

        [Test]
        public void Test_GetSingleIssue()
        {
            var request = new RestRequest($"{partialUrl}/2", Method.Get);
            var response = client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code check");
            Assert.That(issue.title, Is.EqualTo("My Second Issue"));
            Assert.That(issue.number, Is.EqualTo(2));
            Assert.That(issue.id, Is.EqualTo(1564948133), "Issue ID");
        }

        [Test]
        public void Test_GetAllIssues()
        {
            var request = new RestRequest(partialUrl, Method.Get);
            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code check");
            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content!);
            Assert.That(issues.Count > 1);
            foreach (var issue in issues)
            {
                Assert.That(issue.title, Is.Not.Empty);
                Assert.That(issue.number, Is.GreaterThan(0));
                Assert.That(issue.id, Is.GreaterThan(0), "Issue ID");
            }
        }
        [Test]
        public void Test_GetCommentsFromIssue()
        {
            var request = new RestRequest($"{partialUrl}/2/comments", Method.Get);
            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var comments = JsonSerializer.Deserialize<List<Comments>>(response.Content!);
            foreach (var comment in comments)
            {
                Assert.That(comment.id, Is.GreaterThan(0));
                Assert.That(comment.body, Is.Not.Empty);
            }
        }
        [Test]
        public void Test_InvalidIssue()
        {

            var request = new RestRequest($"{partialUrl}/2345", Method.Get);
            var response = client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Status code check");
        }
        [Test]
        public void Test_GetSpecifiedComments()
        {
            var request = new RestRequest($"{partialUrl}/comments/1411029616", Method.Get);
            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var comment = JsonSerializer.Deserialize<Comment>(response.Content!);
            Assert.That(comment.id, Is.EqualTo(1411029616));
            Assert.That(comment.body, Is.Not.Empty);
            Assert.That(comment.created_at, Is.Not.Null);
            Assert.That(comment.updated_at, Is.Not.Null);
        }
        [Test]
        public void Test_GetLabelsFromIssue()
        {
            var request = new RestRequest($"{partialUrl}/11/labels", Method.Get);
            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            //var labels = JsonSerializer.Deserialize<List<Labels>>(response.Content!);

            //foreach (var label in labels)
            //{
            //    Assert.That(label.id, Is.GreaterThan(0));
            //    Assert.That(label.name, Is.Not.Empty);
            //}
        }
        [Test]
        public void Test_CreateNewIssue()
        {
            var request = new RestRequest(partialUrl, Method.Post);
            var issueBody = new
            {
                title = "Test issue from RestSharp" + DateTime.Now.Ticks,
                body = "Some body"
            };

            request.AddBody(issueBody);
            var response = client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(issue.title, Is.Not.Empty);
            Assert.That(issue.body, Is.EqualTo("Some body"));
        }

        [Test]
        public void Test_CreateIssueWithNoAuth()
        {
            var request = new RestRequest(partialUrl, Method.Post);
            var issueBody = new
            {
                title = "Test from RestSharp",
                body = "Some body",
                labels = new string[] { "bug", "critical" }
            };
            request.AddBody(issueBody);
            var response = client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void Test_CreateIssueWithoutTitle()
        {
            var request = new RestRequest(partialUrl, Method.Post);
            var issueBody = new
            { body = "Some Body" };
            request.AddBody(issueBody);
            var response = client.Execute(request);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
        }

        [Test]
        public void Test_CreateComment()
        {
            var request = new RestRequest($"{partialUrl}/34", Method.Post);
            var commentBody = new {body = "New comment from RestSharp" };
            request.AddBody(commentBody);
            var response = client.Execute(request);
            var comment = JsonSerializer.Deserialize<Comment>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(comment.body, Is.Not.Empty);
        }

        [Test]
        public void Test_DeleteIssueComment()
        { 
            var request = new RestRequest($"{partialUrl}/comments/1579598804", Method.Delete);
            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void Test_EditComment() 
        {
            var request = new RestRequest($"{partialUrl}/54", Method.Patch);
            var commentBody = new
            {
                body = "RestSharp comment change"
            };
            request.AddJsonBody(commentBody);
            var response = client.Execute(request);
            var comment = JsonSerializer.Deserialize<Comment>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(comment.id, Is.GreaterThan(0));
            Assert.That(comment.body, Is.EquivalentTo(commentBody.body), "Body");
        }
    }
}