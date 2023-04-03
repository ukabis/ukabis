<!-- ScanAreaComp -->
<script setup lang="ts">
// Util
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'
import { UseUtil } from '@/stores/util'
import { isIndividualIdentificationNumber } from '@/util/isIndividualIdentificationNumber'
import { calcCheckDigit } from '@/util/calcCheckDigit'

// Component
import QrCodeReader from '@/components/common/QrCodeReader.vue'
import ClearWhiteLetterBtn from '@/components/common/ClearWhiteLetterBtn.vue'

// Util
const util = UseUtil()
const { video } = storeToRefs(util)

// Data
const useData = UseData()
const { shipmentQrCode } = storeToRefs(useData)

onMounted(() => {
    shipmentQrCode.value = "コードをスキャンしてください"
})

// Func
const changeMsg = (input_msg: string) => {
    // メッセージ表示切り替え
    shipmentQrCode.value = input_msg
}
const clickCancel = () => {
    util.cameraStop()
    emits("clickCancel")
}

const debugOnClick = () => {
    emits("debugOnClick")
}

const emits = defineEmits<{
    (e: "clickCancel"): void,
    (e: "debugOnClick"): void,
}>()

</script>

<template>
    <div class="scan_area_wrap">
        <div class="msg">
            {{ shipmentQrCode }}
        </div>
        <div class="scan_area">
            <QrCodeReader @change-msg="changeMsg" @click="debugOnClick" />
        </div>
        <ClearWhiteLetterBtn btn-label="キャンセル" input-width="auto" @click="clickCancel" />
    </div>
</template>

<style scoped lang="scss">
.scan_area_wrap {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    flex: 1;

    .msg {
        color: var(--white-color);
        font-weight: 600;
    }

    .scan_area {
        display: flex;
        place-content: center;
        margin: 40px auto;
    }
}
</style>