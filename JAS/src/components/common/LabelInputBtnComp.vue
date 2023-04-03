<script setup lang="ts">
import BlackBgBtn from './BlackBgBtn.vue'
import Notice from './Notice.vue'

const props = defineProps<{
    word: string
    isOperation: boolean
    value?: string
}>()

const onClick = () => {
    emits("clickBtn")
}

const emits = defineEmits<{
    (e: "clickBtn", value?: string): void
}>()

</script>

<template>
    <div class="label_input_btn_wrap">
        <div class="label_wrap">
            {{ props.word }}
        </div>
        <div class="input_wrap">
            <input type="text" :value="props.value" placeholder="0239128471823">
        </div>
        <div class="btn_wrap">
            <BlackBgBtn v-if="isOperation" btn-label="入力完了" input-width="263px" @click="onClick" />
            <Notice v-if="!isOperation" word="操作権限がありません" />
        </div>
    </div>
</template>

<style scoped lang="scss">
.label_input_btn_wrap {
    margin-bottom: 40px;

    .label_wrap {
        display: flex;
        justify-content: center;
        font-weight: 600;
        margin-bottom: 20px;
    }

    .input_wrap {
        input {
            width: 100%;
            max-width: 360px;
            height: 51px;
            border-radius: 7px;
            border: 1px solid #707070;
            padding: 0px 26px;
            font-size: 16px;
            font-weight: 300;
        }

        margin-bottom: 5vh;
    }

    .btn_wrap {
        min-height: 70px;
    }
}
</style>