<!-- LogoutView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'

const util = UseUtil()
const { contentRef } = storeToRefs(util)

const clickBtn = () => {
    util.transition("/home")
}

</script>

<template>
    <Header title-label="JAS関連出荷情報入力" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="msg_wrap">
                ログアウトしました
            </div>
            <div class="btn_group">
                <BlackBgBtn btn-label="再ログイン" input-width="auto" @click="clickBtn" />
            </div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.content {
    display: flex;
    flex-direction: column;
    justify-content: stretch;

    .msg_wrap {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        flex: 1;
        font-size: 16px;
        font-weight: 600;
        color: var(--font-color);
    }

    .btn_group {
        margin: 0px auto 79px;

        &>div:first-child {
            margin-bottom: 26px;
        }
    }
}
</style>