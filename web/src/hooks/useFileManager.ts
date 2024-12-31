import { useState } from "react";

export function useFileManager() {
  const [files, setFiles] = useState<File[]>([]);

  const addUniqueFiles = (newFiles: File[], prevFiles: File[]): File[] => {
    return [
      ...prevFiles,
      ...newFiles.filter(
        (newFile) =>
          !prevFiles.some(
            (existingFile) =>
              existingFile.name === newFile.name &&
              existingFile.size === newFile.size
          )
      ),
    ];
  };

  const addFiles = (newFiles: File[]) => {
    setFiles((prevFiles) => addUniqueFiles(newFiles, prevFiles));
  };

  const removeFile = (file: File) => {
    setFiles((prevFiles) => prevFiles.filter((f) => f.name !== file.name));
  };

  const clearFiles = () => {
    setFiles([]);
  };

  return {
    files,
    addFiles,
    removeFile,
    clearFiles,
  };
}
