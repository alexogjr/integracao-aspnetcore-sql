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

        private string inserirComando(string comando)
        {
            try
            {

                myConnection = new MySqlConnection(myConnectionString);

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

                // Verifica se o reader tem linhas antes de come�ar o while
                if (!myReader.HasRows)
                {
                    Console.WriteLine("Nenhum registro encontrado.");
                    IndexModel.logs += "Nenhum registro encontrado.\n";
                    CriarArquivo("Nenhum registro encontrado.");
                    return "Nenhum registro encontrado.";
                }

                while (myReader.Read())
                {

                    // gambiarra do gepeteco pra organizar as strings nas logs
                    string linhaAtual = "";
                        for (int i = 0; i < myReader.FieldCount; i++)
                        {
                            var valor = myReader.GetValue(i); // coletar poss�veis colunas
                            // filtra valores num�ricos pra n�o irem para as logs
                            if (!(valor is int || valor is long || valor is decimal || valor is double || valor is float))
                            {
                                linhaAtual += $"{valor}\t";
                            }
                        }

                    resultado += linhaAtual.TrimEnd() + "\n"; // quebra de linha
                    IndexModel.logs += $"{linhaAtual.TrimEnd()}\n"; // loggando a linha
                    CriarArquivo(linhaAtual.TrimEnd()); // salvando a linha no logs.txt

                }

                return resultado;

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                var mensagem = $"Erro: {ex.Message}";
                return mensagem;
            }
        }

        // M�todo para criar um arquivo
        public static void CriarArquivo(string conteudo)
        {
            // caminho extremamente especificamente espec�fico, mas vou mudar isso
            using (StreamWriter writer = System.IO.File.AppendText("C:/Users/Junior/Source/Repos/integracao-aspnetcore-sql/logs/logs.txt"))
            {
                writer.WriteLine(conteudo);
            }
        }
        
        // captura o input do html e recebe o comando. utiliza a funcao "inserirComando(comando) pra envi�-lo para a DB
        [HttpPost]
        public IActionResult OnPostComandos(string comando)
        {
            IndexModel.setComando = comando;

            comando = IndexModel.setComando;
            
            // esses blocos garantem que a conex�o esteja ativa, e, se n�o estiver, ele reabre ela e segue
            try
            {
                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open();
                IndexModel.statusConexao = true;

                try
                {
                    inserirComando(comando);

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
                    verTabelas(database);

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
            myConnectionString = $"server={server};uid={uid};pwd={pwd};database={database}";

            abrirConexao(myConnectionString, database);
            // return pq se n�o o IActionResult d� cria
            return RedirectToPage();
        }

        // fechando a conex�o 
        [HttpPost]
        public IActionResult OnPostFecharConexao()
        {
            try
            {
                myConnection = new MySqlConnection(myConnectionString);
                myConnection.Close();
                Console.WriteLine("Conex�o fechada");

                IndexModel.logs = "Conex�o fechada com sucesso";
                statusConexao = false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Erro ao fechar conex�o: " + ex.Message);
            }

            // return pq se n�o o IActionResult d� cria
            return RedirectToPage();
        }

        // essa fun��o � executada logo ap�s a conex�o do DB ser estabelecida, ela captura as tabelas dispon�veis na db e imprime no console
        private string verTabelas(string database)
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

                        string tableName = myReader.GetString(0);
                        Console.WriteLine($"Tabela encontrada: {tableName}");
                        resultado += $"{tableName} \n";
                        CriarArquivo(resultado);
                        IndexModel.logs = resultado;

                }

                return resultado;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                var mensagem = $"Erro: {ex.Message}";
                return mensagem; 
            }
        }

        [HttpGet]
        public void OnGet()
        {
        }
    }
}
