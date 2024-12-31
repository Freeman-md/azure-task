import { JSX } from "react";

type TaskItemProps = {
  title: string;
  description: string;
};

export default function TaskItem({
  title,
  description,
}: TaskItemProps): JSX.Element {
  return (
    <div className="task-card transition-transform duration-300 space-y-6">
      <div className="flex justify-between space-x-2 items-center">
      <h3 className="text-lg font-medium">{title}</h3>
      </div>
      <div className="space-y-4">
        <p className="text-sm text-primary-500 font-light">{description}</p>
      </div>
    </div>
  );
}
