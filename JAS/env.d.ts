/// <reference types="vite/client" />

interface ImportMetaEnv {
    // readonly VITE_API: string
    readonly VITE_IDCS_URL: string
    readonly VITE_CUSTOM_LOGIN_CLIENT_ID: string
    readonly VITE_CUSTOM_LOGIN_CLIENT_SECRET: string
    readonly VITE_CUSTOM_LOGIN_APPLICATION_ID: string
    readonly VITE_API_URL: string
    readonly VITE_OPENID_ENDPOINT: string
    readonly VITE_OPENIDCONNECT_TENANT_ID: string
    readonly VITE_OPENIDCONNECT_CLIENT_ID: string
    readonly VITE_OPENIDCONNECT_CLIENT_SECRET: string
    readonly VITE_OPENIDCONNECT_SCOPE: string
    readonly VITE_OPENIDCONNECT_USERNAME: string
    readonly VITE_OPENIDCONNECT_PASSWORD: string
    readonly VITE_VENDOR_CLIENT_ID: string
    readonly VITE_VENDOR_CLIENT_SECRET: string
}

interface ImportMeta {
    readonly env: ImportMetaEnv
}
