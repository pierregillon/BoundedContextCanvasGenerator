using LivingDocumentation.Domain;
using LivingDocumentation.Infrastructure;

if (!args.Any()) {
    Console.WriteLine("No solution file provided");
    return;
}

var name = new SolutionName(args[0]);

ITypeDefinitionRepository repository = new SourceCodeAnalyserTypeDefinitionRepository();

await foreach (var typeDefinition in repository.GetAll(name)) {
    if (typeDefinition.ImplementedInterfaces.Any(x => x.Value.Contains("ICommand"))) {
        Console.WriteLine($"COMMAND : {typeDefinition.Name.Value}");
    }
    if (typeDefinition.ImplementedInterfaces.Any(x => x.Value.Contains("IQuery"))) {
        Console.WriteLine($"QUERY : {typeDefinition.Name.Value}");
    }
}