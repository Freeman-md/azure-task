import { IconUpload } from "@tabler/icons-react";
import { createRef } from "react";

export default function DragAndDrop({
  onFilesAdd
}: {
  onFilesAdd: (files: File[]) => void;
}) {
  const fileInput = createRef<HTMLInputElement>();

  const toggleFilesUpload = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    fileInput.current?.click();
  };

  const handleFilesUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files) {
      const newFiles = Array.from(event.target.files);
      onFilesAdd(newFiles);
    }
  };

  const handleFilesDrop = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    if (event.dataTransfer.files) {
      const newFiles = Array.from(event.dataTransfer.files);
      onFilesAdd(newFiles);
    }
  };

  return (
    <div
      
      onDragOver={(event) => event.preventDefault()}
      onDrop={handleFilesDrop}
    >
      <div className="flex flex-col items-center justify-center space-y-1">
        <button
          type="button"
          className="bg-gray-100 rounded p-2 text-gray-500 transition hover:text-primary"
          onClick={toggleFilesUpload}
        >
          <IconUpload size={20} />
        </button>
        <p className="text-center text-sm">
          Drag & Drop to upload files or &nbsp;
          <button
            className="text-primary hover:underline"
            onClick={toggleFilesUpload}
          >
            browse files
          </button>
        </p>

        <input
          type="file"
          ref={fileInput}
          hidden
          multiple
          onChange={handleFilesUpload}
        />
      </div>
    </div>
  );
}
