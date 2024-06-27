
namespace Library.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Member()
        {
            this.BookIssues = new HashSet<BookIssue>();
            this.Loans = new HashSet<Loan>();
        }

        public int MemberId { get; set; }
        [DisplayName(" Member ")]
        [Required(ErrorMessage = "Member Name is Required")]
        public string MemberName { get; set; }
        [DisplayName("Joined On ")]
        [Required(ErrorMessage = "Joining date is Required")]
        public Nullable<System.DateTime> Joined_Membership { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookIssue> BookIssues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Loan> Loans { get; set; }
    }
}
