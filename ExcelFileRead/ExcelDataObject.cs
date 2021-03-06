﻿namespace ExcelFileRead
{

    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class ExcelDataObject
    {
        public string S_shipmentno { get; set; }
        public string pcs { get; set; }
        public string S_packageno { get; set; }
        public string S_pkgwei { get; set; }
        public string S_shptwei { get; set; }
        public string S_dimwei { get; set; }
        public string S_weiunit { get; set; }
        public string svl { get; set; }
        public string pymt { get; set; }
        public string S_shipdate { get; set; }
        public string S_pkuptime { get; set; }
        public string S_billtype { get; set; }
        public string value { get; set; }
        public string currency { get; set; }
        public string S_1stinvoicelinedesc { get; set; }
        public string S_expslic { get; set; }
        public string S_shpr { get; set; }
        public string S_shippercompany { get; set; }
        public string address { get; set; }
        public string S_orgcity { get; set; }
        public string S_orgpsl { get; set; }
        public string S_shptctc { get; set; }
        public string S_shptph { get; set; }
        public string S_impslic { get; set; }
        public string S_impr { get; set; }
        public string S_receivercompany { get; set; }
        public string S_address1 { get; set; }
        public string S_dstcity { get; set; }
        public string S_dstpsl { get; set; }
        public string S_cneectc { get; set; }
        public string S_ph { get; set; }
        public string S_inflight { get; set; }
        //public object S_outflight { get; set; }
    }
}



