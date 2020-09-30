using System;
using System.Configuration;
using SapNwRfc;

namespace ShipExecSapConnectorUI
{
    class Program
    {
        private const string table = "ZCOSS_V_THRD";
        private const string delimiter = "|";
        private const string no_data = " ";
        private const string options  = "MANDT,KUNRE,CARRIER,ZCOSS_BILLPARTY,SHP_THRDA";
        private const string filter = "";

        static void Main(string[] args)
        {  
            SapRfcReadTable sapRfcReadTable = new SapRfcReadTable();
            var rfc = sapRfcReadTable;
            var data = rfc.SapReadTable(table, delimiter, no_data, options, filter);
            foreach(var item in data.Data)
            {
                Console.WriteLine(item.Wa);
            }
            // System.Console.WriteLine("Done!");
        }
    }

    public class SapRfcReadTable
    {
        public ClassRfcReadTable SapReadTable(string query_table, string delimiter, string no_data, string fields, string filter)
        {
            SapLibrary.EnsureLibraryPresent();
            ClassRfcReadTable data = null;
            // System.Console.WriteLine("Starting...");
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using var conn = new SapConnection(connectionString);
            conn.Connect();
            // Console.WriteLine("SAP Connction was opened...");
            ClassRfcReadTableInput fc = new ClassRfcReadTableInput();
            fc.QueryTable = query_table;
            fc.Delimiter = delimiter;
            fc.NoData = no_data;
            string[] af = fields.Split(',');
            fc.Fields = new RFC_DB_FLD[af.Length];
            for (int i = 0; i < af.Length; i++)
            {
                fc.Fields[i] = new RFC_DB_FLD();
                fc.Fields[i].FieldName = af[i];
            }
            string[] ao = filter.Split(',');
            if (ao != null && ao.Length > 0) fc.Options = new RFC_DB_OPT[ao.Length]; 
            for (int i = 0; i < ao.Length; i++)
            {
                fc.Options[i] = new RFC_DB_OPT();
                if (!string.IsNullOrEmpty(ao[i]))
                {
                    if (ao[i].Length > 72)
                    {
                        throw new Exception("Error by rfc_read_table:; an option string is longer as 72 -> " + ao[i]);
                    }
                    fc.Options[i].Text = ao[i];
                }
            }
            using var func = conn.CreateFunction("RFC_READ_TABLE");
            data = func.Invoke<ClassRfcReadTable>(fc);         
            return data;
        }
    }

    public class ClassRfcReadTableInput : IDisposable
    {
        [SapName("QUERY_TABLE")]
        public string QueryTable { get; set; }

        [SapName("DELIMITER")]
        public string Delimiter { get; set; }

        [SapName("NO_DATA")]
        public string NoData { get; set; }

        [SapName("FIELDS")]
        public RFC_DB_FLD[] Fields { get; set; }

        [SapName("OPTIONS")]
        public RFC_DB_OPT[] Options { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class ClassRfcReadTable : IDisposable
    {
        [SapName("QUERY_TABLE")]
        public string QueryTable { get; set; }

        [SapName("DELIMITER")]
        public string Delimiter { get; set; }

        [SapName("NO_DATA")]
        public string NoData { get; set; }

        [SapName("ROWSKIPS")]
        public long RowSkips { get; set; }

        [SapName("ROWCOUNT")]
        public long RowCount { get; set; }

        [SapName("FIELDS")]
        public RFC_DB_FLD[] Fields { get; set; }

        [SapName("OPTIONS")]
        public RFC_DB_OPT[] Options { get; set; }

        [SapName("DATA")]
        public TAB512[] Data { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }        
    }

    public class TAB512 : IDisposable
    {
        [SapName("WA")]
        public string Wa { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }         
    }

    public class RFC_DB_FLD : IDisposable
    {
        [SapName("FIELDNAME")]
        public string FieldName { get; set; }
        [SapName("OFFSET")]
        public int Offset { get; set; }
        [SapName("LENGTH")]
        public int Length { get; set; }
        [SapName("TYPE")]
        public string Type { get; set; }
        [SapName("FIELDTEXT")]
        public string FieldText { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }         
    }

    public class RFC_DB_OPT : IDisposable
    {
        [SapName("TEXT")]
        public string Text { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
