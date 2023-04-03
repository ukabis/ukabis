<!-- AdminDistributionListView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import AdminContentHead from '@/components/AdminCommon/AdminContentHead.vue'

// グループデータ
import group_data from '@/sample_data/group_data.json'

// サンプルデータ
import admin_distribution_list from '@/sample_data/admin_distribution_list.json'

const sizuokaList = admin_distribution_list["静岡メロン生産者の会"]

const util = UseUtil()
const clickGroupName = () => {
    util.transition("/admin/groupdetail")
}

// Breadcrumbs
const breadcrumbs = [{
    path: "",
    label: "流通工程管理"
}]

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理"
        :is-show-breadcrumbs="true" />
    <div class="content_bg">
        <div class="content_wrap">
            <Breadcrumbs :breadcrumbs-item="breadcrumbs" />
            <AdminContentHead :is-show-register-group-btn="false" :is-show-group-name="true" />
            <div class="admin_content body">
                <table>
                    <tr>
                        <th>農作物名</th>
                        <th>グループ機関代表者</th>
                        <th>個体識別番号</th>
                        <th>初回出荷日</th>
                        <th>最終入荷日</th>
                    </tr>
                    <tr v-for="item in sizuokaList">
                        <td><span class="">{{ item.CropName }}</span></td>
                        <td><span class="">{{ item.GroupRepresentative }}</span></td>
                        <td><span class="cursor_pointer underline">{{ item.IdentificationNumber }}</span></td>
                        <td><span class="">{{ item.FirstShipmentDate }}</span></td>
                        <td><span class="">{{ item.LastShipmentDate }}</span></td>
                    </tr>
                </table>
            </div>
            <div class="admin_content foot">
                <ul class="pagination_items">
                    <li class="current cursor_pointer">1</li>
                    <li class="cursor_pointer">2</li>
                    <li class="cursor_pointer">3</li>
                    <li class="cursor_pointer">4</li>
                    <li class="cursor_pointer">5</li>
                    <li class="cursor_pointer">…</li>
                    <li class="cursor_pointer">99</li>
                </ul>
            </div>
        </div>
    </div>
    <AdminFooter />
</template>

<style scoped lang="scss">
.content_bg {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
    width: 100%;
    background: var(--main-bg-color);
}

.content_wrap {
    // margin: 0 auto;
    // display: flex;
    // flex-direction: column;
    // width: 100%;
    // height: 100%;
    // min-height: calc(100vh - 202px);
    max-width: var(--admin-max-width);
    // overflow-y: scroll;

    .admin_content {
        width: 100%;
        max-width: var(--admin-max-width);
        padding: 0px 10.4%;

        &.body {
            padding-top: 17.6px;

            table {
                width: 100%;
                border-collapse: collapse;

                tr {
                    th {
                        text-align: left;
                        padding-left: 7.1px;
                        height: 26px;
                        background: var(--input-btn-color);
                        font-size: 14px;
                        font-weight: 300;
                        color: var(--white-color);
                        border-right: 1px solid var(--white-color);
                    }

                    border: solid var(--main-bg-color);
                    border-width: 3px 3px 3px 3px;

                    &:first-child {
                        border-width: 0px;
                    }

                    td {
                        text-align: left;
                        padding-left: 7.1px;
                        height: 60px;
                        background: var(--white-color);
                        font-size: 16px;
                        font-weight: 300;
                        color: var(--font-color);
                        // width: 21.8%;
                        width: 20%;
                    }
                }
            }
        }

        &.foot {
            padding-bottom: 20px;

            .pagination_items {
                display: flex;
                flex-wrap: nowrap;
                width: 264.1px;
                align-items: center;
                justify-content: space-between;
                padding: 0px 19.7px;
                margin-top: 16px;

                &::before {
                    content: url("/src/assets/Admin/common/pagination_arrow.svg");
                    position: relative;
                    top: 0px;
                    ;
                    left: -10px;
                }

                &::after {
                    content: url("/src/assets/Admin/common/pagination_arrow.svg");
                    position: relative;
                    transform: scaleX(-1);
                    top: 0px;
                    ;
                    right: -10px;
                }

                li {
                    width: 28px;
                    height: 27px;
                    background: var(--white-color);
                    color: var(--font-color);
                    font-size: 12px;
                    font-weight: 300;
                    border-radius: 5px;

                    &.current {
                        background: var(--font-color);
                        color: var(--white-color);
                    }
                }
            }
        }
    }
}

.check_form_btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 58%;
    max-width: 170px;
    min-width: 150px;
    max-height: 35px;
    font-size: 16px;
    font-weight: 300;
    color: var(--input-btn-color);
    border: 2px solid var(--input-btn-color);
    border-radius: 5px;
    padding-right: 0px;
    margin: 0 auto;

    &::after {
        content: url("/src/assets/Admin/common/check_form_btn_arrow.svg");
        position: relative;
        top: -0.6px;
        right: -8px;
    }
}
</style>