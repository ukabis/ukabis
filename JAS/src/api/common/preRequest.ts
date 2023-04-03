import { getAccessToken } from "./getAccessToken"
import { getOpenIdToken } from "./getOpenIdToken"

export const preRequest = async () => {
    // Pre Request
    return {
        "access_token": await getAccessToken(),
        "openid_token": await getOpenIdToken()
    }
}
