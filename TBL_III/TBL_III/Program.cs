using System;
using Npgsql;

public class Program
{
    private static string ConnectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=PostgreSQL123;Database=TBL_III";

    public static void InserirAluno(string nome, int idade)
    {
        string sql = "INSERT INTO alunos (nome, idade) VALUES (@nome, @idade)";

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("nome", nome);
                    cmd.Parameters.AddWithValue("idade", idade);
                    int linhas = cmd.ExecuteNonQuery();
                    Console.WriteLine($" {linhas} aluno(s) inserido(s) com sucesso!");
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao inserir aluno: {ex.Message}");
            }
        }
    }

    public static void ListarAlunos()
    {
        string sql = "SELECT id, nome, idade FROM alunos ORDER BY id";

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nLista de Alunos:");
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Nenhum aluno encontrado.");
                        return;
                    }

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string nome = reader.GetString(1);
                        int idade = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        Console.WriteLine($"ID: {id}, Nome: {nome}, Idade: {idade}");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao listar alunos: {ex.Message}");
            }
        }
    }

    public static void BuscarAlunoPorNome(string nome)
    {
        string sql = "SELECT id, nome, idade FROM alunos WHERE nome ILIKE @nome";

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("nome", "%" + nome + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"\nResultados da busca por '{nome}':");
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Nenhum aluno encontrado.");
                            return;
                        }

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string nomeRetornado = reader.GetString(1);
                            int idade = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                            Console.WriteLine($"ID: {id}, Nome: {nomeRetornado}, Idade: {idade}");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao buscar aluno: {ex.Message}");
            }
        }
    }

    public static void AtualizarIdade(int id, int novaIdade)
    {
        string sql = "UPDATE alunos SET idade = @idade WHERE id = @id";

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("idade", novaIdade);
                    cmd.Parameters.AddWithValue("id", id);
                    int linhas = cmd.ExecuteNonQuery();

                    if (linhas > 0)
                        Console.WriteLine("Idade atualizada com sucesso!");
                    else
                        Console.WriteLine("Aluno não encontrado.");
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao atualizar idade: {ex.Message}");
            }
        }
    }

    public static void ExcluirAluno(int id)
    {
        string sql = "DELETE FROM alunos WHERE id = @id";

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    int linhas = cmd.ExecuteNonQuery();

                    if (linhas > 0)
                        Console.WriteLine("Aluno excluído com sucesso!");
                    else
                        Console.WriteLine("Aluno não encontrado.");
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao excluir aluno: {ex.Message}");
            }
        }
    }

    public static void Main(string[] args)
    {
        int opcao = 0;
        do
        {
            Console.WriteLine("\n  {Menu Alunos}  ");
            Console.WriteLine("1. Inserir aluno");
            Console.WriteLine("2. Listar alunos");
            Console.WriteLine("3. Buscar aluno por nome");
            Console.WriteLine("4. Atualizar idade");
            Console.WriteLine("5. Excluir aluno");
            Console.WriteLine("6. Sair");
            Console.Write("Escolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao))
            {
                Console.WriteLine("Opção inválida!");
                continue;
            }

            switch (opcao)
            {
                case 1:
                    Console.Write("Nome: ");
                    string nome = Console.ReadLine();
                    Console.Write("Idade: ");
                    int idade = int.Parse(Console.ReadLine());
                    InserirAluno(nome, idade);
                    break;

                case 2:
                    ListarAlunos();
                    break;

                case 3:
                    Console.Write("Digite o nome para buscar: ");
                    string busca = Console.ReadLine();
                    BuscarAlunoPorNome(busca);
                    break;

                case 4:
                    Console.Write("ID do aluno: ");
                    int idAtualiza = int.Parse(Console.ReadLine());
                    Console.Write("Nova idade: ");
                    int novaIdade = int.Parse(Console.ReadLine());
                    AtualizarIdade(idAtualiza, novaIdade);
                    break;

                case 5:
                    Console.Write("ID do aluno a excluir: ");
                    int idExclui = int.Parse(Console.ReadLine());
                    ExcluirAluno(idExclui);
                    break;

                case 6:
                    Console.WriteLine("Encerrando o programa...");
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

        } while (opcao != 6);
    }
}
