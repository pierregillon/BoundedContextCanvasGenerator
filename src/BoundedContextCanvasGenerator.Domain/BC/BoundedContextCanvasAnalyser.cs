using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC;

public class BoundedContextCanvasAnalyser
{
    public async Task<BoundedContextCanvas> Analyse(TypeDefinitionExtract typeDefinitionExtract, ICanvasSettings canvasSettings) =>
        new(
            canvasSettings.Name,
            canvasSettings.Definition,
            GenerateUbiquitousLanguage(typeDefinitionExtract),
            GenerateInboundCommunication(typeDefinitionExtract, canvasSettings)
        );

    private static UbiquitousLanguage GenerateUbiquitousLanguage(TypeDefinitionExtract typeDefinitionExtract)
        => typeDefinitionExtract
            .Aggregates
            .Values
            .Select(CoreConcept.FromTypeDefinition)
            .ToArray()
            .Pipe(UbiquitousLanguage.FromConcepts);

    private static InboundCommunication GenerateInboundCommunication(TypeDefinitionExtract typeDefinitionExtract, ICanvasSettings canvasSettings)
    {
        var commandsGroupedByModule = typeDefinitionExtract.Commands.Values
            .Select(x => new CommandWrapper(x))
            .GroupBy(x => x.ModuleName)
            .ToArray();

        var modules = GenerateDomainModules(commandsGroupedByModule, canvasSettings.InboundCommunicationSettings);

        return new InboundCommunication(modules);
    }

    private static IEnumerable<DomainModule> GenerateDomainModules(IEnumerable<IGrouping<Namespace, CommandWrapper>> commandsGroupedByModule, InboundCommunicationSettings inboundCommunicationSettings)
        => commandsGroupedByModule.Select(commandsFromSameModule =>
            new DomainModule(
                commandsFromSameModule.Key.Name.ToReadableSentence(),
                GetDomainFlows(commandsFromSameModule, inboundCommunicationSettings)
            )
        );

    private static IEnumerable<DomainFlow> GetDomainFlows(IEnumerable<CommandWrapper> commands, InboundCommunicationSettings inboundCommunicationSettings)
        => commands.Select(command => command.BuildDomainFlow(inboundCommunicationSettings));

    private record CommandWrapper(TypeDefinition TypeDefinition)
    {
        private const string COMMAND_SUFFIX = "Command";

        private Namespace ParentNamespace { get; } = TypeDefinition.FullName.Namespace;
        public Namespace ModuleName => ParentNamespace.TrimStart(TypeDefinition.AssemblyDefinition.Namespace);

        public DomainFlow BuildDomainFlow(InboundCommunicationSettings inboundCommunicationSettings)
            => new(
                GetCollaborators(inboundCommunicationSettings.CollaboratorDefinitions),
                BuildCommand(),
                GetPolicies(inboundCommunicationSettings.PolicyDefinitions)
            );

        private IEnumerable<Collaborator> GetCollaborators(IEnumerable<CollaboratorDefinition> collaboratorDefinitions)
            => TypeDefinition.Instanciators
                .SelectMany(i => i.FilterCollaboratorDefinitionsMatching(collaboratorDefinitions))
                .Select(Collaborator.FromCollaboratorDefinition)
                .ToArray();

        private Command BuildCommand()
            => new(
                TypeDefinition.FullName.Name.TrimWord(COMMAND_SUFFIX).ToReadableSentence(),
                TypeDefinition.FullName
            );

        private IEnumerable<Policy> GetPolicies(IEnumerable<PolicyDefinition> policyDefinitions)
            => TypeDefinition.Instanciators
                .SelectMany(i => i.FilterMethodsMatching(policyDefinitions))
                .Select(Policy.FromMethod)
                .ToArray();
    }
}