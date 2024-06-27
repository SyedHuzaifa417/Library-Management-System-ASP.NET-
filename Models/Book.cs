
namespace Library.Models
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Book()
        {
            this.Loans = new HashSet<Loan>();
            this.BookIssues = new HashSet<BookIssue>();
        }

        public int BookId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [DisplayName("Author")]
        [Required(ErrorMessage = "Author is required")]
        public Nullable<int> AuthorId { get; set; }
        [DisplayName("Publication Year ")]
        [Required ,Range(1000, 3000, ErrorMessage = "Invalid publication year")]
        public string PublicationYear { get; set; }
        [DisplayName("Copies")]
        [Required ,Range(1, int.MaxValue, ErrorMessage = "Available copies must be greater than zero")]
        public Nullable<int> AvailableCopies { get; set; }

        public virtual Author Author { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Loan> Loans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookIssue> BookIssues { get; set; }
    }
}
