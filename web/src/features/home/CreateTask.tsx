"use client";

import createTask from "@/actions/tasks/create-task";
import DragAndDrop from "@/components/ui/DragAndDrop";
import { useFileManager } from "@/hooks/useFileManager";
import { IconPlus, IconX } from "@tabler/icons-react";
import { useState } from "react";

export default function CreateTask() {
  const [isFormVisible, setIsFormVisible] = useState<boolean>(false);
  const { files, addFiles, removeFile } = useFileManager();

  const showTaskForm = () => setIsFormVisible(() => !isFormVisible);

  const handleKeyDown = (event: React.KeyboardEvent<HTMLFormElement>) => {
    if (event.key === "Enter" && event.target instanceof HTMLInputElement) {
      console.log("Form submitted via Enter key");
    }
  };

  ;

  return (
    <section role="create-task" className="space-y-4">
      {isFormVisible && (
        <form
          className="task-card space-y-2"
          action={createTask}
          onKeyDown={handleKeyDown}
        >
          <input
            type="text"
            className="block transparent-input text-xl"
            placeholder="Title"
            name="title"
          />

          <textarea
            className="block transparent-input text-base resize-none"
            placeholder="Description"
            rows={2}
            name="description"
          />

          <section
            role="drag-and-drop"
            className="border border-dashed border-gray-300 p-4 rounded-md space-y-2"
          >
            <DragAndDrop onFilesAdd={addFiles} />

            {files.length > 0 &&
              files.map((file) => {
                return (
                  <div
                    key={file.name}
                    className="flex items-center justify-between space-x-4 border p-2 border-gray-200 rounded-xl"
                    onClick={() => removeFile(file)}
                  >
                    <p>{file.name}</p>
                    <button
                      type="button"
                      className="text-red-500 flex-shrink-0 rounded-full border border-transparent hover:border-red-500"
                    >
                      <IconX size={16} />
                    </button>
                  </div>
                );
              })}
          </section>

          <button className="btn btn-outline" type="submit">
            Create
          </button>
        </form>
      )}

      <button
        onClick={showTaskForm}
        className="flex space-x-2 items-center text-gray-500 transition hover:text-primary"
      >
        <IconPlus width={20} />
        <span>Create Task</span>
      </button>
    </section>
  );
}
