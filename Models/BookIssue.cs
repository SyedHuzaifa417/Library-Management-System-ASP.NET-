
namespace Library.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class BookIssue
    {
        public int IssueId { get; set; }
        [DisplayName("Issue Code")]
        public string IssueCode { get; set; }
        [DisplayName("Issue Date")]
        public System.DateTime IssueDate { get; set; }
        [DisplayName("Return Date")]
        public System.DateTime ExpectedReturnDate { get; set; }
        [DisplayName("Member")]
        public Nullable<int> MemberId { get; set; }
        public Nullable<int> BookId { get; set; }
        
        public Nullable<int> Quantity { get; set; }

        public virtual Member Member { get; set; }
        public virtual Book Book { get; set; }

        public List<BookIssueDetail> BookIssueDetails { get; set; }

        public class BookIssueDetail
        {
            public int BookId { get; set; }
            public string BookTitle { get; set; }
            public Nullable<int> Quantity { get; set; }
        }
    }

}
