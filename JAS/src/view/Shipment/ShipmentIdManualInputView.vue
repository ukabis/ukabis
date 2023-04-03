<!-- ShipmentIdManualInputView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { UseData } from '@/stores/data'
import { UseApi } from '@/stores/api'
import { storeToRefs } from 'pinia'

// API
import { preRequest } from '@/api/common/preRequest'
import { getShipmentByProductCode } from '@/api/shipment/getShipmentByProductCode'
import { getCompanyProductByGtinCode } from '@/api/shipment/getCompanyProductByGtinCode'
import { getCompanyNameByCompanyId } from '@/api/company/getCompanyByCompanyId'
import { getCropNameByCropCode } from '@/api/master/getCropNameByCropCode'
import { getBrandNameByBrandCode } from '@/api/master/getBrandNameByBrandCode'
import { getGradeList } from '@/api/traceability/getCapacityUnit'
import { getProductSizeList } from '@/api/traceability/getProductSize'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import LabelInputBtnComp from '@/components/common/LabelInputBtnComp.vue'
import LabeledBtn from '@/components/common/LabeledBtn.vue'
import { getPreLinkedSensorIDs } from '@/api/traceability/getPreLinkedSensorIDs'
import { getProductDetailByGTINCode } from '@/api/traceability/getProductDetail'
import { getGroupListByGroupId } from '@/api/common/getGroupListByGroupId'
import { ref } from 'vue'


// Util
const util = UseUtil()
const { contentRef, isFade } = storeToRefs(util)

// Data
const useData = UseData()
const { shipmentQrCode, shipmentGtinCode, shipmentObj, companyProductObj, shipmentInfo, shipmentOpts, ProductDetail, GroupList } = storeToRefs(useData)

// API
const api = UseApi()
const { companyIdAndNames, destinationCompany } = storeToRefs(api)

const reloading = () => {
    util.transition("/shipment/idscan")
}

const shipmentProductCode = ref<string>("")

const clickBtn = () => {
    isFade.value = true
    // 個体識別番号で出荷情報問い合わせAPIリクエスト
    preRequest()
        .then(async (res) => {

            // debug用サンプルデータ
            shipmentGtinCode.value = "14569951110013"

        // API実行

            // Get ProductDetail By GtinCode
            ProductDetail.value = await getProductDetailByGTINCode(res.access_token, res.openid_token, shipmentGtinCode.value)

            // Get CompanyProduct By GtinCode
            companyProductObj.value = await getCompanyProductByGtinCode(res.access_token, res.openid_token, shipmentGtinCode.value)

            // ProductDetailからProductCode取得、shipmentProductCodeに格納
            shipmentProductCode.value = ProductDetail.value[0].ProductCode

            // Get Shipment By ProductCode
            shipmentObj.value = await getShipmentByProductCode(res.access_token, res.openid_token, shipmentProductCode.value)

            // shipmentInfo作成
            shipmentInfo.value.shipping_destination = destinationCompany.value.companyName
            shipmentInfo.value.id = shipmentGtinCode.value
            shipmentInfo.value.shipping_company_name = await getCompanyNameByCompanyId(res.access_token, res.openid_token, companyProductObj.value.CompanyId)
            shipmentInfo.value.product_name = await getCropNameByCropCode(res.access_token, res.openid_token, companyProductObj.value.Profile.CropCode)
            shipmentInfo.value.brand = await getBrandNameByBrandCode(res.access_token, res.openid_token, companyProductObj.value.Profile.BrandCode)

            // 各Option取得・セット
            const gradeRaw = await getGradeList(res.access_token, res.openid_token)
            const productSizeRaw = await getProductSizeList(res.access_token, res.openid_token)
            gradeRaw.map(item => {
                shipmentOpts.value.Grade[item.ProductGradeCode] = item.ProductGradeName
            })
            productSizeRaw.map(item => {
                shipmentOpts.value.ProductSize[item.ProductSizeCode] = item.ProductSizeName
            })
            shipmentOpts.value.Weight = {}
            shipmentOpts.value.CapacityUnit = {}

            // ProductDetailにあるGroupIdから流通行程管理グループ取得
            const groupId = ProductDetail.value[0].Jas?.JasGroupId
            if(groupId) {
                GroupList.value = await getGroupListByGroupId(res.access_token, res.openid_token, groupId)
            }

            // 事前紐付きセンサー番号取得
            shipmentInfo.value.PreLinkedSensorIDs = await getPreLinkedSensorIDs(res.access_token, res.openid_token)

            // 問い合わせ完了後遷移
            util.transition("/shipment/confirm")
            isFade.value = false
        })
        .catch(err => util.errorHandling(err))
}

</script>

<template>
    <Header title-label="出荷情報登録" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="spacer top"></div>
            <div class="content_group">
                <LabelInputBtnComp word="個体識別番号を入力してください" @click-btn="clickBtn" :is-operation="true"
                    :value="shipmentProductCode" />
                <div class="labeled_btn_wrap">
                    <LabeledBtn label="個体識別番号を再読込する" text_align="center" @click="reloading" />
                </div>
            </div>
            <div class="spacer bottom"></div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.spacer {
    min-height: 20px;

    &.top {
        flex: 0.27;
    }

    &.bottom {
        flex: 0.17;
    }
}

.content {
    display: flex;
    flex-direction: column;

    .content_group {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        flex: 1;

        .labeled_btn_wrap {
            margin: 0 auto;
            width: 100%;
            max-width: 263px;
        }
    }
}
</style>