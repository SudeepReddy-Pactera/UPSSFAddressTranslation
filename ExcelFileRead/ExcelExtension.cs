using ExcelDataReader;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExcelFileRead
{
    public class CellValidation
    {
        public string fieldName { get; set; }

        public int length { get; set; }

        public string regexValue { get; set; }


    }



    public class ExcelExtensionReponse
    {
        public string ExcelExtensionReponseData { get; set; }

        public string ExcelFailureData { get; set; }

        public bool success { get; set; }

        public Exception exception { get; set; }
    }

    public static class DataExtensions
    {

        public static bool HasNull(this System.Data.DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                if (table.Rows.OfType<DataRow>().Any(r => r.IsNull(column)))
                {
                    return true;
                }

            }
            return false;

        }




    }


    public class ExcelExtension
    {

        


        public ExcelExtensionReponse Test(string fileName, string[] validationSet, string[] regexSet, string[] columnLengths, string[] mandatoryColun)
        {
            ExcelExtensionReponse excelExtensionReponse = new ExcelExtensionReponse();
            List<int> colIndex = mandatoryColun.Select(int.Parse).ToList();
            string JSONString = string.Empty;
            string DataJSONString = string.Empty;
            IExcelDataReader excelReader;
            try
            {
                FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);

                if (fileName.Contains("xlsx"))
                {
                    excelReader = ExcelReaderFactory.CreateReader(stream);
                }
                else
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }



                DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });


                System.Data.DataTable successtable = result.Tables[0].Clone();
                System.Data.DataTable failuretable = result.Tables[0].Clone();
                successtable.Clear();
                failuretable.Clear();
                failuretable.Columns.Add("Exception_Message", typeof(string));


                excelReader.Close();
                List<string> getValidationErrors;
                bool getDesiredColumnExistence = ColExistence(result, validationSet, out getValidationErrors);

                if (getDesiredColumnExistence)
                {
                    
                   

                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        List<bool> errcol = new List<bool>();

                        for (int i=0;i < result.Tables[0].Columns.Count; i++)
                        {
                            var test = colIndex.Contains(i);
                            var regex = new Regex(regexSet[i]);
                            bool temp = true;
                            if (row.ItemArray[i].ToString().Length>0)
                            {
                                if (!regex.IsMatch(row.ItemArray[i].ToString()))
                                {
                                    temp = false;
                                    
                                }
                                else if(row.ItemArray[i].ToString().Length > Int32.Parse(columnLengths[i]))
                                {
                                    temp = false;
                                    
                                }

                            }
                            else if (colIndex.Contains(i))
                            {
                                if (row.ItemArray[i].ToString().Length == 0)
                                {
                                    temp = false;
                                }
                            }

                            
                            errcol.Add(temp);
                        }


                        if (errcol.Contains(false))
                        {
                            failuretable.ImportRow(row);
                            string excepMsg=null;
                            for(int j=0;j < errcol.Count; j++)
                            {
                                if (errcol[j]==false)
                                {
                                    excepMsg = excepMsg==null ? failuretable.Columns[j].ColumnName.ToString():
                                        failuretable.Columns[j].ColumnName.ToString()+"," + excepMsg;
                                }
                            }
                            excepMsg = excepMsg + " has invalid data";

                            //row["Exception_Message"] = excepMsg;
                            //failuretable.Rows[failuretable.Rows.Count-1].ItemArray[33] = excepMsg
                            
                            failuretable.Rows[failuretable.Rows.Count - 1][33] = excepMsg;


                        }
                        else
                        {
                            successtable.ImportRow(row);
                        }

                    }
                    System.Data.DataTable originalData = new System.Data.DataTable();
                    originalData = result.Tables[0];
                    var tableName = result.Tables[0].TableName;
                    result.Tables.Remove(tableName);

                    result.Tables.Add(successtable);

                    var regexItem2 = new Regex("[^0-9a-zA-Z]+");

                    for (int i = 0; i < failuretable.Columns.Count; i++)
                    {

                        if (regexItem2.IsMatch(failuretable.Columns[i].ColumnName.ToString()))
                        {
                            failuretable.Columns[i].ColumnName = "s_" + Regex.Replace(failuretable.Columns[i].ColumnName, @"[^0-9a-zA-Z]+", "");
                        }
                    }

                    












                    for (int i=0;i < result.Tables[0].Columns.Count;i++)
                    {
                      bool regexStatus = regexValdiation(result.Tables[0].DefaultView.ToTable(false, result.Tables[0].Columns[i].ColumnName), regexSet[i], Int32.Parse(columnLengths[i]));

                        if (!regexStatus)
                        {
                            throw new Exception("'"+result.Tables[0].Columns[i].ColumnName + "' column has identified invalid data");
                        }
                    }


                    //foreach (string dataColumn in validationSet)
                    //{
                    //    if (DataExtensions.HasNull(result.Tables[0].DefaultView.ToTable(true, dataColumn)))
                    //    {
                    //        throw new Exception("'"+dataColumn + "' column has identified invalid data");
                    //    }
                    //}

                    var regexItem = new Regex("[^0-9a-zA-Z]+");

                    for (int i = 0; i < result.Tables[0].Columns.Count; i++)
                    {

                        if (regexItem.IsMatch(result.Tables[0].Columns[i].ColumnName.ToString()))
                        {
                            result.Tables[0].Columns[i].ColumnName = "S_" + Regex.Replace(result.Tables[0].Columns[i].ColumnName, @"[^0-9a-zA-Z]+", "");
                        }
                    }

                    result.AcceptChanges();
                    JSONString = JsonConvert.SerializeObject(result.Tables[0]);
                    DataJSONString= JsonConvert.SerializeObject(failuretable);
                    excelExtensionReponse.ExcelExtensionReponseData = JSONString;
                    excelExtensionReponse.ExcelFailureData = DataJSONString;
                    excelExtensionReponse.success = true;
                }
                else
                {
                    excelExtensionReponse.exception = new ArgumentException("Required Columns " + JsonConvert.SerializeObject(getValidationErrors) + " are not found");
                }

            }
            catch (Exception ex)
            {
                excelExtensionReponse.exception = ex;
            }

            return excelExtensionReponse;

        }

        private static bool ColExistence(DataSet result, string[] validationSet, out List<string> validationFailedSet)
        {
            bool desiredResult = true;
            int addressesCount = 0;
            validationFailedSet = new List<string>();

            //for (int i = 0; i < result.Tables[0].Columns.Count; i++)
            //{
            //    if(string.Equals(result.Tables[0].Columns[i].ToString(), "address", StringComparison.OrdinalIgnoreCase))
            //    {
            //        addressesCount++;
            //    }
            //}

            //if (addressesCount == 2)
            //{
            desiredResult = DefaultValidation(result, validationSet, validationFailedSet, desiredResult);
            //}
            //else
            //{
            //    desiredResult = DefaultValidation(result, validationSet, validationFailedSet, desiredResult);
            //    validationFailedSet.Add("address");
            //    desiredResult = false;
            //}

            return desiredResult;
        }

        private static bool DefaultValidation(DataSet result, string[] validationSet, List<string> validationFailedSet, bool desiredResult)
        {
            foreach (string validationColumn in validationSet)
            {
                if (!result.Tables[0].Columns.Contains(validationColumn))
                {
                    if (string.Equals(validationColumn, "address_1", StringComparison.OrdinalIgnoreCase))
                    {
                        validationFailedSet.Add("Delivery Address Field is missing ! Which was placed beside - receiver company in your upload excel file");
                    }
                    else
                    {
                        validationFailedSet.Add(validationColumn);
                    }
                    desiredResult = false;
                }
            }

            return desiredResult;
        }


        public bool regexValdiation(System.Data.DataTable table, string regexItem,int columnLength)
        {

            var regex = new Regex(regexItem);
            foreach (DataColumn column in table.Columns)
            {
               foreach(DataRow row in table.Rows)
                {
                    if (row.ItemArray[0].ToString().Length>0 && row.ItemArray[0].ToString().Length < columnLength)
                    {
                        if (!regex.IsMatch(row.ItemArray[0].ToString()))
                        {
                            return false;
                        }

                    }
                    else if(row.ItemArray[0].ToString().Length > columnLength)
                    {
                        return false;
                    }
                    
                }

            }
            return true;

        }
    }
}
