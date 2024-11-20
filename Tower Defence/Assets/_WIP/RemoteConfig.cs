using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace _WIP
{
    public class RemoteConfig : MonoBehaviour
    {
        private static readonly BestagonVersion Version = new(2, 0, 1);

        private static async Task InitializeRemoteConfigAsync()
        {
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // options can be passed in the initializer, e.g if you want to set analytics-user-id or an environment-name use the lines from below:
            // var options = new InitializationOptions()
            //   .SetOption("com.unity.services.core.analytics-user-id", "my-user-id-1234")
            //   .SetOption("com.unity.services.core.environment-name", "production");
            // await UnityServices.InitializeAsync(options);

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private struct UserAttributes {
            // Optionally declare variables for any custom user attributes
        }

        private struct AppAttributes {
            
        }

        // RemoteConfigService.Instance.FetchConfigs() must be called with the attributes structs (empty or with custom attributes) to initiate the WebRequest.

        private async void Awake()
        {
            // initialize Unity's authentication and core services, however check for internet connection
            // in order to fail gracefully without throwing exception if connection does not exist
            if (Utilities.CheckForInternetConnection()) 
            {
                Debug.Log("Setting up connection");
                await InitializeRemoteConfigAsync(); 
            } 
            
            // Add a listener to apply settings when successfully retrieved:
            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            
            // Sets the environment to alpha, comment out to use production
            RemoteConfigService.Instance.SetEnvironmentID("9991561f-c455-4f3c-8da1-830509d49ad0");

            Debug.Log("Fetching Configs from " + RemoteConfigService.Instance.appConfig.environmentId);
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());

            // Example on how to fetch configuration settings if you have dedicated configType:
            //const string configType = "specialConfigType";
            // Fetch configs of that configType
            //RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());

            // All examples from above will also work asynchronously, returning Task<RuntimeConfig>
            //await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            //await RemoteConfigService.Instance.FetchConfigsAsync(configType, new userAttributes(), new appAttributes());
            
        }

        private static void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            // Conditionally update settings, depending on the response's origin:
            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
                    Debug.Log("No settings loaded this session; using default values.");
                    break;
                case ConfigOrigin.Cached:
                    Debug.Log("No settings loaded this session; using cached values from a previous session.");
                    break;
                case ConfigOrigin.Remote:
                    //Debug.Log("New settings loaded this session; update values accordingly.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static BestagonVersion GetVersion()
        {
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
            string forcedVersion = RemoteConfigService.Instance.appConfig.GetJson("ForcedVersion");
            var version = DeserializeJson<BestagonVersion>(forcedVersion);
            return version;
        }

        public static bool IsValidVersion()
        {
            return GetVersion().IsValidVersion(Version);
        }

        /// <summary>
        /// Does what it says on the tin. Deserializes Jason.
        /// </summary>
        /// <param name="result">The result of the api request</param>
        /// <typeparam name="T">Jason, but deserialized</typeparam>
        /// <returns></returns>
        private static T DeserializeJson<T>(string result)
        {
            var jsonSer = new DataContractJsonSerializer(typeof(T));
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            ms.Position = 0;
            return (T)jsonSer.ReadObject(ms);
        }
    }
    
    [DataContract]
    public class BestagonVersion
    {
        [DataMember]
        public float Major;
        [DataMember]
        public float Minor;
        [DataMember]
        public float Patch;

        public BestagonVersion(float major, float minor, float patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public bool IsValidVersion(BestagonVersion comparison)
        {
            Debug.Log("comp = " + comparison);
            Debug.Log(ToString());
            return !(Major > comparison.Major || Minor > comparison.Minor || Patch > comparison.Patch);
        }

        public override string ToString()
        {
            return Major + "." + Minor + "." + Patch;
        }
    }
}