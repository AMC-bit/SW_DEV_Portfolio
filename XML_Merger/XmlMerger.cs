//
//
using System.Data;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XMLM
{
    public class XMLMerger
    {

        public XMLMerger(DataDB dataDB, XDocument doc1, int idlicenza, string DBSource)
        {
            _sharedConnection = dataDB;
            _doc1 = doc1;
            _idlicenza = idlicenza;
            _DBSource = DBSource;
        }

        private DataDB _sharedConnection;
        private XDocument _doc1;
        private string _DBSource;
        private int _idlicenza;
        private HashSet<(string Parent, string Child)> _containerMap = new HashSet<(string Parent, string Child)>();


        // Attribute mapping dict
        private Dictionary<string, string> attributeMap = new Dictionary<string, string>
    {
        { "DataLayers", "DataLayerName" },
        { "MenuItems", "ItemName" },
        { "UIExtensions", "AssemblyQualifiedName" },
        { "ESCodes", "ViewName" },
        { "EntityDataInfos", "Name" },
        { "WebServices", "Name" },
        { "PagedDatas", "Name" },
        { "LocaleMessages", "IdMessage" },
        { "ItemsTimeout", "ItemName" }
    };

        // Secondary dict
        private Dictionary<string, string> secondaryAttributeMap = new Dictionary<string, string>
    {
        { "PagedDatas", "RowQualifiedName" }
    };

        private Dictionary<string, List<string>> tipoNodoMap = new Dictionary<string, List<string>>
{
    { "DataLayers", new List<string> { "DataLayer", "DataLayerOverride" } },
    { "MenuItems", new List<string> { "Card", "CardMonoRow", "CardOverride", "CardAuto", "Report", "ReportOverride", "ReportAuto", "ActionView", "ActionViewOverride", "Wizard", "WizardOverride", "Dataview", "DataviewOverride", "ActiveCard", "ActiveCardOverride", "ExternalExe", "Folder", "Modal", "ModalOverride", "DataQuery" } },
    { "ESCodes", new List<string> { "ESCode", "ESCodeOverride" } },
    { "EntityDataInfos", new List<string> { "EntityDataInfo", "EntityDataInfoOverride", "EntityDataInfoSql" } },
    { "WebServices", new List<string> { "WebService" } },
    { "PagedDatas", new List<string> { "AddPagedData", "PagedDataOverride" } },
    { "LocaleMessages", new List<string> { "LocaleMessage" } },
    { "Packages", new List<string> { "Package" } },
    { "Areas", new List<string> { "Area" } },
    { "OData", new List<string> { "Feed", "FeedOverride" } },
    { "RestServices", new List<string> { "CardService", "ReportService", "EDIService", "ODataFeedService" } },
    { "WidgetItems", new List<string> { "Widget", "WidgetOverride" } },
    { "ItemsTimeout", new List<string> { "ItemName", "Seconds", "SDP", "Timeout" } },
    { "DashBoardParts", new List<string> { "DashBoardPart" } },
    { "Notifications", new List<string> { "Notification", "NotificationOverride" } },
    { "SqlCommands", new List<string> { "SqlCommand" } },
    { "Executables", new List<string> { "Executable", "ExecutableOverride" } }
};

        // incoming wins
        // .SubstituteNode
        // XdocumentHelper.AddNodeWithIndent

        public void ResetMyTempTables()
        {
            string sql = @"
        DROP TABLE IF EXISTS GlobalConfig;
        CREATE TABLE GlobalConfig (
            IdLicenza INT NOT NULL,
            DBSource VARCHAR(50),
            TipoNodo NVARCHAR(255) NOT NULL,
            Chiave NVARCHAR(255),
            ContentXML NVARCHAR(MAX) NOT NULL
        );
        
        --DROP TABLE IF EXISTS SourceConfig;
        --CREATE TABLE SourceConfig (
            --IdLicenza INT NOT NULL,
            --DBSource VARCHAR(50),
            --TipoNodo NVARCHAR(255) NOT NULL,
            --Chiave NVARCHAR(255),
            --ContentXML NVARCHAR(MAX) NOT NULL
        --);
        
        DROP TABLE IF EXISTS MergedGlobalConfig;
        CREATE TABLE MergedGlobalConfig (
            IdLicenza INT NOT NULL,
            DBSource VARCHAR(50),
            TipoNodo NVARCHAR(255) NOT NULL,
            Chiave NVARCHAR(255),
            ContentXML NVARCHAR(MAX) NOT NULL
        );
    ";

            _sharedConnection.ExecuteCommand(sql);


        }

        public XDocument MergeGlobalConfig(string inputTable)
        {
            {
                //ResetMyTempTables();

                // load docs
                XDocument sourceXml = _doc1;
                XDocument incomingXml = DeserializeSourceConfig();

                // start from root of incoming
                XElement incomingRoot = incomingXml.Root;
                XElement sourceRoot = sourceXml.Root;

                
                // equivalent : incomingRoot.Elements();

                // get all direct children of root

                //IEnumerable<XElement> incDescendants = incomingRoot.XPathSelectElements("*");
                IEnumerable<XElement> incDescendants = incomingRoot.Elements();

                // cycle through direct children of root
                foreach (var l1desc in incDescendants)
                {
                    // Datalayers, MenuItems etc.
                    Console.WriteLine($"L1 source Name: {l1desc.Name}");

                    // KEY
                    // "if there isn't any name of any direct child of SOURCE root that matches the name of any direct child of INCOMING root?

                    // aka, compare names of l1 elements of source and l1 elements of incoming (is there a child's name in incoming that doesn't match the same thing but in source?)


                    //if (!sourceRoot.XPathSelectElements("*").Any(p => p.Name == l1desc.Name))
                        if (!sourceRoot.Elements().Any(p => p.Name == l1desc.Name))
                    {
                        // two equivalent ish ways
                        // create element with the incoming child name's in question
                        XElement l1targetparent = new XElement(l1desc.Name);
                        //XDocumentHelper.AddNodeWithIndent(sourceRoot, l1targetparent);
                        // tack it onto the root of source
                        sourceRoot.Add(l1targetparent);
                    }



                    // if it doesn't, append it to the root of the other file ^ 

                    // then move on... 


                    // Child elements
                    foreach (var l2desc in l1desc.Elements())
                    {
                        Console.WriteLine($"L2 source Name: {l2desc.Name}");

                        string nodeAttribute = null;


                        // yoink attributes w/dict
                        foreach (var attr in l2desc.Attributes())

                        {
                            string dictvalue = null;

                            // find 1st l1desc.Name that matches a Key in AttributeMap
                            // return me the first kvp you find where you find a (the first) attrmap's key matching the name of l1 child
                            KeyValuePair<string, string> matchingParent = attributeMap.FirstOrDefault(x => x.Key == l1desc.Name);

                            if (matchingParent.Key == null)
                            {
                                KeyValuePair<string, string> matchingParent2 = secondaryAttributeMap.FirstOrDefault(x => x.Key == l1desc.Name);
                                dictvalue = matchingParent2.Value;
                            }
                            else
                            {
                                dictvalue = matchingParent.Value;
                            }

                            // name of current attribute
                            var attrname = attr.Name.LocalName;

                            // matching attribute is 1st found instance where within all attributes of grandchildren, their localname matches the value of matching parent (key of attributemap OR secattrmap matching name of parent container)
                            var matchingAttribute = l2desc.Attributes().FirstOrDefault(x => x.Name.LocalName == dictvalue);

                            //Console.WriteLine($"Attr val/Chiave: {matchingAttribute.Value}");
                            nodeAttribute = matchingAttribute.Value;

                        }

                        // Now we must check if l2desc.Name && nodeattribute.Value has an equivalent in the other file
                        // can be sourceroot.Elements().Any() also



                        // grab all children of children of root
                        // that match l2desc.Name
                        // and have an attribute matching nodeAttribute
                        // if CAN navigate to it (different than null)
                        // elfound = true
                        // otherwise (must be false)

                        // SelectElement = returns element, firstordefault returns element
                        // Any returns whether it exists or not within what I'm searching

                        //var elFound = sourceRoot.XPathSelectElement($"//{l2desc.Name}[@*='{nodeAttribute}']") != null;

                        // "Find if there's any element at this path" (anything under sourceroot at any depth that has grandchild's name and matchingattribute's value
                        var elFound = sourceRoot.XPathSelectElements($"//{l2desc.Name}[@*='{nodeAttribute}']").Any();

                        if (elFound)
                        {
                            Console.WriteLine($"MATCH FOUND {l2desc.Name} and {nodeAttribute}");

                            // the same matching attribute we found before, repeated, nodeAttribute is unique anyway

                            var eOld = sourceRoot.XPathSelectElement($"//{l2desc.Name}[@*='{nodeAttribute}']");
                            var eNew = l2desc;
                            var Parent = eOld.Parent;


                            XDocumentHelper.SubstituteNode(eOld, eNew, Parent);
                        }

//Quick ref:

//   . = current element
//   .. = parent element
//   / = document root
//   // = anywhere below
//   ./ = from current element (explicit, same as no prefix)


                            else
                            {
                                Console.WriteLine($"MATCH _NOT_ FOUND {l2desc.Name} and {nodeAttribute}");

                                var parent = sourceRoot.XPathSelectElement($"./{l1desc.Name}");
                                var nodetoadd = l2desc;

                                XDocumentHelper.AddNodeWithIndent(parent, nodetoadd);
                            }
                        

                        //Console.WriteLine($"Attr val/Chiave: {nodeAttribute}");

                    }
                }

                return sourceXml;
            }


        }




        private XDocument DeserializeSourceConfig()
        {

            // query merged table
            string query = @"
SELECT 
    IdLicenza,
    DBSource,
    TipoNodo,
    Chiave,
    ContentXML
FROM SourceConfig
WHERE IdLicenza = @IdLicenza
AND DBSource = @DBSource"
;

            // create list of obj to hold the data
            List<DBXMLRow> rows = new List<DBXMLRow>();

            DataParameters queryparameters = new DataParameters();
            queryparameters.Add(new DataParameter("IdLicenza", DataType.Int, _idlicenza));
            queryparameters.Add(new DataParameter("DBSource", DataType.String, _DBSource));

            DataTable dt = new DataTable();
            _sharedConnection.ExecuteRead(dt, query, queryparameters);

            foreach (DataRow row in dt.Rows)
            {
                rows.Add(new DBXMLRow
                {
                    IdLicenza = (int)row["IdLicenza"],
                    DBSource = GetStringValue(row["DBSource"]),
                    TipoNodo = (string)row["TipoNodo"],
                    Chiave = (string)row["Chiave"],
                    ContentXML = (string)row["ContentXML"],

                });
            }


            // ---- CONSTRUCT ---- //

            // Declare root, first "Root" tiponodo in db or default if not fount

            XElement root = new XElement("Root");
            Console.WriteLine($"- Created default root element");




            foreach (var row in rows)
            {

                if (root != null)
                {
                    // Check tipoNodoMap, find key where TipoNodo is in the list of values
                    var mapMatch = tipoNodoMap.FirstOrDefault(kvp => kvp.Value.Contains(row.TipoNodo));

                    if (mapMatch.Key != null)
                    {
                        string containerName = mapMatch.Key;
                        // XPath: search for direct child matching containerName
                        XElement container = root.XPathSelectElement($"./{containerName}");
                        // if you didn't find it, create it
                        if (container == null)
                        {
                            container = new XElement(containerName);
                            root.Add(container);
                            Console.WriteLine($"Created container {containerName}");
                        }
                        // Add content to the container
                        container.Add(XElement.Parse(row.ContentXML));
                        Console.WriteLine($"     |___ Added Chiave: {row.Chiave}, TipoNodo: {row.TipoNodo} to container: {containerName}");
                    }
                    else
                    {
                        // debug - no matching combo found
                        Console.WriteLine($"NO MATCHING COMBO for TipoNodo: {row.TipoNodo} (Chiave: {row.Chiave})");
                        Console.WriteLine($"Aborting...");
                        // return null
                        return null;
                    }
                }
                else
                {
                    // debug
                    Console.WriteLine($"MISSING ROOT for: (Chiave: {row.Chiave})");
                    Console.WriteLine($"Aborting...");
                    // return null
                    return null;
                }


            }

            // return doc if properly formed

            XDocument output = new XDocument(root);
            return output;
        }









        private string GetStringValue(object value)
        {
            return value == DBNull.Value ? null : (string)value;
        }


        private class DBXMLRow
        {
            public int IdLicenza { get; set; }
            public string DBSource { get; set; }
            public string TipoNodo { get; set; }
            public string Chiave { get; set; }
            public string ContentXML { get; set; }

        }


    }
}
