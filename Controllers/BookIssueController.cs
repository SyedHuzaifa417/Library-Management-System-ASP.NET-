using Library.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Library.Models.BookIssue;
using System.Diagnostics;

namespace Library.Controllers
{
    public class BookIssueController : ApiController
    {

        private string directConnectionString = ConfigurationManager.ConnectionStrings["DirectSqlConnection"].ConnectionString;

        // GET: api/BookIssue
        [HttpGet]
        [Route("api/BookIssue/Get")]
        public IHttpActionResult Get()
        {
            try
            {
                var response = new
                {
                    Books = GetBooks(),
                    Members = GetMembers(),
                    IssuedBooks = GetAllBookIssues(),
                    Model = new BookIssue
                    {
                        IssueDate = DateTime.Now,
                        ExpectedReturnDate = DateTime.Now.AddDays(14),
                        BookIssueDetails = new List<BookIssueDetail> { new BookIssueDetail() }
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/BookIssue
        [HttpPost]
        [Route("api/BookIssue/Post")]
        public IHttpActionResult Post(BookIssue model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (IsIssueCodeExists(model.IssueCode))
            {
                return Conflict();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(directConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand("IssueBooks", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@IssueCode", model.IssueCode);
                            cmd.Parameters.AddWithValue("@IssueDate", model.IssueDate);
                            cmd.Parameters.AddWithValue("@ExpectedReturnDate", model.ExpectedReturnDate);
                            cmd.Parameters.AddWithValue("@MemberId", model.MemberId);

                            DataTable tvp = new DataTable();
                            tvp.Columns.Add(new DataColumn("BookTitle", typeof(string)));
                            tvp.Columns.Add(new DataColumn("Quantity", typeof(int)));

                            foreach (var detail in model.BookIssueDetails)
                            {
                                DataRow row = tvp.NewRow();
                                row["BookTitle"] = detail.BookTitle;
                                row["Quantity"] = detail.Quantity;
                                tvp.Rows.Add(row);
                            }

                            SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Books", tvp);
                            tvpParam.SqlDbType = SqlDbType.Structured;
                            tvpParam.TypeName = "dbo.BookIssueType";

                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                }

                return Ok("Books issued successfully!");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/BookIssue
        [HttpDelete]
        [Route("api/BookIssue/Delete")]
        public IHttpActionResult Delete()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(directConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM BookIssues;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return Ok("All book issues deleted successfully!");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/BookIssue/GetMaxIssueId")]
        public IHttpActionResult GetMaxIssueId()
        {
            int maxIssueId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(directConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT MAX(IssueId) FROM BookIssues", connection);
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        maxIssueId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Returning a proper response with error message
            }

            return Ok(maxIssueId); // Returning a proper response with maxIssueId
        }

        private bool IsIssueCodeExists(string issueCode)
        {
            using (SqlConnection conn = new SqlConnection(directConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM BookIssues WHERE IssueCode = @IssueCode", conn))
                {
                    cmd.Parameters.AddWithValue("@IssueCode", issueCode);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private List<Book> GetBooks()
        {
            var books = new List<Book>();

            using (SqlConnection conn = new SqlConnection(directConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT BookId, Title, AvailableCopies FROM Books WHERE AvailableCopies > 0", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book
                            {
                                BookId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                AvailableCopies = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }

            return books;
        }

        private List<Member> GetMembers()
        {
            var members = new List<Member>();

            using (SqlConnection conn = new SqlConnection(directConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT MemberId, MemberName FROM Members", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            members.Add(new Member
                            {
                                MemberId = reader.GetInt32(0),
                                MemberName = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return members;
        }

        private List<BookIssue> GetAllBookIssues()
        {
            var bookIssues = new List<BookIssue>();

            using (SqlConnection conn = new SqlConnection(directConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT bi.IssueId, bi.IssueCode, bi.IssueDate, bi.ExpectedReturnDate, bi.MemberId, m.MemberName " +
                                                       "FROM BookIssues bi " +
                                                       "JOIN Members m ON bi.MemberId = m.MemberId", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bookIssue = new BookIssue
                            {
                                IssueId = reader.GetInt32(0),
                                IssueCode = reader.GetString(1),
                                IssueDate = reader.GetDateTime(2),
                                ExpectedReturnDate = reader.GetDateTime(3),
                                MemberId = reader.GetInt32(4),
                                Member = new Member
                                {
                                    MemberId = reader.GetInt32(4),
                                    MemberName = reader.GetString(5)
                                },
                                BookIssueDetails = GetBookIssueDetails(reader.GetInt32(0))
                            };

                            bookIssues.Add(bookIssue);
                        }
                    }
                }
            }

            return bookIssues;
        }

        private List<BookIssueDetail> GetBookIssueDetails(int issueId)
        {
            var issueDetails = new List<BookIssueDetail>();

            using (SqlConnection conn = new SqlConnection(directConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT b.Title, bi.Quantity " +
                                                       "FROM BookIssues bi " +
                                                       "Inner JOIN Books b ON bi.BookId = b.BookId " +
                                                       "WHERE bi.IssueId = @IssueId", conn))
                {
                    cmd.Parameters.AddWithValue("@IssueId", issueId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            issueDetails.Add(new BookIssueDetail
                            {
                                BookTitle = reader.GetString(0),
                                Quantity = reader.GetInt32(1)
                            });
                        }
                    }
                }
            }

            return issueDetails;
        }
    }
}
