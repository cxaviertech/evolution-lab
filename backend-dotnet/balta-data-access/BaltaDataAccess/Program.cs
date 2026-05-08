using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Data;

namespace BaltaDataAccess
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=localhost,1433; Database=balta;User Id=sa;Password=Since@1993!;TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                Console.WriteLine("Conectado");
                connection.Open();


                using (var command = new SqlCommand())
                {
                    // CreateManyCategory(connection);
                    // DeleteCategory(connection);
                    // UpdateCategory(connection);
                    // ListCategories(connection);
                    // GetCategory(connection);
                    // ExecuteProcedure(connection);
                    // ExecuteReadProcedure(connection); 
                    // ExecuteScarlar(connection);
                    // ReadView(connection);
                    OneToOne(connection);
                }
            }

            Console.WriteLine("** Processo Finalizado **");
        }

        static void ListCategories(SqlConnection connection)
        {
            var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
            foreach (var item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void GetCategory(SqlConnection connection)
        {
            var category = connection
                .QueryFirstOrDefault<Category>(
                    "SELECT TOP 1 [Id], [Title] FROM [Category] WHERE [Id]=@id",
                    new
                    {
                        id = "af3407aa-11ae-4621-a2ef-2028b85507c4"
                    });
            Console.WriteLine($"{category.Id} - {category.Title}");

        }

        static void CreateCategory(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Catregoria destinada a serviços do AWS";
            category.Featured = false;

            //SQL Injection                
            var insertSql = @"INSERT INTO 
                                [Category] 
                                VALUES (
                                    @Id,
                                    @Title, 
                                    @Url, 
                                    @Summary, 
                                    @Order, 
                                    @Description, 
                                    @Featured)";
            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            Console.WriteLine($"{rows} linha(s) inserida(s)");
        }
        static void UpdateCategory(SqlConnection connection)
        {
            var updateQuery = "UPDATE [Category] SET [Title]=@title WHERE [Id]=@Id";
            var rows = connection.Execute(updateQuery, new
            {
                id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                title = "Front-End"
            });

            Console.WriteLine($"{rows} linhas atualizadas");
        }

        static void DeleteCategory(SqlConnection connection)
        {
            var deleteQuery = "DELETE [Category] WHERE [Id]=@id";
            var rows = connection.Execute(deleteQuery, new
            {
                id = new Guid("231570fb-ff4c-4281-bbfa-2916000f247e"),
            });

            Console.WriteLine($"{rows} registros excluídos");
        }

        static void CreateManyCategory(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Catregoria destinada a serviços do AWS";
            category.Featured = false;

            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "Nova Categoria";
            category2.Url = "categoria nova";
            category2.Summary = "Categoria nova";
            category2.Order = 9;
            category2.Description = "Categoria";
            category2.Featured = false;

            //SQL Injection                
            var insertSql = @"INSERT INTO 
                                [Category] 
                                VALUES (
                                    @Id,
                                    @Title, 
                                    @Url, 
                                    @Summary, 
                                    @Order, 
                                    @Description, 
                                    @Featured)";
            var rows = connection.Execute(insertSql, new[]
                {
                        new {
                        category.Id,
                        category.Title,
                        category.Url,
                        category.Summary,
                        category.Order,
                        category.Description,
                        category.Featured
                        },
                        new {
                        category2.Id,
                        category2.Title,
                        category2.Url,
                        category2.Summary,
                        category2.Order,
                        category2.Description,
                        category2.Featured
                        }
                    });
            Console.WriteLine($"{rows} linha(s) inserida(s)");
        }

        static void ExecuteProcedure(SqlConnection connection)
        {
            var procedure = "[spDeleteStudent]";
            var pars = new { StudentId = new Guid("ff67af2d-32cc-48e8-ac98-170c996fee7f") };
            var affectRows = connection.Execute(
                procedure,
                pars,
                commandType: CommandType.StoredProcedure);
            Console.WriteLine($"{affectRows} linhas afetadas");
        }

        static void ExecuteReadProcedure(SqlConnection connection)
        {
            var procedure = "[spGetCoursesByCategory]";
            var pars = new { CategoryId = new Guid("09ce0b7b-cfca-497b-92c0-3290ad9d5142") };
            var courses = connection.Query<Category>(
                procedure,
                pars,
                commandType: CommandType.StoredProcedure);

            foreach (var item in courses)
            {
                Console.WriteLine(item.Id + "|" + item.Title);
            }
        }
    
        static void ExecuteScarlar(SqlConnection connection)
        {
            var category = new Category();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Categoria destinada a serviços do AWS";
            category.Featured = false;

            //SQL Injection                
            var insertSql = @"INSERT INTO 
                                [Category] 
                            OUTPUT inserted.[Id]
                            VALUES (
                                    NEWID(),
                                    @Title, 
                                    @Url, 
                                    @Summary, 
                                    @Order, 
                                    @Description, 
                                    @Featured)";

            var rows = connection.ExecuteScalar<Guid>(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            Console.WriteLine($"categoria inserida foi: {rows} ");
        }
        static void ReadView(SqlConnection connection)
        {
            var sql = "SELECT * FROM [vwCourses]";
            var courses = connection.Query<Category>(sql);
            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void OneToOne(SqlConnection connection)
        {
            var sql = @"
                SELECT 
                    * 
                FROM 
                    [CareerItem] 
                INNER JOIN 
                    [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

            var items = connection.Query<CareerItem, Course, CareerItem>(
                sql,
                (careerItem, course) =>
                {
                    careerItem.Course = course;
                    return careerItem;
                }, splitOn: "Id");

            foreach (var item in items)
            {
                Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
            }
        }   
    }
}