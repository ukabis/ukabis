<!-- IncomingPreRegistrationView -->
<script setup lang="ts">

import { ref } from 'vue'
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import InfoTableComp from '@/components/common/InfoTableComp.vue'
import LeftLabelSelectBox from '@/components/common/LeftLabelSelectBox.vue'
import LeftLabelInputBox from '@/components/common/LeftLabelInputBox.vue'
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'

// サンプルデータ
import sample_qr_data from '@/sample_data/sample_qr_data.json'
import { UseData } from '@/stores/data'

// Util
const util = UseUtil()
const { contentRef } = storeToRefs(util)

// Data
const data = UseData()
const { shipmentQrCode, shipmentOpts } = storeToRefs(data)

// QRコードから取得
const qrData = {
    shipping_destination: "JA静岡大谷支店",
    id: shipmentQrCode.value,
    shipping_company_name: "静岡メロン生産者の会",
    product_name: "JA静岡大谷支店",
    brand: "静岡クラウン"
}

// Func
const clickBlackBtn = () => {
    util.transition("/incoming/complete")
}

const isShowNotice = ref<boolean>(false)

</script>

<template>
    <Header title-label="入荷情報登録" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <InfoTableComp :data="qrData" />
            <div class="notice_wrap" v-if="isShowNotice">
                <span class="notice">
                    出荷情報はありません。<br />
                    入荷情報を登録する場合は下記項目を<br />
                    登録してください。
                </span>
            </div>
            <div class="input_wrap">
                <LeftLabelSelectBox name="Grade" label="等級(IDを指定)" :option="shipmentOpts.Grade" />
                <LeftLabelSelectBox name="ProductSize" label="階級" :option="shipmentOpts.ProductSize" />
                <LeftLabelSelectBox name="Weight" label="量目" :option="shipmentOpts.Weight" />
                <LeftLabelSelectBox name="CapacityUnit" label="荷姿" :option="shipmentOpts.CapacityUnit" />
                <LeftLabelInputBox label="入数" placeholder="5" :is-require="true" />
                <LeftLabelInputBox label="数量" placeholder="0" />
                <LeftLabelInputBox label="小売伝票番号" />
                <LeftLabelInputBox label="画像" :is-image="true" />
                <!-- <LeftLabelInputBox label="品名詳細" /> -->
                <LeftLabelInputBox label="備考" />
            </div>
            <div class="btn_wrap">
                <BlackBgBtn btn-label="入荷登録する" input-width="263px" @click-black-btn="clickBlackBtn" />
            </div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.notice_wrap {
    display: flex;
    padding: 47px 35px 4px;

    .notice {
        font-size: 16px;
        font-weight: 600;
        color: var(--notice-font-color);
        text-align: left;
    }
}

.input_wrap {
    width: 100%;
    max-width: 430px;
    margin: 0 auto 0;
    padding: 53px 35px 0px;

    &+.btn_wrap {
        margin-top: 120px;
        width: 100%;
        max-width: 100%;
        padding-bottom: 130px;

        &>div {
            margin-bottom: 40px;

            &:last-child {
                margin-bottom: 0px;
            }
        }
    }
}
</style>