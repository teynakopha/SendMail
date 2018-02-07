using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Newtonsoft.Json;
using Npgsql;

namespace SendMail
{
    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine("enter directory of files to send mail:");
            string dir = Console.ReadLine();
            string file = "C:\\Users\\administrator01\\Desktop\\test.xls";
            MailMessage mail = new MailMessage("SECAP-Backup01@segroup.co.th", "sattaya@got.co.th");
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "10.9.1.4";
            mail.Subject = "อีเมลล์ นี้เก็บข้อมูล report netvault SECAP";
            mail.Body = "Netvault backup server:10.9.1.168 \n from Postgres on port:51486 \n On Table : phasehistory";
            // Create  the file attachment for this e-mail message.
            getDate();
            Attachment data = new Attachment(file);
            mail.Attachments.Add(data);
            client.Send(mail);
            Console.WriteLine("Press RETURN to exit");
            Console.ReadLine();

        }
        public static void getDate()
        {
         string Host = "localhost";
         string User = "postgres";
         string DBname = "netvault_scheduling";
         string Password = "p@ssw0rd";
         string Port = "51486";
         string connString =
    String.Format(
        "Server={0}; User Id={1}; Database={2}; Port={3}; Password={4}; SSL Mode=Prefer; Trust Server Certificate=true",
        Host,
        User,
        DBname,
        Port,
        Password);

        var conn = new NpgsqlConnection(connString);
            Console.Out.WriteLine("Opening connection");
            conn.Open();
            Console.Out.WriteLine("Opened db");
            var command = conn.CreateCommand();
            command.CommandText = "select * from phasehistory;";
            var reader = command.ExecuteReader();
            //get the data reader, etc.
            while (reader.Read())
            {
                //string startDate = reader["startdate"].ToString();
                //string title = reader["title"].ToString();
                //string jobid = reader["jobid"].ToString();
                //string messege = reader["message"].ToString();
                //string instance = reader["instance"].ToString();
                //string show = string.Format("{0},{1},{2},{3},{4}", startDate, jobid, instance, title, messege);

                Console.WriteLine(reader[6].ToString()+reader[12].ToString());
            }

            //Console.WriteLine(JsonConvert.SerializeObject(objs));
            Console.ReadLine();
        }

        private static string script()
        {
            string sc = "SELECT DISTINCT get_time_from_systime(phasehistory.starttime) AS starttime, " +
                "get_date_from_systime(phasehistory.starttime) AS startdate," +
                "phasehistory.duration," +
    "phasehistory.starttime + phasehistory.duration AS endsystime," +
    "get_date_from_systime(phasehistory.starttime + phasehistory.duration) AS enddate," +
    "get_time_from_systime(phasehistory.starttime + phasehistory.duration) AS endtime," +
    "jobdescription.title, phasehistory.jobid, phasehistory.instance, " +
    "phasehistory.phasenumber, jobdescription.clientname, " +
    "COALESCE(phasehistory.bytestransferred, 0::bigint) AS bytestransferred," +
    "phasehistory.message, phasehistory.exitstatus AS exitcode, " +
    "jobdescription.primarypluginname AS plugin, jobdescription.selectionsset," +
    "jobdescription.seloptionsset, jobdescription.scheduleset," +
    "jobdescription.targetset, jobdescription.advoptionsset, " +
    "jobdescription.jobtype, jobdescription.policyname AS policy," +
    "phasehistory.starttime AS startsystime, streams.isencrypted AS encrypted," +
    "calculate_transfer_rate(phasehistory.bytestransferred, phasehistory.duration) AS transferrate," +
    "COALESCE((SELECT jobinstance.filecount" +
     "      FROM jobinstance" +
      "    WHERE jobinstance.jobid = phasehistory.jobid AND jobinstance.instance = phasehistory.instance AND phasehistory.phasenumber = 1), 0::bigint) AS filecount" +


  " FROM phasehistory" +
   "LEFT JOIN jobdescription ON phasehistory.jobid = jobdescription.jobid" +
   "LEFT JOIN dblink(get_dblink_connstr('netvault_mediamanagement'::text), 'SELECT DISTINCT sessionjobid, sessionphase, sessioninstance, isencrypted FROM streams_view'::text) streams(sessionjobid integer, sessionphase integer, sessioninstance integer, isencrypted boolean) ON phasehistory.jobid = streams.sessionjobid AND phasehistory.instance = streams.sessioninstance AND phasehistory.phasenumber = streams.sessionphase" +
  "WHERE jobdescription.jobtype <> 3; ";
            return sc;
        }

    }
}
