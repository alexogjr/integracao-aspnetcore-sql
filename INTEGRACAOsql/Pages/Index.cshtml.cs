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
        // vari�veis globais
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

                // Criando o comando e setando os par�metros
                MySqlCommand myCommand = new MySqlCommand
                {
                    Connection = myConnection,
                    CommandText = comando
                };

                myConnection.Open();

                // Executando o comando e vendo os resultados
                using var myReader = myCommand.ExecuteReader();
                string resultado = "";

                while (myReader.Read())
                {
                    //var result = myReader.GetValue(0);
                    //Console.WriteLine($"Resultado: {result}");


                        // tenho que acertar esse for aqui, mas j� s�o 00:35 e preciso acordar cedo :c
                        for (int i = 0; i < myReader.FieldCount; i++)
                        {

                            var valor = myReader.GetValue(i); // Obter o valor gen�rico

                            // indesejado > valor sendo floodado no console por conta desse amig�o aqui
                            resultado += $"{valor}\n"; // Concatenar os valores separados por tabula��o

                            Console.WriteLine($"Dentro do for: " + resultado);
                        }

                         IndexModel.logs += $"Resultado: {resultado}";
                         CriarArquivo(resultado);

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

        // M�todo para criar um arquivo
        public static void CriarArquivo(string conteudo)
        {
            // caminho extremamente especificamente espec�fico, mas vou mudar isso
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

            // ter certeza de que a conex�o est� online

            try
            {
                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open();
                IndexModel.statusConexao = true;
                Console.WriteLine("Conex�o aberta " + 2);

                try
                {
                    Console.WriteLine("Abriu o try-catch da func de comandos");
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

            // return pq se n�o o IActionResult d� cria
            return RedirectToPage();
        }

        // eu sei, coment�rio desnecess�rio, mas preciso dizer que essa fun��o aqui abre a conex�o
        void abrirConexao(string stringconexao, string database)
        {
            try
            {
                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open();
                IndexModel.statusConexao = true;
                Console.WriteLine("Conex�o aberta");
                
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
            // return pq se n�o o IActionResult d� cria
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
                    Console.WriteLine("Conex�o n�o est� aberta. Tentando abrir...");
                    myConnection.Open();
                }

                Console.WriteLine("DB: " + database);

                /* inserir este comando na funcao TESTE()
                * SHOW TABLES - FEITO OK;
                */

                // Criando o comando e setando os par�metros
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
                        //Guardar e enviar os resultados para a vari�vel logs
                        //Console.WriteLine("Logs: " + table);
                        //resultado = $"{table}";

                        // nova vers�o >

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
