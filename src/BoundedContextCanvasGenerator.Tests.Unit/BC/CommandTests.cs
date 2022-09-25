using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Tests.Unit.Utils;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.BC;

public class CommandTests
{
    [Fact]
    public void Creating_command_from_type_remove_command_keyword_suffix()
    {
        var command = Command.FromType(A.Class("CreateUserCommand"));

        command.FriendlyName.Should().Be("Create user");
    }
}