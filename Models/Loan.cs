
namespace Library.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class Loan
    {
        public int LoanId { get; set; }
        public Nullable<int> BookId { get; set; }
        public Nullable<int> MemberId { get; set; }
        [DisplayName("Loan Date")]
        public Nullable<System.DateTime> LoanDate { get; set; }
        [DisplayName("Return Date")]
        public Nullable<System.DateTime> returnDate { get; set; }

        public virtual Book Book { get; set; }
        public virtual Member Member { get; set; }
    }
}
