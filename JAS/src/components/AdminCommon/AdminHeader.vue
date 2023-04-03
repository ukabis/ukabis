<script setup lang="ts">
import { ref } from 'vue'
import { UseUtil } from '@/stores/util'
import { useRoute } from 'vue-router'

const isLoginView = ref<boolean>(false)

const route = useRoute()

const path = route.path
isLoginView.value = path == "/admin/login"

const props = defineProps<{
    isShowSubMenu?: boolean
    isShowBottomWrap?: boolean
    bottomWrapLabel?: string
}>()

const util = UseUtil()

const clickHome = () => {
    if(isLoginView.value) return
    util.transition("/admin/home")
}
const clickLogout = () => {
    util.transition("/admin/login")
}

</script>

<template>
    <div class="admin_header">
        <div class="admin_header_bg top">
            <div class="admin_header_wrap">
                <div class="admin_header_inner left" :class="{cursor_pointer: !isLoginView}" @click="clickHome">
                    <img src="/src/assets/Admin/common/admin_logo.svg" alt="ukabis">
                </div>
                <div class="admin_header_inner right" v-if="isShowSubMenu">
                    <span class="cursor_pointer" @click="clickHome"><img src="/src/assets/Admin/common/home_icon.svg" alt="HOME"></span>
                    <span class="cursor_pointer" @click="clickLogout"><img src="/src/assets/Admin/common/logout_icon.svg" alt="ログアウト"></span>
                </div>
            </div>
        </div>
        <div class="admin_header_bg bottom content-padding" v-if="isShowBottomWrap">
            <div class="admin_header_wrap">
                <span class="label">
                    {{ bottomWrapLabel }}
                </span>
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.admin_header {
    display: flex;
    flex-direction: column;
    width: 100%;
}

.admin_header_bg {
    width: 100%;

    &.top {
        height: 70px;
        background: var(--white-color);
    }

    &.bottom {
        height: 52px;
        background: var(--main-yellow-color);

        .admin_header_wrap {
            justify-content: flex-start;

            .label {
                font-size: 16px;
                font-weight: 600;
                color: var(--font-color);
            }
        }
    }

    .admin_header_wrap {
        width: 100%;
        height: 100%;
        max-width: var(--admin-max-width);
        margin: 0 auto;
        display: flex;
        align-items: center;
        justify-content: space-between;

        .admin_header_inner {
            &.left {
                margin-left: 39px;
            }

            &.right {
                margin-right: 52px;
                display: flex;
                align-items: center;
                justify-content: space-between;
                width: 200px;
            }
        }
    }
}
</style>