import axios, { AxiosError } from "axios";

const app = axios.create({
    baseURL: "https://localhost:7137",
    headers: {
        'Access-Control-Allow-Origin': '*',
        'Content-Type': 'application/json',
    },
    withCredentials: true,
})

export namespace Ajax {
    export async function get<T>(url: string): Promise<{ data: T | null, error: string, loaded: boolean }> {
        let data = null;
        let error = "";
        let loaded = false;
        await app.get(url)
            .then(response => {
                data = response.data as T;
            })
            .catch((err: Error | AxiosError) => {
                if (axios.isAxiosError(err))  {
                    error = err.message;
                } else {
                  error = String(err);
                }
            })
            .finally(() => {
                loaded = true;
            });
        return { data, error, loaded };
    }

    export async function post<T>(url: string, payload: any): Promise<{ data: T | null, error: string, loaded: boolean }> {
        let data = null;
        let error = "";
        let loaded = false;
        await app.post(url, payload)
            .then(response => {
                data = response.data as T;
            })
            .catch((err: Error | AxiosError) => {
                if (axios.isAxiosError(err))  {
                    error = err.message;
                } else {
                  error = String(err);
                }
            })
            .finally(() => {
                loaded = true;
            });
        
        return { data, error, loaded };
    }
}