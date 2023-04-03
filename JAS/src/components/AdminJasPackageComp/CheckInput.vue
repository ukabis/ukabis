<script setup lang="ts">

const props = defineProps<{
    name: string
    value: boolean
    label?: string
}>()

const onCheck = () => {
    emits("check", props.name)
}

const emits = defineEmits<{
    (e: 'check', name: string): void
}>()

</script>

<template>
    <div class="check_box_wrap">
        <span class="checkbox">
            <input type="checkbox" :name="props.name" :value="props.value" :checked="props.value" @change="onCheck">
        </span>
        <span class="label" @click="onCheck">{{ props.label }}</span>
    </div>
</template>

<style scoped lang="scss">
.check_box_wrap {
    display: flex;
    flex-wrap: nowrap;
    justify-content: stretch;
    align-items: center;
    background: var(--white-color);

    .checkbox {
        margin-right: 14px;

        input[type=checkbox] {
            transform: scale(1.6);
            margin: 0 6px 0 2.5px;
        }
    }
    .label {
        // flex: 1;
        text-align: left;
        white-space: pre-wrap;
    }
}
</style>
