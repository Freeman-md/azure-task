import Menu from "@/features/home/Menu";
import TaskItem from "@/features/home/TaskItem";

export default async function Home() {
  const data = await fetch("https://azuretaskapi.azurewebsites.net/api/task-items", { cache: 'force-cache' });
  const response : ApiResponse<Task> = await data.json();

  const tasks: Task[] = response.payload;

  const boards: Record<string, Task[]> = tasks.reduce(
    (acc: Record<string, Task[]>, task: Task) => {
      if (!acc[task.status]) {
        acc[task.status] = [];
      }

      acc[task.status].push(task);

      return acc;
    },
    {} as Record<string, Task[]>
  );

  return (
    <>
      <Menu />

      <section role="board" className="mt-8">
        <div className="flex space-x-6 overflow-x-auto">
          {Object.entries(boards).map(([status, tasks]) => (
            <div
              key={status}
              className=" min-w-96 bg-primary-100/30 p-6 rounded-lg shadow-sm border transition-transform duration-300 space-y-6"
            >
              <div className="flex justify-between space-x-2 items-center">
                <h2 className="uppercase">{status}</h2>
              </div>
              <div className="space-y-4">
              {tasks.map((task) => (
                <TaskItem key={task.id} title={task.title} description={task.description} />
              ))}
              </div>
            </div>
          ))}
        </div>
      </section>
    </>
  );
}
