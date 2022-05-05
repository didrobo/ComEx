using System;
using System.IO;
using System.Data;
using Excel;

namespace ComEx.Helpers
{
    public static class Excel
    {
        public static DataTable GetData(string strFile, string strSheet)
        {
            FileStream fsFile = null;
            IExcelDataReader excelReader = null;

            try
            {
                if (string.IsNullOrWhiteSpace(strFile))
                    throw new ArgumentNullException("strFile");

                if (string.IsNullOrWhiteSpace(strSheet))
                    throw new ArgumentNullException("strSheet");

                fsFile = new FileStream(strFile, FileMode.Open, FileAccess.Read);
                //Extensión del archivo
                string strExtension = Path.GetExtension(strFile);
                if (strExtension.Equals(".xls"))
                    excelReader = ExcelReaderFactory.CreateBinaryReader(fsFile);
                else
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fsFile);

                excelReader.IsFirstRowAsColumnNames = true;

                DataSet dtSet = excelReader.AsDataSet();

                return dtSet.Tables[strSheet];
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(fsFile != null)
                {
                    fsFile.Dispose();
                }
            }
        }
    }
}