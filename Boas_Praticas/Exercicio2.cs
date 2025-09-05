using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

// 1. Nomenclatura vaga e inconsistente:
// Classe em portugês, os atributos devem seguir a classe.
public class Tarefa
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class Program
{
    // 2. Lista Static Global:
    // Deveria estar em uma classe própria
    // Se poassível, deveria ser um banco de dados - MSSQL, SQLite
    private static List<Tarefa> tarefas = new List<Tarefa>();

    public static void Main(string[] a)
    {
        // 3. Falta injeção de dependência
        // Falta uma classe service para fazer a lógica
        var builder = WebApplication.CreateBuilder(a);
        var app = builder.Build();

        // 4. Falta lógica de negócio
        app.MapGet("/", () => "API de Tarefas, mas com código ruim.");

        // 5. Criação ou recepção de dados deve ser feita por um método HTTP POST
        // Parametros pouco descritivos
        // Post com parametros, é menos seguro que usar o body
        app.MapGet("/create", ([FromQuery] string t, [FromQuery] string d) =>
        {
            try
            {
                // 6. Não estamos validando os itens de entrada
                tarefas.Add(new Tarefa { Title = t, Description = d });
                
                // 7. I/O - Entrada e saída sincronizada, não precisa travar uma thread
                Thread.Sleep(500); 

                return Results.Ok("Tarefa adicionada!");
            }
            // 8. Excessão genérica
            catch (System.Exception ex)
            {// 500 - um erro qualquer no servidor
                return Results.Ok($"Erro! {ex.Message}"); // 8.1 Retorna 200 no erro
            }
        });

        // 9. Método errado, deveria ser um DELETE
        // Parâmetro pouco descritivo
        app.MapGet("/remove/{i}", (int i) =>
        {
            try
            {
                tarefas.RemoveAt(i);
                return Results.Ok("Tarefa removida."); // Ok/200 ou NoContent/204
            }
            catch
            {
                // 10. Retorno errado para um erro
                return Results.Ok("Erro ao remover tarefa ou índice inválido.");
            }
        });

        // 11. Operação post com o retorno errado
        app.MapPost("/api/tarefas", ([FromBody] Tarefa novaTarefa) =>
        {
            // 12. Apenas valida o objeto todo, mas não valida os atributos
            // Deveriamos utilizar um DTO, ou uma classe de Request para receber dados
            if (novaTarefa == null)
            {
                return Results.Ok("O objeto de tarefa não pode ser nulo.");
            }
            
            tarefas.Add(novaTarefa);
            return Results.Ok(novaTarefa);
        });

        app.MapGet("/list", () =>
        {
            return Results.Ok(tarefas); // É bom usar uma classe de Response para limitar os dados expostos
        });

        app.Run();
    }
}
