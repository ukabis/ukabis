<script setup lang="ts">
// Util
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Util
const util = UseUtil()
const { isFade } = storeToRefs(util)

</script>

<template>
    <div v-if="isFade" class="fade_wrap">
        <div class="fade_inner">
            読み込み中...
        </div>
    </div>
</template>

<style scoped lang="scss">
.fade_wrap {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    position: fixed;
    top: 0px;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.7);
    z-index: 10001;
    .fade_inner {
        color: #fff;
        font-weight: bold;
        font-size: 28px;
    }
}
</style>