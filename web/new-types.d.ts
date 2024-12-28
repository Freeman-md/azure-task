type Status = "pending" | "in progress" | "completed";

type Task = {
    id: string;
    title: string;
    description: string;
    status: Status;
}

type ApiResponse<T> = {
    success: boolean;
    statusCode: number;
    payload: Array<T>,
    error: object
}