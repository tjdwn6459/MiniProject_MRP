//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 템플릿에서 생성되었습니다.
//
//     이 파일을 수동으로 변경하면 응용 프로그램에서 예기치 않은 동작이 발생할 수 있습니다.
//     이 파일을 수동으로 변경하면 코드가 다시 생성될 때 변경 내용을 덮어씁니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MRPApp.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Schedules
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Schedules()
        {
            this.Process = new HashSet<Process>();
        }
    
        public int SchIdx { get; set; }
        public string PlantCode { get; set; }
        public System.DateTime SchDate { get; set; }
        public int SchLoadTime { get; set; }
        public Nullable<System.TimeSpan> SchStartTime { get; set; }
        public Nullable<System.TimeSpan> SchEnd { get; set; }
        public string SchFacilityID { get; set; }
        public Nullable<int> SchAmount { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public string RegID { get; set; }
        public Nullable<System.DateTime> ModDate { get; set; }
        public string ModID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Process> Process { get; set; }
        public virtual Settings Settings { get; set; }
        public virtual Settings Settings1 { get; set; }
    }
}