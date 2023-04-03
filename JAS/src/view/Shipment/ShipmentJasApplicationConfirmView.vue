<!-- ShipmentJasApplicationConfirmView -->
<script setup lang="ts">
import { ref } from 'vue'
import { storeToRefs } from 'pinia'
import { UseUtil } from '@/stores/util'
import { UseData } from '@/stores/data'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import LeftLabelInputBox from '@/components/common/LeftLabelInputBox.vue'
import ClearBlackLetterBtn from '@/components/common/ClearBlackLetterBtn.vue'
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'

// Util
const useUtil = UseUtil()
const { contentRef } = storeToRefs(useUtil)
const useData = UseData()
const { jasCheckListObjs } = storeToRefs(useData)

const isCheckListObjsTrueItemExist = () => {
    let trueItemNum = 0
    jasCheckListObjs.value.map(item => {
        if (item.value) {
            trueItemNum++
        }
    })
    return trueItemNum ? true : false
}

const back = () => {
    useUtil.transition("/shipment/jasapplication")
}

const clickSubmit = () => {
    useData.resetJasCheckListObjs()
    useUtil.transition("/shipment/sent")
}

// debug
const groupName = ref<string>("静岡県メロン生産者の会")

</script>

<template>
    <Header title-label="内容確認" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <LeftLabelInputBox label="流通行程管理グループ" :value="groupName" :is-read-only="true" />
            <div class="check_items_wrap">
                <ul class="check_items" v-if="isCheckListObjsTrueItemExist()">
                    <li class="check_item" :class="{ hidden: !item.value }" v-for="item in jasCheckListObjs">{{ item.label
                    }}</li>
                </ul>
                <div class="" v-if="!isCheckListObjsTrueItemExist()">
                    チェック項目はありません。
                </div>
            </div>
            <div class="btn_group">
                <ClearBlackLetterBtn btn-label="内容修正" input-width="auto" @click="back" />
                <BlackBgBtn btn-label="送信する" input-width="auto" @click="clickSubmit" />
            </div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.content {
    padding: 55px 0px 86px;
}

.check_items_wrap {
    width: 100%;
    max-width: 360px;
    margin: 0 auto;
    display: flex;
    flex-direction: column;
    justify-content: center;
    font-size: 16px;
    font-weight: 600;
    color: var(--font-color);

    .check_items {
        margin-top: 20px;

        .check_item {
            min-height: 73px;
            display: flex;
            justify-content: start;
            text-align: left;
            padding-bottom: 1em;

            &.hidden {
                display: none;
            }

            &::before {
                content: url('/src/assets/common/checkmark.svg');
                padding-top: 3px;
                margin-right: 14px;
            }
        }
    }
}

.btn_group {
    margin: 64px auto 0px;
    width: 100%;
    max-width: 263px;

    &>div:first-child {
        margin-bottom: 26px;
    }
}
</style>