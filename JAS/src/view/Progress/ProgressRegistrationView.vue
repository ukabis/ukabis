<!-- ProgressRegistrationView -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseUtil } from '@/stores/util'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import InfoTableComp from '@/components/common/InfoTableComp.vue'
import sample_qr_data from '@/sample_data/sample_qr_data.json'
import LeftLabelInputBox from '@/components/common/LeftLabelInputBox.vue'
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'

const util = UseUtil()
const { contentRef } = storeToRefs(util)
const clickBlackBtn = () => {
    util.transition("/progress/sent")
}

</script>

<template>
    <Header title-label="経過登録" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <InfoTableComp :data="sample_qr_data[0]" />
            <div class="input_wrap">
                <LeftLabelInputBox label="タイトル" :is-require="true" />
                <LeftLabelInputBox label="本文" :is-require="true" :is-textarea="true" />
                <LeftLabelInputBox label="画像" :is-image="true" />
            </div>
            <div class="btn_wrap">
                <BlackBgBtn btn-label="送信する" input-width="263px" @click-black-btn="clickBlackBtn" />
            </div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">

.input_wrap {
    width: 100%;
    max-width: 430px;
    margin: 0 auto 0;
    padding: 53px 35px 0px;
    & + .btn_wrap {
        margin-top: 120px;
        width: 100%;
        max-width: 100%;
        padding-bottom: 130px;
        & > div {
            margin-bottom: 40px;
            &:last-child {
                margin-bottom: 0px;
            }
        }
    }
}


</style>