<script setup lang="ts">
import jsQR from 'jsqr'
import { onMounted, ref } from 'vue'
import { UseUtil } from '@/stores/util'
import { UseData } from '@/stores/data'
import { parityCheck } from '@/util/parityCheck'
import { storeToRefs } from 'pinia'

// Util
const util = UseUtil()
const { video } = storeToRefs(util)

// Data
const data = UseData()
const { shipmentQrCode } = storeToRefs(data)

// Camera
const canvas = ref<HTMLCanvasElement>()
const captureStream = ref<MediaStream | undefined>()

// カメラ準備
const constraints: MediaStreamConstraints = {
    audio: false,
    video: {
        width: 216,
        height: 216,
        facingMode: { exact: "environment" }
    }
}

// QRコード読み取り
const readQr = () => {
    if (video.value != undefined && video.value.readyState === video.value.HAVE_ENOUGH_DATA) {
        if (canvas.value == undefined) return
        const ctx = canvas.value.getContext("2d")
        canvas.value.width = video.value.videoWidth
        canvas.value.height = video.value.videoHeight
        if (ctx == null) return
        ctx.drawImage(video.value, 0, 0, canvas.value.width, canvas.value.height)
        let img = ctx.getImageData(0, 0, canvas.value.width, canvas.value.height)
        let code = jsQR(img.data, img.width, img.height, { inversionAttempts: "dontInvert" })
        if (parityCheck(code)) {
            console.log("読み取り成功")
            shipmentQrCode.value = code?.data
            util.cameraStop()
            util.transition("/shipment/idmanualinput")
        } else if (code) {
            // 読み取れたが、値が不正
            console.log("不正な値です")
            // emits("changeMsg", code.data)
            emits("changeMsg", "QRコードが正しくありません。")
        } else {
            // QRコード検出中
            emits("changeMsg", "コードをスキャンしてください")
        }
    }
    setTimeout(readQr, 250)
}

const gotStream = (stream: MediaStream) => {
    captureStream.value = stream
    readQr()
}

const handleError = (error: Error) => {
    console.error(error.name, error.message)
}

onMounted(() => {
    navigator.mediaDevices.getUserMedia(constraints)
        .then(gotStream)
        .catch(handleError)
})

const emits = defineEmits<{
    (e: "changeMsg", msg: string): void
}>()

</script>

<template>
    <div class="qr_code_reader_wrap">
        <div class="grid_line_area">
            <div class="grid_line upper_left"></div>
            <div class="grid_line upper_right"></div>
            <div class="grid_line lower_right"></div>
            <div class="grid_line lower_left"></div>
        </div>
        <div class="reader_area">
            <video ref="video" :srcObject.prop="captureStream" autoplay playsinline muted />
            <canvas ref="canvas" />
        </div>
    </div>
</template>

<style scoped lang="scss">
.qr_code_reader_wrap {
    display: flex;
    flex-direction: column;
    position: relative;

    .grid_line_area {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: calc(100% + 44px);
        height: calc(100% + 44px);

        .grid_line {
            position: absolute;

            &::before {
                content: "";
                display: inline-block;
                width: 45px;
                height: 8px;
                background: var(--white-color);
                border-radius: 0px 10px 10px 0px;
                position: absolute;
                top: 0px;
                left: 0px;
            }

            &::after {
                content: "";
                display: inline-block;
                width: 8px;
                height: 45px;
                background: var(--white-color);
                border-radius: 0px 0px 10px 10px;
                position: absolute;
                top: 0px;
                left: 0px;
            }

            &.upper_left {
                top: 0px;
                left: 0px;
            }

            &.upper_right {
                top: 0px;
                right: 0px;
                transform: scaleX(-1);
            }

            &.lower_right {
                bottom: 0px;
                right: 0px;
                transform: scale(-1);
            }

            &.lower_left {
                bottom: 0px;
                left: 0px;
                transform: scaleY(-1);
            }
        }
    }

    .reader_area {
        width: 216px;
        height: 216px;
        background: var(--white-color);
        display: flex;
        flex-direction: column;
        align-items: center;
        margin: 0px auto;
        position: relative;
    }

    canvas {
        display: none;
        width: 100%;
        height: 100%;
    }

    video {
        width: 100%;
        height: 100%;
    }
}

.msg {
    color: var(--white-color);
    font-weight: 600;
}
</style>