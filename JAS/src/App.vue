<script setup lang="ts">
import { RouterView } from 'vue-router'
import Fade from '@/components/common/Fade.vue'
import AdminForMobileView from '@/view/Admin/AdminForMobileView.vue'

</script>

<template>
  <RouterView />
  <Fade />
  <AdminForMobileView />
</template>

<style scoped>

</style>