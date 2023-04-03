import { axios } from '@/util/axios'

export type Shipment = {
  ShipmentId: string
  Shipment: ShipmentCompany
  Shipping: Shipping
  Delivery: any
  IsInHouseDelivery: number
  ShipmentProducts: ShipmentProduct[]
  InvoiceCode: string
  ShipmentDate: string
  DeliveryDate: any
  ShipmentTypeCode: string
  ProducingAreaCode: any
  FirstShipmentId: any
  PreviousShipmentId: any
  Message: any
  Farmer: any
  _Owner_Id: string
}

export type ShipmentCompany = {
  ShipmentCompanyId: string
  ShipmentOfficeId: string
  ShipmentGln: string
}

export type Shipping = {
  ShippingCompanyId: string
  ShippingOfficeId: string
  ShippingGln: string
}

export type ShipmentProduct = {
  ProductCode: string
  Quantity: number
  PackageQuantity: number
  SinglePackageWeight: number
  ArrivalProductMap: any
}


// Get Shipment By ProductCode
export const getShipmentByProductCode = async (vendor_access_token: string, openidconnect_access_token: string, productCode: string) => {
    // console.debug("run getShipmentByProductCode")

    // console.log("入力されたQRコード:", productCode)

    // Main
    const apiBaseUrl = "/API/Traceability/V3/Private/Shipment/GetByProductCode"
    const param = "ProductCode=" + productCode
    const apiPath = apiBaseUrl + "?" + param
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const shipmentData = response?.data
    // console.log("shipmentData:", shipmentData)
    return shipmentData as Shipment[]
}
