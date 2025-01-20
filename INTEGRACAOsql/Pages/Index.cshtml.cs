using INTEGRACAOsql.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace INTEGRACAOsql.Pages
{
    using Microsoft.AspNetCore.Hosting.Server;
    using Mysqlx.Expr;
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class IndexModel : PageModel
    {
        // variáveis globais
        public static string setComando { get; set; }
        public static string logs { get; set; }
        public static string myConnectionString { get; set; }
        public static string server { get; set; }
        public static string uid { get; set; }
        public static string pwd { get; set; }
        public static string database { get; set; }
        public static bool statusConexao { get; set; }
        
        MySqlConnection myConnection;
        
        string comando = IndexModel.setComando;

        private string conectar(string comando)
        {
            try
            {

                myConnection = new MySqlConnection(myConnectionString);
                Console.WriteLine("passou na db");

                // Criando o comando e setando os parâmetros
                MySqlCommand myCommand = new MySqlCommand
                {
                    Connection = myConnection,
                    CommandText = comando
                };

                // Executando o comando e vendo os resultados
                using var myReader = myCommand.ExecuteReader();
                string resultado = "";
                int index = 0;

                while (myReader.Read())
                {
                    index = index + 1;
                    Console.WriteLine("Index: " + index);
                    if (index > 3)
                    {
                        Console.WriteLine($"Index: {index}, com resultado: {resultado}");
                        CriarArquivo(resultado);
                    } else
                    {
                        var id = myReader.GetInt32("idusuarios");
                        var usuarios = myReader.GetString("nomeusuarios");

                        // Guardar e enviar os resultados para a variável logs
                        Console.WriteLine("Logs: " + resultado);
                        resultado += $"{id}, {usuarios}\n";

                        Console.WriteLine($"Index: {index}, com resultado: {resultado}");

                        if (index == 3)
                        {
                            IndexModel.logs = $"Resultado: {resultado}\n";
                            CriarArquivo(resultado);
                        }
                    }
                        
                }

                myConnection.Close();
                return resultado;  // Retorna o resultado obtido

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                var mensagem = $"Erro: {ex.Message}";
                return mensagem;  // Retorna a mensagem de erro
            }
        }

        // Método para criar um arquivo
        public static void CriarArquivo(string conteudo)
        {
            // caminho extremamente especificamente específico, mas vou mudar isso
            using (StreamWriter writer = System.IO.File.AppendText("C:/Users/Junior/Source/Repos/INTEGRACAOsql/logs/logs.txt"))
            {
                writer.WriteLine(conteudo);
            }
        }

        // coletando o input
        [HttpPost]
        public IActionResult OnPostComandos(string comando)
        {
            Console.WriteLine("Recebido no Post: " + comando);
            IndexModel.setComando = comando;

            comando = IndexModel.setComando;

            // ter certeza de que a conexão está online

            try
            {
                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open();
                IndexModel.statusConexao = true;
                Console.WriteLine("Conexão aberta");

                try
                {
                    conectar(comando);

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                IndexModel.statusConexao = false;
            }

            // return pq se não o IActionResult dá cria
            return RedirectToPage();
        }

        void abrirConexao(string stringconexao, string database)
        {
            try
            {
                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open();
                IndexModel.statusConexao = true;
                Console.WriteLine("Conexão aberta");
                
                try
                {
                teste(database);

                } catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            } catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                IndexModel.statusConexao = false;
            }
        }

        // coletando o input de login
        [HttpPost]
        public IActionResult OnPostCredenciais(string server, string uid, string pwd, string database)
        {

            Console.WriteLine($"Server: {server}");
            Console.WriteLine($"UID: {uid}");
            Console.WriteLine($"Password: {pwd}");
            Console.WriteLine($"Database: {database}");

            myConnectionString = $"server={server};uid={uid};pwd={pwd};database={database}";

            abrirConexao(myConnectionString, database);
            // return pq se não o IActionResult dá cria
            return RedirectToPage();
        }

        private string teste(string database)
        {
            try
            {

                MySqlConnection myConnection;

                myConnection = new MySqlConnection(myConnectionString);

                if (myConnection.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Conexão não está aberta. Tentando abrir...");
                    myConnection.Open();
                }

                Console.WriteLine("DB: " + database);

                /* inserir este comando na funcao TESTE()
                * SHOW TABLES - FEITO OK;
                */

                // Criando o comando e setando os parâmetros
                MySqlCommand myCommand = new MySqlCommand
                {
                    Connection = myConnection,
                    CommandText = $"SHOW TABLES FROM {database}"
                };

                // Executando o comando e vendo os resultados
                using var myReader = myCommand.ExecuteReader();
                string resultado = "";

                while (myReader.Read())
                {
                        //var table = myReader.GetSchemaTable(); DESCARTA 
                        //Guardar e enviar os resultados para a variável logs
                        //Console.WriteLine("Logs: " + table);
                        //resultado = $"{table}";

                        // nova versão >

                        string tableName = myReader.GetString(0);
                        Console.WriteLine($"Tabela encontrada: {tableName}");
                        resultado += $"{tableName} \n";
                        CriarArquivo(resultado);
                        IndexModel.logs = resultado;

                }

                myConnection.Close();
                return resultado;  // Retorna o resultado obtido

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                var mensagem = $"Erro: {ex.Message}";
                return mensagem;  // Retorna a mensagem de erro
            }
        }


        // get normal
        [HttpGet]
        public void OnGet()
        {
        }
    }
}
