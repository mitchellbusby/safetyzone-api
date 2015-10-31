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
            SqlConnectionStringBuilder csBuilder;
            csBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SafetyZone_dbConnectionString"].ConnectionString);
            //After you have built your connection string, you can use the SQLConnection class to connect the SQL Database server:
            using (SqlConnection conn = new SqlConnection(csBuilder.ToString()))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select top(1) * from lga";
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int i=0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine(reader[i]);
                    }
                } 
                /*var query = "select TOP(1) * from lga";
                using (var da = new SqlDataAdapter(query, conn))
                {

                }*/
            };
        }
        public virtual int DetermineCrimeRatingIndex(double rate)
        {
            return 0;
        }
        public virtual string DetermineLga(double latitude, double longitude)
        {
            SqlConnectionStringBuilder csBuilder;
            csBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SafetyZone_dbConnectionString"].ConnectionString);
            using (SqlConnection conn = new SqlConnection(csBuilder.ToString()))
            {
                var cmd = conn.CreateCommand();
                //cmd.CommandType = System.Data.CommandType.Text;
                //cmd.CommandText = "select top(1) * from lga ";
                var point = string.Format("POINT({0} {1})", longitude, latitude);
                cmd.CommandText =  @"
                    declare @p geometry;
                    set @p = geometry::STPointFromText(@POINT, 4283);
                    select TOP(1) * FROM SafetyZone_db.dbo.lga WHERE geom.STIsValid()=1 AND geom.STDistance(@p) IS NOT NULL
                    ORDER BY geom.STDistance(@p)";
                cmd.Parameters.AddWithValue("@POINT", point);
                //cmd.Parameters.AddWithValue("@longitude", longitude.ToString());
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return reader[0].ToString();
                }
                return null;
                /*var query = "select TOP(1) * from lga";
                using (var da = new SqlDataAdapter(query, conn))
                {

                }*/
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
                cmd.CommandText = @"DROP TABLE IF EXISTS crimedata;
                                    CREATE TABLE crimedata (lga varchar(160) NOT NULL, rate float)";
                var result = cmd.ExecuteNonQuery();
                var populateData = new StringBuilder();
                foreach (var value in valuesToInsert.Keys)
                {
                    populateData.Append(string.Format("INSERT INTO crimedata VALUES({0},{1})", value, valuesToInsert[value]));
                }
                cmd = conn.CreateCommand();
                cmd.CommandText = populateData.ToString();
                result += cmd.ExecuteNonQuery();
                return result;
            };
        }
    }
}