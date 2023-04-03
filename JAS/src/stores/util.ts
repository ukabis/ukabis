import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

export const UseUtil = defineStore('util', {
    state: () => ({
        router: useRouter(),
        displayNum: 10,
        pageMaxNum: 99,
        currentPageNum: 1,
        contentRef: ref(),
        video: ref<HTMLVideoElement>(),
        newRegistRowNum: {
            distributionResponsible: 1,
            distributionChargeList: 1,
            ratingResponsible: 1,
            ratingChargeList: 1,
        },
        isFade: false
    }),
    getters: {},
    actions: {
        transition(pathName: string, query?: string) {
            this.router.push(pathName)
        },
        cameraStop() {
            if (!this.video) return
            const stream = this.video.srcObject as MediaStream
            const tracks = stream.getTracks()
            tracks.forEach(function (track) {
                track.stop()
            })
            this.video.srcObject = null
        },
        errorHandling(err: any) {
            alert(`エラーが発生しました。タイトル画面に戻ります: ${err}`)
            this.transition("/")
            this.isFade = false
        },
    }
})
