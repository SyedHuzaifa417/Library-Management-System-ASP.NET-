
namespace Library.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Author
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Author()
        {
            this.Books = new HashSet<Book>();
            this.Biographies = new HashSet<Biography>();
        }

        public int AuthorId { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage="Author Name is Required")]
        public string AuthorName { get; set; }
        [DisplayName("Birth Date")]
        [Required(ErrorMessage = "Author Birth Date is Required")]
        public Nullable<System.DateTime> BirthDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Book> Books { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Biography> Biographies { get; set; }
    }
}
