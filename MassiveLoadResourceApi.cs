using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Exceptions;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Interfaces;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.OAuth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Gnoss.ApiWrapper
{
    /// <summary>
    /// Resource Api for Massive Data Load
    /// </summary>
    public class MassiveLoadResourceApi : ResourceApi
    {       
        /// <summary>
        /// Massive load identifier
        /// </summary>
        public Guid MassiveLoadIdentifier { get; set; }
        
        /// <summary>
        /// Massive data load name
        /// </summary>
        public string LoadName { get; set; }
        
        /// <summary>
        /// Directory path of the files
        /// </summary>    
        public string FilesDirectory { get; set; }
        
        /// <summary>
        /// Number of resources and files
        /// </summary>
        public Dictionary<string, OntologyCount> counter = new Dictionary<string, OntologyCount>();
        
        /// <summary>
        /// Virtual directory of data
        /// </summary>
        public string Uri { get; set; }
        
        public bool IsDebugMode { get; set; }
        
        ///<summary>
        /// Max num of resources per packages
        /// </summary>
        private int MaxResourcesPerPackage { get; set; }

        private StreamWriter streamData;
        private StreamWriter streamOntology;
        private StreamWriter streamSearch;

        private static readonly int DEBUG_PACKAGE_SIZE = 10;
        private bool onlyPrepareMassiveLoad;

        /// <summary>
        /// Constructor of <see cref="MassiveLoadResourceApi"/>
        /// </summary>
        /// <param name="communityShortName">Community short name which you want to use the API</param>
        /// <param name="oauth">OAuth information to sign the Api requests</param> 
        /// <param name="isDebugMode">Only for debugging</param>
        /// <param name="maxResourcesPerPackage">Num max of resources per package</param>
        /// <param name="developerEmail">(Optional) If you want to be informed of any incident that may happends during a large load of resources, an email will be sent to this email address</param>
        /// <param name="ontologyName">(Optional) Ontology name of the resources that you are going to query, upload or modify</param>
        public MassiveLoadResourceApi(OAuthInfo oauth, ILogHelper logHelper = null)
            : base(oauth, logHelper)
        {
            this.IsDebugMode = IsDebugMode;
        }

        /// <summary>
        /// Consturtor of <see cref="MassiveLoadResourceApi"/>
        /// </summary>
        /// <param name="configFilePath">Configuration file path, with a structure like http://api.gnoss.com/v3/exampleConfig.txt </param>
        public MassiveLoadResourceApi(string configFilePath) : base(configFilePath)
        {
        }

        /// <summary>
        /// Create a new massive data load
        /// </summary>
        /// <param name="pName">Massive data load name</param>
        /// <param name="pFilesDirectory">Path directory of the massive data load files</param>
        /// <param name="pUrl">Url where the file directory should be responding</param>
        /// <param name="pOnlyPrepareMassiveLoad">True if the massive data load should not be uploaded</param>
        /// <returns>Identifier of the load</returns>
        public Guid CreateMassiveDataLoad(string pName, string pFilesDirectory, string pUrl, bool pOnlyPrepareMassiveLoad = false)
        {
            try
            {
                if (pOnlyPrepareMassiveLoad && IsDebugMode)
                {
                    throw new Exception("MassiveDataLoad can not be prepared when debugMode is activated. Please turn off debugMode and try again.");
                }

                this.onlyPrepareMassiveLoad = pOnlyPrepareMassiveLoad;
                FilesDirectory = pFilesDirectory;
                Uri = pUrl;
                LoadName = pName;
                MassiveLoadIdentifier = Guid.NewGuid();

                if (!IsDebugMode && !onlyPrepareMassiveLoad)
                {
                    TestConnection();
                }

                CreateMassiveDataLoad();

                Log.Debug($"Massive data load create with the identifier {MassiveLoadIdentifier}");
                return MassiveLoadIdentifier;
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating the massive data load {MassiveLoadIdentifier}{ex.Message} {ex.StackTrace}");
                throw new GnossAPIException($"Error creating the massive data load {MassiveLoadIdentifier}{ex.Message} {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Test the connection with a nq file
        /// </summary>
        private void TestConnection()
        {
            try
            {
                if (!Directory.Exists(FilesDirectory))
                {
                    Directory.CreateDirectory(FilesDirectory);
                }

                string testFilePath = $"{FilesDirectory}/test.nq";
                string downloadedTestFilePath = $"{FilesDirectory}/downloadedtest.nq";

                //nq file creation
                File.WriteAllText(testFilePath, $"Testing file... {DateTime.Now.Ticks}");

                //summarie of nq file
                byte[] fileHash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(testFilePath));

                //download nq file
                WebClient client = new WebClient();
                client.DownloadFile($"{Uri}/test.nq", downloadedTestFilePath);

                //summarie of downloaded nq file
                byte[] downloadedFileHash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(downloadedTestFilePath));

                //if the summaries are different, something is wrong
                if (!fileHash.SequenceEqual(downloadedFileHash))
                {
                    Console.Error.Write("The connection to the server could not be established or nq files are not supported.");
                    throw new WebException("The connection to the server could not be established or nq files are not supported.");
                }

                //Test resource api
                string url = $"{ApiUrl}/resource/test-massive-load";

                MassiveDataLoadTestResource resource = new MassiveDataLoadTestResource()
                {
                    fileHash = fileHash,
                    url = $"{Uri}/test.nq"
                };
                WebRequestPostWithJsonObject(url, resource);
            }
            catch (Exception)
            {
                throw new WebException($"The connection to the server could not be established or nq files are not supported. {Uri}");
            }
        }

        /// <summary>
        /// Uploads an existing massive data load
        /// </summary>
        /// <param name="pMassiveLoadIdentifier">Massive data load identifier</param>
        public void UploadPreparedMassiveLoad(Guid pMassiveLoadIdentifier)
        {
            //MassiveLoadIdentifier = pMassiveLoadIdentifier;
            if (Directory.Exists(FilesDirectory))
            {
                int length = Directory.GetFiles(FilesDirectory, $"{OntologyNameWithoutExtension}_acid_{pMassiveLoadIdentifier}*.txt").Length;
                counter.Add(OntologyNameWithoutExtension, new OntologyCount(0, 0));

                if (length == 0)
                {
                    throw new Exception("The massive load doesn't exists.");
                }

                for (int i = 0; i < length; i++)
                {
                    SendPackage(pMassiveLoadIdentifier);
                    counter[OntologyNameWithoutExtension].FileCount++;
                }
            }
            else
            {
                throw new Exception("The massive load directory doesn't exists.");
            }
        }

        /// <summary>
        /// Create a new package massive data load
        /// </summary>
        /// <param name="resource">Interface of the Gnoss Methods</param>
        /// <returns>Identifier of the package</returns>
        public void AddResourceToPackage(IGnossOCBase resource)
        {
            try
            {
                if (!counter.Keys.Contains(OntologyNameWithoutExtension))
                {
                    counter.Add(OntologyNameWithoutExtension, new OntologyCount(0, 0));
                }

                List<string> ontologyTriples = resource.ToOntologyGnossTriples(this);
                List<string> searchTriples = resource.ToSearchGraphTriples(this);
                KeyValuePair<Guid, string> acidData = resource.ToAcidData(this);

                string pathOntology = $"{FilesDirectory}\\{OntologyNameWithoutExtension}_{MassiveLoadIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.nq";
                string pathSearch = $"{FilesDirectory}\\{OntologyNameWithoutExtension}_search_{MassiveLoadIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.nq";
                string pathAcid = $"{FilesDirectory}\\{OntologyNameWithoutExtension}_acid_{MassiveLoadIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.txt";

                if (streamData == null || streamOntology == null || streamSearch == null)
                {
                    streamData = new StreamWriter(new FileStream(pathAcid, FileMode.OpenOrCreate), Encoding.UTF8);
                    streamOntology = new StreamWriter(new FileStream(pathOntology, FileMode.OpenOrCreate), Encoding.UTF8);
                    streamSearch = new StreamWriter(new FileStream(pathSearch, FileMode.OpenOrCreate), Encoding.UTF8);
                }

                foreach (string triple in ontologyTriples)
                {
                    streamOntology.WriteLine(triple);
                }
                foreach (string triple in searchTriples)
                {
                    streamSearch.WriteLine(triple);
                }
                streamData.WriteLine($"{acidData.Key}|||{acidData.Value}");

                if (counter[OntologyNameWithoutExtension].ResourcesCount >= MaxResourcesPerPackage || (IsDebugMode && counter[OntologyNameWithoutExtension].ResourcesCount >= DEBUG_PACKAGE_SIZE))
                {
                    if (IsDebugMode)
                    {
                        this.Log.Warn("DebugMode On, use it only for testing purpose. Please turn DebugMode off as soon as posible.");
                    }
                    SendPackage();
                    
                    counter[OntologyNameWithoutExtension].ResourcesCount = 0;
                    counter[OntologyNameWithoutExtension].FileCount++;
                }
                else
                {
                    counter[OntologyNameWithoutExtension].ResourcesCount++;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating the package of massive data load {ex.Message} {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Close a massive data load
        /// </summary>
        /// <returns>True if the data load is closed</returns>
        public bool CloseMassiveDataLoad()
        {
            string url = $"{ApiUrl}/resource/close-massive-load";
            CloseMassiveDataLoadResource model = null;
            bool closed = false;
            try
            {
                SendPackage();
                InitializeResourceCounter();

                model = new CloseMassiveDataLoadResource()
                {
                    DataLoadIdentifier = MassiveLoadIdentifier
                };
                WebRequestPostWithJsonObject(url, model);
                Log.Debug("Data load is closed");
                closed = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error closing the data load {MassiveLoadIdentifier}. \r\n Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return closed;
        }

        /// <summary>
        /// Initialize the resource counter and the file ontology counter
        /// </summary>
        private void InitializeResourceCounter()
        {
            counter[OntologyNameWithoutExtension].ResourcesCount = 0;
            counter[OntologyNameWithoutExtension].FileCount = 0;
        }

        /// <summary>
        /// Create the massive data load
        /// <param name="organizationID">OPTIONAL: Organization identifier. If is not write, is '11111111-1111-1111-1111-111111111111'</param>
        /// <returns>True if the load is correctly created</returns>
        /// </summary>
        private bool CreateMassiveDataLoad()
        {
            bool created = false;
            MassiveDataLoadResource model = null;
            
            try
            {
                string url = $"{ApiUrl}/resource/create-massive-load";

                model = new MassiveDataLoadResource()
                {
                    load_id = MassiveLoadIdentifier,
                    name = LoadName,
                    community_name = CommunityShortName
                };
                WebRequestPostWithJsonObject(url, model);
                created = true;
                Log.Debug("Massive data load created");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating massive data load {MassiveLoadIdentifier}. \r\n Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return created;
        }

        private void SendPackage(Guid? pMassiveLoadIdentifier = null)
        {
            try
            {
                CloseStreams();

                if (onlyPrepareMassiveLoad)
                {
                    return;
                }

                Guid massiveLoadFilesIdentifier = MassiveLoadIdentifier;
                if (pMassiveLoadIdentifier.HasValue)
                {
                    massiveLoadFilesIdentifier = pMassiveLoadIdentifier.Value;
                }

                string uriOntology = $"{Uri}/{OntologyNameWithoutExtension}_{massiveLoadFilesIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.nq";
                string uriSearch = $"{Uri}/{OntologyNameWithoutExtension}_search_{massiveLoadFilesIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.nq";
                string uriAcid = $"{Uri}/{OntologyNameWithoutExtension}_acid_{massiveLoadFilesIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.txt";

                MassiveDataLoadPackageResource model = new MassiveDataLoadPackageResource();
                model.package_id = Guid.NewGuid();
                model.load_id = MassiveLoadIdentifier;

                //Si es modo debug queremos los bytes de los ficheros directamente 
                if (IsDebugMode)
                {
                    model.ontology_bytes = File.ReadAllBytes($"{FilesDirectory}\\{OntologyNameWithoutExtension}_{MassiveLoadIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.nq");
                    model.search_bytes = File.ReadAllBytes($"{FilesDirectory}\\{OntologyNameWithoutExtension}_search_{MassiveLoadIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.nq");
                    model.sql_bytes = File.ReadAllBytes($"{FilesDirectory}\\{OntologyNameWithoutExtension}_acid_{MassiveLoadIdentifier}_{counter[OntologyNameWithoutExtension].FileCount}.txt");
                }

                model.ontology_rute = uriOntology;
                model.search_rute = uriSearch;
                model.sql_rute = uriAcid;
                model.ontology = OntologyUrl;
                model.isLast = false;

                CreatePackageMassiveDataLoad(model);

                Log.Debug($"Package massive data load create with the identifier {MassiveLoadIdentifier}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating the package of massive data load {ex.Message}");
            }
        }

        private void CloseStreams()
        {
            if (streamData != null)
            {
                streamData.Flush();
                streamData.Close();
                streamData = null;
            }

            if (streamOntology != null)
            {
                streamOntology.Flush();
                streamOntology.Close();
                streamOntology = null;
            }

            if (streamSearch != null)
            {
                streamSearch.Flush();
                streamSearch.Close();
                streamSearch = null;
            }
        }

        /// <summary>
        /// Creates a new package in a massive data load
        /// </summary>
        /// <param name="model">Data model of the package massive data load</param>       
        /// <returns>True if the new massive data load package was created succesfully</returns>
        private bool CreatePackageMassiveDataLoad(MassiveDataLoadPackageResource model)
        {
            bool created = false;
            try
            {
                string url = $"{ApiUrl}/resource/create-massive-load-package";
                WebRequestPostWithJsonObject(url, model);
                created = true;
                Log.Debug("Massive data load package created");
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating massive data load package {model.package_id}. \r\n Json: {JsonConvert.SerializeObject(model)}", ex.Message);
                throw;
            }

            return created;
        }

        public EstadoCargaModel LoadState(Guid pLoadId)
        {
            EstadoCargaModel estadoCarga;
            try
            {
                string url = $"{ApiUrl}/resource/load-state";
                string response = WebRequestPostWithJsonObject(url, pLoadId);

                estadoCarga = JsonConvert.DeserializeObject<EstadoCargaModel>(response);
            }
            catch (Exception ex)
            {
                throw;
            }

            return estadoCarga;
        }
    }
}
