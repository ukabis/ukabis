<!-- LoginView -->
<script setup lang="ts">

// Util
import { UseUtil } from '@/stores/util'
// import { getAccessTokenObj } from '@/api/common/getAccessToken'

// Component
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'
import BottomDesign from '@/components/LoginViewComp/BottomDesign.vue'

// Fetch

// Util
const util = UseUtil()
const onClick = () => {
    // getAccessTokenObj()
    util.transition("/home")
}

// Debug
const clickToAdmin = () => {
    util.transition("/admin/login")
}

</script>

<template>
    <div class="login_wrap">
        <div class="inner">
            <div class="logo_wrap">
                <img src="/src/assets/LoginView/ukabis_logo.svg" alt="ukabisロゴ">
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
            <BlackBgBtn btn-label="ログイン" input-width="263" @click="onClick" />
            <div class="link_wrap">
                <a class="forget" href="">パスワードを忘れた方はこちらから</a>
                <a class="create" href="">新規アカウント作成</a>
            </div>
        </div>
    </div>
    <BottomDesign />
</template>

<style scoped lang="scss">

.admin {
    // for debug
    cursor: pointer;
    text-decoration: underline;
    font-weight: bold;
}

.login_wrap {
    height: 100%;
    height: 100vh;
    height: 100svh;
    height: 100dvh;

    .inner {
        // border: 10px solid blue;
        display: flex;
        flex-direction: column;
        place-content: stretch;
        height: 90%;
        max-width: 430px;

        &>div {
            padding-top: 10px;
            padding-bottom: 10px;
        }
    }
}

.logo_wrap {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    flex: 1;
    max-height: 414px;

    img {
        position: relative;
        top: 10%;
        left: -6px;
        max-width: 310px;
        max-height: 136px;
    }
}

.input_wrap {
    padding: 0px 36px;
}

.input_box {
    display: flex;
    flex-direction: column;
    margin-bottom: 10px;

    input {
        height: 58px;
    }

    .label {
        font-size: 12px;
        text-align: left;
    }
}

.link_wrap {
    display: flex;
    flex-direction: column;
    padding-top: 10px;

    .forget {
        font-size: 14px;
        font-weight: 300;
        margin-bottom: 50px;
    }

    .create {
        font-size: 16px;
        font-weight: 600;
    }
}
</style>