import { axios } from '@/util/axios'

// Get OpenIdTokenObj
export const getOpenIdTokenObj = async (accessToken: string) => {
    const apiUrl = import.meta.env.VITE_OPENID_ENDPOINT
    const body = {
        client_id: import.meta.env.VITE_OPENIDCONNECT_CLIENT_ID,
        client_secret: import.meta.env.VITE_OPENIDCONNECT_CLIENT_SECRET,
        username: import.meta.env.VITE_OPENIDCONNECT_USERNAME,
        password: import.meta.env.VITE_OPENIDCONNECT_PASSWORD,
        grant_type: "password",
        scope: import.meta.env.VITE_OPENIDCONNECT_SCOPE,
    }
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }
    const openIdTokenObj = await axios.post(apiUrl, body, options)
        .then(res => {
            return res
        })
        .catch(err => console.error({ err }))
    return openIdTokenObj
}

// Get OpenIdToken
export const getOpenIdToken = async () => {
    const apiUrl = import.meta.env.VITE_OPENID_ENDPOINT
    const body = {
        client_id: import.meta.env.VITE_OPENIDCONNECT_CLIENT_ID,
        client_secret: import.meta.env.VITE_OPENIDCONNECT_CLIENT_SECRET,
        username: import.meta.env.VITE_OPENIDCONNECT_USERNAME,
        password: import.meta.env.VITE_OPENIDCONNECT_PASSWORD,
        grant_type: "password",
        scope: import.meta.env.VITE_OPENIDCONNECT_SCOPE,
    }
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }
    const openIdTokenObj = await axios.post(apiUrl, body, options)
        .then(res => {
            return res
        })
        .catch(err => console.error({ err }))
    const openIdToken = openIdTokenObj?.data.access_token
    return openIdToken
}
