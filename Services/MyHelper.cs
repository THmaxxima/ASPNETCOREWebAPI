using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public class MyHelper
    {
        public static string GetAuthorizationHeader(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
            {
                return string.Empty;
            }
            return request.Headers["Authorization"].ToString().Trim();
        }
        public static string GetErrorFromStoreToApiError(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0
                || ds.Tables[0].Rows.Count == 0
                || ds.Tables[0].Columns.Count == 0
                || !ds.Tables[0].Rows[0][0].ToString().ToUpper().StartsWith("ERROR:"))
                return string.Empty;

            DataRow row = ds.Tables[0].Rows[0];
            string oldValue = row[0].ToString();
            string tmpValue = row[0].ToString().ToUpper();
            string findWord = "ERROR:";
            int startIdx = tmpValue.IndexOf(findWord);
            string newTmpValue = oldValue.Substring(startIdx + findWord.Length, oldValue.Length - findWord.Length);
            return newTmpValue.Trim();
        }
        public static DataSet RemoveColumnFromStore(DataSet ds, string[] removeColumns)
        {
            DataSet dsResult = ds.Copy();
            DataColumnCollection columns = dsResult.Tables["Data"].Columns;
            if (columns.Count == 0)
            {
                return dsResult;
            }

            foreach (String columnName in removeColumns)
            {
                if (!columns.Contains(columnName))
                    continue;
                columns.Remove(columnName);
            }
             
            return dsResult;
        }
        public static DataSet ChangeColumnNameFromMapping(DataSet ds
                        , Dictionary<string, string> dicOfColumnMapping)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Columns.Count == 0)
                return ds;

            DataSet dsResult = ds.Copy();
            if (dicOfColumnMapping == null || dicOfColumnMapping.Count == 0)
                return dsResult;

            DataColumnCollection columns = dsResult.Tables[0].Columns;
            foreach (KeyValuePair<string, string> kv in dicOfColumnMapping)
            {
                string colName = kv.Key;
                string newColName = kv.Value;
                if (columns.Contains(colName))
                {
                    columns[colName].ColumnName = newColName;
                }
            }
            return dsResult;
        }

        public static string GetDBConn()
        {
            string connectionString = Environment.GetEnvironmentVariable("ASPNETCORE_SQLCONNECTION_STRING").ToString();
            return connectionString;
        }

        public static string GetX10APIURL()
        {
            string X10APIUrl = "";
            X10APIUrl = Environment.GetEnvironmentVariable("ASPNETCORE_X10API_URL");
            return X10APIUrl;
        }

    }

    
}
