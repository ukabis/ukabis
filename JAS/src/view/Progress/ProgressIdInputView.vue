<!-- ProgressIdInputView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import LabelInputBtnComp from '@/components/common/LabelInputBtnComp.vue'
import LabeledBtn from '@/components/common/LabeledBtn.vue'

const util = UseUtil()
const { contentRef } = storeToRefs(util)

const clickBtn = () => {
    util.transition("/progress/registration")
}

const reloading = () => {
    util.transition("/progress/scan")
}

</script>

<template>
    <Header title-label="入荷情報登録" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="spacer top"></div>
            <div class="content_group">
                <LabelInputBtnComp word="個体識別番号を入力してください" @click-btn="clickBtn" :is-operation="true" />
                <div class="labeled_btn_wrap">
                    <LabeledBtn label="個体識別番号を再読込する" text_align="center" @click="reloading" />
                </div>
            </div>
            <div class="spacer bottom"></div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.spacer {
    min-height: 20px;

    &.top {
        flex: 0.27;
    }

    &.bottom {
        flex: 0.17;
    }
}

.content {
    display: flex;
    flex-direction: column;
    .content_group {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        flex: 1;
        .labeled_btn_wrap {
            margin: 0 auto;
            width: 100%;
            max-width: 263px;
        }
    }
}
</style>