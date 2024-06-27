

namespace Library.Models
{
    using System;
    
    public partial class sp_helpdiagrams_Result
    {
        public string Database { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
        public string Owner { get; set; }
        public int OwnerID { get; set; }
    }
}
