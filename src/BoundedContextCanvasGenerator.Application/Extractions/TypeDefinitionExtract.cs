﻿using BoundedContextCanvasGenerator.Domain.BC;

namespace BoundedContextCanvasGenerator.Application.Extractions;

public record TypeDefinitionExtract(ExtractedElements Commands, ExtractedElements DomainEvents, ExtractedElements Aggregates);