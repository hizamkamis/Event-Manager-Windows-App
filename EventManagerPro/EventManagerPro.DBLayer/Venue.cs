//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventManagerPro.DBLayer
{
    public partial class Venue
    {
        public Venue()
        {
            this.SubEvents = new HashSet<SubEvent>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    
        public virtual ICollection<SubEvent> SubEvents { get; set; }
    }
    
}
