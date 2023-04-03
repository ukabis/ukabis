<script setup lang="ts">
import { UseUtil } from '@/stores/util'

const clickClose = () => {
    emits("clickClose")
}

const emits = defineEmits<{
    (e: "clickClose", value?: string): void
}>()

const util = UseUtil()

const incomingClick = () => {
    util.transition("/incoming/scan")
}
const shipmentClick = () => {
    util.transition("/shipment/registration")
}
const progressClick = () => {
    util.transition("/progress/scan")
}
const logout = () => {
    util.transition("/")
}

</script>

<template>
    <div class="menu_wrap">
        <ul>
            <li class="close_btn_wrap">
                <span class="close_btn" @click="clickClose"></span>
            </li>
            <li @click="incomingClick">入荷登録</li>
            <li @click="shipmentClick">出荷登録</li>
            <li @click="progressClick">経過登録</li>
            <li @click="logout">ログアウト</li>
        </ul>
    </div>
</template>

<style scoped lang="scss">
.menu_wrap {
    display: flex;
    width: 271px;
    position: fixed;
    top: 0px;
    right: 0px;

    ul {
        width: 100%;

        li {
            min-height: 95px;
            font-size: 16px;
            font-weight: 600;
            color: var(--font-color);
            text-align: left;
            padding: 35.5px 22px;
            background: #fff;
            border: 1px solid #BFBFBF;

            &.close_btn_wrap {
                position: relative;

                .close_btn {
                    position: absolute;
                    top: 48.3px;
                    right: 14.2px;
                    display: inline-block;
                    width: 30px;
                    height: 30px;

                    &::before {
                        content: '';
                        position: absolute;
                        top: 50%;
                        left: 50%;
                        display: block;
                        width: 29.5px;
                        height: 3px;
                        background: #000;
                        transform: translate(-50%, -50%) rotate(45deg);
                    }

                    &::after {
                        content: '';
                        position: absolute;
                        top: 50%;
                        left: 50%;
                        display: block;
                        width: 29.5px;
                        height: 3px;
                        background: #000;
                        transform: translate(-50%, -50%) rotate(-45deg);
                    }
                }
            }
        }
    }
}
</style>