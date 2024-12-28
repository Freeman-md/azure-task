import Menu from "@/features/home/Menu";
import TaskItem from "@/features/home/TaskItem";

export default function Home() {
  const tasks: Task[] = [
    {
      id: "1",
      title: "Create a new project",
      description: "Create a new project using the Azure CLI",
      status: "pending",
    },
    {
      id: "2",
      title: "Deploy a new project",
      description: "Deploy a new project using the Azure CLI",
      status: "in progress",
    },
    {
      id: "3",
      title: "Create a new project",
      description: "Create a new project using the Azure CLI",
      status: "completed",
    },
    {
      id: "4",
      title: "Deploy a new project",
      description: "Deploy a new project using the Azure CLI",
      status: "completed",
    },
  ];

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
