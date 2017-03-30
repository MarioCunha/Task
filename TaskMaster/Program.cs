using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TaskMaster
{
    class Program
    {
        private void readXml(string XmlPath) {
            try{

                    XElement root = XElement.Load(XmlPath);
                    XElement Users = new XElement("users");
                    IEnumerable<XElement> usersXml =
                        from el in root.Elements("user")
                        let userid = (decimal)el.Element("userid")
                        orderby userid
                        select el;
                    foreach (XElement el in usersXml)
                    {

                        Users.Add(

                                new XElement("user",
                                    new XElement("userid", (decimal)el.Element("userid")),
                                    new XElement("firstname", (string)el.Element("firstname")),
                                    new XElement("surname", (string)el.Element("surname")),
                                    new XElement("username", (string)el.Element("username")),
                                    new XElement("type", (string)el.Element("type")),
                                    new XElement("lastlogintime", Convert.ToDateTime((string)el.Element("lastlogintime")).ToString("o"))
                              )
                            );
                    }
                    Users.Save("xml_out.xml");
            }
            catch (Exception e) {
                Console.WriteLine("Xml could not be read.");
                Console.WriteLine(e.Message);
            }

            
        }

        private void readCsv(string CSVPath) {
            try {
                string[] allLines = File.ReadAllLines(CSVPath);

                StringBuilder csvcontent = new StringBuilder();

                csvcontent.AppendLine("User ID,First Name,Last Name,Username,User Type,Last Login Time");
          
                var query = from line in allLines
                            where line != "User ID,First Name,Last Name,Username,User Type,Last Login Time" 
                            let data = line.Split(',')
                            orderby Convert.ToDecimal(data[0])
                            select new
                            {
                                userid = data[0],
                                firstname = data[1],
                                surname = data[2],
                                username = data[3],
                                type = data[4],
                                lastlogintime = Convert.ToDateTime(data[5]).ToString("o")
                            };
                foreach (var el in query)
                {
                    csvcontent.AppendLine(el.userid+","+el.firstname+","+el.surname+","+el.username+","+el.type+","+el.lastlogintime);
                }
                File.AppendAllText("csv_out.csv",csvcontent.ToString());
            }
            catch (Exception e) {
                Console.WriteLine("Csv could not be read.");
                Console.WriteLine(e.Message);
            }

        
        }

        private void readJson(string JsonPath) {
            try {

                string st = File.ReadAllText(JsonPath);

                JArray json = JArray.Parse(st);
                StringBuilder jsoncontent = new StringBuilder();
                var query = from data in json
                            orderby data["user_id"].Value<int>()
                            select new 
                            {
                                user_id = data["user_id"].Value<int>(),
                                first_name = data["first_name"].Value<string>(),
                                last_name = data["last_name"].Value<string>(),
                                username = data["username"].Value<string>(),
                                user_type = data["user_type"].Value<string>(),
                                last_login_time = Convert.ToDateTime(data["last_login_time"].Value<string>()).ToString("o")
                            };


                File.WriteAllText("json_out.json", JsonConvert.SerializeObject(query, Formatting.Indented));

            }
            catch (Exception e) {
                Console.WriteLine("Json could not be read.");
                Console.WriteLine(e.Message);
            }
        }


        static void Main(string[] args)
        {
            Program exec = new Program();
            exec.readXml("users.xml");
            exec.readCsv("users.csv");
            exec.readJson("users.json");
        }
    }
}