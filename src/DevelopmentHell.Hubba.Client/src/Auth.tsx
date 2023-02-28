import Cookies from "js-cookie";
import jwtDecode from "jwt-decode";

export namespace Auth {
    export enum Roles {
        ADMIN_USER = "AdminUser",
        VERIFIED_USER = "VerifiedUser",
    }

    export interface IJWTDecoded {
        accountId: number,
        role: Roles,
        email: string,
        nbf: number,
        exp: number,
        iat: number,
    }
    
    export function isAuthenticated(): IJWTDecoded | undefined {
        const cookie = Cookies.get("access_token");
        if (!cookie) {
            return undefined;
        }
    
        const decodedJwt = jwtDecode<IJWTDecoded>(cookie);
        if (decodedJwt.exp * 1000 < Date.now()) {
            Cookies.remove("access_token");
            return undefined;
        }
    
        return decodedJwt;
    }
}
