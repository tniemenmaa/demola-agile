using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFiles.VAF;
using MFiles.VAF.Common;
using MFilesAPI;
using Newtonsoft.Json.Linq;

namespace MFAgilePMVaultApp
{
    /// <summary>
    /// Simple configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Reference to a test class.
        /// </summary>
        [MFClass(Required = false)]
        public MFIdentifier TestClassID = "FailAlias";
    }

    /// <summary>
    /// Simple vault application to demonstrate VAF.
    /// </summary>
    public class VaultApplication : VaultApplicationBase
    {
        /// <summary>
        /// Simple configuration member. MFConfiguration-attribute will cause this member to be loaded from the named value storage from the given namespace and key.
        /// Here we override the default alias of the Configuration class with a default of the config member.
        /// The default will be written in the named value storage, if the value is missing.
        /// Use Named Value Manager to change the configurations in the named value storage.
        /// </summary>
        [MFConfiguration("MFAgilePMVaultApp", "config")]
        private Configuration config = new Configuration() { TestClassID = "TestClassAlias" };

        /// <summary>
        /// The method, that is run when the vault goes online.
        /// </summary>
        protected override void StartApplication()
        {
            // Start writing extension method output to the event log every ten seconds. The background operation will continue until the vault goes offline.
            this.BackgroundOperations.StartRecurringBackgroundOperation("Recurring Hello World Operation", TimeSpan.FromSeconds(10), () =>
             {
                    // Prepare input for the extension method.
                    string input = "{'id': '1'}";

                    // Execute the extension method. Wrapping code to an extension method ensures transactionality for the vault operations.
                    string output = this.PermanentVault.ExtensionMethodOperations.ExecuteVaultExtensionMethod("GetUserStoriesByFeatureWithTasks", input);

                    // Report extension method output to event log.
                    SysUtils.ReportInfoToEventLog(output);
             });
        }

        /// <summary>
        /// A vault extension method, that will be installed to the vault with the application.
        /// The vault extension method can be called through the API.
        /// </summary>
        /// <param name="env">The event handler environment for the method.</param>
        /// <returns>The output string to the caller.</returns>
        [VaultExtensionMethod("GetProducts", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string GetProducts(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;
 
            /* JSON parsing example
             http://www.newtonsoft.com/json/help/html/SelectToken.htm

             1JObject o = JObject.Parse(@"{
             2  'Stores': [
             3    'Lambton Quay',
             4    'Willis Street'
             5  ],
             6  'Manufacturers': [
             7    {
             8      'Name': 'Acme Co',
             9      'Products': [
            10        {
            11          'Name': 'Anvil',
            12          'Price': 50
            13        }
            14      ]
            15    },
            16    {
            17      'Name': 'Contoso',
            18      'Products': [
            19        {
            20          'Name': 'Elbow Grease',
            21          'Price': 99.95
            22        },
            23        {
            24          'Name': 'Headlight Fluid',
            25          'Price': 4
            26        }
            27      ]
            28    }
            29  ]
            30}");
            31
            32string name = (string)o.SelectToken("Manufacturers[0].Name");
            33// Acme Co
            34
            35decimal productPrice = (decimal)o.SelectToken("Manufacturers[0].Products[0].Price");
            36// 50
            37
            38string productName = (string)o.SelectToken("Manufacturers[1].Products[0].Name");
            39// Elbow Grease

            */


            string jsonQuery = @"{
                'ObjectClass': 'MF.OC.Product'
            }";


            // JObject o = JObject.Parse(env.Input);

            PropertyValuesOfMultipleObjects pvomo = GetPropertyValuesOfObjects(v, jsonQuery);

            List<Product> plist = new List<Product>();

            foreach ( PropertyValues pvs in pvomo)
            {
                // Build a list with the objects as needed according to the DataModel
                Product p = new Product();
                p.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
                plist.Add(p);              
            }

            // Serialize the product list
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(plist);
            return jsonString;

        }


        /// <summary>
        /// A vault extension method, that will be installed to the vault with the application.
        /// The vault extension method can be called through the API.
        /// </summary>
        /// <param name="env">The event handler environment for the method.</param>
        /// <returns>The output string to the caller.</returns>
        [VaultExtensionMethod("GetBacklogs", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string GetBacklogs(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;

            string jsonQuery = @"{
                'ObjectClass': 'MF.OC.Backlog',
                'ObjectOwnerClass': 'MF.OC.Product',
                'OwnerId': '1'
            }";


            // JObject o = JObject.Parse(env.Input);

            PropertyValuesOfMultipleObjects pvomo = GetPropertyValuesOfObjects(v, jsonQuery);

            List<Product> plist = new List<Product>();

            foreach (PropertyValues pvs in pvomo)
            {
                // Build a list with the objects as needed according to the DataModel
                //Product p = new Product();
                string NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
                //plist.Add(p);

 
            }

            // Serialize the product list
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(plist);
            return jsonString;

        }


        /// <summary>
        /// A vault extension method, that will be installed to the vault with the application.
        /// The vault extension method can be called through the API.
        /// </summary>
        /// <param name="env">The event handler environment for the method.</param>
        /// <returns>The output string to the caller.</returns>
        [VaultExtensionMethod("GetUserStoriesByFeatureWithTasks", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string GetUserStoriesByFeatureWithTasks(EventHandlerEnvironment env)
        {

            Vault v = env.Vault;
            
            string jsonQuery = @"{
                'ObjectClass': 'MF.OC.UserStory',
                'ObjectOwnerClass': 'MF.OC.Backlog',
                'OwnerId': '1'
            }";


            // JObject o = JObject.Parse(env.Input);

            PropertyValuesOfMultipleObjects pvomo = GetPropertyValuesOfObjects(v, jsonQuery);

            List<Product> plist = new List<Product>();

            foreach (PropertyValues pvs in pvomo)
            {
                // Build a list with the objects as needed according to the DataModel
                //Product p = new Product();
                string NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
                //plist.Add(p);


            }

            // Serialize the product list
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(plist);
            return jsonString;

        }


        private PropertyValuesOfMultipleObjects GetPropertyValuesOfObjects(Vault v, string jsonQuery)
        {
            JObject jo = JObject.Parse(jsonQuery);
            string ocAlias = (string)jo.SelectToken("ObjectClass");
            string oocAlias = (string)jo.SelectToken("ObjectOwnerClass");
            object ownerId = Convert.ToInt32(jo.SelectToken("OwnerId"));

            SearchConditions scs = new SearchConditions();

            int objClassId = v.ClassOperations.GetObjectClassIDByAlias(ocAlias);
            scs.Add(-1, MFSearchUtils.isObjectClass(objClassId));

            // Some searches do not require owner filter
            if (ocAlias != null && oocAlias != null)
            {
                int objOwnerClassId = v.ClassOperations.GetObjectClassIDByAlias(oocAlias);
                ObjectClass oc = v.ClassOperations.GetObjectClass(objOwnerClassId);
                ObjType ot = v.ObjectTypeOperations.GetObjectType(oc.ObjectType);
                int ownerPropDef = ot.OwnerPropertyDef;
                scs.Add(-1, MFSearchUtils.isOwner(ownerPropDef, ownerId));
            }

            ObjectSearchResults osr = MFSearchUtils.SearchForObjectsByConditions(v, scs);


            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();
            PropertyValuesOfMultipleObjects pvomo = v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVers);

            return pvomo;
        }

        /*
        Function SearchForObjectsByClass(ByVal MyClassID)


            Dim oSearchConditions: Set oSearchConditions = CreateObject("MFilesAPI.SearchConditionS")

            ' Create search conditions.
            Dim oSearchCondition: Set oSearchCondition = CreateObject("MFilesAPI.SearchCondition")
            oSearchCondition.ConditionType = MFConditionType.MFConditionTypeEqual
            oSearchCondition.Expression.DataPropertyValuePropertyDef = MFBuiltInPropertyDef.MFBuiltInPropertyDefClass
            oSearchCondition.TypedValue.SetValue MFDataType.MFDatatypeLookup, MyClassID

            oSearchConditions.Add -1, oSearchCondition

            Dim oSearchCondition1: Set oSearchCondition1 = CreateObject("MFilesAPI.SearchCondition")
            oSearchCondition1.ConditionType = MFConditionType.MFConditionTypeEqual
            oSearchCondition1.Expression.DataStatusValueType = MFStatusTypeDeleted
            oSearchCondition1.TypedValue.SetValue MFDataType.MFDatatypeBoolean, False

            oSearchConditions.Add -1, oSearchCondition1

            ' Invoke the search operation.
            Dim oObjectVersions: Set oObjectVersions = Vault.ObjectSearchOperations.SearchForObjectsByConditionsEx(_
                    oSearchConditions, MFSearchFlagNone, False, 10000, 60)


            Set SearchForObjectsByClass = oObjectVersions

        End Function
        */
    }
}