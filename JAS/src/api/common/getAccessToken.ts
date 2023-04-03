import { axios } from '@/util/axios'

// Get AccessTokenObj
export const getAccessTokenObj = async () => {
    const apiPath = "/Token"
    const body = {
        grant_type: "client_credentials",
        client_id: import.meta.env.VITE_VENDOR_CLIENT_ID,
        client_secret: import.meta.env.VITE_VENDOR_CLIENT_SECRET
    }
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }
    const accessTokenObj = await axios.post(apiPath, body, options)
        .then(res => {
            return res
        })
        .catch(err => console.error({ err }))
    return accessTokenObj
}

// Get AccessToken
export const getAccessToken = async () => {
    const apiPath = "/Token"
    const body = {
        grant_type: "client_credentials",
        client_id: import.meta.env.VITE_VENDOR_CLIENT_ID,
        client_secret: import.meta.env.VITE_VENDOR_CLIENT_SECRET
    }
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    }
    const accessToken = await axios.post(apiPath, body, options)
        .then(res => {
            return res.data.access_token
        })
        .catch(err => console.error({ err }))
    return accessToken
}
