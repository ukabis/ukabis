<!-- IncomingScanView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import ScanAreaComp from '@/components/common/ScanAreaComp.vue'

const util = UseUtil()
const { contentRef } = storeToRefs(util)

// debug
const debugOnClick = () => {
    util.transition("/incoming/idinput")
}

// Cancel
const clickCancel = () => {
    util.transition("/home")
}
</script>

<template>
    <Header title-label="個体識別番号を読む" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="spacer top"></div>
                <ScanAreaComp @click-cancel="clickCancel" @debug-on-click="debugOnClick" />
            <div class="spacer bottom"></div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.content {
    background: rgb(51 51 51 / 70%);
    display: flex;
    flex-direction: column;
    align-items: center;
    .spacer {
        width: 100%;
        min-height: 20px;
        &.top {
            flex: 0.27;
        }
        &.bottom {
            flex: 0.18;
        }
    }
}
</style>