using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UPS.DataObjects.Shipment
{
    [Table("SF-CTY")]
    public class SFCity
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [Column("CTY-NME")]
        [StringLength(500)]
        public string CTY_NME { get; set; }

    }
}
