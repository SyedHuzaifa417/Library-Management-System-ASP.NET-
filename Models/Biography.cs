
namespace Library.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class Biography
    {
        public int BiographyID { get; set; }
        public Nullable<int> AuthorID { get; set; }
        [DisplayName("Information")]
        public string BiographyText { get; set; }
        [DisplayName("Author")]
        public virtual Author Author { get; set; }
    }
}

