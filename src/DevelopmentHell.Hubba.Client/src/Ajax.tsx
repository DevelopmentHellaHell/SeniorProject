import axios from "axios";

export namespace Ajax {
    export async function get<T>(url: string): Promise<{ data: T | null, error: string, loaded: boolean }> {
        let data = null;
        let error = "";
        let loaded = false;
        await axios.get(url)
            .then(response => {
                data = response.data as T;
            })
            .catch(error => {
                const message = axios.isAxiosError(error) && error.response ?
                    error.response.data.message : String(error);
                error = message;
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
        await axios.post(url, payload)
            .then(response => {
                data = response.data as T;
            })
            .catch(error => {
                const message = axios.isAxiosError(error) && error.response ?
                    error.response.data.message : String(error);
                error = message;
            })
            .finally(() => {
                loaded = true;
            });
        
        return { data, error, loaded };
    }
}