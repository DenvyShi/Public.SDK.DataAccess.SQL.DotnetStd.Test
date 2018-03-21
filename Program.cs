using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Public.SDK.Database;

namespace Public.SDK.DataAccess.SQL.DotnetStd.Test
{
    class Program
    {
        static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            try
            {

                XmlDocument log4netConfig = new XmlDocument();
                log4netConfig.Load(File.OpenRead("Config//log4net.config"));
                var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(Hierarchy));
                XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

                _logger.Debug("Program started");
                TestMsSQL();
                TestMySQL();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static void TestMySQL()
        {
            _logger.Debug($"Begin to TestMySQL");
            string myConnectionString;

            myConnectionString = "server=127.0.0.1;uid=root;" +
                                 "pwd=p@ssw0rd;database=sandbox";
            using (Public.SDK.Database.MySQLDB db = new MySQLDB(myConnectionString))
            {
                string sql = "SELECT * FROM sandbox.Staffs";

                var ds = db.ExecuteDataSet(sql, CommandType.Text);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _logger.Debug($"Found {ds.Tables[0].Rows.Count} records");
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        _logger.Debug($"{dataRow["StaffId"]} - {dataRow["Name"]} ");
                    }
                }
            }

        }

        private static void TestMsSQL()
        {
            _logger.Debug($"Begin to Test Ms SQL");
            using (Public.SDK.Database.MSSQLDB db = new MSSQLDB("Server=127.0.0.1;Database=Test;User Id=sa;Password=p@ssw0rd;"))
            {
                string sql = @"SELECT [sensorid]
      ,[macaddress]
      ,[sensorname]
      ,[dateadded]
      ,[temperature]
      ,[maxreaddate]
      ,[maxrecdate]
  FROM [dbo].[sensors] ORDER BY maxrecdate DESC";
                var ds = db.ExecuteDataSet(sql, CommandType.Text);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _logger.Debug($"Found {ds.Tables[0].Rows.Count} Loggers");
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        _logger.Debug($"{dataRow["sensorid"]} - {dataRow["macaddress"]} - {dataRow["sensorname"]} - {dataRow["dateadded"]} - {dataRow["temperature"]} - {dataRow["maxreaddate"]} - {dataRow["maxrecdate"]}");
                    }
                }
            }
        }
    }
}
