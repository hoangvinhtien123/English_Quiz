//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace English_Quiz.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Question_Type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Question_Type()
        {
            this.Questions_Auto_Generate = new HashSet<Questions_Auto_Generate>();
            this.Questions = new HashSet<Question>();
        }
    
        public int TYPE_ID { get; set; }
        public string TYPE_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public bool IS_TEST { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Questions_Auto_Generate> Questions_Auto_Generate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Question> Questions { get; set; }
    }
}
