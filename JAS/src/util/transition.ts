import { useRouter } from "vue-router"

const router = useRouter()
export const transition = (routerFunc: () => void) => {
    routerFunc()
}