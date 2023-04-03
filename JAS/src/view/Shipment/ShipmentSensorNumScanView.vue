<!-- ShipmentSensorNumScanView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import ScanAreaComp from '@/components/common/ScanAreaComp.vue'

// Util
const util = UseUtil()
const { contentRef } = storeToRefs(util)

// Click Cancel Btn
const clickCancel = () => {
    util.transition("/shipment/confirm")
}

</script>

<template>
    <Header title-label="センサー番号を読む" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="spacer top"></div>
                <ScanAreaComp @click-cancel="clickCancel" />
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