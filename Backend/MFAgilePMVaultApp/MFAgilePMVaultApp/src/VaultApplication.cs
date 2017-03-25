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
                    string input = "Hello from MFAgilePMVaultApp";

                    // Execute the extension method. Wrapping code to an extension method ensures transactionality for the vault operations.
                    string output = this.PermanentVault.ExtensionMethodOperations.ExecuteVaultExtensionMethod("GetObjectList", input);

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
        [VaultExtensionMethod("GetObjectList", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string GetObjectList(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;
            // env.Input = {"objectclass": "<Object_Class_Alias>"}

            JObject jo = JObject.Parse(@"{
                'ObjectClass': 'MF.OC.Product'
            }");


            // JObject o = JObject.Parse(env.Input);


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



            string ocAlias = (string)jo.SelectToken("ObjectClass");

            SearchConditions scs = new SearchConditions();
            scs.Add(0, MFSearchUtils.isObjectClass(v.ClassOperations.GetObjectClassIDByAlias(ocAlias)));

            ObjectSearchResults osr = MFSearchUtils.SearchForObjectsByConditions(v, scs);
            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();
            PropertyValuesOfMultipleObjects pvomo = v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVers);

            List<Product> plist = new List<Product>();

            foreach ( PropertyValues pvs in pvomo)
            {
                // Build a list with the objects as needed according to the DataModel
                switch (ocAlias)
                {
                    case "MF.OC.Product":
                        Product p = new Product();
                        p.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
                        plist.Add(p);
                        break;
                    default:
                        break;
                }
                
                   
            }

            // Serialize the product list
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(plist);
            return jsonString;

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