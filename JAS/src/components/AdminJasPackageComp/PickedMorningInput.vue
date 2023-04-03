<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Component
import TextInputWrap from '@/components/AdminCommon/TextInputWrap.vue'

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

const props = defineProps<{
    isGrayout?: boolean
}>()

const receiveValue = (value: string, name?: string) => {
    adminJasPackageData.value.morning_material = value
}

</script>

<template>
    <div class="picked_morning_wrap" :class="{grayout: isGrayout}">
        <TextInputWrap input_mode="text" label="適正な使用緩衝材" name="morning_material" @receive-value="receiveValue" :is-read-only="!adminJasPackageData.picked_morning" />
    </div>
</template>

<style scoped lang="scss">
.picked_morning_wrap {
    margin: 31px 0px 0px 22px;
}
</style>