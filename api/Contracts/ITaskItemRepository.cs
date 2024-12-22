using System.Threading.Tasks;
using api.Models;


namespace api.Contracts;

public interface ITaskItemRepository {

    public Task<TaskItem?> GetOne(int id);
    public Task<IReadOnlyList<TaskItem>> GetAll();
    public Task<TaskItem> Create(TaskItem taskItem);
    public Task<TaskItem> Update(int id, Dictionary<string, object> updates);
    public Task Delete(int id);

}