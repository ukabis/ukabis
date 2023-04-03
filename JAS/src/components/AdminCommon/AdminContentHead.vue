<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import AdminInput from './AdminInput.vue'

// サンプルデータ
import admin_content_head from '@/sample_data/admin_content_head.json'

const admin_content_head_tuple: [string, string][] = admin_content_head.options.map(item => ([item[0], item[1]]))

const util = UseUtil()

const props = defineProps<{
    isShowRegisterGroupBtn?: boolean
    isShowGroupName?: boolean
}>()

const clickRegisterNewGroupBtn = () => {
    util.transition("/admin/newregistration")
}

const clickDisplayNum = (num: number) => {
    util.displayNum = num
}

</script>

<template>
    <div class="admin_content head">
        <span v-if="isShowGroupName" class="group_name">
            <AdminInput name="admin_content_head" input_mode="select" :options="admin_content_head_tuple" />
        </span>
        <ul class="display_num_wrap">
            <li>表示数</li>
            <li><span @click="clickDisplayNum(10)" class="cursor_pointer">10件</span></li>
            <li><span @click="clickDisplayNum(50)" class="cursor_pointer">50件</span></li>
            <li><span @click="clickDisplayNum(100)" class="cursor_pointer">100件</span></li>
        </ul>
        <span class="register_group_btn cursor_pointer" v-if="isShowRegisterGroupBtn" @click="clickRegisterNewGroupBtn">新規グループ登録</span>
    </div>
</template>

<style scoped lang="scss">
.admin_content {
    width: 100%;
    max-width: var(--admin-max-width);
    padding: 0px 10.4%;

    &.head {
        width: 100%;
        height: 50px;
        margin-top: 55.5px;
        display: flex;
        align-items: flex-end;
        justify-content: flex-end;

        .group_name {
            flex: 1;
        }

        .display_num_wrap {
            font-size: 14px;
            font-weight: 300;
            color: #707070;
            display: flex;
            flex-wrap: nowrap;
            justify-content: space-between;
            width: 267px;

            li {
                display: flex;
                justify-content: center;
                padding: 0px;
                margin-right: 0px;
                flex: 1;
                text-decoration: underline;

                &:first-child {
                    text-decoration: none;
                }

                &:last-child {
                    margin-right: 0px;
                }

                &::after {
                    content: "|";
                    position: relative;
                    right: -1em;
                }
            }
        }

        .register_group_btn {
            width: 194px;
            height: 50px;
            display: flex;
            align-items: center;
            justify-content: center;
            background: var(--allow-orange-color);
            border-radius: 5px;
            font-size: 16px;
            font-weight: 600;
            color: var(--white-color);
            margin-left: 19.7px;
        }
    }

    
}
</style>