<!-- ShipmentSentView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import ClearBlackLetterBtn from '@/components/common/ClearBlackLetterBtn.vue'
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'

const util = UseUtil()
const { contentRef } = storeToRefs(util)

const clickContinue = () => {
    util.transition("/shipment/registration")
}
const clickEnd = () => {
    util.transition("/home")
}

</script>

<template>
    <Header title-label="出荷情報登録" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="msg_wrap">
                送信しました
            </div>
            <div class="btn_group">
                <ClearBlackLetterBtn btn-label="続けて出荷登録する" input-width="auto" @click="clickContinue" />
                <BlackBgBtn btn-label="出荷登録終了" input-width="auto" @click="clickEnd" />
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