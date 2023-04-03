<!-- LabeledBtn -->
<script setup lang="ts">
import ClearBlackLetterBtn from '@/components/common/ClearBlackLetterBtn.vue'

const props = defineProps<{
    label: string
    text_align?: string
}>()

</script>

<template>
    <div class="labeled_btn_wrap">
        <span class="label">
            {{ props.label }}
        </span>
        <ClearBlackLetterBtn btn-label="QRコードで読みこむ" input-width="100%" />
    </div>
</template>

<style scoped lang="scss">

.labeled_btn_wrap {
    .label {
        display: flex;
        justify-content: center;
        font-weight: 600;
        margin-bottom: 20px;
    }
}
</style>