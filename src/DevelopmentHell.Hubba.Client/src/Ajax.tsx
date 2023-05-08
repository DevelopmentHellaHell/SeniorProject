import axios, { AxiosError } from "axios";

const app = axios.create({
    baseURL: "https://localhost:7137",
    //baseURL: "https://hubbaswag.com",
    headers: {
        'Access-Control-Allow-Origin': '*',
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});

export namespace Ajax {
    export async function get<T>(url: string): Promise<{ data: T | null, error: string, loaded: boolean, status: number }> {
        let data = null;
        let error = "";
        let loaded = false;
        let status = 500;
        await app.get(url)
            .then(response => {
                data = response.data as T;
                status = response.status;
            })
            .catch((err: Error | AxiosError) => {
                if (axios.isAxiosError(err))  {
                    if (err.response?.data) {
                        error = err.response.data;
                    } else {
                        error = err.message;
                    }
                } else {
                  error = String(err);
                }
            })
            .finally(() => {
                loaded = true;
            });
        return { data, error, loaded, status };
    }

    export async function post<T>(url: string, payload: any): Promise<{ data: T | null, error: string, loaded: boolean, status: number }> {
        let data = null;
        let error = "";
        let loaded = false;
        let status = 500;
        await app.post(url, payload)
            .then(response => {
                data = response.data as T;
                status = response.status;
            })
            .catch((err: Error | AxiosError) => {
                if (axios.isAxiosError(err))  {
                    if (err.response?.data) {
                        error = err.response.data;
                    } else {
                        error = err.message;
                    }
                } else {
                  error = String(err);
                }
            })
            .finally(() => {
                loaded = true;
            });
        
        return { data, error, loaded, status };
    }

    export async function postFormData<T>(url: string, payload: any): Promise<{ data: T | null, error: string, loaded: boolean, status: number }> {
        let data = null;
        let error = "";
        let loaded = false;
        let status = 500;
    
        const formData = payload;
    
        await app.post(url, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
        .then(response => {
            data = response.data as T;
            status = response.status;
        })
        .catch((err: Error | AxiosError) => {
            if (axios.isAxiosError(err))  {
                if (err.response?.data) {
                    error = err.response.data;
                } else {
                    error = err.message;
                }
            } else {
              error = String(err);
            }
        })
        .finally(() => {
            loaded = true;
        });
    
        return { data, error, loaded, status };
    }
    
}