<!-- TextInputWrap -->
<script setup lang="ts">

// Component
import AdminInput from './AdminInput.vue'

const props = defineProps<{
    input_mode: string
    label?: string
    name: string
    isReadOnly?: boolean
    value?: string | number
}>()


const receiveValue = (value: string) => {
    emits("receiveValue", value, props.name)
}

const emits = defineEmits<{
    (e: "receiveValue", value: string, name?: string): void
}>()

</script>

<template>
    <div class="text_input_wrap" v-if="input_mode=='text'">
        <div class="label" v-if="label">
            {{ label }}
        </div>
        <div class="input_wrap">
            <AdminInput :name="name" :value="value" input_mode="text" @emit-value="receiveValue" :is-read-only="isReadOnly" />
        </div>
    </div>
    <div class="textarea_input_wrap" v-if="input_mode=='textarea'">
        <div class="label" v-if="label">
            {{ label }}
        </div>
        <div class="input_wrap">
            <AdminInput :name="name" :value="value" input_mode="textarea" @emit-value="receiveValue" :is-read-only="isReadOnly" />
        </div>
    </div>
</template>

<style scoped lang="scss">
.text_input_wrap {
    width: 100%;
    max-width: 880px;
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    justify-content: space-between;

    .input_wrap {
        width: 100%;
        max-width: 375px;
        text-align: right;
    }
}
</style>