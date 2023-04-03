export const checkAuthenticated = (accessToken: string, openId: string): boolean => {
    if (
        (accessToken == "" || accessToken == undefined || accessToken == null)
        ||
        (openId == "" || openId == undefined || openId == null)
    ) {
        console.log("ログインが必要です。")
        return false
    } else {
        console.log("ログインしています。")
        return true
    }
}
