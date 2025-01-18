using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using System.Collections.Generic;
using System.Data.Common;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using System.Security.Cryptography.Xml;
using Org.BouncyCastle.Asn1.Cms;

namespace INTEGRACAOsql.Pages
{
    public class IndexModel : PageModel
    {

        public static string logs { get; set; }

        public static void conectarDB()
        {

            Console.WriteLine("Entrou aqui");

            MySqlConnection myConnection;

            string myConnectionString;
            //set the correct values for your server, user, password and database name
            myConnectionString = "server=172.93.104.61;uid=u660_KW6rMuJi16;pwd=OJLAy4Jl@3kqHdmScHOSpl!8;database=s660_globais";

            try
            {
                Console.WriteLine("Block try catch");

                myConnection = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString);
                Console.WriteLine("Chegou aqui");
                //open a connection

                myConnection.Open();
                Console.WriteLine("Abriu conexão");

                // create a MySQL command and set the SQL statement with parameters
                MySqlCommand myCommand = new MySqlCommand();
                myCommand.Connection = myConnection;
                myCommand.CommandText = @"SELECT * FROM usuarios";
                Console.WriteLine("Selecionou");

                // execute the command and read the results
                using var myReader = myCommand.ExecuteReader();
                {
                    while (myReader.Read())
                    {
                        Console.WriteLine("Block while");
                        var id = myReader.GetInt32("idusuarios");
                        var usuarios = myReader.GetString("nomeusuarios");

                        Console.WriteLine($"ID do Usuário: {id}");
                        Console.WriteLine($"Nome do Usuário: {usuarios}");

                        var mensagem = $"{id}, {usuarios}";
                        IndexModel.logs = mensagem;
                    }
                }
                myConnection.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                //string message = ex.Message;
            }
        }
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        //public static void mainLogs(string logging)
        //{
        //    string logs = logging;
        //}

        public void OnGet()
        {
            conectarDB();
             Console.WriteLine("Aqui: " + IndexModel.logs);
        }
    }
}
