<!-- LeftLabelInputBox -->
<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'

// Props
const props = defineProps<{
    label: string
    placeholder?: string
    isRequire?: boolean
    isImage?: boolean
    isTextarea?: boolean
    isReadOnly?: boolean
    value?: string
    isNumber?: boolean
}>()

// Func
const onChange = (e: any) => {
    const value = e.target.value
    if (isNaN(value)) {
        emits("emitsValue", value)
    } else {
        emits("emitsNumber", Number(value))
    }
}

// Ref
const imagePreviews = ref<string[]>([])

const uploadFile = (event: Event) => {
    const files = (event.target as HTMLInputElement).files
    if (!files) {
        return
    }

    for (let i = 0; i < files.length; i++) {
        const reader = new FileReader()
        reader.readAsDataURL(files[i])
        reader.onload = () => {
            const dataUrl = reader.result as string
            imagePreviews.value.push(dataUrl)
        }
    }
    emits("emitsFiles", imagePreviews.value)
}

// Emits
const emits = defineEmits<{
    (e: "emitsValue", value: string): void
    (e: "emitsNumber", num: number): void
    (e: "emitsFiles", files: string[]): void
}>()

</script>

<template>
    <div class="label_input_btn_wrap">
        <div class="label_wrap">
            {{ props.label }}
            <span v-if="props.isRequire" class="require_label">
                ※必須
            </span>
        </div>
        <div class="input_wrap">
            <input v-if="!props.isImage && !props.isTextarea" :class="{ readonly: props.isReadOnly }"
                :type="props.isNumber ? 'number' : 'text'" :placeholder="props.placeholder" :value="props.value"
                :readonly="props.isReadOnly" @change="onChange">
            <textarea v-if="!props.isImage && props.isTextarea" name="" id="" rows="10"></textarea>
            <label v-if="props.isImage && !props.isTextarea">
                <input type="file" multiple @change="uploadFile" />
            </label>
            <span class="image_btn" v-if="props.isImage && !props.isTextarea"></span>
        </div>
        <div class="preview_wrap" v-if="imagePreviews.length > 0">
            <div class="preview" v-for="(preview, index) in imagePreviews" :key="index">
                <img :src="preview" />
                <!-- <span class="preview_fade">削除しますか</span> -->
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.preview_wrap {
    .preview {
        position: relative;
        display: block;
        width: 100%;
        height: 100%;
        margin: 20px 0px;

        &>img {
            width: 100%;
            height: 100%;
        }
    }

    .preview_fade {
        position: absolute;
        top: 0px;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, .4);
        color: #fff;
        font-size: 24px;
        font-weight: bold;
    }
}

.label_input_btn_wrap {
    max-width: 370px;
    margin: 0 auto;
    margin-bottom: 40px;

    .label_wrap {
        display: flex;
        justify-content: space-between;
        font-size: 16px;
        font-weight: 600;
        color: var(--font-color);
        margin-bottom: 11px;

        .require_label {
            color: var(--allow-orange-color);
        }

    }

    .input_wrap {
        position: relative;

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

        textarea {
            font-size: 16px;
            font-weight: 300;
            width: 100%;
            border-radius: 7px;
        }

        input.readonly {
            outline: none;
            background: transparent;
            font-size: 20px;
            font-weight: 600;
            color: var(--font-color);
        }

        input[type="file"] {
            display: none;
        }

        label {
            display: block;
            width: 100%;
            max-width: 360px;
            height: 51px;
            border-radius: 7px;
            border: 1px solid #707070;
            background: var(--white-color);
            padding: 0px 26px;
            font-size: 16px;
            font-weight: 300;
        }

        .image_btn {
            position: absolute;
            display: inline-block;
            width: 35px;
            height: 35px;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            border: 1px dotted #696969;
            pointer-events: none;

            &::before {
                position: absolute;
                content: '';
                display: inline-block;
                width: 24px;
                height: 2.5px;
                background: #696969;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
            }

            &::after {
                position: absolute;
                content: '';
                display: inline-block;
                width: 2.5px;
                height: 24px;
                background: #696969;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
            }
        }

        margin-bottom: 5vh;
    }

}
</style>