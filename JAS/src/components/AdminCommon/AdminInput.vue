<script setup lang="ts">
import { getValueType } from '@/util/getValueType'
import { ref } from 'vue'

const props = defineProps<{
    name: string
    input_mode: string
    placeholder?: string
    isReadOnly?: boolean
    value?: string | number
    optionName?: string
    options?: [string, string][] | [string, string, string][] | [string, string, string[]][]
    isDefaultItem?: boolean
    defaultLabel?: string
}>()

const textInput = ref()
const selectInput = ref()
const textareaInput = ref()

const isExistValue = ref<boolean>(false)
if(props.value) {
    isExistValue.value = true
}

const valueType = props.value ? getValueType(props.value): "string"

const inputChange = (e: any) => {
    const inputValue = e.target.value
    if(isNaN(Number(inputValue))) {
        emits("emitValue", inputValue, props.name)
    } else {
        emits("emitNum", inputValue, props.name)
    }
}
const selectChange = (e: any) => {
    emits("emitValue", e.target.value, props.name)
}
const textareaChange = (e: any) => {
    emits("emitValue", e.target.value, props.name)
}

const setOptions = () => {
    emits("setOptions", props.name)
}

const emits = defineEmits<{
    (e: "clickBlackBtn", value?: string): void
    (e: "emitValue", value: string, name: string): void
    (e: "emitNum", num: number, name: string): void
    (e: "setOptions", name: string): void
}>()

if(props.name=="temperature_sensor") {
    // console.log(isExistValue.value, valueType)
}

const sampleFunc = () => {
    console.log("サンプル！")
    console.log({
        "props.name": props.name,
        "props.optionName": props.optionName,
        "selectInput.value": selectInput.value,
        "selectInput.value[0]": selectInput.value[0],
    })
    selectInput.value = "hidden"
}

// Define Expose
defineExpose({
    sampleFunc,
})


</script>

<template>
    <div class="admin_input_wrap">
        <div class="input_text_wrap" v-if="input_mode == 'text'">
            <input v-if="isExistValue && valueType=='string'" ref="textInput" :name="props.name" type="text" :class="{ readonly: isReadOnly }" :placeholder="placeholder" :readonly="props.isReadOnly" :value="value" @change="inputChange">
            <input v-if="!isExistValue && valueType=='string'" ref="textInput" :name="props.name" type="text" :class="{ readonly: isReadOnly }" :placeholder="placeholder" :readonly="props.isReadOnly" @change="inputChange">
            <input v-if="isExistValue && valueType=='number'" ref="textInput" :name="props.name" type="number" :class="{ readonly: isReadOnly }" :placeholder="placeholder" :readonly="props.isReadOnly" :value="value" @change="inputChange">
            <input v-if="!isExistValue && valueType=='number'" ref="textInput" :name="props.name" type="number" :class="{ readonly: isReadOnly }" :placeholder="placeholder" :readonly="props.isReadOnly" @change="inputChange">
        </div>
        <div class="input_select_wrap" v-if="input_mode == 'select'">
            <div class="select_box">
                <select ref="selectInput" :name="props.name" @change="selectChange" @click="setOptions">
                    <option v-if="isDefaultItem" selected disabled value="hidden">{{ defaultLabel }}</option>
                    <option v-for="item in options" :value="item[0]">
                        {{ item[1] }}
                    </option>
                </select>
            </div>
        </div>
        <div class="input_textarea_wrap" v-if="input_mode=='textarea'">
            <textarea ref="textareaInput" :name="props.name" :class="{ readonly: isReadOnly }"
                :placeholder="placeholder" :readonly="props.isReadOnly" :value="value"
                @change="textareaChange"></textarea>
        </div>
    </div>
</template>

<style scoped lang="scss">
.admin_input_wrap {
    width: 100%;
}

.input_text_wrap {
    input {
        width: 100%;
        max-width: 484px;
        height: 45px;
        border: 1px solid #707070;
        border-radius: 7px;
        font-size: 16px;
        font-weight: 300;
        color: var(--font-color);
        padding: 14px 17px;

        &.readonly {
            outline: none;
            background: transparent;
        }
    }
}

.input_select_wrap {
    flex: 1;

    select {
        width: 100%;
        max-width: 361px;
        height: 45px;
        border-radius: 7px;
        padding: 0px 49px 0px 19px;
        -webkit-appearance: none;
        appearance: none;
        font-size: 16px;
        font-weight: 300;
        color: var(--font-color);

        /* デフォルトの矢印を無効 */
        select::-ms-expand {
            display: none;
            /* デフォルトの矢印を無効(IE用) */
        }
    }

    .select_box {
        display: flex;
        justify-content: center;
        align-items: center;
        position: relative;
        max-width: 360px;

        &::before {
            content: '';
            width: 1px;
            height: 80%;
            background: #797979;
            position: absolute;
            right: 50.5px;
        }

        &::after {
            content: url('/src/assets/common/arrow_orange.svg');
            width: auto;
            height: auto;
            position: absolute;
            right: 15.5px;
            pointer-events: none;
        }
    }
}

.input_textarea_wrap {
    textarea {
        width: 100%;
        max-width: 963px;
        height: 5.5em;
        border: 1px solid #707070;
        border-radius: 7px;
        font-size: 16px;
        font-weight: 300;
        color: var(--font-color);
        padding: 10px 17px;
    }
}
</style>