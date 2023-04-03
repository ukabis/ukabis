<script setup lang="ts">
// Util
import { ref, watch } from 'vue'
import { storeToRefs } from 'pinia'

// Store
import { CompanyOpt, OfficeOpt, StaffOpt, UseApi } from '@/stores/api'
import { StaffInfo, UseData } from '@/stores/data'

// API
import { getStaff, Staff } from '@/api/company/getStaff'

// Sample
import sample_staffList from '@/sample_data/sample_staffList.json'

// Props
const props = defineProps<{
    name: string
    num?: number
    isShowAddRow: boolean
    companyOpts: CompanyOpt[]
    officeOpts: OfficeOpt[]
    value?: StaffInfo
}>()

// Data
const useData = UseData()
const { newRegistration, targetOfficeList, targetStaffList, targetStaffLists, targetOfficeLists } = storeToRefs(useData)

// API
const api = UseApi()
const { accessToken, openId, staffObj } = storeToRefs(api)

// v-model
const companyVal = ref<string>("hidden")
const officeVal = ref<string>("hidden")
const staffVal = ref<string>("hidden")

// Init Value Set
if (props.value) {
    companyVal.value = props.value.companyId
    officeVal.value = props.value.officeId
    staffVal.value = props.value.staffId
}

// Target Opts
const targetOfficeOpts = ref<OfficeOpt[]>([])
const targetStaffOpts = ref<StaffOpt[]>([])

const initSetOpts = () => {
    if(props.name == "groupRepresentative") {
        targetOfficeOpts.value = targetOfficeList.value
        targetStaffOpts.value = targetStaffList.value
    } else {
        if(!props.num) return
        targetOfficeOpts.value = targetOfficeLists.value[props.name][props.num - 1]
        targetStaffOpts.value = targetStaffLists.value[props.name][props.num - 1]
    }
}
initSetOpts()

// Staff Data
const staffRawDatas = ref<Staff[]>([])

// Get CompanyName
const getCompanyName = () => {
    const targetObjs = props.companyOpts.filter(item => item.companyId == companyVal.value)
    const targetName = targetObjs[0].companyName
    return targetName
}
// Get OfficeName
const getOfficeName = () => {
    const targetObjs = props.officeOpts.filter(item => item.officeId == officeVal.value)
    const targetName = targetObjs[0].officeName
    return targetName
}
// Get StaffName
const getStaffName = () => {
    // const targetObjs = staffRawDatas.value.filter(item => item.StaffId == staffVal.value)
    const targetObjs = staffObj.value.filter(item => item.staffId == staffVal.value)
    const targetName = targetObjs[0].staffName
    return targetName
}

// setValue Func
const setValue = (itemName: string, num?: number) => {
    if (itemName == "groupRepresentative") {
        newRegistration.value[itemName] = {
            companyId: companyVal.value,
            officeId: officeVal.value,
            staffId: staffVal.value,
            companyName: getCompanyName(),
            officeName: getOfficeName(),
            staffName: getStaffName()
        }
    } else if (num && (
        itemName == "distributionResponsible" ||
        itemName == "distributionChargeList" ||
        itemName == "ratingResponsible" ||
        itemName == "ratingChargeList"
    )) {
        newRegistration.value[itemName][num - 1] = {
            companyId: companyVal.value,
            officeId: officeVal.value,
            staffId: staffVal.value,
            companyName: getCompanyName(),
            officeName: getOfficeName(),
            staffName: getStaffName()
        }
    }
}

// Company変更時実行
watch(companyVal, () => {
    console.log("companyVal:", companyVal.value)
    if (props.name == "groupRepresentative") { // Addボタンなし
        // オフィス、スタッフリセット
        newRegistration.value.groupRepresentative.officeId = "hidden"
        newRegistration.value.groupRepresentative.staffId = "hidden"
        targetStaffList.value = []
        targetStaffOpts.value = targetStaffList.value

        // 新しく選択したcompanyIDで取得したオプションをStoreに保存
        targetOfficeList.value = props.officeOpts.filter(item => item.companyId == companyVal.value)

        // 選択したcompanyIdをもつ事業者を事業者のオプションとしてセットし更新
        targetOfficeOpts.value = targetOfficeList.value

    } else if (( // Addボタンあり
        props.name == "distributionResponsible" ||
        props.name == "distributionChargeList" ||
        props.name == "ratingResponsible" ||
        props.name == "ratingChargeList"
    )) {
        if (!props.num) return

        // (num - 1)番目のオフィス、スタッフリセット
        newRegistration.value[props.name][props.num - 1].officeId = "hidden"
        newRegistration.value[props.name][props.num - 1].staffId = "hidden"
        targetStaffLists.value[props.name][props.num - 1] = []
        targetStaffOpts.value = targetStaffLists.value[props.name][props.num - 1]

        // 選択したcompanyIdをもつオフィスを(num - 1)番目のオフィスリストとしてStoreに保存
        targetOfficeLists.value[props.name][props.num - 1] = props.officeOpts.filter(item => item.companyId == companyVal.value)

        // 選択したcompanyIdをもつ事業者を事業者のオプションとしてセットし更新
        targetOfficeOpts.value = targetOfficeLists.value[props.name][props.num - 1]

    }
})

// Office変更時実行
watch(officeVal, async () => {
    console.log("officeVal:", officeVal.value)
    if (props.name == "groupRepresentative") {
        // スタッフリセット
        newRegistration.value.groupRepresentative.staffId = "hidden"

        // 新しく選択したofficeIDで取得したオフィスオプションをStoreに保存
        targetOfficeList.value = props.officeOpts.filter(item => item.companyId == companyVal.value)

        // 選択したofficeIdをもつオフィスをオフィスのオプションとしてセット
        targetOfficeOpts.value = targetOfficeList.value

        // スタッフオプションリセット
        targetStaffList.value = []

        // 選択したofficeIdをもつスタッフを、(num - 1)番目の番目のスタッフリストとしてStoreに保存
        const sample = sample_staffList.filter(staff => staff.Offices.includes(officeVal.value))
        targetStaffList.value = sample.map(item => ({
            staffId: item.StaffId,
            staffName: item.StaffName,
            officeId: item.Offices
        }))
        targetStaffOpts.value = targetStaffList.value
    } else if ((
        props.name == "distributionResponsible" ||
        props.name == "distributionChargeList" ||
        props.name == "ratingResponsible" ||
        props.name == "ratingChargeList"
    )) {
        if (!props.num) return

        // (num - 1)番目のオフィスとスタッフをリセット
        newRegistration.value[props.name][props.num - 1].officeId = "hidden"
        newRegistration.value[props.name][props.num - 1].staffId = "hidden"

        // 選択したofficeIdをもつスタッフを、(num - 1)番目の番目のスタッフリストとしてStoreに保存
        const sample = sample_staffList.filter(staff => staff.Offices.includes(officeVal.value))
        targetStaffLists.value[props.name][props.num - 1] = sample.map(item => ({
            staffId: item.StaffId,
            staffName: item.StaffName,
            officeId: item.Offices
        }))

        // スタッフのオプションをセット
        targetStaffOpts.value = targetStaffLists.value[props.name][props.num - 1]

    }

    // 事業所がデフォルトの時は以下は実行しない
    if (officeVal.value != "hidden") {
        // スタッフデータ取得
        staffObj.value = sample_staffList.map(({ StaffId, StaffName, Offices }) => ({
            staffId: StaffId,
            staffName: StaffName,
            officeId: Offices,
        }))

        // 選択した事業所のIDをもつスタッフをスタッフのオプションとしてセット
        targetStaffOpts.value = staffObj.value.filter(item => item.officeId.includes(officeVal.value))
    }

})

// Staff変更時実行
watch(staffVal, async () => {
    // 選択した値をセット
    setValue(props.name, props.num)

})

// Get Staff Opts
const getStaffOpts = async (officeId: string) => {
    staffRawDatas.value = await getStaff(accessToken.value, openId.value, officeId)
    return staffRawDatas.value
}

// Click AddRow
const clickAddRow = () => {
    if (!props.num) return
    emits("clickAddRow", props.name, props.num)
}

// Emits
const emits = defineEmits<{
    (e: "clickAddRow", name: string, num: number): void
}>()

/*

・Company、Officeは、親コンポーネントでAPIリクエスト、propsで受け取る
・StaffはOfficeIDで検索をかけ、APIリクエスト
・このコンポーネント自身がどの項目に使用されているかは、props.nameで判定

・事業者名を選択するごとに、事業所、スタッフの欄をデフォルト値にリセット
・事業者名を選択するごとに、選択されたcompanyIdをもつ事業所を取得し、オプションとしてセット

・事業所を選択するごとに、選択されたofficeIdをもつスタッフを取得し、オプションとしてセット

・各選択肢が選択されるたびに、storeの値を更新

*/

</script>

<template>
    <div class="td_content">
        <div class="input_select_wrap">
            <div class="select_box">
                <select v-model="companyVal">
                    <option selected disabled value="hidden">事業会社名選択</option>
                    <option v-for="item in companyOpts" :value="item.companyId">
                        {{ item.companyName }}
                    </option>
                </select>
            </div>
            <div class=" select_box">
                <select v-model="officeVal">
                    <option selected disabled value="hidden">事業所名選択</option>
                    <option  v-for="item in targetOfficeOpts" :value="item.officeId">
                        {{ item.officeName }}
                    </option>
                </select>
            </div>
            <div class="select_box">
                <select v-model="staffVal">
                    <option selected disabled value="hidden">所属スタッフ名</option>
                    <option v-for="item in targetStaffOpts" :value="item.staffId">
                        {{ item.staffName }}
                    </option>
                </select>
            </div>
        </div>
        <span v-if="isShowAddRow" class="plus_btn" @click="clickAddRow">
            <img src="/src/assets/Admin/common/plus_btn.svg" alt="枠を追加">
        </span>
    </div>
</template>

<style scoped lang="scss">
.td_content {
    display: flex;
    justify-content: stretch;
    padding-right: 60px;
    position: relative;

    &>.admin_input_wrap {
        margin-right: 15px;
    }

    .plus_btn {
        cursor: pointer;
        position: absolute;
        top: 5px;
        right: 29px;
    }

    &+.td_content {
        margin-top: 20px;
    }
}

.input_select_wrap {
    display: flex;
    justify-content: space-between;
    padding-right: 20px;
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
        flex: 1;
        display: flex;
        justify-content: center;
        align-items: center;
        position: relative;
        max-width: 360px;
        margin-right: 15px;

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
</style>