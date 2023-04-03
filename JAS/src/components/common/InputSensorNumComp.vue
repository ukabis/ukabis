<!-- InputSensorNumComp -->
<script setup lang="ts">
// Util
import { ref } from 'vue'
import { UseUtil } from '@/stores/util'
import { emit } from 'process';

// Props
const props = defineProps<{
    label: string
    placeholder?: string
    isRequire?: boolean
    isImage?: boolean
}>()

// Util
const util = UseUtil()

// Add SensorBox
const sensorBoxNum = ref<number>(1)
// const sensorBoxList = ref<{ [key: string]: string }>({ "sensorBoxNum1": "" })
const addSensorBox = () => {
    sensorBoxNum.value++
    // sensorBoxList.value[`sensorBoxNum${sensorBoxNum.value}`] = ""
}

// Func
const changeInput = (index: number, e: any) => {
    // sensorBoxList.value[`${index}`] = e.target.value
    // console.log(sensorBoxList.value)
    emits("changeInput", index, e.target.value)
}

// Click ReadCode Btn
const clickReadCode = () => {
    util.transition("/shipment/sensornumscan")
}

// Emits
const emits = defineEmits<{
    (e: "changeInput", index: number, value: string): void
}>()

</script>

<template>
    <div class="label_input_btn_wrap">
        <div class="label_wrap">
            {{ props.label }}
        </div>
        <ul class="input_wrap">
            <li v-for="index in sensorBoxNum" :key="index">
                <input v-if="!props.isImage" type="text" :placeholder="props.placeholder"
                    @input="changeInput(index, $event)">
                <span class="read_code_btn" @click="clickReadCode"><span>コードを</span><span>読み込む</span></span>
            </li>
        </ul>
        <div class="add_box_btn_wrap">
            <div class="add_box_btn">
                <span class="add_mark"></span>
                <span class="label" @click="addSensorBox">センサー番号枠追加</span>
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
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
    }

    .input_wrap {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        width: 100%;

        li {
            width: 100%;
            display: flex;
            justify-content: space-between;
            margin-bottom: 20px;

            input {
                width: 57%;
                max-width: 360px;
                height: 51px;
                border-radius: 7px;
                border: 1px solid #707070;
                padding: 0px 26px;
                font-size: 16px;
                font-weight: 300;
            }

            .read_code_btn {
                width: 40%;
                display: flex;
                flex-wrap: wrap;
                align-items: center;
                justify-content: center;
                background: var(--input-btn-color);
                color: var(--white-color);
                font-size: 16px;
                font-weight: 600;
                line-height: 24px;
                border-radius: 7px;
            }
        }

    }

    .add_box_btn_wrap {
        display: flex;
        justify-content: center;
        width: 100%;
        max-width: 360px;
        margin-top: 10px;

        .add_box_btn {
            display: flex;
            flex-wrap: nowrap;
            align-items: center;
            justify-content: space-between;
            width: 100%;
            max-width: 204px;
            height: 51px;
            border: 1px solid #707070;
            border-radius: 7px;
            padding: 0px 13px 0px 10px;

            .add_mark {
                display: inline-block;
                width: 100%;
                height: 100%;
                max-width: 28px;
                max-height: 28px;
                border-radius: 4px;
                background: var(--font-color);
                position: relative;

                &::before {
                    position: absolute;
                    content: "";
                    display: inline-block;
                    width: 16px;
                    height: 3px;
                    background: var(--white-color);
                    top: 50%;
                    left: 50%;
                    transform: translate(-50%, -50%);
                }

                &::after {
                    position: absolute;
                    content: "";
                    display: inline-block;
                    width: 3px;
                    height: 16px;
                    background: var(--white-color);
                    top: 50%;
                    left: 50%;
                    transform: translate(-50%, -50%);
                }
            }

            .label {
                display: inline-block;
                font-size: 16px;
                font-weight: 600;
                color: var(--font-color);
            }

        }
    }
}
</style>