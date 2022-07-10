var name = new SolutionName(@"C:\Users\gillo\Code\crypto-roi\api\CryptoROI.sln");

ITypeDefinitionRepository repository = new SourceCodeAnalyserTypeDefinitionRepository();

var iCommand = new TypeFullName("ICommand");
var iQuery = new TypeFullName("IQuery");


await foreach (var typeDefinition in repository.GetAll(name)) {
    if (typeDefinition.ImplementedInterfaces.Contains(iCommand)) {
        Console.WriteLine($"COMMAND : {typeDefinition.Name.Value}");
    }
    if (typeDefinition.ImplementedInterfaces.Contains(iQuery)) {
        Console.WriteLine($"QUERY : {typeDefinition.Name.Value}");
    }
}