using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFiles.VAF;
using MFiles.VAF.Common;
using MFilesAPI;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace MFAgilePMVaultApp
{
    /// <summary>
    /// Simple configuration.
    /// </summary>
    public class Configuration
    {
        public string ALIAS_MF_OT_PERSON = "MF.OT.Person";
        public string ALIAS_MF_OT_FEATURE = "MF.OT.Feature";

        public string ALIAS_MF_OC_USER_STORY = "MF.OC.UserStory";
        public string ALIAS_MF_OC_TASK = "MF.OC.Task";

        public string ALIAS_MF_PD_FEATURE_ID = "MF.PD.FeatureId";
        public string ALIAS_MF_PD_TASK_ID = "MF.PD.TaskId";
        public string ALIAS_MF_PD_TASK_STATE = "MF.PD.TaskState";
        public string ALIAS_MF_PD_HOURS_REMAINING = "MF.PD.HoursRemaining";

        public string ALIAS_MF_PD_PERSON_NAME = "MF.PD.PersonName";
        public string ALIAS_MF_PD_FIRST_NAME = "MF.PD.FirstName";
        public string ALIAS_MF_PD_LAST_NAME = "MF.PD.LastName";

        public string ALIAS_MF_PD_DESCRIPTION = "MF.PD.Description";
        public string ALIAS_MF_PD_NAME_OR_TITLE = "MF.PD.NameOrTitle";

        public string ALIAS_MF_PD_USER_STORY_ID = "MF.PD.UserStoryID";
        public string ALIAS_MF_PD_USER_STORY_STATE = "MF.PD.UserStoryState";
        public string ALIAS_MF_PD_USER_STORY_POINTS = "MF.PD.UserStoryPoints";
        public string ALIAS_MF_PD_FEATURE = "MF.PD.Feature";
        public string ALIAS_MF_PD_SPRINT = "MF.PD.Sprint";
        public string ALIAS_MF_PD_RESPONSIBLE_PERSON = "MF.PD.ResponsiblePerson";

        public Dictionary<int, int> FeatureOrderDictionary;
        public Dictionary<int, int> UserStoryOrderDictionary;
        public Dictionary<int, int> TaskOrderDictionary;


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
        private Configuration config = new Configuration(){ TestClassID = "TestClassAlias" };

        /// <summary>
        /// The method, that is run when the vault goes online.
        /// </summary>
        protected override void StartApplication()
        {

            config.FeatureOrderDictionary = new Dictionary<int, int>();
            config.UserStoryOrderDictionary = new Dictionary<int, int>();
            config.TaskOrderDictionary = new Dictionary<int, int>();


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

            ObjectSearchResults osr = GetSubObjectsObjVers(v, jsonQuery);
            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();

            List<Product> plist = new List<Product>();

            foreach (ObjVer objVer in objVers)
            {
                PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

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

            ObjectSearchResults osr = GetSubObjectsObjVers(v, jsonQuery);
            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();

            List<Backlog> blist = new List<Backlog>();

            foreach (ObjVer objVer in objVers)
            {
                PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                // Build a list with the objects as needed according to the DataModel
                Backlog b = new Backlog();
                string NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
                blist.Add(b);

            }

            // Serialize the product list
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(blist);
            return jsonString;

        }


        private string MoveFeatureToBacklog(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;

            string jsonQuery = @"{
                'FeatureId': '1',
                'TargetBacklogId': '2',
                'AfterFeatureId': '0'
            }";

            return null;
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

            ObjectSearchResults osrUS = GetSubObjectsObjVers(v, jsonQuery);
            ObjVers objVersUS = osrUS.ObjectVersions.GetAsObjVers();

            PropertyValuesOfMultipleObjects pvomoUS =
                v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVersUS);

            List<Feature> fList= new List<Feature>();

            // Iterate simultaniously thorugh objVers and pvomo
            IEnumerator propValsEnum = pvomoUS.GetEnumerator();
            foreach (ObjVer objVerUS in objVersUS)
            {
                //PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                propValsEnum.MoveNext();
                PropertyValues pvsUS = propValsEnum.Current as PropertyValues;

                // Build a list with the objects as needed according to the DataModel
                UserStory u = new UserStory();
                u.InternalId = objVerUS.ID;
                u.UserStoryId = MFSearchUtils.getPropertyDisplayValue(
                    pvsUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_USER_STORY_ID));
                u.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvsUS, 0);
                u.Description = MFSearchUtils.getPropertyDisplayValue(
                    pvsUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_DESCRIPTION));
                u.UserStoryState = MFSearchUtils.getLookupPropertyAsInt(
                    pvsUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_USER_STORY_STATE));
                u.StoryPoints = Convert.ToInt16(MFSearchUtils.getPropertyDisplayValue(
                    pvsUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_USER_STORY_POINTS)));

                // Resolve responsible person
                int responsiblePersonId = MFSearchUtils.getLookupPropertyAsInt(
                    pvsUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_RESPONSIBLE_PERSON));

                SearchConditions scs = new SearchConditions();

                ObjID objIDUS = new ObjID();
                objIDUS.SetIDs(v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_PERSON), responsiblePersonId);
                PropertyValues pvPersonUS = v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDUS, true).Properties;

                Person p = new Person();
                p.InternalId = responsiblePersonId;
                p.PersonName = MFSearchUtils.getPropertyDisplayValue(
                    pvPersonUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_PERSON_NAME));
                p.FirstName = MFSearchUtils.getPropertyDisplayValue(
                    pvPersonUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FIRST_NAME));
                p.LastName = MFSearchUtils.getPropertyDisplayValue(
                    pvPersonUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_LAST_NAME));

                u.ResponsiblePerson = p;
                   
                // Resolve Task list (Task is a subtype of User story)


                // JObject o = JObject.Parse(env.Input);

                ObjectSearchResults osrTasks = GetSubObjectsObjVers(v, null, config.ALIAS_MF_OC_TASK, config.ALIAS_MF_OC_USER_STORY, u.InternalId);
                ObjVers objVersTasks = osrTasks.ObjectVersions.GetAsObjVers();

                PropertyValuesOfMultipleObjects pvomoTasks = v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVersTasks);

                List<Task> tList = new List<Task>();

                // Iterate simultaniously thorugh objVers and pvomo
                IEnumerator propValsEnumTask = pvomoTasks.GetEnumerator();
                foreach (ObjVer objVerTask in objVersTasks)
                {
                    propValsEnumTask.MoveNext();
                    PropertyValues pvsTask = propValsEnumTask.Current as PropertyValues;

                    // Build a list with the objects as needed according to the DataModel
                    Task t = new Task();
                    t.InternalId = objVerTask.ID;
                    t.TaskId = MFSearchUtils.getPropertyDisplayValue(
                        pvsTask, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_TASK_ID));
                    t.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvsTask, 0);
                    t.Description = MFSearchUtils.getPropertyDisplayValue(
                        pvsTask, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_DESCRIPTION));
                    t.TaskState = MFSearchUtils.getLookupPropertyAsInt(
                        pvsTask, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_TASK_STATE));

                    // Resolve responsible person
                    int taskResponsiblePersonId = MFSearchUtils.getLookupPropertyAsInt(
                        pvsTask, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_RESPONSIBLE_PERSON));

                    ObjID objIDTask = new ObjID();
                    objIDTask.SetIDs(v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_PERSON), taskResponsiblePersonId);
                    PropertyValues pvTaskPerson = v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDTask, true).Properties;

                    Person tp = new Person();
                    tp.PersonName = MFSearchUtils.getPropertyDisplayValue(
                        pvTaskPerson, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_PERSON_NAME));
                    tp.FirstName = MFSearchUtils.getPropertyDisplayValue(
                        pvTaskPerson, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FIRST_NAME));
                    tp.LastName = MFSearchUtils.getPropertyDisplayValue(
                        pvTaskPerson, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_LAST_NAME));

                    t.ResponsiblePerson = tp;

                    // Handle the task order
                    if(!config.TaskOrderDictionary.ContainsKey(t.InternalId))
                    {
                        t.Order = 1;
                        if (tList.Count > 0)
                        {
                            t.Order = tList.Max(x => x.Order) + 1;
                        }
                        config.TaskOrderDictionary.Add(t.InternalId, t.Order);
                    }
                    else
                    {
                        t.Order = config.TaskOrderDictionary[t.InternalId];
                    }

                    tList.Add(t);
                    // Reorder
                    tList = tList.OrderBy(x => x.Order).ToList();
                }

                u.TaskList = tList;

                // Resolve Feature
                int featureId = MFSearchUtils.getLookupPropertyAsInt(
                    pvsUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FEATURE));

                u.FeatureInternalId = featureId;

                // User stories will be returned as part of Feature hierarchy
                // Check if current feature already is in the return list
                // If so, add current UserStory to that Feature's UserStoryList
                // If not, create new Feature and add the UserStory to it

                Feature f = fList.Find(x => x.InternalId.Equals(featureId));

                bool addFeatureToList = false;
                if (f == null)
                {

                    ObjID objIDFeature = new ObjID();
                    objIDFeature.SetIDs(v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_FEATURE), featureId);
                    PropertyValues pvFeature = v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDFeature, true).Properties;

                    f = new Feature();
                    f.InternalId = featureId;
                    f.FeatureId = MFSearchUtils.getPropertyDisplayValue(
                        pvFeature, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FEATURE_ID));
                    f.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvFeature, 0);
                    f.Description = MFSearchUtils.getPropertyDisplayValue(
                        pvFeature, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_DESCRIPTION));
                    f.UserStoryList = new List<UserStory>();

                    addFeatureToList = true;
                }

                // Ordering of user stories
                if (!config.UserStoryOrderDictionary.ContainsKey(u.InternalId))
                {
                    u.Order = 1;
                    if (f.UserStoryList.Count > 0)
                    {
                        u.Order = f.UserStoryList.Max(x => u.Order) + 1;
                    }
                    config.UserStoryOrderDictionary.Add(u.InternalId, u.Order);
                }
                else
                {
                    u.Order = config.UserStoryOrderDictionary[u.InternalId];
                }

                f.UserStoryList.Add(u);
                f.UserStoryList = f.UserStoryList.OrderBy(x => x.Order).ToList();

                // Add the new feature to the list
                if (addFeatureToList)
                {
                    if (!config.FeatureOrderDictionary.ContainsKey(f.InternalId))
                    {
                        f.Order = 1;
                        if (fList.Count > 0)
                        {
                            f.Order = fList.Max(x => x.Order) + 1;
                        }
                        config.FeatureOrderDictionary.Add(f.InternalId, f.Order);
                    }
                    else
                    {
                        f.Order = config.FeatureOrderDictionary[f.InternalId];
                    }

                    fList.Add(f);
                }

            }

            // Reorder the Feature list
            fList = fList.OrderBy(x => x.Order).ToList();



            // Serialize the product list
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(fList);
            return jsonString;

        }

        // specify either jsonString or the rest of the otptional params
        private ObjectSearchResults GetSubObjectsObjVers(Vault v, string jsonQuery = null, string ocAlias = null, string oocAlias = null, int ownerId = -1)
        {
            string oClassAlias = ocAlias;
            string ooClassAlias = oocAlias;
            object ownerObjectId = ownerId;

            if (jsonQuery != null)
            {
                JObject jo = JObject.Parse(jsonQuery);
                oClassAlias = (string)jo.SelectToken("ObjectClass");
                ooClassAlias = (string)jo.SelectToken("ObjectOwnerClass");
                ownerObjectId = Convert.ToInt32(jo.SelectToken("OwnerId"));
            }

            SearchConditions scs = new SearchConditions();

            int objClassId = v.ClassOperations.GetObjectClassIDByAlias(oClassAlias);
            scs.Add(-1, MFSearchUtils.isObjectClass(objClassId));

            // Some searches do not require owner filter
            if (oClassAlias != null && ooClassAlias != null)
            {
                int objOwnerClassId = v.ClassOperations.GetObjectClassIDByAlias(ooClassAlias);
                ObjectClass oc = v.ClassOperations.GetObjectClass(objOwnerClassId);
                ObjType ot = v.ObjectTypeOperations.GetObjectType(oc.ObjectType);
                int ownerPropDef = ot.OwnerPropertyDef;
                scs.Add(-1, MFSearchUtils.isOwner(ownerPropDef, ownerObjectId));
            }

            ObjectSearchResults osr = MFSearchUtils.SearchForObjectsByConditions(v, scs);

            return osr;
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