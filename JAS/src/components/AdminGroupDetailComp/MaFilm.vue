<script setup lang="ts">
</script>

<template>
    <div class="ma_film_wrap">
        <div class="item">
            <div class="left">
                <div class="check_item">予備項目１</div>
            </div>
            <div class="right">
                ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div class="check_item">予備項目２</div>
            </div>
            <div class="right">
                ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。ここにはコピーが入ります。
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.ma_film_wrap {
    margin-top: 48px;

    .item {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        margin-bottom: 55px;
        width: 100%;
        max-width: 945px;

        &:last-child {
            margin-bottom: 0px;
        }

        .left {
            .check_item {
                margin-top: 0px;
                margin-bottom: 11px;
                display: flex;
                justify-content: start;
                text-align: left;

                &.hidden {
                    display: none;
                }

                &::before {
                    content: url('/src/assets/common/checkmark.svg');
                    padding-top: 3px;
                    margin-right: 4px;
                }
            }
        }

        .right {
            max-width: 723.5px;
            font-weight: 300;
        }
    }
}
</style>
