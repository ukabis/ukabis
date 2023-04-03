import LoginViewVue from '@/view/LoginView.vue'
import LogoutViewVue from '@/view/LogoutView.vue'
import HomeView from '@/view/HomeView.vue'
import ShipmentRegistrationView from '@/view/Shipment/ShipmentRegistrationView.vue'
import ShipmentIdScanView from '@/view/Shipment/ShipmentIdScanView.vue'
import ShipmentIdManualInputView from '@/view/Shipment/ShipmentIdManualInputView.vue'
import ShipmentConfirmView from '@/view/Shipment/ShipmentConfirmView.vue'
import ShipmentJasApplicationView from '@/view/Shipment/ShipmentJasApplicationView.vue'
import ShipmentJasApplicationConfirmView from '@/view/Shipment/ShipmentJasApplicationConfirmView.vue'
import ShipmentSensorNumScanView from '@/view/Shipment/ShipmentSensorNumScanView.vue'
import ShipmentAssociationView from '@/view/Shipment/ShipmentAssociationView.vue'
import ShipmentAssociationConfirmView from '@/view/Shipment/ShipmentAssociationConfirmView.vue'
import ShipmentSentView from '@/view/Shipment/ShipmentSentView.vue'
import IncomingScanView from '@/view/Incoming/IncomingScanView.vue'
import IncomingIdInputView from '@/view/Incoming/IncomingIdInputView.vue'
import IncomingPreRegistrationView from '@/view/Incoming/IncomingPreRegistrationView.vue'
import IncomingCompleteView from '@/view/Incoming/IncomingCompleteView.vue'
import ProgressScanView from '@/view/Progress/ProgressScanView.vue'
import ProgressIdInputView from '@/view/Progress/ProgressIdInputView.vue'
import ProgressRegistrationView from '@/view/Progress/ProgressRegistrationView.vue'
import ProgressSentView from '@/view/Progress/ProgressSentView.vue'
import AdminLoginView from '@/view/Admin/AdminLoginView.vue'
import AdminHomeView from '@/view/Admin/AdminHomeView.vue'
import AdminGroupDetailView from '@/view/Admin/AdminGroupDetailView.vue'
import AdminNewRegistrationView from '@/view/Admin/AdminNewRegistrationView.vue'
import AdminConfirmationView from '@/view/Admin/AdminConfirmationView.vue'
import AdminConfirmedView from '@/view/Admin/AdminConfirmedView.vue'
import AdminJasPackageView from '@/view/Admin/AdminJasPackageView.vue'
import AdminJasPackageConfirmationView from '@/view/Admin/AdminJasPackageConfirmationView.vue'
import AdminJasPackageCompleteView from '@/view/Admin/AdminJasPackageCompleteView.vue'
import AdminDistributionListView from '@/view/Admin/AdminDistributionListView.vue'
import AdminDistributionDetailView from '@/view/Admin/AdminDistributionDetailView.vue'

import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'
const router = createRouter({
    history: createWebHistory(import.meta.env.VITE_DIR_PATH),
    // history: createWebHashHistory(),
    // history: createWebHashHistory(import.meta.env.BASE_URL),
    // history: createMemoryHistory(window.location.origin + import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'login',
            component: LoginViewVue
        },
        {
            path: '/logout',
            name: 'logout',
            component: LogoutViewVue
        },
        {
            path: '/home',
            name: 'home',
            component: HomeView
        },
        {
            path: '/shipment/registration',
            name: 'shipment_registration',
            component: ShipmentRegistrationView
        },
        {
            path: '/shipment/idscan',
            name: 'shipment_idscan',
            component: ShipmentIdScanView
        },
        {
            path: '/shipment/idmanualinput',
            name: 'shipment_idmanualinput',
            component: ShipmentIdManualInputView
        },
        {
            path: '/shipment/confirm',
            name: 'shipment_confirm',
            component: ShipmentConfirmView
        }, {
            path: '/shipment/jasapplication',
            name: 'shipment_jasapplication',
            component: ShipmentJasApplicationView
        }, {
            path: '/shipment/jasapplication_confirm',
            name: 'shipment/jasapplication_confirm',
            component: ShipmentJasApplicationConfirmView
        }, {
            path: '/shipment/sensornumscan',
            name: 'shipment_sensornumscan',
            component: ShipmentSensorNumScanView
        }, {
            path: '/shipment/association',
            name: 'shipment_association',
            component: ShipmentAssociationView
        }, {
            path: '/shipment/association_confirm',
            name: 'shipment/association_confirm',
            component: ShipmentAssociationConfirmView
        }, {
            path: '/shipment/sent',
            name: 'shipment_sent',
            component: ShipmentSentView
        }, {
            path: '/incoming/scan',
            name: 'incoming_scan',
            component: IncomingScanView
        }, {
            path: '/incoming/idinput',
            name: 'incoming_idinput',
            component: IncomingIdInputView
        }, {
            path: '/incoming/preregistration',
            name: 'incoming_preregistration',
            component: IncomingPreRegistrationView
        }, {
            path: '/incoming/complete',
            name: 'incoming_complete',
            component: IncomingCompleteView
        }, {
            path: '/progress/scan',
            name: 'progress_scan',
            component: ProgressScanView
        }, {
            path: '/progress/idinput',
            name: 'progress_idinput',
            component: ProgressIdInputView
        }, {
            path: '/progress/registration',
            name: 'progress_registration',
            component: ProgressRegistrationView
        }, {
            path: '/progress/sent',
            name: 'progress_sent',
            component: ProgressSentView
        }, {
            path: '/admin/login',
            name: 'admin_login',
            component: AdminLoginView
        }, {
            path: '/admin/home',
            name: 'admin_home',
            component: AdminHomeView
        }, {
            path: '/admin/groupdetail',
            name: 'admin_groupdetail',
            component: AdminGroupDetailView
        }, {
            path: '/admin/newregistration',
            name: 'admin_newregistration',
            component: AdminNewRegistrationView
        }, {
            path: '/admin/confirmation',
            name: 'admin_confirmation',
            component: AdminConfirmationView
        }, {
            path: '/admin/confirmed',
            name: 'admin/confirmed',
            component: AdminConfirmedView
        }, {
            path: '/admin/jaspackage',
            name: 'admin_jaspackage',
            component: AdminJasPackageView
        }, {
            path: '/admin/jaspackageconfirmation',
            name: 'admin_jaspackageconfirmation',
            component: AdminJasPackageConfirmationView
        }, {
            path: '/admin/jaspackagecomplete',
            name: 'admin_jaspackagecomplete',
            component: AdminJasPackageCompleteView
        }, {
            path: '/admin/distributionlist',
            name: 'admin_distributionlist',
            component: AdminDistributionListView
        }, {
            path: '/admin/distributiondetail',
            name: 'admin_distributiondetail',
            component: AdminDistributionDetailView
        },
    ]
})
export default router