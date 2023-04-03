<!-- ShipmentConfirmView -->
<script setup lang="ts">

// Util
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Data
import { UseData } from '@/stores/data'

// API
import { UseApi } from '@/stores/api'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import InfoTableComp from '@/components/common/InfoTableComp.vue'
import LeftLabelSelectBox from '@/components/common/LeftLabelSelectBox.vue'
import LeftLabelInputBox from '@/components/common/LeftLabelInputBox.vue'
import InputSensorNumComp from '@/components/common/InputSensorNumComp.vue'
import ClearBlackLetterBtn from '@/components/common/ClearBlackLetterBtn.vue'
import BlackBgBtn from '@/components/common/BlackBgBtn.vue'

// Sample Data
// import sample_qr_data from '@/sample_data/sample_qr_data.json'

// Util
const util = UseUtil()
const { contentRef } = storeToRefs(util)

// Data
const data = UseData()
const { shipmentQrCode, shipmentObj, companyProductObj, shipmentInfo, shipmentOpts } = storeToRefs(data)

// API
const api = UseApi()
const { destinationCompany, companyIdAndNames } = storeToRefs(api)

// Func
const jasClick = () => {
    util.transition("/shipment/jasapplication")
}

// Set Grade
const setGrade = (itemName: string, gradeId: string) => {
    shipmentInfo.value.GradeId = gradeId
    shipmentInfo.value.GradeName = shipmentOpts.value.Grade[gradeId]
}

// Set ProductSize
const setProductSize = (itemName: string, ProductSizeCode: string) => {
    shipmentInfo.value.ProductSizeId = ProductSizeCode
    shipmentInfo.value.ProductSizeName = shipmentOpts.value.ProductSize[ProductSizeCode]
}

// Set Weight
const setWeight = (itemName: string, Weight: string) => {
    shipmentInfo.value.Weight = Weight
}

// Set CapacityUnit
const setCapacityUnit = (itemName: string, CapacityUnitId: string) => {
    shipmentInfo.value.CapacityUnitId = CapacityUnitId
    shipmentInfo.value.CapacityUnitName = shipmentOpts.value.CapacityUnit[CapacityUnitId]
}

// Set StandardPackageNumber
const setStandardPackageNumber = (StandardPackageNumber: number) => {
    shipmentInfo.value.StandardPackageNumber = StandardPackageNumber
}
// Set Quantity ※必須
const setQuantity = (Quantity: number) => {
    shipmentInfo.value.Quantity = Quantity
}
// Set RetailBillNumber
const setRetailBillNumber = (RetailBillNumber: string) => {
    shipmentInfo.value.RetailBillNumber = RetailBillNumber
}
// Set Image
const setImage = (Images: string[]) => {
    console.log({Images})
}
// Set ItemDetails
// const setItemDetails = (ItemDetails: string) => {
//     shipmentInfo.value.ItemDetails = ItemDetails
// }
// Set Remarks
const setRemarks = (Remarks: string) => {
    shipmentInfo.value.Remarks = Remarks
}
// Set ProductDetailSensorMap
const setProductDetailSensorMap = (index: number, ProductDetailSensorMap: string) => {
    shipmentInfo.value.ProductDetailSensorMap[index - 1] = ProductDetailSensorMap
    console.log(shipmentInfo.value)
}

</script>

<template>
    <Header title-label="出荷情報登録" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <InfoTableComp :data="shipmentInfo" />
            <div class="input_wrap">
                <LeftLabelSelectBox name="Grade" label="等級(IDを指定)" :option="shipmentOpts.Grade" @emits-value="setGrade" />
                <LeftLabelSelectBox name="ProductSize" label="階級" :option="shipmentOpts.ProductSize" @emits-value="setProductSize" />
                <LeftLabelSelectBox name="Weight" label="量目" :option="shipmentOpts.Weight" @emits-value="setWeight" />
                <LeftLabelSelectBox name="CapacityUnit" label="荷姿" :option="shipmentOpts.CapacityUnit" @emits-value="setCapacityUnit" />
                <LeftLabelInputBox label="入数" placeholder="5" :is-number="true" @emits-number="setStandardPackageNumber" />
                <LeftLabelInputBox label="数量" placeholder="0"  :is-number="true" :is-require="true" @emits-number="setQuantity" />
                <LeftLabelInputBox label="小売伝票番号" @emits-value="setRetailBillNumber" />
                <LeftLabelInputBox label="画像" :is-image="true" @emits-files="setImage" />
                <!-- <LeftLabelInputBox label="品名詳細" @emits-value="setItemDetails" /> -->
                <LeftLabelInputBox label="備考" @emits-value="setRemarks" />
                <InputSensorNumComp label="センサー番号" @change-input="setProductDetailSensorMap" />
            </div>
            <div class="btn_wrap">
                <ClearBlackLetterBtn btn-label="フードチェーン情報公表JASとして
                        出荷する" input-width="100%" @click="jasClick" />
                <BlackBgBtn btn-label="上記ボタン以外で出荷する" input-width="100%" />
            </div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.input_wrap {
    width: 100%;
    max-width: 430px;
    margin: 0 auto 0;
    padding: 53px 35px 0px;

    &+.btn_wrap {
        margin-top: 120px;
        width: 100%;
        max-width: 360px;
        margin: 0 auto;
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