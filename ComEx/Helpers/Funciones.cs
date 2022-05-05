using ComEx.Context;
using ComEx.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Xml.Serialization;
using System.Xml;

namespace ComEx.Helpers
{
    public class Funciones
    {
        static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public static List<SelectListItem> GetListOfSelectListItem<T>(List<T> tbl, string fieldText, string fieldId) 
        {
            PropertyDescriptor propFieldText = TypeDescriptor.GetProperties(typeof(T)).Find(fieldText, false);
            PropertyDescriptor propFieldId = TypeDescriptor.GetProperties(typeof(T)).Find(fieldId, false);

            List<SelectListItem> listItems = tbl.Select(p => (new SelectListItem
            {
                Text = propFieldText.GetValue(p).ToString(),
                Value = propFieldId.GetValue(p).ToString(),                
            })).ToList();

            listItems.Insert(0, new SelectListItem { Selected = true, Value = "", Text = "Seleccionar..." });

            return listItems;
        }

        public static tcLogOperaciones GetLogOperacion(string proceso, string accion, string tipo, string usuario) {

            tcLogOperaciones mytcLogOperaciones = new tcLogOperaciones() { 
                fecFcha = DateTime.Now,
                varPrcso = proceso,
                varAccion = accion,
                varTpo = tipo,
                varUsuario = usuario
            };

            return mytcLogOperaciones;
        }

        public static IQueryable<T> FiltrarListDataTables<T>(IQueryable<T> listTbl, List<DTFilters> columnFilters) 
        {
            
            string where = "";
            List<object> listValues = new List<object>();

            List<IGrouping<string, DTFilters>> query = columnFilters.GroupBy(g => g.ColumnField)
                .Where(w => w.Count() > 1)
                .Select(s => s)
                .ToList();

            foreach (IGrouping<string, DTFilters> group in query)
            {
                group.First().OpenParen = "( ";
                group.Last().CloseParen = " )";
            }

            foreach (DTFilters filtro in columnFilters)
            {
                filtro.OpenParen = filtro.OpenParen ?? "";
                filtro.CloseParen = filtro.CloseParen ?? "";

                switch (filtro.LogicOperator) 
                {
                    case "like":

                        string campo = IsNullable(typeof(T).GetProperty(filtro.ColumnField).PropertyType) ? filtro.ColumnField + ".Value" : filtro.ColumnField;
                        string conditionValue = IsNullable(typeof(T).GetProperty(filtro.ColumnField).PropertyType) ? filtro.ColumnField + ".HasValue AND " : filtro.ColumnField + " != null AND ";

                        where += string.Format(" {0} {3}{5}{1}.ToString().ToLower().Contains(@{2}){4} ", filtro.Operator ?? "", campo, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen, conditionValue);
                        
                        listValues.Add(filtro.Value.ToLower());

                        break;
                    
                    case "=":
                    case "!=":
                        switch (filtro.ColumnField.Substring(0, 3))
                        { 
                            case "var":
                                where += string.Format(" {0} {4}{1}.ToString().ToLower() {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(filtro.Value.ToLower());                                
                                break;
                            case "int":
                                where += string.Format(" {0} {4}{1} {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(Convert.ToInt32(filtro.Value));                                
                                break;
                            case "num":
                                where += string.Format(" {0} {4}{1} {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(Convert.ToDecimal(filtro.Value));                                
                                break;
                            case "fec":
                                where += string.Format(" {0} {4}{1} {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(Convert.ToDateTime(filtro.Value));                                
                                break;
                            case "bit":
                                where += string.Format(" {0} {4}{1} {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(Convert.ToBoolean(filtro.Value));                                
                                break;
                        }                        
                        break;
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                        switch (filtro.ColumnField.Substring(0, 3))
                        {
                            case "int":
                                where += string.Format(" {0} {4}{1} {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(Convert.ToInt32(filtro.Value));
                                break;
                            case "num":
                                where += string.Format(" {0} {4}{1} {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(Convert.ToDecimal(filtro.Value));
                                break;
                            case "fec":
                                where += string.Format(" {0} {4}{1} {2} @{3}{5} ", filtro.Operator ?? "", filtro.ColumnField, filtro.LogicOperator, columnFilters.IndexOf(filtro), filtro.OpenParen, filtro.CloseParen);
                                listValues.Add(Convert.ToDateTime(filtro.Value));
                                break;
                        }
                        break;
                    case "IsEmpty":
                        
                        switch (filtro.ColumnField.Substring(0, 3))
                        {
                            case "var":
                                where += string.Format(" {0} ( {1} == \"\" OR {1} == null ) ", filtro.Operator ?? "", filtro.ColumnField);
                                listValues.Add("");
                                break;
                            case "int":
                            case "num":
                            case "fec":
                            case "bit":
                                where += string.Format(" {0} {1} == null ", filtro.Operator ?? "", filtro.ColumnField);
                                listValues.Add(null);
                                break;
                        }

                        break;
                    case "IsNotEmpty":

                        switch (filtro.ColumnField.Substring(0, 3))
                        {
                            case "var":
                                where += string.Format(" {0} ( {1} != \"\" AND {1} != null ) ", filtro.Operator ?? "", filtro.ColumnField);
                                listValues.Add("");
                                break;
                            case "int":
                            case "num":
                            case "fec":
                            case "bit":
                                where += string.Format(" {0} {1} != null ", filtro.Operator ?? "", filtro.ColumnField);
                                listValues.Add(null);
                                break;
                        }
                        
                        break;
                }
            }

            if (where != "")
            {
                listTbl = listTbl.Where(where, listValues.ToArray());
            }

            try {
                listTbl.Any();
            }
            catch (Exception ex) {
                listTbl = new List<T>().AsQueryable();
            }

            return listTbl;
        }

        public static bool MergePDFs(IEnumerable<string> fileNames, string targetPdf, bool printJustFirstPage)
        {
            bool merged = true;
            using (FileStream stream = new FileStream(targetPdf, FileMode.Create))
            {
                Document document = new Document();
                PdfCopy pdf = new PdfCopy(document, stream);
                PdfReader reader = null;
                try
                {
                    document.Open();
                    foreach (string file in fileNames)
                    {
                        reader = new PdfReader(file);
                        if (printJustFirstPage)
                        {
                            reader.SelectPages("1");
                        }
                        pdf.AddDocument(reader);
                        reader.Close();
                    }
                }
                catch (Exception)
                {
                    merged = false;
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    if (document != null)
                    {
                        document.Close();
                    }
                }
            }
            return merged;
        }

        public static string SerializarObjetoToXml(object myObject, Type tipo)
        {
            string xmlObj = "";

            XmlSerializerNamespaces emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer xsSubmit = new XmlSerializer(tipo);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            

            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    xsSubmit.Serialize(writer, myObject, emptyNamepsaces);
                    xml = sww.ToString(); // Your XML
                    xmlObj = xml;
                }
            }

            return xmlObj;
        }

        public static object DeSerializarObjetoXml(string myXml, Type tipo)
        {
            object myObject = null;

            XmlSerializer serializer = new XmlSerializer(tipo);

            using (var rdr = new StringReader(myXml))
            {
                myObject = serializer.Deserialize(rdr);
            }

            return myObject;
        }
    }
}