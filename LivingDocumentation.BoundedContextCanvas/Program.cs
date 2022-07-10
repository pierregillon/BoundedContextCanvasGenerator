using LivingDocumentation.BoundedContextCanvas.Domain;
using LivingDocumentation.BoundedContextCanvas.Infrastructure;

var name = new SolutionName(@"C:\Users\gillo\Code\crypto-roi\api\CryptoROI.sln");

ITypeDefinitionRepository repository = new SourceCodeAnalyserTypeDefinitionRepository();

var iCommand = new TypeFullName("CryptoROI.CQRS.ICommand");
var iQuery = new TypeFullName("CryptoROI.CQRS.IQuery");


await foreach (var typeDefinition in repository.GetAll(name)) {
    if (typeDefinition.ImplementedInterfaces.Any(x => x.Contains(iCommand))) {
        Console.WriteLine($"COMMAND : {typeDefinition.Name.Value}");
    }
    if (typeDefinition.ImplementedInterfaces.Any(x => x.Contains(iQuery))) {
        Console.WriteLine($"QUERY : {typeDefinition.Name.Value}");
    }
}