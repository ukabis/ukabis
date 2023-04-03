import { ref } from 'vue'

// カメラ停止
export const cameraStop = (video: HTMLVideoElement | undefined) => {
    if (!video) return
    console.log("カメラを停止します。")
    const stream = video.srcObject as MediaStream
    const tracks = stream.getTracks()
    tracks.forEach(function (track) {
        track.stop()
    })
    video.srcObject = null
}
