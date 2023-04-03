<script setup lang="ts">
// Util
import { onMounted, ref } from 'vue'

// Ref
const isActive = ref<boolean>(false)

// Func
const getIsActive = (): boolean => {
    console.log("run getIsActive")
    // getIsMobileがtrueかつ、getIsAdminPageがtrueのとき、trueを返す
    return (getIsMobile() && getIsAdminPage())
}
const getIsMobile = (): boolean => {
    console.log("run getIsMobile")
    // ユーザーエージェントがモバイルかどうか
    if (navigator.userAgent.match(/iPhone|Android.+Mobile/)) {
        return true
    } else {
        return false
    }
}
const getIsAdminPage = (): boolean => {
    console.log("run getIsMobile")
    // 閲覧しているページのURLに「admin」が含まれているかどうか
    const path = location.pathname
    if (path.match(/admin/)) {
        return true
    } else {
        return false
    }
}

onMounted(() => {
    isActive.value = getIsActive()
})

</script>

<template>
    <div v-if="isActive" class="for_mobile_wrap">
        <div class="inner">
            <span class="msg">
                PCからアクセスしてください
            </span>
        </div>
    </div>
</template>

<style scoped lang="scss">
.for_mobile_wrap {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
    position: fixed;
    top: 0px;
    background: #fff;
    z-index: 10000;

    .inner {
        font-size: 24px;
        font-weight: bold;
    }
}
</style>