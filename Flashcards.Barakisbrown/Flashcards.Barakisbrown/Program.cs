
using Flashcards.Barakisbrown.Data;

var config = Configuration.LoadSettings();

Console.WriteLine($"The SQL for InitDB => {config.InitDbScript}");
Console.WriteLine($"Connection String => {Configuration.GetConnectionString()}");