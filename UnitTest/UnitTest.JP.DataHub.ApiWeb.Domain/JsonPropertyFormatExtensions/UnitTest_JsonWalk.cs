using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    [TestClass]
    public class UnitTest_JsonWalk
    {
        private const string schema_string = @"
{
  'properties': {
    'Code': {
      'type': 'string',
      'required':true,
    },
    'Name': {
      'maxLength': 20,
      'type': 'string',
      'required':true,
    },
    'ArrayString': {
      'type': 'array',
      'item': [ { 'type' : 'string' } ],
      'format':'Protect',
    },
    'ArrayInt': {
      'type': 'array',
      'item': [ { 'type' : 'number', } ],
    },
    'Object': {
      'type': 'object',
      'properties': {
        'Prop1': {
          'type': 'string',
        },
        'Prop2': {
          'type': 'string',
          'format':'Protect',
        },
        'Prop3': {
          'type': 'object',
          'properties': {
            'Prop31': {
              'type': 'string',
              'format':'Protect',
            },
          },
        },
      },
    },
    'ObjectArray': {
      'type': 'array',
      'items': {
        'properties': {
          'Date': {
            'type': 'string',
          },
          'ItemCode': {
            'type': 'string',
            'format' : 'Protect',
          },
          'Value': {
            'type': 'string',
          },
        },
      },
    },
   'additionalProperties' : true
  },
  'type': 'object'
}";
        private const string json_string = @"
{
  'Code' : '1',
  'Name' : 'name',
  'ArrayString' : ['123', '456' ],
  'ArrayInt' : [123, 456, 789 ],
  'Object' : {
    'Prop1' : 'p1',
    'Prop2' : 'p2',
    'Prop3' : {
      'Prop31' : 'p31',
    },
  },
  'ObjectArray' : [
    { 'Date' : '2001/01/01', },
    null,
    { 'Date' : '9999/12/31', },
  ],
}";

        [TestMethod]
        public void JsonWalk()
        {
            var pathList = new List<string>();
            var schemaList = new List<string>();
            var schemaListExpect = new List<string>() { 
// Code
"{  'type': 'string'}",
// Name
"{  'type': 'string',  'maxLength': 20}",
// ArrayString
"{  'item': [    {      'type': 'string'    }  ],  'type': 'array',  'format': 'Protect'}",
// ArrayString[0]
"{  'type': 'string'}",
// ArrayString[1]
"{  'type': 'string'}",
// ArrayInt
"{  'item': [    {      'type': 'number'    }  ],  'type': 'array'}",
// ArrayInt[0]
"{  'type': 'number'}",
// ArrayInt[1]
"{  'type': 'number'}",
// ArrayInt[2]
"{  'type': 'number'}",
// Object
"{  'type': 'object',  'properties': {    'Prop1': {      'type': 'string'    },    'Prop2': {      'type': 'string',      'format': 'Protect'    },    'Prop3': {      'type': 'object',      'properties': {        'Prop31': {          'type': 'string',          'format': 'Protect'        }      }    }  }}",
// Prop1
"{  'type': 'string'}",
// Prop2
"{  'type': 'string',  'format': 'Protect'}",
// Prop3
"{  'type': 'object',  'properties': {    'Prop31': {      'type': 'string',      'format': 'Protect'    }  }}",
// Prop31
"{  'type': 'string',  'format': 'Protect'}",
// ObjectArray
"{  'type': 'array',  'items': {    'properties': {      'Date': {        'type': 'string'      },      'ItemCode': {        'type': 'string',        'format': 'Protect'      },      'Value': {        'type': 'string'      }    }  }}",
// ObjectArray[0]
"{  'type': 'array',  'items': {    'properties': {      'Date': {        'type': 'string'      },      'ItemCode': {        'type': 'string',        'format': 'Protect'      },      'Value': {        'type': 'string'      }    }  }}",
// Date
"{  'type': 'string'}",
// ItemCode
"{  'type': 'string',  'format': 'Protect'}",
// Value
"{  'type': 'string'}",
// ObjectArray[1]
"{  'type': 'array',  'items': {    'properties': {      'Date': {        'type': 'string'      },      'ItemCode': {        'type': 'string',        'format': 'Protect'      },      'Value': {        'type': 'string'      }    }  }}",
// Date
"{  'type': 'string'}",
// ItemCode
"{  'type': 'string',  'format': 'Protect'}",
// Value
"{  'type': 'string'}",
// ObjectArray[2]
"{  'type': 'array',  'items': {    'properties': {      'Date': {        'type': 'string'      },      'ItemCode': {        'type': 'string',        'format': 'Protect'      },      'Value': {        'type': 'string'      }    }  }}",
// Date
"{  'type': 'string'}",
// ItemCode
"{  'type': 'string',  'format': 'Protect'}",
// Value
"{  'type': 'string'}", };
            var pathListExpect = new List<string>() { "Code", "Name", "ArrayString", "ArrayString[0]", "ArrayString[1]", "ArrayInt", "ArrayInt[0]", "ArrayInt[1]", "ArrayInt[2]", "Object", "Prop1", "Prop2", "Prop3", "Prop31", "ObjectArray", "ObjectArray[0]", "Date", "ItemCode", "Value", "ObjectArray[1]", "Date", "ItemCode", "Value", "ObjectArray[2]", "Date", "ItemCode", "Value" };
            var walk = new JsonWalk(json_string.ToJson()) { Schema = JSchema.Parse(schema_string) };
            walk.Walk((JSchema schema, bool isArray, string path, List<JToken> data) => {
                schemaList.Add(schema?.ToString().Replace("\r\n", "").Replace("\"", "'"));
                pathList.Add(path);
            });
            pathList.Count().Is(pathListExpect.Count());
            schemaListExpect.Count().Is(pathListExpect.Count());
            pathList.IsStructuralEqual(pathListExpect);
            schemaList.IsStructuralEqual(schemaListExpect);
        }
    }
}
