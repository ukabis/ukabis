<script setup lang="ts">

const props = defineProps<{
    data: any
}>()

const title_list = [
    "出荷先",
    "個体識別番号",
    "出荷事業者名",
    "品名",
    "ブランド",
]

const key_list = [
    "shipping_destination",
    "id",
    "shipping_company_name",
    "product_name",
    "brand",
]

/*
"出荷先": shipping_destination
"個体識別番号": id
"出荷事業者名": shipping_company_name
"品名": product_name
"ブランド": brand
*/

</script>


<template>
    <div class="info_table_wrap">
        <ul class="info_table_list">
            <li class="info_table_item" v-for="(key, num) in key_list">
                <span class="title">
                    {{ title_list[num] }}
                </span>
                <span class="body">
                    {{ props.data[key] }}
                </span>
            </li>
        </ul>
    </div>
</template>

<style scoped lang="scss">
.info_table_wrap {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    margin: 60px auto 53px;
    width: 100%;
    max-width: 430px;
    padding: 0px 35px;

    .info_table_list {
        display: block;
        width: 100%;

        .info_table_item {
            margin-bottom: 40px;

            &:last-child {
                margin-bottom: 0px;
            }

            .title {
                display: block;
                height: 28px;
                background: var(--input-btn-color);
                color: var(--white-color);
                font-size: 16px;
                font-weight: 600;
            }

            .body {
                display: flex;
                place-content: center;
                place-items: center;
                height: 50px;
                background: var(--white-color);
                font-size: 20px;
                font-weight: 600;
                color: var(--font-color);
            }
        }
    }
}
</style>