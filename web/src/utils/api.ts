const API_URL = process.env.NEXT_PUBLIC_API_URL || '';

export const apiRequest = async <T>(
    endpoint: string,
    method: 'GET' | 'POST' | 'PUT' | 'DELETE',
    body?: T,
    headers: Record<string, string> = {}
): Promise<ApiResponse<T>> => {
    try {
        console.log(API_URL);

        const response = await fetch(`${API_URL}/${endpoint}`, {
            method,
            headers: {
                'Content-Type': 'application/json',
                ...headers,
            },
            body: body ? JSON.stringify(body) : undefined,
        });

        const responseData = await response.json();

        if (!response.ok) {
            throw new Error(
                responseData.error?.message || `API request failed with status ${response.status}`
            );
        }

        return {
            success: response.ok,
            statusCode: response.status,
            payload: responseData.payload,
            error: responseData.error || undefined,
        };
    } catch (err) {
        throw new Error((err as Error).message || 'Unexpected error occurred');
    }
};
