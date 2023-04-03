<!-- ShipmentJasApplicationView -->
<script setup lang="ts">
import { ref } from 'vue'
import { storeToRefs } from 'pinia'
import { UseUtil } from '@/stores/util'
import { UseData } from '@/stores/data'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import LeftLabelSelectBox from '@/components/common/LeftLabelSelectBox.vue'
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'

// Util
const useUtil = UseUtil()
const { contentRef } = storeToRefs(useUtil)
const useData = UseData()
const { jasCheckListObjs, GroupList } = storeToRefs(useData)

const check = (index: number) => {
    jasCheckListObjs.value[index].value = !jasCheckListObjs.value[index].value
}

const checkedValues = ref<boolean[]>([])

const clickBlackBtn = () => {
    useUtil.transition("/shipment/jasapplication_confirm")
}

</script>

<template>
    <Header title-label="フードチェーン情報公表JASとして出荷する" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="top_msg_wrap">
                <p>
                    JAS規格に基づいて、品質を維持するために<br />
                    下記の流通行程管理を行った場合は<br />
                    該当項目にチェックを入れてください。
                </p>
            </div>
            <LeftLabelSelectBox name="group" label="流通行程管理グループ" :option="GroupList" />
            <div class="check_box_wrap">
                <ul class="check_box_items">
                    <li class="check_box_item" v-for="obj, index in jasCheckListObjs">
                        <span class="checkbox">
                            <input type="checkbox" :name="obj.key" :value="obj.key" :checked="obj.value"
                                @change="check(index)">
                        </span>
                        <span class="label" @click="check(index)">{{ obj.label }}</span>
                    </li>
                </ul>
            </div>
            <BlackBgBtn btn-label="確認画面にすすむ" input-width="263px" @click-black-btn="clickBlackBtn" />
            <div class="spacer"></div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.top_msg_wrap {
    margin: 65px auto 39px;

    p {
        margin: 0px;
        font-size: 16px;
        font-weight: 600;
        color: var(--font-color);
    }
}

.check_box_wrap {
    display: flex;
    justify-content: center;

    .check_box_items {
        width: 100%;
        max-width: 360px;
        font-size: 16px;
        font-weight: 600;
        color: var(--font-color);
        margin-bottom: 104px;

        .check_box_item {
            display: flex;
            flex-wrap: nowrap;
            justify-content: stretch;
            align-items: center;
            padding: 0px 23px;
            min-height: 95px;
            border: 1px solid #707070;
            border-bottom: none;
            background: var(--white-color);

            .checkbox {
                margin-right: 14px;

                input[type=checkbox] {
                    transform: scale(1.6);
                    margin: 0 6px 0 2.5px;
                }
            }

            .label {
                flex: 1;
                text-align: left;
            }

            &:last-child {
                border-bottom: 1px solid #707070;
            }
        }
    }
}

.content {
    padding-bottom: 100px;
}
</style>