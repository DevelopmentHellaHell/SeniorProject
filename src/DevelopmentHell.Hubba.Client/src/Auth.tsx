import { redirect } from "react-router-dom";

export namespace Auth {
    export enum Roles {
        ADMIN_USER = "AdminUser",
        VERIFIED_USER = "VerifiedUser",
        DEFAULT_USER = "DefaultUser",
    }

    export interface IJWTDecoded {
        iss: string,
        aud: string,
        azp?: string,
        sub: number,
        iat: number,
        exp: number,
        role?: string
    }

    export function getCookie(name: string): string | undefined {
        const prefix = name + "=";
        const cookieArray = document.cookie.split(";");
        const cookie = cookieArray.find(cookie => {
            return cookie.trim().startsWith(prefix);
        });

        return cookie?.replace(prefix, "");;
    }

    export function removeCookie(name: string) {
        const cookieValue = getCookie(name); 
        if (cookieValue) {
            const prefix = name + "=";
            document.cookie = prefix + "=; Max-Age=-99999999;";  // Setting the cookie to expire which is removed upon page refresh.
        }
    }

    // Code taken from: https://stackoverflow.com/questions/38552003/how-to-decode-jwt-token-in-javascript-without-using-a-library
    // Decoding Json Web Token using base64 then parsing it as a JSON object which is then casted to a generic or unknown type.
    // A JWT consists of three parts: header, payload and signature. This just decrypts the payload using base64 decryption.
    export function parseJwt<T = unknown>(token: string) {
        var base64Url = token.split('.')[1];
        var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
    
        return JSON.parse(jsonPayload) as T;
    }
    
    export function getAccessData(): IJWTDecoded | undefined {
        const cookie = getCookie("access_token");
        if (!cookie) {
            return;
        }
    
        const decodedJwt = parseJwt<IJWTDecoded>(cookie);
        if (decodedJwt.exp * 1000 < Date.now()) {
            clearCookies();
            alert("Session expired. Please log in again.");
            redirect("/login");
            return;
        }
    
        return decodedJwt;
    }

    export function getAuthData(): IJWTDecoded | undefined {
        const cookie = getCookie("id_token");
        if (!cookie) {
            return;
        }

        return parseJwt<IJWTDecoded>(cookie);
    }

    export function clearCookies() {
        removeCookie("access_token");
        removeCookie("id_token");
    }
}
