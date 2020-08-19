using System;
using System.Collections.Generic;
using System.Text;

namespace UPS.DataObjects.Reports
{
    public class SummaryData
    {
        public int WFL_ID { get; set; }
        public string FLE_NA { get; set; }
        public string USR_FST_NA { get; set; }
        public string CTY_TXT { get; set; }
        public DateTime? CRD_DT { get; set; }
        public decimal TOT_UP_ADR { get; set; }
        public decimal TOT_TR_ADR { get; set; }
        public decimal TOT_TR_ADR_PER { get; set; }
        public int TR_FRM_ADR_BK { get; set; }
        public decimal TR_FRM_ADR_BK_PER { get; set; }
        public decimal TR_FRM_TR_TOOL { get; set; }
        public decimal TR_FRM_TR_TOOL_PER { get; set; }
        public decimal ADR_MOD { get; set; }
        public decimal ADR_MOD_PER { get; set; }


    }
}
