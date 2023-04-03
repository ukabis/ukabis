<!-- AdminLoginView -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseUtil } from '@/stores/util'
import { UseApi } from '@/stores/api'
import { getAccessToken } from '@/api/common/getAccessToken'
import { getOpenIdToken } from '@/api/common/getOpenIdToken'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminBlackBgBtn from '@/components/AdminCommon/AdminBlackBgBtn.vue'

// Util
const util = UseUtil()

// API
const api = UseApi()
const { accessToken, openId } = storeToRefs(api)

const clickLogin = async () => {
    // Get AccessToken
    accessToken.value = await getAccessToken()

    // Get OpenIdToken
    openId.value = await getOpenIdToken()

    // TODO: ログインの可否判断必要
    util.transition("/admin/home")
}

</script>

<template>
    <AdminHeader />
    <div class="content_bg">
        <div class="login_wrap">
            <div class="logo_wrap">
                <img src="/src/assets/Admin/Login/ukabis_top_logo.svg" alt="ukabisロゴ">
            </div>
            <div class="input_wrap">
                <form @submit.prevent>
                    <div class="input_box id">
                        <span class="label">ID</span>
                        <input type="text">
                    </div>
                    <div class="input_box password">
                        <span class="label">パスワード</span>
                        <input type="password" autocomplete="off">
                    </div>
                </form>
            </div>
            <AdminBlackBgBtn btn-label="ログイン" input-width="52.3%" @click-black-btn="clickLogin" />
            <div class="link_wrap">
                <a class="forget" href="">パスワードを忘れた方はこちらから</a>
                <a class="creat" href="">新規アカウント作成</a>
            </div>
        </div>
        <div class="footer_logo">
            <img src="/src/assets/Admin/common/footer_logo.svg" alt="@ukabis">
        </div>
    </div>
</template>

<style scoped lang="scss">
.content_bg {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 100%;
    background: var(--main-yellow-color);

    .login_wrap {
        display: flex;
        flex-direction: column;
        width: 27.7%;
        aspect-ratio: 531/741;
        min-width: 354px;
        max-width: 531px;
        max-height: 741px;
        background: var(--white-color);
        margin: 20px auto 25px;

        .logo_wrap {
            margin-top: min(5vh, 79px);

            img {
                width: 71.5%;
            }
        }

        .input_wrap {
            padding: 0px 36px;
            margin-bottom: min(4vh, 40px);
        }

        .input_box {
            display: flex;
            flex-direction: column;
            margin-bottom: 10px;

            &:last-child {
                margin-bottom: 0px;
            }

            input {
                height: 6vh;
                max-height: 60px;
                font-size: 28px;
                padding: 0px 10px;
            }

            .label {
                font-size: 16px;
                font-weight: 600;
                text-align: left;
            }
        }

        .link_wrap {
            padding: 20px 0 7vh;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            flex: 1;

            .forget {
                font-size: 14px;
                font-weight: 300;
                // margin-bottom: 53.9px;
            }

            .creat {
                font-size: 16px;
                font-weight: 600;
            }
        }
    }
}
</style>