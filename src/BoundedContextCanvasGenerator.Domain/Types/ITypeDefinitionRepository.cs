﻿using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.Types;

public interface ITypeDefinitionRepository
{
    Task<IReadOnlyCollection<TypeDefinition>> GetAll(SolutionPath path);
}