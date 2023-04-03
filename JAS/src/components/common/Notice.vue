<script setup lang="ts">

const props = defineProps<{
    word: string
}>()

</script>

<template>
    <div class="notice_wrap">
        <div class="notice">
            {{ word }}
        </div>
    </div>
</template>

<style scoped lang="scss">
.notice_wrap {
    .notice {
        font-size: 16px;
        font-weight: 600;
        color: var(--notice-font-color);
    }
}
</style>