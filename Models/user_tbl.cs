//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MSTourism.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class user_tbl
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string user_type { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<System.DateTime> last_update { get; set; }
    }
}
