<!-- ShipmentRegistrationView -->
<script setup lang="ts">
// Util
import { onMounted, ref } from 'vue'
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// API
import { UseApi } from '@/stores/api'
import { preRequest } from '@/api/common/preRequest'
import { Company, getCompanyList } from '@/api/company/getCompany'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'
import SelectBox from '@/components/common/SelectBox.vue'
import LabeledBtn from '@/components/common/LabeledBtn.vue'

// Util
const util = UseUtil()
const { contentRef, isFade } = storeToRefs(util)
const onClick = () => {
    util.transition("/shipment/idscan")
}

// API
const api = UseApi()
const { destinationCompany, companyIdAndNames } = storeToRefs(api)

// Companyデータ取得
const companyRawDatas = ref<Company[]>([])
onMounted(() => {
    isFade.value = true
    preRequest()
        .then(async (res) => {
            isFade.value = false
            companyRawDatas.value = await getCompanyList(res.access_token, res.openid_token)
            companyRawDatas.value.map(({ CompanyId, CompanyName }) => {
                companyIdAndNames.value[CompanyId] = CompanyName
            })
        })
        .catch(err => console.error(err))
})

// 出荷先登録
const setShippingAddress = (selectedCompanyId: string) => {
    destinationCompany.value.companyId = selectedCompanyId
    destinationCompany.value.companyName = companyIdAndNames.value[selectedCompanyId]
}

</script>

<template>
    <Header title-label="出荷情報登録" line-type="shipment" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="spacer top"></div>
            <div class="content_group">
                <SelectBox label="出荷先登録" :option="companyIdAndNames" @emits-value="setShippingAddress" />
                <div class="labeled_btn_wrap">
                    <LabeledBtn label="個体識別番号を読みこむ" @click="onClick" />
                </div>
            </div>
            <div class="spacer bottom"></div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.content {
    display: flex;
    flex-direction: column;

    .content_group {
        flex: 1.5;
        display: flex;
        flex-direction: column;
        justify-content: space-between;

        .labeled_btn_wrap {
            margin: 0 auto;
            width: 100%;
            max-width: 263px;
        }
    }
}

.spacer {
    width: 100%;
    min-height: 20px;

    &.top {
        flex: 0.37;
    }

    &.bottom {
        flex: 0.25;
    }
}
</style>