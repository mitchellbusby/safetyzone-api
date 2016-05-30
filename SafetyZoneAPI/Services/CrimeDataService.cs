using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SafetyZoneAPI.Services
{
    public class CrimeDataService
    {
        
        public CrimeDataService()
        {
            
        }
        public virtual int DetermineCrimeRatingIndex(double rate)
        {
            if (rate < 162.4)
            {
                return 0;
            }
            else if (rate < 234.4)
            {
                return 1;
            }
            else if (rate < 414.2)
            {
                return 2;
            }
            else if (rate < 688.0)
            {
                return 3;   
            }
            else if (rate >= 688.0)
            {
                return 4;
            }
            return -1;

        }
        public virtual Dictionary<string, double> DetermineLga(double latitude, double longitude)
        {
            var dictResult = new Dictionary<string, double>();
            SqlConnectionStringBuilder csBuilder;
            csBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SafetyZone_dbConnectionString"].ConnectionString);
            using (SqlConnection conn = new SqlConnection(csBuilder.ToString()))
            {
                var cmd = conn.CreateCommand();
                var point = string.Format("POINT({0} {1})", longitude, latitude);
                cmd.CommandText =  @"
                    declare @p geometry;
                    set @p = geometry::STPointFromText(@POINT, 4283);
                    select TOP(1) * FROM SafetyZone_db.dbo.lga WHERE geom.STIsValid()=1 AND geom.STDistance(@p) IS NOT NULL
                    ORDER BY geom.STDistance(@p)";
                cmd.Parameters.AddWithValue("@POINT", point);
                conn.Open();
                var reader = cmd.ExecuteReader();
                string lgaResult = "";
                while (reader.Read())
                {
                    lgaResult = reader[0].ToString();
                }
                reader.Close();
                cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT TOP(1) rate FROM dbo.crimedata 
                    WHERE lga = @lgaResult
                    ORDER BY LEN(lga) DESC
                ";
                cmd.Parameters.Add(new SqlParameter("@lgaResult", lgaResult));
                double resultRate = 0;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resultRate = Double.Parse(reader[0].ToString());
                }
                dictResult[lgaResult] = resultRate;
                return dictResult;
                
            };
        }
        public virtual int InitialiseCrimeStatistics(Dictionary<string, double> valuesToInsert)
        {
            SqlConnectionStringBuilder csBuilder;
            csBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SafetyZone_dbConnectionString"].ConnectionString);
            using (SqlConnection conn = new SqlConnection(csBuilder.ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"IF OBJECT_ID('dbo.crimedata', 'U') IS NOT NULL DROP TABLE dbo.crimedata;
                                    CREATE TABLE dbo.crimedata (lga varchar(160) NOT NULL, rate float);
                                    CREATE CLUSTERED INDEX myIndex ON dbo.crimedata (lga)";
                var result = cmd.ExecuteNonQuery();
                var populateData = new StringBuilder();
                foreach (var value in valuesToInsert.Keys)
                {
                    populateData.Append(string.Format("INSERT INTO dbo.crimedata VALUES('{0}',{1})", value, valuesToInsert[value]));
                }
                cmd = conn.CreateCommand();
                cmd.CommandText = populateData.ToString();
                result += cmd.ExecuteNonQuery();
                return result;
            };
        }
    }
}