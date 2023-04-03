<!-- HomeView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'
import { storeToRefs } from 'pinia'

// Component
import Header from '@/components/common/Header.vue'
import Footer from '@/components/common/Footer.vue'

const util = UseUtil()
const { contentRef } = storeToRefs(util)

const incomingClick = () => {
    util.transition("/incoming/scan")
}
const shipmentClick = () => {
    util.transition("/shipment/registration")
}
const progressClick = () => {
    util.transition("/progress/scan")
}

const clickLogout = () => {
    util.transition("/")
}

</script>

<template>
    <Header title-label="ホーム" />
    <div class="bg">
        <div class="content" ref="contentRef">
            <div class="shipping_top_wrap">
                <div class="spacer"></div>
                <ul class="btn_wrap">
                    <li class="" @click="incomingClick">入荷情報登録</li>
                    <li class="" @click="shipmentClick">出荷情報登録</li>
                    <li class="" @click="progressClick">経過登録</li>
                </ul>
                <div class="logout_wrap">
                    <span class="logout" @click="clickLogout">
                        ログアウト
                    </span>
                </div>
            </div>
        </div>
    </div>
    <Footer />
</template>

<style scoped lang="scss">
.shipping_top_wrap {
    display: flex;
    flex-direction: column;
    place-content: stretch;
    height: 100%;

    .spacer {
        flex: 0.23;
        // background: orange;
    }

    .btn_wrap {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        // background: green;
        padding: 30px 35px;
        flex: 0.5;

        li {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            max-width: 360px;
            height: 70px;
            margin-bottom: 20px;
            color: #fff;
            font-size: 20px;
            font-weight: 600;
            border-radius: 5px;

            &:first-child {
                background: var(--received-bg-color);
            }

            &:nth-child(2) {
                background: var(--shipping-bg-color);
            }

            &:last-child {
                background: var(--progress-bg-color);
                margin-bottom: 0px;
            }
        }
    }

    .logout_wrap {
        display: flex;
        justify-content: center;
        align-items: center;
        flex: 0.4;
        padding: 10px 0px;
        // background: blue;

        .logout {
            display: flex;
            place-content: center;
            line-height: 23px;

            &::before {
                display: flex;
                place-content: center;
                content: url(/src/assets/HomeView/logout_icon.svg);
                margin-right: 6px;
                margin-left: 1px;
            }
        }
    }
}
</style>