import { CompanyProduct } from '@/api/shipment/getCompanyProductByGtinCode'
import { Shipment } from '@/api/shipment/getShipmentByProductCode'
import { Grade } from '@/api/traceability/getGrade'
import { ProductDetail } from '@/api/traceability/getProductDetail'
import { ProductSize } from '@/api/traceability/getProductSize'
import { defineStore } from 'pinia'
import { OfficeOpt, StaffOpt } from './api'

type UseData = {
    shipmentQrCode: string | undefined
    shipmentGtinCode: string | undefined
    jasCheckListObjs: CheckType[]
    associationCheckListObjs: CheckType[]
    adminJasPackageData: AdminJasPackageData
    newRegistration: NewRegist
    targetOfficeList: OfficeOpt[]
    targetStaffList: StaffOpt[]
    targetOfficeLists: { [key: string]: OfficeOpt[][] }
    targetStaffLists: { [key: string]: StaffOpt[][] }
    shipmentObj: Shipment[]
    companyProductObj: CompanyProduct | null
    shipmentInfo: ShipmentInfo
    shipmentOpts: {
        Grade: {[key: string]: string}
        ProductSize: {[key: string]: string}
        Weight: {[key: string]: string}
        CapacityUnit: {[key: string]: string}
    }
    ProductDetail: ProductDetail[]
    JasProductDetail: string[]
    GroupList: {[key: string]: string}
}

type CheckType = {
    label: string
    key: string
    value: boolean
}

type AdminJasPackageData = {
    cropCode: string
    typeCode: string
    varietyCode: string
    lineageCode: string
    management_criteria: string
    certification_body: string
    precooling: boolean
    temperature_control: boolean
    picked_morning: boolean
    impact_management: boolean
    use_cushioning: boolean
    no_cushioning: boolean
    ma_film: boolean
    reserve1: boolean
    reserve2: boolean
    temperature_range_min: number
    temperature_range_max: number
    uncontrolled_temperature_range_1_min: number
    uncontrolled_temperature_range_1_max: number
    uncontrolled_time_1: number
    uncontrolled_temperature_range_2_min: number
    uncontrolled_temperature_range_2_max: number
    uncontrolled_time_2: number
    temperature_sensor: string
    morning_material: string
    shock_material: string
    allowable_g: number
    allowable_acceleration: number
    allowable_shocks: number
    packaging_conditions: string
    shock_sensor: string
    preliminary_1: string
    preliminary_2: string
}

export type StaffInfo = {
    companyId: string // 事業会社ID
    officeId: string // 事業所ID
    staffId: string // スタッフID
    companyName: string // 事業会社名
    officeName: string // 事業所名
    staffName: string // スタッフ名
}

export type NewRegist = {
    groupName: string // グループ名
    groupRepresentative: StaffInfo // グループ機関代表者
    systemUser: string // システム利用代表者
    distributionResponsible: StaffInfo[] // 流通行程管理責任者
    distributionChargeList: StaffInfo[] // 流通行程管理担当者
    ratingResponsible: StaffInfo[] // 格付責任者
    ratingChargeList: StaffInfo[] // 格付担当者
}

export type ShipmentInfo = {
    shipping_destination: string // 出荷先
    id: string // 個体識別番号
    shipping_company_name: string // 出荷事業者名
    product_name: string // 品名
    brand: string // ブランド
    GradeId: string | null // 等級ID
    GradeName: string | null // 等級名
    ProductSizeId: string | null // 階級ID
    ProductSizeName: string | null // 階級名
    CapacityUnitId: string | null // 荷姿ID
    CapacityUnitName: string | null // 荷姿名
    Quantity: number // 数量
    CompanyUrl: string[]
    ProductDetailSensorMap: string[] // センサー番号
    Weight: string | null  // 量目
    StandardPackageNumber: number | null // 入数
    RetailBillNumber: string | null // 小売伝票番号
    Image: string[] // 画像
    Remarks: string | null // 備考
    PreLinkedSensorIDs: string[] // 事前紐付きセンサー番号
}

export type ShipmentOpts = {
    GradeList: {[key: string]: string}[]
    ProductSizeList: {[key: string]: string}[]
    Weight: {[key: string]: string}[]
    CapacityUnitList: {[key: string]: string}[]
    SensorMapList: string[]
}

export const UseData = defineStore('data', {
    state: () => <UseData>({
        shipmentQrCode: "コードをスキャンしてください",
        shipmentGtinCode: "",
        jasCheckListObjs: [
            { label: "予冷管理", key: "precooling", value: false },
            { label: "朝採れ管理", key: "picked_morning", value: false },
            { label: "衝撃緩和効果が証明されている\n緩衝材の適用", key: "cushioning", value: false },
            { label: "湿度保持等の品質維持効果が\n証明されているMAフィルムの適用", key: "ma_film", value: false },
            { label: "予備項目１", key: "reserve1", value: false },
            { label: "予備項目 2", key: "reserve2", value: false },
        ],
        associationCheckListObjs: [
            { label: "予冷管理", key: "precooling", value: false },
            { label: "朝採れ管理", key: "picked_morning", value: false },
            { label: "衝撃緩和効果が証明されている\n緩衝材の適用", key: "cushioning", value: false },
            { label: "湿度保持等の品質維持効果が\n証明されているMAフィルムの適用", key: "ma_film", value: false },
            { label: "予備項目１", key: "reserve1", value: false },
            { label: "予備項目 2", key: "reserve2", value: false },
        ],
        adminJasPackageData: {
            cropCode: "",
            typeCode: "",
            varietyCode: "",
            lineageCode: "",
            management_criteria: "",
            certification_body: "",
            temperature_range_min: 0,
            temperature_range_max: 0,
            uncontrolled_temperature_range_1_min: 0,
            uncontrolled_temperature_range_1_max: 0,
            uncontrolled_time_1: 0,
            uncontrolled_temperature_range_2_min: 0,
            uncontrolled_temperature_range_2_max: 0,
            uncontrolled_time_2: 0,
            allowable_g: 0,
            allowable_acceleration: 0,
            allowable_shocks: 0,
            precooling: false,
            temperature_control: false,
            picked_morning: false,
            impact_management: false,
            use_cushioning: false,
            no_cushioning: false,
            ma_film: false,
            reserve1: false,
            reserve2: false,
            temperature_sensor: "",
            morning_material: "",
            shock_material: "",
            packaging_conditions: "",
            shock_sensor: "",
            preliminary_1: "",
            preliminary_2: "",
        },
        newRegistration: {
            groupName: "",
            groupRepresentative: {
                companyId: "hidden",
                officeId: "hidden",
                staffId: "hidden",
                companyName: "",
                officeName: "",
                staffName: "",
            },
            systemUser: "",
            distributionResponsible: [{
                companyId: "hidden",
                officeId: "hidden",
                staffId: "hidden",
                companyName: "",
                officeName: "",
                staffName: "",
            }],
            distributionChargeList: [{
                companyId: "hidden",
                officeId: "hidden",
                staffId: "hidden",
                companyName: "",
                officeName: "",
                staffName: "",
            }],
            ratingResponsible: [{
                companyId: "hidden",
                officeId: "hidden",
                staffId: "hidden",
                companyName: "",
                officeName: "",
                staffName: "",
            }],
            ratingChargeList: [{
                companyId: "hidden",
                officeId: "hidden",
                staffId: "hidden",
                companyName: "",
                officeName: "",
                staffName: "",
            }],
        },
        targetOfficeList: [],
        targetStaffList: [],
        targetOfficeLists: {
            distributionResponsible: [],
            distributionChargeList: [],
            ratingResponsible: [],
            ratingChargeList: []
        },
        targetStaffLists: {
            distributionResponsible: [],
            distributionChargeList: [],
            ratingResponsible: [],
            ratingChargeList: []
        },
        shipmentObj: [],
        companyProductObj: null,
        shipmentInfo: {
            shipping_destination: "", // 出荷先
            id: "", // 個体識別番号
            shipping_company_name: "", // 出荷事業者名
            product_name: "", // 品名
            brand: "", // ブランド
            GradeId: "", // 等級ID
            GradeName: "", // 等級名
            ProductSizeId: "", // 階級ID
            ProductSizeName: "", // 階級名
            CapacityUnitId: "", // 荷姿ID
            CapacityUnitName: "", // 荷姿名
            Quantity: 0, // 数量
            CompanyUrl: [], // 企業URL
            ProductDetailSensorMap: [], // センサー番号
            Weight: "" , // 量目
            StandardPackageNumber: 0, // 入数
            RetailBillNumber: "", // 小売伝票番号
            Image: [], // 画像
            Remarks: "", // 備考
            PreLinkedSensorIDs: [] // 事前紐付きセンサー番号
        },
        shipmentOpts: {
            Grade: {},
            ProductSize: {},
            Weight: {},
            CapacityUnit: {},
        },
        ProductDetail: [],
        JasProductDetail: [],
        GroupList: {},
    }),
    actions: {
        resetJasCheckListObjs() {
            this.jasCheckListObjs.map(item => {
                item.value = false
            })
        },
        resetAssociationCheckListObjs() {
            this.associationCheckListObjs.map(item => {
                item.value = false
            })
        },
        resetAdminJasPackageData() {
            this.adminJasPackageData.cropCode = ""
            this.adminJasPackageData.typeCode = ""
            this.adminJasPackageData.varietyCode = ""
            this.adminJasPackageData.lineageCode = ""
            this.adminJasPackageData.management_criteria = ""
            this.adminJasPackageData.certification_body = ""
            this.adminJasPackageData.temperature_range_min = 0
            this.adminJasPackageData.temperature_range_max = 0
            this.adminJasPackageData.uncontrolled_temperature_range_1_min = 0
            this.adminJasPackageData.uncontrolled_temperature_range_1_max = 0
            this.adminJasPackageData.uncontrolled_time_1 = 0
            this.adminJasPackageData.uncontrolled_temperature_range_2_min = 0
            this.adminJasPackageData.uncontrolled_temperature_range_2_max = 0
            this.adminJasPackageData.uncontrolled_time_2 = 0
            this.adminJasPackageData.allowable_g = 0
            this.adminJasPackageData.allowable_acceleration = 0
            this.adminJasPackageData.allowable_shocks = 0
            this.adminJasPackageData.precooling = false
            this.adminJasPackageData.temperature_control = false
            this.adminJasPackageData.picked_morning = false
            this.adminJasPackageData.impact_management = false
            this.adminJasPackageData.use_cushioning = false
            this.adminJasPackageData.no_cushioning = false
            this.adminJasPackageData.ma_film = false
            this.adminJasPackageData.reserve1 = false
            this.adminJasPackageData.reserve2 = false
            this.adminJasPackageData.temperature_sensor = ""
            this.adminJasPackageData.morning_material = ""
            this.adminJasPackageData.shock_material = ""
            this.adminJasPackageData.packaging_conditions = ""
            this.adminJasPackageData.shock_sensor = ""
            this.adminJasPackageData.preliminary_1 = ""
            this.adminJasPackageData.preliminary_2 = ""
        },
        resetNewRegistration() {
            this.newRegistration.groupName = ""
            this.newRegistration.groupRepresentative = {
                companyId: "",
                officeId: "",
                staffId: "",
                companyName: "",
                officeName: "",
                staffName: "",
            }
            this.newRegistration.systemUser = ""
            this.newRegistration.distributionResponsible = [{
                companyId: "",
                officeId: "",
                staffId: "",
                companyName: "",
                officeName: "",
                staffName: "",
            }]
            this.newRegistration.distributionChargeList = [{
                companyId: "",
                officeId: "",
                staffId: "",
                companyName: "",
                officeName: "",
                staffName: "",
            }]
            this.newRegistration.ratingResponsible = [{
                companyId: "",
                officeId: "",
                staffId: "",
                companyName: "",
                officeName: "",
                staffName: "",
            }]
            this.newRegistration.ratingChargeList = [{
                companyId: "",
                officeId: "",
                staffId: "",
                companyName: "",
                officeName: "",
                staffName: "",
            }]
        },
    }
})