using ComEx.Context;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;

namespace ComEx.Helpers
{
    public class SqlServer
    {

        private SqlConnection sqlConnection { get; set; }

        public SqlServer()
        {
            db_comexEntities db = new db_comexEntities();
            sqlConnection = new SqlConnection(db.Database.Connection.ConnectionString);
        }

        public SqlServer(string ConnectionString)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new ArgumentNullException("ConnectionString");

            this.sqlConnection = new SqlConnection(ConnectionString);
        }

        public SqlServer(db_comexEntities db)
        {
            if (db == null)
                throw new ArgumentNullException("ConnectionString");

            sqlConnection = new SqlConnection(db.Database.Connection.ConnectionString);
        }

        public bool ConnectionOpen
        {
            get
            {
                return sqlConnection.State != ConnectionState.Open ? false : true;
            }
        }

        public bool ConnectionClosed
        {
            get
            {
                return sqlConnection.State == ConnectionState.Open ? false : true;
            }
        }

        public void Open()
        {
            try
            {
                this.sqlConnection.Open();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                this.sqlConnection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BulkCopy(string strTable, DataTable dtData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strTable))
                    throw new ArgumentNullException("strTable");

                if (dtData == null || dtData.Rows.Count <= 0)
                    throw new ArgumentNullException("dtData");

                //Realizar el bulkCopy a la tabla
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(this.sqlConnection))
                {
                    //Abrir conexión
                    if (this.ConnectionClosed)
                        this.Open();

                    //Ingresar el destino de la tabla
                    bulkCopy.DestinationTableName = strTable;
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.WriteToServer(dtData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.ConnectionOpen)
                    this.Close();
            }
        }

        public void BulkCopy(string strTable, DataTable dtData, List<SqlBulkCopyColumnMapping> lstColumns)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strTable))
                    throw new ArgumentNullException("strTable");

                if (dtData == null || dtData.Rows.Count <= 0)
                    throw new ArgumentNullException("dtData");

                if (lstColumns == null || lstColumns.Count <= 0)
                    throw new ArgumentNullException("lstColumns");

                //Realizar el bulkCopy a la tabla
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(this.sqlConnection))
                {
                    //Abrir conexión
                    if (this.ConnectionClosed)
                        this.Open();

                    //Agregar columnas
                    foreach (SqlBulkCopyColumnMapping column in lstColumns)
                    {
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = column.DestinationColumn, SourceOrdinal = column.SourceOrdinal });
                    }

                    //Ingresar el destino de la tabla
                    bulkCopy.DestinationTableName = strTable;
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.WriteToServer(dtData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.ConnectionOpen)
                    this.Close();
            }
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }

        public DataTable EjecutarConsulta(string strQuery, object[] args = null)
        {
            DataTable dtData = new DataTable();

            try
            {
                if (string.IsNullOrWhiteSpace(strQuery))
                    throw new ArgumentNullException("strQuery");

                string querySelect = "";

                if (args == null)
                {
                    querySelect = string.Format(strQuery);
                }
                else
                {
                    querySelect = string.Format(strQuery, args);
                }

                //Abrir conexión
                if (this.ConnectionClosed)
                    this.Open();

                DataSet dsDatos = new DataSet("Datos");
                SqlDataAdapter da = new SqlDataAdapter(querySelect, this.sqlConnection);

                da.Fill(dsDatos, "tblDatos");
                if (this.ConnectionOpen)
                    this.Close();

                dtData = dsDatos.Tables["tblDatos"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.ConnectionOpen)
                    this.Close();
            }

            return dtData;
        }
    }
}