using Catalog.Application;

namespace Catalog.Web;

public interface ICommandDispatcher
{
    Task Dispatch(ICommand command);
}