<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'

// Util
const router = useRouter()
const util = UseUtil()
const { contentRef } = storeToRefs(util)

const back = () => {
    router.back()
}
const home = () => {
    util.transition("/home")
}
const top = () => {
    contentRef.value.scrollTop = 0
}

</script>

<template>
    <div class="footer_wrap">
        <div class="footer_bg">
            <div class="footer inner">
                <ul>
                    <li @click="back">
                        <img src="/src/assets/common/back_icon.svg" alt="戻る">
                    </li>
                    <li @click="home">
                        <img src="/src/assets/common/home_icon.svg" alt="ホーム">
                    </li>
                    <li @click="top">
                        <img src="/src/assets/common/to_page_top_icon.svg" alt="ページトップ">
                    </li>
                </ul>
            </div>
        </div>
    </div>
</template>

<style scoped>
.footer_wrap {
    width: 100%;
    position: fixed;
    bottom: 0px;
    background: var(--white-color);
}
.footer_bg {
    display: flex;
    width: 100%;
    height: 103px;
    border-top: 1px solid #B9B9B9;
}
.footer.inner {
    width: 100%;
}
.footer.inner ul {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-top: 15px;
    padding: 0px 30px;
}
.footer.inner ul li {
    display: flex;
    justify-content: center;
    min-width: 75px;
}
</style>