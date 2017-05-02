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
        public string ALIAS_MF_OC_BACKLOG = "MF.OC.Backlog";

        public string ALIAS_MF_PD_BACKLOG_TYPE = "MF.PD.BacklogType";

        public string ALIAS_MF_PD_FEATURE_ID = "MF.PD.FeatureId";
        public string ALIAS_MF_PD_TASK_ID = "MF.PD.TaskId";
        public string ALIAS_MF_PD_TASK_STATE = "MF.PD.TaskState";
        public string ALIAS_MF_PD_HOURS_REMAINING = "MF.PD.HoursRemaining";

        public string ALIAS_MF_PD_PERSON_NAME = "MF.PD.PersonName";
        public string ALIAS_MF_PD_FIRST_NAME = "MF.PD.FirstName";
        public string ALIAS_MF_PD_LAST_NAME = "MF.PD.LastName";

        public string ALIAS_MF_PD_DESCRIPTION = "MF.PD.Description";
        public string ALIAS_MF_PD_NAME_OR_TITLE = "MF.PD.NameOrTitle";
        public string ALIAS_MF_PD_START_DATE = "MF.PD.StartDate";
        public string ALIAS_MF_PD_END_DATE = "MF.PD.EndDate";

        public string ALIAS_MF_PD_USER_STORY_ID = "MF.PD.UserStoryID";
        public string ALIAS_MF_PD_USER_STORY_STATE = "MF.PD.UserStoryState";
        public string ALIAS_MF_PD_USER_STORY_POINTS = "MF.PD.UserStoryPoints";
        public string ALIAS_MF_PD_FEATURE = "MF.PD.Feature";
        public string ALIAS_MF_PD_SPRINT = "MF.PD.Sprint";
        public string ALIAS_MF_PD_RESPONSIBLE_PERSON = "MF.PD.ResponsiblePerson";

        public string ALIAS_MF_PD_TEAM_LEADER = "MF.PD.TeamLeader";
        public string ALIAS_MF_PD_TEAM_MEMBERS = "MF.PD.TeamMembers";

        public Dictionary<int, Dictionary<int, int>> ProductDictionary;
        public Dictionary<int, Dictionary<int, int>> BacklogDictionary;
        public Dictionary<int, Dictionary<int, int>> BacklogFeatureDictionary;
        public Dictionary<int, Dictionary<int, int>> BacklogUserStoryDictionary;

        public Dictionary<int, Dictionary<int, int>> TeamDictionary;
        public Dictionary<int, Dictionary<int, int>> SprintDictionary;
        public Dictionary<int, Dictionary<int, int>> SprintFeatureDictionary;
        public Dictionary<int, Dictionary<int, int>> SprintUserStoryDictionary;



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
            config.ProductDictionary = new Dictionary<int, Dictionary<int, int>>();
            config.BacklogDictionary = new Dictionary<int, Dictionary<int, int>>();
            config.BacklogFeatureDictionary = new Dictionary<int, Dictionary<int, int>>();
            config.BacklogUserStoryDictionary = new Dictionary<int, Dictionary<int, int>>();

            config.TeamDictionary = new Dictionary<int, Dictionary<int, int>>();
            config.SprintDictionary = new Dictionary<int, Dictionary<int, int>>();
            config.SprintFeatureDictionary = new Dictionary<int, Dictionary<int, int>>();
            config.SprintUserStoryDictionary = new Dictionary<int, Dictionary<int, int>>();
            /*
            // Start writing extension method output to the event log every ten seconds. The background operation will continue until the vault goes offline.
            this.BackgroundOperations.StartRecurringBackgroundOperation("Recurring Hello World Operation", TimeSpan.FromSeconds(10), () =>
            {

                // Prepare input for the extension method.
                string input = "{'id': '1'}";

                // Execute the extension method. Wrapping code to an extension method ensures transactionality for the vault operations.
                string output = this.PermanentVault.ExtensionMethodOperations.ExecuteVaultExtensionMethod("MyAPIBacklogTests", input);

                // Report extension method output to event log.
                SysUtils.ReportInfoToEventLog(output);
             });*/
        }

        /// <summary>
        /// A Test method for Sprint related API calls
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        [VaultExtensionMethod("MyAPISprintTests", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string MyAPISprintTests(EventHandlerEnvironment env)
        {

            List<Feature> returnFeatureList;

            // 1. GetTeams
            env.Input = @"{
                'ObjectClass': 'MF.OC.Team'
            }";
            string myTeamList = GetTeams(env);
            var TResultTeam = new { Result = new List<Team>() };
            List<Team> teamList = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(myTeamList, TResultTeam).Result;



            // 2. Get Sprints

            foreach (Team team in teamList)
            {
                string jsonQuery = @"
                    'ObjectClass': 'MF.OC.Sprint',
                    'ObjectOwnerClass': 'MF.OC.Team',
                    'OwnerId': '{0}'
                ";
                env.Input = "{" + String.Format(jsonQuery, team.InternalId) + "}";
                string mySprintList = GetSprints(env);

                var TResultSprint = new { Result = new List<Sprint>() };
                List<Sprint> sprintList = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(
                    mySprintList, TResultSprint).Result;


                // 3. Get UserStories By Feature With Tasks
                jsonQuery = @"
                    'ObjectClass': 'MF.OC.UserStory',
                    'PropertyAlias': 'MF.PD.Sprint',
                    'PropertyValue': '{0}'
                ";

                foreach (Sprint sprint in sprintList)
                {
                    env.Input = "{" + String.Format(jsonQuery, sprint.InternalId) + "}";
                    string myUserStoriesByFeatureList = GetUserStoriesByFeatureWithTasksForSprint(env);

                    var TResultFeature = new { Result = new List<Feature>() };
                    List<Feature> featureList = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType
                        (myUserStoriesByFeatureList, TResultFeature).Result;
                    if(featureList.Count > 0)
                    {
                        returnFeatureList = featureList;
                        goto end;
                    }
                }
            }

        end:
            return Convert.ToString(true);
        }



        /// <summary>
        ///  A Test method to test API calls
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        [VaultExtensionMethod("MyAPIBacklogTests", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string MyAPIBacklogTests(EventHandlerEnvironment env)
        {
            // 1. GetProducts
            env.Input = @"{
                'ObjectClass': 'MF.OC.Product'
            }";
            string myProductList = GetProducts(env);

            var TResultProduct = new { Result = new List<Product>() };
            List<Product> pList = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(myProductList, TResultProduct).Result;


            // 2. Get Backlogs
            string jsonQuery = @"
                'ObjectClass': 'MF.OC.Backlog',
                'ObjectOwnerClass': 'MF.OC.Product',
                'OwnerId': '{0}'
            ";
            env.Input = "{" + String.Format(jsonQuery, pList[0].InternalId) + "}";
            string myBacklogList = GetBacklogs(env);

            var TResultBacklog = new { Result = new List<Backlog>() };
            List<Backlog> bList = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(myBacklogList, TResultBacklog).Result;


            // 3. Get UserStories By Feature With Tasks
            jsonQuery = @"
                'ObjectClass': 'MF.OC.UserStory',
                'ObjectOwnerClass': 'MF.OC.Backlog',
                'OwnerId': '{0}'
            ";
            env.Input = "{" + String.Format(jsonQuery, bList[0].InternalId) + "}";

            string myUserStoriesByFeatureList = GetUserStoriesByFeatureWithTasks(env);
            var TResultUserStories = new { Result = new List<Feature>() };
            List<Feature> fList = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(myUserStoriesByFeatureList, TResultUserStories).Result;


            
            // 4. MoveFeatureToBacklog
            jsonQuery = @"{
                'FeatureId': '2',
                'SourceBacklogId': '2',
                'TargetBacklogId': '3',
                'AfterFeatureId': '0'
            }";

            env.Input = jsonQuery;
            string status = MoveFeatureToBacklog(env);

            // 4.1. Get UserStories from Source blog
            jsonQuery = @"{
                'ObjectClass': 'MF.OC.UserStory',
                'ObjectOwnerClass': 'MF.OC.Backlog',
                'OwnerId': '1'
            }";
            env.Input = jsonQuery;
            myUserStoriesByFeatureList = GetUserStoriesByFeatureWithTasks(env);

            // 4.2. Get UserStories from Target blog
            jsonQuery = @"{
                'ObjectClass': 'MF.OC.UserStory',
                'ObjectOwnerClass': 'MF.OC.Backlog',
                'OwnerId': '2'
            }";
            env.Input = jsonQuery;
            myUserStoriesByFeatureList = GetUserStoriesByFeatureWithTasks(env);

            // 4.3. MoveFeatureToBacklog to original state
            jsonQuery = @"{
                'FeatureId': '2',
                'SourceBacklogId': '3',
                'TargetBacklogId': '2',
                'AfterFeatureId': '0'
            }";
            env.Input = jsonQuery;
            status = MoveFeatureToBacklog(env);

            // 4.4. Get UserStories from Source blog
            jsonQuery = @"{
                'ObjectClass': 'MF.OC.UserStory',
                'ObjectOwnerClass': 'MF.OC.Backlog',
                'OwnerId': '2'
            }";
            env.Input = jsonQuery;
            myUserStoriesByFeatureList = GetUserStoriesByFeatureWithTasks(env);

            // 4.5. Get UserStories from Target blog
            jsonQuery = @"{
                'ObjectClass': 'MF.OC.UserStory',
                'ObjectOwnerClass': 'MF.OC.Backlog',
                'OwnerId': '1'
            }";
            env.Input = jsonQuery;
            myUserStoriesByFeatureList = GetUserStoriesByFeatureWithTasks(env);
            

            return Convert.ToString(true);
        }



        private Random _rnd = new Random();
        public bool RandomSuccess(double probability)
        {
            return _rnd.NextDouble() < probability;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        [VaultExtensionMethod("GetTeams", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string GetTeams(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;

            ObjectSearchResults osr = GetSubObjectsAsObjVers(v, env.Input);
            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();

            List<Team> tList = new List<Team>();

            foreach (ObjVer objVer in objVers)
            {
                PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                // Build a list with the objects as needed according to the DataModel
                Team team = new Team();
                team.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
                team.InternalId = objVer.ID;

                // Resolve Team Leader
                int teamLeaderId = MFSearchUtils.getLookupPropertyAsInt(
                    pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_TEAM_LEADER));

                ObjID objIDPerson = new ObjID();
                objIDPerson.SetIDs(v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_PERSON), teamLeaderId);

                try
                {
                    PropertyValues pvPerson = v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDPerson, true).Properties;
                    team.TeamLeader = GetPersonData(v, teamLeaderId, pvPerson);
                }
                catch (Exception e)
                {
                    // In case responsible person is not set
                }


                // Resolve team members
                var membersIdList = MFSearchUtils.getMultiSelectLookupPropertyAsIntList(
                    pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_TEAM_MEMBERS));

                List<Person> teamMembers = new List<Person>();

                foreach (Lookup memberLookup in membersIdList)
                {
                    int memberId = memberLookup.Item;
                    objIDPerson = new ObjID();
                    objIDPerson.SetIDs(v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_PERSON), memberId);

                    try
                    {
                        PropertyValues pvPerson = v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDPerson, true).Properties;
                        Person pMember = GetPersonData(v, memberId, pvPerson);
                        teamMembers.Add(pMember);
                    }
                    catch (Exception e)
                    {
                        // In case responsible person is not set
                    }


                }
                team.TeamMembers = teamMembers;

                tList.Add(team);
            }

            // Serialize the product list
            var container = new { Result = tList };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(container);
            return jsonString;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        [VaultExtensionMethod("GetSprints", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string GetSprints(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;

            /*string jsonQuery = @"{
                'ObjectClass': 'MF.OC.Backlog',
                'ObjectOwnerClass': 'MF.OC.Product',
                'OwnerId': '1'
            }";*/

            JObject o = JObject.Parse(env.Input);
            int teamInternalId = Convert.ToInt32(o.SelectToken("OwnerId"));


            // TODO: Move this code to Team creation
            // Order Dictionary for Sprints is Team specific
            // First time a Team is encountered we have to create a Sprint order dictionary for it

            Dictionary<int, int> sprintOrderDictionary;

            if (!config.TeamDictionary.ContainsKey(teamInternalId))
            {
                sprintOrderDictionary = new Dictionary<int, int>();
                config.TeamDictionary.Add(teamInternalId, sprintOrderDictionary);
            }
            else
            {
                sprintOrderDictionary = config.TeamDictionary[teamInternalId];
            }

            // JObject o = JObject.Parse(env.Input);

            ObjectSearchResults osr = GetSubObjectsAsObjVers(v, env.Input);
            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();

            List<Sprint> sList = new List<Sprint>();

            foreach (ObjVer objVer in objVers)
            {
                PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                Sprint sprint = GetSprintData(v, objVer.ID, pvs);

                // Handle the sprint order
                if (!sprintOrderDictionary.ContainsKey(sprint.InternalId))
                {
                    sprint.Order = 1;
                    if (sList.Count > 0)
                    {
                        sprint.Order = sList.Max(x => x.Order) + 1;
                    }
                    sprintOrderDictionary.Add(sprint.InternalId, sprint.Order);
                }
                else
                {
                    sprint.Order = sprintOrderDictionary[sprint.InternalId];
                }


                sList.Add(sprint);

            }

            // Serialize the product list
            var container = new { Result = sList };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(container);
            return jsonString;

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
 
            ObjectSearchResults osr = GetSubObjectsAsObjVers(v, env.Input);
            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();

            List<Product> plist = new List<Product>();

            foreach (ObjVer objVer in objVers)
            {
                PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                // Build a list with the objects as needed according to the DataModel
                Product p = new Product();
                p.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
                p.InternalId = objVer.ID;
                plist.Add(p);              
            }

            // Serialize the product list
            var container = new { Result = plist };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(container);
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

            /*string jsonQuery = @"{
                'ObjectClass': 'MF.OC.Backlog',
                'ObjectOwnerClass': 'MF.OC.Product',
                'OwnerId': '1'
            }";*/

            JObject o = JObject.Parse(env.Input);
            int productInternalId = Convert.ToInt32(o.SelectToken("OwnerId"));


            // TODO: Move this code to Product creation
            // Order Dictionary for Backlogs is Product specific
            // First time a Product is encountered we have to create a Backlog order dictionary for it

            Dictionary<int, int> backlogOrderDictionary;

            if (!config.ProductDictionary.ContainsKey(productInternalId))
            {
                backlogOrderDictionary = new Dictionary<int, int>();
                config.ProductDictionary.Add(productInternalId, backlogOrderDictionary);
            }
            else
            {
                backlogOrderDictionary = config.ProductDictionary[productInternalId];
            }

            // JObject o = JObject.Parse(env.Input);

            ObjectSearchResults osr = GetSubObjectsAsObjVers(v, env.Input);
            ObjVers objVers = osr.ObjectVersions.GetAsObjVers();

            List<Backlog> bList = new List<Backlog>();

            foreach (ObjVer objVer in objVers)
            {
                PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                // Build a list with the objects as needed according to the DataModel

                // Construct Backlog object
                Backlog backlog = GetBacklogData(v, objVer.ID, pvs);

                // Handle the backlog order
                if (!backlogOrderDictionary.ContainsKey(backlog.InternalId))
                {
                    backlog.Order = 1;
                    if (bList.Count > 0)
                    {
                        backlog.Order = bList.Max(x => x.Order) + 1;
                    }
                    backlogOrderDictionary.Add(backlog.InternalId, backlog.Order);
                }
                else
                {
                    backlog.Order = backlogOrderDictionary[backlog.InternalId];
                }


                bList.Add(backlog);

            }

            // Serialize the product list
            var container = new { Result = bList };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(container);
            return jsonString;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        [VaultExtensionMethod("MoveFeatureToBacklog_", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string MoveFeatureToBacklog_(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };

            Vault v = env.Vault;

            JObject o = JObject.Parse(env.Input);
            int targetBlogInternalId = Convert.ToInt32(o.SelectToken("TargetBacklogId"));

            ObjectSearchResults osrUS = GetUserStoriesForFeatureAsObjVers(v, env.Input, config.ALIAS_MF_OC_USER_STORY, config.ALIAS_MF_OC_BACKLOG);
            ObjVers objVersUS = osrUS.ObjectVersions.GetAsObjVers();

            PropertyValuesOfMultipleObjects pvomoUS =
                v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVersUS);

            // Iterate simultaniously thorugh objVers and pvomo
            IEnumerator propValsEnum = pvomoUS.GetEnumerator();
            foreach (ObjVer objVerUS in objVersUS)
            {
                propValsEnum.MoveNext();
                PropertyValues pvsUS = propValsEnum.Current as PropertyValues;

                // Set the target backlog as the new owner backlog

                // 
                int objOwnerClassId = v.ClassOperations.GetObjectClassIDByAlias(config.ALIAS_MF_OC_BACKLOG);
                ObjectClass oc = v.ClassOperations.GetObjectClass(objOwnerClassId);
                ObjType ot = v.ObjectTypeOperations.GetObjectType(oc.ObjectType);
                int ownerPropDef = ot.OwnerPropertyDef;

                PropertyValue pvOwnerBacklog = new PropertyValue();
                pvOwnerBacklog.PropertyDef = ownerPropDef;
                pvOwnerBacklog.TypedValue.SetValue(MFDataType.MFDatatypeLookup, targetBlogInternalId);

                bool isCheckedOut = v.ObjectOperations.GetObjectInfo(objVerUS, true, true).ObjectCheckedOut;
                if(!isCheckedOut)
                {
                    ObjectVersion objVersionUSCheckedOut = v.ObjectOperations.CheckOut(objVerUS.ObjID);
                    v.ObjectPropertyOperations.SetProperty(objVersionUSCheckedOut.ObjVer, pvOwnerBacklog);
                    v.ObjectOperations.CheckIn(objVersionUSCheckedOut.ObjVer);
                }
                else
                {
                    var message = "Object is checked out: " + objVerUS.ID;
                    result = new { Result = "FAILED", Message = message };
                }

                // Remove feature order from current backlog dictionary and
                // update feature order in the target backlog dictionary
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(result);

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
            List<Feature> fList = GetFeaturesHierarchyForBacklog(env);

            // Serialize the feature (with user stories) list
            var container = new { Result = fList };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(container);

            return jsonString;
        }

        [VaultExtensionMethod("GetUserStoriesByFeatureWithTasksForSprint", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string GetUserStoriesByFeatureWithTasksForSprint(EventHandlerEnvironment env)
        {
            List<Feature> fList = GetFeaturesHierarchyForSprint(env);

            // Serialize the product list
            var container = new { Result = fList };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(container);

            return jsonString;
        }


        /// <summary>
        /// Fetches the whole features hierarchy in a backlog
        /// with User stories and Tasks 
        /// </summary>
        /// <returns></returns>
        private List<Feature> GetFeaturesHierarchyForBacklog(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;

            /* JSON for features in a backlog
            string jsonQuery = @"{
                'ObjectClass': 'MF.OC.UserStory',
                'ObjectOwnerClass': 'MF.OC.Backlog',
                'OwnerId': '1'
            }";
            */

            JObject o = JObject.Parse(env.Input);
            int backlogInternalId = Convert.ToInt32(o.SelectToken("OwnerId"));

            ObjectSearchResults osrUS = GetSubObjectsAsObjVers(v, env.Input);
            ObjVers objVersUS = osrUS.ObjectVersions.GetAsObjVers();

            PropertyValuesOfMultipleObjects pvomoUS =
                v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVersUS);

            List<Feature> fList = new List<Feature>();

            // Iterate simultaniously thorugh objVers and pvomo
            IEnumerator propValsEnum = pvomoUS.GetEnumerator();
            foreach (ObjVer objVerUS in objVersUS)
            {
                //PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                propValsEnum.MoveNext();
                PropertyValues pvsUS = propValsEnum.Current as PropertyValues;

                // Build a list with the objects as needed according to the DataModel
                UserStory userStory = GetUserStoryData(v, objVerUS.ID, pvsUS); 

                // User stories will be returned as part of Feature hierarchy
                // Check if current feature already is in the return list
                // If so, add current UserStory to that Feature's UserStoryList
                // If not, create new Feature and add the UserStory to it

                Feature f = fList.Find(x => x.InternalId.Equals(userStory.FeatureInternalId));

                bool addFeatureToList = false;
                if (f == null)
                {

                    ObjID objIDFeature = new ObjID();
                    objIDFeature.SetIDs(
                        v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_FEATURE),
                        userStory.FeatureInternalId);
                    PropertyValues pvFeature = 
                        v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDFeature, true).Properties;

                    f = GetFeatureData(v, userStory.FeatureInternalId, pvFeature);

                    addFeatureToList = true;
                }

                // TODO: Move this code to Feature creation
                // Order Dictionary for User Stories is Feature specific
                // First time a Feature is encountered we have to create a User Story order dictionary for it

                Dictionary<int, int> userStoryOrderDictionary;

                if (!config.BacklogFeatureDictionary.ContainsKey(f.InternalId))
                {
                    userStoryOrderDictionary = new Dictionary<int, int>();
                    config.BacklogFeatureDictionary.Add(f.InternalId, userStoryOrderDictionary);
                }
                else
                {
                    userStoryOrderDictionary = config.BacklogFeatureDictionary[f.InternalId];
                }

                // Ordering of user stories
                if (!userStoryOrderDictionary.ContainsKey(userStory.InternalId))
                {
                    userStory.Order = 1;
                    if (f.UserStoryList.Count > 0)
                    {
                        userStory.Order = f.UserStoryList.Max(x => x.Order) + 1;
                    }
                    userStoryOrderDictionary.Add(userStory.InternalId, userStory.Order);
                }
                else
                {
                    userStory.Order = userStoryOrderDictionary[userStory.InternalId];
                }

                f.UserStoryList.Add(userStory);
                f.UserStoryList = f.UserStoryList.OrderBy(x => x.Order).ToList();

                // Add the new feature to the list
                if (addFeatureToList)
                {
                    // TODO: Move this code to Blog creation
                    // Order Dictionary for Features is Blog specific
                    // First time a Blog is encountered we have to create a User Story order dictionary for it

                    Dictionary<int, int> featureOrderDictionary;

                    if (!config.BacklogDictionary.ContainsKey(backlogInternalId))
                    {
                        featureOrderDictionary = new Dictionary<int, int>();
                        config.BacklogDictionary.Add(backlogInternalId, featureOrderDictionary);
                    }
                    else
                    {
                        featureOrderDictionary = config.BacklogDictionary[backlogInternalId];
                    }

                    // Handle the feature order
                    if (!featureOrderDictionary.ContainsKey(f.InternalId))
                    {
                        f.Order = 1;
                        if (fList.Count > 0)
                        {
                            f.Order = fList.Max(x => x.Order) + 1;
                        }
                        featureOrderDictionary.Add(f.InternalId, f.Order);
                    }
                    else
                    {
                        f.Order = featureOrderDictionary[f.InternalId];
                    }

                    fList.Add(f);
                }

            }

            // Reorder the Feature list
            fList = fList.OrderBy(x => x.Order).ToList();

            return fList;
        }


        /// <summary>
        /// Fetches the whole features hierarchy in a backlog
        /// with User stories and Tasks 
        /// </summary>
        /// <returns></returns>
        private List<Feature> GetFeaturesHierarchyForSprint(EventHandlerEnvironment env)
        {
            Vault v = env.Vault;
            /* JSON for features in a sprint TBD
            string jsonQuery = @"{
                'ObjectClass': 'MF.OC.UserStory',
                'PropertyAlias' : 'MF.PD.Sprint'
                'PropertyValue': '1'
            }";
            */

            JObject o = JObject.Parse(env.Input);
            int sprintInternalId = Convert.ToInt32(o.SelectToken("PropertyValue"));

            ObjectSearchResults osrUS = GetObjectsWithPropertyAsObjVers(v, env.Input);
            ObjVers objVersUS = osrUS.ObjectVersions.GetAsObjVers();

            PropertyValuesOfMultipleObjects pvomoUS =
                v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVersUS);

            List<Feature> fList = new List<Feature>();

            // Iterate simultaniously thorugh objVers and pvomo
            IEnumerator propValsEnum = pvomoUS.GetEnumerator();
            foreach (ObjVer objVerUS in objVersUS)
            {
                //PropertyValues pvs = v.ObjectPropertyOperations.GetProperties(objVer);

                propValsEnum.MoveNext();
                PropertyValues pvsUS = propValsEnum.Current as PropertyValues;

                // Build a list with the objects as needed according to the DataModel
                UserStory userStory = GetUserStoryData(v, objVerUS.ID, pvsUS);

                // User stories will be returned as part of Feature hierarchy
                // Check if current feature already is in the return list
                // If so, add current UserStory to that Feature's UserStoryList
                // If not, create new Feature and add the UserStory to it

                Feature f = fList.Find(x => x.InternalId.Equals(userStory.FeatureInternalId));

                bool addFeatureToList = false;
                if (f == null)
                {

                    ObjID objIDFeature = new ObjID();
                    objIDFeature.SetIDs(
                        v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_FEATURE),
                        userStory.FeatureInternalId);
                    PropertyValues pvFeature =
                        v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDFeature, true).Properties;

                    f = GetFeatureData(v, userStory.FeatureInternalId, pvFeature);

                    addFeatureToList = true;
                }

                // TODO: Move this code to Feature creation
                // Order Dictionary for User Stories is Feature specific
                // First time a Feature is encountered we have to create a User Story order dictionary for it

                Dictionary<int, int> userStoryOrderDictionary;

                if (!config.SprintFeatureDictionary.ContainsKey(f.InternalId))
                {
                    userStoryOrderDictionary = new Dictionary<int, int>();
                    config.SprintFeatureDictionary.Add(f.InternalId, userStoryOrderDictionary);
                }
                else
                {
                    userStoryOrderDictionary = config.SprintFeatureDictionary[f.InternalId];
                }

                // Ordering of user stories
                if (!userStoryOrderDictionary.ContainsKey(userStory.InternalId))
                {
                    userStory.Order = 1;
                    if (f.UserStoryList.Count > 0)
                    {
                        userStory.Order = f.UserStoryList.Max(x => x.Order) + 1;
                    }
                    userStoryOrderDictionary.Add(userStory.InternalId, userStory.Order);
                }
                else
                {
                    userStory.Order = userStoryOrderDictionary[userStory.InternalId];
                }

                f.UserStoryList.Add(userStory);
                f.UserStoryList = f.UserStoryList.OrderBy(x => x.Order).ToList();

                // Add the new feature to the list
                if (addFeatureToList)
                {
                    // TODO: Move this code to Blog creation
                    // Order Dictionary for Features is Blog specific
                    // First time a Blog is encountered we have to create a User Story order dictionary for it

                    Dictionary<int, int> featureOrderDictionary;

                    if (!config.SprintDictionary.ContainsKey(sprintInternalId))
                    {
                        featureOrderDictionary = new Dictionary<int, int>();
                        config.SprintDictionary.Add(sprintInternalId, featureOrderDictionary);
                    }
                    else
                    {
                        featureOrderDictionary = config.SprintDictionary[sprintInternalId];
                    }

                    // Handle the feature order
                    if (!featureOrderDictionary.ContainsKey(f.InternalId))
                    {
                        f.Order = 1;
                        if (fList.Count > 0)
                        {
                            f.Order = fList.Max(x => x.Order) + 1;
                        }
                        featureOrderDictionary.Add(f.InternalId, f.Order);
                    }
                    else
                    {
                        f.Order = featureOrderDictionary[f.InternalId];
                    }

                    fList.Add(f);
                }

            }

            // Reorder the Feature list
            fList = fList.OrderBy(x => x.Order).ToList();

            return fList;
        }


        // specify either jsonString or the rest of the otptional params
        private ObjectSearchResults GetSubObjectsAsObjVers(Vault v, string jsonQuery = null, string ocAlias = null, string oocAlias = null, int ownerId = -1)
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

            // not deleted
            scs.Add(-1, MFSearchUtils.isDeletedSearchCondition(false));

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

        
        private ObjectSearchResults GetObjectsWithPropertyAsObjVers(Vault v, string jsonQuery)
        {
            string classAlias;
            string propertyAlias;
            int propertyValue;

            JObject jo = JObject.Parse(jsonQuery);
            classAlias = (string)jo.SelectToken("ObjectClass");
            propertyAlias = (string)jo.SelectToken("PropertyAlias");
            propertyValue = Convert.ToInt32(jo.SelectToken("PropertyValue"));

            SearchConditions scs = new SearchConditions();

            // not deleted
            scs.Add(-1, MFSearchUtils.isDeletedSearchCondition(false));

            int objClassId = v.ClassOperations.GetObjectClassIDByAlias(classAlias);
            scs.Add(-1, MFSearchUtils.isObjectClass(objClassId));

            scs.Add(-1, MFSearchUtils.hasProperty(
                v.PropertyDefOperations.GetPropertyDefIDByAlias(propertyAlias),
                propertyValue));

            ObjectSearchResults osr = MFSearchUtils.SearchForObjectsByConditions(v, scs);

            return osr;
        }

        private List<Task> GetTaskListForUserStory(Vault v, int userStoryId)
        {
            List<Task> tList = new List<Task>();

            ObjectSearchResults osrTasks = GetSubObjectsAsObjVers(v, null, config.ALIAS_MF_OC_TASK, config.ALIAS_MF_OC_USER_STORY, userStoryId);
            ObjVers objVersTasks = osrTasks.ObjectVersions.GetAsObjVers();

            PropertyValuesOfMultipleObjects pvomoTasks = v.ObjectPropertyOperations.GetPropertiesOfMultipleObjects(objVersTasks);


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

                try
                {
                    PropertyValues pvTaskPerson = v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDTask, true).Properties;
                    t.ResponsiblePerson = GetPersonData(v, taskResponsiblePersonId, pvTaskPerson);
                }
                catch (Exception e)
                {
                    // In case responsible person is not set
                }

                // TODO: Move this code to User Story creation
                // Order Dictionary for tasks is User Story specific
                // First time a User Story is encountered we have to create a Task order dictionary

                Dictionary<int, int> taskOrderDictionary;

                if (!config.BacklogUserStoryDictionary.ContainsKey(userStoryId))
                {
                    taskOrderDictionary = new Dictionary<int, int>();
                    config.BacklogUserStoryDictionary.Add(userStoryId, taskOrderDictionary);
                }
                else
                {
                    taskOrderDictionary = config.BacklogUserStoryDictionary[userStoryId];
                }

                // Handle the task order
                if (!taskOrderDictionary.ContainsKey(t.InternalId))
                {
                    t.Order = 1;
                    if (tList.Count > 0)
                    {
                        t.Order = tList.Max(x => x.Order) + 1;
                    }
                    taskOrderDictionary.Add(t.InternalId, t.Order);
                }
                else
                {
                    t.Order = taskOrderDictionary[t.InternalId];
                }

                tList.Add(t);

            }

            // Reorder
            tList = tList.OrderBy(x => x.Order).ToList();

            return tList;

        }


        private ObjectSearchResults GetUserStoriesForFeatureAsObjVers(Vault v, string jsonQuery, string ocAlias, string oocAlias)
        {
            int featureId;
            int sourceBacklogId;
            string oClassAlias = ocAlias;
            string ooClassAlias = oocAlias;

            JObject jo = JObject.Parse(jsonQuery);
            featureId = Convert.ToInt32(jo.SelectToken("FeatureId"));
            sourceBacklogId = Convert.ToInt32(jo.SelectToken("SourceBacklogId"));

            SearchConditions scs = new SearchConditions();

            // not deleted
            scs.Add(-1, MFSearchUtils.isDeletedSearchCondition(false));

            int objClassId = v.ClassOperations.GetObjectClassIDByAlias(oClassAlias);
            scs.Add(-1, MFSearchUtils.isObjectClass(objClassId));

            // owner property
            int objOwnerClassId = v.ClassOperations.GetObjectClassIDByAlias(ooClassAlias);
            ObjectClass oc = v.ClassOperations.GetObjectClass(objOwnerClassId);
            ObjType ot = v.ObjectTypeOperations.GetObjectType(oc.ObjectType);
            int ownerPropDef = ot.OwnerPropertyDef;
            scs.Add(-1, MFSearchUtils.isOwner(ownerPropDef, sourceBacklogId));

            // featureId
            scs.Add(-1, MFSearchUtils.hasProperty(
                v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FEATURE), featureId));

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

        /*****************************************************************************
         * Helper methods to create Data objects
        *****************************************************************************/


        private Backlog GetBacklogData(Vault v, int backlogId, PropertyValues pvs)
        {
            Backlog b = new Backlog();
            b.InternalId = backlogId;
            b.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
            b.Description = MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_DESCRIPTION));
            b.BacklogType = MFSearchUtils.getLookupPropertyAsInt(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_BACKLOG_TYPE));
            b.StartDate = Convert.ToDateTime(MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_START_DATE)));
            b.EndDate = Convert.ToDateTime(MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_END_DATE)));

            return b;
        }


        private Sprint GetSprintData(Vault v, int sprintId, PropertyValues pvs)
        {
            // Build a list with the objects as needed according to the DataModel
            Sprint sprint = new Sprint();
            sprint.InternalId = sprintId;
            sprint.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
            sprint.Description = MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_DESCRIPTION));
            sprint.StartDate = Convert.ToDateTime(MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_START_DATE)));
            sprint.EndDate = Convert.ToDateTime(MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_END_DATE)));

            return sprint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pvs"></param>
        /// <returns></returns>
        private UserStory GetUserStoryData(Vault v, int userStoryId, PropertyValues pvsUS)
        {
            UserStory u = new UserStory();
            u.InternalId = userStoryId;
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

            ObjID objIDUS = new ObjID();
            objIDUS.SetIDs(v.ObjectTypeOperations.GetObjectTypeIDByAlias(config.ALIAS_MF_OT_PERSON), responsiblePersonId);
            try
            {
                PropertyValues pvPersonUS = v.ObjectOperations.GetLatestObjectVersionAndProperties(objIDUS, true).Properties;
                u.ResponsiblePerson = GetPersonData(v, responsiblePersonId, pvPersonUS);
            }
            catch (Exception e)
            {
                // In case responsible person is not set
            }




            // Resolve Task list (Task is a subtype of User story)
            u.TaskList = GetTaskListForUserStory(v, u.InternalId);

            // Resolve Feature
            int featureId = MFSearchUtils.getLookupPropertyAsInt(
                pvsUS, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FEATURE));

            u.FeatureInternalId = featureId;

            return u;
        }


        private Person GetPersonData(Vault v, int personId, PropertyValues pvs)
        {
            Person p = new Person();
            p.InternalId = personId;
            p.PersonName = MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_PERSON_NAME));
            p.FirstName = MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FIRST_NAME));
            p.LastName = MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_LAST_NAME));

            return p;
        }


        private Feature GetFeatureData(Vault v, int featureId, PropertyValues pvs)
        {
            Feature f = new Feature();
            f.InternalId = featureId;
            f.FeatureId = MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_FEATURE_ID));
            f.NameOrTitle = MFSearchUtils.getPropertyDisplayValue(pvs, 0);
            f.Description = MFSearchUtils.getPropertyDisplayValue(
                pvs, v.PropertyDefOperations.GetPropertyDefIDByAlias(config.ALIAS_MF_PD_DESCRIPTION));
            f.UserStoryList = new List<UserStory>();

            return f;
        }


        // Unimplemented methods

        [VaultExtensionMethod("MoveUserStoryToBacklog", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string MoveUserStoryToBacklog(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [VaultExtensionMethod("MoveFeatureToSprint", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string MoveFeatureToSprint(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [VaultExtensionMethod("MoveFeatureToBacklog", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string MoveFeatureToBacklog(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [VaultExtensionMethod("MoveUserStoryToSprint", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string MoveUserStoryToSprint(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [VaultExtensionMethod("ReorderFeatureInBacklog", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string ReorderFeatureInBacklog(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [VaultExtensionMethod("ReorderFeatureInSprint", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string ReorderFeatureInSprint(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [VaultExtensionMethod("ReorderUserStoryInFeature", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string ReorderUserStoryInFeature(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [VaultExtensionMethod("ReorderTaskInUserStory", RequiredVaultAccess = MFVaultAccess.MFVaultAccessNone)]
        private string ReorderTaskInUserStory(EventHandlerEnvironment env)
        {
            var result = new { Result = "SUCCESS", Message = "" };
            if (RandomSuccess(0.5))
            {
                result = new { Result = "FAILED", Message = "Not implemented" };
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return jsonString;
        }



    }
}