<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

const morningMaterial = adminJasPackageData.value.morning_material

</script>

<template>
    <div class="picked_morning_wrap">
        <span>適正な使用緩衝材</span>
        <span>{{ morningMaterial }}</span>
</div>
</template>

<style scoped lang="scss">
.picked_morning_wrap {
    display: flex;
    justify-content: space-between;
    margin: 31px 22px 0px;
    width: 68%;
    max-width: 808px;
}
</style>