type Status = "pending" | "in progress" | "completed";

type Task = {
    id: string;
    title: string;
    description: string;
    status: Status;
}