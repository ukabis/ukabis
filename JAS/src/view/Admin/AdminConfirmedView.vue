<!-- AdminConfirmedView -->
<script setup lang="ts">

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import LongBtn from '@/components/AdminCommon/LongBtn.vue'
import { UseUtil } from '@/stores/util'

const util = UseUtil()

const clickToHome = () => {
    util.transition("/admin/home")
}

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理" />
    <div class="content_bg">
        <div class="content_wrap">
            <div class="content_body">
                <div class="registration_completed_msg">
                    登録しました
                </div>
                <div class="btn_wrap">
                    <LongBtn label="ホームへ" btn-type="white" @click="clickToHome" />
                </div>
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
    // flex: 1;
    // display: flex;
    // flex-direction: column;
    // justify-content: center;
    // width: 100%;
    // height: 100%;
    // min-height: calc(100vh - 202px);
    // max-width: var(--admin-max-width);

    .content_body {
        flex: 1;
        display: flex;
        flex-direction: column;
        align-items: stretch;
        justify-content: stretch;
        padding: 100px 10.4% 0px;

        .registration_completed_msg {
            flex: 0.67;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            font-size: 26px;
            font-weight: 600;
            color: var(--font-color);
        }
    }
}
</style>