using Library.Models;
using static Library.Models.BookIssue;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Diagnostics;

public class BookIssuesController : Controller
{
    private readonly string apiBaseUrl = "https://localhost:44356/";

    [Authorize]
    public async Task<ActionResult> Index()
    {
        var model = await PrepareIndexModel();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Index(BookIssue model)
    {
        if (ModelState.IsValid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    Debug.WriteLine("Posting new book issue...");
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/BookIssue/Post", model);
                    Debug.WriteLine($"Post response: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = await response.Content.ReadAsAsync<string>();

                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        ViewBag.Error = "The IssueCode already exists. Please enter a different code.";
                    }
                    else
                    {
                        ViewBag.Error = "Failed to issue books. Please try again later.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Exception: " + ex.Message;
                }
            }
        }

        var updatedModel = await PrepareIndexModel();
        return View(updatedModel);
    }

    [HttpPost]
    public async Task<ActionResult> DeleteAll()
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                Debug.WriteLine("Deleting all book issues...");
                HttpResponseMessage response = await client.DeleteAsync("api/BookIssue/Delete");
                Debug.WriteLine($"Delete response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = await response.Content.ReadAsAsync<string>();
                    Debug.WriteLine($"Delete message is :{response.StatusCode} ");
                }
                else
                {
                    ViewBag.Error = "Failed to delete all book issues. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception: " + ex.Message;
            }
        }

        return RedirectToAction("Index");
    }

    private async Task<BookIssue> PrepareIndexModel()
    {
        List<Book> books = new List<Book>();
        List<Member> members = new List<Member>();
        List<BookIssue> bookIssues = new List<BookIssue>();
        string suggestedIssueCode = "";

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                Debug.WriteLine("Fetching book issue data...");
                HttpResponseMessage response = await client.GetAsync("api/BookIssue/Get");
                Debug.WriteLine($"Get response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<dynamic>();
                    books = data.Books.ToObject<List<Book>>();
                    members = data.Members.ToObject<List<Member>>();
                    bookIssues = data.IssuedBooks.ToObject<List<BookIssue>>();

                    Debug.WriteLine("Fetching max issue ID...");
                    HttpResponseMessage maxIdResponse = await client.GetAsync("api/BookIssue/GetMaxIssueId");
                    Debug.WriteLine($"GetMaxIssueId response: {maxIdResponse.StatusCode}");

                    if (maxIdResponse.IsSuccessStatusCode)
                    {
                        int maxIssueId = await maxIdResponse.Content.ReadAsAsync<int>();
                        suggestedIssueCode = GenerateIssueCode(maxIssueId);
                        Debug.WriteLine($"Max issue ID: {maxIssueId}, Suggested issue code: {suggestedIssueCode}");
                    }
                    else
                    {
                        ViewBag.Error = $"Failed to fetch max IssueId. {maxIdResponse.ReasonPhrase}";
                    }
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Error fetching data from API: {response.ReasonPhrase}, Content: {responseContent}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Exception: " + ex.Message;
            }
        }

        ViewBag.Books = books;
        ViewBag.Members = members;
        ViewBag.IssuedBooks = bookIssues;
        ViewBag.IssueCode = suggestedIssueCode;

        return new BookIssue
        {
            IssueCode = suggestedIssueCode,
            IssueDate = DateTime.Now,
            ExpectedReturnDate = DateTime.Now.AddDays(14),
            BookIssueDetails = new List<BookIssueDetail> { new BookIssueDetail() }
        };
    }

    private string GenerateIssueCode(int maxIssueId)
    {
        // Increment maxIssueId to generate the new IssueCode
        int newIssueId = maxIssueId + 1;
        return $"ISSUE-{newIssueId:D5}"; // Example format: ISSUE-00001
    }
}
