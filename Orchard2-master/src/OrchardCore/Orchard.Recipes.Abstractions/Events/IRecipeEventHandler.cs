using System.Threading.Tasks;
using Orchard.Recipes.Models;

namespace Orchard.Recipes.Events
{
    public interface IRecipeEventHandler
    {
        Task RecipeExecutingAsync(string executionId, RecipeDescriptor descriptor);
        Task RecipeStepExecutingAsync(RecipeExecutionContext context);
        Task RecipeStepExecutedAsync(RecipeExecutionContext context);
        Task RecipeExecutedAsync(string executionId, RecipeDescriptor descriptor);
        Task ExecutionFailedAsync(string executionId, RecipeDescriptor descriptor);
    }
}