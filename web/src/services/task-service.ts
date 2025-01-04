import { apiRequest } from "@/utils/api"

class TaskService {
    static async createTask (task: Omit<Task, 'id' | 'status'>) {
        await apiRequest<Omit<Task, 'id' | 'status'>>('api/task-items', "POST", task);
    }
}

export default TaskService