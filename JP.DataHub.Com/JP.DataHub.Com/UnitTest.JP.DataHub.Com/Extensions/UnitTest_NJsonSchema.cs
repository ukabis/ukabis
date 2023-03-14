using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using JP.DataHub.Com.Extensions;

namespace UnitTest.JP.DataHub.Com.Extensions
{
    [TestClass]
    public class UnitTest_NJsonSchema
    {
        private string model = @"
{
  'description': 'Arrival',
  'definitions': {
    'ArrivalProduct': {
      'description': 'ArrivalProduct',
      'type': 'object',
      'additionalProperties': false,
      'properties': {
        'ArrivalProductId': {
          'title': '入荷商品ID',
          'type': 'string',
          'maxLength': 64
        },
        'InvoiceCode': {
          'title': '商品単位の送り状コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64
        },
        'CropCode': {
          'title': '農作物コード',
          'type': 'string',
          'maxLength': 64,
          'format': 'ForeignKey /API/Master/Crop/Get/{value}'
        },
        'BreedCode': {
          'title': '品種コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64,
          'format': 'ForeignKey /API/SmartFoodChain/V2/Master/CropBreed/Get/{value}'
        },
        'BrandCode': {
          'title': 'ブランドコード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64,
          'format': 'ForeignKey /API/SmartFoodChain/V2/Master/CropBrand/Get/{value}'
        },
        'GradeCode': {
          'title': '等級コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64
        },
        'SizeCode': {
          'title': 'サイズコード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64,
          'format': 'ForeignKey /API/SmartFoodChain/V2/Master/ProductSize/Get/{value}'
        },
        'PackageQuantity': {
          'title': '梱包数=箱の数など',
          'type': 'integer'
        },
        'SinglePackageWeight': {
          'title': '単一の重さ',
          'type': 'integer'
        },
        'CapacityUnitCode': {
          'title': '容量単位コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64
        },
        'ReceivePackageQuantity': {
          'title': '入荷数',
          'type': 'integer'
        },
        'DamagePackageQuantity': {
          'title': '破損数',
          'type': 'integer'
        },
        'ProductCode': {
          'title': '商品コード',
          'type': [
            'array',
            'null'
          ],
          'items': {
            'type': [
              'string',
              'null'
            ]
          },
          'maxLength': 64
        },
        'AttachFileId': {
          'title': '添付ファイルID',
          'type': [
            'array',
            'null'
          ],
          'items': {
            'type': [
              'string',
              'null'
            ]
          },
          'maxLength': 128
        }
      },
      'required': [
        'ArrivalProductId',
        'CropCode',
        'PackageQuantity',
        'SinglePackageWeight',
        'ReceivePackageQuantity',
        'DamagePackageQuantity'
      ]
    },
    'ArrivalProductNull': {
      'description': 'ArrivalProduct',
      'type': [
        'object',
        'null'
      ],
      'additionalProperties': false,
      'properties': {
        'ArrivalProductId': {
          'title': '入荷商品ID',
          'type': 'string',
          'maxLength': 64
        },
        'InvoiceCode': {
          'title': '商品単位の送り状コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64
        },
        'CropCode': {
          'title': '農作物コード',
          'type': 'string',
          'maxLength': 64,
          'format': 'ForeignKey /API/Master/Crop/Get/{value}'
        },
        'BreedCode': {
          'title': '品種コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64,
          'format': 'ForeignKey /API/SmartFoodChain/V2/Master/CropBreed/Get/{value}'
        },
        'BrandCode': {
          'title': 'ブランドコード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64,
          'format': 'ForeignKey /API/SmartFoodChain/V2/Master/CropBrand/Get/{value}'
        },
        'GradeCode': {
          'title': '等級コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64
        },
        'SizeCode': {
          'title': 'サイズコード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64,
          'format': 'ForeignKey /API/SmartFoodChain/V2/Master/ProductSize/Get/{value}'
        },
        'PackageQuantity': {
          'title': '梱包数=箱の数など',
          'type': 'integer'
        },
        'SinglePackageWeight': {
          'title': '単一の重さ',
          'type': 'integer'
        },
        'CapacityUnitCode': {
          'title': '容量単位コード',
          'type': [
            'string',
            'null'
          ],
          'maxLength': 64
        },
        'ReceivePackageQuantity': {
          'title': '入荷数',
          'type': 'integer'
        },
        'DamagePackageQuantity': {
          'title': '破損数',
          'type': 'integer'
        },
        'ProductCode': {
          'title': '商品コード',
          'type': [
            'array',
            'null'
          ],
          'items': {
            'type': [
              'string',
              'null'
            ]
          },
          'maxLength': 64
        },
        'AttachFileId': {
          'title': '添付ファイルID',
          'type': [
            'array',
            'null'
          ],
          'items': {
            'type': [
              'string',
              'null'
            ]
          },
          'maxLength': 128
        }
      },
      'required': [
        'ArrivalProductId',
        'CropCode',
        'PackageQuantity',
        'SinglePackageWeight',
        'ReceivePackageQuantity',
        'DamagePackageQuantity'
      ]
    }
  },
  'type': 'object',
  'additionalProperties': false,
  'properties': {
    'ArrivalId': {
      'title': '入荷ID',
      'type': [
        'string',
        'null'
      ],
      'maxLength': 64
    },
    'ShipmentTypeCode': {
      'title': '入出荷タイプコード',
      'type': 'string',
      'maxLength': 64,
      'format': 'ForeignKey /API/SmartFoodChain/V2/Master/ShipmentType/Get/{value}'
    },
    'ArrivalDate': {
      'title': '入荷日時',
      'type': 'string',
      'format': 'date-time'
    },
    'InvoiceCode': {
      'title': '送り状コード',
      'type': [
        'string',
        'null'
      ],
      'maxLength': 128
    },
    'ShipmentId': {
      'title': '出荷ID',
      'type': [
        'string',
        'null'
      ],
      'maxLength': 64
    },
    'ShipmentPartyId': {
      'title': '出荷業者ID',
      'type': 'string',
      'maxLength': 64
    },
    'ShipmentOfficeId': {
      'title': '出荷事業所',
      'type': 'string',
      'maxLength': 64
    },
    'ShipmentGln': {
      'title': '出荷元GLN',
      'type': 'string',
      'maxLength': 256
    },
    'ArrivalPartyId': {
      'title': '入荷業者ID',
      'type': 'string',
      'maxLength': 64
    },
    'ArrivalOfficeId': {
      'title': '入荷事業所',
      'type': 'string',
      'maxLength': 64
    },
    'ArrivalGln': {
      'title': '入荷業者GLN',
      'type': 'string',
      'maxLength': 256
    },
    'ArrivalProduct': {
      'items': {
        '$ref': '#/definitions/ArrivalProduct'
      },
      'type': 'array'
    }
  },
  'required': [
    'ShipmentTypeCode',
    'ArrivalDate',
    'ShipmentPartyId',
    'ShipmentOfficeId',
    'ShipmentGln',
    'ArrivalPartyId',
    'ArrivalOfficeId',
    'ArrivalGln',
    'ArrivalProduct'
  ]
}";

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void JsonProperty_FindProperty()
        {
            var schema = JsonSchema.FromJsonAsync(model).Result;
            schema.FindProperty("ArrivalProduct").Name.Is("ArrivalProduct");
            //schema.FindProperty("ArrivalProduct.ArrivalProductId").Name.Is("ArrivalProductId");
        }
    }
}
